using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace GKG.UI.General
{
    /// <summary>
    /// 相机操作视图
    /// </summary>
    public partial class PositionTeachView : UserControl
    {
        /// <summary>
        /// 相机操作视图
        /// </summary>
        public PositionTeachView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
     
}
