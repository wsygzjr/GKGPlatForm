using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using MainFrame.ViewModels;

namespace MainFrameView.Views
{
    public partial class PreviewView : UserControl
    {
        // 公开ViewModel属性，便于外部访问
        //public PreviewViewModel ViewModel => DataContext as PreviewViewModel;

        public PreviewView()
        {
            InitializeComponent();

            // 设置数据上下文为ViewModel实例
            DataContext = new PreviewViewModel();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
