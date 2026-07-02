using System;
using System.ComponentModel;
using Avalonia.Media;
using GF_Gereric;
using Griffins;
using PropertyModels.ComponentModel;

namespace GKG.Map.MapCell.Generic.Link
{
    [Serializable]
    [MapPropertyOrder]
    [CategoryPriority("图元信息", 1)]
    [CategoryPriority("画笔", 2)]
    [CategoryPriority("公共", 3)]
    public class LinkPropertyModelEdit : ControlCellPropertyModelEdit
    {
        private LinkBrushInfo _brushInfo = new LinkBrushInfo();
        private LinkCommonInfo _commonInfo = new LinkCommonInfo();

        [DisplayName("画笔设置")]
        [Category("画笔")]
        [PropertySortOrder(1)]
        public LinkBrushInfo BrushInfo
        {
            get => _brushInfo;
            set => SetProperty(ref _brushInfo, value);
        }

        [DisplayName("公共设置")]
        [Category("公共")]
        [PropertySortOrder(1)]
        public LinkCommonInfo CommonInfo
        {
            get => _commonInfo;
            set => SetProperty(ref _commonInfo, value);
        }

        public override bool SetPropertyValue(string propertyID, MapCellPropValue propertyVal)
        {
            if (string.Equals(propertyID, nameof(LinkCommonInfo.LinkText), StringComparison.Ordinal))
            {
                _commonInfo ??= new LinkCommonInfo();
                _commonInfo.LinkText = propertyVal != null ? propertyVal.ToPrimitiveValue<string>() : LinkCommonInfo.Default.LinkText;
                RaisePropertyChanged(nameof(CommonInfo));
                return true;
            }

            if (string.Equals(propertyID, nameof(LinkCommonInfo.Address), StringComparison.Ordinal))
            {
                _commonInfo ??= new LinkCommonInfo();
                _commonInfo.Address = propertyVal != null ? propertyVal.ToPrimitiveValue<string>() : LinkCommonInfo.Default.Address;
                RaisePropertyChanged(nameof(CommonInfo));
                return true;
            }

            if (string.Equals(propertyID, nameof(LinkBrushInfo.TextColor), StringComparison.Ordinal))
            {
                _brushInfo ??= new LinkBrushInfo();
                _brushInfo.TextColor = propertyVal != null ? Color.Parse(propertyVal.ToPrimitiveValue<string>()) : LinkBrushInfo.Default.TextColor;
                RaisePropertyChanged(nameof(BrushInfo));
                return true;
            }

            if (string.Equals(propertyID, nameof(LinkBrushInfo.HoverTextColor), StringComparison.Ordinal))
            {
                _brushInfo ??= new LinkBrushInfo();
                _brushInfo.HoverTextColor = propertyVal != null ? Color.Parse(propertyVal.ToPrimitiveValue<string>()) : LinkBrushInfo.Default.HoverTextColor;
                RaisePropertyChanged(nameof(BrushInfo));
                return true;
            }

            if (string.Equals(propertyID, nameof(BrushInfo), StringComparison.Ordinal))
            {
                var src = propertyVal != null ? DeserializeObject<LinkBrushInfo>(propertyVal) : new LinkBrushInfo();
                _brushInfo ??= new LinkBrushInfo();
                _brushInfo.TextColor = src.TextColor;
                _brushInfo.HoverTextColor = src.HoverTextColor;
                RaisePropertyChanged(nameof(BrushInfo));
                return true;
            }

            if (string.Equals(propertyID, nameof(CommonInfo), StringComparison.Ordinal))
            {
                var src = propertyVal != null ? DeserializeObject<LinkCommonInfo>(propertyVal) : new LinkCommonInfo();
                _commonInfo ??= new LinkCommonInfo();
                _commonInfo.LinkText = src.LinkText;
                _commonInfo.Address = src.Address;
                RaisePropertyChanged(nameof(CommonInfo));
                return true;
            }

            return base.SetPropertyValue(propertyID, propertyVal);
        }

        public void CopyFrom(LinkPropertyModelEdit source)
        {
            if (source == null)
                return;

            base.CopyFrom(source);

            _brushInfo ??= new LinkBrushInfo();
            _brushInfo.TextColor = source.BrushInfo?.TextColor ?? LinkBrushInfo.Default.TextColor;
            _brushInfo.HoverTextColor = source.BrushInfo?.HoverTextColor ?? LinkBrushInfo.Default.HoverTextColor;
            RaisePropertyChanged(nameof(BrushInfo));

            _commonInfo ??= new LinkCommonInfo();
            _commonInfo.LinkText = source.CommonInfo?.LinkText ?? LinkCommonInfo.Default.LinkText;
            _commonInfo.Address = source.CommonInfo?.Address ?? LinkCommonInfo.Default.Address;
            RaisePropertyChanged(nameof(CommonInfo));
        }

        private static T DeserializeObject<T>(MapCellPropValue val) where T : IMPPropObjectValue, new()
        {
            var objectValue = val.ToObjectValue_Json();
            var baseValue = GriffinsBaseValue.Create(objectValue);
            IMPPropObjectValue obj = new T();
            obj.PopulateFromBaseValue(baseValue);
            return (T)obj;
        }
    }
}
