using Avalonia.Controls;
using Griffins.UI;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.TemplateSubPage.Command;
using ReactiveUI;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.TemplateSubPage.Command
{
    /// <summary>
    /// IO指令工作区-视图模型
    /// </summary>
    public class IOWorkAreaViewModel : ReactiveObject
    {
        /// <summary>
        /// 值改变事件
        /// </summary>
        public EventHandler? AfterModified;
        #region 响应式属性
        /// <summary>
        /// IO类型下拉框
        /// </summary>
        public ComboxViewModel IOTypeViewModel { get; }

        /// <summary>
        /// 功能下拉框
        /// </summary>
        public ComboxViewModel IOFunctionViewModel { get; }

        /// <summary>
        /// 状态开关
        /// </summary>
        public ToggleSwitchViewModel IsActiveViewModel { get; }

        /// <summary>
        /// 超时时间（ms）
        /// </summary>
        public NumericViewModel TimeoutViewModel { get; }

        /// <summary>
        /// 指令序号（标签）
        /// </summary>
        public TextBlockViewModel CommandIndexViewModel { get; }

        /// <summary>
        /// IO类型
        /// </summary>
        public IOType IOType
        {
            get => (IOType)((IOTypeViewModel.SelectedItem as ComBoxItem)?.Value ?? IOType.Input);
            set
            {
                if (IOTypeViewModel.ItemsSource != null)
                {
                    var targetItem = IOTypeViewModel.ItemsSource.Cast<ComBoxItem>()
                     .FirstOrDefault(o => (IOType)o.Value == value);
                    if (targetItem != null)
                        IOTypeViewModel.SelectedItem = targetItem;
                }
               
            }
        }

        /// <summary>
        /// 功能
        /// </summary>
        public IOFunction IOFunction
        {
            get => (IOFunction)((IOFunctionViewModel.SelectedItem as ComBoxItem)?.Value ?? IOFunction.StartStopButton);
            set
            {
                if (IOFunctionViewModel.ItemsSource != null)
                {
                    var targetItem = IOFunctionViewModel.ItemsSource.Cast<ComBoxItem>()
                     .FirstOrDefault(o => (IOFunction)o.Value == value);
                    if (targetItem != null)
                        IOFunctionViewModel.SelectedItem = targetItem;
                }
               
            }
        }

        /// <summary>
        /// 状态
        /// </summary>
        public bool IsActive
        {
            get => IsActiveViewModel.IsChecked;
            set => IsActiveViewModel.IsChecked = value;
        }

        /// <summary>
        /// 超时时间（ms）
        /// </summary>
        public int Timeout
        {
            get => (int)TimeoutViewModel.Value;
            set => TimeoutViewModel.Value = value;
        }

        /// <summary>
        /// 指令序号
        /// </summary>
        public int CommandIndex
        {
            get => int.Parse(CommandIndexViewModel.Text);
            set => CommandIndexViewModel.Text = value.ToString();
        }
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public IOWorkAreaViewModel()
        {
            // IO类型下拉框
            IOTypeViewModel = new ComboxViewModel();
            var ioTypeItems = new Dictionary<IOType, string>
            {
                { IOType.Input, "输入" },
                { IOType.Output, "输出" }
            };
            IOTypeViewModel.ItemsSource = EnumExtensions.ToEnumItems(ioTypeItems);
            IOTypeViewModel.DisplayMemberPath = nameof(ComBoxItem.DisplayName);
            IOTypeViewModel.SelectedItem = IOTypeViewModel.ItemsSource.Cast<ComBoxItem>().FirstOrDefault();

            // 功能下拉框
            IOFunctionViewModel = new ComboxViewModel();
            var functionItems = new Dictionary<IOFunction, string>
            {
                { IOFunction.StartStopButton, "启停按钮" },
                { IOFunction.ResetButton, "复位按钮" },
                { IOFunction.EmergencyStopButton, "急停按钮" }
            };
            IOFunctionViewModel.ItemsSource = EnumExtensions.ToEnumItems(functionItems);
            IOFunctionViewModel.DisplayMemberPath = nameof(ComBoxItem.DisplayName);
            IOFunctionViewModel.SelectedItem = IOFunctionViewModel.ItemsSource.Cast<ComBoxItem>().FirstOrDefault();

            // 状态开关
            IsActiveViewModel = new ToggleSwitchViewModel { IsChecked = true };

            // 超时时间（ms）
            TimeoutViewModel = new NumericViewModel
            {
                Increment = 100,
                DecimalPlaces = 0,
                Minimum = 0,
                Maximum = 10000,
                Value = 1000
            };

            // 指令序号（标签，不可编辑）
            CommandIndexViewModel = new TextBlockViewModel { Text = "1" };

            // 订阅值变更事件
            subscribeValueChanges();
        }

        /// <summary>
        /// 从数据模型复制数据到ViewModel
        /// </summary>
        /// <param name="command"></param>
        public void CopyFrom(IOCommandSequence command)
        {
            if (command == null) return;
            IOType = command.IOType;
            IOFunction = command.IOFunction;
            IsActive = command.IsActive;
            Timeout = command.Timeout;
            CommandIndex = command.CommandIndex;
        }

        /// <summary>
        /// 从ViewModel复制数据到数据模型
        /// </summary>
        /// <param name="command"></param>
        public void CopyTo(IOCommandSequence command)
        {
            if (command == null) return;
            command.IOType = IOType;
            command.IOFunction = IOFunction;
            command.IsActive = IsActive;
            command.Timeout = Timeout;
            command.CommandIndex = CommandIndex;
        }
        /// <summary>
        /// 设置视图引用
        /// </summary>
        public void SetViewReference(Control view)
        {
        }
        #region 值改变事件
        /// <summary>
        /// 订阅值改变事件
        /// </summary>
        private void subscribeValueChanges()
        {
            IOTypeViewModel.ValueChanged += onValueChanged;
            IOFunctionViewModel.ValueChanged += onValueChanged;
            IsActiveViewModel.ValueChanged += onValueChanged;
            TimeoutViewModel.ValueChanged += onValueChanged;
            CommandIndexViewModel.ValueChanged += onValueChanged;
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