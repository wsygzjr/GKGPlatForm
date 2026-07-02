using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using GF_Gereric;
using Griffins;
using Griffins.UI2;
using PropertyModels.ComponentModel;

namespace GKG.Map.DeviceStatusFuncCtlMapCell.MapCell_DeviceStatus.Objects
{
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [ExpandableObjectDisplayMode(IsCategoryVisible = NullableBooleanType.No)]
    public class DeviceStatusCommonInfo : MiniReactiveObject, IJsonValueConvert, IGriffinsBaseValue
    {
        public static readonly DeviceStatusCommonInfo Default = new DeviceStatusCommonInfo();
        public static readonly Guid Object_ID = new Guid("{3B56E2F1-8888-4444-9999-555555555555}");

        private List<BitmapData> _imageSources = new List<BitmapData>();
        private int _currentIndex;
        private string _statusName;
        private string _deviceStatusValue;
        private string _deviceStatusUnit;

        public DeviceStatusCommonInfo() : this(new List<BitmapData>(), 0, "状态名称", "设备状态", "")
        {
        }

        public DeviceStatusCommonInfo(List<BitmapData> imageSources, int currentIndex, string statusName, string deviceStatusValue, string deviceStatusUnit)
        {
            _imageSources = imageSources ?? new List<BitmapData>();
            CurrentIndex = currentIndex;
            StatusName = statusName;
            DeviceStatusValue = deviceStatusValue;
            DeviceStatusUnit = deviceStatusUnit;
        }

        [DisplayName("图片列表")]
        [PropertySortOrder(1)]
        public List<BitmapData> ImageSources
        {
            get => _imageSources;
            set => SetProperty(ref _imageSources, value ?? new List<BitmapData>(), nameof(ImageSources));
        }

        [Browsable(false)]
        public int CurrentIndex
        {
            get => _currentIndex;
            set
            {
                var newVal = value;
                if (newVal < 0) newVal = 0;
                SetProperty(ref _currentIndex, newVal, nameof(CurrentIndex));
            }
        }

        [Browsable(false)]
        public int ValidCurrentIndex
        {
            get
            {
                if (_imageSources == null || _imageSources.Count == 0) return 0;
                if (_currentIndex < 0) return 0;
                if (_currentIndex >= _imageSources.Count) return _imageSources.Count - 1;
                return _currentIndex;
            }
        }

        [DisplayName("状态名称")]
        [PropertySortOrder(2)]
        public string StatusName
        {
            get => _statusName;
            set => SetProperty(ref _statusName, value ?? "", nameof(StatusName));
        }

        [DisplayName("设备状态")]
        [PropertySortOrder(3)]
        public string DeviceStatusValue
        {
            get => _deviceStatusValue;
            set => SetProperty(ref _deviceStatusValue, value ?? "", nameof(DeviceStatusValue));
        }

        [DisplayName("单位")]
        [PropertySortOrder(4)]
        public string DeviceStatusUnit
        {
            get => _deviceStatusUnit;
            set => SetProperty(ref _deviceStatusUnit, value ?? "", nameof(DeviceStatusUnit));
        }

        bool IGriffinsBaseValue.IsObject_Byte => true;
        Guid IGriffinsBaseValue.GetObject_ID() => Object_ID;

        GriffinsBaseValue IGriffinsBaseValue.ToBaseValue()
        {
            var payload = ToBinaryPayload();
            var objectValueBytes = CreateObjectValueBytes(Object_ID, payload);
            if (objectValueBytes == null) throw new InvalidOperationException("ObjectValue_Bytes 构造失败");
            return GriffinsBaseValue.Create(objectValueBytes);
        }

        void IGriffinsBaseValue.PopulateFromBaseValue(GriffinsBaseValue baseValue)
        {
            var bytesVal = TryGetBytesFromBaseValue(baseValue, Object_ID);
            if (bytesVal != null) FromBinaryPayload(bytesVal);
        }

