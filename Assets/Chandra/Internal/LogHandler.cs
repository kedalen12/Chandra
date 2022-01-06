using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Chandra.Internal
{
    public class LogHandler : ILogHandler
    {
        private const string ServerLogString = "[SERVER] ";
        private const string ClientLogString = "[CLIENT] ";
        public void Log(string toLog)
        {
            Debug.Log(string.Concat(ServerLogString, toLog));
        }

        public void Log<T>(T toLog)
        {
            if (typeof(T).GetInterface("IEnumerable") != null)
            {
                var enumerable = toLog as IEnumerable;
                foreach (var element in enumerable)
                {
                    Log(element);
                }
                return;
            }

            Debug.Log(string.Concat(ServerLogString, toLog.ToString()));
        }
        
    }
}