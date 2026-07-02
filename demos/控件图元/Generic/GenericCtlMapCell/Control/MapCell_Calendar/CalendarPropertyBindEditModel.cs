using System;
using System.ComponentModel;
using Griffins;
using PropertyModels.ComponentModel;

namespace GKG.Map.MapCell.Generic.Control.MapCell_Calendar
{
    [Serializable]
    [MapPropertyOrder]
    [CategoryPriority("点位信息", 1)]
    [CategoryPriority("绑定信息", 2)]
    public class CalendarPropertyBindEditModel : ControlCellPropertyBindEditModel
    {
        private PropertyBindInfo _selectedDate = CommHelper.CreatePropertyBindInfo(GriffinsBaseDataType.String, Guid.Empty);
        private PropertyBindInfo _blackoutDates = CommHelper.CreatePropertyBindInfo(GriffinsBaseDataType.String, Guid.Empty);

        [DisplayName("SelectedDate")]
        [Category("绑定信息")]
        [PropertySortOrder(1)]
        [BindMPPropertyID]
        public PropertyBindInfo SelectedDate
        {
            get => _selectedDate;
            set => SetProperty(ref _selectedDate, CommHelper.ClonePropertyBindInfo(value, GriffinsBaseDataType.String, Guid.Empty));
        }

        [DisplayName("BlackoutDates")]
        [Category("绑定信息")]
        [PropertySortOrder(2)]
        [BindMPPropertyID]
        public PropertyBindInfo BlackoutDates
        {
            get => _blackoutDates;
            set => SetProperty(ref _blackoutDates, CommHelper.ClonePropertyBindInfo(value, GriffinsBaseDataType.String, Guid.Empty));
        }

        public void CopyFrom(CalendarPropertyBindEditModel? source)
        {
            if (source == null)
                return;

            base.CopyFrom(source);
            SelectedDate = source.SelectedDate;
            BlackoutDates = source.BlackoutDates;
        }
    }
}
