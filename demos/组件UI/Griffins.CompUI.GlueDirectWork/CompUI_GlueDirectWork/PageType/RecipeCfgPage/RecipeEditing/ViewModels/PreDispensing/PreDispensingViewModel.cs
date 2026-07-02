using Avalonia.Controls;
using Avalonia.VisualTree;
using GF_Gereric;
using GKG.UI;
using GKG.UI.General;
using Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.Models;
using Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.Views;
using Griffins.Map.UI;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;

namespace Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.ViewModels
{
    public class PreDispensingViewModel : ReactiveObject
    {
        /// <summary>
        /// 页面布局对象
        /// </summary>
        private Control? _viewReference;

        /// <summary>
        /// 回调接口
        /// </summary>
        private ICompUIRunTimeCallBack? _callBack;

        /// <summary>
        /// 识别参数 
        /// </summary>
        private TwoDDetectionProgrammingCfgInfo _twoDDetectionProgrammingCfgInfo;

        private byte[]? _cfgInfo;
        private PreDispensingCfgModel _cfgModel;

        public event EventHandler? AfterModified;

        private const string CmdID_DryRun = "DryRun";

        private const string CmdId_PreDispensingRun = "PreDispensingRun";

        private const string CmdId_Reset = "Reset";

        public ToggleSwitchViewModel EnablePreDispensingViewModel { get; }
        public ToggleSwitchViewModel EnableRealTimeHeightMeasurementViewModel { get; }
        public ToggleSwitchViewModel Enable2DViewModel { get; }
        public ToggleSwitchViewModel EnableAutoResetViewModel { get; }

        public TextInputViewModel SinglePreDispensingCountViewModel { get; }

        public TextInputViewModel XCountViewModel { get; }

        public TextInputViewModel YCountViewModel { get; }

        public ComboxViewModel DispensingStyleViewModel { get; }

        public TextInputViewModel DispensingPointCountViewModel { get; }

        public TextInputViewModel RemainingCountViewModel { get; }

        public ReactiveCommand<Unit, Unit> ResetCommand { get; }

        /// <summary>试运行</summary>
        public ReactiveCommand<Unit, Unit> DryRunCommand { get; }

        /// <summary>创建模板</summary>
        public ReactiveCommand<Unit, Unit> CreateTemplateCommand { get; }

        /// <summary>预点胶运行</summary>
        public ReactiveCommand<Unit, Unit> PreDispensingRunCommand { get; }

        /// <summary>保存当前配置</summary>
        public ReactiveCommand<Unit, Unit> SaveCommand { get; }

        public TextBlockViewModel EnablePreDispensingLabelViewModel { get; }

        public TextBlockViewModel EnableRealTimeHeightMeasurementLabelViewModel { get; }

        public TextBlockViewModel Enable2DLabelViewModel { get; }

        public TextBlockViewModel EnableAutoResetLabelViewModel { get; }

        public TextBlockViewModel SinglePreDispensingCountLabelViewModel { get; }

        public TextBlockViewModel XCountLabelViewModel { get; }

        public TextBlockViewModel YCountLabelViewModel { get; }

        public TextBlockViewModel DispensingStyleLabelViewModel { get; }

        public TextBlockViewModel DispensingPointCountLabelViewModel { get; }

        public TextBlockViewModel RemainingCountLabelViewModel { get; }

        public TextBlockViewModel DispensingTypeLabelViewModel { get; }

        public TextBlockViewModel DotMatrixLabelViewModel { get; }

        public TextBlockViewModel LeftUpperLabelViewModel { get; }

        public TextBlockViewModel LeftLowerLabelViewModel { get; }

        public TextBlockViewModel RightUpperLabelViewModel { get; }

        public BasePositionViewModel DotMatrix_LeftUpperPositionViewModel { get; }
        public BasePositionViewModel DotMatrix_LeftLowerPositionViewModel { get; }
        public BasePositionViewModel DotMatrix_RightUpperPositionViewModel { get; }

        private bool _isFilteringText;