        void IJsonValueConvert.FromJsonDataObject(string jsonDataObject)
        {
            if (string.IsNullOrEmpty(jsonDataObject)) return;
            using JsonDocument doc = JsonDocument.Parse(jsonDataObject);
            JsonElement root = doc.RootElement;
            JsonElement val;
            CurrentIndex = root.TryGetProperty("CurrentIndex", out val) ? val.GetInt32() : 0;
            StatusName = root.TryGetProperty("StatusName", out val) ? val.GetString() : "";
            DeviceStatusValue = root.TryGetProperty("DeviceStatusValue", out val) ? val.GetString() : "";
            DeviceStatusUnit = root.TryGetProperty("DeviceStatusUnit", out val) ? val.GetString() : "";

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
                            if (bytes != null && bytes.Length > 0) imageSourcesBase64.Add(System.Convert.ToBase64String(bytes));
                            else imageSourcesBase64.Add("");
                        }
                    }
                }
            }
            catch { }

            var obj = new
            {
                CurrentIndex,
                StatusName,
                DeviceStatusValue,
                DeviceStatusUnit,
                ImageSourcesBase64 = imageSourcesBase64
            };
            return JsonSerializer.Serialize(obj);
        }

        public void CopyFrom(DeviceStatusCommonInfo source)
        {
            if (source == null) return;
            _imageSources = source.ImageSources != null ? new List<BitmapData>(source.ImageSources) : new List<BitmapData>();
            CurrentIndex = source.CurrentIndex;
            StatusName = source.StatusName;
            DeviceStatusValue = source.DeviceStatusValue;
            DeviceStatusUnit = source.DeviceStatusUnit;
        }

        private const int BinaryVersion = 2;

        private byte[] ToBinaryPayload()
        {
            try
            {
                using var ms = new MemoryStream();
                using var bw = new BinaryWriter(ms, Encoding.UTF8, leaveOpen: true);
                bw.Write(BinaryVersion);
                bw.Write(CurrentIndex);
                bw.Write(StatusName ?? "");
                bw.Write(DeviceStatusValue ?? "");
                bw.Write(DeviceStatusUnit ?? "");

                var list = _imageSources ?? new List<BitmapData>();
                bw.Write(list.Count);
                for (int i = 0; i < list.Count; i++)
                {
                    var bmp = list[i];
                    byte[] bytes = null;
                    try { bytes = bmp?.ToBytes(); } catch { bytes = null; }
                    if (bytes == null) { bw.Write(0); continue; }
                    bw.Write(bytes.Length);
                    bw.Write(bytes);
                }
                bw.Flush();
                return ms.ToArray();
            }
            catch { return Array.Empty<byte>(); }
        }

        private void FromBinaryPayload(byte[] payload)
        {
            try
            {
                if (payload == null || payload.Length == 0) return;
                using var ms = new MemoryStream(payload);
                using var br = new BinaryReader(ms, Encoding.UTF8, leaveOpen: true);
                var ver = br.ReadInt32();
                if (ver != BinaryVersion) return;
                CurrentIndex = br.ReadInt32();
                StatusName = br.ReadString();
                DeviceStatusValue = br.ReadString();
                DeviceStatusUnit = br.ReadString();

                var cnt = br.ReadInt32();
                if (cnt < 0) cnt = 0;
                var list = new List<BitmapData>(cnt);
                for (int i = 0; i < cnt; i++)
                {
                    var len = br.ReadInt32();
                    if (len <= 0) { list.Add(new BitmapData()); continue; }
                    var bytes = br.ReadBytes(len);
                    var bmp = new BitmapData();
                    try { bmp.FromBytes(bytes); } catch { bmp = new BitmapData(); }
                    list.Add(bmp);
                }
                _imageSources = list;
            }
            catch { }
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
                    if (ps.Length == 1 && ps[0].ParameterType == typeof(Guid)) { inst = c.Invoke(new object[] { objectId }); break; }
                    if (ps.Length == 2 && ps[0].ParameterType == typeof(Guid) && ps[1].ParameterType == typeof(byte[])) { inst = c.Invoke(new object[] { objectId, payload ?? Array.Empty<byte>() }); break; }
                }
                inst ??= Activator.CreateInstance(t);
                if (inst == null) return null;
                if (payload == null) payload = Array.Empty<byte>();

                var byteProp = t.GetProperties().FirstOrDefault(p => p.PropertyType == typeof(byte[]) && p.CanWrite);
                if (byteProp != null) byteProp.SetValue(inst, payload);
                else
                {
                    var byteField = t.GetFields().FirstOrDefault(f => f.FieldType == typeof(byte[]));
                    if (byteField != null) byteField.SetValue(inst, payload);
                }

                var idProp = t.GetProperties().FirstOrDefault(p => p.PropertyType == typeof(Guid) && p.CanWrite && (string.Equals(p.Name, "Object_ID", StringComparison.OrdinalIgnoreCase) || string.Equals(p.Name, "ObjectId", StringComparison.OrdinalIgnoreCase) || string.Equals(p.Name, "ID", StringComparison.OrdinalIgnoreCase)));
                if (idProp != null) idProp.SetValue(inst, objectId);
                else
                {
                    var idField = t.GetFields().FirstOrDefault(f => f.FieldType == typeof(Guid) && (string.Equals(f.Name, "Object_ID", StringComparison.OrdinalIgnoreCase) || string.Equals(f.Name, "ObjectId", StringComparison.OrdinalIgnoreCase) || string.Equals(f.Name, "ID", StringComparison.OrdinalIgnoreCase)));
                    if (idField != null) idField.SetValue(inst, objectId);
                }
                return (ObjectValue_Bytes)inst;
            }
            catch { return null; }
        }

        private static byte[] TryGetBytesFromBaseValue(GriffinsBaseValue baseValue, Guid expectedObjectId)
        {
            try
            {
                if (baseValue?.Val == null) return null;
                var val = baseValue.Val;
                var t = val.GetType();
                if (!string.Equals(t.Name, "ObjectValue_Bytes", StringComparison.Ordinal)) return null;

                var idProp = t.GetProperties().FirstOrDefault(p => p.PropertyType == typeof(Guid) && p.CanRead);
                if (idProp != null) { var id = (Guid)idProp.GetValue(val); if (id != expectedObjectId) return null; }

                var byteProp = t.GetProperties().FirstOrDefault(p => p.PropertyType == typeof(byte[]) && p.CanRead);
                if (byteProp != null) return (byte[])byteProp.GetValue(val);

                var byteField = t.GetFields().FirstOrDefault(f => f.FieldType == typeof(byte[]));
                if (byteField != null) return (byte[])byteField.GetValue(val);
                return null;
            }
            catch { return null; }
        }

        public override string ToString()
        {
            var unit = DeviceStatusUnit ?? "";
            return $"[{_imageSources?.Count ?? 0}张图片] {StatusName}: {DeviceStatusValue}{(string.IsNullOrWhiteSpace(unit) ? "" : " " + unit)}";
        }
    }
}
