using Avalonia.Controls;
using Avalonia.ReactiveUI;
using GKG.Map.AuxiliaryFunctionsFuncCtlMapCell.ViewModel;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Disposables;

namespace GKG.Map.AuxiliaryFunctionsFuncCtlMapCell.View
{
    public partial class AuxiliaryFunctionsView : ReactiveUserControl<AuxiliaryFunctionsViewModel>
    {
    	public AuxiliaryFunctionsView()
    	{
    		InitializeComponent();

    		this.WhenActivated(disposables =>
    		{
    			if (ViewModel == null) return;

                #region 命令绑定

                // 第一行绑定
                this.BindCommand(ViewModel, vm => vm.OneKeyTransferGlueCommand, v => v.OneKeyTransferGlueButton)
                    .DisposeWith(disposables);
                this.BindCommand(ViewModel, vm => vm.OneKeyTransferGlueSetCommand, v => v.OneKeyTransferGlueSetButton)
                    .DisposeWith(disposables);
                this.BindCommand(ViewModel, vm => vm.TransferGluePosCommand, v => v.TransferGluePosButton)
                    .DisposeWith(disposables);
                this.BindCommand(ViewModel, vm => vm.LeftValveOutGlueCommand, v => v.LeftValveOutGlueButton)
                    .DisposeWith(disposables);
                this.BindCommand(ViewModel, vm => vm.RightValveOutGlueCommand, v => v.RightValveOutGlueButton)
                    .DisposeWith(disposables);

                // 第二行绑定
                this.BindCommand(ViewModel, vm => vm.OneKeyMachineCalibrationCommand, v => v.OneKeyMachineCalibrationButton)
                    .DisposeWith(disposables);
                this.BindCommand(ViewModel, vm => vm.OneKeyMachineCalibrationSetCommand, v => v.OneKeyMachineCalibrationSetButton)
                    .DisposeWith(disposables);
                this.BindCommand(ViewModel, vm => vm.GlueTimerManageCommand, v => v.GlueTimerManageButton)
                    .DisposeWith(disposables);

                // 第三行绑定
                this.BindCommand(ViewModel, vm => vm.OneKeyOutGlueCommand, v => v.OneKeyOutGlueButton)
                    .DisposeWith(disposables);
                this.BindCommand(ViewModel, vm => vm.OneKeyOutGlueSetCommand, v => v.OneKeyOutGlueSetButton)
                    .DisposeWith(disposables);
                this.BindCommand(ViewModel, vm => vm.TransferValvePosCommand, v => v.TransferValvePosButton)
                    .DisposeWith(disposables);
                this.BindCommand(ViewModel, vm => vm.ValveAlarmResetCommand, v => v.ValveAlarmResetButton)
                    .DisposeWith(disposables);

                // 第四行绑定
                this.BindCommand(ViewModel, vm => vm.OneKeyOutWaxCommand, v => v.OneKeyOutWaxButton)
                    .DisposeWith(disposables);
                this.BindCommand(ViewModel, vm => vm.OneKeyOutWaxSetCommand, v => v.OneKeyOutWaxSetButton)
                    .DisposeWith(disposables);

                #endregion

                #region 交互绑定

                // ==========================================
                // 注册 ：等待框 弹窗响应
                // ==========================================
                this.BindInteraction(ViewModel,
                    vm => vm.ShowWaitingBoxDialog,
                    async interaction =>
                    {
                        // 获取上下文数据 (Message 和 WorkTask)
                        var context = interaction.Input;

                        // 获取当前的主窗口作为弹窗的父级 (Owner)
                        // 如果 MainView 本身是 Window，可以直接传 this
                        // 如果 MainView 是 UserControl，则通过 VisualRoot 获取顶层 Window
                        var mainWindow = (Window)this.VisualRoot;

                        // 调用我们在上一节写好的静态弹窗方法
                        await WaitingBoxView.ShowAsync(mainWindow, context.WorkTask, context.Message);

                        // 弹窗关闭后，设置交互完成的回调 (Unit.Default 相当于 void 返回)
                        interaction.SetOutput(Unit.Default);

                    }).DisposeWith(disposables);

                // ==========================================
                // 注册 ：余量监控 弹窗响应
                // ==========================================
                this.BindInteraction(ViewModel,
                    vm => vm.ShowGlueMonitorDialog,
                    async interaction =>
                    {
                        // 拿到 ViewModel 给我们的数据模型
                        var monitorVm = interaction.Input;
                        var mainWindow = (Window)this.VisualRoot;

                        // 实例化窗口，并将其 ViewModel 绑定
                        var dialog = new GlueMonitorView
                        {
                            ViewModel = monitorVm // 绑定上下文
                        };

                        // 以模态框形式弹出，并等待它关闭
                        await dialog.ShowDialog(mainWindow);

                        // 窗口关闭后，通知 MainViewModel 继续往下走
                        interaction.SetOutput(Unit.Default);
                    }).DisposeWith(disposables);

                // ==========================================
                // 注册 ：一键校正 弹窗响应
                // ==========================================
                this.BindInteraction(ViewModel,
                    vm => vm.ShowMachineCalibrationDialog,
                    async interaction =>
                    {
                        // 拿到 ViewModel 准备好的数据模型
                        var calibrationVm = interaction.Input;
                        var mainWindow = (Window)this.VisualRoot;

                        // 实例化我们重构后的向导窗口
                        var dialog = new MachineCalibrationView
                        {
                            DataContext = calibrationVm // 注入上下文驱动界面
                        };

                        // 以模态框形式弹出，阻塞当前 UI 操作，直到窗口关闭
                        await dialog.ShowDialog(mainWindow);

                        // 窗口关闭后，通知 ViewModel 可以继续执行后面的代码了
                        interaction.SetOutput(Unit.Default);

                    }).DisposeWith(disposables);

                // ==========================================
                // 注册 ：一键排胶 的 TipShow 弹窗响应
                // ==========================================
                this.BindInteraction(ViewModel,
                    vm => vm.ShowTipDialog,
                    async interaction =>
                    {
                        var tipVm = interaction.Input;
                        var window = (Window)this.VisualRoot;

                        // 实例化我们上一问重构好的 Avalonia 纯净版弹窗
                        var dialog = new TipShowWindowView
                        {
                            DataContext = tipVm
                        };

                        // 模态弹出，阻塞直到关闭
                        await dialog.ShowDialog(window);

                        interaction.SetOutput(Unit.Default);
                    }).DisposeWith(disposables);

                // ==========================================
                // 注册：排胶/排蜡设置 弹窗响应
                // ==========================================
                this.BindInteraction(ViewModel,
                    vm => vm.ShowOutGlueWaxSetDialog,
                    async interaction =>
                    {
                        var setVm = interaction.Input;
                        var mainWindow = (Window)this.VisualRoot;

                        // 实例化我们刚刚重构好的设置窗口
                        var dialog = new OutGlueWaxSetWindowView
                        {
                            DataContext = setVm
                        };

                        // 模态弹出，阻塞直到用户点击保存或取消关闭窗口
                        await dialog.ShowDialog(mainWindow);

                        // 标记交互完成
                        interaction.SetOutput(System.Reactive.Unit.Default);
                    }).DisposeWith(disposables);

                // ==========================================
                // 注册：一键校正设置 弹窗响应
                // ==========================================
                this.BindInteraction(ViewModel,
                    vm => vm.ShowMachineCalibrationSetDialog,
                    async interaction =>
                    {
                        var setVm = interaction.Input;
                        var mainWindow = (Window)this.VisualRoot;
                        // 实例化我们刚刚重构好的设置窗口
                        var dialog = new MachineCalibrationSetView
                        {
                            DataContext = setVm
                        };
                        // 模态弹出，阻塞直到用户点击保存或取消关闭窗口
                        await dialog.ShowDialog(mainWindow);
                        // 标记交互完成
                        interaction.SetOutput(System.Reactive.Unit.Default);
                     }).DisposeWith(disposables);

                // ==========================================
                // 注册：胶水定时管理 弹窗响应
                // ==========================================
                this.BindInteraction(ViewModel,
                    vm => vm.ShowGlueTimerManageDialog,
                    async interaction =>
                    {
                        var setVm = interaction.Input;
                        var mainWindow = (Window)this.VisualRoot;
                        // 实例化我们刚刚重构好的设置窗口
                        var dialog = new GlueTimerManageDialogView
                        {
                            DataContext = setVm
                        };
                        // 模态弹出，阻塞直到用户点击保存或取消关闭窗口
                        await dialog.ShowDialog(mainWindow);
                        // 标记交互完成
                        interaction.SetOutput(System.Reactive.Unit.Default);
                     }).DisposeWith(disposables);

                // ==========================================
                // 注册：一键换胶设置 弹窗响应
                // ==========================================
                this.BindInteraction(ViewModel,
                    vm => vm.ShowGlueMoniterSetDialog,
                    async interaction =>
                    {
                        var setVm = interaction.Input;
                        var mainWindow = (Window)this.VisualRoot;
                        // 实例化我们刚刚重构好的设置窗口
                        var dialog = new GlueMonitorSetView
                        {
                            DataContext = setVm
                        };
                        // 模态弹出，阻塞直到用户点击保存或取消关闭窗口
                        await dialog.ShowDialog(mainWindow);
                        // 标记交互完成
                        interaction.SetOutput(System.Reactive.Unit.Default);
                     }).DisposeWith(disposables);

                #endregion
            });
    	}

	
    }
}