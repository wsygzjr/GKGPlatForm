using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace GKG.UI.General
{
    /// <summary>
    /// 单轨单限位-视图
    /// </summary>
    public partial class SingleControlSingleLimitView : UserControl
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public SingleControlSingleLimitView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}