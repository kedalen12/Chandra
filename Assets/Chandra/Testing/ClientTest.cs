using Chandra.Internal.Attributes;
using Chandra.Internal.Enums;
using UnityEngine;

namespace Chandra.Testing
{
    public class ClientTest
    {
        [MessageHandler(1, MessageSender.SERVER)]
        public static void StaticHandle()
        {
            Debug.Log("Message from Server");
        }
    }
}