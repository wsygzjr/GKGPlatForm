using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace GKG.UI.General
{
    /// <summary>
    /// 双控单限位视图
    /// </summary>
    public partial class DoubleControlSingleLimitView : UserControl
    {
        /// <summary>
        /// 双控单限位视图
        /// </summary>
        public DoubleControlSingleLimitView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
