using Griffins.UI;
using Griffins.CompUI.ElectricalMngObj.FactoryCfgPage.Models;
using ReactiveUI;
using System;
using System.Linq;
using System.Collections.Generic;
namespace Griffins.CompUI.ElectricalMngObj.FactoryCfgPage.ViewModels.Axis
{
    /// <summary>
    /// 回零参数-视图模型
    /// </summary>
    public class ReturnToZeroParamViewModel : ReactiveObject
    {
        private double _returnToZeroAccTime;
        private double _returnToZeroInitSpeed;
        private double _returnToZeroMinSpeed;
        private double _returnToZeroMaxSpeed;
        private double _retreatDistance;

        /// <summary>
        /// 值改变事件
        /// </summary>
        public EventHandler? AfterModified;

        /// <summary>
        /// 回零加速时间-数据模型
        /// </summary>
        public NumericWithLableViewModel ReturnToZeroAccTimeViewModel { get; }
        /// <summary>
        /// 回零初速度-数据模型
        /// </summary>
        public NumericWithLableViewModel ReturnToZeroInitSpeedViewModel { get; }
        /// <summary>
        /// 回零最小速度-数据模型
        /// </summary>
        public NumericWithLableViewModel ReturnToZeroMinSpeedViewModel { get; }
        /// <summary>
        /// 回零最大速度-数据模型
        /// </summary>
        public NumericWithLableViewModel ReturnToZeroMaxSpeedViewModel { get; }
        /// <summary>
        /// 后撤距离-数据模型
        /// </summary>
        public NumericWithLableViewModel RetreatDistanceViewModel { get; }

        /// <summary>
        /// 回零模式-下拉框数据模型
        /// </summary>
        public ComboxViewModel ReturnToZeroModeModel { get; }
        /// <summary>
        /// 回零方向-下拉框数据模型
        /// </summary>
        public ComboxViewModel ReturnToZeroDirectionModel { get; }


        /// <summary>
        /// 选中的回零方向
        /// </summary>
        public ReturnToZeroMode SelectedReturnToZeroMode
        {
           
            get => (ReturnToZeroMode)((ReturnToZeroModeModel.SelectedItem as ComBoxItem)?.Value ?? ReturnToZeroMode.EZ);
            set
            {
                if (ReturnToZeroModeModel.ItemsSource != null)
                {
                    var targetItem = ReturnToZeroModeModel.ItemsSource.Cast<ComBoxItem>()
                    .FirstOrDefault(o => (ReturnToZeroMode)o.Value == value);
                    if (targetItem != null)
                        ReturnToZeroModeModel.SelectedItem = targetItem;
                    this.RaisePropertyChanged(nameof(SelectedReturnToZeroMode));
                }
            }
        }

        /// <summary>
        /// 选中的回零方向
        /// </summary>
        public ReturnToZeroDirection SelectedReturnToZeroDirection
        {
           
            get => (ReturnToZeroDirection)((ReturnToZeroDirectionModel.SelectedItem as ComBoxItem)?.Value ?? ReturnToZeroDirection.Left);
            set
            {
                if (ReturnToZeroDirectionModel.ItemsSource != null)
                {
                    var targetItem = ReturnToZeroDirectionModel.ItemsSource.Cast<ComBoxItem>()
                    .FirstOrDefault(o => (ReturnToZeroDirection)o.Value == value);
                    if (targetItem != null)
                        ReturnToZeroDirectionModel.SelectedItem = targetItem;
                    this.RaisePropertyChanged(nameof(SelectedReturnToZeroDirection));
                }
            }
        }

        /// <summary>
        /// 回零加速时间
        /// </summary>
        public double ReturnToZeroAccTime
        {
            get
            {
                _returnToZeroAccTime = (double)ReturnToZeroAccTimeViewModel.Value;
                return _returnToZeroAccTime;
            }
            set
            {
                _returnToZeroAccTime = value;
                ReturnToZeroAccTimeViewModel.Value = (decimal)_returnToZeroAccTime;
            }
        }
        /// <summary>
        /// 回零初速度
        /// </summary>
        public double ReturnToZeroInitSpeed
        {
            get
            {
                _returnToZeroInitSpeed = (double)ReturnToZeroInitSpeedViewModel.Value;
                return _returnToZeroInitSpeed;
            }
            set
            {
                _returnToZeroInitSpeed = value;
                ReturnToZeroInitSpeedViewModel.Value = (decimal)_returnToZeroInitSpeed;
            }
        }
        /// <summary>
        /// 回零最小速度
        /// </summary>
        public double ReturnToZeroMinSpeed
        {
            get
            {
                _returnToZeroMinSpeed = (double)ReturnToZeroMinSpeedViewModel.Value;
                return _returnToZeroMinSpeed;
            }
            set
            {
                _returnToZeroMinSpeed = value;
                ReturnToZeroMinSpeedViewModel.Value = (decimal)_returnToZeroMinSpeed;
            }
        }
        /// <summary>
        /// 回零最大速度
        /// </summary>
        public double ReturnToZeroMaxSpeed
        {
            get
            {
                _returnToZeroMaxSpeed = (double)ReturnToZeroMaxSpeedViewModel.Value;
                return _returnToZeroMaxSpeed;
            }
            set
            {
                _returnToZeroMaxSpeed = value;
                ReturnToZeroMaxSpeedViewModel.Value = (decimal)_returnToZeroMaxSpeed;
            }
        }
        /// <summary>
        /// 后撤距离
        /// </summary>
        
