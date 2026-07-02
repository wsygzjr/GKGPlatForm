using System;
using System.ComponentModel;
using System.Text.Json;
using GF_Gereric;
using Griffins;
using PropertyModels.ComponentModel;

namespace GKG.Map.MapCell.Generic.Control.MapCell_DisplayList.Objects
{
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [ExpandableObjectDisplayMode(IsCategoryVisible = NullableBooleanType.No)]
    public class DisplayListColumnInfo : MiniReactiveObject, IJsonValueConvert
    {
        private string _fieldID = string.Empty;
        private string _displayName = string.Empty;

        [DisplayName("每列绑定的字段ID")]
        [Category("公共")]
        public string FieldID
        {
            get => _fieldID;
            set => SetProperty(ref _fieldID, value ?? string.Empty);
        }

        [DisplayName("每列的显示名称")]
        [Category("公共")]
        public string DisplayName
        {
            get => _displayName;
            set => SetProperty(ref _displayName, value ?? string.Empty);
        }

        void IJsonValueConvert.FromJsonDataObject(string jsonDataObject)
        {
            if (string.IsNullOrEmpty(jsonDataObject))
                throw new ArgumentNullException(nameof(jsonDataObject));

            using JsonDocument jsonDocument = JsonDocument.Parse(jsonDataObject);
            JsonElement root = jsonDocument.RootElement;

            if (root.TryGetProperty("FieldID", out var f) && f.ValueKind == JsonValueKind.String)
                _fieldID = f.GetString() ?? string.Empty;
            if (root.TryGetProperty("DisplayName", out var d) && d.ValueKind == JsonValueKind.String)
                _displayName = d.GetString() ?? string.Empty;
        }

        string IJsonValueConvert.ToJsonDataObject()
        {
            var value = new
            {
                FieldID = _fieldID,
                DisplayName = _displayName,
            };
            return JsonSerializer.Serialize(value);
        }

        public override string ToString() => $"{_displayName}({_fieldID})";
    }
}
