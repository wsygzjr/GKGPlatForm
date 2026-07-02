using Avalonia.Controls;
using Griffins.CompUI.EletronicManager.CompUI_EletronicManager.PageType.ElectricalMngPage.ControlCard.ViewModels;
using System.ComponentModel;

namespace Griffins.CompUI.EletronicManager.CompUI_EletronicManager.PageType.ElectricalMngPage.ControlCard.Views
{
    public partial class AxisParameterView : UserControl
    {
        private AxisConfigViewModel? _boundAxisConfigViewModel;

        public AxisParameterView()
        {
            InitializeComponent();
            DataContextChanged += (_, _) => RefreshEditorDataContext();
        }

        private void RefreshEditorDataContext()
        {
            if (_boundAxisConfigViewModel != null)
                _boundAxisConfigViewModel.PropertyChanged -= AxisConfigViewModel_PropertyChanged;

            _boundAxisConfigViewModel = DataContext as AxisConfigViewModel;

            if (_boundAxisConfigViewModel != null)
            {
                _boundAxisConfigViewModel.PropertyChanged += AxisConfigViewModel_PropertyChanged;
                SetEditorDataContext(_boundAxisConfigViewModel.SelectedAxis);
                return;
            }

            SetEditorDataContext(DataContext as MotionControlFactoryParameterViewModel);
        }

        private void AxisConfigViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AxisConfigViewModel.SelectedAxis) ||
                e.PropertyName == nameof(AxisConfigViewModel.SelectedAxisItem))
            {
                SetEditorDataContext(_boundAxisConfigViewModel?.SelectedAxis);
            }
        }

        private void SetEditorDataContext(MotionControlFactoryParameterViewModel? axisViewModel)
        {
            if (this.FindControl<MotionControlAxisEditorView>("EditorView") is not { } editorView)
                return;

            if (!ReferenceEquals(editorView.DataContext, axisViewModel))
                editorView.DataContext = axisViewModel;

            editorView.IsEnabled = axisViewModel != null;
        }
    }
}
