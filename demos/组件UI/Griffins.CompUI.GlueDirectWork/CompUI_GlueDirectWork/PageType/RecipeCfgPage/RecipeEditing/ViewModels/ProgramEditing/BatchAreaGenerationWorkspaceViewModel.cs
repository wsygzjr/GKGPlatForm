using GKG.UI;
using GKG.UI.General;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Collections.Generic;

namespace Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.ViewModels
{
    internal sealed class BatchAreaGenerationWorkspaceViewModel : ReactiveObject
    {
        /// <summary>模板集合引用。</summary>
        private readonly ObservableCollection<ProgramTemplateViewModel> _templates;
        /// <summary>区域编辑工作区引用（用于追加生成结果）。</summary>
        private readonly AreaEditingWorkspaceViewModel _areaEditingWorkspace;
        /// <summary>模板名称变化订阅集合。</summary>
        private readonly List<IDisposable> _templateNameSubscriptions = new();

        /// <summary>初始化批量生成区域工作区。</summary>
        public BatchAreaGenerationWorkspaceViewModel(
            ObservableCollection<ProgramTemplateViewModel> templates,
            AreaEditingWorkspaceViewModel areaEditingWorkspace)
        {
            _templates = templates;
            _areaEditingWorkspace = areaEditingWorkspace;
            NameViewModel = new TextInputViewModel();

            TemplateViewModel = new ComboxViewModel
            {
                DisplayMemberPath = nameof(ComBoxItem.DisplayName),
                ItemsSource = new ObservableCollection<ComBoxItem>()
            };
            _templates.CollectionChanged += OnTemplatesChanged;
            AttachTemplateNameSubscriptions();
            RefreshTemplateOptions();

            StartX = new NumericViewModel { DecimalPlaces = 3, Increment = 0.001m };
            StartY = new NumericViewModel { DecimalPlaces = 3, Increment = 0.001m };
            StartZ = new NumericViewModel { DecimalPlaces = 3, Increment = 0.001m };

            GenerationModeViewModel = new ComboxViewModel
            {
                DisplayMemberPath = nameof(ComBoxItem.DisplayName),
                ItemsSource = new ObservableCollection<ComBoxItem>
                {
                    new() { Value = "一维矩阵", DisplayName = "一维矩阵" },
                    new() { Value = "二维矩阵", DisplayName = "二维矩阵" },
                }
            };
            GenerationModeViewModel.SelectedItem = ((ObservableCollection<ComBoxItem>)GenerationModeViewModel.ItemsSource!).First();

            AreaLength = new NumericViewModel { DecimalPlaces = 3, Increment = 0.001m };
            AreaWidth = new NumericViewModel { DecimalPlaces = 3, Increment = 0.001m };
            RowCount = new NumericViewModel { DecimalPlaces = 0, Increment = 1, Minimum = 1, Maximum = 999 };
            ColCount = new NumericViewModel { DecimalPlaces = 0, Increment = 1, Minimum = 1, Maximum = 999 };

            StartPointViewModel = new ComboxViewModel
            {
                DisplayMemberPath = nameof(ComBoxItem.DisplayName),
                ItemsSource = new ObservableCollection<ComBoxItem>
                {
                    new() { Value = "左上", DisplayName = "左上" },
                    new() { Value = "右上", DisplayName = "右上" },
                    new() { Value = "左下", DisplayName = "左下" },
                    new() { Value = "右下", DisplayName = "右下" },
                }
            };
            StartPointViewModel.SelectedItem = ((ObservableCollection<ComBoxItem>)StartPointViewModel.ItemsSource!).First();

            MatrixDirectionViewModel = new ComboxViewModel
            {
                DisplayMemberPath = nameof(ComBoxItem.DisplayName),
                ItemsSource = new ObservableCollection<ComBoxItem>
                {
                    new() { Value = "X方向", DisplayName = "X方向" },
                    new() { Value = "Y方向", DisplayName = "Y方向" },
                }
            };
            MatrixDirectionViewModel.SelectedItem = ((ObservableCollection<ComBoxItem>)MatrixDirectionViewModel.ItemsSource!).First();

            PathShapeViewModel = new ComboxViewModel
            {
                DisplayMemberPath = nameof(ComBoxItem.DisplayName),
                ItemsSource = new ObservableCollection<ComBoxItem>
                {
                    new() { Value = "直线", DisplayName = "直线" },
                    new() { Value = "蛇形", DisplayName = "蛇形" },
                }
            };
            PathShapeViewModel.SelectedItem = ((ObservableCollection<ComBoxItem>)PathShapeViewModel.ItemsSource!).First();

            CornerPositions = new ObservableCollection<CornerPositionRowViewModel>
            {
                new("左上角"),
                new("右上角"),
                new("右下角"),
            };

            EnableNeedleCompensationViewModel = new ToggleSwitchViewModel();
            NeedleCompensationHeight = new NumericViewModel { DecimalPlaces = 3, Increment = 0.001m };

            EnableFirstPointWaitViewModel = new ToggleSwitchViewModel();
            FirstPointWaitMs = new NumericViewModel { DecimalPlaces = 0, Increment = 10, Minimum = 0, Maximum = 600000 };

            GenerateCommand = ReactiveCommand.Create(GenerateAreaFromBatch);
            TeachStartCommand = ReactiveCommand.Create(() => { });
            MoveCameraToStartCommand = ReactiveCommand.Create(() => { });
        }