        private DispensingType _preDispensingType = DispensingType.Point;
        public DispensingType PreDispensingType
        {
            get => _preDispensingType;
            set
            {
                if (_preDispensingType != value)
                {
                    this.RaiseAndSetIfChanged(ref _preDispensingType, value);
                    this.RaisePropertyChanged(nameof(IsPointType));
                    this.RaisePropertyChanged(nameof(IsLineType));
                    AfterModified?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public bool IsPointType
        {
            get => PreDispensingType == DispensingType.Point;
            set
            {
                if (value)
                {
                    PreDispensingType = DispensingType.Point;
                }
            }
        }

        public bool IsLineType
        {
            get => PreDispensingType == DispensingType.Line;
            set
            {
                if (value)
                {
                    PreDispensingType = DispensingType.Line;
                }
            }
        }

        public PreDispensingViewModel()
        {
            _cfgModel = new PreDispensingCfgModel();
            _twoDDetectionProgrammingCfgInfo = new TwoDDetectionProgrammingCfgInfo();

            EnablePreDispensingViewModel = new ToggleSwitchViewModel { IsChecked = false };
            EnableRealTimeHeightMeasurementViewModel = new ToggleSwitchViewModel { IsChecked = false };
            Enable2DViewModel = new ToggleSwitchViewModel { IsChecked = false };
            EnableAutoResetViewModel = new ToggleSwitchViewModel { IsChecked = false };

            SinglePreDispensingCountViewModel = new TextInputViewModel { Text = "1" };
            XCountViewModel = new TextInputViewModel { Text = "0" };
            YCountViewModel = new TextInputViewModel { Text = "0" };

            DispensingStyleViewModel = new ComboxViewModel
            {
                ItemsSource = new ObservableCollection<ComBoxItem>
                {
                    new ComBoxItem { DisplayName = "样式1" },
                    new ComBoxItem { DisplayName = "样式2" },
                    new ComBoxItem { DisplayName = "样式3" },
                },
                DisplayMemberPath = nameof(ComBoxItem.DisplayName),
            };

            DispensingPointCountViewModel = new TextInputViewModel { Text = "0" };
            RemainingCountViewModel = new TextInputViewModel { Text = "0" };

            EnablePreDispensingLabelViewModel = new TextBlockViewModel { Text = "是否开启预点胶" };
            EnableRealTimeHeightMeasurementLabelViewModel = new TextBlockViewModel { Text = "实时测高" };
            Enable2DLabelViewModel = new TextBlockViewModel { Text = "是否开启2D" };
            EnableAutoResetLabelViewModel = new TextBlockViewModel { Text = "自动重置" };
            SinglePreDispensingCountLabelViewModel = new TextBlockViewModel { Text = "预点胶个数" };
            XCountLabelViewModel = new TextBlockViewModel { Text = "X个数" };
            YCountLabelViewModel = new TextBlockViewModel { Text = "Y个数" };
            DispensingStyleLabelViewModel = new TextBlockViewModel { Text = "样式" };
            DispensingPointCountLabelViewModel = new TextBlockViewModel { Text = "点胶点数" };
            RemainingCountLabelViewModel = new TextBlockViewModel { Text = "剩余个数" };

            DispensingTypeLabelViewModel = new TextBlockViewModel { Text = "打胶类型" };
            DotMatrixLabelViewModel = new TextBlockViewModel { Text = "点阵" };
            LeftUpperLabelViewModel = new TextBlockViewModel { Text = "左上" };
            LeftLowerLabelViewModel = new TextBlockViewModel { Text = "左下" };
            RightUpperLabelViewModel = new TextBlockViewModel { Text = "右上" };

            DotMatrix_LeftUpperPositionViewModel = new BasePositionViewModel();
            DotMatrix_LeftLowerPositionViewModel = new BasePositionViewModel();
            DotMatrix_RightUpperPositionViewModel = new BasePositionViewModel();

            ResetCommand = ReactiveCommand.Create(OnResetCommand);

            DryRunCommand = ReactiveCommand.Create(OnDryRunCommand);
            CreateTemplateCommand = ReactiveCommand.CreateFromTask(onCreateTemplateCommand);
            PreDispensingRunCommand = ReactiveCommand.Create(OnPreDispensingRunCommand);
            SaveCommand = ReactiveCommand.Create(() =>
            {
                _ = CfgInfo;
                AfterModified?.Invoke(this, EventArgs.Empty);
            });

            attachNumericOnlyFilter(SinglePreDispensingCountViewModel, "1");
            attachNumericOnlyFilter(XCountViewModel, "0");
            attachNumericOnlyFilter(YCountViewModel, "0");
            attachNumericOnlyFilter(DispensingPointCountViewModel, "0");
            attachNumericOnlyFilter(RemainingCountViewModel, "0");

            subscribeChildViewModelEvents();
        }

        public PreDispensingViewModel(ICompUIRunTimeCallBack? callBack)
        {
            _callBack = callBack;

            // 重复初始化逻辑
            _cfgModel = new PreDispensingCfgModel();
            _twoDDetectionProgrammingCfgInfo = new TwoDDetectionProgrammingCfgInfo();

            EnablePreDispensingViewModel = new ToggleSwitchViewModel { IsChecked = false };
            EnableRealTimeHeightMeasurementViewModel = new ToggleSwitchViewModel { IsChecked = false };
            Enable2DViewModel = new ToggleSwitchViewModel { IsChecked = false };
            EnableAutoResetViewModel = new ToggleSwitchViewModel { IsChecked = false };

            SinglePreDispensingCountViewModel = new TextInputViewModel { Text = "1" };
            XCountViewModel = new TextInputViewModel { Text = "0" };
            YCountViewModel = new TextInputViewModel { Text = "0" };

            DispensingStyleViewModel = new ComboxViewModel
            {
                ItemsSource = new ObservableCollection<ComBoxItem>
                {
                    new ComBoxItem { DisplayName = "样式1" },
                    new ComBoxItem { DisplayName = "样式2" },
                    new ComBoxItem { DisplayName = "样式3" },
                },
                DisplayMemberPath = nameof(ComBoxItem.DisplayName),
            };

            DispensingPointCountViewModel = new TextInputViewModel { Text = "0" };
            RemainingCountViewModel = new TextInputViewModel { Text = "0" };

            EnablePreDispensingLabelViewModel = new TextBlockViewModel { Text = "是否开启预点胶" };
            EnableRealTimeHeightMeasurementLabelViewModel = new TextBlockViewModel { Text = "实时测高" };
            Enable2DLabelViewModel = new TextBlockViewModel { Text = "是否开启2D" };
            EnableAutoResetLabelViewModel = new TextBlockViewModel { Text = "自动重置" };
            SinglePreDispensingCountLabelViewModel = new TextBlockViewModel { Text = "预点胶个数" };
            XCountLabelViewModel = new TextBlockViewModel { Text = "X个数" };
            YCountLabelViewModel = new TextBlockViewModel { Text = "Y个数" };
            DispensingStyleLabelViewModel = new TextBlockViewModel { Text = "样式" };
            DispensingPointCountLabelViewModel = new TextBlockViewModel { Text = "点胶点数" };
            RemainingCountLabelViewModel = new TextBlockViewModel { Text = "剩余个数" };

            DispensingTypeLabelViewModel = new TextBlockViewModel { Text = "打胶类型" };
            DotMatrixLabelViewModel = new TextBlockViewModel { Text = "点阵" };
            LeftUpperLabelViewModel = new TextBlockViewModel { Text = "左上" };
            LeftLowerLabelViewModel = new TextBlockViewModel { Text = "左下" };
            RightUpperLabelViewModel = new TextBlockViewModel { Text = "右上" };

            DotMatrix_LeftUpperPositionViewModel = new BasePositionViewModel();
            DotMatrix_LeftLowerPositionViewModel = new BasePositionViewModel();
            DotMatrix_RightUpperPositionViewModel = new BasePositionViewModel();

            ResetCommand = ReactiveCommand.Create(OnResetCommand);

            DryRunCommand = ReactiveCommand.Create(OnDryRunCommand);
            CreateTemplateCommand = ReactiveCommand.CreateFromTask(onCreateTemplateCommand);
            PreDispensingRunCommand = ReactiveCommand.Create(OnPreDispensingRunCommand);
            SaveCommand = ReactiveCommand.Create(() =>
            {
                _ = CfgInfo;
                AfterModified?.Invoke(this, EventArgs.Empty);
            });

            attachNumericOnlyFilter(SinglePreDispensingCountViewModel, "1");
            attachNumericOnlyFilter(XCountViewModel, "0");
            attachNumericOnlyFilter(YCountViewModel, "0");
            attachNumericOnlyFilter(DispensingPointCountViewModel, "0");
            attachNumericOnlyFilter(RemainingCountViewModel, "0");

            subscribeChildViewModelEvents();
        }

        /// <summary>
        /// 设置回调接口
        /// </summary>
        /// <param name="callBack">回调接口</param>
        public void SetCallBack(ICompUIRunTimeCallBack? callBack)
        {
            _callBack = callBack;
        }

        /// <summary>
        /// 设置视图引用
        /// </summary>
        /// <param name="view">视图控件</param>
        public void SetViewReference(Control view)
        {
            _viewReference = view;
        }

        public void Init(byte[]? cfgInfo)
        {
            _cfgInfo = cfgInfo;
            loadCfgInfo(cfgInfo);
        }


        public byte[]? CfgInfo
        {
            get
            {
                updateToModel();
                _cfgInfo = JsonObjConvert.ToJSonBytes(_cfgModel);
                return _cfgInfo;
            }
        }

        /// <summary>
        /// 值改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onAfterModified(object? sender, EventArgs e)
        {
            AfterModified?.Invoke(sender, e);
        }

        private void loadCfgInfo(byte[]? cfgInfo)
        {
            if (cfgInfo != null)
            {
                try
                {
                    _cfgModel = JsonObjConvert.FromJSonBytes<PreDispensingCfgModel>(cfgInfo);
                }
                catch
                {
                    _cfgModel = new PreDispensingCfgModel();
                }
            }
            else
            {
                _cfgModel = new PreDispensingCfgModel();
            }

            copyFromModel();
        }

        private void copyFromModel()
        {
            EnablePreDispensingViewModel.IsChecked = _cfgModel.EnablePreDispensing;
            EnableRealTimeHeightMeasurementViewModel.IsChecked = _cfgModel.EnableRealTimeHeightMeasurement;
            Enable2DViewModel.IsChecked = _cfgModel.Enable2D;
            EnableAutoResetViewModel.IsChecked = _cfgModel.EnableAutoReset;
            SinglePreDispensingCountViewModel.Text = _cfgModel.SinglePreDispensingCount.ToString();

            PreDispensingType = _cfgModel.PreDispensingType;

            XCountViewModel.Text = _cfgModel.XCount.ToString();
            YCountViewModel.Text = _cfgModel.YCount.ToString();
            DispensingPointCountViewModel.Text = _cfgModel.DispensingPointCount.ToString();
            RemainingCountViewModel.Text = _cfgModel.RemainingCount.ToString();

            if (DispensingStyleViewModel.ItemsSource != null)
            {
                foreach (var item in DispensingStyleViewModel.ItemsSource)
                {
                    if (item is ComBoxItem cbi && cbi.DisplayName == _cfgModel.DispensingStyle)
                    {
                        DispensingStyleViewModel.SelectedItem = cbi;
                        break;
                    }
                }
            }

            DotMatrix_LeftUpperPositionViewModel.CopyFrom(_cfgModel.DotMatrix_LeftUpperPositionInfo);
            DotMatrix_LeftLowerPositionViewModel.CopyFrom(_cfgModel.DotMatrix_LeftLowerPositionInfo);
            DotMatrix_RightUpperPositionViewModel.CopyFrom(_cfgModel.DotMatrix_RightUpperPositionInfo);
        }

        private void updateToModel()
        {
            _cfgModel.EnablePreDispensing = EnablePreDispensingViewModel.IsChecked;
            _cfgModel.EnableRealTimeHeightMeasurement = EnableRealTimeHeightMeasurementViewModel.IsChecked;
            _cfgModel.Enable2D = Enable2DViewModel.IsChecked;
            _cfgModel.EnableAutoReset = EnableAutoResetViewModel.IsChecked;
            _cfgModel.SinglePreDispensingCount = parseIntOrDefault(SinglePreDispensingCountViewModel.Text, 1);
            _cfgModel.PreDispensingType = PreDispensingType;

            _cfgModel.XCount = parseIntOrDefault(XCountViewModel.Text, 0);
            _cfgModel.YCount = parseIntOrDefault(YCountViewModel.Text, 0);
            _cfgModel.DispensingPointCount = parseIntOrDefault(DispensingPointCountViewModel.Text, 0);
            _cfgModel.RemainingCount = parseIntOrDefault(RemainingCountViewModel.Text, 0);
            _cfgModel.DispensingStyle = (DispensingStyleViewModel.SelectedItem as ComBoxItem)?.DisplayName ?? string.Empty;

            DotMatrix_LeftUpperPositionViewModel.CopyTo(_cfgModel.DotMatrix_LeftUpperPositionInfo);
            DotMatrix_LeftLowerPositionViewModel.CopyTo(_cfgModel.DotMatrix_LeftLowerPositionInfo);
            DotMatrix_RightUpperPositionViewModel.CopyTo(_cfgModel.DotMatrix_RightUpperPositionInfo);
        }

        private void subscribeChildViewModelEvents()
        {
            EnablePreDispensingViewModel.ValueChanged += onChildValueChanged;
            EnableRealTimeHeightMeasurementViewModel.ValueChanged += onChildValueChanged;
            Enable2DViewModel.ValueChanged += onChildValueChanged;
            EnableAutoResetViewModel.ValueChanged += onChildValueChanged;
            SinglePreDispensingCountViewModel.ValueChanged += onChildValueChanged;

            XCountViewModel.ValueChanged += onChildValueChanged;
            YCountViewModel.ValueChanged += onChildValueChanged;
            DispensingPointCountViewModel.ValueChanged += onChildValueChanged;
            RemainingCountViewModel.ValueChanged += onChildValueChanged;
            DispensingStyleViewModel.ValueChanged += onChildValueChanged;

            DotMatrix_LeftUpperPositionViewModel.AfterModified += onChildValueChanged;
            DotMatrix_LeftLowerPositionViewModel.AfterModified += onChildValueChanged;
            DotMatrix_RightUpperPositionViewModel.AfterModified += onChildValueChanged;
        }

        private void onChildValueChanged(object? sender, EventArgs e)
        {
            AfterModified?.Invoke(this, EventArgs.Empty);
        }

        private static int parseIntOrDefault(string? text, int defaultValue)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return defaultValue;
            }

            return int.TryParse(text.Trim(), out var value) ? value : defaultValue;
        }

        private void attachNumericOnlyFilter(TextInputViewModel vm, string defaultText)
        {
            vm.ValueChanged += (_, __) =>
            {
                if (_isFilteringText)
                {
                    return;
                }

                var current = vm.Text ?? string.Empty;
                var filtered = new string(current.Where(char.IsDigit).ToArray());
                if (string.IsNullOrWhiteSpace(filtered))
                {
                    filtered = defaultText;
                }

                if (!string.Equals(current, filtered, StringComparison.Ordinal))
                {
                    _isFilteringText = true;
                    try
                    {
                        vm.Text = filtered;
                    }
                    finally
                    {
                        _isFilteringText = false;
                    }
                }
            };
        }

        //创建模板命令
        private async Task onCreateTemplateCommand()
        {
            var parentWindow = _viewReference?.GetVisualRoot() as Window;
            if (parentWindow == null)
            {
                await MessageBox.ShowErrorDialog("错误", "无法获取窗口上下文，操作失败", _viewReference);
                return;
            }

            var editViewModel = new TwoDDetectionProgrammingWindowViewModel();
            editViewModel.CopyFrom(_twoDDetectionProgrammingCfgInfo);
            var editWindow = new TwoDDetectionProgrammingWindow
            {
                DataContext = editViewModel,
                Title = "2D模板编辑",
                Width = 1920,
                Height = 1020,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            editViewModel.SetViewReference(editWindow);

            var result = await editWindow.ShowDialog<bool>(parentWindow);
            if ((result))
            {
                editViewModel.CopyTo(_twoDDetectionProgrammingCfgInfo);
                onAfterModified(this, new EventArgs());
            }
        }

        /// <summary>
        /// 预运行命令处理
        /// </summary>
        private void OnDryRunCommand()
        {
            try
            {
                _callBack?.ExecConfigSvrCtlCmd(CmdID_DryRun, null);
            }
            catch (Exception ex)
            {
                // 可以在这里添加错误处理，比如显示错误消息
                System.Diagnostics.Debug.WriteLine($"预运行命令执行失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 预点胶运行命令处理
        /// </summary>
        private void OnPreDispensingRunCommand()
        {
            try
            {
                _callBack?.ExecConfigSvrCtlCmd(CmdId_PreDispensingRun, null);
            }
            catch (Exception ex)
            {
                // 可以在这里添加错误处理，比如显示错误消息
                System.Diagnostics.Debug.WriteLine($"预点胶运行命令执行失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 重置命令处理
        /// </summary>
        private void OnResetCommand()
        {
            try
            {
                _callBack?.ExecConfigSvrCtlCmd(CmdId_Reset, null);
            }
            catch (Exception ex)
            {
                // 可以在这里添加错误处理，比如显示错误消息
                System.Diagnostics.Debug.WriteLine($"重置命令执行失败: {ex.Message}");
            }
        }
    }
}
