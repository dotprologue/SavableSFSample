using System;

namespace SavableSFSample
{
    public class RecoveryExecutor : IRecoveryExecutor
    {
        private readonly IRecoveryRequester recoveryRequester;
        private readonly ISceneReloader sceneReloader;

        public RecoveryExecutor(IRecoveryRequester recoveryRequester, ISceneReloader sceneReloader)
        {
            this.recoveryRequester = recoveryRequester ?? throw new ArgumentNullException(nameof(recoveryRequester));
            this.sceneReloader = sceneReloader ?? throw new ArgumentNullException(nameof(sceneReloader));
        }

        public void ExecuteRecovery(LogRecord logRecord)
        {
            recoveryRequester.RequestRecovery(logRecord);
            sceneReloader.ReloadScene();
        }
    }
}
