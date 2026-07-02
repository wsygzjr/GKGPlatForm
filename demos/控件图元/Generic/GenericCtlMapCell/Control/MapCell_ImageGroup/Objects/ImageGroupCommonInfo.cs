using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using GF_Gereric;
using Griffins;
using GKG.Map.MapCell.Generic.Control.Lable;
using PropertyModels.ComponentModel;

namespace GKG.Map.MapCell.Generic.Control.MapCell_ImageGroup.Objects
{
    /// <summary>
    /// 图片组拉伸模式枚举
    /// </summary>
    public enum ImageGroupStretchMode
    {
        [Description("无")] None = 0,
        [Description("填充")] Fill = 1,
        [Description("等比缩放")] Uniform = 2,
        [Description("等比填充")] UniformToFill = 3
    }

    /// <summary>
    /// 图片组图元公共信息对象
    /// 包含多图片设置、填充方式、当前索引、是否启用、提示文本
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [ExpandableObjectDisplayMode(IsCategoryVisible = NullableBooleanType.No)]
    public class ImageGroupCommonInfo : MiniReactiveObject, IJsonValueConvert, IMPPropObjectValue
    {
        #region 静态字段

        public static readonly ImageGroupCommonInfo Default = new ImageGroupCommonInfo();
        public static readonly Guid Object_ID = new Guid("{D3C4E5F6-2004-4001-9001-000000000004}");

        #endregion

        #region 私有字段

        private List<BitmapData> _imageSources = new List<BitmapData>();
        private int _currentIndex;
        private ImageGroupStretchMode _stretchMode;
        private bool _isEnabled;
        private string _toolTip;

        #endregion

        #region 构造函数

        public ImageGroupCommonInfo() : this(new List<BitmapData>(), 0, ImageGroupStretchMode.Uniform, true, "")
        {
        }

        public ImageGroupCommonInfo(List<BitmapData> imageSources, int currentIndex, ImageGroupStretchMode stretchMode, bool isEnabled, string toolTip)
        {
            _imageSources = imageSources ?? new List<BitmapData>();
            CurrentIndex = currentIndex;
            StretchMode = stretchMode;
            IsEnabled = isEnabled;
            ToolTip = toolTip;
        }

        #endregion

        #region 属性

        /// <summary>
        /// 图片源列表（支持多图片）
        /// </summary>
        [DisplayName("图片列表")]
        [PropertySortOrder(1)]
        public List<BitmapData> ImageSources
        {
            get => _imageSources;
            set => SetProperty(ref _imageSources, value ?? new List<BitmapData>(), nameof(ImageSources));
        }

        /// <summary>
        /// 当前显示的图片索引（运行时可动态修改以切换显示的图片）
        /// </summary>
        [Browsable(false)]
        public int CurrentIndex
        {
            get => _currentIndex;
            set
            {
                var newVal = value;
                if (newVal < 0) newVal = 0;
                // 允许设置超出范围的值，但显示时会自动限制
                SetProperty(ref _currentIndex, newVal, nameof(CurrentIndex));
            }
        }

        /// <summary>
        /// 获取有效的当前索引（确保在图片列表范围内）
        /// </summary>
        [Browsable(false)]
        public int ValidCurrentIndex
        {
            get
            {
                if (_imageSources == null || _imageSources.Count == 0)
                    return 0;
                if (_currentIndex < 0)
                    return 0;
                if (_currentIndex >= _imageSources.Count)
                    return _imageSources.Count - 1;
                return _currentIndex;
            }
        }

        [DisplayName("填充方式")]
        [PropertySortOrder(3)]
        public ImageGroupStretchMode StretchMode
        {
            get => _stretchMode;
            set => SetProperty(ref _stretchMode, value, nameof(StretchMode));
        }

        [DisplayName("是否启用")]
        [PropertySortOrder(4)]
        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetProperty(ref _isEnabled, value, nameof(IsEnabled));
        }

        [DisplayName("提示文本")]
        [PropertySortOrder(5)]
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
                throw new InvalidOperationException("ObjectValue_Bytes 构造失败，无法序列化 ImageGroupCommonInfo。");

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
            StretchMode = Enum.TryParse<ImageGroupStretchMode>(stretchStr, out var result) ? result : ImageGroupStretchMode.Uniform;

