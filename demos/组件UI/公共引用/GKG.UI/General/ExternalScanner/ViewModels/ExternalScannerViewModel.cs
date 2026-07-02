using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using ReactiveUI;

namespace GKG.UI.General
{
    /// <summary>
    /// 通用外接扫码器视图模型
    /// </summary>
    public class ExternalScannerViewModel : ReactiveObject
    {
        #region 私有字段

        /// <summary>
        /// 视图引用（用于弹窗等UI操作）
        /// </summary>
        private Control? _viewReference;

        #endregion

        #region UI组件

        /// <summary>
        /// 是否启用-开关按钮视图模型
        /// </summary>
        public ToggleSwitchViewModel EnableViewModel { get; }

        /// <summary>
        /// 条码校验字符串-输入框视图模型
        /// </summary>
        public TextInputViewModel BarcodeValidationStringViewModel { get; }

        /// <summary>
        /// 条码类型-下拉框视图模型
        /// </summary>
        public ComboxViewModel BarcodeTypeViewModel { get; }

        /// <summary>
        /// 扫码器类型-输入框视图模型
        /// </summary>
        public TextInputViewModel ScannerTypeViewModel { get; }

        #endregion

        #region 值改变事件

        /// <summary>
        /// 值改变事件
        /// </summary>
        public event EventHandler? AfterModified;

        #endregion

        #region 响应式属性

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enable
        {
            get => EnableViewModel.IsChecked;
            set
            {
                EnableViewModel.IsChecked = value;
                this.RaisePropertyChanged(nameof(Enable));
            }
        }

        /// <summary>
        /// 条码校验字符串
        /// </summary>
        public string BarcodeValidationString
        {
            get => BarcodeValidationStringViewModel.Text;
            set
            {
                BarcodeValidationStringViewModel.Text = value;
                this.RaisePropertyChanged(nameof(BarcodeValidationString));
            }
        }

        /// <summary>
        /// 选中的条码类型
        /// </summary>
        public BarcodeType SelectedBarcodeType
        {
            get => (BarcodeType)((BarcodeTypeViewModel.SelectedItem as ComBoxItem)?.Value ?? BarcodeType.QR);
            set
            {
                if (BarcodeTypeViewModel.ItemsSource != null)
                {
                    var targetItem = BarcodeTypeViewModel.ItemsSource.Cast<ComBoxItem>()
                        .FirstOrDefault(o => (BarcodeType)o.Value == value);
                    if (targetItem != null)
                        BarcodeTypeViewModel.SelectedItem = targetItem;

                    this.RaisePropertyChanged(nameof(SelectedBarcodeType));
                }
            }
        }

        /// <summary>
        /// 扫码器类型
        /// </summary>
        public string ScannerType
        {
            get => ScannerTypeViewModel.Text;
            set
            {
                ScannerTypeViewModel.Text = value;
                this.RaisePropertyChanged(nameof(ScannerType));
            }
        }

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public ExternalScannerViewModel()
        {
            #region 初始化UI组件

            EnableViewModel = new();
            BarcodeValidationStringViewModel = new();
            BarcodeTypeViewModel = new();
            ScannerTypeViewModel = new();

            // 初始化条码类型选项
            var barcodeTypeDisplayNames = new Dictionary<BarcodeType, string>
            {
                { BarcodeType.QR, "QR" },
                { BarcodeType.Code128, "Code128" },
                { BarcodeType.Code39, "Code39" },
                { BarcodeType.EAN13, "EAN13" }
            };
            BarcodeTypeViewModel.ItemsSource = EnumExtensions.ToEnumItems(barcodeTypeDisplayNames);
            BarcodeTypeViewModel.DisplayMemberPath = nameof(ComBoxItem.DisplayName);

            #endregion

            // 订阅值变更
            subscribeValueChanges();
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 设置视图引用（用于弹窗等UI操作）
        /// </summary>
        public void SetViewReference(Control view)
        {
            _viewReference = view;
        }

        /// <summary>
        /// 从数据模型复制数据到ViewModel
        /// </summary>
        public void CopyFrom(ExternalScannerCfg model)
        {
            if (model == null)
            {
                return;
            }

            Enable = model.Enable;
            BarcodeValidationString = model.BarcodeValidationString;
            SelectedBarcodeType = model.BarcodeType;
            ScannerType = model.ScannerType;
        }

        /// <summary>
        /// 从ViewModel复制数据到数据模型
        /// </summary>
        public void CopyTo(ExternalScannerCfg model)
        {
            if (model == null)
            {
                return;
            }

            model.Enable = Enable;
            model.BarcodeValidationString = BarcodeValidationString;
            model.BarcodeType = SelectedBarcodeType;
            model.ScannerType = ScannerType;
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 订阅值改变事件
        /// </summary>
        private void subscribeValueChanges()
        {
            EnableViewModel.ValueChanged += onValueChanged;
            BarcodeValidationStringViewModel.ValueChanged += onValueChanged;
            BarcodeTypeViewModel.ValueChanged += onValueChanged;
            ScannerTypeViewModel.ValueChanged += onValueChanged;
        }

        /// <summary>
        /// 值改变事件处理
        /// </summary>
        private void onValueChanged(object? sender, ValueChangedEventArgs e)
        {
            AfterModified?.Invoke(sender, e);
        }

        #endregion
    }
}
