using System;
using Chandra.Internal;
using Chandra.Internal.Enums;
using Chandra.Internal.Interfaces;
using UnityEngine;

namespace Chandra.API
{
    public class GameServer : ServerBase
    {
        private void Awake()
        {
            Create(1999, 500);
            OnServerConnect();
            OnClientDisconnected(null);
        }
        
    }
}
