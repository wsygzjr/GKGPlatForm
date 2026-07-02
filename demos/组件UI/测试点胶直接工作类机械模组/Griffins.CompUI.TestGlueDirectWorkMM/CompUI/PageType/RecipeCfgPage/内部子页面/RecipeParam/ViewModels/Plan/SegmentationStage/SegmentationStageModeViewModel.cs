using Avalonia.Controls;
using Griffins.UI;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.Plan;
using ReactiveUI;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.Plan
{
    /// <summary>
    /// 分段模式设置参数-视图模型
    /// </summary>
    public class SegmentationStageModeViewModel : ReactiveObject
    {
        /// <summary>
        /// 值改变事件
        /// </summary>
        public EventHandler? AfterModified; 
        /// <summary>
        /// 分段模式下拉框-数据模型
        /// </summary>
        public ComboxViewModel SegmentModeComboBox { get; }
        /// <summary>
        /// 分段模式
        /// </summary>
        public SegmentMode SegmentMode
        {
            get => (SegmentMode)((SegmentModeComboBox.SelectedItem as ComBoxItem)?.Value ?? SegmentMode.BoardSplitting);
            set
            {
                if (SegmentModeComboBox.ItemsSource != null)
                {
                    var targetItem = SegmentModeComboBox.ItemsSource.Cast<ComBoxItem>()
                    .FirstOrDefault(o => (SegmentMode)o.Value == value);
                    if (targetItem != null)
                        SegmentModeComboBox.SelectedItem = targetItem;
                    this.RaisePropertyChanged(nameof(SegmentMode));
                }
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public SegmentationStageModeViewModel()
        {
            // 初始化编辑方式下拉框
            SegmentModeComboBox = new ComboxViewModel();
            //SegmentModeComboBox.ValueChanged += onSegmentModeChanged;
            initSegmentModeComboBox();
            // 订阅值变更事件
            subscribeValueChanges();
        }

        /// <summary>
        /// 从数据模型复制数据到ViewModel
        /// </summary>
        /// <param name="segmentationStagePlanInfo"></param>
        public void CopyFrom(SegmentationStagePlanInfo segmentationStagePlanInfo)
        {
            SegmentMode = segmentationStagePlanInfo.SegmentMode;
        }

        /// <summary>
        /// 从ViewModel复制数据到数据模型
        /// </summary>
        /// <param name="segmentationStagePlanInfo"></param>
        public void CopyTo(SegmentationStagePlanInfo segmentationStagePlanInfo)
        {
            segmentationStagePlanInfo.SegmentMode= SegmentMode;
        }
        /// <summary>
        /// 设置视图引用（用于弹窗）
        /// </summary>
        public void SetViewReference(Control view)
        {

        }
        /// <summary>
        /// 初始化编辑方式下拉框数据源
        /// </summary>
        private void initSegmentModeComboBox()
        {
            var segmentModeItems = new List<ComBoxItem>
            {
                new ComBoxItem { Value = SegmentMode.BoardSplitting, DisplayName = "分板模式" },
                new ComBoxItem { Value = SegmentMode.SecondBoardFeeding, DisplayName = "二次进板模式" },
                new ComBoxItem { Value = SegmentMode.ThirdBoardFeeding, DisplayName = "三次进板模式" }
            };

            SegmentModeComboBox.ItemsSource = segmentModeItems;
            SegmentModeComboBox.DisplayMemberPath = nameof(ComBoxItem.DisplayName);
            SegmentModeComboBox.SelectedItem = segmentModeItems.FirstOrDefault();
        }
        #region 值改变事件
        /// <summary>
        /// 订阅值改变事件
        /// </summary>
        private void subscribeValueChanges()
        {
            SegmentModeComboBox.ValueChanged += onValueChanged;
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