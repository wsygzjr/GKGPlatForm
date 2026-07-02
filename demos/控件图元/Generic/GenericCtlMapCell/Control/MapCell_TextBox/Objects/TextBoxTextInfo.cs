using System;
using System.ComponentModel;
using Avalonia.Media;
using PropertyModels.ComponentModel;
using System.Text.Json;
using GF_Gereric;
using Griffins;

namespace GKG.Map.MapCell.Generic.Control.MapCell_TextBox
{
    public enum TextBoxFontFamilyType
    {
        [Description("默认")] 默认 = 0,
        [Description("微软雅黑")] 微软雅黑 = 1,
        [Description("宋体")] 宋体 = 2,
        [Description("黑体")] 黑体 = 3,
        [Description("Segoe UI")] 英文_SegoeUI = 4,
        [Description("Arial")] 英文_Arial = 5,
        [Description("Consolas")] 等宽_Consolas = 6,
        [Description("Times New Roman")] 英文_TimesNewRoman = 7
    }

    public enum TextBoxTextAlignmentType
    {
        [Description("左对齐")] Left = 0,
        [Description("居中")] Center = 1,
        [Description("右对齐")] Right = 2,
        [Description("两端对齐")] Justify = 3
    }

    public enum TextBoxVerticalTextAlignmentType
    {
        [Description("顶部")] Top = 0,
        [Description("居中")] Center = 1,
        [Description("底部")] Bottom = 2,
        [Description("拉伸")] Stretch = 3
    }

    /// <summary>
    /// 文本输入框文本信息
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [ExpandableObjectDisplayMode(IsCategoryVisible = NullableBooleanType.No)]
    public class TextBoxTextInfo : MiniReactiveObject, IJsonValueConvert, IMPPropObjectValue
    {
        public static readonly Guid Object_ID = new Guid("{C7A47B7C-9C4A-46A1-A45F-68E0712B6A14}");
        public static readonly TextBoxTextInfo Default = new TextBoxTextInfo();

        private TextBoxFontFamilyType _fontFamily = TextBoxFontFamilyType.微软雅黑;
        // 颜色内部使用字符串持久化（便于 Json 存储/兼容不同序列化来源），对外仍暴露 Color 类型
        private string _fontColorStr = Colors.Black.ToColorString();
        private int _fontSize = 14;
        private bool _isItalic;
        private bool _isBold;
        private TextBoxTextAlignmentType _textAlignment = TextBoxTextAlignmentType.Left;
        private TextBoxVerticalTextAlignmentType _verticalTextAlignment = TextBoxVerticalTextAlignmentType.Center;

        [DisplayName("字体族")]
        [Category("文本")]
        public TextBoxFontFamilyType FontFamily
        {
            get => _fontFamily;
            set => SetProperty(ref _fontFamily, value);
        }

        private static string ToFontFamilyName(TextBoxFontFamilyType v)
        {
            return v switch
            {
                TextBoxFontFamilyType.微软雅黑 => "Microsoft YaHei",
                TextBoxFontFamilyType.宋体 => "SimSun",
                TextBoxFontFamilyType.黑体 => "SimHei",
                TextBoxFontFamilyType.英文_SegoeUI => "Segoe UI",
                TextBoxFontFamilyType.英文_Arial => "Arial",
                TextBoxFontFamilyType.等宽_Consolas => "Consolas",
                TextBoxFontFamilyType.英文_TimesNewRoman => "Times New Roman",
                _ => string.Empty
            };
        }

        private static TextBoxFontFamilyType FromFontFamilyName(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
                return TextBoxFontFamilyType.默认;

            s = s.Trim();
            return s switch
            {
                "Microsoft YaHei" => TextBoxFontFamilyType.微软雅黑,
                "SimSun" => TextBoxFontFamilyType.宋体,
                "SimHei" => TextBoxFontFamilyType.黑体,
                "Segoe UI" => TextBoxFontFamilyType.英文_SegoeUI,
                "Arial" => TextBoxFontFamilyType.英文_Arial,
                "Consolas" => TextBoxFontFamilyType.等宽_Consolas,
                "Times New Roman" => TextBoxFontFamilyType.英文_TimesNewRoman,
                _ => TextBoxFontFamilyType.默认
            };
        }

        [DisplayName("字体颜色")]
        [Category("文本")]
        public Color FontColor
        {
            get => Color.Parse(_fontColorStr);
            set => SetProperty(ref _fontColorStr, value.ToColorString(), nameof(FontColor));
        }

        [DisplayName("字体大小")]
        [Category("文本")]
        public int FontSize
        {
            get => _fontSize;
            set
            {
                // 字号不允许小于等于 0
                if (value <= 0) value = 1;
                SetProperty(ref _fontSize, value);
            }
        }

        [DisplayName("是否斜体")]
        [Category("文本")]
        public bool IsItalic
        {
            get => _isItalic;
            set => SetProperty(ref _isItalic, value);
        }

        [DisplayName("是否加粗")]
        [Category("文本")]
        public bool IsBold
        {
            get => _isBold;
            set => SetProperty(ref _isBold, value);
        }

