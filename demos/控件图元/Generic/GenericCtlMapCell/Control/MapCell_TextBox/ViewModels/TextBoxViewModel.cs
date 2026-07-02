using System;
using System.ComponentModel;
using PropertyModels.ComponentModel;

namespace GKG.Map.MapCell.Generic.Control.MapCell_TextBox.ViewModels
{
    /// <summary>
    /// 文本输入框视图模型
    /// </summary>
    public class TextBoxViewModel : PropertyModels.ComponentModel.ReactiveObject, IDisposable
    {
        #region 私有字段

        private readonly TextBoxPropertyModelEdit _propertyModel;
        private bool _disposed;
        private bool _syncingBrushTextColor;

        #endregion

        #region 属性

        public TextBoxBrushInfo BrushInfo => _propertyModel.BrushInfo;
        public TextBoxAppearanceInfo AppearanceInfo => _propertyModel.AppearanceInfo;
        public TextBoxCommonInfo CommonInfo => _propertyModel.CommonInfo;
        public TextBoxLayoutInfo LayoutInfo => _propertyModel.LayoutInfo;
        public TextBoxTextInfo TextInfo => _propertyModel.TextInfo;
        public TextBoxPropertyModelEdit Model => _propertyModel;

        /// <summary>
        /// 透明度换算（属性面板为 0-100，Avalonia 为 0-1）
        /// </summary>
        public double OpacityValue => 1.0 - (AppearanceInfo.Opacity / 100.0);

        #endregion

        #region 构造函数

        public TextBoxViewModel()
            : this(new TextBoxPropertyModelEdit())
        {
        }

        public TextBoxViewModel(TextBoxPropertyModelEdit propertyModel)
        {
            _propertyModel = propertyModel;

            // 监听根对象与子属性对象变更，用于统一向 View 侧转发刷新通知
            _propertyModel.PropertyChanged += PropertyModel_PropertyChanged;
            _propertyModel.BrushInfo.PropertyChanged += ChildPropertyChanged;
            _propertyModel.AppearanceInfo.PropertyChanged += ChildPropertyChanged;
            _propertyModel.CommonInfo.PropertyChanged += ChildPropertyChanged;
            _propertyModel.LayoutInfo.PropertyChanged += ChildPropertyChanged;
            _propertyModel.TextInfo.PropertyChanged += ChildPropertyChanged;
        }

        public void ReloadFromModel()
        {
            RaisePropertyChanged(nameof(BrushInfo));
            RaisePropertyChanged(nameof(AppearanceInfo));
            RaisePropertyChanged(nameof(CommonInfo));
            RaisePropertyChanged(nameof(LayoutInfo));
            RaisePropertyChanged(nameof(TextInfo));
            RaisePropertyChanged(nameof(OpacityValue));
        }

        #endregion

        #region 事件转发

        private void PropertyModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // 属性组对象被替换/整体刷新时直接透传属性名
            RaisePropertyChanged(e.PropertyName);
        }

        private void ChildPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // 子属性对象内部字段变更时，提升到属性组级别通知，便于 View 绑定刷新
            if (sender is TextBoxBrushInfo)
            {
                if (!_syncingBrushTextColor && e.PropertyName == nameof(TextBoxBrushInfo.ForegroundColor))
                {
                    try
                    {
                        _syncingBrushTextColor = true;
                        if (TextInfo.FontColor != BrushInfo.ForegroundColor)
                            TextInfo.FontColor = BrushInfo.ForegroundColor;
                    }
                    finally
                    {
                        _syncingBrushTextColor = false;
                    }
                }
                RaisePropertyChanged(nameof(BrushInfo));
            }
            else if (sender is TextBoxAppearanceInfo)
            {
                RaisePropertyChanged(nameof(AppearanceInfo));
                if (e.PropertyName == nameof(TextBoxAppearanceInfo.Opacity))
                    RaisePropertyChanged(nameof(OpacityValue));
            }
            else if (sender is TextBoxCommonInfo)
                RaisePropertyChanged(nameof(CommonInfo));
            else if (sender is TextBoxLayoutInfo)
            {
                RaisePropertyChanged(nameof(LayoutInfo));
            }
            else if (sender is TextBoxTextInfo)
            {
                if (!_syncingBrushTextColor && e.PropertyName == nameof(TextBoxTextInfo.FontColor))
                {
                    try
                    {
                        _syncingBrushTextColor = true;
                        if (BrushInfo.ForegroundColor != TextInfo.FontColor)
                            BrushInfo.ForegroundColor = TextInfo.FontColor;
                    }
                    finally
                    {
                        _syncingBrushTextColor = false;
                    }
                }
                RaisePropertyChanged(nameof(TextInfo));
            }
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                // 释放事件订阅，避免 ViewModel 长生命周期导致内存泄漏
                _propertyModel.PropertyChanged -= PropertyModel_PropertyChanged;
                _propertyModel.BrushInfo.PropertyChanged -= ChildPropertyChanged;
                _propertyModel.AppearanceInfo.PropertyChanged -= ChildPropertyChanged;
                _propertyModel.CommonInfo.PropertyChanged -= ChildPropertyChanged;
                _propertyModel.LayoutInfo.PropertyChanged -= ChildPropertyChanged;
                _propertyModel.TextInfo.PropertyChanged -= ChildPropertyChanged;
            }

            _disposed = true;
        }

        #endregion
    }
}
