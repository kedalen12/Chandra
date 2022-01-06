using System;
using Chandra.Internal.Enums;

namespace Chandra.Internal.Attributes
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public class MessageHandler : Attribute
    {
        protected ushort Method { get; }
        protected MessageSender Sender { get; }
        public MessageHandler(ushort method, MessageSender sender)
        {
            Method = method;
            Sender = sender;
        }
    }
}