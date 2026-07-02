using System;
using System.Collections.ObjectModel;
using System.Reactive;
using Griffins.CompUI.SL.InitCfgPage.Models;
using Griffins.Map;
using Griffins.Map.UI;
using ReactiveUI;

namespace Griffins.CompUI.SL.InitCfgPage.ViewModels
{
    public class TransportMotorItemCompUIViewModel : ReactiveObject, IDisposable
    {
        #region 私有字段
        private bool isDesign;

        private ICompUIRunTimeCallBack callBack;
        #endregion

        #region 响应式属性
        private object _viewTag;
        /// <summary>
        /// 对应View的Tag属性（支持双向绑定）
        /// </summary>
        public object ViewTag
        {
            get => _viewTag;
            set => this.RaiseAndSetIfChanged(ref _viewTag, value);
        }

        private ObservableCollection<OperationModeItem> _frontTrackCombo;
        /// <summary>
        /// 前边轨-轨道运转模式下拉框数据源
        /// </summary>
        public ObservableCollection<OperationModeItem> FrontTrackCombo
        {
            get => _frontTrackCombo;
            set => this.RaiseAndSetIfChanged(ref _frontTrackCombo, value);
        }

        private ObservableCollection<OperationModeItem> _behindTrackCombo;
        /// <summary>
        /// 后边轨-轨道运转模式下拉框数据源
        /// </summary>
        public ObservableCollection<OperationModeItem> BehindTrackCombo
        {
            get => _behindTrackCombo;
            set => this.RaiseAndSetIfChanged(ref _behindTrackCombo, value);
        }

        private OperationModeItem _frontSelectedItem;
        /// <summary>
        /// 前边轨-轨道运转模式下拉框被选中的项
        /// </summary>
        public OperationModeItem FrontSelectedItem
        {
            get => _frontSelectedItem;
            set => this.RaiseAndSetIfChanged(ref _frontSelectedItem, value);
        }

        private OperationModeItem _behindSelectedItem;
        /// <summary>
        /// 后边轨-轨道运转模式下拉框被选中的项
        /// </summary>
        public OperationModeItem BehindSelectedItem
        {
            get => _behindSelectedItem;
            set => this.RaiseAndSetIfChanged(ref _behindSelectedItem, value);
        }

        private bool _readOnly;
        /// <summary>
        /// 只读
        /// </summary>
        public bool ReadOnly
        {
            get => _readOnly;
            set => this.RaiseAndSetIfChanged(ref _readOnly, value);
        }

        private string _motorName;
        /// <summary>
        /// 电机名称
        /// </summary>
        public string MotorName
        {
            get => _motorName;
            set => this.RaiseAndSetIfChanged(ref _motorName, value);
        }
        #endregion

        #region 命令
        public ReactiveCommand<Unit, Unit> ButtonClickCommand { get; }
        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数（初始化）
        /// </summary>
        public TransportMotorItemCompUIViewModel(bool isDesign, ICompUIRunTimeCallBack callBack)
        {
            this.isDesign = isDesign;
            this.callBack = callBack;

            //前边轨下拉框初始化
            FrontTrackCombo = new ObservableCollection<OperationModeItem>()
            {
                new(){DisPlayName="自动", Type=OperationModeType.Automatic },
                new(){DisPlayName="过板", Type=OperationModeType.OverBoard },
                new(){DisPlayName="手动", Type=OperationModeType.Manual },
                new(){DisPlayName="老化", Type=OperationModeType.Aging }
            };
            FrontSelectedItem = FrontTrackCombo[0];

            //后边轨下拉框初始化
            BehindTrackCombo = new ObservableCollection<OperationModeItem>()
            {
                new(){DisPlayName="自动", Type=OperationModeType.Automatic },
                new(){DisPlayName="过板", Type=OperationModeType.OverBoard },
                new(){DisPlayName="手动", Type=OperationModeType.Manual },
                new(){DisPlayName="老化", Type=OperationModeType.Aging }
            };
            BehindSelectedItem = BehindTrackCombo[0];

            ButtonClickCommand = ReactiveCommand.Create(OnButtonClicked);
        }
        #endregion

        #region 公共方法
        public void Dispose()
        {

        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 按钮点击事件
        /// </summary>
        private void OnButtonClicked()
        {
            if (isDesign)
            {
                return;
            }
            var response = this.callBack.ExecConfigSvrCtlCmd("TransportMotor", null);
        }
        #endregion
    }
}