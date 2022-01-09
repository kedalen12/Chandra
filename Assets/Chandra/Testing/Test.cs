using Chandra.Attributes;
using Chandra.Interfaces;
namespace Chandra.Testing
{
    public class Test
    {
        [ChandraMessageHandler(typeof(Test), 1)]
        public void HandleMessage()
        {
            
        }
    }
}