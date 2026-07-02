using Avalonia;
using Avalonia.Media;
using GKG.Map.AuxiliaryFunctionsFuncCtlMapCell.Model;
using Griffins;
using Griffins.Map.Cmd;
using Griffins.Map.UI;
using Griffins.PF;
using Griffins.UI2;
using ReactiveUI;
using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace GKG.Map.AuxiliaryFunctionsFuncCtlMapCell.ViewModel
{
    public class AuxiliaryFunctionsViewModel : ReactiveObject, IDisposable
    {
        private readonly string _mapNo;
        private readonly Func<MapCmdExector> _getCmdExector;

        #region 硬件与流程配置属性

        private bool _isDoubleValve;
        public bool IsDoubleValve
        {
            get => _isDoubleValve;
            set => this.RaiseAndSetIfChanged(ref _isDoubleValve, value);
        }

        private bool _hasOutGlue = true;
        public bool HasOutGlue
        {
            get => _hasOutGlue;
            set => this.RaiseAndSetIfChanged(ref _hasOutGlue, value);
        }

        private bool _hasLaser = true;
        public bool HasLaser
        {
            get => _hasLaser;
            set => this.RaiseAndSetIfChanged(ref _hasLaser, value);
        }

        private bool _hasPreciseTeach = true;
        public bool HasPreciseTeach
        {
            get => _hasPreciseTeach;
            set => this.RaiseAndSetIfChanged(ref _hasPreciseTeach, value);
        }

        private bool _hasObliqueTeach = true;
        public bool HasObliqueTeach
        {
            get => _hasObliqueTeach;
            set => this.RaiseAndSetIfChanged(ref _hasObliqueTeach, value);
        }

        private bool _hasOnePointWeight = true;
        public bool HasOnePointWeight
        {
            get => _hasOnePointWeight;
            set => this.RaiseAndSetIfChanged(ref _hasOnePointWeight, value);
        }

        #endregion

        #region 按钮绑定命令定义

        // 第一行 Commands
        public ReactiveCommand<Unit, Unit> OneKeyTransferGlueCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> OneKeyTransferGlueSetCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> TransferGluePosCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> LeftValveOutGlueCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> RightValveOutGlueCommand { get; private set; }

        // 第二行 Commands
        public ReactiveCommand<Unit, Unit> OneKeyMachineCalibrationCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> OneKeyMachineCalibrationSetCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> GlueTimerManageCommand { get; private set; }

        // 第三行 Commands
        public ReactiveCommand<Unit, Unit> OneKeyOutGlueCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> OneKeyOutGlueSetCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> TransferValvePosCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> ValveAlarmResetCommand { get; private set; }

        // 第四行 Commands
        public ReactiveCommand<Unit, Unit> OneKeyOutWaxCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> OneKeyOutWaxSetCommand { get; private set; }

        #endregion

        #region 交互通道定义

        // 声明一个用于触发弹窗的 Interaction 交互通道
        public Interaction<WaitingBoxContext, Unit> ShowWaitingBoxDialog { get; }
        // 余量监控窗口的交互通道
        public Interaction<GlueMonitorViewModel, Unit> ShowGlueMonitorDialog { get; }
        // 一键校正窗口的交互通道
        public Interaction<MachineCalibrationViewModel, Unit> ShowMachineCalibrationDialog { get; }
        // TipShow 提示窗的交互通道
        public Interaction<TipShowWindowViewModel, Unit> ShowTipDialog { get; }
        // 排胶排蜡设置窗口的交互通道
        public Interaction<OutGlueWaxSetViewModel, Unit> ShowOutGlueWaxSetDialog {  get; }
        // 一键校正设置窗口的交互通道
        public Interaction<MachineCalibrationSetViewModel, Unit> ShowMachineCalibrationSetDialog { get; }
        // 胶水定时器管理窗口的交互通道
        public Interaction<GlueTimerManageViewModel, Unit> ShowGlueTimerManageDialog { get; }
        // 一键换胶设置窗口的交互通道
        public Interaction<GlueMonitorSetViewModel, Unit> ShowGlueMoniterSetDialog { get; }

        #endregion

        public AuxiliaryFunctionsViewModel(AuxiliaryFunctionsPropertyModelEdit pressurePropertyModelEdit, Func<MapCmdExector> getCmdExector, string mapNo)
        {
            _mapNo = mapNo;
            _getCmdExector = getCmdExector;
            IsDoubleValve = true;

            // 初始化交互通道
            ShowWaitingBoxDialog = new Interaction<WaitingBoxContext, Unit>();
            ShowGlueMonitorDialog = new Interaction<GlueMonitorViewModel, Unit>();
            ShowMachineCalibrationDialog = new Interaction<MachineCalibrationViewModel, Unit>();
            ShowTipDialog = new Interaction<TipShowWindowViewModel, Unit>();
            ShowOutGlueWaxSetDialog = new Interaction<OutGlueWaxSetViewModel, Unit>();
            ShowMachineCalibrationSetDialog = new Interaction<MachineCalibrationSetViewModel, Unit>();
            ShowGlueTimerManageDialog = new Interaction<GlueTimerManageViewModel, Unit>();
            ShowGlueMoniterSetDialog = new Interaction<GlueMonitorSetViewModel, Unit>();

            // 在构造函数中调用初始化方法
            InitializeCommands();
        }

        /// <summary>
        /// 初始化命令
        /// </summary>
        private void InitializeCommands()
        {
            // ==========================================
            // 一键换胶业务流
            // ==========================================
            OneKeyTransferGlueCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                // ================= 第一步：机器移动等待 =================
                var waitContext = new WaitingBoxContext
                {
                    Message = "正在移动到换胶位置，请稍候...",
                    WorkTask = async () =>
                    {
                        try
                        {
                            //await Task.Run(() =>
                            //{
                            //    GFBaseTypeParamValueList result = _getCmdExector().ExecUIDataObjCommand(_mapNo, "MoveToGluePos", null);
                            //});
                            await Task.Delay(1000);

                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"执行 MoveToValvePos 失败: {ex.Message}");
                            throw;
                        }
                    }
                };
                await ShowWaitingBoxDialog.Handle(waitContext);


                // ================= 第二步：弹出余量监控 =================
                var glueMonitorVm = new GlueMonitorViewModel(this.IsDoubleValve);
                await ShowGlueMonitorDialog.Handle(glueMonitorVm);


                // ================= 第三步：呼出一键校正向导 =================
                // 此时胶水已经换新，需要自动或者半自动地校正机器状态
                var config = new CalibrationConfig
                {
                    HasOutGlue = this.HasOutGlue,
                    HasLaser = this.HasLaser,
                    HasPreciseTeach = this.HasPreciseTeach,
                    HasObliqueTeach = this.HasObliqueTeach,
                    HasOnePointWeight = this.HasOnePointWeight
                };
                var calibrationVm = new MachineCalibrationViewModel(this.IsDoubleValve, config);
                await ShowMachineCalibrationDialog.Handle(calibrationVm);
            });
            OneKeyTransferGlueSetCommand = ReactiveCommand.CreateFromTask(async () => 
            {
                    var glueMoniterSetVm = new GlueMonitorSetViewModel(IsDoubleValve, _getCmdExector(), _mapNo);
                    await ShowGlueMoniterSetDialog.Handle(glueMoniterSetVm);
            });
            TransferGluePosCommand = ReactiveCommand.CreateFromTask(async () => 
            {
                var waitContext = new WaitingBoxContext
                {
                    Message = "正在移动到换胶位置，请稍候...",
                    WorkTask = async () =>
                    {
                        try
                        {
                            //await Task.Run(() =>
                            //{
                            //    GFBaseTypeParamValueList result = _getCmdExector().ExecUIDataObjCommand(_mapNo, "MoveToGluePos", null);
                            //});
                            await Task.Delay(1000);

                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"执行 MoveToValvePos 失败: {ex.Message}");
                            throw;
                        }
                    }
                };
                await ShowWaitingBoxDialog.Handle(waitContext);
            });
            LeftValveOutGlueCommand = ReactiveCommand.CreateFromTask(async () => 
            {
                var waitContext = new WaitingBoxContext
                {
                    Message = "正在左阀排胶，请稍候...",
                    WorkTask = async () =>
                    {
                        try
                        {
                            //await Task.Run(() =>
                            //{
                            //    GFBaseTypeParamValueList result = _getCmdExector().ExecUIDataObjCommand(_mapNo, "ManualOutGlueLeftValve", null);
                            //});
                            await Task.Delay(1000);

                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"执行 ManualOutGlueLeftValve 失败: {ex.Message}");
                            throw;
                        }
                    }
                };
                await ShowWaitingBoxDialog.Handle(waitContext);
               
            });
            RightValveOutGlueCommand = ReactiveCommand.CreateFromTask(async () => 
            {
                var waitContext = new WaitingBoxContext
                {
                    Message = "正在右阀排胶，请稍候...",
                    WorkTask = async () =>
                    {
                        try
                        {
                            //await Task.Run(() =>
                            //{
                            //    GFBaseTypeParamValueList result = _getCmdExector().ExecUIDataObjCommand(_mapNo, "ManualOutGlueRightValve", null);
                            //});
                            await Task.Delay(1000);

                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"执行 ManualOutGlueRightValve 失败: {ex.Message}");
                            throw;
                        }
                    }
                };
                await ShowWaitingBoxDialog.Handle(waitContext);
               
            });

            // ==========================================
            // 一键校正业务流
            // ==========================================
            OneKeyMachineCalibrationCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var config = new CalibrationConfig
                {
                    HasOutGlue = this.HasOutGlue,
                    HasLaser = this.HasLaser,
                    HasPreciseTeach = this.HasPreciseTeach,
                    HasObliqueTeach = this.HasObliqueTeach,
                    HasOnePointWeight = this.HasOnePointWeight
                };

                var calibrationVm = new MachineCalibrationViewModel(this.IsDoubleValve, config);
                await ShowMachineCalibrationDialog.Handle(calibrationVm);
            });
            OneKeyMachineCalibrationSetCommand = ReactiveCommand.CreateFromTask(async () => 
            {
                var setVm = new MachineCalibrationSetViewModel(this.IsDoubleValve, _getCmdExector(), _mapNo);
                await ShowMachineCalibrationSetDialog.Handle(setVm);
            });
            GlueTimerManageCommand = ReactiveCommand.CreateFromTask(async () => 
            {
                var glueTimerManageVm = new GlueTimerManageViewModel(_getCmdExector(), _mapNo);
                await ShowGlueTimerManageDialog.Handle(glueTimerManageVm);
            });

            // ==========================================
            // 一键排胶业务流
            // ==========================================
            OneKeyOutGlueCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                // 1. 发令：调用插件的排胶开始指令
                try
                {
                    GFBaseTypeParamValueList result = _getCmdExector().ExecUIDataObjCommand(_mapNo, "StartPurge", null);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"执行 StartPurge 失败: {ex.Message}");
                }

                // 2. 准备回调：如果操作员在弹窗上点了“取消”，应该通知插件停止排胶
                Action cancelAction = () =>
                {
                    try
                    {
                        GFBaseTypeParamValueList result = _getCmdExector().ExecUIDataObjCommand(_mapNo, "StopPurge", null);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"执行 StopPurge 失败: {ex.Message}");
                    }
                };

                // 3. 准备 UI 数据：实例化提示窗的 ViewModel
                var tipVm = new TipShowWindowViewModel(OneKeyType.OutGlue, cancelAction);

                // 4. 呼出 UI：通过 Interaction 让 View 层弹出提示框
                await ShowTipDialog.Handle(tipVm);

            });
            OneKeyOutGlueSetCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                // 传入 true 代表排胶模式
                var vm = new OutGlueWaxSetViewModel(isOutGlue: true, _getCmdExector(), _mapNo);
                await ShowOutGlueWaxSetDialog.Handle(vm);

                // 当设置窗口关闭后，如果需要刷新主界面的某些数据，可以在这里写逻辑
            });
            TransferValvePosCommand = ReactiveCommand.CreateFromTask(async () => 
            {
                var waitContext = new WaitingBoxContext
                {
                    Message = "正在移动到换阀位置，请稍候...",
                    WorkTask = async () => 
                    {
                        try
                        {
                            await Task.Run(() =>
                            {
                                GFBaseTypeParamValueList result = _getCmdExector().ExecUIDataObjCommand(_mapNo, "MoveToValvePos", null);
                            });
                            
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"执行 MoveToValvePos 失败: {ex.Message}");
                            throw;
                        }
                    }
                };
                await ShowWaitingBoxDialog.Handle(waitContext);
            });
            ValveAlarmResetCommand = ReactiveCommand.CreateFromTask(async () => 
            {
                var waitContext = new WaitingBoxContext
                {
                    WorkTask = async () =>
                    {
                        try
                        {
                            await Task.Run(() =>
                            {
                                GFBaseTypeParamValueList result = _getCmdExector().ExecUIDataObjCommand(_mapNo, "SetValveSingleData", new GFBaseTypeParamValueList
                                {
                                    new GFBaseTypeParamValue("ValveId", new GriffinsBaseValue(0))//0:主阀, 1:副阀
                                });
                            });

                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"执行 SetValveSingleData 失败: {ex.Message}");
                            throw;
                        }
                    }
                };
                await ShowWaitingBoxDialog.Handle(waitContext);
            });

            // ==========================================
            // 一键排蜡业务流
            // ==========================================
            OneKeyOutWaxCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                // 1. 发令：调用插件的排蜡开始指令
                try
                {
                    GFBaseTypeParamValueList result = _getCmdExector().ExecUIDataObjCommand(_mapNo, "StartOutWax", null);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"执行 StartOutWax 失败: {ex.Message}");
                }

                // 2. 准备回调：如果操作员在弹窗上点了“取消”，应该通知插件停止排蜡
                Action cancelAction = () =>
                {
                    try
                    {
                        GFBaseTypeParamValueList result = _getCmdExector().ExecUIDataObjCommand(_mapNo, "StopOutWax", null);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"执行 StopOutWax 失败: {ex.Message}");
                    }
                };

                // 3. 准备 UI 数据：实例化提示窗的 ViewModel
                var tipVm = new TipShowWindowViewModel(OneKeyType.OutWax, cancelAction);

                // 4. 呼出 UI：通过 Interaction 让 View 层弹出提示框
                await ShowTipDialog.Handle(tipVm);
            }); 
            OneKeyOutWaxSetCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                // 传入 false 代表排蜡模式
                var vm = new OutGlueWaxSetViewModel(isOutGlue: false, _getCmdExector(), _mapNo);
                await ShowOutGlueWaxSetDialog.Handle(vm);
            });
        }


        public void Dispose()
        {

        }
    }

    public class WaitingBoxContext
    {
        public string Message { get; set; }
        public Func<Task> WorkTask { get; set; }
    }
}
