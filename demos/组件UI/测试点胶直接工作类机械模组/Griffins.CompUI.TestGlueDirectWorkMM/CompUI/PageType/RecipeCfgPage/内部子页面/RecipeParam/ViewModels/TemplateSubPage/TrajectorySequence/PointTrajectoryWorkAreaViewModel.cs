using Griffins.UI;
using ReactiveUI;
using System.Reactive.Linq;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.TemplateSubPage;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.TemplateSubPage.TrajectorySequence;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.TemplateSubPage.TrajectorySequence
{
    /// <summary>
    /// 点轨迹工作区-视图模型
    /// </summary>
    public class PointTrajectoryWorkAreaViewModel : ReactiveObject
    {
        /// <summary>
        /// 值改变事件
        /// </summary>
        public EventHandler? AfterModified;
        /// <summary>
        /// 目标点X轴
        /// </summary>
        public NumericViewModel XViewModel { get; }

        /// <summary>
        /// 目标点Y轴
        /// </summary>
        public NumericViewModel YViewModel { get; }

        /// <summary>
        /// 目标点Z轴
        /// </summary>
        public NumericViewModel ZViewModel { get; }

        #region 响应式属性

        /// <summary>
        /// X轴坐标
        /// </summary>
        public decimal X
        {
            get => XViewModel.Value;
            set
            {
                XViewModel.Value = value;
                this.RaisePropertyChanged(nameof(X));
            }
        }

        /// <summary>
        /// Y轴坐标
        /// </summary>
        public decimal Y
        {
            get => YViewModel.Value;
            set
            {
                YViewModel.Value = value;
                this.RaisePropertyChanged(nameof(Y));
            }
        }

        /// <summary>
        /// Z轴坐标
        /// </summary>
        public decimal Z
        {
            get => ZViewModel.Value;
            set
            {
                ZViewModel.Value = value;
                this.RaisePropertyChanged(nameof(Z));
            }
        }

        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public PointTrajectoryWorkAreaViewModel()
        {
            XViewModel = new NumericViewModel() { Increment = 0.001m, DecimalPlaces = 3, Minimum = 0.000m, Maximum = 50.000m, };
            YViewModel = new NumericViewModel() { Increment = 0.001m, DecimalPlaces = 3, Minimum = 0.000m, Maximum = 50.000m, };
            ZViewModel = new NumericViewModel() { Increment = 0.001m, DecimalPlaces = 3, Minimum = 0.000m, Maximum = 50.000m, };

            // 订阅值变更事件
            subscribeValueChanges();
        }

        /// <summary>
        /// 从数据模型复制数据到ViewModel
        /// </summary>
        /// <param name="pointCalculateTrajectory"></param>
        public void CopyFrom(PointCalculateTrajectory pointCalculateTrajectory)
        {
            if (pointCalculateTrajectory == null)
                return;

            // 从轨迹数据初始化XYZ坐标（假设CoordinateAxisValue包含轴标识和值）
            foreach (var axisValue in pointCalculateTrajectory.TargetPoint.CoordinateAxisValues)
            {
                switch (axisValue.Axis)
                {
                    case CoordinateAxisConstant.X:
                        X = axisValue.AxisValue;
                        break;
                    case CoordinateAxisConstant.Y:
                        Y = axisValue.AxisValue;
                        break;
                    case CoordinateAxisConstant.Z:
                        Z = axisValue.AxisValue;
                        break;
                }
            }
        }

        /// <summary>
        /// 从ViewModel复制数据到数据模型
        /// </summary>
        /// <param name="pointCalculateTrajectory"></param>
        public void CopyTo(PointCalculateTrajectory pointCalculateTrajectory)
        {
            if (pointCalculateTrajectory == null)
                return;

            // 清空原有坐标数据
            pointCalculateTrajectory.TargetPoint.CoordinateAxisValues.Clear();

            // 添加XYZ坐标到轨迹模型
            pointCalculateTrajectory.TargetPoint.CoordinateAxisValues.Add(new CoordinateAxisValue { Axis = CoordinateAxisConstant.X, AxisValue = X });
            pointCalculateTrajectory.TargetPoint.CoordinateAxisValues.Add(new CoordinateAxisValue { Axis = CoordinateAxisConstant.Y, AxisValue = Y });
            pointCalculateTrajectory.TargetPoint.CoordinateAxisValues.Add(new CoordinateAxisValue { Axis = CoordinateAxisConstant.Z, AxisValue = Z });
        }
        /// <summary>
        /// 平移
        /// </summary>
        /// <param name="translationParam"></param>
        public void Translation(TranslationParam translationParam)
        {
            this.X += translationParam.X;
            this.Y += translationParam.Y;
            this.Z += translationParam.Z;
        }
        /// <summary>
        /// 设置点坐标
        /// </summary>
        public void SetPointWhenEditChanged(decimal x, decimal y, decimal z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;

        }
        /// <summary>
        /// 订阅点改变事件
        /// </summary>
        /// <param name="styleItemChanged"></param>
        public void SubscribPointChanged(EventHandler<PointChangedEventArgs>? pointChanged)
        {
            this.WhenAnyValue(
                     vm => vm.X,
                     vm => vm.Y,
                     vm => vm.Z
                 )
                 .Subscribe(_ =>
                 {
                     pointChanged?.Invoke(this, new PointChangedEventArgs()
                     {
                         X = this.X,
                         Y= this.Y,
                         Z= this.Z
                     });
                 });
        }
      
       
        #region 值改变事件
        /// <summary>
        /// 订阅值改变事件
        /// </summary>
        private void subscribeValueChanges()
        {
            XViewModel.ValueChanged += XViewModel_ValueChanged;
            YViewModel.ValueChanged += YViewModel_ValueChanged;
            ZViewModel.ValueChanged += ZViewModel_ValueChanged;
        }
        private void XViewModel_ValueChanged(object? sender, ValueChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                X = (decimal)e.NewValue;
            }
            AfterModified?.Invoke(sender, e);
        }
        private void YViewModel_ValueChanged(object? sender, ValueChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                Y = (decimal)e.NewValue;
            }
            AfterModified?.Invoke(sender, e);
        }
        private void ZViewModel_ValueChanged(object? sender, ValueChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                Z = (decimal)e.NewValue;
            }
            AfterModified?.Invoke(sender, e);
        }
        
        #endregion
    }
}