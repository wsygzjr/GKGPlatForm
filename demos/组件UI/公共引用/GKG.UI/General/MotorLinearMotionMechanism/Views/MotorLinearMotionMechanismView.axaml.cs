using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace GKG.UI.General
{
    /// <summary>
    /// 电机型直线移动机构参数配置界面View
    /// </summary>
    public partial class MotorLinearMotionMechanismView : UserControl
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public MotorLinearMotionMechanismView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
