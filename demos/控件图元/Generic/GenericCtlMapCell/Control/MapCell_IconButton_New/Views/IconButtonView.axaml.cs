using Avalonia.Controls;
using Avalonia.Input;
using GKG.Map.MapCell.Generic.IconButton.ViewModels;

namespace GKG.Map.MapCell.Generic.IconButton.Views
{
    public partial class IconButtonView : UserControl
    {
        public IconButtonView()
        {
            InitializeComponent();

            // 1. 鼠标按下 (MouseDown & Click)
            RootBorder.PointerPressed += (s, e) =>
            {
                if (DataContext is IconButtonViewModel vm && vm.IsEnabled)
                {
                    var point = e.GetCurrentPoint(this);
                    if (point.Properties.IsLeftButtonPressed)
                    {
                        RootBorder.Classes.Add("pressed");
                        vm.NotifyMouseDown();
                        vm.NotifyClicked();
                    }
                }
            };

            // 2. 鼠标松开 (MouseUp)
            RootBorder.PointerReleased += (s, e) =>
            {
                RootBorder.Classes.Remove("pressed");
                if (DataContext is IconButtonViewModel vm && vm.IsEnabled)
                {
                    vm.NotifyMouseUp();
                }
            };

            // 3. 鼠标进入 (MouseEnter)
            RootBorder.PointerEntered += (s, e) =>
            {
                if (DataContext is IconButtonViewModel vm && vm.IsEnabled)
                {
                    vm.NotifyMouseEnter();
                }
            };

            // 4. 鼠标离开 (MouseLeave)
            RootBorder.PointerExited += (s, e) =>
            {
                RootBorder.Classes.Remove("pressed"); // 安全机制
                if (DataContext is IconButtonViewModel vm && vm.IsEnabled)
                {
                    vm.NotifyMouseLeave();
                }
            };

            // 5. 鼠标双击 (MouseDoubleClick)
            RootBorder.DoubleTapped += (s, e) =>
            {
                if (DataContext is IconButtonViewModel vm && vm.IsEnabled)
                {
                    vm.NotifyMouseDoubleClick();
                }
            };
        }
    }
}