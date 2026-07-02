using Avalonia.Controls;
using Avalonia.Markup.Xaml; 
using MsBox.Avalonia.Enums;
using ReactiveUI;
using System.Reactive;

namespace DispensingPageType.Views.RecipeParamCfgPage.ParametersAndCalculation
{
    /// <summary>
    /// 相机操作视图
    /// </summary>
    public partial class PositionTeachHeaderWithNameView : UserControl
    {
        public PositionTeachHeaderWithNameView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
     
}
