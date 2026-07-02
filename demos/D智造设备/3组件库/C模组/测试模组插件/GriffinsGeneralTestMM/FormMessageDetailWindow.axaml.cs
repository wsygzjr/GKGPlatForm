using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace GriffinsGeneralTestMM
{
    public partial class FormMessageDetailWindow : Window
    {
        private readonly FormMessageDetailViewModel _viewModel;

        /// <summary>
        /// 构造函数：接收消息文本并初始化ViewModel
        /// </summary>
        /// <param name="message">需要展示的消息内容</param>
        public FormMessageDetailWindow(string message)
        {
            InitializeComponent();
            // 初始化ViewModel并绑定到视图
            _viewModel = new FormMessageDetailViewModel(message);
            DataContext = _viewModel;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}