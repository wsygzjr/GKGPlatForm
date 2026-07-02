using System;
using System.ComponentModel;
using GF_Gereric;
using GKG.Map.MapCell.Generic.Control.MapCell_DateInput.Objects;
using Griffins;
using PropertyModels.ComponentModel;

namespace GKG.Map.MapCell.Generic.Control.MapCell_DateInput
{
    [Serializable]
    [MapPropertyOrder]
    [CategoryPriority("图元信息", 1)]
    [CategoryPriority("公共", 2)]
    public class DateInputPropertyModelEdit : ControlCellPropertyModelEdit
    {
        private DateInputCommonInfo _commonInfo = new();

        [DisplayName("公共设置")]
        [Category("公共")]
        [PropertySortOrder(1)]
        public DateInputCommonInfo CommonInfo
        {
            get => _commonInfo;
            set => SetProperty(ref _commonInfo, value);
        }

        public override bool SetPropertyValue(string propertyID, MapCellPropValue propertyVal)
        {
            if (string.Equals(propertyID, nameof(DateInputCommonInfo.SelectedDate), StringComparison.Ordinal))
            {
                _commonInfo ??= new DateInputCommonInfo();
                _commonInfo.SelectedDate = propertyVal?.ToPrimitiveValue<string>() ?? string.Empty;
                RaisePropertyChanged(nameof(CommonInfo));
                return true;
            }

            if (string.Equals(propertyID, nameof(CommonInfo), StringComparison.Ordinal))
            {
                DateInputCommonInfo src = propertyVal != null ? DeserializeObject<DateInputCommonInfo>(propertyVal) : new DateInputCommonInfo();
                _commonInfo ??= new DateInputCommonInfo();
                _commonInfo.SelectedDate = src.SelectedDate;
                RaisePropertyChanged(nameof(CommonInfo));
                return true;
            }

            return base.SetPropertyValue(propertyID, propertyVal);
        }

        public void CopyFrom(DateInputPropertyModelEdit source)
        {
            if (source == null)
                return;

            base.CopyFrom(source);
            CommonInfo.SelectedDate = source.CommonInfo?.SelectedDate ?? DateInputCommonInfo.Default.SelectedDate;
            RaisePropertyChanged(nameof(CommonInfo));
        }

        private static T DeserializeObject<T>(MapCellPropValue val) where T : IMPPropObjectValue, new()
        {
            ObjectValue_Json objectValueJson = val.ToObjectValue_Json();
            GriffinsBaseValue griffinsBaseValue = GriffinsBaseValue.Create(objectValueJson);
            IMPPropObjectValue obj = new T();
            obj.PopulateFromBaseValue(griffinsBaseValue);
            return (T)obj;
        }
    }
}
