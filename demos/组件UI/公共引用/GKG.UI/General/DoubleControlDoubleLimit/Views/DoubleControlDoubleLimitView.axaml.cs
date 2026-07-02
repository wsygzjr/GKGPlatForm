using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace GKG.UI.General
{
    /// <summary>
    /// 双控双限位-视图
    /// </summary>
    public partial class DoubleControlDoubleLimitView : UserControl
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public DoubleControlDoubleLimitView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}