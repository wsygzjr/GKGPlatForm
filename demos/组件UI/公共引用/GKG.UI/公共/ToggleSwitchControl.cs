using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media;
using ReactiveUI;
using System;
using System.Collections;
using System.Reactive.Disposables;

namespace GKG.UI
{
    /// <summary>
    /// 下拉框控件
    /// </summary>
    public class ToggleSwitchControl : BasicControl<ToggleSwitchViewModel>
    { 
        private ToggleSwitch _toggleSwitch;

        /// <summary>
        /// 
        /// </summary>
        public ToggleSwitchControl() : base()
        {
            // 1. 实例化 ToggleSwitch
            _toggleSwitch = new ToggleSwitch
            {
                // 2. 配置属性
                Name = $"InnerToggle", // 唯一标识  
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
                IsChecked = false, // 默认未选中 
                OnContent = "",
                OffContent = "",
            };
            initializeControl();
        }

        private void initializeControl()
        {
            base.SetContent(_toggleSwitch , Avalonia.Layout.HorizontalAlignment.Stretch);

            this.WhenActivated(disposables =>
            {
                // 当前值
                this.Bind(ViewModel, vm => vm.IsChecked, v => v._toggleSwitch!.IsChecked)
                    .DisposeWith(disposables);

                // 禁用
                this.Bind(ViewModel, vm => vm.IsEnabled, v => v._toggleSwitch!.IsEnabled)
                    .DisposeWith(disposables); 

                // 显示
                this.Bind(ViewModel, vm => vm.IsVisible, v => v._toggleSwitch!.IsVisible)
                    .DisposeWith(disposables);

            });
             
        }
    }

    /// <summary>
    /// 数字输入框控件-视图模型
    /// </summary>
    public class ToggleSwitchViewModel : BasicControlViewModel
    {
        private bool _isChecked;
        private bool _isEnabled = true;        

        /// <summary>
        /// 当前值（双向绑定）
        /// </summary>
        public bool IsChecked
        {
            get => _isChecked; 
            set
            {
                if (_isChecked != value)
                {
                    var oldValue = _isChecked; // 记录旧值
                    this.RaiseAndSetIfChanged(ref _isChecked , value);
                    OnValueChanged(oldValue, value); // 触发基类事件
                }
            }
        } 

        /// <summary>
        /// 是否禁用，缺省不禁用true
        /// </summary> 
        public bool IsEnabled
        {
            get => _isEnabled;
            set => this.RaiseAndSetIfChanged(ref _isEnabled, value);
        } 

        private bool _isVisible = true;
        /// <summary>
        /// 是否显示
        /// </summary>
        public bool IsVisible
        {
            get => _isVisible; 
            set => this.RaiseAndSetIfChanged(ref _isVisible, value);
        }

    }

}