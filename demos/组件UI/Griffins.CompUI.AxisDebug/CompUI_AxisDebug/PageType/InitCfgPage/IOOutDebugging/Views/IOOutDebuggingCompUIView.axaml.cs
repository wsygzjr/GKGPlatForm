using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Griffins.CompUI.AxisDebug.CompUI_AxisDebug.PageType.InitCfgPage.IOOutDebugging.ViewModels;

namespace Griffins.CompUI.AxisDebug.CompUI_AxisDebug.PageType.InitCfgPage.IOOutDebugging.Views
{
    public partial class IOOutDebuggingCompUIView : UserControl
    {
        public IOOutDebuggingCompUIView()
        {
            InitializeComponent();
        }

        private void RowBorder_OnPointerPressed(object sender, PointerPressedEventArgs e)
        {
            if (DataContext is not IOOutDebuggingCompUIViewModel vm)
                return;

            if (sender is not Control row)
                return;

            if (row.DataContext is not IOOutItemViewModel item)
                return;

            vm.SelectItem(item);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
