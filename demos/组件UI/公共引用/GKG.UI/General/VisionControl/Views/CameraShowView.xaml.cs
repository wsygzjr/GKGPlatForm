using Avalonia.Markup.Xaml;
using Avalonia.Controls;

namespace GKG.UI.General
{
    /// <summary>
    /// 相机显示视图
    /// </summary>
    public partial class CameraShowView : UserControl
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public CameraShowView()
        {
            InitializeComponent();
           
        }
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
 

}
