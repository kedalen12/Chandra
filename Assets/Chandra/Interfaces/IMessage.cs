using System.Collections.Generic;

namespace Chandra.Interfaces
{
    public interface IMessage
    {
        /// <summary>
        /// Writes an element / array of elements into a chandra message
        /// </summary>
        void Write<TElement>(TElement toWrite);
        /// <summary>
        /// Reads an element of type <typeparamref name="TElement"/> from the message
        /// </summary>
        void Read<TElement>(TElement toRead);
        /// <summary>
        /// Some times for complex objects Chandra is not able to deserialize them by default, for that you can write a custom
        /// <see cref="IChandraSerializationEngine{T}"/> to teach chandra how to Serialize and Deserialize the item
        /// </summary>
        /// <param name="toWrite"></param>
        /// <param name="serializationEngine"></param>
        /// <typeparam name="TElement"></typeparam>
        void Write<TElement>(TElement toWrite, IChandraSerializationEngine<TElement> serializationEngine);
    }

    public interface IChandraSerializationEngine<T>
    {
        public abstract T Deserialize();
        public abstract void Serialize(T element);
    }


    public class ChandraMessage : IMessage
    {
        private static Queue<IMessage> MessageQueue = new Queue<IMessage>();
        public void Write<TElement>(TElement toWrite)
        {
            throw new System.NotImplementedException();
        }

        public void Read<TElement>(TElement toRead)
        {
            throw new System.NotImplementedException();
        }

        public void Write<TElement>(TElement toWrite, IChandraSerializationEngine<TElement> serializationEngine)
        {
            throw new System.NotImplementedException();
        }
    }
}