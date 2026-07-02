using Avalonia.Controls;
using Griffins.UI;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.Area.CustomEdit;
using ReactiveUI;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.Area
{
    /// <summary>
    /// 点胶执行对象基类模型
    /// </summary>
    public class DispensingExecutionObjectBaseModel : DataGridItemBaseViewModel<DispensingExecutionObject>
    {

        /// <summary>
        /// 执行对象类型
        /// </summary>
        public ExecutionObjectType ObjectType { get; set; } = ExecutionObjectType.BasicCommand;

        #region UI组件模型

        /// <summary>
        /// 执行指令类型-下拉框
        /// </summary>
        public ComboxViewModel CommandTypeModel { get; }

        #endregion

        #region 响应式属性


        /// <summary>
        /// 执行指令类型
        /// </summary>
        public ExecutionCommandType CommandType
        {
            get => (ExecutionCommandType)((CommandTypeModel.SelectedItem as ComBoxItem)?.Value ?? ExecutionCommandType.LiftNeedle);
            set
            {
                if (CommandTypeModel.ItemsSource != null)
                {
                    var targetItem = CommandTypeModel.ItemsSource.Cast<ComBoxItem>()
                        .FirstOrDefault(o => (ExecutionCommandType)o.Value == value);

                    if (targetItem != null)
                    {
                        CommandTypeModel.SelectedItem = targetItem;
                        _onUpdateByCommandType();
                        this.RaisePropertyChanged(nameof(CommandType));
                    }
                }
            }
        }
        /// <summary>
        /// 执行指令类型
        /// </summary>
        protected virtual void _onUpdateByCommandType()
        {
            
        }
        /// <summary>
        /// 执行对象实例ID
        /// </summary>
        private Guid _instanceId = Guid.NewGuid();
        public Guid InstanceId
        {
            get => _instanceId;
            set => this.RaiseAndSetIfChanged(ref _instanceId, value);
        }
        #endregion


        /// <summary>
        /// 构造函数
        /// </summary>
        public DispensingExecutionObjectBaseModel()
        {
            // 初始化UI组件
            CommandTypeModel = new ComboxViewModel();

            // 初始化数据源
            initCommandTypeSource();


            // 订阅事件
            subscribeEvents();
        }

        #region 初始化方法

        /// <summary>
        /// 初始化指令类型数据源
        /// </summary>
        private void initCommandTypeSource()
        {
            var commandTypeItems = new Dictionary<ExecutionCommandType, string>
            {
                { ExecutionCommandType.LiftNeedle, "抬针" },
                { ExecutionCommandType.Delay, "延时" },
                { ExecutionCommandType.CleanGlue, "清胶" },
                { ExecutionCommandType.Template, "模板" }
            };

            CommandTypeModel.ItemsSource = EnumExtensions.ToEnumItems(commandTypeItems);
            CommandTypeModel.DisplayMemberPath = nameof(ComBoxItem.DisplayName);
            CommandTypeModel.SelectedItem = CommandTypeModel.ItemsSource.Cast<ComBoxItem>().FirstOrDefault(o => (ExecutionCommandType)o.Value == ExecutionCommandType.LiftNeedle);
        }

       
        #endregion

        #region 更新逻辑
       
       
        #endregion

        #region 事件订阅
        /// <summary>
        /// 订阅子组件事件
        /// </summary>
        private void subscribeEvents()
        {

            CommandTypeModel.ValueChanged += (s, e) =>
            {
                this.RaisePropertyChanged(nameof(CommandType));
            };

        }
        #endregion

        #region 数据同步
        /// <summary>
        /// 从数据模型复制
        /// </summary>
        public override void CopyFrom(DispensingExecutionObject info)
        {
            if (info == null) return;

            ObjectType = info.ObjectType;
            CommandType = info.CommandType;

            //if (info.ExecutionObject is DispensingTemplateInstanceExecutionObject templateObj)
            //{
            //    //IsEnabled = templateObj.IsEnabled;
            //    //SelectedTemplateId = templateObj.TemplateInstance.TemplateId;
            //}
            //else if (info.ExecutionObject is BasicDispensingCommandExecutionObject basicObj)
            //{
            //    //DelayTime = basicObj.DelayTime;
            //    //LiftHeight = basicObj.LiftHeight;
            //}
        }

        /// <summary>
        /// 复制到数据模型
        /// </summary>
        public override void CopyTo(DispensingExecutionObject targetInfo)
        {
            if (targetInfo == null) return;

            targetInfo.ObjectType = ObjectType;
            targetInfo.CommandType = CommandType;

            //switch (ObjectType)
            //{
            //    case ExecutionObjectType.BasicCommand:
            //        var basicObj = new BasicDispensingCommandExecutionObject
            //        {
            //            InstanceId = InstanceId,
            //            //CommandType = CommandType switch
            //            //{
            //            //    ExecutionCommandType.LiftNeedle => BasicDispensingCommandType.LiftNeedle,
            //            //    ExecutionCommandType.Delay => BasicDispensingCommandType.Delay,
            //            //    ExecutionCommandType.CleanGlue => BasicDispensingCommandType.CleanGlue,
            //            //    _ => BasicDispensingCommandType.LiftNeedle
            //            //},
            //        };
            //        targetInfo.ExecutionObject = basicObj;
            //        break;

            //    case ExecutionObjectType.TemplateInstance:
            //        var templateObj = new DispensingTemplateInstanceExecutionObject
            //        {
            //            InstanceId = InstanceId,
            //            //TemplateInstance = new DispensingCommandTemplateInstance
            //            //{
            //            //    TemplateId = SelectedTemplateId
            //            //}
            //        };
            //        targetInfo.ExecutionObject = templateObj;
            //        break;
            //}
        }
        #endregion

    }
}