using Avalonia;
using Avalonia.Media;
using ReactiveUI;
using System;
using System.ComponentModel;

namespace GKG.Map.MapCell.Generic.PasswordBox.ViewModel
{
    public class PasswordBoxViewModel : ReactiveObject, IDisposable
    {
        private PasswordBoxPropertyModelEdit _propertyModel;
        private bool _disposed = false;

        // 私有字段用于存储属性值
        private PasswordBoxBrushInfo _brushInfo;
        private PasswordBoxAppearanceInfo _appearanceInfo;
        private PasswordBoxCommonInfo _commonInfo;
        private PasswordBoxLayoutInfo _layoutInfo;
        private PasswordBoxTextInfo _textInfo;

        public PasswordBoxPropertyModelEdit Model => _propertyModel;

        public PasswordBoxBrushInfo BrushInfo
        {
            get => _brushInfo;
            set
            {
                if (!ReferenceEquals(_brushInfo, value))
                {
                    _brushInfo = value;
                    this.RaisePropertyChanged(nameof(BrushInfo));
                }
            }
        }

        public PasswordBoxAppearanceInfo AppearanceInfo
        {
            get => _appearanceInfo;
            set
            {
                if (!ReferenceEquals(_appearanceInfo, value))
                {
                    _appearanceInfo = value;
                    this.RaisePropertyChanged(nameof(AppearanceInfo));
                }
            }
        }

        public PasswordBoxCommonInfo CommonInfo
        {
            get => _commonInfo;
            set
            {
                if (!ReferenceEquals(_commonInfo, value))
                {
                    _commonInfo = value;
                    this.RaisePropertyChanged(nameof(CommonInfo));
                }
            }
        }

        public PasswordBoxLayoutInfo LayoutInfo
        {
            get => _layoutInfo;
            set
            {
                if (!ReferenceEquals(_layoutInfo, value))
                {
                    _layoutInfo = value;
                    this.RaisePropertyChanged(nameof(LayoutInfo));
                }
            }
        }

        public PasswordBoxTextInfo TextInfo
        {
            get => _textInfo;
            set
            {
                if (!ReferenceEquals(_textInfo, value))
                {
                    _textInfo = value;
                    this.RaisePropertyChanged(nameof(TextInfo));
                }
            }
        }

        public PasswordBoxViewModel()
        {
            // 设计时构造函数
            _brushInfo = new PasswordBoxBrushInfo();
            _appearanceInfo = new PasswordBoxAppearanceInfo();
            _commonInfo = new PasswordBoxCommonInfo();
            _layoutInfo = new PasswordBoxLayoutInfo();
            _textInfo = new PasswordBoxTextInfo();
        }

        public PasswordBoxViewModel(bool designTime, PasswordBoxPropertyModelEdit propertyModel)
        {
            _propertyModel = propertyModel;
            // No direct subscription to Model events to ensure updates only happen via Operation Atoms
            InitializeFromModel(propertyModel);
        }

        private void InitializeFromModel(PasswordBoxPropertyModelEdit model)
        {
            if (model == null) return;

            // 创建副本而不是引用
            _brushInfo = new PasswordBoxBrushInfo();
            if (model.BrushInfo != null)
            {
                _brushInfo.BackColorStr = model.BrushInfo.BackColorStr;
                _brushInfo.BorderColorStr = model.BrushInfo.BorderColorStr;
                _brushInfo.ForeColorStr = model.BrushInfo.ForeColorStr;
                _brushInfo.FocusBorderColorStr = model.BrushInfo.FocusBorderColorStr;
            }

            _appearanceInfo = new PasswordBoxAppearanceInfo();
            if (model.AppearanceInfo != null)
            {
                _appearanceInfo.Opacity = model.AppearanceInfo.Opacity;
                _appearanceInfo.BorderThicknessLeft = model.AppearanceInfo.BorderThicknessLeft;
                _appearanceInfo.BorderThicknessTop = model.AppearanceInfo.BorderThicknessTop;
                _appearanceInfo.BorderThicknessRight = model.AppearanceInfo.BorderThicknessRight;
                _appearanceInfo.BorderThicknessBottom = model.AppearanceInfo.BorderThicknessBottom;
            }

            _commonInfo = new PasswordBoxCommonInfo();
            if (model.CommonInfo != null)
            {
                _commonInfo.PasswordValue = model.CommonInfo.PasswordValue;
                _commonInfo.CursorType = model.CommonInfo.CursorType;
                _commonInfo.Enabled = model.CommonInfo.Enabled;
                _commonInfo.PlaceholderText = model.CommonInfo.PlaceholderText;
                _commonInfo.PasswordVisible = model.CommonInfo.PasswordVisible;
            }

            _layoutInfo = new PasswordBoxLayoutInfo();
            if (model.LayoutInfo != null)
            {
                // 宽高主数据统一走父类 Width/Height，LayoutInfo 不再承载宽高。
                _layoutInfo.HorizontalAlignment = model.LayoutInfo.HorizontalAlignment;
                _layoutInfo.VerticalAlignment = model.LayoutInfo.VerticalAlignment;
                _layoutInfo.MarginLeft = model.LayoutInfo.MarginLeft;
                _layoutInfo.MarginTop = model.LayoutInfo.MarginTop;
                _layoutInfo.MarginRight = model.LayoutInfo.MarginRight;
                _layoutInfo.MarginBottom = model.LayoutInfo.MarginBottom;
                _layoutInfo.MinWidth = model.LayoutInfo.MinWidth;
                _layoutInfo.MaxWidth = model.LayoutInfo.MaxWidth;
                _layoutInfo.MinHeight = model.LayoutInfo.MinHeight;
                _layoutInfo.MaxHeight = model.LayoutInfo.MaxHeight;
            }

            _textInfo = new PasswordBoxTextInfo();
            if (model.TextInfo != null)
            {
                _textInfo.FontFamily = model.TextInfo.FontFamily;
                _textInfo.FontSize = model.TextInfo.FontSize;
                _textInfo.IsItalic = model.TextInfo.IsItalic;
                _textInfo.IsBold = model.TextInfo.IsBold;
            }
        }

        public void Dispose()
        {
            if (!_disposed && _propertyModel != null)
            {
                _disposed = true;
            }
        }
    }
}
