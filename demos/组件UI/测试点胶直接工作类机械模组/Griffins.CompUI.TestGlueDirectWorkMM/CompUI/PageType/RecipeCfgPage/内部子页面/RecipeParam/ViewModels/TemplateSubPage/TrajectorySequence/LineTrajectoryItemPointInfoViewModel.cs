using Griffins.UI;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.TemplateSubPage;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.TemplateSubPage.TrajectorySequence;
using ReactiveUI;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.TemplateSubPage.TrajectorySequence
{
    /// <summary>
    /// 线轨迹中的点明细信息-视图模型
    /// </summary>
    public class LineTrajectoryItemPointInfoViewModel : ReactiveObject
    {

        /// <summary>
        /// 点类型-下拉框数据模型
        /// </summary>
        public ComboxViewModel LinePointKindModel { get; }

        /// <summary>
        /// X值-数据模型
        /// </summary>
        public NumericViewModel XViewModel { get; }

        /// <summary>
        /// Y值-数据模型
        /// </summary>
        public NumericViewModel YViewModel { get; }

        /// <summary>
        /// Z值-数据模型
        /// </summary>
        public NumericViewModel ZViewModel { get; }


        /// <summary>
        /// 序号
        /// </summary>
        private int _serialNumber;
        public int SerialNumber
        {
            get => _serialNumber;
            set => this.RaiseAndSetIfChanged(ref _serialNumber, value);
        }
        /// <summary>
        /// 点ID
        /// </summary>
        private Guid _pointID;
        public Guid PointID
        {
            get => _pointID;
            set => this.RaiseAndSetIfChanged(ref _pointID, value);
        }

        /// <summary>
        /// 点类型
        /// </summary>
        public LinePointKind LinePointKind
        {
           
            get => (LinePointKind)((LinePointKindModel.SelectedItem as ComBoxItem)?.Value ?? LinePointKind.Middle);
            set
            {
                if (LinePointKindModel.ItemsSource != null)
                {
                    var targetItem = LinePointKindModel.ItemsSource.Cast<ComBoxItem>()
                    .FirstOrDefault(o => (LinePointKind)o.Value == value);
                    if (targetItem != null)
                        LinePointKindModel.SelectedItem = targetItem;
                    this.RaisePropertyChanged(nameof(LinePointKind));
                }
            }
        }
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

        /// <summary>
        /// 构造函数
        /// </summary>
        public LineTrajectoryItemPointInfoViewModel()
        {
            // 坐标默认支持负数（如机械原点偏移），步长0.001mm
            XViewModel = new NumericViewModel { Increment = 0.001m, DecimalPlaces = 3, Minimum = 0.000m, Maximum = 50.000m, };
            YViewModel = new NumericViewModel { Increment = 0.001m, DecimalPlaces = 3, Minimum = 0.000m, Maximum = 50.000m, };
            ZViewModel = new NumericViewModel { Increment = 0.001m, DecimalPlaces = 3, Minimum = 0.000m, Maximum = 50.000m, };

            // 初始化点类型数据源
            LinePointKindModel = new ComboxViewModel();
            var lineCalculateTrajectoryTypeDisplayNames = new Dictionary<LinePointKind, string>
            {
                { LinePointKind.Start, "起点" },
                { LinePointKind.Middle, "中间点" },
                { LinePointKind.End, "结束点" },
                { LinePointKind.CenterCircle, "圆心" },
            };
            LinePointKindModel.ItemsSource = EnumExtensions.ToEnumItems(lineCalculateTrajectoryTypeDisplayNames);
            LinePointKindModel.DisplayMemberPath = nameof(ComBoxItem.DisplayName);
            LinePointKindModel.IsEnabled = false;

            subscribeChildViewModelEvents();
        }

        /// <summary>
        /// 从数据模型复制数据到ViewModel
        /// </summary>
        public void CopyFrom(PointInfo pointInfo)
        {
            PointID = pointInfo.PointID;
            SerialNumber = pointInfo.SerialNumber;
            LinePointKindModel.SelectedItem = LinePointKindModel.ItemsSource?.Cast<ComBoxItem>().FirstOrDefault(o => (LinePointKind)o.Value == LinePointKind);
            foreach (var axisValue in pointInfo.CoordinateAxisValues)
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
        /// <param name="pointInfo"></param>
        public void CopyTo(PointInfo pointInfo)
        {
            pointInfo.PointID = PointID;
            pointInfo.SerialNumber = SerialNumber;

            pointInfo.CoordinateAxisValues = new CoordinateAxisValueList
            {
                new CoordinateAxisValue { Axis = CoordinateAxisConstant.X, AxisValue = X },
                new CoordinateAxisValue { Axis = CoordinateAxisConstant.Y, AxisValue = Y },
                new CoordinateAxisValue { Axis = CoordinateAxisConstant.Z, AxisValue = Z }
            };

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
                      pointChanged?.Invoke(this,new PointChangedEventArgs()
                      {
                          X=this.X,
                          Y=this.Y,
                          Z=this.Z
                      });
                  });
        }
        /// <summary>
        /// 订阅子ViewModel的事件
        /// </summary>
        private void subscribeChildViewModelEvents()
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
        }
        private void YViewModel_ValueChanged(object? sender, ValueChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                Y = (decimal)e.NewValue;
            }
        }
        private void ZViewModel_ValueChanged(object? sender, ValueChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                Z = (decimal)e.NewValue;
            }
        }
    }
}
