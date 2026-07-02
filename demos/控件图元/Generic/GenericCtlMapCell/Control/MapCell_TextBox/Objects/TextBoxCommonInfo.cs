using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using PropertyModels.ComponentModel;
using System.Text.Json;
using GF_Gereric;
using Griffins;

namespace GKG.Map.MapCell.Generic.Control.MapCell_TextBox
{
    /// <summary>
    /// 通用光标类型（属性面板显示用）
    /// </summary>
    public enum CommonCursorType
    {
        [Description("默认箭头")] 默认箭头 = 0,
        [Description("文本输入")] 文本输入 = 1,
        [Description("等待")] 等待 = 2,
        [Description("十字")] 十字 = 3,
        [Description("向上箭头")] 向上箭头 = 4,
        [Description("左右调整")] 左右调整 = 5,
        [Description("上下调整")] 上下调整 = 6,
        [Description("移动")] 移动 = 7,
        [Description("禁止")] 禁止 = 8,
        [Description("手型")] 手型 = 9,
        [Description("启动中")] 启动中 = 10,
        [Description("帮助")] 帮助 = 11
    }

    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [ExpandableObjectDisplayMode(IsCategoryVisible = NullableBooleanType.No)]
    /// <summary>
    /// 文本输入框公共信息
    /// </summary>
    public class TextBoxCommonInfo : MiniReactiveObject, IJsonValueConvert, IMPPropObjectValue
    {
        public static readonly Guid Object_ID = new Guid("{C7A47B7C-9C4A-46A1-A45F-68E0712B6A13}");
        public static readonly TextBoxCommonInfo Default = new TextBoxCommonInfo();

        private string _text = string.Empty;
        private string _tooltipText = string.Empty;
        private bool _enabled = true;
        private bool _isReadOnly;

        private int _selectedTextOpacity = 100;
        private bool _enableSpellCheck;

        private CommonCursorType _hoverCursor = CommonCursorType.文本输入;

        [DisplayName("所选文本的不透明度")]
        [Category("公共")]
        [Range(0, 100)]
        public int SelectedTextOpacity
        {
            get => _selectedTextOpacity;
            // 属性面板范围：0-100
            set => SetProperty(ref _selectedTextOpacity, Math.Max(0, Math.Min(100, value)));
        }

        [DisplayName("是否启用拼写检查")]
        [Category("公共")]
        public bool EnableSpellCheck
        {
            get => _enableSpellCheck;
            set => SetProperty(ref _enableSpellCheck, value);
        }

        [DisplayName("文本内容")]
        [Category("公共")]
        public string Text
        {
            get => _text;
            // 文本内容为空时统一为 string.Empty，避免 null 传播
            set => SetProperty(ref _text, value ?? string.Empty);
        }

        [DisplayName("鼠标悬停的光标")]
        [Category("公共")]
        public CommonCursorType HoverCursor
        {
            get => _hoverCursor;
            set => SetProperty(ref _hoverCursor, value);
        }

        [DisplayName("是否启用")]
        [Category("公共")]
        public bool Enabled
        {
            get => _enabled;
            set => SetProperty(ref _enabled, value);
        }

        [DisplayName("是否只读")]
        [Category("公共")]
        public bool IsReadOnly
        {
            get => _isReadOnly;
            set => SetProperty(ref _isReadOnly, value);
        }

        [DisplayName("提示文本")]
        [Category("公共")]
        public string TooltipText
        {
            get => _tooltipText;
            set => SetProperty(ref _tooltipText, value ?? string.Empty);
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
                    throw new Exception("对象值不是TextBoxCommonInfo转换的");
                if (ov.Object_ID != Object_ID)
                    throw new Exception("对象值不是TextBoxCommonInfo转换的");
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

            if (root.TryGetProperty("SelectedTextOpacity", out var so) && so.ValueKind == JsonValueKind.Number)
                _selectedTextOpacity = so.GetInt32();
            if (root.TryGetProperty("EnableSpellCheck", out var sc) && (sc.ValueKind == JsonValueKind.True || sc.ValueKind == JsonValueKind.False))
                _enableSpellCheck = sc.GetBoolean();
            if (root.TryGetProperty("Text", out var t) && t.ValueKind == JsonValueKind.String)
                _text = t.GetString() ?? string.Empty;
            if (root.TryGetProperty("HoverCursor", out var hc))
            {
                // 兼容字符串与数字两种枚举序列化形式
                if (hc.ValueKind == JsonValueKind.String && Enum.TryParse<CommonCursorType>(hc.GetString(), out var ce))
                    _hoverCursor = ce;
                else if (hc.ValueKind == JsonValueKind.Number && hc.TryGetInt32(out var i) && Enum.IsDefined(typeof(CommonCursorType), i))
                    _hoverCursor = (CommonCursorType)i;
            }
            if (root.TryGetProperty("Enabled", out var en) && (en.ValueKind == JsonValueKind.True || en.ValueKind == JsonValueKind.False))
                _enabled = en.GetBoolean();
            if (root.TryGetProperty("IsReadOnly", out var ro) && (ro.ValueKind == JsonValueKind.True || ro.ValueKind == JsonValueKind.False))
                _isReadOnly = ro.GetBoolean();
            if (root.TryGetProperty("TooltipText", out var tip) && tip.ValueKind == JsonValueKind.String)
                _tooltipText = tip.GetString() ?? string.Empty;
        }

        string IJsonValueConvert.ToJsonDataObject()
        {
            var value = new
            {
                SelectedTextOpacity = _selectedTextOpacity,
                EnableSpellCheck = _enableSpellCheck,
                Text = _text,
                HoverCursor = _hoverCursor.ToString(),
                Enabled = _enabled,
                IsReadOnly = _isReadOnly,
                TooltipText = _tooltipText
            };
            return System.Text.Json.JsonSerializer.Serialize(value);
        }
    }
}
