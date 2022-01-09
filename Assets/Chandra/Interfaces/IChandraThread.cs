using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Chandra.Interfaces
{
    public interface IChandraThread
    {
        public bool IsAlive { get; }
        int ServerCount();
        public void Start(IServer server);

        public void Stop();
        public void AddServer(IServer server);
        public void RemoveServer(IServer server);
        IServer GetBestServer();
        bool IsMyServer(int serverId);
        IServer GetServer(int serverId);
    }
}