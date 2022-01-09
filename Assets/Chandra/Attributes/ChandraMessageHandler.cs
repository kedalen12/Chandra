using System;
using System.Linq;
using Chandra.Interfaces;

namespace Chandra.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class ChandraMessageHandler : Attribute
    {
        public IServer Server { get; private set; }
        public ushort PacketId { get; private set; }
        public ChandraMessageHandler(Type server, ushort packetId)
        {
            if (!server.GetInterfaces().Contains(typeof(IServer)))
            {
                throw new Exception($"Attribute must reference a type that implements {nameof(IServer)} interface.");
                return;
            }
            Server = (IServer) server;
            PacketId = packetId;
        }
    }
}