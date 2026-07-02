using Avalonia.Controls;
using ReactiveUI;
using System;
using System.Globalization;

namespace GKG.UI.General
{
    /// <summary>
    /// 气缸延时-视图模型
    /// </summary>
    public class CylinderDelayViewModel : ReactiveObject
    {
        private Control? _viewReference;

        /// <summary>
        /// 气缸超时时间-文本输入框视图模型
        /// </summary>
        public TextInputViewModel DelayTextViewModel { get; }

        /// <summary>
        /// 事件（通知外部数据变更）
        /// </summary>
        public event EventHandler? AfterModified;

        /// <summary>
        /// 气缸超时时间，单位 ms
        /// </summary>
        public decimal DelayNumeric
        {
            get => ParseDelay(DelayTextViewModel.Text);
            set
            {
                DelayTextViewModel.Text = value.ToString(CultureInfo.InvariantCulture);
                this.RaisePropertyChanged(nameof(DelayNumeric));
            }
        }

        public CylinderDelayViewModel()
        {
            DelayTextViewModel = new TextInputViewModel
            {
                Text = "100",
            };

            subscribeValueChanges();
        }

        private void subscribeValueChanges()
        {
            DelayTextViewModel.ValueChanged += onValueChanged;
        }

        private void onValueChanged(object? sender, ValueChangedEventArgs e)
        {
            this.RaisePropertyChanged(nameof(DelayNumeric));
            AfterModified?.Invoke(sender, e);
        }

        public void SetViewReference(Control view)
        {
            _viewReference = view ?? throw new ArgumentNullException(nameof(view));
        }

        public void CopyFrom(CylinderDelayCfgInfo model)
        {
            DelayNumeric = model?.DelayNumeric ?? 100m;
        }

        public void CopyTo(CylinderDelayCfgInfo model)
        {
            if (model == null)
            {
                return;
            }

            model.DelayNumeric = DelayNumeric;
        }

        private static decimal ParseDelay(string? text)
        {
            return decimal.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value) && value >= 0
                ? value
                : 100m;
        }
    }
}
