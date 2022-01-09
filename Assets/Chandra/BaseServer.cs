using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Chandra.Interfaces;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Chandra
{
    public abstract class BaseServer : MonoBehaviour, IServer
    {

        private Action<int> OnServerDestroyCallBack;
        public Transform GetUnityObject => this.transform;
        public int ServerIdentifier { get; private set; }
        public ushort Port { get; private set; }

        private int _threadId = -1;
        public int MaxPlayers { get; private set; }

        public string Tag { get; private set; }


        public void SetThread(int expectedThreadId, int serverId, Action<int> onServerDestroyCallBack)
        {
            if (_threadId != -1)
            {
                return;
            }

            var stackTrace = new StackTrace();
            var callingType = stackTrace.GetFrame(1).GetMethod().ReflectedType;
            if (callingType is null)
            {
                return;
            }
            if (!callingType.GetInterfaces().Contains(typeof(IChandraThread)))
            {
                return;
            }

            _threadId = expectedThreadId;
            ServerIdentifier = serverId;
            OnServerDestroyCallBack = onServerDestroyCallBack;
            OnThreadStart();
        }

        /// <summary>
        /// This is called right after the Server has been successfully attached to a ChandraThread <see cref="IChandraThread"/>
        /// </summary>
        protected virtual void OnThreadStart() { }
        public int CurrentConnections => clients.Count;
        public bool IsFull => clients.Count >= MaxPlayers;

        public List<string> clients = new List<string>();
        public event Action OnServerHasStarted;
        public event Action<object> OnClientConnected;
        public event Action<object> OnClientDisconnected;
        public event Func<Task> OnBeforeServerKill;
        

        public void Configure(ushort serverPort, int maxPlayerCount, string serverTag = "default",
            byte ticksPerSecond = 10)
        {
            Port = serverPort;
            MaxPlayers = maxPlayerCount;
            if (string.IsNullOrWhiteSpace(serverTag))
                serverTag = "default";
            Tag = serverTag;
            OnBeforeFirstTick();
            Orchestrator.AddToOrCreateThread(this, ticksPerSecond);
        }

        public void BeginListening(uint dieTime = uint.MinValue)
        {
            if (dieTime != uint.MinValue)
            {
            }

            OnServerHasStarted?.Invoke();
        }

        private bool _destroyOnNextTick = false;
        private bool _isAwaitingDestruction = false;

        
        /// <summary>
        /// This verifies if the server KILL conditions are met... If they are calls <see cref="OnBeforeServerKill"/> and awaits its execution.
        /// once the <see cref="OnBeforeServerKill"/> Task has completed the server will be destroyed... If the task faults or throws the server will be terminated
        /// instantly...
        /// </summary>
        private async Task KillServerAsync()
        {
            if (_destroyOnNextTick || _isAwaitingDestruction)
            {
                return;
            }

            if (clients.Count > 0) return;
            if (OnBeforeServerKill is null)
            {
                _destroyOnNextTick = true;
                return;
            }
            _isAwaitingDestruction = true;
            OnServerDestroyCallBack.Invoke(ServerIdentifier);
            try
            {
                await OnBeforeServerKill.Invoke();
                _destroyOnNextTick = true;
            }
            catch (Exception)
            {
                _destroyOnNextTick = true;
            }

        }

        
        public void Tick()
        {
            if (_threadId != Thread.CurrentThread.ManagedThreadId)
            {
                Debug.LogWarning($"Attempted to Tick from wrong Thread [Expected Thread -> {_threadId}...Ticking Thread -> {Thread.CurrentThread.ManagedThreadId}]");
                return;
            }

            _isTick = true;
            OnTick();
            OnTickMainThread();
            KillServerAsync().Wait();
        }

        /// <summary>
        /// This method is called before the Thread that will be running this server is Started And/Or the server is attached to it. Useful for making last second custom checks and for logging
        /// Kinda like the default Start Method in Unity
        /// </summary>
        protected virtual void OnBeforeFirstTick()
        {
        }

        /// <summary>
        /// Use this to interact with Chandra Orchestrator or CallBacks, if a UnityObject is referenced from here the intended behaviour might not be possible
        /// </summary>
        protected abstract void OnTick();

        /// <summary>
        /// Use this to interact with UnityMainThread. eg: Updating a Transform, Updating UI elements etc...
        /// This method will execute on the [NEXT UNITY-OBJECT.UPDATE CALL] so the behaviour inside this call might not be instantaneous (sub ms delay is expected).
        /// </summary>
        protected abstract void OnTickMainThread();

        private bool _isTick;
        private readonly Queue<Action> _mainThreadActions = new Queue<Action>();

        private void Update()
        {

            if (_destroyOnNextTick)
            {
                Destroy(this);
                return;
            }
            OnTickMainThread();
            if (_mainThreadActions.Count > 0)
            {
                _mainThreadActions.Dequeue().Invoke();
            }
        }
    }
}