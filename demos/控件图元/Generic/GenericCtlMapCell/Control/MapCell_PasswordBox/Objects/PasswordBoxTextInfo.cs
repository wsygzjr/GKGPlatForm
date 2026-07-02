using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using GF_Gereric;
using Griffins;
using PropertyModels.ComponentModel;

namespace GKG.Map.MapCell.Generic.PasswordBox
{
    /// <summary>
    /// 字体族类型枚举
    /// </summary>
    public enum FontFamilyType
    {
        [Description("微软雅黑")] MicrosoftYaHei = 0,
        [Description("宋体")] SimSun = 1,
        [Description("黑体")] SimHei = 2,
        [Description("楷体")] KaiTi = 3,
        [Description("仿宋")] FangSong = 4,
        [Description("新宋体")] NSimSun = 5,
        [Description("Arial")] Arial = 6,
        [Description("Times New Roman")] TimesNewRoman = 7,
        [Description("Consolas")] Consolas = 8,
        [Description("Segoe UI")] SegoeUI = 9,
    }

    /// <summary>
    /// 密码输入框文本信息
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [ExpandableObjectDisplayMode(IsCategoryVisible = NullableBooleanType.No)]
    public class PasswordBoxTextInfo : MiniReactiveObject, IJsonValueConvert, IMPPropObjectValue
    {
        public static readonly Guid Object_ID = new Guid("{B1C2D3E4-5001-4AC8-BF66-281412CDE105}");
        public static readonly PasswordBoxTextInfo Default = new PasswordBoxTextInfo();

        private FontFamilyType _fontFamily = FontFamilyType.MicrosoftYaHei;
        private double _fontSize = 14;
        private bool _isItalic = false;
        private bool _isBold = false;

        [DisplayName("字体族")]
        public FontFamilyType FontFamily
        {
            get => _fontFamily;
            set => SetProperty(ref _fontFamily, value, nameof(FontFamily));
        }

        [DisplayName("字体大小")]
        [FloatPrecision(0)]
        [Range(1, 200)]
        public double FontSize
        {
            get => _fontSize;
            set => SetProperty(ref _fontSize, value, nameof(FontSize));
        }

        [DisplayName("斜体")]
        public bool IsItalic
        {
            get => _isItalic;
            set => SetProperty(ref _isItalic, value, nameof(IsItalic));
        }

        [DisplayName("粗体")]
        public bool IsBold
        {
            get => _isBold;
            set => SetProperty(ref _isBold, value, nameof(IsBold));
        }

        #region IMPPropObjectValue 实现

        bool IMPPropObjectValue.IsObject_Byte => false;
        Guid IMPPropObjectValue.GetObject_ID() => Object_ID;

        GriffinsBaseValue IMPPropObjectValue.ToBaseValue()
        {
            ObjectValue_Json objectValue_Json = new ObjectValue_Json(Object_ID);
            objectValue_Json.JsonVal = ((IJsonValueConvert)this).ToJsonDataObject();
            return GriffinsBaseValue.Create(objectValue_Json);
        }

        void IMPPropObjectValue.PopulateFromBaseValue(GriffinsBaseValue baseValue)
        {
            if (baseValue != null && baseValue.Val != null)
            {
                if (!(baseValue.Val is ObjectValue_Json))
                    throw new Exception("对象值不是PasswordBoxTextInfo转换的");
                if ((baseValue.Val as ObjectValue_Json).Object_ID != Object_ID)
                    throw new Exception("对象值不是PasswordBoxTextInfo转换的");
                ((IJsonValueConvert)this).FromJsonDataObject((baseValue.Val as ObjectValue_Json).JsonVal);
            }
        }

        #endregion

        #region IJsonValueConvert 实现

        void IJsonValueConvert.FromJsonDataObject(string jsonDataObject)
        {
            if (string.IsNullOrEmpty(jsonDataObject))
                throw new ArgumentNullException(nameof(jsonDataObject));

            using JsonDocument jsonDocument = JsonDocument.Parse(jsonDataObject);
            JsonElement rootElement = jsonDocument.RootElement;

            JsonElement value;
            if (rootElement.TryGetProperty("FontFamily", out value))
            {
                var str = value.GetString();
                if (Enum.TryParse<FontFamilyType>(str, out var fontType))
                    FontFamily = fontType;
                else
                    FontFamily = FontFamilyType.MicrosoftYaHei;
            }
            if (rootElement.TryGetProperty("FontSize", out value))
                FontSize = value.GetDouble();
            if (rootElement.TryGetProperty("IsItalic", out value))
                IsItalic = value.GetBoolean();
            if (rootElement.TryGetProperty("IsBold", out value))
                IsBold = value.GetBoolean();
        }

        string IJsonValueConvert.ToJsonDataObject()
        {
            var value = new
            {
                FontFamily = _fontFamily.ToString(),
                FontSize = _fontSize,
                IsItalic = _isItalic,
                IsBold = _isBold
            };
            return System.Text.Json.JsonSerializer.Serialize(value);
        }

        #endregion
    }
}
