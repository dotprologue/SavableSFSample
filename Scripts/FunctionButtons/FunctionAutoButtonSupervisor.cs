using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Cysharp.Threading.Tasks.Triggers;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace SavableSFSample
{
    public class FunctionAutoButtonSupervisor : ICancellationInitializable
    {
        private readonly IAutoSwitch autoSwitch;
        private readonly IMessageWindow messageWindow;
        private readonly Button autoButton;
        private readonly Image buttonImage;
        private readonly Sprite spriteOnEnable;
        private readonly Sprite spriteOnDisable;

        public FunctionAutoButtonSupervisor(IAutoSwitch autoSwitch, IMessageWindow messageWindow, Settings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));
            this.autoSwitch = autoSwitch ?? throw new ArgumentNullException(nameof(autoSwitch));
            this.messageWindow = messageWindow ?? throw new ArgumentNullException(nameof(messageWindow));
            autoButton = settings.AutoButton ?? throw new ArgumentNullException(nameof(settings));
            buttonImage = autoButton.GetComponent<Image>();
            spriteOnEnable = settings.SpriteOnEnable ?? throw new ArgumentNullException(nameof(settings));
            spriteOnDisable = settings.SpriteOnDisable ?? throw new ArgumentNullException(nameof(settings));
        }

        public void InitializeWithCancellation(CancellationToken cancellationToken)
        {
            //When the auto button is cliced, turn on/off the auto mode
            autoButton.OnClickAsAsyncEnumerable(cancellationToken: cancellationToken)
                .ForEachAsync(_ => autoSwitch.SwitchAuto(!autoSwitch.IsAutoActive), cancellationToken: cancellationToken);
            //When the auto mode is switched, notify it
            UniTaskAsyncEnumerable.EveryValueChanged(autoSwitch, x => x.IsAutoActive)
                .ForEachAsync(isActive =>
                {
                    buttonImage.sprite = isActive ? spriteOnEnable : spriteOnDisable;
                }, cancellationToken: cancellationToken);
            //On pointer enter
            autoButton.GetAsyncPointerEnterTrigger()
                .AsUniTaskAsyncEnumerable()
                .ForEachAsync(_ =>
                {
                    if (autoButton.interactable && !autoSwitch.IsAutoActive)
                    {
                        buttonImage.sprite = spriteOnEnable;
                    }
                    messageWindow.ShowMessage(Message.Auto);
                }, cancellationToken: cancellationToken);
            //On pointer exit
            autoButton.GetAsyncPointerExitTrigger()
                .AsUniTaskAsyncEnumerable()
                .ForEachAsync(_ =>
                {
                    if (!autoSwitch.IsAutoActive)
                    {
                        buttonImage.sprite = spriteOnDisable;
                    }
                    messageWindow.ClearMessage();
                }, cancellationToken: cancellationToken);

        }

        [Serializable]
        public class Settings
        {
            public Button AutoButton;
            public Sprite SpriteOnEnable;
            public Sprite SpriteOnDisable;
        }
    }
}