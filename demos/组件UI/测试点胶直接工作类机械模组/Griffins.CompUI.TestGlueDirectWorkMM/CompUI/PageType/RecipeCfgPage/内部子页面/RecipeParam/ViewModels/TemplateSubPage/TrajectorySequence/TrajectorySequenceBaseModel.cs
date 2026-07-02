using Griffins.UI;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.GlueDispensingStyle;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.TemplateSubPage;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.TemplateSubPage.TrajectorySequence;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.GlueDispensingStyle;
using ReactiveUI;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.TemplateSubPage.TrajectorySequence
{
    /// <summary>
    /// 轨迹序列配置基类模型
    /// </summary>
    public  class TrajectorySequenceBaseModel : DataGridItemBaseViewModel<TrajectorySequenceCfgInfo>
    {
        /// <summary>
        /// TrackID-数据模型
        /// </summary>
        public TextBlockViewModel TrackIDViewModel { get; }

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
        /// 计算轨迹类型-下拉框数据模型
        /// </summary>
        public ComboxViewModel CalculateTrajectoryTypeModel { get; }

        /// <summary>
        /// 是否可编辑
        /// </summary>
        private bool _canEdit = false;
        public bool CanEdit
        {
            get => _canEdit;
            set
            {
                if (_canEdit == value) return;
                TrackIDViewModel.IsEnabled = value;
                SerialNumberViewModel.IsEnabled = value;
                IsDispensingViewModel.IsEnabled = value;
                SelectedStyleModel.IsEnabled = value;
                WeightUnitModel.IsEnabled = value;
                WeightViewModel.IsEnabled = value;
                CalculateTrajectoryTypeModel.IsEnabled = value;
                this.RaiseAndSetIfChanged(ref _canEdit, value);
            }
        }
        /// <summary>
        /// 轨迹ID
        /// </summary>
        public Guid TrackID
        {
            get =>Guid.Parse(TrackIDViewModel.Text);
            set
            {
                TrackIDViewModel.Text = value.ToString();
                this.RaisePropertyChanged(nameof(TrackID));
            }
        }

        /// <summary>
        /// 是否点胶
        /// </summary>
        public bool IsDispensing
        {
            get => IsDispensingViewModel.IsChecked;
            set
            {
                IsDispensingViewModel.IsChecked = value;
                this.RaisePropertyChanged(nameof(IsDispensing));
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
            get => (WeightUnit)((WeightUnitModel.SelectedItem as ComBoxItem)?.Value ?? WeightUnit.MG);
            set
            {
                if (WeightUnitModel.ItemsSource != null)
                {
                    var targetItem = WeightUnitModel.ItemsSource.Cast<ComBoxItem>()
                    .FirstOrDefault(o => (WeightUnit)o.Value == value);
                    if (targetItem != null)
                        WeightUnitModel.SelectedItem = targetItem;
                    this.RaisePropertyChanged(nameof(SelectedStyleID));
                }
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
        /// 计算轨迹类型
        /// </summary>
        public CalculateTrajectoryType CalculateTrajectoryType
        {

            get => (CalculateTrajectoryType)((CalculateTrajectoryTypeModel.SelectedItem as ComBoxItem)?.Value ?? CalculateTrajectoryType.Point);
            set
            {
                if (CalculateTrajectoryTypeModel.ItemsSource != null)
                {
                    var targetItem = CalculateTrajectoryTypeModel.ItemsSource.Cast<ComBoxItem>()
                    .FirstOrDefault(o => (CalculateTrajectoryType)o.Value == value);
                    if (targetItem != null)
                        CalculateTrajectoryTypeModel.SelectedItem = targetItem;
                    _onUpdateTrajectoryByCalculateTrajectoryType();
                    this.RaisePropertyChanged(nameof(CalculateTrajectoryType));
                }
            }
        }
        /// <summary>
        /// 计算轨迹类型改变
        /// </summary>
        protected virtual void _onUpdateTrajectoryByCalculateTrajectoryType()
        {
            switch (CalculateTrajectoryType)
            {
                case CalculateTrajectoryType.Point:
                    SelectedStyleModel.ItemsSource = getStyleSource(CalculateTrajectoryType.Point);
                    break;
                case CalculateTrajectoryType.Line:
                    //线轨迹工作区
                    SelectedStyleModel.ItemsSource = getStyleSource(CalculateTrajectoryType.Line);
                    break;
            }
        }

      

        /// <summary>
        /// 构造函数
        /// </summary>
        public TrajectorySequenceBaseModel()
        {
            //TrackID = Guid.NewGuid();  //zgl 代码报错
            TrackIDViewModel = new TextBlockViewModel(); 
            IsDispensingViewModel = new ToggleSwitchViewModel { IsChecked = true };
            WeightViewModel = new NumericViewModel { Minimum = 1, Increment = 1, Value = 1 };

            // 初始化样式数据源
            SelectedStyleModel = new ComboxViewModel
            {
                ItemsSource = getStyleSource(CalculateTrajectoryType.Point),
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
            WeightUnitModel.ItemsSource = EnumExtensions.ToEnumItems(weightUnitDisplayNames);
            WeightUnitModel.DisplayMemberPath = nameof(ComBoxItem.DisplayName);
            WeightUnitModel.ValueChanged += (s, e) => this.RaisePropertyChanged(nameof(WeightUnit));

            // 初始化轨迹类型数据源
            CalculateTrajectoryTypeModel = new ComboxViewModel();
            var calculateTrajectoryTypeDisplayNames = new Dictionary<CalculateTrajectoryType, string>
            {
                { CalculateTrajectoryType.Point, "点" },
                { CalculateTrajectoryType.Line, "线" },
            };
            CalculateTrajectoryTypeModel.ItemsSource = EnumExtensions.ToEnumItems(calculateTrajectoryTypeDisplayNames);
            CalculateTrajectoryTypeModel.DisplayMemberPath = nameof(ComBoxItem.DisplayName);
            CalculateTrajectoryTypeModel.IsEnabled = false;
            CalculateTrajectoryTypeModel.ValueChanged += (s, e) => this.RaisePropertyChanged(nameof(CalculateTrajectoryType));

            CacheDataExchange.SubscribStyleChanged(cacheData_StyleChanged);
        }

       

        /// <summary>
        /// 从信息对象复制数据
        /// </summary>
        /// <param name="info"></param>
        public override void CopyFrom(TrajectorySequenceCfgInfo info)
        {
            if (info == null) return;

            TrackID = info.TrackID;
            SerialNumber = info.SerialNumber;
            IsDispensing = info.IsDispensing;
            SelectedStyleID = info.SelectedStyleID;
            WeightUnit = info.WeightUnit;
            Weight = info.Weight;
            CalculateTrajectoryType = info.CalculateTrajectoryType;
        }

        /// <summary>
        /// 复制到信息对象
        /// </summary>
        /// <param name="targetInfo"></param>
        public override void CopyTo(TrajectorySequenceCfgInfo targetInfo)
        {
            if (targetInfo == null) return;
            targetInfo.TrackID = TrackID;
            targetInfo.SerialNumber = SerialNumber;
            targetInfo.IsDispensing = IsDispensing;
            targetInfo.SelectedStyleID = SelectedStyleID;
            targetInfo.WeightUnit = WeightUnit;
            targetInfo.Weight = Weight;
            targetInfo.CalculateTrajectoryType = CalculateTrajectoryType;
            
        }

        /// <summary>
        /// 点胶类型改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cacheData_StyleChanged(object? sender, GlueDispensingStyleChangedEventArgs e)
        {
            _onUpdateTrajectoryByCalculateTrajectoryType();
        }
        /// <summary>
        /// 获取样式数据源
        /// </summary>
        /// <returns></returns>
        private List<ComBoxItem> getStyleSource(CalculateTrajectoryType calculateTrajectoryType)
        {
            List<DispensingStyleCfgBaseInfo> dispensingStyleCfgBaseInfos = new List<DispensingStyleCfgBaseInfo>();
           DispensingStyleCfgInfo dispensingStyleCfgInfo=CacheDataExchange.GetDispensingStyleCfgInfo();
            switch (calculateTrajectoryType)
            {
                case CalculateTrajectoryType.Point:
                    //取点的点胶前和点胶后的样式
                    foreach (var item in dispensingStyleCfgInfo.DispensingPointStyleCfgInfo.DispensingBeforePointStyleCfgInfoes)
                    {
                        dispensingStyleCfgBaseInfos.Add(item);
                    }
                    foreach (var item in dispensingStyleCfgInfo.DispensingPointStyleCfgInfo.DispensingAfterPointStyleCfgInfoes)
                    {
                        dispensingStyleCfgBaseInfos.Add(item);
                    }
                    break;
                case CalculateTrajectoryType.Line:
                    //取线点胶前和点胶后的样式
                    foreach (var item in dispensingStyleCfgInfo.DispensingLineStyleCfgInfo.DispensingBeforeAfterLineStyleCfgInfoes)
                    {
                        dispensingStyleCfgBaseInfos.Add(item);
                    }
                    break;
                default:
                    break;
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
    }
}