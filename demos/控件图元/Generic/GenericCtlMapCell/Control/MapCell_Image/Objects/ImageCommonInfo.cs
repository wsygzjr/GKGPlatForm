using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using GF_Gereric;
using Griffins;
using GKG.Map.MapCell.Generic.Control.Lable;
using PropertyModels.ComponentModel;

namespace GKG.Map.MapCell.Generic.Control.MapCell_Image.Objects
{
    /// <summary>
    /// 图片拉伸模式枚举
    /// </summary>
    public enum ImageStretchMode
    {
        [Description("无")] None = 0,
        [Description("填充")] Fill = 1,
        [Description("等比缩放")] Uniform = 2,
        [Description("等比填充")] UniformToFill = 3
    }

    /// <summary>
    /// 图片图元公共信息对象
    /// 包含图片设置、填充方式、是否启用、提示文本
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [ExpandableObjectDisplayMode(IsCategoryVisible = NullableBooleanType.No)]
    public class ImageCommonInfo : MiniReactiveObject, IJsonValueConvert, IMPPropObjectValue
    {
        #region 静态字段

        public static readonly ImageCommonInfo Default = new ImageCommonInfo();
        public static readonly Guid Object_ID = new Guid("{C2B3D4E5-1004-4001-9001-000000000004}");

        #endregion

        #region 私有字段

        private BitmapData _imageSource = new BitmapData();
        private ImageStretchMode _stretchMode;
        private bool _isEnabled;
        private string _toolTip;

        #endregion

        #region 构造函数

        public ImageCommonInfo() : this(new BitmapData(), ImageStretchMode.Uniform, true, "")
        {
        }

        public ImageCommonInfo(BitmapData imageSource, ImageStretchMode stretchMode, bool isEnabled, string toolTip)
        {
            _imageSource = imageSource ?? new BitmapData();
            StretchMode = stretchMode;
            IsEnabled = isEnabled;
            ToolTip = toolTip;
        }

        #endregion

        #region 属性

        /// <summary>
        /// 图片源（使用BitmapData类型，支持图片选择器）
        /// </summary>
        [DisplayName("图片")]
        [PropertySortOrder(1)]
        public BitmapData ImageSource
        {
            get => _imageSource;
            set => SetProperty(ref _imageSource, value ?? new BitmapData(), nameof(ImageSource));
        }

        [DisplayName("填充方式")]
        [PropertySortOrder(2)]
        public ImageStretchMode StretchMode
        {
            get => _stretchMode;
            set => SetProperty(ref _stretchMode, value, nameof(StretchMode));
        }

        [DisplayName("是否启用")]
        [PropertySortOrder(3)]
        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetProperty(ref _isEnabled, value, nameof(IsEnabled));
        }

        [DisplayName("提示文本")]
        [PropertySortOrder(4)]
        public string ToolTip
        {
            get => _toolTip;
            set => SetProperty(ref _toolTip, value ?? "", nameof(ToolTip));
        }

        #endregion

        #region IMPPropObjectValue 实现

        bool IMPPropObjectValue.IsObject_Byte => true;

        Guid IMPPropObjectValue.GetObject_ID() => Object_ID;

        GriffinsBaseValue IMPPropObjectValue.ToBaseValue()
        {
            var payload = ToBinaryPayload();
            var objectValueBytes = CreateObjectValueBytes(Object_ID, payload);
            if (objectValueBytes == null)
                throw new InvalidOperationException("ObjectValue_Bytes 构造失败，无法序列化 ImageCommonInfo。");

            return GriffinsBaseValue.Create(objectValueBytes);
        }

