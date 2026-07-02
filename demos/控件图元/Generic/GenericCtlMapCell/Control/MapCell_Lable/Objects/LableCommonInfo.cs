using System;
using System.ComponentModel;
using System.Text.Json;
using GF_Gereric;
using Griffins;
using PropertyModels.ComponentModel;

namespace GKG.Map.MapCell.Generic.Lable
{
    /// <summary>
    /// 标签公共信息对象
    /// 包含文字、鼠标样式、是否启用、提示文字设置
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [ExpandableObjectDisplayMode(IsCategoryVisible = NullableBooleanType.No)]
    public class LableCommonInfo : MiniReactiveObject, IJsonValueConvert, IMPPropObjectValue
    {
        #region 静态字段

        /// <summary>
        /// 默认公共信息
        /// </summary>
        public static readonly LableCommonInfo Default = new LableCommonInfo("", CursorType.Arrow, true, "");

        /// <summary>
        /// 对象唯一标识符
        /// </summary>
        public static readonly Guid Object_ID = new Guid("{A1B2C3D4-6666-4AC8-BF66-281412CDE006}");

        #endregion

        #region 私有字段

        private string _lableValue;
        private CursorType _cursorType;
        private bool _isEnabled;
        private string _toolTip;

        #endregion

        #region 构造函数

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public LableCommonInfo()
            : this("", CursorType.Arrow, true, "")
        {
        }

        /// <summary>
        /// 带参数构造函数
        /// </summary>
        public LableCommonInfo(string lableValue, CursorType cursorType, bool isEnabled, string toolTip)
        {
            LableValue = lableValue;
            CursorType = cursorType;
            IsEnabled = isEnabled;
            ToolTip = toolTip;
        }

        #endregion

        #region 属性

        /// <summary>
        /// 文字
        /// </summary>
        [DisplayName("文字")]
        public string LableValue
        {
            get { return _lableValue; }
            set { SetProperty(ref _lableValue, value, nameof(LableValue)); }
        }

        /// <summary>
        /// 鼠标样式
        /// </summary>
        [DisplayName("鼠标样式")]
        public CursorType CursorType
        {
            get { return _cursorType; }
            set { SetProperty(ref _cursorType, value, nameof(CursorType)); }
        }

        /// <summary>
        /// 是否启用
        /// </summary>
        [DisplayName("是否启用")]
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { SetProperty(ref _isEnabled, value, nameof(IsEnabled)); }
        }

        /// <summary>
        /// 提示文字
        /// </summary>
        [DisplayName("提示文字")]
        public string ToolTip
        {
            get { return _toolTip; }
            set { SetProperty(ref _toolTip, value, nameof(ToolTip)); }
        }

        #endregion

        #region IMPPropObjectValue 实现

        bool IMPPropObjectValue.IsObject_Byte => false;

        Guid IMPPropObjectValue.GetObject_ID()
        {
            return Object_ID;
        }

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
                {
                    throw new Exception("对象值不是LableCommonInfo转换的");
                }

                if ((baseValue.Val as ObjectValue_Json).Object_ID != Object_ID)
                {
                    throw new Exception("对象值不是LableCommonInfo转换的");
                }

                ((IJsonValueConvert)this).FromJsonDataObject((baseValue.Val as ObjectValue_Json).JsonVal);
            }
        }

        #endregion

        #region IJsonValueConvert 实现

        void IJsonValueConvert.FromJsonDataObject(string jsonDataObject)
        {
            if (string.IsNullOrEmpty(jsonDataObject))
            {
                throw new ArgumentNullException(nameof(jsonDataObject));
            }

            using JsonDocument jsonDocument = JsonDocument.Parse(jsonDataObject);
            JsonElement rootElement = jsonDocument.RootElement;

            JsonElement value;
            LableValue = rootElement.TryGetProperty("LableValue", out value) ? value.GetString() : "";
            
            string cursorStr = rootElement.TryGetProperty("CursorType", out value) ? value.GetString() : "Arrow";
            CursorType = Enum.TryParse<CursorType>(cursorStr, out var result) ? result : CursorType.Arrow;
            
            IsEnabled = rootElement.TryGetProperty("IsEnabled", out value) ? value.GetBoolean() : true;
            ToolTip = rootElement.TryGetProperty("ToolTip", out value) ? value.GetString() : "";
        }

        string IJsonValueConvert.ToJsonDataObject()
        {
            var value = new
            {
                LableValue = LableValue ?? "",
                CursorType = CursorType.ToString(),
                IsEnabled = IsEnabled,
                ToolTip = ToolTip ?? ""
            };
            return System.Text.Json.JsonSerializer.Serialize(value);
        }

        #endregion

        #region 重写方法

        public override string ToString()
        {
            string text = string.IsNullOrEmpty(LableValue) ? "(空)" : 
                (LableValue.Length > 10 ? LableValue.Substring(0, 10) + "..." : LableValue);
            return text;
        }

        #endregion
    }
}
