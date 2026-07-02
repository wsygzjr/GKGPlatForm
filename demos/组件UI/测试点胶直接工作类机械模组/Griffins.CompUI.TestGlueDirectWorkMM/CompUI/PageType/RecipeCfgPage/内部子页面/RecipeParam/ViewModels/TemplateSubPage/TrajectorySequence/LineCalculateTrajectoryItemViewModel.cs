using Griffins.UI;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.GlueDispensingStyle;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.TemplateSubPage;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.TemplateSubPage.TrajectorySequence;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.GlueDispensingStyle;
using ReactiveUI;
using System.Collections.ObjectModel;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.TemplateSubPage.TrajectorySequence
{
    /// <summary>
    /// 线计算轨迹项-视图模型
    /// </summary>
    public class LineCalculateTrajectoryItemViewModel : DataGridItemBaseViewModel<CalculateTrajectoryItem>
    {
        private EventHandler<PointChangedEventArgs>? _pointChanged;
        /// <summary>
        /// 计算轨迹类型-下拉框数据模型
        /// </summary>
        public ComboxViewModel LineCalculateTrajectoryTypeModel { get; }

        /// <summary>
        /// 线计算轨迹类型(线类型)
        /// </summary>
        public LineCalculateTrajectoryType LineCalculateTrajectoryType
        {
           
            get => (LineCalculateTrajectoryType)((LineCalculateTrajectoryTypeModel.SelectedItem as ComBoxItem)?.Value ?? LineCalculateTrajectoryType.StraightLine);
            set
            {
                if (LineCalculateTrajectoryTypeModel.ItemsSource != null)
                {
                    var targetItem = LineCalculateTrajectoryTypeModel.ItemsSource.Cast<ComBoxItem>()
                    .FirstOrDefault(o => (LineCalculateTrajectoryType)o.Value == value);
                    if (targetItem != null)
                        LineCalculateTrajectoryTypeModel.SelectedItem = targetItem;
                    this.RaisePropertyChanged(nameof(LineCalculateTrajectoryType));
                }
            }
        }
        /// <summary>
        /// 重量单位数据源
        /// </summary>
        private List<ComBoxItem> _weightUnitItems;


        /// <summary>
        /// 是否点胶-数据模型
        /// </summary>
        public ToggleSwitchViewModel IsDispensingViewModel { get; }

        /// <summary>
        /// 选中的样式ID-下拉框数据模型
        /// </summary>
        public ComboxViewModel SelectedStyleModel { get; }

        /// <summary>
        /// 重量单位-下拉框数据模型
        /// </summary>
        public ComboxViewModel WeightUnitModel { get; }

        /// <summary>
        /// 重量值-数据模型
        /// </summary>
        public NumericViewModel WeightViewModel { get; }
         
        /// <summary>
        /// 点列表
        /// </summary>
        public ObservableCollection<LineTrajectoryItemPointInfoViewModel> PointInfoItems { get; }

        /// <summary>
        /// 当前选中的点
        /// </summary>
        private LineTrajectoryItemPointInfoViewModel? _selectedPointInfo;
        public LineTrajectoryItemPointInfoViewModel? SelectedPointInfo
        {
            get => _selectedPointInfo;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedPointInfo, value);
                if(_selectedPointInfo!=null)
                {
                    _pointChanged?.Invoke(this, new PointChangedEventArgs()
                    {
                        X = _selectedPointInfo.X,
                        Y = _selectedPointInfo.Y,
                        Z = _selectedPointInfo.Z
                    });
                }
            }
        }
        /// <summary>
        /// 线轨迹ID
        /// </summary>
        private Guid _trackID;
        public Guid TrackID
        {
            get => _trackID;
            set => this.RaiseAndSetIfChanged(ref _trackID, value);
        }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnabled
        {
            get => IsDispensingViewModel.IsChecked;
            set
            {
                IsDispensingViewModel.IsChecked = value;
                this.RaisePropertyChanged(nameof(IsEnabled));
            }
        }

        /// <summary>
        /// 选中的样式ID
        /// </summary>
        public Guid SelectedStyleID
        {
            get => (Guid)((SelectedStyleModel.SelectedItem as ComBoxItem)?.Value ?? Guid.Empty);
            set
            {
                if (SelectedStyleModel.ItemsSource != null)
                {
                    var targetItem = SelectedStyleModel.ItemsSource.Cast<ComBoxItem>()
                    .FirstOrDefault(o => (Guid)o.Value == value);
                    if (targetItem != null)
                        SelectedStyleModel.SelectedItem = targetItem;
                    this.RaisePropertyChanged(nameof(SelectedStyleID));
                }
            }
        }

        /// <summary>
        /// 重量单位
        /// </summary>
        public WeightUnit WeightUnit
        {
            get
            {
                if (WeightUnitModel.SelectedItem != null)
                    return (WeightUnit)(WeightUnitModel.SelectedItem as ComBoxItem)!.Value; ;
                return WeightUnit.MG;
            }
            set
            {
                WeightUnitModel.SelectedItem = _weightUnitItems.FirstOrDefault(o => (WeightUnit)o.Value == value);
            }
        }

        /// <summary>
        /// 重量值
        /// </summary>
        public decimal Weight
        {
            get => WeightViewModel.Value;
            set
            {
                WeightViewModel.Value = value;
                this.RaisePropertyChanged(nameof(Weight));
            }
        } 
        /// <summary>
        /// 构造函数
        /// </summary>
        public LineCalculateTrajectoryItemViewModel()
        { 
            IsDispensingViewModel = new ToggleSwitchViewModel { IsChecked = true };
            WeightViewModel = new NumericViewModel { Minimum = 1, Increment = 1, Value = 1 };

            // 初始化样式数据源
            SelectedStyleModel = new ComboxViewModel
            {
                ItemsSource = getStyleSource(),
                DisplayMemberPath = nameof(ComBoxItem.DisplayName)
            };
            SelectedStyleModel.ValueChanged += (s, e) => this.RaisePropertyChanged(nameof(SelectedStyleID));

            // 初始化重量单位数据源
            WeightUnitModel = new ComboxViewModel();
            var weightUnitDisplayNames = new Dictionary<WeightUnit, string>
            {
                { WeightUnit.MG, "mg" },
                { WeightUnit.MMS, "mm/s" },
                { WeightUnit.MGMM, "mg/mm" },
                { WeightUnit.DOT, "dot" },
            };
            _weightUnitItems = EnumExtensions.ToEnumItems(weightUnitDisplayNames);
            WeightUnitModel.ItemsSource = _weightUnitItems;
            WeightUnitModel.DisplayMemberPath = nameof(ComBoxItem.DisplayName);
            WeightUnitModel.ValueChanged += (s, e) => this.RaisePropertyChanged(nameof(WeightUnit));
             
            // 初始化轨迹类型数据源
            LineCalculateTrajectoryTypeModel = new ComboxViewModel();
            var lineCalculateTrajectoryTypeDisplayNames = new Dictionary<LineCalculateTrajectoryType, string>
            {
                { LineCalculateTrajectoryType.StraightLine, "直线" },
                { LineCalculateTrajectoryType.CircularArcA, "圆弧A" },
                { LineCalculateTrajectoryType.CircularArcB, "圆弧B" },
                { LineCalculateTrajectoryType.Circle, "圆" },
            };
            LineCalculateTrajectoryTypeModel.ItemsSource = EnumExtensions.ToEnumItems(lineCalculateTrajectoryTypeDisplayNames);
            LineCalculateTrajectoryTypeModel.DisplayMemberPath = nameof(ComBoxItem.DisplayName);
            LineCalculateTrajectoryTypeModel.ValueChanged += (s, e) => this.RaisePropertyChanged(nameof(LineCalculateTrajectoryType));
            LineCalculateTrajectoryTypeModel.IsEnabled = false;

            PointInfoItems = new ObservableCollection<LineTrajectoryItemPointInfoViewModel>();
            SubscribeChildViewModelEvents();

            CacheDataExchange.SubscribStyleChanged(cacheData_StyleChanged);
        }

      

        /// <summary>
        /// 从源复制
        /// </summary>
        public override void CopyFrom(CalculateTrajectoryItem item)
        {
            base.CopyBasePropertiesFrom(item);
            this.TrackID = item.CalculateTrajectory.TrackID;
            this.SerialNumber = item.CalculateTrajectory.SerialNumber;

            this.IsEnabled = item.CalculateTrajectory.LineProcessingCfgInfo.IsEnabled;
            this.SelectedStyleID = item.CalculateTrajectory.LineProcessingCfgInfo.DispensingMiddleStyleID;
            this.Weight = item.CalculateTrajectory.LineProcessingCfgInfo.Weight;
            this.WeightUnit = item.CalculateTrajectory.LineProcessingCfgInfo.WeightUnit;

            this.LineCalculateTrajectoryType = item.LineCalculateTrajectoryType;

            switch (item.LineCalculateTrajectoryType)
            {
                case LineCalculateTrajectoryType.StraightLine:
                    var straightLineACalculateTrajectory = (StraightLineACalculateTrajectory)item.CalculateTrajectory;
                    var endpointPointViewModel = new LineTrajectoryItemPointInfoViewModel();
                    endpointPointViewModel.LinePointKind = LinePointKind.End;
                    endpointPointViewModel.CopyFrom(straightLineACalculateTrajectory.EndpointPoint);
                    endpointPointViewModel.SubscribPointChanged(_pointChanged);
                    PointInfoItems.Add(endpointPointViewModel);
                    break;
                case LineCalculateTrajectoryType.CircularArcA:
                    var circularArcACalculateTrajectory = (CircularArcACalculateTrajectory)item.CalculateTrajectory;

                    var middlePoint = new LineTrajectoryItemPointInfoViewModel();
                    middlePoint.LinePointKind = LinePointKind.Middle;
                    middlePoint.CopyFrom(circularArcACalculateTrajectory.MiddlePoint);
                    middlePoint.SubscribPointChanged(_pointChanged);
                    PointInfoItems.Add(middlePoint);

                    var circularArcAendpointPointViewModel = new LineTrajectoryItemPointInfoViewModel();
                    circularArcAendpointPointViewModel.LinePointKind = LinePointKind.End;
                    circularArcAendpointPointViewModel.CopyFrom(circularArcACalculateTrajectory.EndpointPoint);
                    circularArcAendpointPointViewModel.SubscribPointChanged(_pointChanged);
                    PointInfoItems.Add(circularArcAendpointPointViewModel);

                    break;
                case LineCalculateTrajectoryType.CircularArcB:
                    var circularArcBCalculateTrajectory = (CircularArcBCalculateTrajectory)item.CalculateTrajectory;

                    var centerCircle = new LineTrajectoryItemPointInfoViewModel();
                    centerCircle.LinePointKind = LinePointKind.CenterCircle;
                    centerCircle.CopyFrom(circularArcBCalculateTrajectory.CenterCirclePoint);
                    centerCircle.SubscribPointChanged(_pointChanged);
                    PointInfoItems.Add(centerCircle);

                    var bEndpointPoint = new LineTrajectoryItemPointInfoViewModel();
                    bEndpointPoint.LinePointKind = LinePointKind.End;
                    bEndpointPoint.CopyFrom(circularArcBCalculateTrajectory.EndpointPoint);
                    bEndpointPoint.SubscribPointChanged(_pointChanged);
                    PointInfoItems.Add(bEndpointPoint);

                    break;
                case LineCalculateTrajectoryType.Circle:
                    break;
                default:
                    break;
            }
            LineCalculateTrajectoryTypeModel.SelectedItem = LineCalculateTrajectoryTypeModel.ItemsSource?.Cast<ComBoxItem>().FirstOrDefault(o => (LineCalculateTrajectoryType)o.Value == LineCalculateTrajectoryType);

        }


        /// <summary>
        /// 复制到目标对象
        /// </summary>
        /// <param name="item">目标数据模型</param>
        public override void CopyTo(CalculateTrajectoryItem item)
        {
            base.CopyBasePropertiesTo(item);
            item.LineCalculateTrajectoryType = LineCalculateTrajectoryType;

            var baseTraj = item.CalculateTrajectory;
            baseTraj.TrackID = TrackID;
            baseTraj.SerialNumber = SerialNumber;
            baseTraj.LineProcessingCfgInfo.IsEnabled = IsEnabled;
            baseTraj.LineProcessingCfgInfo.DispensingMiddleStyleID = SelectedStyleID;
            baseTraj.LineProcessingCfgInfo.Weight = Weight;
            baseTraj.LineProcessingCfgInfo.WeightUnit = WeightUnit;


            switch (LineCalculateTrajectoryType)
            {
                case LineCalculateTrajectoryType.StraightLine:
                    var endPointVm = PointInfoItems.FirstOrDefault(p => p.LinePointKind == LinePointKind.End);
                    if (endPointVm == null)
                        throw new Exception("结束点对象为空");
                    var straightLineTraj = new StraightLineACalculateTrajectory
                    {
                        TrackID = baseTraj.TrackID,
                        SerialNumber = baseTraj.SerialNumber,
                        LineProcessingCfgInfo = baseTraj.LineProcessingCfgInfo 
                    };
                    endPointVm.CopyTo(straightLineTraj.EndpointPoint);
                    item.CalculateTrajectory = straightLineTraj;
                    break;

                case LineCalculateTrajectoryType.CircularArcA:
                    var arcAEndVm = PointInfoItems.FirstOrDefault(p => p.LinePointKind == LinePointKind.End);
                    var arcAMiddleVm = PointInfoItems.FirstOrDefault(p => p.LinePointKind == LinePointKind.Middle);
                    if (arcAEndVm == null || arcAMiddleVm == null)
                        throw new Exception("结束点对象或中间点对象为空");
                    var arcATraj = new CircularArcACalculateTrajectory
                    {
                        TrackID = baseTraj.TrackID,
                        SerialNumber = baseTraj.SerialNumber,
                        LineProcessingCfgInfo = baseTraj.LineProcessingCfgInfo
                    };
                    arcAEndVm.CopyTo(arcATraj.EndpointPoint);
                    arcAMiddleVm.CopyTo(arcATraj.MiddlePoint);
                    item.CalculateTrajectory = arcATraj;
                    break;

                case LineCalculateTrajectoryType.CircularArcB:
                    var arcBEndVm = PointInfoItems.FirstOrDefault(p => p.LinePointKind == LinePointKind.End);
                    var arcBCenterVm = PointInfoItems.FirstOrDefault(p => p.LinePointKind == LinePointKind.CenterCircle);
                    if (arcBEndVm == null || arcBCenterVm == null)
                        throw new Exception("终点对象或圆心点对象为空");
                    var arcBTraj = new CircularArcBCalculateTrajectory
                    {
                        TrackID = baseTraj.TrackID,
                        SerialNumber = baseTraj.SerialNumber,
                        LineProcessingCfgInfo = baseTraj.LineProcessingCfgInfo
                    };
                    arcBEndVm.CopyTo(arcBTraj.EndpointPoint);
                    arcBCenterVm.CopyTo(arcBTraj.CenterCirclePoint);
                    item.CalculateTrajectory = arcBTraj;
                    break;

                case LineCalculateTrajectoryType.Circle:
                    var circleCenterVm = PointInfoItems.FirstOrDefault(p => p.LinePointKind == LinePointKind.CenterCircle);
                    var circleEdgeVm = PointInfoItems.FirstOrDefault(p => p.LinePointKind == LinePointKind.End);
                    if (circleCenterVm == null || circleEdgeVm == null)
                    {
                        return;
                    }

                    var circleTraj = new CircleCalculateTrajectory
                    {
                        TrackID = baseTraj.TrackID,
                        SerialNumber = baseTraj.SerialNumber,
                        LineProcessingCfgInfo = baseTraj.LineProcessingCfgInfo
                    };
                    item.CalculateTrajectory = circleTraj;
                    break;

                default:
                    break;
            }
        }
        /// <summary>
        /// 平移
        /// </summary>
        /// <param name="translationParam"></param>
        public void Translation(TranslationParam translationParam)
        {
            foreach (var item in PointInfoItems)
            {
                item.Translation(translationParam);
            }
        }
        /// <summary>
        /// 设置点坐标
        /// </summary>
        public void SetPointWhenEditChanged(decimal x, decimal y, decimal z)
        {
            if (SelectedPointInfo == null)
                return;
            SelectedPointInfo.X = x;
            SelectedPointInfo.Y = y;
            SelectedPointInfo.Z = z;
        }
        /// <summary>
        /// 订阅点改变事件
        /// </summary>
        /// <param name="styleItemChanged"></param>
        public void SubscribPointChanged(EventHandler<PointChangedEventArgs>? pointChanged)
        {
            _pointChanged = pointChanged;
        }
        /// <summary>
        /// 订阅子ViewModel的事件
        /// </summary>
        private void SubscribeChildViewModelEvents()
        {

        }

        /// <summary>
        /// 获取样式数据源
        /// </summary>
        /// <returns></returns>
        private List<ComBoxItem> getStyleSource()
        {
            List<DispensingStyleCfgBaseInfo> dispensingStyleCfgBaseInfos = new List<DispensingStyleCfgBaseInfo>();
            DispensingStyleCfgInfo dispensingStyleCfgInfo = CacheDataExchange.GetDispensingStyleCfgInfo();
            //取线的点胶中的样式
            foreach (var item in dispensingStyleCfgInfo.DispensingLineStyleCfgInfo.DispensingMiddleLineStyleCfgInfoes)
            {
                dispensingStyleCfgBaseInfos.Add(item);
            }
            List<ComBoxItem> items = new List<ComBoxItem>();
            foreach (var item in dispensingStyleCfgBaseInfos)
            {
                items.Add(new ComBoxItem()
                {
                    Value = item.StyleID,
                    DisplayName = item.StyleName
                });
            }
            return items;
        }
        private void cacheData_StyleChanged(object? sender, GlueDispensingStyleChangedEventArgs e)
        {
            if (e.ChangedType == GlueDispensingStyleChangedType.Line)
            {
                SelectedStyleModel.ItemsSource = getStyleSource();
                SelectedStyleModel.SelectedItem = SelectedStyleModel.ItemsSource.Cast<ComBoxItem>().FirstOrDefault(o => (Guid)o.Value == SelectedStyleID);
            }
        }
    }
}