        void IMPPropObjectValue.PopulateFromBaseValue(GriffinsBaseValue baseValue)
        {
            var bytesVal = TryGetBytesFromBaseValue(baseValue, Object_ID);
            if (bytesVal != null)
            {
                FromBinaryPayload(bytesVal);
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

            string stretchStr = root.TryGetProperty("StretchMode", out val) ? val.GetString() : "Uniform";
            StretchMode = Enum.TryParse<ImageStretchMode>(stretchStr, out var result) ? result : ImageStretchMode.Uniform;

            IsEnabled = root.TryGetProperty("IsEnabled", out val) ? val.GetBoolean() : true;
            ToolTip = root.TryGetProperty("ToolTip", out val) ? val.GetString() ?? "" : "";

            if (root.TryGetProperty("ImageSourceBase64", out val))
            {
                try
                {
                    var base64 = val.GetString();
                    if (!string.IsNullOrWhiteSpace(base64))
                    {
                        var bytes = System.Convert.FromBase64String(base64);
                        _imageSource ??= new BitmapData();
                        _imageSource.FromBytes(bytes);
                    }
                    else
                    {
                        _imageSource = new BitmapData();
                    }
                }
                catch
                {
                    _imageSource = new BitmapData();
                }
            }
        }

        string IJsonValueConvert.ToJsonDataObject()
        {
            string imageSourceBase64 = "";
            try
            {
                if (_imageSource != null)
                {
                    var bytes = _imageSource.ToBytes();
                    if (bytes != null && bytes.Length > 0)
                    {
                        imageSourceBase64 = System.Convert.ToBase64String(bytes);
                    }
                }
            }
            catch
            {
                imageSourceBase64 = "";
            }

            var obj = new
            {
                StretchMode = StretchMode.ToString(),
                IsEnabled,
                ToolTip = ToolTip ?? "",
                ImageSourceBase64 = imageSourceBase64
            };
            return JsonSerializer.Serialize(obj);
        }

        #endregion

        #region 公共方法

        public void CopyFrom(ImageCommonInfo source)
        {
            if (source == null) return;
            // 图片数据必须按字节独立复制，避免和源对象共用同一个可变 BitmapData 实例。
            ImageSource = CloneBitmapData(source.ImageSource);
            StretchMode = source.StretchMode;
            IsEnabled = source.IsEnabled;
            ToolTip = source.ToolTip;
        }

        private static BitmapData CloneBitmapData(BitmapData source)
        {
            if (source == null)
                return new BitmapData();

            try
            {
                var bytes = source.ToBytes();
                if (bytes == null || bytes.Length == 0)
                    return new BitmapData();

                var clone = new BitmapData();
                clone.FromBytes(bytes);
                return clone;
            }
            catch
            {
                return new BitmapData();
            }
        }

        private const int BinaryVersion = 1;

        private byte[] ToBinaryPayload()
        {
            try
            {
                using var ms = new MemoryStream();
                using var bw = new BinaryWriter(ms, Encoding.UTF8, leaveOpen: true);

                bw.Write(BinaryVersion);
                bw.Write((int)StretchMode);
                bw.Write(IsEnabled);
                bw.Write(ToolTip ?? string.Empty);

                byte[] imgBytes = null;
                try
                {
                    imgBytes = _imageSource?.ToBytes();
                }
                catch
                {
                    imgBytes = null;
                }

                if (imgBytes == null)
                {
                    bw.Write(0);
                }
                else
                {
                    bw.Write(imgBytes.Length);
                    bw.Write(imgBytes);
                }

                bw.Flush();
                return ms.ToArray();
            }
            catch
            {
                return Array.Empty<byte>();
            }
        }

        private void FromBinaryPayload(byte[] payload)
        {
            try
            {
                if (payload == null || payload.Length == 0)
                    return;

                using var ms = new MemoryStream(payload);
                using var br = new BinaryReader(ms, Encoding.UTF8, leaveOpen: true);

                var ver = br.ReadInt32();
                if (ver != BinaryVersion)
                    return;

                StretchMode = (ImageStretchMode)br.ReadInt32();
                IsEnabled = br.ReadBoolean();
                ToolTip = br.ReadString();

                var len = br.ReadInt32();
                if (len <= 0)
                {
                    _imageSource = new BitmapData();
                    return;
                }

                var bytes = br.ReadBytes(len);
                _imageSource ??= new BitmapData();
                try
                {
                    _imageSource.FromBytes(bytes);
                }
                catch
                {
                    _imageSource = new BitmapData();
                }
            }
            catch
            {
            }
        }

        private static ObjectValue_Bytes CreateObjectValueBytes(Guid objectId, byte[] payload)
        {
            try
            {
                var t = typeof(ObjectValue_Bytes);

                object inst = null;
                var ctors = t.GetConstructors();
                foreach (var c in ctors)
                {
                    var ps = c.GetParameters();
                    if (ps.Length == 1 && ps[0].ParameterType == typeof(Guid))
                    {
                        inst = c.Invoke(new object[] { objectId });
                        break;
                    }
                    if (ps.Length == 2 && ps[0].ParameterType == typeof(Guid) && ps[1].ParameterType == typeof(byte[]))
                    {
                        inst = c.Invoke(new object[] { objectId, payload ?? Array.Empty<byte>() });
                        break;
                    }
                }

                inst ??= Activator.CreateInstance(t);
                if (inst == null)
                    return null;

                if (payload == null)
                    payload = Array.Empty<byte>();

                var byteProp = t.GetProperties().FirstOrDefault(p => p.PropertyType == typeof(byte[]) && p.CanWrite);
                if (byteProp != null)
                {
                    byteProp.SetValue(inst, payload);
                }
                else
                {
                    var byteField = t.GetFields().FirstOrDefault(f => f.FieldType == typeof(byte[]));
                    if (byteField != null)
                        byteField.SetValue(inst, payload);
                }

                var idProp = t.GetProperties().FirstOrDefault(p => p.PropertyType == typeof(Guid) && p.CanWrite
                    && (string.Equals(p.Name, "Object_ID", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(p.Name, "ObjectId", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(p.Name, "ID", StringComparison.OrdinalIgnoreCase)));
                if (idProp != null)
                {
                    idProp.SetValue(inst, objectId);
                }
                else
                {
                    var idField = t.GetFields().FirstOrDefault(f => f.FieldType == typeof(Guid)
                        && (string.Equals(f.Name, "Object_ID", StringComparison.OrdinalIgnoreCase)
                            || string.Equals(f.Name, "ObjectId", StringComparison.OrdinalIgnoreCase)
                            || string.Equals(f.Name, "ID", StringComparison.OrdinalIgnoreCase)));
                    if (idField != null)
                        idField.SetValue(inst, objectId);
                }

                return (ObjectValue_Bytes)inst;
            }
            catch
            {
                return null;
            }
        }

        private static byte[] TryGetBytesFromBaseValue(GriffinsBaseValue baseValue, Guid expectedObjectId)
        {
            try
            {
                if (baseValue?.Val == null)
                    return null;

                var val = baseValue.Val;
                var t = val.GetType();
                if (!string.Equals(t.Name, "ObjectValue_Bytes", StringComparison.Ordinal))
                    return null;

                var idProp = t.GetProperties().FirstOrDefault(p => p.PropertyType == typeof(Guid) && p.CanRead);
                if (idProp != null)
                {
                    var id = (Guid)idProp.GetValue(val);
                    if (id != expectedObjectId)
                        return null;
                }

                var byteProp = t.GetProperties().FirstOrDefault(p => p.PropertyType == typeof(byte[]) && p.CanRead);
                if (byteProp != null)
                    return (byte[])byteProp.GetValue(val);

                var byteField = t.GetFields().FirstOrDefault(f => f.FieldType == typeof(byte[]));
                if (byteField != null)
                    return (byte[])byteField.GetValue(val);

                return null;
            }
            catch
            {
                return null;
            }
        }

        #endregion

        public override string ToString()
        {
            if (_imageSource?.Bitmap != null)
                return $"[图片] {StretchMode}";
            return "无图片";
        }
    }
}
