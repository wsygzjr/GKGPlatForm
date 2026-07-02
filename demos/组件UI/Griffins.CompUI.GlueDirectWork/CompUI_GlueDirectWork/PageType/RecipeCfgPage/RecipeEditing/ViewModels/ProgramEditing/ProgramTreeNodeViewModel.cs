using ReactiveUI;
using System.Collections.ObjectModel;

namespace Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.ViewModels
{
    /// <summary>程序编辑左侧树节点（ViewModel）。</summary>
    internal sealed class ProgramTreeNodeViewModel : ReactiveObject
    {
        /// <summary>节点显示名称。</summary>
        private string _name;
        /// <summary>节点展开状态。</summary>
        private bool _isExpanded;

        /// <summary>初始化树节点。</summary>
        public ProgramTreeNodeViewModel(
            string name,
            string nodeType,
            object? payload = null,
            ProgramTreeNodeViewModel[]? children = null,
            NodeKey? key = null)
        {
            _name = name;
            NodeType = nodeType;
            Payload = payload;
            Key = key ?? new NodeKey(nodeType);
            Children = new ObservableCollection<ProgramTreeNodeViewModel>(children ?? []);
        }

        /// <summary>树内强类型键（用于刷新后恢复展开与选中状态）。</summary>
        public NodeKey Key { get; }

        /// <summary>树节点显示名。</summary>
        public string Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }

        /// <summary>节点类型标识（用于右侧工作区模板选择）。</summary>
        public string NodeType { get; }

        /// <summary>节点关联的数据对象（例如模板/区块）。</summary>
        public object? Payload { get; }

        /// <summary>TreeView 展开状态（用于刷新树时保持展开/折叠）。</summary>
        public bool IsExpanded
        {
            get => _isExpanded;
            set => this.RaiseAndSetIfChanged(ref _isExpanded, value);
        }

        /// <summary>子节点列表。</summary>
        public ObservableCollection<ProgramTreeNodeViewModel> Children { get; }
    }
}

