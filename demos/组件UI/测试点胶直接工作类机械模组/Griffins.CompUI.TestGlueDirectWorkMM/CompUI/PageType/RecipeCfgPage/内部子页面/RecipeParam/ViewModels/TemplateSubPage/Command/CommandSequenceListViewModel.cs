using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.TemplateSubPage.Command;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Linq;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Griffins.CompUI.TestGlueDirectWorkMM.CompUI.PageType;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.TemplateSubPage.Command
{
    /// <summary>
    /// 指令序列配置列表-视图模型
    /// </summary>
    public class CommandSequenceListViewModel :
        DataGridListBaseViewModel<CommandSequenceModel, CommandSequenceBaseCfgInfo>
    {
        /// <summary>
        /// 所属模板ID
        /// </summary>
        private Guid _templateID;
        #region 命令定义
        /// <summary>
        /// 添加清胶指令
        /// </summary>
        public ReactiveCommand<Unit, Unit> AddCleanCommand { get; }
        /// <summary>
        /// 添加抬针指令
        /// </summary>
        public ReactiveCommand<Unit, Unit> AddLiftNeedleCommand { get; }
        /// <summary>
        /// 添加涂覆指令
        /// </summary>
        public ReactiveCommand<Unit, Unit> AddCoatingCommand { get; }
        /// <summary>
        /// 添加延时指令
        /// </summary>
        public ReactiveCommand<Unit, Unit> AddDelayCommand { get; }
        /// <summary>
        /// 添加轨迹指令序列命令
        /// </summary>
        public ReactiveCommand<Unit, Unit> AddTrajectorySequenceCommand { get; }
        /// <summary>
        /// 添加子点胶指令
        /// </summary>
        public ReactiveCommand<Unit, Unit> AddSubDispensingCommand { get; }
        /// <summary>
        /// 添加扫二维码指令
        /// </summary>
        public ReactiveCommand<Unit, Unit> AddQrCodeCommand { get; }
        /// <summary>
        /// 添加BadMark设置指令
        /// </summary>
        public ReactiveCommand<Unit, Unit> AddBadMarkCommand { get; }
        /// <summary>
        /// 添加IO指令
        /// </summary>
        public ReactiveCommand<Unit, Unit> AddIOCommand { get; }
        /// <summary>
        /// 添加2D设置指令
        /// </summary>
        public ReactiveCommand<Unit, Unit> AddTwoDCommand { get; }
        /// <summary>
        /// 添加测高指令
        /// </summary>
        public ReactiveCommand<Unit, Unit> AddMeasurementHeightDCommand { get; }
        /// <summary>
        /// 添加测高测厚指令
        /// </summary>
        public ReactiveCommand<Unit, Unit> AddMeasurementThicknessDCommand { get; }
        /// <summary>
        /// 添加抓边指令
        /// </summary>
        public ReactiveCommand<Unit, Unit> AddClampCommand { get; }
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public CommandSequenceListViewModel() : base()
        {
            AddTrajectorySequenceCommand = ReactiveCommand.Create(() => { addCommand(DispensingCommandType.Dispensing, new DispensingCommandSequence()); }, Observable.Return(true));
            AddLiftNeedleCommand = ReactiveCommand.Create(() => { addCommand(DispensingCommandType.NeedleLift, new LiftNeedleCommandSequence()); }, Observable.Return(true));
            AddDelayCommand = ReactiveCommand.Create(() => { addCommand(DispensingCommandType.Delay, new DelayCommandSequence()); }, Observable.Return(true));
            AddCleanCommand = ReactiveCommand.Create(() => { addCommand(DispensingCommandType.Clean, new CleanCommandSequence()); }, Observable.Return(true));
            AddCoatingCommand = ReactiveCommand.Create(() => { addCommand(DispensingCommandType.Coating, new CoatingCommandSequence()); }, Observable.Return(true));
            AddIOCommand = ReactiveCommand.Create(() => { addCommand(DispensingCommandType.IO, new IOCommandSequence()); }, Observable.Return(true));
            AddBadMarkCommand = ReactiveCommand.Create(() => { addCommand(DispensingCommandType.BadMark, new BadMarkCommandSequence()); }, Observable.Return(true));
            AddTwoDCommand = ReactiveCommand.Create(() => { addCommand(DispensingCommandType.TwoD, new TwoDCommandSequence()); }, Observable.Return(true));
            AddQrCodeCommand = ReactiveCommand.Create(() => { addCommand(DispensingCommandType.QrCode, new QrCodeCommandSequence()); }, Observable.Return(true));
            AddSubDispensingCommand = ReactiveCommand.Create(() => { addCommand(DispensingCommandType.SubDispensing, new SubDispensingCommandSequence()); }, Observable.Return(true));
            AddMeasurementHeightDCommand = ReactiveCommand.Create(() => { addCommand(DispensingCommandType.MeasurementHeight, new BaseHeightMeasurementCommandSequence()); }, Observable.Return(true));
            AddMeasurementThicknessDCommand = ReactiveCommand.Create(() => { addCommand(DispensingCommandType.MeasurementThickness, new ThicknessMeasurementCommandSequence()); }, Observable.Return(true));
            AddClampCommand = ReactiveCommand.Create(() => { addCommand(DispensingCommandType.Clamp, new ClampCommandSequence()); }, Observable.Return(true));
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
        /// 从数据模型复制数据到ViewModel
        /// </summary>
        /// <param name="entityList"></param>
        public void CopyFrom(List<CommandSequenceCfgInfo> entityList)
        {
            ItemsSource.Clear();
            if (entityList == null) return;

            foreach (var entity in entityList)
            {
                var vm = new CommandSequenceModel();
                vm.SetViewReference(_viewReference!);
                vm.AfterModified += onAfterModified;
                vm.CopyFrom(entity);
                ItemsSource.Add(vm);
            }
            refreshSerialNumber();
        }

        /// <summary>
        /// 从ViewModel复制数据到数据模型
        /// </summary>
        /// <param name="targetList"></param>
        public void CopyTo(List<CommandSequenceCfgInfo> targetList)
        {
            if (targetList == null) return;

            targetList.Clear();
            foreach (var vm in ItemsSource)
            {
                var entity = new CommandSequenceCfgInfo();
                vm.CopyTo(entity);
                targetList.Add(entity);
            }
        }
        /// <summary>
        /// 添加指令
        /// </summary>
        /// <param name="dispensingCommandType">点胶指令类型枚举</param>
        /// <param name="commandSequence">命令参数对象</param>
        private void addCommand(DispensingCommandType dispensingCommandType, CommandSequenceBase commandSequence)
        {
            var newCommandSequenceModel = new CommandSequenceModel();
            newCommandSequenceModel.SetViewReference(_viewReference!);
            newCommandSequenceModel.SetTemplateID(_templateID!);
            var commandsequenceCfgInfo = new CommandSequenceCfgInfo();
            commandsequenceCfgInfo.CommandID = Guid.NewGuid();
            var prefix = "";
            var existingNames = ItemsSource.Select(item => item.SerialNumber.ToString()).ToList();
            int newSerialNumber = SerialNumberGenerator.GetMinUnusedSerialNumber(prefix, existingNames);

            commandsequenceCfgInfo.SerialNumber = newSerialNumber;
            commandsequenceCfgInfo.DispensingCommandType = dispensingCommandType;
            commandsequenceCfgInfo.CommandSequence = commandSequence;
            newCommandSequenceModel.CopyFrom(commandsequenceCfgInfo);
            newCommandSequenceModel.AfterModified += onAfterModified;
            ItemsSource.Add(newCommandSequenceModel);
            SelectedItem = newCommandSequenceModel;
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
        /// <summary>
        /// 
        /// </summary>
        protected override async Task _addItem()
        {
            throw new Exception("不支持的命令");
        }
        /// <summary>
        /// 批量删除项
        /// </summary>
        protected override async Task<bool> _batchDeleteItems()
        {
            bool success = await base._batchDeleteItems();
            if (!success)
                return success;
            refreshSerialNumber();

            return true;
        }
        #endregion
        /// <summary>
        /// 刷新所有项的SerialNumber
        /// </summary>
        private void refreshSerialNumber()
        {
            int newSerialNumber = 1;
            foreach (var item in ItemsSource)
            {
                item.SerialNumber = newSerialNumber++;
            }
        }
    }


}