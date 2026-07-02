using Avalonia.Controls;
using Avalonia.VisualTree;
using Griffins.UI;
using Griffins.UI.General;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.TemplateSubPage.Command;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.TemplateSubPage.Mark;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Linq;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.TemplateSubPage.Command
{
    /// <summary>
    /// BadMark设置指令工作区-视图模型
    /// </summary>
    public class BadMarkWorkAreaViewModel : ReactiveObject
    {

        /// <summary>
        /// 页面布局对象
        /// </summary>
        private Control? _viewReference;
        /// <summary>
        /// Mark识别参数 
        /// </summary>
        private MarkPointRecognizeCfgInfo _markPointRecognizeCfgInfo;
        /// <summary>
        /// 值改变事件
        /// </summary>
        public EventHandler? AfterModified;
        /// <summary>
        /// 模式选择下拉框
        /// </summary>
        public ComboxViewModel ModelTypeViewModel { get; }

        /// <summary>
        /// BadMark位置-视图模型
        /// </summary>

        public CamreaPositionViewModel BadMarkPositionViewModel { get; }

        /// <summary>
        /// 操作类型下拉框
        /// </summary>
        public ComboxViewModel OperationTypeViewModel { get; }
        #region 响应式属性
        /// <summary>
        /// 操作类型
        /// </summary>
        public BadMarkOperationType OperationType
        {
            get => (BadMarkOperationType)((OperationTypeViewModel.SelectedItem as ComBoxItem)?.Value ?? BadMarkOperationType.InkDot);
            set
            {
                if (OperationTypeViewModel.ItemsSource != null)
                {
                    var targetItem = OperationTypeViewModel.ItemsSource.Cast<ComBoxItem>()
                    .FirstOrDefault(o => (BadMarkOperationType)o.Value == value);
                    if (targetItem != null)
                        OperationTypeViewModel.SelectedItem = targetItem;
                }

            }
        }

        /// <summary>
        /// 模式类型
        /// </summary>
        public BadMarkMode BadMarkMode
        {
            get => (BadMarkMode)((ModelTypeViewModel.SelectedItem as ComBoxItem)?.Value ?? BadMarkMode.NGSkip);
            set
            {
                if (ModelTypeViewModel.ItemsSource != null)
                {
                    var targetItem = ModelTypeViewModel.ItemsSource.Cast<ComBoxItem>()
                    .FirstOrDefault(o => (BadMarkMode)o.Value == value);
                    if (targetItem != null)
                        ModelTypeViewModel.SelectedItem = targetItem;
                }

            }
        }
        #endregion

        /// <summary>
        /// 创建模板命令
        /// </summary>
        public ReactiveCommand<Unit, Unit> CreateTemplateCommand { get; }
        /// <summary>
        /// 测试模板命令
        /// </summary>
        public ReactiveCommand<Unit, Unit> TestTemplateCommand { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public BadMarkWorkAreaViewModel()
        {
            _markPointRecognizeCfgInfo = new MarkPointRecognizeCfgInfo();
            BadMarkPositionViewModel = new CamreaPositionViewModel();

            // 操作类型下拉框
            OperationTypeViewModel = new ComboxViewModel();
            var operationItems = new Dictionary<BadMarkOperationType, string>
            {
                { BadMarkOperationType.InkDot, "墨点" },
                { BadMarkOperationType.Match, "匹配" }
            };
            OperationTypeViewModel.ItemsSource = EnumExtensions.ToEnumItems(operationItems);
            OperationTypeViewModel.DisplayMemberPath = nameof(ComBoxItem.DisplayName);
            OperationTypeViewModel.SelectedItem = OperationTypeViewModel.ItemsSource.Cast<ComBoxItem>().FirstOrDefault();

            // 模式选择下拉框
            ModelTypeViewModel = new ComboxViewModel();
            var modelTypes = new Dictionary<BadMarkMode, string>
            {
                { BadMarkMode.NGSkip, "识别结果NG跳过" },
                { BadMarkMode.OKSkip, "识别结果OK跳过" }
            };
            ModelTypeViewModel.ItemsSource = EnumExtensions.ToEnumItems(modelTypes);
            ModelTypeViewModel.DisplayMemberPath = nameof(ComBoxItem.DisplayName);
            ModelTypeViewModel.SelectedItem = ModelTypeViewModel.ItemsSource.Cast<ComBoxItem>().FirstOrDefault();

            CreateTemplateCommand = ReactiveCommand.CreateFromTask(onCreateTemplateCommand);
            TestTemplateCommand = ReactiveCommand.CreateFromTask(onTestTemplateCommand);

            // 订阅值变更事件
            subscribeValueChanges();
        }

        /// <summary>
        /// 从数据模型复制数据到ViewModel
        /// </summary>
        /// <param name="command"></param>
        public void CopyFrom(BadMarkCommandSequence command)
        {
            if (command == null) return;
            BadMarkMode = command.BadMarkMode;
            BadMarkPositionViewModel.CopyFrom(command.BadMarkPositionInfo);

            OperationType = command.OperationType;
            _markPointRecognizeCfgInfo = command.MarkPointRecognizeCfgInfo;
        }

        /// <summary>
        /// 从ViewModel复制数据到数据模型
        /// </summary>
        /// <param name="command"></param>
        public void CopyTo(BadMarkCommandSequence command)
        {
            if (command == null) return;
            command.BadMarkMode = BadMarkMode;
            BadMarkPositionViewModel.CopyTo(command.BadMarkPositionInfo);
            command.OperationType = OperationType;
            command.MarkPointRecognizeCfgInfo = _markPointRecognizeCfgInfo;
        }
        /// <summary>
        /// 设置视图引用
        /// </summary>
        public void SetViewReference(Control view)
        {
            _viewReference = view;
        }
        #region 执行命令

        /// <summary>
        ///创建模板
        /// </summary>
        private async Task onCreateTemplateCommand()
        {
            var parentWindow = _viewReference?.GetVisualRoot() as Window;
            if (parentWindow == null)
            {
                await MessageBox.ShowErrorDialog("错误", "无法获取窗口上下文，操作失败", _viewReference);
                return;
            }

            //var editViewModel = new MarkTemplateEditWindowViewModel();
            //editViewModel.CopyFrom(_markPointRecognizeCfgInfo);
            //var editWindow = new MarkTemplateEditWindow
            //{
            //    DataContext = editViewModel,
            //    Title = "Mark模板编辑",
            //    Width = 1920,
            //    Height = 1020,
            //    WindowStartupLocation = WindowStartupLocation.CenterScreen
            //};
            //editViewModel.SetViewReference(editWindow);

            //var result = await editWindow.ShowDialog<bool>(parentWindow);
            //if ((result))
            //{
            //    editViewModel.CopyTo(_markPointRecognizeCfgInfo);
            //    onAfterModified(this, new EventArgs());
            //}
        }
        /// <summary>
        ///测试模板
        /// </summary>
        private async Task onTestTemplateCommand()
        {

        }


        #endregion

        #region 值改变事件
        /// <summary>
        /// 订阅值改变事件
        /// </summary>
        private void subscribeValueChanges()
        {
           
            ModelTypeViewModel.ValueChanged += onValueChanged;
            OperationTypeViewModel.ValueChanged += onValueChanged;
            BadMarkPositionViewModel.AfterModified += onAfterModified;

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