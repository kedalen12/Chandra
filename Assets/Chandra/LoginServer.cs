using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Chandra
{
    public class LoginServer : BaseServer
    {
        private void Awake()
        {
            Configure(400, 100, "login", 1);
        }
        
        public int serverID;
        protected override void OnThreadStart()
        {
            serverID = ServerIdentifier;
        }

        protected override void OnTick()
        {
        }

        protected override void OnTickMainThread()
        {
        }
    }
}