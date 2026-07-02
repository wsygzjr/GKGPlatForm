using System;
using System.ComponentModel;
using GF_Gereric;
using PropertyModels.ComponentModel;

namespace GKG.Map.MapCell.Generic.Control.Lable
{
    /// <summary>
    /// 标签图元属性绑定编辑模型对象
    /// 彻底摒弃 CommHelper，采用标准原生实例化与深拷贝
    /// </summary>
    [Serializable]
    [MapPropertyOrder]
    [CategoryPriority("绑定信息", 1)]
    public class LablePropertyBindEditModel : ControlCellPropertyBindEditModel
    {
        private PropertyBindInfo _lableText = new PropertyBindInfo(GriffinsBaseDataType.String);
        private PropertyBindInfo _foregroundColor = new PropertyBindInfo(GriffinsBaseDataType.String);
        private PropertyBindInfo _backgroundColor = new PropertyBindInfo(GriffinsBaseDataType.String);

        [DisplayName("标签文本")]
        [Category("绑定信息")]
        [PropertySortOrder(1)]
        [BindMPPropertyID]
        public PropertyBindInfo LableText
        {
            get => _lableText;
            set
            {
                var clone = new PropertyBindInfo(GriffinsBaseDataType.String);
                clone.CopyFrom(value);
                SetProperty(ref _lableText, clone);
            }
        }

        [DisplayName("文字颜色")]
        [Category("绑定信息")]
        [PropertySortOrder(2)]
        [BindMPPropertyID]
        public PropertyBindInfo ForegroundColor
        {
            get => _foregroundColor;
            set
            {
                var clone = new PropertyBindInfo(GriffinsBaseDataType.String);
                clone.CopyFrom(value);
                SetProperty(ref _foregroundColor, clone);
            }
        }

        [DisplayName("背景颜色")]
        [Category("绑定信息")]
        [PropertySortOrder(3)]
        [BindMPPropertyID]
        public PropertyBindInfo BackgroundColor
        {
            get => _backgroundColor;
            set
            {
                var clone = new PropertyBindInfo(GriffinsBaseDataType.String);
                clone.CopyFrom(value);
                SetProperty(ref _backgroundColor, clone);
            }
        }

        public void CopyFrom(LablePropertyBindEditModel source)
        {
            if (source == null) return;
            base.CopyFrom(source);
            LableText = source.LableText;
            ForegroundColor = source.ForegroundColor;
            BackgroundColor = source.BackgroundColor;
        }
    }
}