        [DisplayName("水平文本对齐")]
        [Category("文本")]
        public TextBoxTextAlignmentType TextAlignment
        {
            get => _textAlignment;
            set => SetProperty(ref _textAlignment, value);
        }

        [DisplayName("垂直文本对齐")]
        [Category("文本")]
        public TextBoxVerticalTextAlignmentType VerticalTextAlignment
        {
            get => _verticalTextAlignment;
            set => SetProperty(ref _verticalTextAlignment, value);
        }

        bool IMPPropObjectValue.IsObject_Byte => false;
        Guid IMPPropObjectValue.GetObject_ID() => Object_ID;

        GriffinsBaseValue IMPPropObjectValue.ToBaseValue()
        {
            // 将对象序列化为 GriffinsBaseValue（ObjectValue_Json）供属性系统存储
            ObjectValue_Json objectValue_Json = new ObjectValue_Json(Object_ID);
            objectValue_Json.JsonVal = ((IJsonValueConvert)this).ToJsonDataObject();
            return GriffinsBaseValue.Create(objectValue_Json);
        }

        void IMPPropObjectValue.PopulateFromBaseValue(GriffinsBaseValue baseValue)
        {
            if (baseValue != null && baseValue.Val != null)
            {
                if (baseValue.Val is not ObjectValue_Json ov)
                    throw new Exception("对象值不是TextBoxTextInfo转换的");
                if (ov.Object_ID != Object_ID)
                    throw new Exception("对象值不是TextBoxTextInfo转换的");
                // 由 Json 反填充内部字段
                ((IJsonValueConvert)this).FromJsonDataObject(ov.JsonVal);
            }
        }

        void IJsonValueConvert.FromJsonDataObject(string jsonDataObject)
        {
            if (string.IsNullOrEmpty(jsonDataObject))
                throw new ArgumentNullException(nameof(jsonDataObject));

            using JsonDocument jsonDocument = JsonDocument.Parse(jsonDataObject);
            JsonElement root = jsonDocument.RootElement;

            if (root.TryGetProperty("FontFamily", out var ff))
            {
                if (ff.ValueKind == JsonValueKind.String)
                    _fontFamily = FromFontFamilyName(ff.GetString() ?? string.Empty);
                else if (ff.ValueKind == JsonValueKind.Number && ff.TryGetInt32(out var i) && Enum.IsDefined(typeof(TextBoxFontFamilyType), i))
                    _fontFamily = (TextBoxFontFamilyType)i;
            }
            if (root.TryGetProperty("FontColor", out var fc) && fc.ValueKind == JsonValueKind.String)
                _fontColorStr = fc.GetString() ?? _fontColorStr;
            if (root.TryGetProperty("FontSize", out var fs) && fs.ValueKind == JsonValueKind.Number)
            {
                // 兼容历史序列化为 double/int 两种
                int size;
                if (!fs.TryGetInt32(out size))
                    size = (int)Math.Round(fs.GetDouble());
                _fontSize = Math.Max(1, size);
            }
            if (root.TryGetProperty("IsItalic", out var it) && (it.ValueKind == JsonValueKind.True || it.ValueKind == JsonValueKind.False))
                _isItalic = it.GetBoolean();
            if (root.TryGetProperty("IsBold", out var bd) && (bd.ValueKind == JsonValueKind.True || bd.ValueKind == JsonValueKind.False))
                _isBold = bd.GetBoolean();
            if (root.TryGetProperty("TextAlignment", out var ta))
            {
                if (ta.ValueKind == JsonValueKind.String && Enum.TryParse<TextBoxTextAlignmentType>(ta.GetString(), out var align))
                    _textAlignment = align;
                else if (ta.ValueKind == JsonValueKind.Number && ta.TryGetInt32(out var alignInt) && Enum.IsDefined(typeof(TextBoxTextAlignmentType), alignInt))
                    _textAlignment = (TextBoxTextAlignmentType)alignInt;
            }
            if (root.TryGetProperty("VerticalTextAlignment", out var vta))
            {
                if (vta.ValueKind == JsonValueKind.String && Enum.TryParse<TextBoxVerticalTextAlignmentType>(vta.GetString(), out var verticalAlign))
                    _verticalTextAlignment = verticalAlign;
                else if (vta.ValueKind == JsonValueKind.Number && vta.TryGetInt32(out var verticalAlignInt) && Enum.IsDefined(typeof(TextBoxVerticalTextAlignmentType), verticalAlignInt))
                    _verticalTextAlignment = (TextBoxVerticalTextAlignmentType)verticalAlignInt;
            }
        }

        string IJsonValueConvert.ToJsonDataObject()
        {
            var value = new
            {
                FontFamily = ToFontFamilyName(_fontFamily),
                FontColor = _fontColorStr,
                FontSize = _fontSize,
                IsItalic = _isItalic,
                IsBold = _isBold,
                TextAlignment = _textAlignment.ToString(),
                VerticalTextAlignment = _verticalTextAlignment.ToString()
            };
            return System.Text.Json.JsonSerializer.Serialize(value);
        }
    }
}
