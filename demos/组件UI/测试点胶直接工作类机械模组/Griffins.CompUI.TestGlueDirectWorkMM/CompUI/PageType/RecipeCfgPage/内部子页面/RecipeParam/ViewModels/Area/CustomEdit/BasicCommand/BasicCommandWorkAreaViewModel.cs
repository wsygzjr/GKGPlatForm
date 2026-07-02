using Avalonia.Controls;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.Area;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.Area.CustomEdit;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.TemplateSubPage.Command;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.TemplateSubPage.Command;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.Area
{
    /// <summary>
    /// 基础指令工作区视图模型
    /// </summary>
    public class BasicCommandWorkAreaViewModel
    {
        /// <summary>
        /// 页面布局对象
        /// </summary>
        private Control? _viewReference;
        /// <summary>
        /// 所属模板ID
        /// </summary>
        private Guid _templateID;
        /// <summary>
        /// 
        /// </summary>
        private CommandSequenceModel? _commandSequenceModel { set; get; }
        /// <summary>
        /// 值改变事件
        /// </summary>
        public EventHandler? AfterModified;
        public BasicCommandWorkAreaViewModel()
        {
            // 订阅值变更事件
            subscribeValueChanges();
        }
        /// <summary>
        /// 设置视图引用
        /// </summary>
        public  void SetViewReference(Control view)
        {
            _viewReference = view;
        }
        /// <summary>
        /// 设置所属模板ID
        /// </summary>
        /// <param name="templateID"></param>
        public void SetTemplateID(Guid templateID)
        {
            this._templateID = templateID;
        }
        /// <summary>
        /// 从源复制
        /// </summary>
        public object? CopyFrom(BasicDispensingCommandExecutionObject source)
        {
            _commandSequenceModel = new CommandSequenceModel();
            _commandSequenceModel.SetViewReference(_viewReference!);
            _commandSequenceModel.SetTemplateID(_templateID!);
            var commandsequenceCfgInfo = new CommandSequenceCfgInfo();
            commandsequenceCfgInfo.CommandID = Guid.NewGuid();

            //commandsequenceCfgInfo.SerialNumber = newSerialNumber;
            commandsequenceCfgInfo.DispensingCommandType = source.DispensingCommandType;
            commandsequenceCfgInfo.CommandSequence = source.CommandSequence;
            _commandSequenceModel.CopyFrom(commandsequenceCfgInfo);
            _commandSequenceModel.AfterModified += onAfterModified;
            return _commandSequenceModel.WorkAreaView;
        }
      

        /// <summary>
        /// 复制到目标
        /// </summary>
        /// <param name="target"></param>
        public void CopyTo(BasicDispensingCommandExecutionObject target)
        {
            if (_commandSequenceModel == null)
                return;
            CommandSequenceCfgInfo targetInfo=new CommandSequenceCfgInfo();
            _commandSequenceModel.CopyTo(targetInfo);
            target.DispensingCommandType = targetInfo.DispensingCommandType;
            target.CommandSequence = targetInfo.CommandSequence;
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
        private void onAfterModified(object? sender, EventArgs e)
        {
            AfterModified?.Invoke(sender, e);
        }
        #endregion
    }
}
