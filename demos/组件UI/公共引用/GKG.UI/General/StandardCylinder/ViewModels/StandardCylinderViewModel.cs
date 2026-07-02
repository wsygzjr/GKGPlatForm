using Avalonia.Controls;
using ReactiveUI;
using System;
using System.Collections.Generic;

namespace GKG.UI.General
{
    /// <summary>
    /// 标准气缸视图模型
    /// </summary>
    public class StandardCylinderViewModel : ReactiveObject
    {
        private Control? _viewReference;

        /// <summary>
        /// 模式选择下拉框
        /// </summary>
        public ComboxViewModel ModeSelectorViewModel { get; }

        /// <summary>
        /// 气缸页面类型行
        /// </summary>
        public CylinderPageTypeRowViewModel PageTypeRowViewModel { get; }

        /// <summary>
        /// 单控单限位
        /// </summary>
        public SingleControlSingleLimitViewModel SingleControlSingleLimitViewModel { get; }

        /// <summary>
        /// 单控双限位
        /// </summary>
        public SingleControlDoubleLimitViewModel SingleControlDoubleLimitViewModel { get; }

        /// <summary>
        /// 双控双限位
        /// </summary>
        public DoubleControlDoubleLimitViewModel DoubleControlDoubleLimitViewModel { get; }

        /// <summary>
        /// 双控单限位
        /// </summary>
        public DoubleControlSingleLimitViewModel DoubleControlSingleLimitViewModel { get; }

        /// <summary>
        /// 值变更事件
        /// </summary>
        public event EventHandler? AfterModified;

        private StandardCylinderMode _mode;

        /// <summary>
        /// 当前模式
        /// </summary>
        public StandardCylinderMode Mode
        {
            get => _mode;
            set
            {
                if (_mode != value)
                {
                    this.RaiseAndSetIfChanged(ref _mode, value);
                    UpdateContent();
                }
            }
        }

        private object? _currentContentViewModel;

        /// <summary>
        /// 当前内容视图模型
        /// </summary>
        public object? CurrentContentViewModel
        {
            get => _currentContentViewModel;
            private set => this.RaiseAndSetIfChanged(ref _currentContentViewModel, value);
        }

        public StandardCylinderViewModel()
        {
            ModeSelectorViewModel = new ComboxViewModel
            {
                DisplayMemberPath = nameof(ComBoxItem.DisplayName),
                PlaceholderText = "\u8bf7\u9009\u62e9\u6c14\u7f38\u7c7b\u578b",
            };

            PageTypeRowViewModel = new CylinderPageTypeRowViewModel
            {
                Label = "\u9875\u9762\u7c7b\u578b",
            };

            var modeItems = new List<ComBoxItem>
            {
                new() { Value = StandardCylinderMode.SingleControlSingleLimit, DisplayName = "\u5355\u63a7\u5355\u9650\u4f4d" },
                new() { Value = StandardCylinderMode.SingleControlDoubleLimit, DisplayName = "\u5355\u63a7\u53cc\u9650\u4f4d" },
                new() { Value = StandardCylinderMode.DoubleControlSingleLimit, DisplayName = "\u53cc\u63a7\u5355\u9650\u4f4d" },
                new() { Value = StandardCylinderMode.DoubleControlDoubleLimit, DisplayName = "\u53cc\u63a7\u53cc\u9650\u4f4d" },
            };
            ModeSelectorViewModel.ItemsSource = modeItems;

            SingleControlSingleLimitViewModel = new();
            SingleControlDoubleLimitViewModel = new();
            DoubleControlDoubleLimitViewModel = new();
            DoubleControlSingleLimitViewModel = new();

            Mode = StandardCylinderMode.SingleControlSingleLimit;
            ModeSelectorViewModel.SelectedItem = modeItems[0];

            subscribeValueChanges();
            UpdateContent();
        }

        private void subscribeValueChanges()
        {
            ModeSelectorViewModel.ValueChanged += onValueChanged;
            PageTypeRowViewModel.AfterModified += onValueChanged;
            SingleControlSingleLimitViewModel.AfterModified += onValueChanged;
            SingleControlDoubleLimitViewModel.AfterModified += onValueChanged;
            DoubleControlSingleLimitViewModel.AfterModified += onValueChanged;
            DoubleControlDoubleLimitViewModel.AfterModified += onValueChanged;
        }

        private void onValueChanged(object? sender, EventArgs e)
        {
            if (sender == ModeSelectorViewModel)
            {
                if (ModeSelectorViewModel.SelectedItem is ComBoxItem item && item.Value is StandardCylinderMode mode)
                {
                    Mode = mode;
                }
            }

            AfterModified?.Invoke(sender, e);
        }

        private void UpdateContent()
        {
            CurrentContentViewModel = Mode switch
            {
                StandardCylinderMode.SingleControlSingleLimit => SingleControlSingleLimitViewModel,
                StandardCylinderMode.SingleControlDoubleLimit => SingleControlDoubleLimitViewModel,
                StandardCylinderMode.DoubleControlSingleLimit => DoubleControlSingleLimitViewModel,
                StandardCylinderMode.DoubleControlDoubleLimit => DoubleControlDoubleLimitViewModel,
                _ => SingleControlSingleLimitViewModel,
            };
        }

        /// <summary>
        /// 设置视图引用
        /// </summary>
        public void SetViewReference(Control view)
        {
            _viewReference = view;
        }

        /// <summary>
        /// 从数据模型复制到视图模型
        /// </summary>
        public void CopyFrom(StandardCylinderCfgInfo model)
        {
            if (model == null)
            {
                return;
            }

            PageTypeRowViewModel.CopyFrom(model.PageTypeRow);

            Mode = model.Mode;

            SingleControlSingleLimitViewModel.CopyFrom(model.SingleControlSingleLimit);
            SingleControlDoubleLimitViewModel.CopyFrom(model.SingleControlDoubleLimit);
            DoubleControlSingleLimitViewModel.CopyFrom(model.DoubleControlSingleLimit);
            DoubleControlDoubleLimitViewModel.CopyFrom(model.DoubleControlDoubleLimit);

            if (ModeSelectorViewModel.ItemsSource is IEnumerable<ComBoxItem> items)
            {
                foreach (var item in items)
                {
                    if (item.Value is StandardCylinderMode itemMode && itemMode == Mode)
                    {
                        ModeSelectorViewModel.SelectedItem = item;
                        break;
                    }
                }
            }

            UpdateContent();
        }

        /// <summary>
        /// 从视图模型复制到数据模型
        /// </summary>
        public void CopyTo(StandardCylinderCfgInfo model)
        {
            if (model == null)
            {
                return;
            }

            PageTypeRowViewModel.CopyTo(model.PageTypeRow);

            model.Mode = Mode;
            SingleControlSingleLimitViewModel.CopyTo(model.SingleControlSingleLimit);
            SingleControlDoubleLimitViewModel.CopyTo(model.SingleControlDoubleLimit);
            DoubleControlSingleLimitViewModel.CopyTo(model.DoubleControlSingleLimit);
            DoubleControlDoubleLimitViewModel.CopyTo(model.DoubleControlDoubleLimit);
        }
    }
}
