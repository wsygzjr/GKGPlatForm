using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace GKG.UI.General
{
    /// <summary>
    /// 标准气缸视图
    /// </summary>
    public partial class StandardCylinderView : UserControl
    {
        /// <summary>
        /// 标准气缸视图
        /// </summary>
        public StandardCylinderView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
