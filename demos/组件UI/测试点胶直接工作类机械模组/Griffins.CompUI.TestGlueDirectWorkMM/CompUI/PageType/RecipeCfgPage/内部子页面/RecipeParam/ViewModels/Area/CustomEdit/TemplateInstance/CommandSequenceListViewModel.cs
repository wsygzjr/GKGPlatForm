using DynamicData;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.TemplateSubPage.Command;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.TemplateSubPage.Command;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.TemplateSubPage.TrajectorySequence;
using ReactiveUI;
using System.Reactive.Linq;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.Area
{
    /// <summary>
    /// 模板中的指令序列配置列表-视图模型
    /// </summary>
    public class DispensingCommandSequenceListViewModel :
        DataGridListBaseViewModel<CommandSequenceBaseModel, CommandSequenceBaseCfgInfo>
    {
        /// <summary>
        /// 指令序列
        /// </summary>
        
        private List<CommandSequenceModel>? _commandSequenceModels;
        /// <summary>
        /// 轨迹成像工作区
        /// </summary>
        private TrajectoryimagingWorkAreaViewModel _trajectoryimagingWorkAreaViewModel { get; }
        /// <summary>
        /// 模板ID
        /// </summary>
        private Guid _templateID;
        /// <summary>
        /// 构造函数
        /// </summary>
        public DispensingCommandSequenceListViewModel(TrajectoryimagingWorkAreaViewModel trajectoryimagingWorkAreaViewModel) : base()
        {
            _trajectoryimagingWorkAreaViewModel = trajectoryimagingWorkAreaViewModel;
            this.WhenAnyValue(
              vm => vm.SelectedItem
          ).Subscribe(_ =>
          {
              //如果是轨迹指令，则将指令中的轨迹序列展示到轨迹成像框
              if((base.SelectedItem is CommandSequenceBaseModel commandSequenceBaseModel)&&
              commandSequenceBaseModel.DispensingCommandType== DispensingCommandType.Dispensing&&
              _commandSequenceModels!=null)
              {
                  //查找点胶轨迹指令对象
                  var commandSequenceModel=_commandSequenceModels.Find(o => o.DispensingCommandType == DispensingCommandType.Dispensing);
                  if(commandSequenceModel!=null)
                  {
                      //获取指定对应的轨迹序列基础信息
                      List<TrajectorySequenceBaseModel> trajectorySequenceBaseModels = commandSequenceModel.GetTrajectorySequenceBaseModel();
                      //查找轨迹序列列表
                      var sequenceInfoes = CacheDataExchange.GetTrajectorySequenceModel(_templateID);
                      var selectedInfoes = sequenceInfoes.FindAll(O => trajectorySequenceBaseModels.Select(T => T.TrackID).Contains(O.TrackID));
                      _trajectoryimagingWorkAreaViewModel.TrajectorySequence = selectedInfoes;
                  }
              }
          });
          
        }
        /// <summary>
        /// 重新加载模板下的指令
        /// </summary>
        /// <param name="templateID"></param>
        public void ReLoadCommandSequence(Guid templateID)
        {
            _templateID = templateID;
            this.ItemsSource.Clear();
            var cmdInfoes = CacheDataExchange.GetCommandSequenceModel(templateID);
            this.ItemsSource.AddRange(cmdInfoes);
            _commandSequenceModels = cmdInfoes;
        }
        #region 重载基类方法
        /// <summary>
        /// 
        /// </summary>
        protected override async Task _addItem()
        {
            throw new Exception("不支持的命令");
        }
        #endregion
       
    }


}