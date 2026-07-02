using System;
using System.ComponentModel;
using Griffins;
using PropertyModels.ComponentModel;

namespace GKG.Map.MapCell.Generic.Control.MapCell_DateInput
{
    [Serializable]
    [MapPropertyOrder]
    [CategoryPriority("点位信息", 1)]
    [CategoryPriority("绑定信息", 2)]
    public class DateInputPropertyBindEditModel : ControlCellPropertyBindEditModel
    {
        private PropertyBindInfo _selectedDate = CommHelper.CreatePropertyBindInfo(GriffinsBaseDataType.String, Guid.Empty);

        [DisplayName("选中日期")]
        [Category("绑定信息")]
        [PropertySortOrder(1)]
        [BindMPPropertyID]
        public PropertyBindInfo SelectedDate
        {
            get => _selectedDate;
            set => SetProperty(ref _selectedDate, CommHelper.ClonePropertyBindInfo(value, GriffinsBaseDataType.String, Guid.Empty));
        }

        public void CopyFrom(DateInputPropertyBindEditModel source)
        {
            if (source == null)
                return;

            base.CopyFrom(source);
            SelectedDate = source.SelectedDate;
        }
    }
}
