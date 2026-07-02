
using Avalonia.Markup.Xaml;

using GriffinsGeneralTestMM;
 
using Avalonia;
using Avalonia.Controls; 

namespace GriffinsGeneralTestMM
{
    public partial class MessageDetailWindow : Window
    {
        public MessageDetailWindow(string message)
        {
            InitializeComponent();
            DataContext = new FormMessageDetailViewModel(message);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}