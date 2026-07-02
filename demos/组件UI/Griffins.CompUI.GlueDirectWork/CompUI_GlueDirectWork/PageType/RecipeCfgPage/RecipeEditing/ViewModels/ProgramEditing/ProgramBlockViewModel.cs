using ReactiveUI;
using System;
using System.Collections.ObjectModel;

namespace Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.ViewModels
{
    internal enum ProgramBlockType
    {
        Trajectory,
        SubArea
    }

    internal sealed class ProgramBlockViewModel : ReactiveObject
    {
        /// <summary>区块名称。</summary>
        private string _name;
        /// <summary>区块显示序号。</summary>
        private int _order;
        /// <summary>区块勾选状态。</summary>
        private bool _isChecked;

        /// <summary>初始化区块模型并生成唯一 ID。</summary>
        public ProgramBlockViewModel(ProgramBlockType blockType, string name)
        {
            Id = Guid.NewGuid().ToString("N");
            BlockType = blockType;
            _name = name;
        }

        /// <summary>区块唯一标识。</summary>
        public string Id { get; }

        /// <summary>区块类型。</summary>
        public ProgramBlockType BlockType { get; }

        /// <summary>区块名称。</summary>
        public string Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }

        /// <summary>区块显示序号。</summary>
        public int Order
        {
            get => _order;
            set => this.RaiseAndSetIfChanged(ref _order, value);
        }

        /// <summary>区块勾选状态。</summary>
        public bool IsChecked
        {
            get => _isChecked;
            set => this.RaiseAndSetIfChanged(ref _isChecked, value);
        }

        /// <summary>区块类型显示文本。</summary>
        public string BlockTypeDisplayName => BlockType switch
        {
            ProgramBlockType.Trajectory => "轨迹区块",
            ProgramBlockType.SubArea => "子区域区块",
            _ => BlockType.ToString()
        };

        /// <summary>子区域内嵌套「模板管理」专用模板列表（与整板外层 <see cref="ProgramEditingViewModel.Templates"/> 无关）。</summary>
        public ObservableCollection<ProgramTemplateViewModel> NestedTemplates { get; } = new();
    }
}

