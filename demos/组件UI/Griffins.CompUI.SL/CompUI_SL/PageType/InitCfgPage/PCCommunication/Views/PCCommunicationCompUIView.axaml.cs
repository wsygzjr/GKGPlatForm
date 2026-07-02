using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Griffins.CompUI.SL.InitCfgPage.ViewModels;

namespace Griffins.CompUI.SL.InitCfgPage.Views;

public partial class PcCommunicationCompUIView : UserControl
{
    public PcCommunicationCompUIView()
    {
        InitializeComponent();
        DataContext = new PcCommunicationCompUIViewModel(true, null);
        
    }
}