using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Griffins.CompUI.EletronicManager.CompUI_EletronicManager.PageType.DebugPage.ViewModels;
using System;

namespace Griffins.CompUI.EletronicManager.CompUI_EletronicManager.PageType.DebugPage.Views
{
    public partial class AxisDebugIOInView : UserControl
    {
        private IDisposable? _visibilitySubscription;
        private bool _isPollingActive;

        public AxisDebugIOInView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);
            _visibilitySubscription = this.GetObservable(IsVisibleProperty).Subscribe(SetPollingActive);
            SetPollingActive(IsVisible);
        }

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            _visibilitySubscription?.Dispose();
            _visibilitySubscription = null;
            SetPollingActive(false);
            base.OnDetachedFromVisualTree(e);
        }

        private void SetPollingActive(bool active)
        {
            if (DataContext is not AxisDebugWindowViewModel viewModel)
            {
                return;
            }

            if (active)
            {
                if (_isPollingActive)
                {
                    return;
                }

                _isPollingActive = true;
                viewModel.OnViewAttached();
                return;
            }

            if (!_isPollingActive)
            {
                return;
            }

            _isPollingActive = false;
            viewModel.OnViewDetached();
        }
    }
}