        /// <summary>批量生成名称输入控件。</summary>
        public TextInputViewModel NameViewModel { get; }
        /// <summary>模板选择下拉控件。</summary>
        public ComboxViewModel TemplateViewModel { get; }

        /// <summary>起点 X 输入控件。</summary>
        public NumericViewModel StartX { get; }
        /// <summary>起点 Y 输入控件。</summary>
        public NumericViewModel StartY { get; }
        /// <summary>起点 Z 输入控件。</summary>
        public NumericViewModel StartZ { get; }

        /// <summary>生成模式下拉控件。</summary>
        public ComboxViewModel GenerationModeViewModel { get; }

        /// <summary>区域长度输入控件。</summary>
        public NumericViewModel AreaLength { get; }
        /// <summary>区域宽度输入控件。</summary>
        public NumericViewModel AreaWidth { get; }
        /// <summary>行数输入控件。</summary>
        public NumericViewModel RowCount { get; }
        /// <summary>列数输入控件。</summary>
        public NumericViewModel ColCount { get; }

        /// <summary>起始角下拉控件。</summary>
        public ComboxViewModel StartPointViewModel { get; }
        /// <summary>矩阵方向下拉控件。</summary>
        public ComboxViewModel MatrixDirectionViewModel { get; }
        /// <summary>路径形状下拉控件。</summary>
        public ComboxViewModel PathShapeViewModel { get; }

        /// <summary>角点坐标集合。</summary>
        public ObservableCollection<CornerPositionRowViewModel> CornerPositions { get; }

        /// <summary>换行抬针补偿开关控件。</summary>
        public ToggleSwitchViewModel EnableNeedleCompensationViewModel { get; }
        /// <summary>抬针补偿高度输入控件。</summary>
        public NumericViewModel NeedleCompensationHeight { get; }

        /// <summary>首点等待开关控件。</summary>
        public ToggleSwitchViewModel EnableFirstPointWaitViewModel { get; }
        /// <summary>首点等待时长输入控件。</summary>
        public NumericViewModel FirstPointWaitMs { get; }

        /// <summary>起点示教命令（预留）。</summary>
        public ReactiveCommand<Unit, Unit> TeachStartCommand { get; }
        /// <summary>相机移动到起点命令（预留）。</summary>
        public ReactiveCommand<Unit, Unit> MoveCameraToStartCommand { get; }
        /// <summary>执行批量生成并追加区域命令。</summary>
        public ReactiveCommand<Unit, Unit> GenerateCommand { get; }

        /// <summary>把批量生成参数映射为一个区域行并添加到区域编辑列表。</summary>
        private void GenerateAreaFromBatch()
        {
            var row = new AreaEditingRowViewModel(_templates);

            // 区域名：优先用批量界面输入；空则用区域编辑列表自动序号
            var name = NameViewModel.Text;
            if (string.IsNullOrWhiteSpace(name))
                name = $"区域{_areaEditingWorkspace.Items.Count + 1}";
            row.AreaNameViewModel.Text = name;

            // 模板：同步批量界面当前选中的模板
            var batchSelected = TemplateViewModel.SelectedItem as ComBoxItem;
            if (batchSelected != null && row.TemplateViewModel.ItemsSource is ObservableCollection<ComBoxItem> options)
            {
                var selectedId = batchSelected.Value?.ToString();
                var selectedName = batchSelected.DisplayName;
                var match = options.FirstOrDefault(x => x.Value?.ToString() == selectedId && x.DisplayName == selectedName)
                             ?? options.FirstOrDefault(x => x.Value?.ToString() == selectedId);
                if (match != null)
                    row.TemplateViewModel.SelectedItem = match;
            }

            // 起始点坐标（作为区域基准点）
            row.BasePointPosition.X = StartX.Value;
            row.BasePointPosition.Y = StartY.Value;
            row.BasePointPosition.Z = StartZ.Value;

            // 区域参数
            row.AreaLengthViewModel.Value = AreaLength.Value;
            row.AreaWidthViewModel.Value = AreaWidth.Value;

            // 换行抬针 & 换行清洁（批量界面映射到区域编辑字段）
            row.EnableNeedleLiftViewModel.IsChecked = EnableNeedleCompensationViewModel.IsChecked;
            row.NeedleLiftHeightViewModel.Value = NeedleCompensationHeight.Value;

            row.EnableLineCleanViewModel.IsChecked = EnableFirstPointWaitViewModel.IsChecked;
            row.FirstPointStableTimeViewModel.Value = FirstPointWaitMs.Value;

            _areaEditingWorkspace.AppendRowFromBatch(row);
        }

        /// <summary>模板集合变化时刷新下拉选项和监听。</summary>
        private void OnTemplatesChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            AttachTemplateNameSubscriptions();
            RefreshTemplateOptions();
        }

        /// <summary>重建模板名称变更订阅。</summary>
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

        /// <summary>按当前模板集合刷新模板下拉，并尽量保留选中项。</summary>
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

    internal sealed class CornerPositionRowViewModel : ReactiveObject
    {
        /// <summary>初始化角点行。</summary>
        public CornerPositionRowViewModel(string name)
        {
            Name = name;
            Position = new BasePositionViewModel();
        }

        /// <summary>角点名称。</summary>
        public string Name { get; }
        /// <summary>角点坐标。</summary>
        public BasePositionViewModel Position { get; }
    }
}