        public double RetreatDistance
        {
            get
            {
                _retreatDistance = (double)RetreatDistanceViewModel.Value;
                return _retreatDistance;
            }
            set
            {
                _retreatDistance = value;
                RetreatDistanceViewModel.Value = (decimal)_retreatDistance;
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ReturnToZeroParamViewModel()
        {
            ReturnToZeroAccTimeViewModel = new NumericWithLableViewModel() { DecimalPlaces=1,Increment=(decimal)0.1 ,LableText="s" };
            ReturnToZeroInitSpeedViewModel = new NumericWithLableViewModel() { DecimalPlaces=1,Increment=(decimal)0.1 , LableText = "mm/s" };
            ReturnToZeroMinSpeedViewModel = new NumericWithLableViewModel() { DecimalPlaces=1,Increment=(decimal)0.1, LableText = "mm/s" };
            ReturnToZeroMaxSpeedViewModel = new NumericWithLableViewModel() { DecimalPlaces=1,Increment=(decimal)0.1, LableText = "mm/s" };
            RetreatDistanceViewModel = new NumericWithLableViewModel() { DecimalPlaces=1,Increment=(decimal)0.1, LableText = "mm" };

            ReturnToZeroModeModel = new ComboxViewModel();
            var reactionDisplayNames = new Dictionary<ReturnToZeroMode, string>
            {
                { ReturnToZeroMode.OneOrigin, "1次原点回零" },
                { ReturnToZeroMode.TowOrigin, "2次原点回零" },
                { ReturnToZeroMode.NegativeLimit, "负极限回零" },
                { ReturnToZeroMode.EZ, "EZ回零" },
                { ReturnToZeroMode.EZStop, "找一个EZ停止" },
                { ReturnToZeroMode.EZLockAndRetrieve, "找一个EZ锁存回找" },
            };
            ReturnToZeroModeModel.ItemsSource = EnumExtensions.ToEnumItems<ReturnToZeroMode>(reactionDisplayNames);
            ReturnToZeroModeModel.DisplayMemberPath = nameof(ComBoxItem.DisplayName);

            ReturnToZeroDirectionModel = new ComboxViewModel();
            var directioDisplayNames = new Dictionary<ReturnToZeroDirection, string>
            {
                { ReturnToZeroDirection.Left, "左" },
                { ReturnToZeroDirection.Right, "右" }
            };
            ReturnToZeroDirectionModel.ItemsSource = EnumExtensions.ToEnumItems<ReturnToZeroDirection>(directioDisplayNames);
            ReturnToZeroDirectionModel.DisplayMemberPath = nameof(ComBoxItem.DisplayName);
            // 订阅值变更事件
            subscribeValueChanges();
        }

        /// <summary>
        /// 从数据模型复制数据到ViewModel
        /// </summary>
        /// <param name="returnToZeroParamInfo"></param>
        public void CopyFrom(ReturnToZeroParamInfo returnToZeroParamInfo)
        {
            this.SelectedReturnToZeroMode = returnToZeroParamInfo.ReturnToZeroMode;
            this.SelectedReturnToZeroDirection = returnToZeroParamInfo.ReturnToZeroDirection;
            this.ReturnToZeroAccTime = returnToZeroParamInfo.ReturnToZeroAccTime;
            this.ReturnToZeroInitSpeed = returnToZeroParamInfo.ReturnToZeroInitSpeed;
            this.ReturnToZeroMinSpeed = returnToZeroParamInfo.ReturnToZeroMinSpeed;
            this.ReturnToZeroMaxSpeed = returnToZeroParamInfo.ReturnToZeroMaxSpeed;
            this.RetreatDistance = returnToZeroParamInfo.RetreatDistance;
        }

        /// <summary>
        /// 从ViewModel复制数据到数据模型
        /// </summary>
        /// <param name="returnToZeroParamInfo"></param>
        public void CopyTo(ReturnToZeroParamInfo returnToZeroParamInfo)
        {
            returnToZeroParamInfo.ReturnToZeroMode = this.SelectedReturnToZeroMode;
            returnToZeroParamInfo.ReturnToZeroDirection = this.SelectedReturnToZeroDirection;
            returnToZeroParamInfo.ReturnToZeroAccTime = this.ReturnToZeroAccTime;
            returnToZeroParamInfo.ReturnToZeroInitSpeed = this.ReturnToZeroInitSpeed;
            returnToZeroParamInfo.ReturnToZeroMinSpeed = this.ReturnToZeroMinSpeed;
            returnToZeroParamInfo.ReturnToZeroMaxSpeed = this.ReturnToZeroMaxSpeed;
            returnToZeroParamInfo.RetreatDistance = this.RetreatDistance;
        }

        #region 值改变事件
        /// <summary>
        /// 订阅值改变事件
        /// </summary>
        private void subscribeValueChanges()
        {
            ReturnToZeroAccTimeViewModel.ValueChanged += onValueChanged;
            ReturnToZeroInitSpeedViewModel.ValueChanged += onValueChanged;
            ReturnToZeroMinSpeedViewModel.ValueChanged += onValueChanged;
            ReturnToZeroMaxSpeedViewModel.ValueChanged += onValueChanged;
            RetreatDistanceViewModel.ValueChanged += onValueChanged;
            ReturnToZeroModeModel.ValueChanged += onValueChanged;
            ReturnToZeroDirectionModel.ValueChanged += onValueChanged;
        }

        /// <summary>
        /// 值改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onValueChanged(object? sender, ValueChangedEventArgs e)
        {
            AfterModified?.Invoke(sender, e);
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
        #endregion
    }
}