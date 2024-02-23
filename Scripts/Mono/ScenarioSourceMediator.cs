using System;
using UnityEngine;

namespace SavableSFSample
{
    public class ScenarioSourceMediator : MonoBehaviour, IScenarioSourceProvider, INewScenarioRequester, IRecoveryRequester
    {
        private string scenarioScriptPath = string.Empty;
        private LogRecord recoveryLogRecord = null;

        public bool TryAcceptNewScenario(out string scenarioScriptPath)
        {
            scenarioScriptPath = this.scenarioScriptPath;
            this.scenarioScriptPath = null;
            return !string.IsNullOrWhiteSpace(scenarioScriptPath);
        }

        public bool TryAcceptRecovery(out LogRecord recoveryLogRecord)
        {
            recoveryLogRecord = this.recoveryLogRecord;
            this.recoveryLogRecord = null;
            return recoveryLogRecord != null;
        }

        public void RequestNewScenario(string scenarioScriptPath)
        {
            if (string.IsNullOrWhiteSpace(scenarioScriptPath))
                throw new ArgumentException(nameof(scenarioScriptPath));
            this.scenarioScriptPath = scenarioScriptPath;
        }

        public void RequestRecovery(LogRecord recoveryLogRecord)
        {
            this.recoveryLogRecord = recoveryLogRecord ?? throw new ArgumentNullException(nameof(recoveryLogRecord));
        }
    }
}
