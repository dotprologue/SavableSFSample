using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SavableSFSample
{
    [Serializable]
    public class LogRecord
    {
        [SerializeField]
        private int scenarioCount;
        [SerializeField]
        private LogText[] logTexts;
        [SerializeField]
        private History[] histories;

        public int ScenarioCount => scenarioCount;
        public IEnumerable<LogText> LogTexts => logTexts;
        public IEnumerable<History> Histories => histories;

        public LogRecord(int scenarioCount, IEnumerable<LogText> logTexts, IEnumerable<History> histories)
        {
            if (logTexts == null)
                throw new ArgumentNullException(nameof(logTexts));
            if (histories == null)
                throw new ArgumentException(nameof(histories));
            this.scenarioCount = scenarioCount;
            this.logTexts = logTexts.ToArray();
            this.histories = histories.ToArray();
        }

        [Serializable]
        public class History
        {
            [SerializeField]
            private string id;
            [SerializeField]
            private HistoryFragment[] historyFragments;

            public string Id => id;
            public IEnumerable<HistoryFragment> HistoryFragments => historyFragments;

            public History(string id, IEnumerable<HistoryFragment> historyFragments)
            {
                if (historyFragments == null)
                    throw new ArgumentNullException(nameof(historyFragments));
                this.id = id ?? throw new ArgumentNullException(nameof(id));
                this.historyFragments = historyFragments.ToArray();
            }
        }

        [Serializable]
        public class HistoryFragment
        {
            [SerializeField]
            private int scenarioIndex;
            [SerializeField]
            private string value;

            public int ScenarioIndex => scenarioIndex;
            public string Value => value;

            public HistoryFragment(int scenarioIndex, string value)
            {
                this.scenarioIndex = scenarioIndex;
                this.value = value ?? throw new ArgumentNullException(nameof(value));
            }
        }
    }
}
