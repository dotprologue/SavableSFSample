using System.Collections.Generic;

namespace SavableSFSample
{
    public interface ILogTextGetter
    {
        IEnumerable<LogText> LogTexts { get; }
    }
}