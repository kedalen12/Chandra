using System;

namespace Chandra.Internal.Interfaces
{
    public interface IClientInformation
    { 
        string UserName { get; }
        string ConnectionAddress { get; }
        Guid InternalGuid { get; }
        double ConnectedOn { get; }
    }
}