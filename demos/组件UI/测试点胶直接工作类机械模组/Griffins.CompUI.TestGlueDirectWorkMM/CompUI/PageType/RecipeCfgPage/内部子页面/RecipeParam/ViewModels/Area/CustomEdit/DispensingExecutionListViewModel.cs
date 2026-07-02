using Avalonia.Controls;
using DynamicData;
using Griffins.UI;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.Area;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.Area.CustomEdit;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.TemplateSubPage.Command;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Linq;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Griffins.CompUI.TestGlueDirectWorkMM.CompUI.PageType;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.Area
{
    /// <summary>
    /// 点胶执行对象列表视图模型
    /// </summary>
    public class DispensingExecutionListViewModel :
        DataGridListBaseViewModel<DispensingExecutionObjectModel, DispensingExecutionObject>
    {

        /// <summary>
        /// 添加的指令序列类型-下拉框数据模型
        /// </summary>
        public ComboxViewModel DispensingCommandTypeModel { get; }
        #region 命令定义
        /// <summary>
        /// 添加基础指令
        /// </summary>
        public ReactiveCommand<Unit, Unit> AddBasicCommandCommand { get; }

        /// <summary>
        /// 添加模板实例命令
        /// </summary>
        public ReactiveCommand<Unit, Unit> AddTemplateInstanceCommand { get; }
        #endregion

        /// <summary>
        /// 指令序列类型
        /// </summary>
        public DispensingCommandType DispensingCommandType
        {
            get => (DispensingCommandType)((DispensingCommandTypeModel.SelectedItem as ComBoxItem)?.Value ?? DispensingCommandType.Clean);
            set
            {
                if (DispensingCommandTypeModel.ItemsSource != null)
                {
                    var targetItem = DispensingCommandTypeModel.ItemsSource.Cast<ComBoxItem>()
                    .FirstOrDefault(o => (DispensingCommandType)o.Value == value);
                    if (targetItem != null)
                        DispensingCommandTypeModel.SelectedItem = targetItem;
                    this.RaisePropertyChanged(nameof(DispensingCommandType));
                }
            }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public DispensingExecutionListViewModel() : base()
        {
            AddBasicCommandCommand = ReactiveCommand.Create(addBasicCommand, Observable.Return(true));
            AddTemplateInstanceCommand = ReactiveCommand.Create(addTemplateInstance, Observable.Return(true));

            // 初始化指令类型下拉框
            DispensingCommandTypeModel = new ComboxViewModel();
            List<ComBoxItem> dataItems = new List<ComBoxItem>();
            dataItems.Add(new ComBoxItem()
            {
                Value = DispensingCommandType.Clean,
                DisplayName = "清洁"
            });
            dataItems.Add(new ComBoxItem()
            {
                Value = DispensingCommandType.Delay,
                DisplayName = "延时"
            });
            dataItems.Add(new ComBoxItem()
            {
                Value = DispensingCommandType.NeedleLift,
                DisplayName = "抬针"
            });
            DispensingCommandTypeModel.ItemsSource = dataItems;
            DispensingCommandTypeModel.DisplayMemberPath = nameof(ComBoxItem.DisplayName);
            DispensingCommandTypeModel.IsEnabled = false;
            DispensingCommandTypeModel.ValueChanged += (s, e) => this.RaisePropertyChanged(nameof(DispensingCommandType));
            DispensingCommandTypeModel.SelectedItem = DispensingCommandTypeModel.ItemsSource.Cast<ComBoxItem>().First();

        }
        /// <summary>
        /// 获取模板实例列表
        /// </summary>
        /// <returns></returns>
        public List<DispensingTemplateInstanceExecutionObject> GetCommandTemplateInstance()
        {
            var intances = new List<DispensingTemplateInstanceExecutionObject>();
            int index = 0;
            var dispensingExecutionObjectModels = this.ItemsSource.Cast<DispensingExecutionObjectModel>().ToList().FindAll(o => o.ObjectType == ExecutionObjectType.TemplateInstance);
            foreach (var item in dispensingExecutionObjectModels)
            {
                if (item.TemplateInstanceWorkAreaViewModel == null)
                    throw new Exception("模板实例工作区视图模型为空");
                var intance = new DispensingTemplateInstanceExecutionObject();
                intance.SerialNumber = index;
                item.TemplateInstanceWorkAreaViewModel.CopyTo(intance);
                intances.Add(intance);
                index++;
            }
            return intances;
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
        /// 添加模板实例
        /// </summary>
        private void addTemplateInstance()
        {
            var prefix = "";
            var existingNames = ItemsSource.Select(item => item.SerialNumber.ToString()).ToList();
            int newSerialNumber = SerialNumberGenerator.GetMinUnusedSerialNumber(prefix, existingNames);

            var newModel = new DispensingExecutionObjectModel
            {
                InstanceId = Guid.NewGuid(),
                SerialNumber = newSerialNumber,
            };
            newModel.ShowName = $"模板{newSerialNumber}";
            newModel.SetViewReference(_viewReference!);
            newModel.ObjectType = ExecutionObjectType.TemplateInstance;
            newModel.CommandType = ExecutionCommandType.Template;
            newModel.AfterModified += onAfterModified;
            ItemsSource.Add(newModel);

            SelectedItem = newModel;
        }
       
        /// <summary>
        /// 添加基础指令
        /// </summary>
        private void addBasicCommand()
        {
            var prefix = "";
            var existingNames = ItemsSource.Select(item => item.SerialNumber.ToString()).ToList();
            int newSerialNumber = SerialNumberGenerator.GetMinUnusedSerialNumber(prefix, existingNames);

            var newModel = new DispensingExecutionObjectModel
            {
                InstanceId = Guid.NewGuid(),
                SerialNumber = newSerialNumber,
            };
            newModel.ShowName = $"指令{newSerialNumber}";
            newModel.SetViewReference(_viewReference!);
            newModel.ObjectType = ExecutionObjectType.BasicCommand;
            newModel.CommandType = toExecutionCommandType(DispensingCommandType);
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
        /// <summary>
        /// 指令类型转换
        /// </summary>
        /// <param name="dispensingCommandType"></param>
        /// <returns></returns>
        private ExecutionCommandType toExecutionCommandType(DispensingCommandType dispensingCommandType)
        {
            switch (dispensingCommandType)
            {
                case DispensingCommandType.Clean:
                    return ExecutionCommandType.CleanGlue;
                case DispensingCommandType.Delay:
                    return ExecutionCommandType.Delay;
                case DispensingCommandType.NeedleLift:
                    return ExecutionCommandType.LiftNeedle;
                case DispensingCommandType.Coating:
                    break;
                case DispensingCommandType.Dispensing:
                    break;
                case DispensingCommandType.SubDispensing:
                    break;
                case DispensingCommandType.QrCode:
                    break;
                case DispensingCommandType.BadMark:
                    break;
                case DispensingCommandType.IO:
                    break;
                case DispensingCommandType.TwoD:
                    break;
                case DispensingCommandType.MeasurementHeight:
                    break;
                case DispensingCommandType.MeasurementThickness:
                    break;
                case DispensingCommandType.Segment:
                    break;
                case DispensingCommandType.EdgeGrasping:
                    break;
                case DispensingCommandType.ValveOpening:
                    break;
                default:
                    break;
            }
            return ExecutionCommandType.Template;
        }

        #region 重载基类方法
        protected override async Task _addItem()
        {
            throw new Exception("不支持的操作");
        }
       
        #endregion
    }
}