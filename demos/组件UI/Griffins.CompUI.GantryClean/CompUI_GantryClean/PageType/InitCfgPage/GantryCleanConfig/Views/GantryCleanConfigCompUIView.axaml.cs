using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using System;
using System.Linq;
using System.Reactive.Linq;

namespace Griffins.CompUI.GantryClean.CompUI_GantryClean.PageType.InitCfgPage.GantryCleanConfig.Views
{
    public partial class GantryCleanConfigCompUIView : UserControl
    {
        private IDisposable? _viewportSub;

        public GantryCleanConfigCompUIView()
        {
            InitializeComponent();

            AttachedToVisualTree += OnAttached;
            DetachedFromVisualTree += OnDetached;
        }

        private void OnAttached(object? sender, VisualTreeAttachmentEventArgs e)
        {
            BindToParentViewport();
        }

        private void OnDetached(object? sender, VisualTreeAttachmentEventArgs e)
        {
            _viewportSub?.Dispose();
            _viewportSub = null;
        }

        private void BindToParentViewport()
        {
            _viewportSub?.Dispose();
            _viewportSub = null;

            if (RootGrid == null) return;

            var ancestors = this.GetVisualAncestors();
            var sv = Enumerable.FirstOrDefault(Enumerable.OfType<ScrollViewer>(ancestors));
            if (sv == null)
            {
                _viewportSub = this.GetObservable(BoundsProperty)
                    .Subscribe(b => ApplySize(b.Width, b.Height));
                return;
            }

            // Initial apply from current viewport
            ApplySize(sv.Viewport.Width, sv.Viewport.Height);

            _viewportSub = sv.GetObservable(ScrollViewer.ViewportProperty)
                .Subscribe(v => ApplySize(v.Width, v.Height));
        }

        private void ApplySize(double width, double height)
        {
            if (RootGrid == null) return;

            if (width > 0) RootGrid.Width = width;
            if (height > 0) RootGrid.Height = height;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            RootGrid = this.FindControl<Grid>("RootGrid");
        }
    }
}
