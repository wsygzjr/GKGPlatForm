using System;
using System.ComponentModel;
using System.Reactive;
using Avalonia;
using ReactiveUI;

namespace GKG.Map.MapCell.Generic.Link
{
    public class LinkViewModel : PropertyModels.ComponentModel.ReactiveObject, IDisposable
    {
        private readonly LinkPropertyModelEdit _propertyModel;
        private readonly Action<Point> _clickAction;
        private bool _disposed;

        public LinkBrushInfo BrushInfo => _propertyModel.BrushInfo;
        public LinkCommonInfo CommonInfo => _propertyModel.CommonInfo;

        public ReactiveCommand<Point, Unit> ClickCommand { get; }

        public LinkViewModel()
            : this(new LinkPropertyModelEdit(), _ => { })
        {
        }

        public LinkViewModel(LinkPropertyModelEdit propertyModel, Action<Point> clickAction)
        {
            _propertyModel = propertyModel;
            _clickAction = clickAction;
            ClickCommand = ReactiveCommand.Create<Point>(OnClicked);

            _propertyModel.PropertyChanged += PropertyModel_PropertyChanged;
            _propertyModel.BrushInfo.PropertyChanged += ChildPropertyChanged;
            _propertyModel.CommonInfo.PropertyChanged += ChildPropertyChanged;
        }

        public void ReloadFromModel()
        {
            RaisePropertyChanged(nameof(BrushInfo));
            RaisePropertyChanged(nameof(CommonInfo));
        }

        private void PropertyModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
        }

        private void ChildPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is LinkBrushInfo)
                RaisePropertyChanged(nameof(BrushInfo));
            else if (sender is LinkCommonInfo)
                RaisePropertyChanged(nameof(CommonInfo));
        }

        private void OnClicked(Point screenPoint)
        {
            _clickAction?.Invoke(screenPoint);
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _propertyModel.PropertyChanged -= PropertyModel_PropertyChanged;
            _propertyModel.BrushInfo.PropertyChanged -= ChildPropertyChanged;
            _propertyModel.CommonInfo.PropertyChanged -= ChildPropertyChanged;
            _disposed = true;
        }
    }
}
