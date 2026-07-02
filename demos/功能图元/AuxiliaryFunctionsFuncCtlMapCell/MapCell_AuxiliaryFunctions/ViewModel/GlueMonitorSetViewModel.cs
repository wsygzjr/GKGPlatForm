using Griffins;
using Griffins.Map.Cmd;
using ReactiveUI;
using System;
using System.Reactive;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GKG.Map.AuxiliaryFunctionsFuncCtlMapCell.ViewModel
{
    // ==========================================
    // 单阀胶水监控模型
    // ==========================================
    public class ValveGlueMonitorViewModel : ReactiveObject
    {
        private readonly MapCmdExector _cmdExector;
        private readonly string _mapNo;
        private readonly int _valveIndex; // 0 = 左阀(Vice), 1 = 右阀(Main)

        public string ValveName { get; }

        // ================== 时间监控 (Time) ==================
        private bool _isTimeModeEnabled;
        public bool IsTimeModeEnabled { get => _isTimeModeEnabled; set => this.RaiseAndSetIfChanged(ref _isTimeModeEnabled, value); }

        private string _targetTimeText = "30";
        public string TargetTimeText { get => _targetTimeText; set => this.RaiseAndSetIfChanged(ref _targetTimeText, value); }
        public double TargetTime => double.TryParse(TargetTimeText, out var v) ? v : 0;

        private string _currentTimeStr = "关闭";
        public string CurrentTimeStr { get => _currentTimeStr; set => this.RaiseAndSetIfChanged(ref _currentTimeStr, value); }

        private bool _isTimeAlarm;
        public bool IsTimeAlarm { get => _isTimeAlarm; set => this.RaiseAndSetIfChanged(ref _isTimeAlarm, value); }

        // ================== 重量监控 (Weight) ==================
        private bool _isWeightModeEnabled;
        public bool IsWeightModeEnabled { get => _isWeightModeEnabled; set => this.RaiseAndSetIfChanged(ref _isWeightModeEnabled, value); }

        private string _targetWeightText = "0";
        public string TargetWeightText { get => _targetWeightText; set => this.RaiseAndSetIfChanged(ref _targetWeightText, value); }
        public double TargetWeight => double.TryParse(TargetWeightText, out var v) ? v : 0;

        private string _currentWeightStr = "关闭";
        public string CurrentWeightStr { get => _currentWeightStr; set => this.RaiseAndSetIfChanged(ref _currentWeightStr, value); }

        private bool _isWeightAlarm;
        public bool IsWeightAlarm { get => _isWeightAlarm; set => this.RaiseAndSetIfChanged(ref _isWeightAlarm, value); }

        // --- 重量报警模式 (互斥逻辑) ---
        private bool _isWeightPercentMode = true;
        public bool IsWeightPercentMode
        {
            get => _isWeightPercentMode;
            set
            {
                this.RaiseAndSetIfChanged(ref _isWeightPercentMode, value);
                this.RaisePropertyChanged(nameof(IsWeightAbsoluteMode)); // 联动刷新另一个单选框
            }
        }
        // 绝对值模式永远和百分比模式相反
        public bool IsWeightAbsoluteMode
        {
            get => !_isWeightPercentMode;
            set => IsWeightPercentMode = !value;
        }

        private string _targetWeightPercentText = "0";
        public string TargetWeightPercentText { get => _targetWeightPercentText; set => this.RaiseAndSetIfChanged(ref _targetWeightPercentText, value); }
        public double TargetWeightPercent => double.TryParse(TargetWeightPercentText, out var v) ? v : 0;

        private string _targetWeightAlarmText = "0";
        public string TargetWeightAlarmText { get => _targetWeightAlarmText; set => this.RaiseAndSetIfChanged(ref _targetWeightAlarmText, value); }
        public double TargetWeightAlarmValue => double.TryParse(TargetWeightAlarmText, out var v) ? v : 0;

        // ================== 打点数监控 (Pcs) ==================
        private bool _isPcsModeEnabled;
        public bool IsPcsModeEnabled { get => _isPcsModeEnabled; set => this.RaiseAndSetIfChanged(ref _isPcsModeEnabled, value); }

        private string _targetPcsText = "0";
        public string TargetPcsText { get => _targetPcsText; set => this.RaiseAndSetIfChanged(ref _targetPcsText, value); }
        public int TargetPcs => int.TryParse(TargetPcsText, out var v) ? v : 0;

        private string _currentPcsStr = "关闭";
        public string CurrentPcsStr { get => _currentPcsStr; set => this.RaiseAndSetIfChanged(ref _currentPcsStr, value); }

        private bool _isPcsAlarm;
        public bool IsPcsAlarm { get => _isPcsAlarm; set => this.RaiseAndSetIfChanged(ref _isPcsAlarm, value); }

        // ================== 行内操作命令 ==================
        public ReactiveCommand<Unit, Unit> ClearTimeCommand { get; }
        public ReactiveCommand<Unit, Unit> ClearPcsCommand { get; }

        public ValveGlueMonitorViewModel(string valveName, int valveIndex, MapCmdExector cmdExector, string mapNo)
        {
            ValveName = valveName;
            _valveIndex = valveIndex;
            _cmdExector = cmdExector;
            _mapNo = mapNo;

            // 清零时间命令
            ClearTimeCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                string cmdStr = _valveIndex == 0 ? "ClearLeftValveTime" : "ClearRightValveTime";

                try
                {
                    await Task.Run(() =>
                    {
                        GFBaseTypeParamValueList result = _cmdExector.ExecUIDataObjCommand(_mapNo, cmdStr, new GFBaseTypeParamValueList());
                    });
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"执行 {cmdStr} 失败: {ex.Message}");
                }
            });

            // 清零点数命令
            ClearPcsCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                string cmdStr = _valveIndex == 0 ? "ClearLeftValvePcs" : "ClearRightValvePcs";

                try
                {
                    await Task.Run(() =>
                    {
                        GFBaseTypeParamValueList result = _cmdExector.ExecUIDataObjCommand(_mapNo, cmdStr, new GFBaseTypeParamValueList());
                    });
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"执行 {cmdStr} 失败: {ex.Message}");
                }
            });
        }

        /// <summary>
        /// 刷新界面的实时读数和报警颜色
        /// </summary>
        public void UpdateCurrentStatus(double actualTime, double actualWeight, int actualPcs)
        {
            if (IsTimeModeEnabled)
            {
                CurrentTimeStr = actualTime.ToString("F2");
                IsTimeAlarm = (actualTime == 0);
            }
            else { CurrentTimeStr = "关闭"; IsTimeAlarm = false; }

            if (IsWeightModeEnabled)
            {
                CurrentWeightStr = actualWeight.ToString("F2");
                IsWeightAlarm = (actualWeight == 0);
            }
            else { CurrentWeightStr = "关闭"; IsWeightAlarm = false; }

            if (IsPcsModeEnabled)
            {
                CurrentPcsStr = actualPcs.ToString();
                IsPcsAlarm = (actualPcs == 0);
            }
            else { CurrentPcsStr = "关闭"; IsPcsAlarm = false; }
        }
    }

    // ==========================================
    // 2. 主窗体 ViewModel
    // ==========================================
    public class GlueMonitorSetViewModel : ReactiveObject
    {
        private readonly MapCmdExector _cmdExector;
        private readonly string _mapNo;

        public bool IsDoubleValve { get; }

        // 动态窗体宽度：双阀 1150，单阀 600
        public double WindowWidth => IsDoubleValve ? 1150 : 600;

        public ValveGlueMonitorViewModel LeftValve { get; }
        public ValveGlueMonitorViewModel RightValve { get; }

        public Action<bool> CloseAction { get; set; }

        public ReactiveCommand<Unit, Unit> SaveCommand { get; }
        public ReactiveCommand<Unit, Unit> CloseCommand { get; }

        public GlueMonitorSetViewModel(bool isDoubleValve, MapCmdExector cmdExector, string mapNo)
        {
            IsDoubleValve = isDoubleValve;
            _cmdExector = cmdExector;
            _mapNo = mapNo;

            // 实例化左右阀，将执行器传递给它们，实现组件自治
            LeftValve = new ValveGlueMonitorViewModel("左阀", 0, _cmdExector, _mapNo);
            RightValve = new ValveGlueMonitorViewModel(isDoubleValve ? "右阀" : "阀", 1, _cmdExector, _mapNo);

            SaveCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                await SaveParamsAsync();
                CloseAction?.Invoke(true);
            });

            CloseCommand = ReactiveCommand.Create(() =>
            {
                CloseAction?.Invoke(false);
            });
        }

        public async Task InitDataAsync()
        {
            // TODO: 调用 _cmdExector 拉取后台数据
            // 这里用模拟数据演示

            // 模拟左阀配置
            LeftValve.IsTimeModeEnabled = true;
            LeftValve.TargetTimeText = "30";
            LeftValve.IsWeightPercentMode = true; // 百分比模式
            LeftValve.TargetWeightPercentText = "15";
            LeftValve.IsPcsModeEnabled = false;

            // 模拟右阀配置
            RightValve.IsTimeModeEnabled = false;
            RightValve.IsWeightModeEnabled = true;
            RightValve.IsWeightPercentMode = false; // 绝对值报警模式
            RightValve.TargetWeightAlarmText = "50.5";
            RightValve.IsPcsModeEnabled = true;
            RightValve.TargetPcsText = "5000";

            // 模拟当前机器状态 (比如左阀时间耗尽报警)
            LeftValve.UpdateCurrentStatus(actualTime: 0, actualWeight: 100, actualPcs: 0);
            RightValve.UpdateCurrentStatus(actualTime: 0, actualWeight: 55.2, actualPcs: 1200);
        }

        private async Task SaveParamsAsync()
        {
            try
            {
                // TODO: 将 LeftValve 和 RightValve 的属性打包成 GFBaseTypeParamValueList
                // 调用 _cmdExector 存储
                System.Diagnostics.Debug.WriteLine("胶水监控参数保存完毕！");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"保存失败: {ex.Message}");
            }
        }
    }
}