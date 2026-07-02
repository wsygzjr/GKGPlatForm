using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.PropertyGrid.Controls;
using Avalonia.PropertyGrid.Controls.Factories;
using GKG.Map.MapCell.Generic.Control.MapCell_Slider;
using System;
using System.Globalization;

namespace GKG.Map.MapCell.Generic.Control.MapCell_Slider.Factories
{
    public class SliderValueStepEditorFactory : AbstractCellEditFactory
    {
        public override bool Accept(object accessToken)
        {
            return true;
        }

        public override int ImportPriority => 10000;

        public override Avalonia.Controls.Control HandleNewProperty(PropertyCellContext context)
        {
            // 不再为Value属性创建特殊编辑器，使用默认编辑器
            return null;
        }

        public override bool HandlePropertyChanged(PropertyCellContext context)
        {
            // 不再处理Value属性的变化，使用默认编辑器
            return false;
        }
    }
}
