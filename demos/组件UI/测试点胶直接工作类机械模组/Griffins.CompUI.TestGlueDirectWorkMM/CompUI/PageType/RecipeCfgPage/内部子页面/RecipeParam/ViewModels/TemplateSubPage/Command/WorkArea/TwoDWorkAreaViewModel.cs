using Avalonia.Controls;
using Avalonia.VisualTree;
using Griffins.UI;
using Griffins.UI.General;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.TemplateSubPage.Command;
using ReactiveUI;
using System.Reactive;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.TemplateSubPage.Command
{
    /// <summary>
    /// 2D设置指令工作区-视图模型
    /// </summary>
    public class TwoDWorkAreaViewModel : ReactiveObject
    {
        /// <summary>
        /// 值改变事件
        /// </summary>
        public EventHandler? AfterModified;
        /// <summary>
        /// 页面布局对象
        /// </summary>
        private Control? _viewReference;

        //private TwoDDetectionProgrammingCfgInfo _twoDDetectionProgrammingCfgInfo;

        /// <summary>
        /// 初始位置-视图模型
        /// </summary>
        public CamreaPositionViewModel InitalPositionViewModel { get; }
        /// <summary>
        /// 序号（标签）
        /// </summary>
        public TextBlockViewModel SerialNumberViewModel { get; }

        /// <summary>
        /// 胶宽检测开关
        /// </summary>
        public ToggleSwitchViewModel IsGlueWidthDetectionEnabledViewModel { get; }

        /// <summary>
        /// 检测方式下拉框
        /// </summary>
        public ComboxViewModel DetectionModeViewModel { get; }

        #region 响应式属性

        /// <summary>
        /// 序号
        /// </summary>
        public int SerialNumber
        {
            get => int.Parse(SerialNumberViewModel.Text);
            set => SerialNumberViewModel.Text = value.ToString();
        }

        /// <summary>
        /// 胶宽检测
        /// </summary>
        public bool IsGlueWidthDetectionEnabled
        {
            get => IsGlueWidthDetectionEnabledViewModel.IsChecked;
            set => IsGlueWidthDetectionEnabledViewModel.IsChecked = value;
        }

        /// <summary>
        /// 检测方式
        /// </summary>
        public DetectionMode DetectionMode
        {
            get => (DetectionMode)((DetectionModeViewModel.SelectedItem as ComBoxItem)?.Value ?? DetectionMode.DoNotSaveImage);
            set
            {
                if (DetectionModeViewModel.ItemsSource != null)
                {
                    var targetItem = DetectionModeViewModel.ItemsSource.Cast<ComBoxItem>()
                      .FirstOrDefault(o => (DetectionMode)o.Value == value);
                    if (targetItem != null)
                        DetectionModeViewModel.SelectedItem = targetItem;
                }

            }
        }
        #endregion

        /// <summary>
        /// 2D检测编程弹窗命令
        /// </summary>
        public ReactiveCommand<Unit, Unit> CreateTemplateCommand { get; }
        /// <summary>
        /// 测试模板命令
        /// </summary>
        public ReactiveCommand<Unit, Unit> TestTemplateCommand { get; }
        /// <summary>
        /// 构造函数
        /// </summary>
        public TwoDWorkAreaViewModel()
        {
            //_twoDDetectionProgrammingCfgInfo = new TwoDDetectionProgrammingCfgInfo();

            InitalPositionViewModel = new CamreaPositionViewModel();

            // 序号（标签）
            SerialNumberViewModel = new TextBlockViewModel { Text = "1" };

            // 胶宽检测开关
            IsGlueWidthDetectionEnabledViewModel = new ToggleSwitchViewModel { IsChecked = false };

            // 检测方式下拉框
            DetectionModeViewModel = new ComboxViewModel();
            var detectionItems = new Dictionary<DetectionMode, string>
            {
                { DetectionMode.DoNotSaveImage, "不保存图片" },
                { DetectionMode.SaveNGImage, "NG保存图片" },
                { DetectionMode.SaveOKImage, "OK保存图片" },
                { DetectionMode.SaveAllImages, "保存所有图片" }
            };
            DetectionModeViewModel.ItemsSource = EnumExtensions.ToEnumItems(detectionItems);
            DetectionModeViewModel.DisplayMemberPath = nameof(ComBoxItem.DisplayName);
            DetectionModeViewModel.SelectedItem = DetectionModeViewModel.ItemsSource.Cast<ComBoxItem>().FirstOrDefault();

            CreateTemplateCommand = ReactiveCommand.CreateFromTask(onCreateTemplateCommand);
            TestTemplateCommand = ReactiveCommand.CreateFromTask(onTestTemplateCommand);

            // 订阅值变更事件
            subscribeValueChanges();
        }

        /// <summary>
        /// 从数据模型复制数据到ViewModel
        /// </summary>
        /// <param name="command"></param>
        public void CopyFrom(TwoDCommandSequence command)
        {
            if (command == null) return;
            InitalPositionViewModel.CopyFrom(command.InitalPositionInfo);

            SerialNumber = command.SerialNumber;

            IsGlueWidthDetectionEnabled = command.IsGlueWidthDetectionEnabled;
            DetectionMode = command.DetectionMode;
        }

        /// <summary>
        /// 从ViewModel复制数据到数据模型
        /// </summary>
        /// <param name="command"></param>
        public void CopyTo(TwoDCommandSequence command)
        {
            if (command == null) return;

            command.SerialNumber = SerialNumber;
            InitalPositionViewModel.CopyTo(command.InitalPositionInfo);

            command.IsGlueWidthDetectionEnabled = IsGlueWidthDetectionEnabled;
            command.DetectionMode = DetectionMode;
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
        ///2D检测编程弹窗
        /// </summary>
        private async Task onCreateTemplateCommand()
        {
            //var parentWindow = _viewReference?.GetVisualRoot() as Window;
            //if (parentWindow == null)
            //{
            //    await MessageBox.ShowErrorDialog("错误", "无法获取窗口上下文，操作失败", _viewReference);
            //    return;
            //}

            //var editViewModel = new TwoDDetectionProgrammingWindowViewModel();
            //editViewModel.CopyFrom(_twoDDetectionProgrammingCfgInfo);
            //// 绑定保存配置事件
            //editViewModel.DetectionResultViewModel.SaveCfg += () =>
            //{
            //    editViewModel.CopyTo(_twoDDetectionProgrammingCfgInfo);
            //    onAfterModified(this, new EventArgs());
            //};

            //var editWindow = new TwoDDetectionProgrammingWindow
            //{
            //    DataContext = editViewModel,
            //    Title = "模板管理",
            //    Width = 1920,
            //    Height = 1020,
            //    WindowStartupLocation = WindowStartupLocation.CenterScreen
            //};

            //editViewModel.SetViewReference(editWindow);//暂时未实现

            //var result = await editWindow.ShowDialog<bool>(parentWindow);
            //if ((result))
            //{
            //    editViewModel.CopyTo(_twoDDetectionProgrammingCfgInfo);
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
            InitalPositionViewModel.AfterModified += onAfterModified;

            SerialNumberViewModel.ValueChanged += onValueChanged;
            IsGlueWidthDetectionEnabledViewModel.ValueChanged += onValueChanged;
            DetectionModeViewModel.ValueChanged += onValueChanged;
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