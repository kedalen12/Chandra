using System.Collections;
using System.Collections.Generic;

namespace Chandra.Internal
{
    public interface ILogHandler
    {
        void Log(string toLog);
        void Log<T>(T toLog);
    }
} 