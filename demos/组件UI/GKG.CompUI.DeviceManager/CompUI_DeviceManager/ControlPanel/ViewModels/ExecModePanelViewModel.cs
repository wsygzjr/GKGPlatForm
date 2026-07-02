using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Griffins.Map.UI;
using Griffins;
using Griffins.ImeIOT;
using GKG.MM;

namespace GKG.CompUI.DeviceManager.ControlPanel.ViewModels
{
    /// <summary>
    /// 执行模式控制面板视图模型
    /// </summary>
    public class ExecModePanelViewModel : ReactiveObject
    {
        // 资源根路径
        private const string AssetRoot = "avares://GKG.CompUI.DeviceManager/Assets/Icons";

        // 关闭请求事件流
        private readonly Subject<Unit> _closeRequested = new();
        // 控制面板回调接口
        private readonly IControlPanelCallBack _controlPanelCallBack;

        // 关闭请求事件
        public IObservable<Unit> CloseRequested => _closeRequested;

        // 模式卡片集合
        public ObservableCollection<ExecModeCardViewModel> ModeCards { get; } = new();

        // 命令定义
        public ReactiveCommand<ExecModeCardViewModel, Unit> SelectModeCommand { get; }
        public ReactiveCommand<Unit, Unit> NextStepCommand { get; }
        public ReactiveCommand<Unit, Unit> CloseCommand { get; }
        public ReactiveCommand<Unit, Unit> StartCommand { get; }
        public ReactiveCommand<Unit, Unit> StopCommand { get; }

        // 是否已经开始执行
        private bool _isStarted;
        public bool IsStarted
        {
            get => _isStarted;
            private set => this.RaiseAndSetIfChanged(ref _isStarted, value);
        }

        // 是否已经选择了模式
        private bool _isModeSelected;
        public bool IsModeSelected
        {
            get => _isModeSelected;
            private set => this.RaiseAndSetIfChanged(ref _isModeSelected, value);
        }

        public ExecModePanelViewModel(IControlPanelCallBack controlPanelCallBack)
        {
            _controlPanelCallBack = controlPanelCallBack;

            // 初始化模式卡片
            ModeCards.Add(new ExecModeCardViewModel(
                ImeExecMode.SingleStep,
                "单步执行",
                $"{AssetRoot}/单步执行01.png",
                $"{AssetRoot}/单步执行02.png",
                "#EAF4FF",
                "#3B82F6",
                "#3B82F6"));

            ModeCards.Add(new ExecModeCardViewModel(
                ImeExecMode.SingleCycle,
                "单工艺流程周期执行",
                $"{AssetRoot}/单工艺流程周期执行01.png",
                $"{AssetRoot}/单工艺流程周期执行02.png",
                "#FFF1EE",
                "#FF7A64",
                "#FF7A64"));

            // 初始化命令
            SelectModeCommand = ReactiveCommand.Create<ExecModeCardViewModel>(SelectMode);
            NextStepCommand = ReactiveCommand.Create(ExecuteNextStep, this.WhenAnyValue(x => x.IsStarted));
            CloseCommand = ReactiveCommand.Create(ExecuteClose);
            StartCommand = ReactiveCommand.Create(ExecuteStart, this.WhenAnyValue(x => x.IsModeSelected));
            StopCommand = ReactiveCommand.Create(ExecuteStop, this.WhenAnyValue(x => x.IsStarted));
        }

        // 选择执行模式
        private void SelectMode(ExecModeCardViewModel card)
        {
            foreach (var modeCard in ModeCards)
            {
                modeCard.IsSelected = modeCard == card;
            }

            // 标记已选择模式
            IsModeSelected = true;

            // 调用后端设置执行模式
            try
            {
                var cmdParam = new GFBaseTypeParamValueList
                {
                    new GFBaseTypeParamValue("ExecMode", new GriffinsBaseValue((int)card.Mode))
                };
                _controlPanelCallBack.ExecNormalCtlCmd(DeviceManagerMachineModulesConst.SetExecMode, cmdParam);
            }
            catch
            {
            }
        }

        // 执行下一步
        private void ExecuteNextStep()
        {
            try
            {
                _controlPanelCallBack.ExecNormalCtlCmd(DeviceManagerMachineModulesConst.NextStep, new GFBaseTypeParamValueList());
            }
            catch
            {
            }
        }

        // 开始执行
        private void ExecuteStart()
        {
            IsStarted = true;
            // 禁用未选中的模式卡片
            foreach (var modeCard in ModeCards)
            {
                modeCard.IsEnabled = modeCard.IsSelected;
            }

            // 调用后端开始工作
            try
            {
                _controlPanelCallBack.ExecNormalCtlCmd(SvrForMMProcessCmd.CmdID_StartWork, new GFBaseTypeParamValueList());
            }
            catch
            {
            }
        }

        // 停止执行
        private void ExecuteStop()
        {
            IsStarted = false;
            // 启用所有模式卡片
            foreach (var modeCard in ModeCards)
            {
                modeCard.IsEnabled = true;
            }

            // 调用后端停止工作
            try
            {
                _controlPanelCallBack.ExecNormalCtlCmd(SvrForMMProcessCmd.CmdID_StopWork, new GFBaseTypeParamValueList());
            }
            catch
            {
            }
        }

        // 关闭面板
        private void ExecuteClose()
        {
            // 将执行模式设置为连续执行
            try
            {
                var cmdParam = new GFBaseTypeParamValueList
                {
                    new GFBaseTypeParamValue("ExecMode", new GriffinsBaseValue((int)ImeExecMode.Continuous))
                };
                _controlPanelCallBack.ExecNormalCtlCmd(DeviceManagerMachineModulesConst.SetExecMode, cmdParam);
            }
            catch
            {
            }

            // 触发关闭请求
            _closeRequested.OnNext(Unit.Default);
        }
    }
}
