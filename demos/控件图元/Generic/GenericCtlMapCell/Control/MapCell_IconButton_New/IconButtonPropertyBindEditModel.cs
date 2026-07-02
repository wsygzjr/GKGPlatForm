using System;
using System.ComponentModel;
using GF_Gereric;
using PropertyModels.ComponentModel;

namespace GKG.Map.MapCell.Generic.IconButton
{
    [Serializable]
    [MapPropertyOrder]
    [CategoryPriority("绑定信息", 1)]
    public class IconButtonPropertyBindEditModel : ControlCellPropertyBindEditModel
    {
        private PropertyBindInfo _buttonText = new PropertyBindInfo(GriffinsBaseDataType.String);
        private PropertyBindInfo _backgroundColor = new PropertyBindInfo(GriffinsBaseDataType.String);
        private PropertyBindInfo _isEnabled = new PropertyBindInfo(GriffinsBaseDataType.Bool);
        private PropertyBindInfo _iconBase64 = new PropertyBindInfo(GriffinsBaseDataType.String); // 允许后端动态下发 Base64 切换图片！

        [DisplayName("按钮文本")]
        [Category("绑定信息")]
        [BindMPPropertyID]
        public PropertyBindInfo ButtonText { get => _buttonText; set { var clone = new PropertyBindInfo(GriffinsBaseDataType.String); clone.CopyFrom(value); SetProperty(ref _buttonText, clone); } }

        [DisplayName("背景颜色")]
        [Category("绑定信息")]
        [BindMPPropertyID]
        public PropertyBindInfo BackgroundColor { get => _backgroundColor; set { var clone = new PropertyBindInfo(GriffinsBaseDataType.String); clone.CopyFrom(value); SetProperty(ref _backgroundColor, clone); } }

        [DisplayName("是否启用")]
        [Category("绑定信息")]
        [BindMPPropertyID]
        public PropertyBindInfo IsEnabled { get => _isEnabled; set { var clone = new PropertyBindInfo(GriffinsBaseDataType.Bool); clone.CopyFrom(value); SetProperty(ref _isEnabled, clone); } }

        [DisplayName("图标(Base64)")]
        [Category("绑定信息")]
        [BindMPPropertyID]
        public PropertyBindInfo IconBase64 { get => _iconBase64; set { var clone = new PropertyBindInfo(GriffinsBaseDataType.String); clone.CopyFrom(value); SetProperty(ref _iconBase64, clone); } }

        public void CopyFrom(IconButtonPropertyBindEditModel source)
        {
            if (source == null) return;
            base.CopyFrom(source);
            ButtonText = source.ButtonText;
            BackgroundColor = source.BackgroundColor;
            IsEnabled = source.IsEnabled;
            IconBase64 = source.IconBase64;
        }
    }
}