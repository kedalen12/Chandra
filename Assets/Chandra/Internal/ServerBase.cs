using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using Chandra.Internal.Enums;
using Chandra.Internal.Interfaces;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Chandra.Internal
{
    public abstract class ServerBase : MonoBehaviour
    {
        private ILogHandler _logger;
        private ushort _maxPlayers;
        private static TcpListener _connectionListener;
        private bool _isRunning;
        protected void Create(ushort port, ushort maxPlayers)
        {
            _connectionListener = new TcpListener(IPAddress.Any, port);
            _maxPlayers = maxPlayers;
            _logger = new LogHandler();
            _logger.Log("Server Created");
            _logger.Log(string.Concat("Max Players", maxPlayers));
            _logger.Log(string.Concat("Listening On ", port));
        }

        protected virtual void StartServer()
        {
            if(_connectionListener is null) 
                return;
            if(_isRunning)
                return;
            _connectionListener.BeginAcceptTcpClient(OnTCPConnectionEstablished, null);
        }

        private void OnTCPConnectionEstablished(IAsyncResult result)
        {
            var client = _connectionListener.EndAcceptTcpClient(result);
            _connectionListener.BeginAcceptTcpClient(OnTCPConnectionEstablished, null);
        }
        
        public void SetMultiThreadingProfile(MultiThreadingProfile threadingProfile)
        {
            
        }
        internal virtual void OnServerConnect()
        {
            
        }

        internal virtual void OnClientConnect(IClientInformation client)
        {
            if (client is null)
            {
                _logger.Log($"Client was null");
                return;
            }
            _logger.Log($"{client} has connected to the server");
        }

        internal virtual void OnClientDisconnected(IClientInformation client)
        {
            if (client is null)
            {
                _logger.Log($"Client was null");
                return;
            }
            
            _logger.Log($"Client {client.UserName} {client.ConnectionAddress} has disconnected...");
        }
        private void FixedUpdate()
        {
            Tick();
        }
        private static void Tick()
        {
            
        }
    }
}