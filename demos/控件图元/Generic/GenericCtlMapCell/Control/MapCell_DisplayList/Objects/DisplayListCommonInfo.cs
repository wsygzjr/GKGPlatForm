using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text.Json;
using GF_Gereric;
using Griffins;
using PropertyModels.ComponentModel;

namespace GKG.Map.MapCell.Generic.Control.MapCell_DisplayList.Objects
{
    public enum DisplayListSortDirection
    {
        [Description("升序")]
        Asc = 0,
        [Description("降序")]
        Desc = 1,
    }

    public static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());
            if (fieldInfo == null)
                return value.ToString();
            
            var attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : value.ToString();
        }
        
        public static T GetEnumByDescription<T>(string description) where T : Enum
        {
            foreach (var field in typeof(T).GetFields())
            {
                if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
                {
                    if (attribute.Description == description)
                        return (T)field.GetValue(null);
                }
                
                if (field.Name == description)
                    return (T)field.GetValue(null);
            }
            
            throw new ArgumentException($"No enum value with description '{description}' found.", nameof(description));
        }
    }

    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [ExpandableObjectDisplayMode(IsCategoryVisible = NullableBooleanType.No)]
    public class DisplayListCommonInfo : MiniReactiveObject, IJsonValueConvert, IMPPropObjectValue
    {
        public static readonly Guid Object_ID = new Guid("{4F1E2A48-0B9E-4C5A-8B2E-5FA4E1DD0A13}");
        public static readonly DisplayListCommonInfo Default = new DisplayListCommonInfo();

        private ObservableCollection<DisplayListColumnInfo> _columns = new ObservableCollection<DisplayListColumnInfo>();
        private bool _enableSelectAll = true;
        private string _sortField = string.Empty;
        private DisplayListSortDirection _sortDirection = DisplayListSortDirection.Asc;

        public DisplayListCommonInfo()
        {
            _columns.CollectionChanged += Columns_CollectionChanged;

            if (_columns.Count == 0)
            {
                _columns.Add(new DisplayListColumnInfo { FieldID = "Name", DisplayName = "姓名" });
                _columns.Add(new DisplayListColumnInfo { FieldID = "Age", DisplayName = "年龄" });
                _columns.Add(new DisplayListColumnInfo { FieldID = "Gender", DisplayName = "性别" });
            }
        }

        private void Columns_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(Columns));
        }

        [DisplayName("设置每列绑定的字段及属性")]
        [Category("公共")]
        public ObservableCollection<DisplayListColumnInfo> Columns
        {
            get => _columns;
            set
            {
                var newCols = value ?? new ObservableCollection<DisplayListColumnInfo>();
                if (ReferenceEquals(_columns, newCols))
                    return;
                _columns.CollectionChanged -= Columns_CollectionChanged;
                if (SetProperty(ref _columns, newCols))
                {
                    _columns.CollectionChanged += Columns_CollectionChanged;
                    RaisePropertyChanged(nameof(Columns));
                }
            }
        }

        [DisplayName("是否支持全选")]
        [Category("公共")]
        public bool EnableSelectAll
        {
            get => _enableSelectAll;
            set => SetProperty(ref _enableSelectAll, value);
        }

        [DisplayName("排序字段")]
        [Category("公共")]
        public string SortField
        {
            get => _sortField;
            set => SetProperty(ref _sortField, value ?? string.Empty);
        }

        [DisplayName("排序类型")]
        [Category("公共")]
        public DisplayListSortDirection SortDirection
        {
            get => _sortDirection;
            set => SetProperty(ref _sortDirection, value);
        }

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
                if (baseValue.Val is not ObjectValue_Json ov)
                    throw new Exception("对象值不是DisplayListCommonInfo转换的");
                if (ov.Object_ID != Object_ID)
                    throw new Exception("对象值不是DisplayListCommonInfo转换的");
                ((IJsonValueConvert)this).FromJsonDataObject(ov.JsonVal);
            }
        }

        void IJsonValueConvert.FromJsonDataObject(string jsonDataObject)
        {
            if (string.IsNullOrEmpty(jsonDataObject))
                throw new ArgumentNullException(nameof(jsonDataObject));

            using JsonDocument jsonDocument = JsonDocument.Parse(jsonDataObject);
            JsonElement root = jsonDocument.RootElement;

            if (root.TryGetProperty("Columns", out var cols) && cols.ValueKind == JsonValueKind.Array)
            {
                var list = new ObservableCollection<DisplayListColumnInfo>();
                foreach (var el in cols.EnumerateArray())
                {
                    if (el.ValueKind == JsonValueKind.Object)
                    {
                        var c = new DisplayListColumnInfo();
                        ((IJsonValueConvert)c).FromJsonDataObject(el.GetRawText());
                        list.Add(c);
                    }
                }
                _columns.CollectionChanged -= Columns_CollectionChanged;
                _columns = list;
                _columns.CollectionChanged += Columns_CollectionChanged;
                RaisePropertyChanged(nameof(Columns));
            }

            if (root.TryGetProperty("EnableSelectAll", out var sa) && (sa.ValueKind == JsonValueKind.True || sa.ValueKind == JsonValueKind.False))
                _enableSelectAll = sa.GetBoolean();
            if (root.TryGetProperty("SortField", out var sf) && sf.ValueKind == JsonValueKind.String)
                _sortField = sf.GetString() ?? string.Empty;
            if (root.TryGetProperty("SortDirection", out var sd))
            {
                if (sd.ValueKind == JsonValueKind.String)
                {
                    var sortDirStr = sd.GetString();
                    if (!string.IsNullOrEmpty(sortDirStr))
                    {
                        try
                        {
                            _sortDirection = EnumExtensions.GetEnumByDescription<DisplayListSortDirection>(sortDirStr);
                        }
                        catch { }
                    }
                }
                else if (sd.ValueKind == JsonValueKind.Number && sd.TryGetInt32(out var i) && Enum.IsDefined(typeof(DisplayListSortDirection), i))
                    _sortDirection = (DisplayListSortDirection)i;
            }

            RaisePropertyChanged(nameof(EnableSelectAll));
            RaisePropertyChanged(nameof(SortField));
            RaisePropertyChanged(nameof(SortDirection));
        }

        string IJsonValueConvert.ToJsonDataObject()
        {
            var cols = new object[_columns.Count];
            for (var i = 0; i < _columns.Count; i++)
            {
                cols[i] = new
                {
                    FieldID = _columns[i].FieldID,
                    DisplayName = _columns[i].DisplayName,
                };
            }

            var value = new
            {
                Columns = cols,
                EnableSelectAll = _enableSelectAll,
                SortField = _sortField,
                SortDirection = _sortDirection.GetDescription(),
            };
            return JsonSerializer.Serialize(value);
        }
    }
}
