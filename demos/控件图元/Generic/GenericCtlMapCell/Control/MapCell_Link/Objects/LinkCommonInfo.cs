using System;
using System.ComponentModel;
using System.Text.Json;
using GF_Gereric;
using Griffins;
using PropertyModels.ComponentModel;

namespace GKG.Map.MapCell.Generic.Link
{
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [ExpandableObjectDisplayMode(IsCategoryVisible = NullableBooleanType.No)]
    public class LinkCommonInfo : MiniReactiveObject, IJsonValueConvert, IMPPropObjectValue
    {
        public static readonly Guid Object_ID = new Guid("{B570D0B4-26CD-4CA5-8C4A-B7A4B4D08112}");
        public static readonly LinkCommonInfo Default = new LinkCommonInfo("链接", string.Empty);

        private string _linkText = "链接";
        private string _address = string.Empty;

        public LinkCommonInfo()
            : this("链接", string.Empty)
        {
        }

        public LinkCommonInfo(string linkText, string address)
        {
            LinkText = linkText;
            Address = address;
        }

        [DisplayName("链接文本")]
        [Category("公共")]
        public string LinkText
        {
            get => _linkText;
            set => SetProperty(ref _linkText, value ?? string.Empty, nameof(LinkText));
        }

        [DisplayName("跳转地址")]
        [Category("公共")]
        public string Address
        {
            get => _address;
            set => SetProperty(ref _address, value ?? string.Empty, nameof(Address));
        }

        bool IMPPropObjectValue.IsObject_Byte => false;

        Guid IMPPropObjectValue.GetObject_ID() => Object_ID;

        GriffinsBaseValue IMPPropObjectValue.ToBaseValue()
        {
            var objectValue = new ObjectValue_Json(Object_ID)
            {
                JsonVal = ((IJsonValueConvert)this).ToJsonDataObject()
            };
            return GriffinsBaseValue.Create(objectValue);
        }

        void IMPPropObjectValue.PopulateFromBaseValue(GriffinsBaseValue baseValue)
        {
            if (baseValue?.Val is not ObjectValue_Json objectValue || objectValue.Object_ID != Object_ID)
                throw new Exception("对象值不是LinkCommonInfo转换的");

            ((IJsonValueConvert)this).FromJsonDataObject(objectValue.JsonVal);
        }

        void IJsonValueConvert.FromJsonDataObject(string jsonDataObject)
        {
            if (string.IsNullOrWhiteSpace(jsonDataObject))
                throw new ArgumentNullException(nameof(jsonDataObject));

            using var jsonDocument = JsonDocument.Parse(jsonDataObject);
            var root = jsonDocument.RootElement;
            if (root.TryGetProperty(nameof(LinkText), out var linkText))
                _linkText = linkText.GetString() ?? Default.LinkText;
            if (root.TryGetProperty(nameof(Address), out var address))
                _address = address.GetString() ?? Default.Address;
        }

        string IJsonValueConvert.ToJsonDataObject()
        {
            return JsonSerializer.Serialize(new
            {
                LinkText,
                Address
            });
        }

        public override string ToString() => string.IsNullOrWhiteSpace(LinkText) ? "(空)" : LinkText;
    }
}
