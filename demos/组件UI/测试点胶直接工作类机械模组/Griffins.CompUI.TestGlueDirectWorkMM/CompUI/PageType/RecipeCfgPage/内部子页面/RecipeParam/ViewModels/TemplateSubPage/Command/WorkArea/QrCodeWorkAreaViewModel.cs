using Avalonia.Controls;
using DynamicData;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.TemplateSubPage.Command;
using Griffins.UI;
using Griffins.UI.General;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.TemplateSubPage.Command;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.TemplateSubPage.Command
{
    /// <summary>
    /// 扫二维码指令工作区-视图模型
    /// </summary>
    public class QrCodeWorkAreaViewModel : ReactiveObject
    {
        /// <summary>
        /// 页面布局对象
        /// </summary>
        private Control? _viewReference;

        /// <summary>
        /// 值改变事件
        /// </summary>
        public EventHandler? AfterModified;
        #region 响应式属性
        /// <summary>
        /// 扫码方式下拉框
        /// </summary>
        public ComboxViewModel ScanModeViewModel { get; }

        /// <summary>
        /// 码类型下拉框
        /// </summary>
        public ComboxViewModel CodeTypeViewModel { get; }

        /// <summary>
        /// 扫码方式
        /// </summary>
        public QrCodeScanMode ScanMode
        {
            get => (QrCodeScanMode)((ScanModeViewModel.SelectedItem as ComBoxItem)?.Value ?? QrCodeScanMode.CCD);
            set
            {
                if (ScanModeViewModel.ItemsSource != null)
                {
                    var targetItem = ScanModeViewModel.ItemsSource.Cast<ComBoxItem>()
                   .FirstOrDefault(o => (QrCodeScanMode)o.Value == value);
                    if (targetItem != null)
                        ScanModeViewModel.SelectedItem = targetItem;
                }
            }
        }

        /// <summary>
        /// 码类型
        /// </summary>
        public QrCodeType CodeType
        {
            get => (QrCodeType)((CodeTypeViewModel.SelectedItem as ComBoxItem)?.Value ?? QrCodeType.QRCode);
            set
            {
                if (CodeTypeViewModel.ItemsSource != null)
                {
                    var targetItem = CodeTypeViewModel.ItemsSource.Cast<ComBoxItem>()
                   .FirstOrDefault(o => (QrCodeType)o.Value == value);
                    if (targetItem != null)
                        CodeTypeViewModel.SelectedItem = targetItem;
                }
            }
        }

        /// <summary>
        /// 扫码数据列表
        /// </summary>
        private ObservableCollection<string> _scanDataList = new ObservableCollection<string>();
        public ObservableCollection<string> ScanDataList
        {
            get => _scanDataList;
        }
        #endregion

        /// <summary>
        /// 二维码位置-视图模型
        /// </summary>

        public QrCodePositionViewModel PositionViewModel { get; }

        /// <summary>
        /// 测试命令
        /// </summary>
        public ReactiveCommand<Unit, Unit> TestCommand { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public QrCodeWorkAreaViewModel()
        {
            // 扫码方式下拉框
            ScanModeViewModel = new ComboxViewModel();
            var scanModeItems = new Dictionary<QrCodeScanMode, string>
            {
                { QrCodeScanMode.CCD, "CCD" },
                { QrCodeScanMode.Laser, "激光" }
            };
            ScanModeViewModel.ItemsSource = EnumExtensions.ToEnumItems(scanModeItems);
            ScanModeViewModel.DisplayMemberPath = nameof(ComBoxItem.DisplayName);
            ScanModeViewModel.SelectedItem = ScanModeViewModel.ItemsSource.Cast<ComBoxItem>().FirstOrDefault();

            // 码类型下拉框
            CodeTypeViewModel = new ComboxViewModel();
            var codeTypeItems = new Dictionary<QrCodeType, string>
            {
                { QrCodeType.BarCode, "一维码" },
                { QrCodeType.QRCode, "二维码QR" },
                { QrCodeType.MatrixCode, "二维码Martrix" }
            };
            CodeTypeViewModel.ItemsSource = EnumExtensions.ToEnumItems(codeTypeItems);
            CodeTypeViewModel.DisplayMemberPath = nameof(ComBoxItem.DisplayName);
            CodeTypeViewModel.SelectedItem = CodeTypeViewModel.ItemsSource.Cast<ComBoxItem>()
                .FirstOrDefault(o => (QrCodeType)o.Value == QrCodeType.QRCode);

            PositionViewModel = new QrCodePositionViewModel();
            TestCommand = ReactiveCommand.CreateFromTask(onTestCommand);

            // 订阅值变更事件
            subscribeValueChanges();
        }

        /// <summary>
        /// 从数据模型复制数据到ViewModel
        /// </summary>
        /// <param name="command"></param>
        public void CopyFrom(QrCodeCommandSequence command)
        {
            if (command == null) return;
            ScanMode = command.ScanMode;
            CodeType = command.CodeType;
            ScanDataList.Clear();
            ScanDataList.AddRange(command.ScanDataList);

            PositionViewModel.CopyFrom(command.PositionInfo);

        }

        /// <summary>
        /// 从ViewModel复制数据到数据模型
        /// </summary>
        /// <param name="command"></param>
        public void CopyTo(QrCodeCommandSequence command)
        {
            if (command == null) return;
            command.ScanMode = ScanMode;
            command.CodeType = CodeType;
            command.ScanDataList = ScanDataList.ToList();
            PositionViewModel.CopyTo(command.PositionInfo);

        }
        /// <summary>
        ///测试
        /// </summary>
        private async Task onTestCommand()
        {
            try
            {


            }
            catch (Exception ex)
            {
                await MessageBox.ShowErrorDialog("失败", $"{ex.Message}", _viewReference);
            }
            finally
            {
            }
        }
        /// <summary>
        /// 设置视图引用
        /// </summary>
        public void SetViewReference(Control view)
        {
            _viewReference = view;
            PositionViewModel.SetViewReference(view);
        }
        #region 值改变事件
        /// <summary>
        /// 订阅值改变事件
        /// </summary>
        private void subscribeValueChanges()
        {
            //PositionViewModel.AfterModified += onAfterModified;

            ScanModeViewModel.ValueChanged += onValueChanged;
            CodeTypeViewModel.ValueChanged += onValueChanged;
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