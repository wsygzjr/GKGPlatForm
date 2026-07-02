using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace GKG.UI.General
{
    /// <summary>
    /// 运控卡ID-IO通道号-水平排列视图
    /// </summary>
    public partial class HorizontalControlCardStateInitView : UserControl
    {
        /// <summary>
        /// 水平排列-运控卡ID-IO通道号
        /// </summary>
        public HorizontalControlCardStateInitView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
