using System;
using System.ComponentModel;
using Avalonia.Media;
using PropertyModels.ComponentModel;
using System.Text.Json;
using GF_Gereric;
using Griffins;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace GKG.Map.MapCell.Generic.Control.MapCell_Slider
{
    /// <summary>
    /// 滑块画笔信息
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [ExpandableObjectDisplayMode(IsCategoryVisible = NullableBooleanType.No)]
    public class SliderBrushInfo : MiniReactiveObject, IJsonValueConvert, IMPPropObjectValue
    {
        /// <summary>
        /// 对象ID
        /// </summary>
        public static readonly Guid Object_ID = new Guid("{1A2B3C4D-5E6F-7A8B-9C0D-E1F2A3B4C5D8}");

        /// <summary>
        /// 默认值
        /// </summary>
        public static readonly SliderBrushInfo Default = new SliderBrushInfo();

        private string _backgroundColorStr = Colors.Blue.ToColorString();

        /// <summary>
        /// 背景色
        /// </summary>
        [DisplayName("背景色")]
        [Category("画笔")]
        public Color BackgroundColor
        {
            get => Color.Parse(_backgroundColorStr);
            set => SetProperty(ref _backgroundColorStr, value.ToColorString(), nameof(BackgroundColor));
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
                    throw new Exception("对象值不是SliderBrushInfo转换的");
                if ((baseValue.Val as ObjectValue_Json).Object_ID != Object_ID)
                    throw new Exception("对象值不是SliderBrushInfo转换的");
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
            if (rootElement.TryGetProperty("BackgroundColor", out value))
                _backgroundColorStr = value.GetString() ?? Colors.White.ToColorString();
        }

        string IJsonValueConvert.ToJsonDataObject()
        {
            var value = new
            {
                BackgroundColor = _backgroundColorStr
            };
            return System.Text.Json.JsonSerializer.Serialize(value);
        }

        #endregion
    }
}