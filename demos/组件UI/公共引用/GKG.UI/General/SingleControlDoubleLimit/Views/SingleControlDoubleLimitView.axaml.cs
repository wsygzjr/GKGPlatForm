using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace GKG.UI.General
{
    /// <summary>
    /// 单轨双限位-视图
    /// </summary>
    public partial class SingleControlDoubleLimitView : UserControl
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public SingleControlDoubleLimitView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}