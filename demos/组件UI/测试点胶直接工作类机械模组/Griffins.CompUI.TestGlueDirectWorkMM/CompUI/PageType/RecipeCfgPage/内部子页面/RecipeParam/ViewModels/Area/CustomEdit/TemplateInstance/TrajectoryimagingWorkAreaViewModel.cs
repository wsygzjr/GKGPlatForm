using DynamicData;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.TemplateSubPage.TrajectorySequence;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.TemplateSubPage.TrajectorySequence;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.Area
{
    /// <summary>
    /// 轨迹成像工作区-视图模型
    /// </summary>
    public class TrajectoryimagingWorkAreaViewModel :
        DataGridListBaseViewModel<TrajectorySequenceBaseModel, TrajectorySequenceCfgInfo>
    {
        /// <summary>
        /// 轨迹指令下的轨迹序列
        /// </summary>
        public List<TrajectorySequenceModel> TrajectorySequence { set; get; }
        /// <summary>
        /// 构造函数
        /// </summary>
        public TrajectoryimagingWorkAreaViewModel() :base()
        {
            TrajectorySequence = new List<TrajectorySequenceModel>();

        }
        /// <summary>
        /// 重新加载指定模板中的轨迹
        /// </summary>
        /// <param name="templateID"></param>
        public void ReLoadTrajectory(Guid templateID)
        {
            this.ItemsSource.Clear();
            var cmdInfoes = CacheDataExchange.GetTrajectorySequenceModel(templateID);
            this.ItemsSource.AddRange(cmdInfoes);
        }
       
        #region 重载基类方法
        /// <summary>
        /// 
        /// </summary>
        protected override async Task _addItem()
        {
            throw new Exception("不支持的操作");
        }

        #endregion
       
    }
}