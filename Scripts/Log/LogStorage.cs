using System;
using System.Collections.Generic;
using System.Linq;

namespace SavableSFSample
{
    public class LogStorage : ILogStacker, IScenarioRecordStacker, ILogTextGetter, ILogCapturer, ILogRecoverer
    {
        private List<LogText> logTexts = new List<LogText>();
        private Dictionary<string, SortedDictionary<int, string>> scenarioHistoryDictionary = new Dictionary<string, SortedDictionary<int, string>>();

        public int ScenarioCount { get; private set; } = 0;
        public IEnumerable<LogText> LogTexts => logTexts;

        public void StackLog(string characterName, string dialogueLine, bool doesAttachScenario)
        {
            if (ScenarioCount == 0)
                throw new InvalidOperationException("No scenario to be attached exists.");
            logTexts.Add(new LogText(ScenarioCount - 1, doesAttachScenario, characterName, dialogueLine));
        }

        public void StackScenario(ScenarioRecord scenarioRecord)
        {
            if (scenarioRecord == null)
                throw new ArgumentNullException(nameof(ScenarioRecord));
            if (ScenarioCount == (logTexts.Count > 0 ? logTexts.Max(t => t.ScenarioIndex) + 1 : 0))
            {
                ScenarioCount++;
            }
            //Stack log
            var index = ScenarioCount - 1;
            foreach (var fragment in scenarioRecord.Fragments)
            {
                if (!scenarioHistoryDictionary.ContainsKey(fragment.Id))
                {
                    scenarioHistoryDictionary.Add(fragment.Id, new SortedDictionary<int, string>());
                }
                var fragmentDictionary = scenarioHistoryDictionary[fragment.Id];
                //Remove the previous fragment with the same scenario count
                if (fragmentDictionary.Count > 0 && fragmentDictionary.ContainsKey(index))
                {
                    fragmentDictionary.Remove(index);
                }
                //If the fragment value is the same as the last one, then don't stack it
                if (fragmentDictionary.Count == 0 || fragmentDictionary.Values.Last() != fragment.Value)
                {
                    fragmentDictionary.Add(index, fragment.Value);
                }
            }
        }

        public LogRecord Capture(int scenarioIndex)
        {
            if (scenarioIndex < 0 || ScenarioCount <= scenarioIndex)
                throw new ArgumentOutOfRangeException(nameof(scenarioIndex));

            var histories = scenarioHistoryDictionary.Select(pair =>
            {
                var historyFragments = pair.Value.Where(p => p.Key <= scenarioIndex).Select(p => new LogRecord.HistoryFragment(p.Key, p.Value));
                return new LogRecord.History(pair.Key, historyFragments);
            });
            return new LogRecord(scenarioIndex + 1, logTexts.Where(x => x.ScenarioIndex < scenarioIndex), histories);
        }

        public ScenarioRecord Recover(LogRecord logRecord)
        {
            //Recovery
            ScenarioCount = logRecord.ScenarioCount;
            logTexts = logRecord.LogTexts.ToList();
            scenarioHistoryDictionary = logRecord.Histories.ToDictionary(history => history.Id, history => new SortedDictionary<int, string>(history.HistoryFragments.ToDictionary(fragment => fragment.ScenarioIndex, fragment => fragment.Value)));
            //Build the last scenario record
            return new ScenarioRecord(scenarioHistoryDictionary.Select(pair => new ScenarioRecord.Fragment(pair.Key, pair.Value.Values.Last())));
        }
    }
}
