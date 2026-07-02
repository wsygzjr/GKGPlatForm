using System;
using System.ComponentModel;
using PropertyModels.ComponentModel;
using GKG.Map.MapCell.Generic.Control.MapCell_Slider;
using GKG.Map.MapCell.Generic.Control.MapCell_TextBox;

namespace GKG.Map.MapCell.Generic.Control.MapCell_Slider.ViewModels
{
    /// <summary>
    /// 滑块视图模型
    /// </summary>
    public class SliderViewModel : PropertyModels.ComponentModel.ReactiveObject, IDisposable
    {
        private readonly SliderPropertyModelEdit _propertyModel;
        private bool _disposed = false;

        /// <summary>
        /// 画笔信息
        /// </summary>
        public SliderBrushInfo BrushInfo => _propertyModel.BrushInfo;

        /// <summary>
        /// 外观信息
        /// </summary>
        public SliderAppearanceInfo AppearanceInfo => _propertyModel.AppearanceInfo;

        /// <summary>
        /// 公共信息
        /// </summary>
        public SliderCommonInfo CommonInfo => _propertyModel.CommonInfo;

        /// <summary>
        /// 布局信息
        /// </summary>
        public SliderLayoutInfo LayoutInfo => _propertyModel.LayoutInfo;
        public SliderPropertyModelEdit Model => _propertyModel;

        /// <summary>
        /// 透明度值（0-1之间）
        /// </summary>
        public double OpacityValue => 1.0 - (AppearanceInfo.Opacity / 100.0);

        /// <summary>
        /// 是否显示ToolTip
        /// </summary>
        public bool IsTooltipEnabled => !string.IsNullOrEmpty(CommonInfo.TooltipText);

        public CommonCursorType EffectiveHoverCursor =>
            CommonInfo.HoverCursor;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="propertyModel">属性模型</param>
        public SliderViewModel(SliderPropertyModelEdit propertyModel)
        {
            _propertyModel = propertyModel;

            // 监听属性变化，更新相关属性
            _propertyModel.PropertyChanged += PropertyModel_PropertyChanged;

            // 为所有子对象订阅PropertyChanged事件，确保所有属性变化都能被捕获
            _propertyModel.BrushInfo.PropertyChanged += ChildPropertyChanged;
            _propertyModel.AppearanceInfo.PropertyChanged += ChildPropertyChanged;
            _propertyModel.LayoutInfo.PropertyChanged += ChildPropertyChanged;
            _propertyModel.CommonInfo.PropertyChanged += ChildPropertyChanged;
        }

        public void ReloadFromModel()
        {
            RaisePropertyChanged(nameof(BrushInfo));
            RaisePropertyChanged(nameof(AppearanceInfo));
            RaisePropertyChanged(nameof(CommonInfo));
            RaisePropertyChanged(nameof(LayoutInfo));
            RaisePropertyChanged(nameof(Model));
            RaisePropertyChanged(nameof(OpacityValue));
            RaisePropertyChanged(nameof(EffectiveHoverCursor));
            RaisePropertyChanged(nameof(IsTooltipEnabled));
        }

        private void PropertyModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
        }

        private void ChildPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // 根据发送者类型确定需要通知的属性名
            string propertyName = string.Empty;
            if (sender is SliderBrushInfo)
            {
                propertyName = nameof(BrushInfo);
            }
            else if (sender is SliderAppearanceInfo)
            {
                propertyName = nameof(AppearanceInfo);
            }
            else if (sender is SliderLayoutInfo)
            {
                propertyName = nameof(LayoutInfo);
            }
            else if (sender is SliderCommonInfo)
            {
                propertyName = nameof(CommonInfo);
            }

            if (!string.IsNullOrEmpty(propertyName))
            {
                RaisePropertyChanged(propertyName);

                // 确保具体属性变化时UI能更新
                if (sender is SliderBrushInfo)
                {
                    RaisePropertyChanged($"{nameof(BrushInfo)}.{e.PropertyName}");
                }
                else if (sender is SliderCommonInfo)
                {
                    RaisePropertyChanged($"{nameof(CommonInfo)}.{e.PropertyName}");

                    if (e.PropertyName == nameof(SliderCommonInfo.HoverCursor))
                    {
                        RaisePropertyChanged(nameof(EffectiveHoverCursor));
                    }
                }
                else if (sender is SliderAppearanceInfo)
                {
                    RaisePropertyChanged($"{nameof(AppearanceInfo)}.{e.PropertyName}");

                    // 当Opacity变化时，引发OpacityValue变化通知
                    if (e.PropertyName == nameof(SliderAppearanceInfo.Opacity))
                    {
                        RaisePropertyChanged(nameof(OpacityValue));
                    }
                }
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="disposing">是否手动释放</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // 取消订阅事件
                    _propertyModel.PropertyChanged -= PropertyModel_PropertyChanged;
                    
                    _propertyModel.BrushInfo.PropertyChanged -= ChildPropertyChanged;
                    _propertyModel.AppearanceInfo.PropertyChanged -= ChildPropertyChanged;
                    _propertyModel.LayoutInfo.PropertyChanged -= ChildPropertyChanged;
                    _propertyModel.CommonInfo.PropertyChanged -= ChildPropertyChanged;
                }

                _disposed = true;
            }
        }
    }
}
