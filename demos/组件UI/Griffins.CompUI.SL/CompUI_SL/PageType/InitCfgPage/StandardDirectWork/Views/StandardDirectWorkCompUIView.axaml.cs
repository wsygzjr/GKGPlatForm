using Avalonia.Controls;
using Griffins.CompUI.SL.InitCfgPage.ViewModels;

namespace Griffins.CompUI.SL.InitCfgPage.Views;

public partial class StandardDirectWorkCompUIView : UserControl
{
    public StandardDirectWorkCompUIView()
    {
        InitializeComponent();
        this.DataContext = new StandardDirectWorkCompUIViewModel(true, null);
    }
}