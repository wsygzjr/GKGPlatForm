using Avalonia.Controls;
using DispensingPageType.Views.RecipeParamCfgPage.ParametersAndCalculation;
using Griffins.UI;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.Area;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.Area.CustomEdit;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.TemplateSubPage.Command;
using ReactiveUI;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.Area
{
    /// <summary>
    /// 点胶执行对象模型（带工作区视图）
    /// </summary>
    public class DispensingExecutionObjectModel : DispensingExecutionObjectBaseModel
    {
        /// <summary>
        /// 模板实例工作区视图实例
        /// </summary>
        private TemplateInstanceWorkAreaView? _templateInstanceWorkAreaView;

        /// <summary>
        /// 基础指令工作区视图模型
        /// </summary>
        public BasicCommandWorkAreaViewModel? BasicCommandWorkAreaViewModel { get; set; }

        /// <summary>
        /// 模板实例工作区视图模型
        /// </summary>
        public TemplateInstanceWorkAreaViewModel? TemplateInstanceWorkAreaViewModel { get; set; }

        /// <summary>
        /// 当前工作区视图
        /// 创建对象时就决定是工作区的view，所以不需要通知UI处理
        /// </summary>
        public object? WorkAreaView { get; set; }

        /// <summary>
        /// 流程名称标签-数据模型
        /// </summary>
        public TextInputViewModel ShowNameVieMode { get; }
        /// <summary>
        /// 显示名称
        /// </summary>
        public string ShowName
        {
            get => ShowNameVieMode.Text;
            set
            {
                ShowNameVieMode.Text = value;
                this.RaisePropertyChanged(nameof(ShowName));
            }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public DispensingExecutionObjectModel() : base()
        {
            ShowNameVieMode = new TextInputViewModel();
            // 订阅值变更事件
            subscribeValueChanges();
        }
        /// <summary>
        /// 设置视图引用
        /// </summary>
        public new void SetViewReference(Control view)
        {
            base.SetViewReference(view);
            _viewReference = view;

        }
        /// <summary>
        /// 根据执行对象类型更新工作区
        /// </summary>
        protected override void _onUpdateByCommandType()
        {
            base._onUpdateByCommandType();
            switch (CommandType)
            {
                case ExecutionCommandType.Template:
                    TemplateInstanceWorkAreaViewModel = new TemplateInstanceWorkAreaViewModel();
                    TemplateInstanceWorkAreaViewModel.SetViewReference(_viewReference!);
                    TemplateInstanceWorkAreaViewModel.CopyFrom(new DispensingTemplateInstanceExecutionObject());
                    TemplateInstanceWorkAreaViewModel.AfterModified += onAfterModified;
                    _templateInstanceWorkAreaView = new TemplateInstanceWorkAreaView { DataContext = TemplateInstanceWorkAreaViewModel };
                    WorkAreaView = _templateInstanceWorkAreaView;
                    break;
                case ExecutionCommandType.LiftNeedle:
                    createBasicCommandWorkArea(DispensingCommandType.NeedleLift, new LiftNeedleCommandSequence());
                    break;
                case ExecutionCommandType.Delay:
                    createBasicCommandWorkArea(DispensingCommandType.Delay, new DelayCommandSequence());
                    break;
                case ExecutionCommandType.CleanGlue:
                    createBasicCommandWorkArea(DispensingCommandType.Clean, new CleanCommandSequence());
                    break;
                default:
                    break;
            }
        }
        
        /// <summary>
        /// 从数据模型复制（包含工作区数据）
        /// </summary>
        public new void CopyFrom(DispensingExecutionObject info)
        {
            base.CopyFrom(info);
            ShowName = info.ShowName;
            switch (info.CommandType)
            {
                case ExecutionCommandType.Template:
                    if (TemplateInstanceWorkAreaViewModel == null)
                        throw new Exception("模板实例工作区视图模型为空");
                    TemplateInstanceWorkAreaViewModel.CopyFrom((DispensingTemplateInstanceExecutionObject)info.ExecutionObject);
                    break;
                case ExecutionCommandType.LiftNeedle:
                case ExecutionCommandType.Delay:
                case ExecutionCommandType.CleanGlue:
                    if (BasicCommandWorkAreaViewModel == null)
                        throw new Exception("基础指令工作区视图模型为空");
                    BasicCommandWorkAreaViewModel.CopyFrom((BasicDispensingCommandExecutionObject)info.ExecutionObject);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 复制到数据模型（包含工作区数据）
        /// </summary>
        public new void CopyTo(DispensingExecutionObject targetInfo)
        {
            base.CopyTo(targetInfo);
            targetInfo.ShowName =ShowName;
            switch (targetInfo.CommandType)
            {
                case ExecutionCommandType.Template:
                    if (TemplateInstanceWorkAreaViewModel == null)
                        throw new Exception("模板实例工作区视图模型为空");
                    var pointCalculateTrajectory = new DispensingTemplateInstanceExecutionObject();
                    TemplateInstanceWorkAreaViewModel.CopyTo(pointCalculateTrajectory);
                    targetInfo.ExecutionObject = pointCalculateTrajectory;
                    break;
                case ExecutionCommandType.LiftNeedle:
                case ExecutionCommandType.Delay:
                case ExecutionCommandType.CleanGlue:
                    if (BasicCommandWorkAreaViewModel == null)
                        throw new Exception("基础指令工作区视图模型为空");
                    var basicDispensingCommandExecutionObject = new BasicDispensingCommandExecutionObject();
                    BasicCommandWorkAreaViewModel.CopyTo(basicDispensingCommandExecutionObject);
                    targetInfo.ExecutionObject = basicDispensingCommandExecutionObject;

                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 创建基础指令工作区
        /// </summary>
        /// <param name="dispensingCommandType"></param>
        /// <param name="commandSequence"></param>
        private void createBasicCommandWorkArea(DispensingCommandType dispensingCommandType, CommandSequenceBase commandSequence)
        {
            BasicCommandWorkAreaViewModel = new BasicCommandWorkAreaViewModel();
            BasicCommandWorkAreaViewModel.SetViewReference(_viewReference!);
            WorkAreaView = BasicCommandWorkAreaViewModel.CopyFrom(new BasicDispensingCommandExecutionObject()
            {
                DispensingCommandType = dispensingCommandType,
                CommandSequence = commandSequence
            });
            BasicCommandWorkAreaViewModel.AfterModified += onAfterModified;

        }
        #region 值改变事件
        /// <summary>
        /// 订阅值改变事件
        /// </summary>
        private void subscribeValueChanges()
        {
          
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