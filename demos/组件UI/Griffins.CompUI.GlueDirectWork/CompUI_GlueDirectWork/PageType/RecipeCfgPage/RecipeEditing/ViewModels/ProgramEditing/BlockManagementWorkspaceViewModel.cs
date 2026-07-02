using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.ViewModels
{
    /// <summary>
    /// 区块管理工作区（新增轨迹区块/新增子区域区块/删除，并驱动左侧树刷新）。
    /// </summary>
    internal sealed class BlockManagementWorkspaceViewModel : ReactiveObject
    {
        /// <summary>当前所属模板。</summary>
        private readonly ProgramTemplateViewModel _template;
        /// <summary>区块变化后通知树刷新的回调。</summary>
        private readonly Action _notifyTreeChanged;
        /// <summary>当前选中区块。</summary>
        private ProgramBlockViewModel? _selectedBlock;
        /// <summary>是否存在勾选区块。</summary>
        private bool _hasCheckedBlocks;
        /// <summary>区块勾选状态订阅集合。</summary>
        private readonly List<IDisposable> _checkSubscriptions = new();

        /// <summary>初始化区块管理工作区。</summary>
        public BlockManagementWorkspaceViewModel(ProgramTemplateViewModel template, Action notifyTreeChanged)
        {
            _template = template;
            _notifyTreeChanged = notifyTreeChanged;

            Blocks = template.Blocks;
            UpdateOrder();
            AttachCheckSubscriptionsForAll();
            UpdateHasCheckedBlocks();

            AddTrajectoryBlockCommand = ReactiveCommand.Create(AddTrajectoryBlock);
            AddSubAreaBlockCommand = ReactiveCommand.Create(AddSubAreaBlock);
            DeleteCheckedBlocksCommand = ReactiveCommand.Create(DeleteCheckedBlocks, this.WhenAnyValue(x => x.HasCheckedBlocks));
            DeleteBlockItemCommand = ReactiveCommand.Create<ProgramBlockViewModel>(DeleteBlockItem);
        }

        /// <summary>标题文本。</summary>
        public string Title => "区块管理";

        /// <summary>区块列表数据源。</summary>
        public System.Collections.ObjectModel.ObservableCollection<ProgramBlockViewModel> Blocks { get; }

        public bool HasCheckedBlocks
        {
            get => _hasCheckedBlocks;
            private set => this.RaiseAndSetIfChanged(ref _hasCheckedBlocks, value);
        }

        /// <summary>当前选中的区块项。</summary>
        public ProgramBlockViewModel? SelectedBlock
        {
            get => _selectedBlock;
            set => this.RaiseAndSetIfChanged(ref _selectedBlock, value);
        }

        /// <summary>新增轨迹区块命令。</summary>
        public ReactiveCommand<Unit, Unit> AddTrajectoryBlockCommand { get; }

        /// <summary>新增子区域区块命令。</summary>
        public ReactiveCommand<Unit, Unit> AddSubAreaBlockCommand { get; }

        /// <summary>批量删除勾选区块命令。</summary>
        public ReactiveCommand<Unit, Unit> DeleteCheckedBlocksCommand { get; }

        /// <summary>删除单个区块命令。</summary>
        public ReactiveCommand<ProgramBlockViewModel, Unit> DeleteBlockItemCommand { get; }

        /// <summary>新增一个轨迹区块并刷新排序与树节点。</summary>
        private void AddTrajectoryBlock()
        {
            var index = Blocks.Count(b => b.BlockType == ProgramBlockType.Trajectory) + 1;
            Blocks.Add(new ProgramBlockViewModel(ProgramBlockType.Trajectory, $"轨迹区块{index}"));
            UpdateOrder();
            AttachCheckSubscriptionsForAll();
            UpdateHasCheckedBlocks();
            _notifyTreeChanged();
        }

        /// <summary>新增一个子区域区块并刷新排序与树节点。</summary>
        private void AddSubAreaBlock()
        {
            var index = Blocks.Count(b => b.BlockType == ProgramBlockType.SubArea) + 1;
            Blocks.Add(new ProgramBlockViewModel(ProgramBlockType.SubArea, $"子区域区块{index}"));
            UpdateOrder();
            AttachCheckSubscriptionsForAll();
            UpdateHasCheckedBlocks();
            _notifyTreeChanged();
        }

        /// <summary>删除所有勾选区块并维护选中项。</summary>
        private void DeleteCheckedBlocks()
        {
            var targets = Blocks.Where(b => b.IsChecked).ToList();
            if (targets.Count == 0)
                return;

            foreach (var b in targets)
                Blocks.Remove(b);

            SelectedBlock = Blocks.FirstOrDefault();
            UpdateOrder();
            AttachCheckSubscriptionsForAll();
            UpdateHasCheckedBlocks();
            _notifyTreeChanged();
        }

        /// <summary>删除指定区块并维护选中项。</summary>
        private void DeleteBlockItem(ProgramBlockViewModel block)
        {
            if (Blocks.Contains(block))
                Blocks.Remove(block);
            if (ReferenceEquals(SelectedBlock, block))
                SelectedBlock = Blocks.FirstOrDefault();
            UpdateOrder();
            AttachCheckSubscriptionsForAll();
            UpdateHasCheckedBlocks();
            _notifyTreeChanged();
        }

        /// <summary>重排区块序号。</summary>
        private void UpdateOrder()
        {
            for (int i = 0; i < Blocks.Count; i++)
                Blocks[i].Order = i + 1;
        }

        /// <summary>重新订阅所有区块的勾选变化。</summary>
        private void AttachCheckSubscriptionsForAll()
        {
            foreach (var d in _checkSubscriptions)
                d.Dispose();
            _checkSubscriptions.Clear();

            foreach (var b in Blocks)
            {
                _checkSubscriptions.Add(
                    b.WhenAnyValue(x => x.IsChecked)
                        .Subscribe(_ => UpdateHasCheckedBlocks()));
            }
        }

        /// <summary>更新是否存在勾选区块。</summary>
        private void UpdateHasCheckedBlocks()
        {
            HasCheckedBlocks = Blocks.Any(b => b.IsChecked);
        }
    }
}

