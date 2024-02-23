using System.Collections.Generic;

namespace SavableSFSample
{
    public interface ILogObjectBuilder
    {
        IEnumerable<LogObject> BuildLogObjects();
    }
}