using Avalonia.Controls;
using Griffins.UI;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.GlueDispensingStyle;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.TemplateSubPage.Command;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.GlueDispensingStyle;
using ReactiveUI;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.TemplateSubPage.Command
{
    /// <summary>
    /// 涂覆指令工作区-视图模型
    /// </summary>
    public class CoatingWorkAreaViewModel : ReactiveObject
    {
        /// <summary>
        /// 值改变事件
        /// </summary>
        public EventHandler? AfterModified;
        #region 响应式属性

        /// <summary>
        /// 选中的点胶中线样式ID-下拉框数据模型
        /// </summary>
        public ComboxViewModel SelectedMiddleLineStyleModel { get; }
        /// <summary>
        /// 选中的点胶前后线样式ID
        /// </summary>
        public ComboxViewModel SelectedBeforeAfterLineStyleModel { get; }

        /// <summary>
        /// 形状选择下拉框
        /// </summary>
        public ComboxViewModel CoatingShapeViewModel { get; }

        /// <summary>
        /// 重量
        /// </summary>
        public NumericViewModel WeightViewModel { get; }

        /// <summary>
        /// 重量类型下拉框
        /// </summary>
        public ComboxViewModel WeightTypeViewModel { get; }

        /// <summary>
        /// 间距（mm）
        /// </summary>
        public NumericViewModel SpacingViewModel { get; }

        /// <summary>
        /// 初始位置-视图模型
        /// </summary>

        public CamreaPositionViewModel InitialPositionViewModel { get; }
        /// <summary>
        /// X向终点位置-视图模型
        /// </summary>

        public CamreaPositionViewModel XEndPositionViewModel { get; }
        /// <summary>
        /// Y向终点位置-视图模型
        /// </summary>

        public CamreaPositionViewModel YEndPositionViewModel { get; }

        /// <summary>
        /// 选中的点胶中线样式ID
        /// </summary>
        public Guid SelectedMiddleLineStyleID
        {
            get => (Guid)((SelectedMiddleLineStyleModel.SelectedItem as ComBoxItem)?.Value ?? Guid.Empty);
            set
            {
                if (SelectedMiddleLineStyleModel.ItemsSource != null)
                {
                    var targetItem = SelectedMiddleLineStyleModel.ItemsSource.Cast<ComBoxItem>()
                    .FirstOrDefault(o => (Guid)o.Value == value);
                    if (targetItem != null)
                        SelectedMiddleLineStyleModel.SelectedItem = targetItem;
                    this.RaisePropertyChanged(nameof(SelectedMiddleLineStyleID));
                }
            }
        }


        /// <summary>
        /// 选中的点胶前后线样式ID
        /// </summary>
        public Guid SelectedBeforeAfterLineStyleID
        {
            get => (Guid)((SelectedBeforeAfterLineStyleModel.SelectedItem as ComBoxItem)?.Value ?? Guid.Empty);
            set
            {
                if (SelectedBeforeAfterLineStyleModel.ItemsSource != null)
                {
                    var targetItem = SelectedBeforeAfterLineStyleModel.ItemsSource.Cast<ComBoxItem>()
                    .FirstOrDefault(o => (Guid)o.Value == value);
                    if (targetItem != null)
                        SelectedBeforeAfterLineStyleModel.SelectedItem = targetItem;
                    this.RaisePropertyChanged(nameof(SelectedBeforeAfterLineStyleID));
                }
            }
        }
        /// <summary>
        /// 形状选择
        /// </summary>
        public CoatingShape CoatingShape
        {
            get => (CoatingShape)((CoatingShapeViewModel.SelectedItem as ComBoxItem)?.Value ?? CoatingShape.Line);
            set
            {
                if (CoatingShapeViewModel.ItemsSource != null)
                {
                    var targetItem = CoatingShapeViewModel.ItemsSource.Cast<ComBoxItem>()
                    .FirstOrDefault(o => (CoatingShape)o.Value == value);
                    if (targetItem != null)
                        CoatingShapeViewModel.SelectedItem = targetItem;
                }
               
            }
        }

        /// <summary>
        /// 重量
        /// </summary>
        public decimal Weight
        {
            get => WeightViewModel.Value;
            set => WeightViewModel.Value = value;
        }

        /// <summary>
        /// 重量类型
        /// </summary>
        public WeightType WeightType
        {
            get => (WeightType)((WeightTypeViewModel.SelectedItem as ComBoxItem)?.Value ?? WeightType.Gram);
            set
            {
                if (WeightTypeViewModel.ItemsSource != null)
                {
                    var targetItem = WeightTypeViewModel.ItemsSource.Cast<ComBoxItem>()
                    .FirstOrDefault(o => (WeightType)o.Value == value);
                    if (targetItem != null)
                        WeightTypeViewModel.SelectedItem = targetItem;
                }
            }
        }

        /// <summary>
        /// 间距（mm）
        /// </summary>
        public decimal Spacing
        {
            get => SpacingViewModel.Value;
            set => SpacingViewModel.Value = value;
        }
        #endregion


        /// <summary>
        /// 构造函数
        /// </summary>
        public CoatingWorkAreaViewModel()
        {
            InitialPositionViewModel = new CamreaPositionViewModel();
            XEndPositionViewModel = new CamreaPositionViewModel();
             YEndPositionViewModel = new CamreaPositionViewModel();


            // 初始化点胶中线样式数据源
            SelectedMiddleLineStyleModel = new ComboxViewModel
            {
                ItemsSource = getMiddleLineStyleSource(),
                DisplayMemberPath = nameof(ComBoxItem.DisplayName)
            };
            SelectedMiddleLineStyleModel.ValueChanged += (s, e) => this.RaisePropertyChanged(nameof(SelectedMiddleLineStyleID));


            // 初始化点胶前后线样式数据源
            SelectedBeforeAfterLineStyleModel = new ComboxViewModel
            {
                ItemsSource = getBeforeAfterLineStyleSource(),
                DisplayMemberPath = nameof(ComBoxItem.DisplayName)
            };
            SelectedBeforeAfterLineStyleModel.ValueChanged += (s, e) => this.RaisePropertyChanged(nameof(SelectedBeforeAfterLineStyleID));

            // 形状选择下拉框
            CoatingShapeViewModel = new ComboxViewModel();
            var shapeItems = new Dictionary<CoatingShape, string>
            {
                { CoatingShape.Line, "线" },
                { CoatingShape.Rectangle, "矩形" },
                { CoatingShape.Loop, "回形" }
            };
            CoatingShapeViewModel.ItemsSource = EnumExtensions.ToEnumItems(shapeItems);
            CoatingShapeViewModel.DisplayMemberPath = nameof(ComBoxItem.DisplayName);
            CoatingShapeViewModel.SelectedItem = CoatingShapeViewModel.ItemsSource.Cast<ComBoxItem>().FirstOrDefault();

            // 重量（最小0.01）
            WeightViewModel = new NumericViewModel
            {
                Increment = 0.01m,
                DecimalPlaces = 2,
                Minimum = 0.01m,
                Maximum = 100.00m,
                Value = 0.01m
            };

            // 重量类型下拉框
            WeightTypeViewModel = new ComboxViewModel();
            var weightTypeItems = new Dictionary<WeightType, string>
            {
                { WeightType.Gram, "克" },
                { WeightType.Milligram, "毫克" }
            };
            WeightTypeViewModel.ItemsSource = EnumExtensions.ToEnumItems(weightTypeItems);
            WeightTypeViewModel.DisplayMemberPath = nameof(ComBoxItem.DisplayName);
            WeightTypeViewModel.SelectedItem = WeightTypeViewModel.ItemsSource.Cast<ComBoxItem>().FirstOrDefault();

            // 间距（mm）
            SpacingViewModel = new NumericViewModel
            {
                Increment = 0.1m,
                DecimalPlaces = 1,
                Minimum = 0.1m,
                Maximum = 100.0m,
                Value = 1.0m
            };

            CacheDataExchange.SubscribStyleChanged(cacheData_StyleChanged);

            // 订阅值变更事件
            subscribeValueChanges();
        }

        /// <summary>
        /// 从数据模型复制数据到ViewModel
        /// </summary>
        /// <param name="coatingCommandSequence"></param>
        public void CopyFrom(CoatingCommandSequence coatingCommandSequence)
        {
            if (coatingCommandSequence == null) return;
            //CoatingStyle = coatingCommandSequence.CoatingStyle;
            SelectedMiddleLineStyleID = coatingCommandSequence.SelectedMiddleLineStyleID;
            SelectedBeforeAfterLineStyleID = coatingCommandSequence.SelectedBeforeAfterLineStyleID;
            CoatingShape = coatingCommandSequence.CoatingShape;
            Weight = coatingCommandSequence.Weight;
            WeightType = coatingCommandSequence.WeightType;
            Spacing = coatingCommandSequence.Spacing;

            InitialPositionViewModel.CopyFrom(coatingCommandSequence.InitialPositionInfo);
            XEndPositionViewModel.CopyFrom(coatingCommandSequence.XEndPositionInfo);
            YEndPositionViewModel.CopyFrom(coatingCommandSequence. YEndPositionInfo);
        }

        /// <summary>
        /// 从ViewModel复制数据到数据模型
        /// </summary>
        /// <param name="coatingCommandSequence"></param>
        public void CopyTo(CoatingCommandSequence coatingCommandSequence)
        {
            if (coatingCommandSequence == null) return;
            coatingCommandSequence.SelectedMiddleLineStyleID = SelectedMiddleLineStyleID;
            coatingCommandSequence.SelectedBeforeAfterLineStyleID = SelectedBeforeAfterLineStyleID;
            coatingCommandSequence.CoatingShape = CoatingShape;
            coatingCommandSequence.Weight = Weight;
            coatingCommandSequence.WeightType = WeightType;
            coatingCommandSequence.Spacing = Spacing;

            InitialPositionViewModel.CopyTo(coatingCommandSequence.InitialPositionInfo);
            XEndPositionViewModel.CopyTo(coatingCommandSequence.XEndPositionInfo);
            YEndPositionViewModel.CopyTo(coatingCommandSequence. YEndPositionInfo);
        }
        /// <summary>
        /// 设置视图引用
        /// </summary>
        public void SetViewReference(Control view)
        {
        }
       

        /// <summary>
        /// 获取点胶中线样式数据源
        /// </summary>
        /// <returns></returns>
        private List<ComBoxItem> getMiddleLineStyleSource()
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
        /// <summary>
        /// 获取点胶前后线线样式数据源
        /// </summary>
        /// <returns></returns>
        private List<ComBoxItem> getBeforeAfterLineStyleSource()
        {
            List<DispensingStyleCfgBaseInfo> dispensingStyleCfgBaseInfos = new List<DispensingStyleCfgBaseInfo>();
            DispensingStyleCfgInfo dispensingStyleCfgInfo = CacheDataExchange.GetDispensingStyleCfgInfo();
            //取线的点胶中的样式
            foreach (var item in dispensingStyleCfgInfo.DispensingLineStyleCfgInfo.DispensingBeforeAfterLineStyleCfgInfoes)
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
                SelectedMiddleLineStyleModel.ItemsSource = getMiddleLineStyleSource();
                SelectedMiddleLineStyleModel.SelectedItem = SelectedMiddleLineStyleModel.ItemsSource.Cast<ComBoxItem>().FirstOrDefault(o => (Guid)o.Value == SelectedMiddleLineStyleID);

                SelectedBeforeAfterLineStyleModel.ItemsSource = getBeforeAfterLineStyleSource();
                SelectedBeforeAfterLineStyleModel.SelectedItem = SelectedBeforeAfterLineStyleModel.ItemsSource.Cast<ComBoxItem>().FirstOrDefault(o => (Guid)o.Value == SelectedBeforeAfterLineStyleID);
            }
        }
        #region 值改变事件
        /// <summary>
        /// 订阅值改变事件
        /// </summary>
        private void subscribeValueChanges()
        {
            InitialPositionViewModel.AfterModified += onAfterModified;
            XEndPositionViewModel.AfterModified += onAfterModified;
            YEndPositionViewModel.AfterModified += onAfterModified;

            SelectedMiddleLineStyleModel.ValueChanged += onValueChanged;
            SelectedBeforeAfterLineStyleModel.ValueChanged += onValueChanged;
            WeightViewModel.ValueChanged += onValueChanged;
            CoatingShapeViewModel.ValueChanged += onValueChanged;
            WeightTypeViewModel.ValueChanged += onValueChanged;
            SpacingViewModel.ValueChanged += onValueChanged;
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