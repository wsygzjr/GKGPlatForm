using Griffins.UI;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.TemplateSubPage.TrajectorySequence;
using ReactiveUI;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.TemplateSubPage.TrajectorySequence
{
    /// <summary>
    /// 线轨迹模型
    /// </summary>
    public class LineTrajectoryModel : ReactiveObject
    {
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
        /// 点计算轨迹信息-视图模型
        /// </summary>
        public PointTrajectoryWorkAreaViewModel? PointTrajectoryCfgViewModel { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public LineTrajectoryModel()
        {

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

        }

        

        /// <summary>
        /// 从信息对象复制数据
        /// </summary>
        /// <param name="info"></param>
        public void CopyFrom(TrajectorySequenceCfgInfo info)
        {
            if (info == null) return;

        }

        /// <summary>
        /// 复制到信息对象
        /// </summary>
        /// <param name="targetInfo"></param>
        public void CopyTo(TrajectorySequenceCfgInfo targetInfo)
        {
            if (targetInfo == null) return;

        }
        
    }
}