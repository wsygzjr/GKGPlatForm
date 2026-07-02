using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using PropertyModels.ComponentModel;
using System.Text.Json;
using GF_Gereric;
using Griffins;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace GKG.Map.MapCell.Generic.Control.MapCell_Slider
{
    /// <summary>
    /// 滑块外观信息
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [ExpandableObjectDisplayMode(IsCategoryVisible = NullableBooleanType.No)]
    public class SliderAppearanceInfo : MiniReactiveObject, IJsonValueConvert, IMPPropObjectValue
    {
        /// <summary>
        /// 对象ID
        /// </summary>
        public static readonly Guid Object_ID = new Guid("{2A2B3C4D-5E6F-7A8B-9C0D-E1F2A3B4C5D8}");

        /// <summary>
        /// 默认值
        /// </summary>
        public static readonly SliderAppearanceInfo Default = new SliderAppearanceInfo();

        private int _opacity = 0;

        /// <summary>
        /// 透明度
        /// </summary>
        [DisplayName("透明度")]
        [Category("外观")]
        [Range(0, 100)]
        public int Opacity
        {
            get => _opacity;
            set => SetProperty(ref _opacity, Math.Max(0, Math.Min(100, value)));
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
                    throw new Exception("对象值不是SliderAppearanceInfo转换的");
                if ((baseValue.Val as ObjectValue_Json).Object_ID != Object_ID)
                    throw new Exception("对象值不是SliderAppearanceInfo转换的");
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
            if (rootElement.TryGetProperty("Opacity", out value))
                _opacity = value.GetInt32();
        }

        string IJsonValueConvert.ToJsonDataObject()
        {
            var value = new
            {
                Opacity = _opacity
            };
            return System.Text.Json.JsonSerializer.Serialize(value);
        }

        #endregion
    }
}
