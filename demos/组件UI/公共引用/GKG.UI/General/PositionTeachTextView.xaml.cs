using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace GKG.UI.General
{
    /// <summary>
    /// 相机操作视图
    /// </summary>
    public partial class PositionTeachTextView : UserControl
    {
        /// <summary>
        /// 相机操作视图
        /// </summary>
        public PositionTeachTextView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
     
}
