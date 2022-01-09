using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Chandra
{
    public class GameServer : BaseServer
    {
        private void Awake()
        {
            Configure(400, 100);
            OnBeforeServerKill += BeforeServerKill;
            clients.Add("");
        }

        public int serverId;
        private async Task BeforeServerKill()
        {
            await Task.Delay(1000);
        }

        protected override void OnThreadStart()
        {
            serverId = ServerIdentifier;
        }

        protected override void OnTick()
        {
        }

        protected override void OnTickMainThread()
        {
        }
    }
}