using Griffins;
using Griffins.Map.Cmd;
using ReactiveUI;
using System;
using System.Reactive;

namespace GKG.Map.AuxiliaryFunctionsFuncCtlMapCell.ViewModel
{
    // ==========================================
    // 单个阀的数据模型 (抽象出左右阀完全共用的结构)
    // ==========================================
    public class ValveConfigViewModel : ReactiveObject
    {
        public string ValveName { get; }

        // --- 时间模式 ---
        private bool _isTimeModeEnabled;
        public bool IsTimeModeEnabled { get => _isTimeModeEnabled; set => this.RaiseAndSetIfChanged(ref _isTimeModeEnabled, value); }

        private string _targetTimeText = "0";
        public string TargetTimeText { get => _targetTimeText; set => this.RaiseAndSetIfChanged(ref _targetTimeText, value); }

        private string _currentTimeStr = "关闭";
        public string CurrentTimeStr { get => _currentTimeStr; set => this.RaiseAndSetIfChanged(ref _currentTimeStr, value); }

        private bool _isTimeAlarm;
        public bool IsTimeAlarm { get => _isTimeAlarm; set => this.RaiseAndSetIfChanged(ref _isTimeAlarm, value); }

        // 实际数值属性（供后台逻辑使用，不直接绑定 UI）
        public double TargetTime
        {
            get => double.TryParse(TargetTimeText, out var val) ? val : 0;
            set => TargetTimeText = value.ToString();
        }

        // --- 重量模式 ---
        private bool _isWeightModeEnabled;
        public bool IsWeightModeEnabled { get => _isWeightModeEnabled; set => this.RaiseAndSetIfChanged(ref _isWeightModeEnabled, value); }

        private double _targetWeight;
        public double TargetWeight { get => _targetWeight; set => this.RaiseAndSetIfChanged(ref _targetWeight, value); }

        private string _currentWeightStr = "关闭";
        public string CurrentWeightStr { get => _currentWeightStr; set => this.RaiseAndSetIfChanged(ref _currentWeightStr, value); }

        private bool _isWeightAlarm;
        public bool IsWeightAlarm { get => _isWeightAlarm; set => this.RaiseAndSetIfChanged(ref _isWeightAlarm, value); }

        public ValveConfigViewModel(string valveName)
        {
            ValveName = valveName;
        }

        // 模拟刷新界面当前值和报警状态
        public void UpdateCurrentStatus(double actualTime, double actualWeight)
        {
            // 时间更新逻辑
            if (IsTimeModeEnabled)
            {
                CurrentTimeStr = actualTime.ToString("F2");
                IsTimeAlarm = (actualTime == 0); // 如果启用且当前值为0，则报警
            }
            else
            {
                CurrentTimeStr = "关闭";
                IsTimeAlarm = false;
            }

            // 重量更新逻辑
            if (IsWeightModeEnabled)
            {
                CurrentWeightStr = actualWeight.ToString("F2");
                IsWeightAlarm = (actualWeight == 0);
            }
            else
            {
                CurrentWeightStr = "关闭";
                IsWeightAlarm = false;
            }
        }
    }

    // ==========================================
    // 窗口的主 ViewModel
    // ==========================================
    public class MachineCalibrationSetViewModel : ReactiveObject
    {
        private readonly MapCmdExector _cmdExector;
        private readonly string _mapNo;

        // 是否双阀 (绑定到界面的 IsVisible 控制左阀显示)
        public bool IsDoubleValve { get; }

        // 窗口宽度 (单阀500，双阀930)
        public double WindowWidth => IsDoubleValve ? 930 : 500;

        // 左右阀实例
        public ValveConfigViewModel LeftValve { get; }
        public ValveConfigViewModel RightValve { get; }

        // 命令
        public ReactiveCommand<Unit, Unit> SaveCommand { get; }
        public ReactiveCommand<Unit, Unit> CloseCommand { get; }

        // 通知 View 关闭自身的委托
        public Action CloseAction { get; set; }

        public MachineCalibrationSetViewModel(bool isDoubleValve, MapCmdExector cmdExector, string mapNo)
        {
            IsDoubleValve = isDoubleValve;
            _cmdExector = cmdExector;
            _mapNo = mapNo;

            // 实例化阀门模型
            LeftValve = new ValveConfigViewModel("左阀");
            RightValve = new ValveConfigViewModel(isDoubleValve ? "右阀" : "阀");

            LoadParams();

            // 保存命令
            SaveCommand = ReactiveCommand.Create(() =>
            {
                SaveParams();
                CloseAction?.Invoke();
            });

            // 关闭命令
            CloseCommand = ReactiveCommand.Create(() =>
            {
                CloseAction?.Invoke();
            });
        }

