using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using GF_Gereric;
using Griffins;
using PropertyModels.ComponentModel;

namespace GKG.Map.MapCell.Generic.Control.MapCell_ImageGroup.Objects
{
    /// <summary>
    /// 图片组图元外观信息对象
    /// 包含透明度设置
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [ExpandableObjectDisplayMode(IsCategoryVisible = NullableBooleanType.No)]
    public class ImageGroupAppearanceInfo : MiniReactiveObject, IJsonValueConvert, IMPPropObjectValue
    {
        #region 静态字段

        public static readonly ImageGroupAppearanceInfo Default = new ImageGroupAppearanceInfo(1.0);
        public static readonly Guid Object_ID = new Guid("{D3C4E5F6-2002-4001-9001-000000000002}");

        #endregion

        #region 私有字段

        private double _opacity;

        #endregion

        #region 构造函数

        public ImageGroupAppearanceInfo() : this(1.0)
        {
        }

        public ImageGroupAppearanceInfo(double opacity)
        {
            Opacity = opacity;
        }

        #endregion

        #region 属性

        [DisplayName("透明度")]
        [PropertySortOrder(1)]
        [FloatPrecision(2)]
        [Range(0.0, 1.0)]
        public double Opacity
        {
            get => _opacity;
            set => SetProperty(ref _opacity, Math.Max(0, Math.Min(1, value)), nameof(Opacity));
        }

        #endregion

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
            if (baseValue?.Val is ObjectValue_Json json && json.Object_ID == Object_ID)
            {
                ((IJsonValueConvert)this).FromJsonDataObject(json.JsonVal);
            }
        }

        #endregion

        #region IJsonValueConvert 实现

        void IJsonValueConvert.FromJsonDataObject(string jsonDataObject)
        {
            if (string.IsNullOrEmpty(jsonDataObject)) return;

            using JsonDocument doc = JsonDocument.Parse(jsonDataObject);
            JsonElement root = doc.RootElement;
            JsonElement val;

            Opacity = root.TryGetProperty("Opacity", out val) ? val.GetDouble() : 1.0;
        }

        string IJsonValueConvert.ToJsonDataObject()
        {
            var obj = new { Opacity };
            return JsonSerializer.Serialize(obj);
        }

        #endregion

        #region 公共方法

        public void CopyFrom(ImageGroupAppearanceInfo source)
        {
            if (source == null) return;
            Opacity = source.Opacity;
        }

        #endregion

        public override string ToString() => $"透明度:{Opacity:F2}";
    }
}
