using System;
using System.ComponentModel;
using System.Text.Json;
using Avalonia.Media;
using GF_Gereric;
using Griffins;
using Newtonsoft.JsonG;
using PropertyModels.ComponentModel;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace GKG.Map.MapCell.Generic.Lable
{
    /// <summary>
    /// 标签画笔信息对象
    /// 包含背景色和前景色设置
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [ExpandableObjectDisplayMode(IsCategoryVisible = NullableBooleanType.No)]
    public class LableBrushInfo : MiniReactiveObject, IJsonValueConvert, IMPPropObjectValue
    {
        #region 静态字段

        public static readonly LableBrushInfo Default = new LableBrushInfo(
            Color.FromArgb(33, 0, 0, 0),
            Colors.Black
        );

        public static readonly Guid Object_ID = new Guid("{A1B2C3D4-1111-4AC8-BF66-281412CDE001}");

        #endregion

        #region 私有字段

        // 存储颜色字符串用于序列化
        private string _backColorStr = "#21000000";
        private string _foreColorStr = "#FF000000";

        #endregion

        #region 构造函数

        public LableBrushInfo()
            : this(Color.FromArgb(33, 0, 0, 0), Colors.Black)
        {
        }

        public LableBrushInfo(Color backColor, Color foreColor)
        {
            _backColorStr = backColor.ToColorString();
            _foreColorStr = foreColor.ToColorString();
        }

        #endregion

        #region 属性 - 用于 PropertyGrid 显示（不参与序列化）

        [DisplayName("背景色")]
        [JsonIgnore]
        public Color BackColor
        {
            get { return Color.Parse(_backColorStr ?? "#21000000"); }
            set 
            { 
                var newStr = value.ToColorString();
                if (_backColorStr != newStr)
                {
                    _backColorStr = newStr;
                    RaisePropertyChanged(nameof(BackColor));
                    RaisePropertyChanged(nameof(BackColorStr));
                }
            }
        }

        [DisplayName("前景色")]
        [JsonIgnore]
        public Color ForeColor
        {
            get { return Color.Parse(_foreColorStr ?? "#FF000000"); }
            set 
            { 
                var newStr = value.ToColorString();
                if (_foreColorStr != newStr)
                {
                    _foreColorStr = newStr;
                    RaisePropertyChanged(nameof(ForeColor));
                    RaisePropertyChanged(nameof(ForeColorStr));
                }
            }
        }

        #endregion

        #region 属性 - 用于 JSON 序列化

        [Browsable(false)]
        public string BackColorStr
        {
            get { return _backColorStr; }
            set 
            { 
                if (_backColorStr != value)
                {
                    _backColorStr = value ?? "#21000000";
                    RaisePropertyChanged(nameof(BackColorStr));
                    RaisePropertyChanged(nameof(BackColor));
                }
            }
        }

        [Browsable(false)]
        public string ForeColorStr
        {
            get { return _foreColorStr; }
            set 
            { 
                if (_foreColorStr != value)
                {
                    _foreColorStr = value ?? "#FF000000";
                    RaisePropertyChanged(nameof(ForeColorStr));
                    RaisePropertyChanged(nameof(ForeColor));
                }
            }
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
            if (baseValue != null && baseValue.Val != null)
            {
                if (!(baseValue.Val is ObjectValue_Json))
                    throw new Exception("对象值不是LableBrushInfo转换的");
                if ((baseValue.Val as ObjectValue_Json).Object_ID != Object_ID)
                    throw new Exception("对象值不是LableBrushInfo转换的");
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
            BackColorStr = rootElement.TryGetProperty("BackColor", out value) ? value.GetString() : "#21000000";
            ForeColorStr = rootElement.TryGetProperty("ForeColor", out value) ? value.GetString() : "#FF000000";
        }

        string IJsonValueConvert.ToJsonDataObject()
        {
            var value = new
            {
                BackColor = BackColorStr,
                ForeColor = ForeColorStr
            };
            return JsonSerializer.Serialize(value);
        }

        #endregion

        public override string ToString() => $"背景:{BackColor}, 前景:{ForeColor}";
    }
}