            CurrentIndex = root.TryGetProperty("CurrentIndex", out val) ? val.GetInt32() : 0;
            IsEnabled = root.TryGetProperty("IsEnabled", out val) ? val.GetBoolean() : true;
            ToolTip = root.TryGetProperty("ToolTip", out val) ? val.GetString() ?? "" : "";

            // 读取图片列表
            if (root.TryGetProperty("ImageSourcesBase64", out val) && val.ValueKind == JsonValueKind.Array)
            {
                _imageSources = new List<BitmapData>();
                foreach (var item in val.EnumerateArray())
                {
                    try
                    {
                        var base64 = item.GetString();
                        if (!string.IsNullOrWhiteSpace(base64))
                        {
                            var bytes = System.Convert.FromBase64String(base64);
                            var bitmapData = new BitmapData();
                            bitmapData.FromBytes(bytes);
                            _imageSources.Add(bitmapData);
                        }
                    }
                    catch { }
                }
            }
        }

        string IJsonValueConvert.ToJsonDataObject()
        {
            var imageSourcesBase64 = new List<string>();
            try
            {
                if (_imageSources != null)
                {
                    foreach (var bitmapData in _imageSources)
                    {
                        if (bitmapData != null)
                        {
                            var bytes = bitmapData.ToBytes();
                            if (bytes != null && bytes.Length > 0)
                            {
                                imageSourcesBase64.Add(System.Convert.ToBase64String(bytes));
                            }
                            else
                            {
                                imageSourcesBase64.Add("");
                            }
                        }
                    }
                }
            }
            catch { }

            var obj = new
            {
                StretchMode = StretchMode.ToString(),
                CurrentIndex,
                IsEnabled,
                ToolTip = ToolTip ?? "",
                ImageSourcesBase64 = imageSourcesBase64
            };
            return JsonSerializer.Serialize(obj);
        }

        #endregion

        #region 公共方法

        public void CopyFrom(ImageGroupCommonInfo source)
        {
            if (source == null) return;

            _imageSources = source.ImageSources != null ? new List<BitmapData>(source.ImageSources) : new List<BitmapData>();
            CurrentIndex = source.CurrentIndex;
            StretchMode = source.StretchMode;
            IsEnabled = source.IsEnabled;
            ToolTip = source.ToolTip;
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
                bw.Write(CurrentIndex);
                bw.Write(IsEnabled);

                var tip = ToolTip ?? string.Empty;
                bw.Write(tip);

                var list = _imageSources ?? new List<BitmapData>();
                bw.Write(list.Count);
                for (int i = 0; i < list.Count; i++)
                {
                    var bmp = list[i];
                    byte[] bytes = null;
                    try
                    {
                        bytes = bmp?.ToBytes();
                    }
                    catch
                    {
                        bytes = null;
                    }

                    if (bytes == null)
                    {
                        bw.Write(0);
                        continue;
                    }

                    bw.Write(bytes.Length);
                    bw.Write(bytes);
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

                StretchMode = (ImageGroupStretchMode)br.ReadInt32();
                CurrentIndex = br.ReadInt32();
                IsEnabled = br.ReadBoolean();
                ToolTip = br.ReadString();

                var cnt = br.ReadInt32();
                if (cnt < 0) cnt = 0;
                var list = new List<BitmapData>(cnt);
                for (int i = 0; i < cnt; i++)
                {
                    var len = br.ReadInt32();
                    if (len <= 0)
                    {
                        list.Add(new BitmapData());
                        continue;
                    }

                    var bytes = br.ReadBytes(len);
                    var bmp = new BitmapData();
                    try
                    {
                        bmp.FromBytes(bytes);
                    }
                    catch
                    {
                        bmp = new BitmapData();
                    }
                    list.Add(bmp);
                }

                _imageSources = list;
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

                // 确保 Object_ID 被设置（有些实现可能没有 Guid 构造）
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
            int count = _imageSources?.Count ?? 0;
            if (count > 0)
                return $"[{count}张图片] 索引:{CurrentIndex}";
            return "无图片";
        }
    }
}
