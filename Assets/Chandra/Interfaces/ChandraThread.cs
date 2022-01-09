using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Chandra.Interfaces
{
    public class ChandraThread : IChandraThread
    {
        private Thread _thread;
        private readonly List<IServer> _servers = new List<IServer>();
        private readonly float _tickRateInMs;
        private readonly int _ticksPerSecond;

        public ChandraThread(byte ticksPerSecond)
        {
            try
            {
                ticksPerSecond = ticksPerSecond == 0 ? (byte)1 : ticksPerSecond;
                _ticksPerSecond = ticksPerSecond;
                _tickRateInMs = 1000f / ticksPerSecond;
            }
            catch (Exception ex)
            {
                //This is a theoretically impossible Catch to be executed if the code is not modified externally since a Byte cannot be bigger than 255 or smaller than 0 
                //and if the byte is 0 we will just set it to 1
                Debug.LogError(ex.Message);
            }
        }

        public int ServerCount()
        {
            lock (_servers)
            {
                return _servers.Count;
            }
        }
        public void Start(IServer server)
        {
            _thread = new Thread(Tick);
            AddServer(server);
            _thread.Start();
        }

        private void Tick()
        {
            while (true)
            {
                Debug.Log("Thread Running...");
                try
                {
                    Thread.Sleep(TimeSpan.FromMilliseconds(_tickRateInMs));
                    lock (_servers)
                    {
                        if (_servers.Count <= 0)
                        {
                            //Kill the thread if no servers are currently attached to it...
                            _thread.Abort();
                        }
                        for (var i = _servers.Count - 1; i >= 0; i--)
                        {
                            //Call the TICK method on each server... this is like the Update Function but it is guaranteed to run (ticksPerSecond) times each second
                            _servers[i].Tick();
                        }
                    }
                }
                catch
                {
                    //Ignore
                }

            }
            // ReSharper disable once FunctionNeverReturns
        }

        public bool IsAlive => _thread.IsAlive;
        public void AddServer(IServer server)
        {
            lock (_servers)
            {
                if (_servers.Contains(server))
                {
                    return;
                }
                server.SetThread(_thread.ManagedThreadId, _servers.Count, OnServerDestroy);
                _servers.Add(server);
            }
        }

        private void OnServerDestroy(int serverId)
        {
            var server = GetServer(serverId);
            if (server is null)
            {
                return;
            }
            RemoveServer(server);
        }
        public void RemoveServer(IServer server)
        {
            lock (_servers)
            {
                if (_servers.Contains(server))
                {
                    _servers.Remove(server);
                }
            }
        }

        public IServer GetBestServer()
        {
            lock (_servers)
            {
                var cBestServer = -1;
                if (_servers.Count == 1)
                {
                    if (!_servers[0].IsFull)
                        return _servers[0];
                    var tServer = Orchestrator.CloneServer(_servers[0]);
                    return tServer;
                }
                for (var i = 0; i < _servers.Count; i++)
                {
                    if (_servers[i].IsFull)
                    {
                        continue;
                    }

                    var calculatedServerIndex = cBestServer == -1 ? 0 : cBestServer;
                    if (_servers[i].CurrentConnections > _servers[calculatedServerIndex].CurrentConnections)
                    {
                        continue;
                    }

                    if (_servers[calculatedServerIndex].IsFull)
                    {
                        cBestServer = i;
                        continue;
                    }

                    if (_servers[calculatedServerIndex].CurrentConnections > _servers[i].CurrentConnections)
                    {
                        cBestServer = i;
                    }
                    
                }


                // No server is available
                if (cBestServer == -1)
                {
                    var tServer = Orchestrator.CloneServer(_servers[0]);
                    return tServer;
                }
                return _servers[cBestServer];
            }
        }

        public bool IsMyServer(int serverId)
        {
            lock (_servers)
            {
                return _servers.Any(r => r.ServerIdentifier == serverId);
            }
        }

        public IServer GetServer(int serverId)
        {
            lock (_servers)
            {
                return IsMyServer(serverId) ? _servers.Single(r => r.ServerIdentifier == serverId) : null;
            }
        }

        public void Stop()
        {
            _thread.Abort();
        }
    }
}