using ReactiveUI;
using System;
using System.Collections.ObjectModel;

namespace Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.ViewModels
{
    internal sealed class ProgramTemplateViewModel : ReactiveObject
    {
        /// <summary>模板名称。</summary>
        private string _name;
        /// <summary>模板显示序号。</summary>
        private int _order;
        /// <summary>模板勾选状态。</summary>
        private bool _isChecked;
        /// <summary>参考点 X 坐标。</summary>
        private decimal _refBaseX;
        /// <summary>参考点 Y 坐标。</summary>
        private decimal _refBaseY;
        /// <summary>Mark 点 X 坐标。</summary>
        private decimal _markX;
        /// <summary>Mark 点 Y 坐标。</summary>
        private decimal _markY;
        /// <summary>Mark 参考模板 ID。</summary>
        private string? _markTemplateId;

        /// <summary>初始化模板模型并生成唯一 ID。</summary>
        public ProgramTemplateViewModel(string name)
        {
            Id = Guid.NewGuid().ToString("N");
            _name = name;
            Blocks = new ObservableCollection<ProgramBlockViewModel>();
        }

        /// <summary>模板唯一标识。</summary>
        public string Id { get; }

        /// <summary>模板名称。</summary>
        public string Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }

        /// <summary>显示用序号（由拥有者在列表变化时维护）。</summary>
        public int Order
        {
            get => _order;
            set => this.RaiseAndSetIfChanged(ref _order, value);
        }

        /// <summary>表格勾选框（用于批量操作预留）。</summary>
        public bool IsChecked
        {
            get => _isChecked;
            set => this.RaiseAndSetIfChanged(ref _isChecked, value);
        }

        /// <summary>模板下的区块集合。</summary>
        public ObservableCollection<ProgramBlockViewModel> Blocks { get; }

        /// <summary>参考点 X 坐标。</summary>
        public decimal RefBaseX
        {
            get => _refBaseX;
            set => this.RaiseAndSetIfChanged(ref _refBaseX, value);
        }

        /// <summary>参考点 Y 坐标。</summary>
        public decimal RefBaseY
        {
            get => _refBaseY;
            set => this.RaiseAndSetIfChanged(ref _refBaseY, value);
        }

        /// <summary>Mark 点 X 坐标。</summary>
        public decimal MarkX
        {
            get => _markX;
            set => this.RaiseAndSetIfChanged(ref _markX, value);
        }

        /// <summary>Mark 点 Y 坐标。</summary>
        public decimal MarkY
        {
            get => _markY;
            set => this.RaiseAndSetIfChanged(ref _markY, value);
        }

        /// <summary>Mark 参考模板 ID。</summary>
        public string? MarkTemplateId
        {
            get => _markTemplateId;
            set => this.RaiseAndSetIfChanged(ref _markTemplateId, value);
        }
    }
}

