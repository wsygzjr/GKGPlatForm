using System;
using System.Reactive;
using Avalonia;
using Avalonia.Media;
using Griffins.UI2;
using ReactiveUI;

namespace Griffins.Map.PressureFuncCtlMapCell.ViewModel
{
    public class PressureViewModel : ReactiveObject, IDisposable
    {
        private FontInfo _fontInfo;

        public FontInfo TextFont
        {
            get
            {
                return _fontInfo;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref _fontInfo, value);
            }
        }

        private Color _textColor;
        /// <summary>
        /// 文本颜色
        /// </summary>
        public Color TextColor
        {
            get
            {
                return _textColor;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref _textColor, value);
            }
        }

        private Color _backColor;
        /// <summary>
        /// 背景颜色
        /// </summary>
        public Color BackColor
        {
            get
            {
                return _backColor;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref _backColor, value);
            }
        }

        private string _pressureValue;
        /// <summary>
        /// 气压值
        /// </summary>
        public string PressureValue
        {
            get
            {
                return _pressureValue;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref _pressureValue, value);
            }
        }

        public ReactiveCommand<Point, Unit> PointerPressedCommand { get; }

        private Action _clickAction;

        public PressureViewModel(PressurePropertyModelEdit pressurePropertyModelEdit, Action clickAction)
        {
            this.TextFont = pressurePropertyModelEdit.TextFont;
            this.TextColor = pressurePropertyModelEdit.TextColor;
            this.BackColor = pressurePropertyModelEdit.BackColor;
            this.PressureValue = pressurePropertyModelEdit.PressureValue.ToString();
            PointerPressedCommand = ReactiveCommand.Create<Point>(OnPointerPressed);
            _clickAction = clickAction;
        }

        private void OnPointerPressed(Point screenP)
        {
            _clickAction?.Invoke();
        }

        public void Dispose()
        {

        }
    }
}
