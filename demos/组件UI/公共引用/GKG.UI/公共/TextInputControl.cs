using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media;
using ReactiveUI;
using System.Reactive.Disposables;

namespace GKG.UI
{
    /// <summary>
    /// 带标题的文本输入框控件
    /// </summary>
    public class TextInputControl : BasicControl<TextInputViewModel>
    {
        private TextBox? _innerTextBox;

        /// <summary>
        /// 
        /// </summary>
        public TextInputControl() : base()
        {
            initializeTextBox();
        }

        private void initializeTextBox()
        {
            _innerTextBox = new TextBox
            {
                Name = "TextBox1",
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch
            };

            base.SetContent(_innerTextBox, Avalonia.Layout.HorizontalAlignment.Stretch);

            this.WhenActivated(disposables =>
            {
                this.Bind(ViewModel, vm => vm.Text, v => v._innerTextBox!.Text)
                    .DisposeWith(disposables); 

                // 禁用
                this.Bind(ViewModel, vm => vm.IsEnabled, v => v._innerTextBox!.IsEnabled)
                    .DisposeWith(disposables); 

                // 显示
                this.Bind(ViewModel, vm => vm.IsVisible, v => v._innerTextBox!.IsVisible)
                    .DisposeWith(disposables);

            });
        }
    }

    /// <summary>
    /// 文本输入框-视图模型
    /// </summary>
    public class TextInputViewModel : BasicControlViewModel
    {
        private string _text = "";
        private bool _isEnabled = true;

        /// <summary>
        /// 显示的文本
        /// </summary>
        public string Text
        {
            get => _text;
            set
            {
                if (_text != value)
                {
                    var oldValue = _text; // 记录旧值
                    this.RaiseAndSetIfChanged(ref _text, value);
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