using ScenarioFlow;
using ScenarioFlow.Scripts.SFText;
using ScenarioFlow.Tasks;
using System;

namespace SavableSFSample
{
    public class BranchMaker : IReflectable
    {
        private readonly ILabelOpener labelOpener;

        public BranchMaker(ILabelOpener labelOpener)
        {
            this.labelOpener = labelOpener ?? throw new ArgumentNullException(nameof(labelOpener));
        }

        [CommandMethod("jump to label")]
        [Category("Branch")]
        [Description("Jump to the specified label.")]
        [Snippet("Jump to {${1:label}}.")]
        public void JumpLabel(string label)
        {
            labelOpener.OpenLabel(label);
        }
    }
}
