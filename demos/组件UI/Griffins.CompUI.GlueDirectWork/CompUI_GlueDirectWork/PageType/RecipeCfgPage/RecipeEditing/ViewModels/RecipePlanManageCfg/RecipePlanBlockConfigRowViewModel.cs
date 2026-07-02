using ReactiveUI;

namespace Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.ViewModels
{
    /// <summary>方案配置 → 区块配置：列表行 ViewModel（来源于程序编辑区块）。</summary>
    internal sealed class RecipePlanBlockConfigRowViewModel : ReactiveObject
    {
        private int _serialNumber;
        private bool _blockEnabledInPlan;
        private bool _canMoveUp;
        private bool _canMoveDown;

        public RecipePlanBlockConfigRowViewModel(ProgramBlockViewModel sourceBlock, string blockName, bool enabled)
        {
            SourceBlock = sourceBlock;
            BlockName = blockName;
            _blockEnabledInPlan = enabled;
        }

        /// <summary>来源区块（程序编辑的区块对象）。</summary>
        public ProgramBlockViewModel SourceBlock { get; }

        /// <summary>区块名称（展示用）。</summary>
        public string BlockName { get; }

        /// <summary>显示序号。</summary>
        public int SerialNumber
        {
            get => _serialNumber;
            set => this.RaiseAndSetIfChanged(ref _serialNumber, value);
        }

        /// <summary>方案内是否启用该区块。</summary>
        public bool BlockEnabledInPlan
        {
            get => _blockEnabledInPlan;
            set
            {
                if (!this.RaiseAndSetIfChanged(ref _blockEnabledInPlan, value))
                    return;
                this.RaisePropertyChanged(nameof(EnableStatusText));
            }
        }

        /// <summary>启用状态文本。</summary>
        public string EnableStatusText => _blockEnabledInPlan ? "启用" : "禁用";

        /// <summary>当前行是否可上移。</summary>
        public bool CanMoveUp
        {
            get => _canMoveUp;
            set => this.RaiseAndSetIfChanged(ref _canMoveUp, value);
        }

        /// <summary>当前行是否可下移。</summary>
        public bool CanMoveDown
        {
            get => _canMoveDown;
            set => this.RaiseAndSetIfChanged(ref _canMoveDown, value);
        }
    }
}

