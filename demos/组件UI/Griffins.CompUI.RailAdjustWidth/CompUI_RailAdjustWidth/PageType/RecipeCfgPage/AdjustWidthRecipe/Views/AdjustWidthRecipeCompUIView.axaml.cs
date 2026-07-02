using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Griffins.CompUI.RailAdjustWidth.CompUI_RailAdjustWidth.PageType.RecipeCfgPage.AdjustWidthRecipe.ViewModels;
using System;

namespace Griffins.CompUI.RailAdjustWidth.CompUI_RailAdjustWidth.PageType.RecipeCfgPage.AdjustWidthRecipe.Views
{
    public partial class AdjustWidthRecipeCompUIView : UserControl
    {
        public AdjustWidthRecipeCompUIView()
        {
            InitializeComponent();
            ButtonAddPressedHandler(this.FindControl<Button>("Button_MoveFront"), ForwardButton_OnPointerPressed);
            ButtonAddPressedHandler(this.FindControl<Button>("Button_MoveBack"), BackwardButton_OnPointerPressed);
            ButtonAddReleasedHandler(this.FindControl<Button>("Button_MoveFront"), ForwardButton_OnPointerReleased);
            ButtonAddReleasedHandler(this.FindControl<Button>("Button_MoveBack"), BackwardButton_OnPointerReleased);
        }
        private void ButtonAddPressedHandler(Avalonia.Controls.Button button, Delegate @delegate)
        {
            button.AddHandler(
                InputElement.PointerPressedEvent,
                @delegate,
                RoutingStrategies.Tunnel | RoutingStrategies.Bubble,
                handledEventsToo: true);
        }
        private void ButtonAddReleasedHandler(Avalonia.Controls.Button button, Delegate @delegate)
        {
            button.AddHandler(
                InputElement.PointerReleasedEvent,
                @delegate,
                RoutingStrategies.Tunnel | RoutingStrategies.Bubble,
                handledEventsToo: true);
        }
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void ForwardButton_OnPointerPressed(object sender, PointerPressedEventArgs e)
        {
            if (DataContext is AdjustWidthRecipeCompUIViewModel viewModel)
            {
                viewModel.StartContinueMove(isForward: true);
            }
        }

        private void BackwardButton_OnPointerPressed(object sender, PointerPressedEventArgs e)
        {
            if (DataContext is AdjustWidthRecipeCompUIViewModel viewModel)
            {
                viewModel.StartContinueMove(isForward: false);
            }
        }

        private void ForwardButton_OnPointerReleased(object sender, PointerReleasedEventArgs e)
        {
            StopContinueMove();
        }

        private void BackwardButton_OnPointerReleased(object sender, PointerReleasedEventArgs e)
        {
            StopContinueMove();
        }

        private void MoveButton_OnPointerCaptureLost(object sender, PointerCaptureLostEventArgs e)
        {
            StopContinueMove();
        }

        private void StopContinueMove()
        {
            if (DataContext is AdjustWidthRecipeCompUIViewModel viewModel)
            {
                viewModel.StopSelectedRail();
            }
        }
    }
}
