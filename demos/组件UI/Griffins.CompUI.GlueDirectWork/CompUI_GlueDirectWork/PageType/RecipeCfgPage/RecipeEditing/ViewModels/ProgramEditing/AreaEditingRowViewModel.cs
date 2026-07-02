using GKG.UI;
using GKG.UI.General;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.ViewModels
{
    internal sealed class AreaEditingRowViewModel : ReactiveObject
    {
        /// <summary>当前行显示序号。</summary>
        private int _serialNumber;
        /// <summary>当前行勾选状态。</summary>
        private bool _isChecked;
        /// <summary>可选模板源集合。</summary>
        private readonly ObservableCollection<ProgramTemplateViewModel> _templates;
        /// <summary>模板名称变化订阅集合。</summary>
        private readonly List<IDisposable> _templateNameSubscriptions = new();

        /// <summary>初始化区域编辑行及其默认参数控件。</summary>
        public AreaEditingRowViewModel(ObservableCollection<ProgramTemplateViewModel> templates)
        {
            _templates = templates;
            AreaNameViewModel = new TextInputViewModel();

            TemplateViewModel = new ComboxViewModel
            {
                DisplayMemberPath = nameof(ComBoxItem.DisplayName),
                ItemsSource = new ObservableCollection<ComBoxItem>()
            };
            _templates.CollectionChanged += OnTemplatesChanged;
            AttachTemplateNameSubscriptions();
            RefreshTemplateOptions();

            EnableNeedleLiftViewModel = new ToggleSwitchViewModel();
            NeedleLiftHeightViewModel = new NumericViewModel { DecimalPlaces = 3, Increment = 0.001m };

            EnableLineCleanViewModel = new ToggleSwitchViewModel();
            FirstPointStableTimeViewModel = new NumericViewModel { DecimalPlaces = 0, Increment = 10, Minimum = 0, Maximum = 600000 };

            BasePointPosition = new BasePositionViewModel();

            AreaLengthViewModel = new NumericViewModel { DecimalPlaces = 3, Increment = 0.001m };
            AreaWidthViewModel = new NumericViewModel { DecimalPlaces = 3, Increment = 0.001m };

            TemplateViewModel.WhenAnyValue(x => x.SelectedItem)
                .Subscribe(_ => this.RaisePropertyChanged(nameof(TemplateDisplayText)));
        }

        /// <summary>仅用于详情区只读展示，与列表中模板下拉同步。</summary>
        public string TemplateDisplayText =>
            TemplateViewModel.SelectedItem is ComBoxItem c ? c.DisplayName : string.Empty;

        /// <summary>区域名称输入控件。</summary>
        public TextInputViewModel AreaNameViewModel { get; }
        /// <summary>模板选择下拉控件。</summary>
        public ComboxViewModel TemplateViewModel { get; }
        /// <summary>换行抬针开关控件。</summary>
        public ToggleSwitchViewModel EnableNeedleLiftViewModel { get; }
        /// <summary>抬针高度输入控件。</summary>
        public NumericViewModel NeedleLiftHeightViewModel { get; }
        /// <summary>换行清洁开关控件。</summary>
        public ToggleSwitchViewModel EnableLineCleanViewModel { get; }
        /// <summary>首点稳定时间输入控件。</summary>
        public NumericViewModel FirstPointStableTimeViewModel { get; }
        /// <summary>区域基准点坐标控件。</summary>
        public BasePositionViewModel BasePointPosition { get; }
        /// <summary>区域长度输入控件。</summary>
        public NumericViewModel AreaLengthViewModel { get; }
        /// <summary>区域宽度输入控件。</summary>
        public NumericViewModel AreaWidthViewModel { get; }

        /// <summary>列表中的序号字段。</summary>
        public int SerialNumber
        {
            get => _serialNumber;
            set => this.RaiseAndSetIfChanged(ref _serialNumber, value);
        }

        /// <summary>列表中的勾选字段。</summary>
        public bool IsChecked
        {
            get => _isChecked;
            set => this.RaiseAndSetIfChanged(ref _isChecked, value);
        }

        /// <summary>模板集合变化时，刷新模板选项和订阅。</summary>
        private void OnTemplatesChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            AttachTemplateNameSubscriptions();
            RefreshTemplateOptions();
        }

        /// <summary>重新挂接模板名称变化监听。</summary>
        private void AttachTemplateNameSubscriptions()
        {
            foreach (var d in _templateNameSubscriptions)
                d.Dispose();
            _templateNameSubscriptions.Clear();

            foreach (var t in _templates)
            {
                _templateNameSubscriptions.Add(
                    t.WhenAnyValue(x => x.Name)
                        .Skip(1)
                        .Subscribe(_ => RefreshTemplateOptions()));
            }
        }

        /// <summary>按当前模板集合重建下拉选项并尽量保留原选中项。</summary>
        private void RefreshTemplateOptions()
        {
            if (TemplateViewModel.ItemsSource is not ObservableCollection<ComBoxItem> options)
                return;

            var selected = TemplateViewModel.SelectedItem as ComBoxItem;
            var selectedTemplateId = selected?.Value?.ToString();

            options.Clear();
            foreach (var t in _templates)
                options.Add(new ComBoxItem { Value = t.Id, DisplayName = t.Name });

            TemplateViewModel.SelectedItem = options.FirstOrDefault(x => Equals(x.Value, selectedTemplateId))
                ?? options.FirstOrDefault();
        }
    }
}
