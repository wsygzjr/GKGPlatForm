using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace GKG.Map.Page.UIContainer.TabControlContainer.Views
{
    public partial class ConfigWindow : Window
    {
        public ConfigWindow()
        {
            InitializeComponent();
        }
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
