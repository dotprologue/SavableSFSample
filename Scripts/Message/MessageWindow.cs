using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using TMPro;

namespace SavableSFSample
{
    public class MessageWindow : IMessageWindow, IDisposable
    {
        private readonly TextMeshProUGUI messageText;
        private readonly float waitTime;

        private bool isDisposed = false;
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        public MessageWindow(Settings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));
            messageText = settings.MessageText ?? throw new ArgumentNullException(nameof(settings.MessageText));
            waitTime = settings.WaitTime;
        }

        public void ShowMessage(string message)
        {
            CheckDisposed();
            ResetTokenSource();
            messageText.text = message;
        }

        public void ShowVolatileMessage(string message)
        {
            CheckDisposed();
            ResetTokenSource();
            ShowMessage(message);
            UniTask.Void(async token =>
            {
                await UniTask.Delay(TimeSpan.FromSeconds(waitTime), cancellationToken: token);
                ClearMessage();
            }, cancellationToken: cancellationTokenSource.Token);
        }

        public void ClearMessage()
        {
            CheckDisposed();
            ResetTokenSource();
            messageText.text = string.Empty;
        }

        public void Dispose()
        {
            if (!isDisposed)
            {
                cancellationTokenSource?.Cancel();
                cancellationTokenSource?.Dispose();
                isDisposed = true;
            }
        }

        private void ResetTokenSource()
        {
            cancellationTokenSource?.Cancel();
            cancellationTokenSource?.Dispose();
            cancellationTokenSource = new CancellationTokenSource();
        }

        private void CheckDisposed()
        {
            if (isDisposed)
            {
                throw new ObjectDisposedException(nameof(MessageWindow));
            }
        }

        [Serializable]
        public class Settings
        {
            public TextMeshProUGUI MessageText;
            public float WaitTime;
        }
    }
}
