using Avalonia.Controls;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.Plan;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Linq;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Griffins.CompUI.TestGlueDirectWorkMM.CompUI.PageType;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.Plan
{
    /// <summary>
    /// 点胶流程列表视图模型
    /// </summary>
    public class  DispensingProcessListViewModel :
        DataGridListBaseViewModel<DispensingProcessItemViewModel, DispensingProcessItemInfo>
    {
        #region 命令定义
        /// <summary>
        /// 添加单点胶指令
        /// </summary>
        public ReactiveCommand<Unit, Unit> AddSingleStageCommand { get; }

        /// <summary>
        /// 添加分段进板流程命令
        /// </summary>
        public ReactiveCommand<Unit, Unit> AddSegmentationStageCommand { get; }
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public DispensingProcessListViewModel() : base()
        {
            AddSingleStageCommand = ReactiveCommand.Create(addSingleStageCommand, Observable.Return(true));
            AddSegmentationStageCommand = ReactiveCommand.Create(addSegmentationStage, Observable.Return(true));
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
        /// 添加分段进板流程
        /// </summary>
        private void addSegmentationStage()
        {
            var prefix = "";
            var existingNames = ItemsSource.Select(item => item.SerialNumber.ToString()).ToList();
            int newSerialNumber = SerialNumberGenerator.GetMinUnusedSerialNumber(prefix, existingNames);

            DispensingProcessItemInfo entity = new DispensingProcessItemInfo()
            {
                ID = Guid.NewGuid(),
                SerialNumber = newSerialNumber,
                DispensingProcessType= DispensingProcessType.SegmentationStage,
                DispensingProcessInfo=new SegmentationStagePlanInfo()
            };
            var newModel = new DispensingProcessItemViewModel();
            newModel.ProcessName = $"分段{newSerialNumber}";
            newModel.SetViewReference(_viewReference!);
            newModel.CopyFrom(entity);
            newModel.AfterModified += onAfterModified;
            ItemsSource.Add(newModel);
            SelectedItem = newModel;
        }
        /// <summary>
        /// 添加单点胶指令
        /// </summary>
        private void addSingleStageCommand()
        {
            var prefix = "";
            var existingNames = ItemsSource.Select(item => item.SerialNumber.ToString()).ToList();
            int newSerialNumber = SerialNumberGenerator.GetMinUnusedSerialNumber(prefix, existingNames);

            DispensingProcessItemInfo entity = new DispensingProcessItemInfo()
            {
                ID = Guid.NewGuid(),
                SerialNumber = newSerialNumber,
                DispensingProcessType = DispensingProcessType.SingleStage,
                DispensingProcessInfo = new SingleStagePlanInfo()
            };
            var newModel = new DispensingProcessItemViewModel();
            newModel.ProcessName = $"流程{newSerialNumber}";
            newModel.SetViewReference(_viewReference!);
            newModel.CopyFrom(entity);
            newModel.AfterModified += onAfterModified;
            ItemsSource.Add(newModel);

            SelectedItem = newModel;
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
        #region 重载基类方法
        protected override async Task _addItem()
        {
           throw new Exception("不支持该操作");
        }
       
        #endregion
    }
}