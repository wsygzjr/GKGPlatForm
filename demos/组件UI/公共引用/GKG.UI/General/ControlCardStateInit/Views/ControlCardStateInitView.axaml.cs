using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace GKG.UI.General
{
    /// <summary>
    /// 单控双限位-视图
    /// </summary>
    public partial class ControlCardStateInitView : UserControl
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ControlCardStateInitView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
