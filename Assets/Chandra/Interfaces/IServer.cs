
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Chandra.Interfaces
{
    public interface IServer
    {
        public Transform GetUnityObject { get; }
        /// <summary>
        /// A unique identifier for the server... this is unique only within the server tag
        /// </summary>
        public int ServerIdentifier { get; }
        /// <summary>
        /// The port this server instance is currently listening on, Cannot be changed at runtime
        /// </summary>
       public ushort Port { get; }
        /// <summary>
        /// The maximum amount of players this server will accepted, if the Orchestrator is set to Automatic or has been configured to
        /// start new servers if max capacity is reached. This will invoke the Orchestrator.CreateServerCopy automatically when the maximum is reached
        /// </summary>
        public int MaxPlayers { get; }
       public string Tag { get; }

       /// <summary>
       /// Configures the new server this step is required before calling Server.BeginListening()
       /// </summary>
       /// <param name="serverPort">The port where the server will listen to</param>
       /// <param name="maxPlayerCount">The maximum number of connections before the server stops accepting</param>
       /// <param name="serverTag">Comma separated, this can be used to quickly find multiple instances of the same server type, or just as syntactic sugar for the orchestrator</param>
       /// <param name="ticksPerSecond">The amount of times the server should update default is 10</param>
       public void Configure(ushort serverPort, int maxPlayerCount, string serverTag = "", byte ticksPerSecond = 10);

       /// <summary>
       /// Starts the server, and begins accepting connections
       /// </summary>
       /// <param name="dieTime">None by default, if set to a positive number, the server will die if no connections are present and no connections are waiting after the specified time in ms </param>
       public void BeginListening(uint dieTime = uint.MinValue);


       /// <summary>
       /// This event fires once per server instance.
       /// </summary>
       public event Action OnServerHasStarted;
       /// <summary>
       /// This event fires every time a client connects to this instance and passes the IClient object
       /// </summary>
       public event Action<object> OnClientConnected;
       /// <summary>
       /// This event fires every time a client disconnects from this instance and passes the IClientDisconnected object
       /// </summary>
       public event Action<object> OnClientDisconnected;
       /// <summary>
       /// Exposes an Asynchronous task that will be awaited right before killing the server
       /// </summary>
       public event Func<Task> OnBeforeServerKill;

       /// <summary>
       /// Can only be called from a chandra thread, it exposes 2 abstract methods that can be overriden in your implementation (OnTick(); OnTickMainThread();)
       /// </summary>
       void Tick();

       /// <summary>
       /// Can only be called from an IChandraThread implementation
       /// </summary>
       /// <param name="expectedThreadId">The expected thread id</param>
       /// <param name="serverId">The newly set server id <see cref="ServerIdentifier"/></param>
       /// <param name="onServerDestroyCallback">This is the callback the server will invoke right before it's process is killed</param>
       void SetThread(int expectedThreadId, int serverId, Action<int> onServerDestroyCallback);

       /// <summary>
       /// The current amount of connections that this server is currently handling <see cref="MaxPlayers"/>
       /// </summary>
       int CurrentConnections { get; }
       
       /// <summary>
       /// Returns true if <see cref="CurrentConnections"/> is bigger or equal to <see cref="MaxPlayers"/>
       /// </summary>
       bool IsFull { get; }
    }
}