        private void LoadParams()
        {
            double leftValveCurrentTime = 0;
            double leftValveCurrentWeight = 0;
            double rightValveCurrentTime = 0;
            double rightValveCurrentWeight = 0;
            try
            {
                GFBaseTypeParamValueList result = _cmdExector.ExecUIDataObjCommand(_mapNo, "LoadMachineCalibrationSetParams", new GFBaseTypeParamValueList());
                if (result != null)
                {
                    foreach (var param in result)
                    {
                        switch (param.ID)
                        {
                            case "LeftValveIsTimeModeEnabled":
                                LeftValve.IsTimeModeEnabled = param.Value?.ToPrimitiveValue<bool>() ?? false;
                                break;
                            case "LeftValveTargetTime":
                                LeftValve.TargetTime = param.Value?.ToPrimitiveValue<double>() ?? 0;
                                break;
                            case "LeftValveCurrentTime":
                                leftValveCurrentTime = param.Value?.ToPrimitiveValue<double>() ?? 0;
                                break;
                            case "LeftValveIsWeightModeEnabled":
                                LeftValve.IsWeightModeEnabled = param.Value?.ToPrimitiveValue<bool>() ?? false;
                                break;
                            case "LeftValveTargetWeight":
                                LeftValve.TargetWeight = param.Value?.ToPrimitiveValue<double>() ?? 0;
                                break;
                            case "LeftValveCurrentWeight":
                                leftValveCurrentWeight = param.Value?.ToPrimitiveValue<double>() ?? 0;
                                break;
                            case "RightValveIsTimeModeEnabled":
                                RightValve.IsTimeModeEnabled = param.Value?.ToPrimitiveValue<bool>() ?? false;
                                break;
                            case "RightValveTargetTime":
                                RightValve.TargetTime = param.Value?.ToPrimitiveValue<double>() ?? 0;
                                break;
                            case "RightValveCurrentTime":
                                rightValveCurrentTime = param.Value?.ToPrimitiveValue<double>() ?? 0;
                                break;
                            case "RightValveIsWeightModeEnabled":
                                RightValve.IsWeightModeEnabled = param.Value?.ToPrimitiveValue<bool>() ?? false;
                                break;
                            case "RightValveTargetWeight":
                                RightValve.TargetWeight = param.Value?.ToPrimitiveValue<double>() ?? 0;
                                break;
                            case "RightValveCurrentWeight":
                                rightValveCurrentWeight = param.Value?.ToPrimitiveValue<double>() ?? 0;
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // 假数据模拟左阀加载
                LeftValve.IsTimeModeEnabled = true;
                LeftValve.TargetTime = 30;
                LeftValve.IsWeightModeEnabled = true;
                LeftValve.TargetWeight = 0;

                // 假数据模拟右阀加载
                RightValve.IsTimeModeEnabled = true;
                RightValve.TargetTime = 45.5;
                rightValveCurrentTime = 98.2;
                RightValve.IsWeightModeEnabled = true;
                RightValve.TargetWeight = 100.2;
                rightValveCurrentWeight = 200.3;
            }

            LeftValve.UpdateCurrentStatus(leftValveCurrentTime, leftValveCurrentWeight);
            RightValve.UpdateCurrentStatus(rightValveCurrentTime, rightValveCurrentWeight);
        }

        private void SaveParams()
        {
            try
            {
                GFBaseTypeParamValueList cmdParam = new GFBaseTypeParamValueList
                {
                    new GFBaseTypeParamValue("RightValveIsTimeModeEnabled", new GriffinsBaseValue( RightValve.IsTimeModeEnabled)),
                    new GFBaseTypeParamValue("RightValveTargetTime", new GriffinsBaseValue(RightValve.TargetTime)),
                    new GFBaseTypeParamValue("RightValveIsWeightModeEnabled", new GriffinsBaseValue(RightValve.IsWeightModeEnabled)),
                    new GFBaseTypeParamValue("RightValveTargetWeight", new GriffinsBaseValue(RightValve.TargetWeight))
                };

                if (IsDoubleValve)
                {
                    cmdParam.Add(new GFBaseTypeParamValue("LeftValveIsTimeModeEnabled", new GriffinsBaseValue(LeftValve.IsTimeModeEnabled)));
                    cmdParam.Add(new GFBaseTypeParamValue("LeftValveTargetTime", new GriffinsBaseValue(LeftValve.TargetTime)));
                    cmdParam.Add(new GFBaseTypeParamValue("LeftValveIsWeightModeEnabled", new GriffinsBaseValue(LeftValve.IsWeightModeEnabled)));
                    cmdParam.Add(new GFBaseTypeParamValue("LeftValveTargetWeight", new GriffinsBaseValue(LeftValve.TargetWeight)));
                }

                _cmdExector.ExecUIDataObjCommand(_mapNo, "SaveMachineCalibrationSetParams", cmdParam);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"执行 SaveMachineCalibrationSetParams 失败: {ex.Message}");
            }
        }
    }
}