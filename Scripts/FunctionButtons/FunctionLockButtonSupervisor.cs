using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Cysharp.Threading.Tasks.Triggers;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace SavableSFSample
{
    public class FunctionLockButtonSupervisor : ICancellationInitializable
    {
        private readonly IMessageWindow messageWindow;
        private readonly GameObject headerObject;
        private readonly float velocity;
        private readonly float upperLimit;
        private readonly float lowerLimit;
        private readonly Button lockButton;
        private readonly Sprite lockButtonSpriteOnEnable;
        private readonly Sprite lockButtonSpriteOnDisable;

        private bool isPointerEnter = false;

        private static bool isLocked = false;

        public FunctionLockButtonSupervisor(IMessageWindow messageWindow, Settings settings)
        {
            this.messageWindow = messageWindow ?? throw new ArgumentNullException(nameof(messageWindow));
            headerObject = settings.HeaderObject ?? throw new ArgumentNullException(nameof(settings.HeaderObject));
            upperLimit = settings.UpperLimit;
            lowerLimit = settings.LowerLimit;
            velocity = (upperLimit - lowerLimit) / Mathf.Max(settings.HeaderMoveDuration, 0.1f);
            lockButton = settings.LockButton ?? throw new ArgumentNullException(nameof(settings.LockButton));
            lockButtonSpriteOnEnable = settings.LockButtonSpriteOnEnable ?? throw new ArgumentNullException(nameof(settings.LockButtonSpriteOnEnable));
            lockButtonSpriteOnDisable = settings.LockButtonSpriteOnDisable ?? throw new ArgumentNullException(nameof(settings.LockButtonSpriteOnDisable));
        }

        public void InitializeWithCancellation(CancellationToken cancellationToken)
        {
            lockButton.GetComponent<Image>().sprite = isLocked ? lockButtonSpriteOnEnable : lockButtonSpriteOnDisable;
            if (isLocked)
            {
                headerObject.transform.localPosition = new Vector3(headerObject.transform.localPosition.x, lowerLimit, headerObject.transform.localPosition.z);
            }
            lockButton
                .GetAsyncPointerEnterTrigger()
                .AsUniTaskAsyncEnumerable()
                .ForEachAsync(_ =>
                {
                    if (!isLocked)
                    {
                        lockButton.GetComponent<Image>().sprite = lockButtonSpriteOnEnable;
                    }
                    messageWindow.ShowMessage(Message.Lock);
                }, cancellationToken: cancellationToken);
            lockButton
                .GetAsyncPointerExitTrigger()
                .AsUniTaskAsyncEnumerable()
                .ForEachAsync(_ =>
                {
                    if (!isLocked)
                    {
                        lockButton.GetComponent<Image>().sprite = lockButtonSpriteOnDisable;
                    }
                    messageWindow.ClearMessage();
                }, cancellationToken: cancellationToken);
            lockButton
                .OnClickAsAsyncEnumerable(cancellationToken: cancellationToken)
                .ForEachAsync(_ => isLocked = !isLocked, cancellationToken: cancellationToken);
            headerObject
                .GetAsyncPointerEnterTrigger()
                .AsUniTaskAsyncEnumerable()
                .ForEachAsync(_ => isPointerEnter = true, cancellationToken: cancellationToken);
            headerObject
                .GetAsyncPointerExitTrigger()
                .AsUniTaskAsyncEnumerable()
                .ForEachAsync(_ => isPointerEnter = false, cancellationToken: cancellationToken);
            UniTaskAsyncEnumerable.EveryUpdate()
                .ForEachAsync(_ =>
                {
                    var distance = velocity * Time.deltaTime;
                    if (isPointerEnter || isLocked)
                    {
                        if (headerObject.transform.localPosition.y - distance > lowerLimit)
                        {
                            headerObject.transform.localPosition += Vector3.down * distance;
                        }
                        else
                        {
                            headerObject.transform.localPosition = new Vector3(0.0f, lowerLimit, 0.0f);
                        }
                    }
                    else
                    {
                        if (headerObject.transform.localPosition.y + distance < upperLimit)
                        {
                            headerObject.transform.localPosition += Vector3.up * distance;
                        }
                        else
                        {
                            headerObject.transform.localPosition = new Vector3(0.0f, upperLimit, 0.0f);
                        }
                    }
                }, cancellationToken: cancellationToken);
        }

        [Serializable]
        public class Settings
        {
            public GameObject HeaderObject;
            public float HeaderMoveDuration;
            public float UpperLimit;
            public float LowerLimit;
            public Button LockButton;
            public Sprite LockButtonSpriteOnEnable;
            public Sprite LockButtonSpriteOnDisable;
        }
    }
}