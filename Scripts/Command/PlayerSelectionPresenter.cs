using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using ScenarioFlow;
using ScenarioFlow.Scripts.SFText;
using ScenarioFlow.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace SavableSFSample
{
	public class PlayerSelectionPresenter : IReflectable
    {
		private static readonly string SelectionLogTitle = "- Selections -";
		private static readonly string AnswerLogTitle = "- Answer -";

		private readonly ILabelOpener labelOpener;
        private readonly ILogStacker logStacker;
        private readonly GameObject branchSelectionButtonParent;
        private readonly BranchSelectionButton branchSelectionButtonPrefab;

        public PlayerSelectionPresenter(ILabelOpener labelOpener, ILogStacker logStacker, Settings settings)
        {
            this.labelOpener = labelOpener ?? throw new ArgumentNullException(nameof(labelOpener));
            this.logStacker = logStacker ?? throw new ArgumentNullException(nameof(logStacker));
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));
            branchSelectionButtonParent = settings.BranchSelectionButtonParent ?? throw new ArgumentNullException(nameof(settings.BranchSelectionButtonParent));
            branchSelectionButtonPrefab = settings.BranchSelectionButtonPrefab ?? throw new ArgumentNullException(nameof(settings.BranchSelectionButtonPrefab));
        }

        [CommandMethod("present 1 selection async")]
        [Category("Branch")]
        [Description("Present 1 selection to the player.")]
        [Snippet("Selection: {${1:selection}}.")]
        public async UniTask PresentOneSelectionAsync(string selection, CancellationToken cancellationToken)
        {
            logStacker.StackLog("<Select>", selection, false);
            var selectionButton = GameObject.Instantiate(branchSelectionButtonPrefab, branchSelectionButtonParent.transform);
            selectionButton.Text.text = selection;
            try
            {
                await selectionButton.OnClickAsAsyncEnumerable(cancellationToken).FirstOrDefaultAsync(cancellationToken: cancellationToken);
            }
            finally
            {
                GameObject.Destroy(selectionButton.gameObject);
            }
        }

        [CommandMethod("branch on 2 selections async")]
        [Category("Branch")]
        [Description("Present 2 selections to the player, and branch based on the selection.")]
        [Snippet("Selection 1: {${1:Selection1}} ")]
        [Snippet("- Jump to {${2:Label1}}")]
        [Snippet("Selection 2: {${3:Selection2}}")]
        [Snippet("- Jump to {${4:Label2}}")]
        public UniTask BranchBasedOnTwoSelectionsAsync(string selection1, string label1, string selection2, string label2, CancellationToken cancellationToken)
        {
            return BranchBasedOnSelectionsAsync(new KeyValuePair<string, string>[]
            {
            new KeyValuePair<string, string>(selection1, label1),
            new KeyValuePair<string, string>(selection2, label2),
            }, cancellationToken);
        }

        [CommandMethod("branch on 3 selections async")]
        [Category("Branch")]
        [Description("Present 3 selections to the player, and branch based on the selection.")]
        [Snippet("Selection 1: {${1:Selection1}} ")]
        [Snippet("- Jump to {${2:Label1}}")]
        [Snippet("Selection 2: {${3:Selection2}}")]
        [Snippet("- Jump to {${4:Label2}}")]
        [Snippet("Selection 3: {${3:Selection3}}")]
        [Snippet("- Jump to {${4:Label3}}")]
        public UniTask BranchBasedOnThreeSelectionsAsync(string selection1, string label1, string selection2, string label2, string selection3, string label3, CancellationToken cancellationToken)
        {
            return BranchBasedOnSelectionsAsync(new KeyValuePair<string, string>[]
            {
            new KeyValuePair<string, string>(selection1, label1),
            new KeyValuePair<string, string>(selection2, label2),
            new KeyValuePair<string, string>(selection3, label3),
            }, cancellationToken);
        }

        private async UniTask BranchBasedOnSelectionsAsync(IEnumerable<KeyValuePair<string, string>> selectionLabelPairs, CancellationToken cancellationToken)
        {
            var selectionsText = selectionLabelPairs.Select(pair => pair.Key).Aggregate((a, b) => $"{a}<br>{b}");
            logStacker.StackLog($"<i>{SelectionLogTitle}</i>", $"<i>{string.Join("<br>", selectionLabelPairs.Select(p => p.Key))}<i>", true);
			var (_, label) = await UniTask.WhenAny(
                selectionLabelPairs.Select(async pair =>
                {
                    var selectionButton = GameObject.Instantiate(branchSelectionButtonPrefab, branchSelectionButtonParent.transform);
                    selectionButton.Text.text = pair.Key;
                    try
                    {
                        await selectionButton.OnClickAsAsyncEnumerable(cancellationToken).FirstOrDefaultAsync(cancellationToken: cancellationToken);
                        logStacker.StackLog($"<i>{AnswerLogTitle}</i>", pair.Key, false);
                        return pair.Value;
                    }
                    finally
                    {
                        GameObject.Destroy(selectionButton.gameObject);
                    }
                }));
            labelOpener.OpenLabel(label);
        }

        [Serializable]
        public class Settings
        {
            public GameObject BranchSelectionButtonParent;
            public BranchSelectionButton BranchSelectionButtonPrefab;
        }
    }
}
