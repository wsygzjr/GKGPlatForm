using Avalonia.Controls;
using DispensingPageType.Views.RecipeParamCfgPage.ParametersAndCalculation;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.TemplateSubPage;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.TemplateSubPage.Command;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.TemplateSubPage.Mark;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.TemplateSubPage.SubTemplate;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.TemplateSubPage.TrajectorySequence;
using ReactiveUI;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.TemplateSubPage
{
    /// <summary>
    /// 模板配置-视图模型
    /// </summary>
    public class TemplateCfgViewModel : ReactiveObject
    {
        /// <summary>
        /// 视图引用（用于获取窗口上下文，显示对话框）
        /// </summary> 
        private Control? _viewReference;
        private Guid _templateID;
        private string _templateName = "";
        /// <summary>
        /// 值改变事件
        /// </summary>
        public EventHandler? AfterModified;
        /// <summary>
        /// 模板ID
        /// </summary>
        public Guid TemplateID
        {
            get
            {
                return _templateID;
            }
            set
            {
                _templateID = value;
            }
        }

        /// <summary>
        /// 模板名称
        /// </summary>
        public string TemplateName
        {
            get
            {
                return _templateName;
            }
            set
            {
                _templateName = value;
            }
        }
        /// <summary>
        /// 缓存的视图实例（避免切换Tab时重建）
        /// </summary>
        public TemplateCfgView CachedView { get;  }

        /// <summary>
        /// 轨迹序列配置
        /// </summary>
        public TrajectorySequenceListViewModel TrajectorySequenceListViewModel { get; }
        /// <summary>
        /// Mark配置
        /// </summary>
        public MarkConfigViewModel MarkConfigViewModel { get; }
        /// <summary>
        /// 子模板配置参数
        /// </summary>
        public SubTemplateConfigViewModel SubTemplateConfigViewModel { get; }

        /// <summary>
        /// 指令序列配置
        /// </summary>
        public CommandSequenceListViewModel CommandSequenceListViewModel { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public TemplateCfgViewModel()
        {
            TemplateID=Guid.NewGuid();
            TrajectorySequenceListViewModel=new TrajectorySequenceListViewModel();
            MarkConfigViewModel = new MarkConfigViewModel(); 

            SubTemplateConfigViewModel=new SubTemplateConfigViewModel();
            CommandSequenceListViewModel = new CommandSequenceListViewModel();

            // 初始化时创建视图实例并缓存
            CachedView = new TemplateCfgView { DataContext = this };
            // 订阅值变更事件
            subscribeValueChanges();
        }

        /// <summary>
        /// 从数据模型复制数据到ViewModel
        /// </summary>
        /// <param name="templateCfgInfo"></param>
        public void CopyFrom(TemplateCfgInfo templateCfgInfo)
        {
            if (templateCfgInfo == null)
                return;
            TemplateID = templateCfgInfo.TemplateID;
            TemplateName = templateCfgInfo.TemplateName;

            SubTemplateConfigViewModel.SetTemplateID(TemplateID);
            CommandSequenceListViewModel.SetTemplateID(TemplateID);

            TrajectorySequenceListViewModel.CopyFrom(templateCfgInfo.TrajectorySequenceCfgInfoes.ToList());
            MarkConfigViewModel.CopyFrom(templateCfgInfo.MarkConfigInfo);
            SubTemplateConfigViewModel.CopyFrom(templateCfgInfo.SubTemplateConfigInfo);
            CommandSequenceListViewModel.CopyFrom(templateCfgInfo.CommandSequenceCfgInfoes.ToList());
        }


        /// <summary>
        /// 设置视图引用（用于弹窗、对话框等UI操作）
        /// </summary>
        public void SetViewReference(Control view)
        {
            _viewReference = view;
            TrajectorySequenceListViewModel.SetViewReference(view);
            MarkConfigViewModel.SetViewReference(view);
            SubTemplateConfigViewModel.SetViewReference(view);
            CommandSequenceListViewModel.SetViewReference(view);
        }

        /// <summary>
        /// 从ViewModel复制数据到数据模型
        /// </summary>
        /// <param name="controlCardBaseCfg"></param>
        public void CopyTo(TemplateCfgInfo templateCfgInfo)
        {
            templateCfgInfo.TemplateID = TemplateID;
            templateCfgInfo.TemplateName = TemplateName;
            TrajectorySequenceListViewModel.CopyTo(templateCfgInfo.TrajectorySequenceCfgInfoes);
            MarkConfigViewModel.CopyTo(templateCfgInfo.MarkConfigInfo);
            SubTemplateConfigViewModel.CopyTo(templateCfgInfo.SubTemplateConfigInfo);
            CommandSequenceListViewModel.CopyTo(templateCfgInfo.CommandSequenceCfgInfoes);
        }
        #region 值改变事件
        /// <summary>
        /// 订阅值改变事件
        /// </summary>
        private void subscribeValueChanges()
        {
            TrajectorySequenceListViewModel.AfterModified += onAfterModified;
            MarkConfigViewModel.AfterModified += onAfterModified;
            SubTemplateConfigViewModel.AfterModified += onAfterModified;
            CommandSequenceListViewModel.AfterModified += onAfterModified;
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