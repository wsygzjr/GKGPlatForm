using Griffins.CompUI.SL.ComUI_SL.InitCfgPage.Models;
using Griffins.CompUI.SL.InitCfgPage.Models;
using Griffins.Map;
using Griffins.Map.UI;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Reactive;

namespace Griffins.CompUI.SL.InitCfgPage.ViewModels
{
    public class TransportMotorCompUIViewModel : ReactiveObject, IDisposable
    {
        #region 私有字段
        private bool isDesign;

        private ICompUIRunTimeCallBack callBack;

        private const int MOTOR_COUNT = 2;
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

        private bool _readOnly;
        /// <summary>
        /// 只读
        /// </summary>
        public bool ReadOnly
        {
            get => _readOnly;
            set => this.RaiseAndSetIfChanged(ref _readOnly, value);
        }

        private ObservableCollection<TransportMotorItemCompUIViewModel> _motors;
        /// <summary>
        /// 单层轨道电机列表
        /// </summary>
        public ObservableCollection<TransportMotorItemCompUIViewModel> Motors
        {
            get => _motors;
            set => this.RaiseAndSetIfChanged(ref _motors, value);
        }
        #endregion

        #region 命令
        public ReactiveCommand<Unit, Unit> ButtonClickCommand { get; }
        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数（初始化）
        /// </summary>
        public TransportMotorCompUIViewModel(bool isDesign, ICompUIRunTimeCallBack callBack)
        {
            this.isDesign = isDesign;
            this.callBack = callBack;

            Motors = new ObservableCollection<TransportMotorItemCompUIViewModel>();
            InitializeMotors();
            ButtonClickCommand = ReactiveCommand.Create(OnButtonClicked);
        }
        #endregion

        #region 公共方法
        /// <summary>
        /// 设置数据
        /// </summary>
        public void SetData(TransportMotorCompUIModel model)
        {
            if (model?.Motors == null)
            {
                return;
            }

            // 遍历固定的电机列表，从数据模型中加载对应的数据
            for (int i = 0; i < Motors.Count; i++)
            {
                var motorViewModel = Motors[i];

                // 如果模型中有对应的数据，则进行赋值
                if (i < model.Motors.Count)
                {
                    var motorModel = model.Motors[i];
                    motorViewModel.FrontSelectedItem = motorModel.FrontSelectedItem;
                    motorViewModel.BehindSelectedItem = motorModel.BehindSelectedItem;
                }
            }
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        public TransportMotorCompUIModel GetData()
        {
            var model = new TransportMotorCompUIModel();

            foreach (var motorViewModel in Motors)
            {
                var motorModel = new TransportMotorItemCompUIModel
                {
                    FrontSelectedItem = motorViewModel.FrontSelectedItem,
                    BehindSelectedItem = motorViewModel.BehindSelectedItem
                };

                model.Motors.Add(motorModel);
            }
            return model;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (Motors != null)
            {
                foreach (var motor in Motors)
                {
                    motor.Dispose();
                }
                Motors.Clear();
            }
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
            string alias = ViewTag != null ? ViewTag.ToString() : string.Empty;
            if (!string.IsNullOrWhiteSpace(alias))
            {
                string response = this.callBack.ExecConfigSvrCtlCmd("TransportMotor", null);
            }
        }

        /// <summary>
        /// 初始化固定数量的电机
        /// </summary>
        private void InitializeMotors()
        {
            for (int i = 0; i < MOTOR_COUNT; i++)
            {
                var motorViewModel = new TransportMotorItemCompUIViewModel(isDesign, callBack);
                motorViewModel.MotorName = $"电机{i + 1}";
                Motors.Add(motorViewModel);
            }
        }
        #endregion
    }
}