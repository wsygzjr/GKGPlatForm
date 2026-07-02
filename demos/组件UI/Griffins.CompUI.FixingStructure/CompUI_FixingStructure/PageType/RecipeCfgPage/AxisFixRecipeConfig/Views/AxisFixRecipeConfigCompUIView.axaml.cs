using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Griffins.CompUI.FixingStructure.CompUI_FixingStructure.PageType.RecipeCfgPage.AxisFixRecipeConfig.ViewModels;
using System;
using System.Reactive.Linq;

namespace Griffins.CompUI.FixingStructure.CompUI_FixingStructure.PageType.RecipeCfgPage.AxisFixRecipeConfig.Views
{
    /// <summary>
    /// 电机固定机构配方配置视图。
    /// </summary>
    public partial class AxisFixRecipeConfigCompUIView : UserControl
    {
        public AxisFixRecipeConfigCompUIView()
        {
            InitializeComponent();
            ButtonAddPressedHandler(this.FindControl<Button>("Button_MoveUp"), MoveUpButton_OnPointerPressed);
            ButtonAddPressedHandler(this.FindControl<Button>("Button_MoveDown"), MoveDownButton_OnPointerPressed);
            ButtonAddReleasedHandler(this.FindControl<Button>("Button_MoveUp"), MoveButton_OnPointerReleased);
            ButtonAddReleasedHandler(this.FindControl<Button>("Button_MoveDown"), MoveButton_OnPointerReleased);
        }

        private void ButtonAddPressedHandler(Button button, Delegate @delegate)
        {
            button.AddHandler(
                InputElement.PointerPressedEvent,
                @delegate,
                RoutingStrategies.Tunnel | RoutingStrategies.Bubble,
                handledEventsToo: true);
        }

        private void ButtonAddReleasedHandler(Button button, Delegate @delegate)
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

        private void MoveUpButton_OnPointerPressed(object sender, PointerPressedEventArgs e)
        {
            if (DataContext is AxisFixRecipeConfigCompUIViewModel viewModel)
            {
                viewModel.MoveUpCommand.Execute().Subscribe();
            }
        }

        private void MoveDownButton_OnPointerPressed(object sender, PointerPressedEventArgs e)
        {
            if (DataContext is AxisFixRecipeConfigCompUIViewModel viewModel)
            {
                viewModel.MoveDownCommand.Execute().Subscribe();
            }
        }

        private void MoveButton_OnPointerReleased(object sender, PointerReleasedEventArgs e)
        {
            StopContinueMove();
        }

        private void StopContinueMove()
        {
            if (DataContext is AxisFixRecipeConfigCompUIViewModel viewModel)
            {
                viewModel.AxisStopCommand.Execute().Subscribe();
            }
        }
    }
}
