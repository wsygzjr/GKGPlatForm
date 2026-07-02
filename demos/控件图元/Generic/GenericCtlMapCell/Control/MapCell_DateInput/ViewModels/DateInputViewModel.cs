using System;
using System.ComponentModel;
using GKG.Map.MapCell.Generic.Control.MapCell_Calendar;
using GKG.Map.MapCell.Generic.Control.MapCell_DateInput.Objects;

namespace GKG.Map.MapCell.Generic.Control.MapCell_DateInput.ViewModels
{
    public class DateInputViewModel : PropertyModels.ComponentModel.ReactiveObject, IDisposable
    {
        private readonly DateInputPropertyModelEdit _propertyModel;
        private bool _disposed;

        public DateInputPropertyModelEdit Model => _propertyModel;

        public DateInputCommonInfo CommonInfo => _propertyModel.CommonInfo;

        public DateTime? SelectedDateValue
        {
            get => CalendarValueHelpers.TryParseDate(CommonInfo.SelectedDate, out DateTime date) ? date : null;
            set => CommonInfo.SelectedDate = CalendarValueHelpers.FormatDate(value);
        }

        public DateInputViewModel()
            : this(new DateInputPropertyModelEdit())
        {
        }

        public DateInputViewModel(DateInputPropertyModelEdit propertyModel)
        {
            _propertyModel = propertyModel;
            _propertyModel.PropertyChanged += PropertyModel_PropertyChanged;
            _propertyModel.CommonInfo.PropertyChanged += CommonInfo_PropertyChanged;
        }

        public void ReloadFromModel()
        {
            RaisePropertyChanged(nameof(CommonInfo));
            RaisePropertyChanged(nameof(SelectedDateValue));
        }

        private void PropertyModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
        }

        private void CommonInfo_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(CommonInfo));
            if (e.PropertyName == nameof(DateInputCommonInfo.SelectedDate))
                RaisePropertyChanged(nameof(SelectedDateValue));
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _propertyModel.PropertyChanged -= PropertyModel_PropertyChanged;
            _propertyModel.CommonInfo.PropertyChanged -= CommonInfo_PropertyChanged;
            _disposed = true;
        }
    }
}
