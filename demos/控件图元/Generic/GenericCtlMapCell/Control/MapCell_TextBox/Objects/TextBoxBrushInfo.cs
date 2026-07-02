using System;
using System.ComponentModel;
using Avalonia.Media;
using PropertyModels.ComponentModel;
using System.Text.Json;
using GF_Gereric;
using Griffins;

namespace GKG.Map.MapCell.Generic.Control.MapCell_TextBox
{
    /// <summary>
    /// 文本输入框画笔信息
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [ExpandableObjectDisplayMode(IsCategoryVisible = NullableBooleanType.No)]
    public class TextBoxBrushInfo : MiniReactiveObject, IJsonValueConvert, IMPPropObjectValue
    {
        public static readonly Guid Object_ID = new Guid("{C7A47B7C-9C4A-46A1-A45F-68E0712B6A10}");
        public static readonly TextBoxBrushInfo Default = new TextBoxBrushInfo();

        // 颜色内部使用字符串持久化（便于 Json 存储/兼容不同序列化来源），对外仍暴露 Color 类型
        private string _backgroundColorStr = Colors.White.ToColorString();
        private string _borderColorStr = Colors.Gray.ToColorString();
        private string _foregroundColorStr = Colors.Black.ToColorString();
        private string _selectedBorderColorStr = Colors.DodgerBlue.ToColorString();

        [DisplayName("背景色")]
        [Category("画笔")]
        public Color BackgroundColor
        {
            get => Color.Parse(_backgroundColorStr);
            set => SetProperty(ref _backgroundColorStr, value.ToColorString(), nameof(BackgroundColor));
        }

        [DisplayName("边框颜色")]
        [Category("画笔")]
        public Color BorderColor
        {
            get => Color.Parse(_borderColorStr);
            set => SetProperty(ref _borderColorStr, value.ToColorString(), nameof(BorderColor));
        }

        [DisplayName("前景色")]
        [Category("画笔")]
        public Color ForegroundColor
        {
            get => Color.Parse(_foregroundColorStr);
            set => SetProperty(ref _foregroundColorStr, value.ToColorString(), nameof(ForegroundColor));
        }

        [DisplayName("选中时边框颜色")]
        [Category("画笔")]
        public Color SelectedBorderColor
        {
            get => Color.Parse(_selectedBorderColorStr);
            set => SetProperty(ref _selectedBorderColorStr, value.ToColorString(), nameof(SelectedBorderColor));
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
                    throw new Exception("对象值不是TextBoxBrushInfo转换的");
                if (ov.Object_ID != Object_ID)
                    throw new Exception("对象值不是TextBoxBrushInfo转换的");
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

            if (root.TryGetProperty("BackgroundColor", out var v1))
                _backgroundColorStr = v1.GetString() ?? Colors.White.ToColorString();
            if (root.TryGetProperty("BorderColor", out var v2))
                _borderColorStr = v2.GetString() ?? Colors.Gray.ToColorString();
            if (root.TryGetProperty("ForegroundColor", out var v3))
                _foregroundColorStr = v3.GetString() ?? Colors.Black.ToColorString();
            if (root.TryGetProperty("SelectedBorderColor", out var v4))
                _selectedBorderColorStr = v4.GetString() ?? Colors.DodgerBlue.ToColorString();
        }

        string IJsonValueConvert.ToJsonDataObject()
        {
            // 字段名保持与属性名一致，便于跨版本兼容
            var value = new
            {
                BackgroundColor = _backgroundColorStr,
                BorderColor = _borderColorStr,
                ForegroundColor = _foregroundColorStr,
                SelectedBorderColor = _selectedBorderColorStr
            };
            return System.Text.Json.JsonSerializer.Serialize(value);
        }
    }
}
