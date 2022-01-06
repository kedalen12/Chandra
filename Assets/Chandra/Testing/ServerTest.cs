using Chandra.Internal.Attributes;
using Chandra.Internal.Enums;
using UnityEngine;

namespace Chandra.Testing
{
    public class ServerTest
    {
        [MessageHandler(2, MessageSender.CLIENT)]
        public static void ClientSendReceiver()
        {
            Debug.Log("Message From Client");
        }

        public static void SendMessageToClient()
        {
            
        }
    }
}