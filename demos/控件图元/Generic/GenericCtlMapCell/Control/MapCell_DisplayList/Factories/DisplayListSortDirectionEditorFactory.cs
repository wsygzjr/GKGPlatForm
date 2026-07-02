using Avalonia.Controls;
using Avalonia.PropertyGrid.Controls;
using Avalonia.PropertyGrid.Controls.Factories;
using GKG.Map.MapCell.Generic.Control.MapCell_DisplayList.Objects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace GKG.Map.MapCell.Generic.Control.MapCell_DisplayList.Factories
{
    public class DisplayListSortDirectionEditorFactory : AbstractCellEditFactory
    {
        public override bool Accept(object accessToken)
        {
            return true;
        }

        public override int ImportPriority => 9999;

        public override Avalonia.Controls.Control HandleNewProperty(PropertyCellContext context)
        {
            if (context == null)
                return null;

            var propertyDescriptor = context.Property;
            var target = context.Target;

            if (propertyDescriptor?.PropertyType != typeof(DisplayListSortDirection))
                return null;

            if (!string.Equals(propertyDescriptor.Name, nameof(DisplayListCommonInfo.SortDirection), StringComparison.Ordinal))
                return null;

            // 创建下拉框并设置中文选项
            var combo = new ComboBox
            {
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
                MinWidth = 120
            };

            // 添加中文选项
            var items = new List<ComboBoxItem>();
            foreach (DisplayListSortDirection value in Enum.GetValues(typeof(DisplayListSortDirection)))
            {
                var description = GetEnumDescription(value);
                var item = new ComboBoxItem
                {
                    Content = description,
                    Tag = value
                };
                items.Add(item);
            }

            combo.ItemsSource = items;

            // 设置当前选中项
            var currentValue = (DisplayListSortDirection)propertyDescriptor.GetValue(target);
            foreach (ComboBoxItem item in items)
            {
                if ((DisplayListSortDirection)item.Tag == currentValue)
                {
                    combo.SelectedItem = item;
                    break;
                }
            }

            // 处理选择变化
            combo.SelectionChanged += (s, e) =>
            {
                if (combo.SelectedItem is ComboBoxItem selectedItem)
                {
                    var value = (DisplayListSortDirection)selectedItem.Tag;
                    SetAndRaise(context, combo, value);
                }
            };

            return combo;
        }

        private string GetEnumDescription(Enum value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());
            if (fieldInfo == null)
                return value.ToString();

            var attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : value.ToString();
        }

        public override bool HandlePropertyChanged(PropertyCellContext context)
        {
            if (context == null) return false;

            var propertyDescriptor = context.Property;
            var target = context.Target;
            var control = context.CellEdit as Avalonia.Controls.Control;

            if (propertyDescriptor?.PropertyType != typeof(DisplayListSortDirection))
                return false;

            if (!string.Equals(propertyDescriptor.Name, nameof(DisplayListCommonInfo.SortDirection), StringComparison.Ordinal))
                return false;

            if (control is Avalonia.Controls.ComboBox combo)
            {
                var currentValue = (DisplayListSortDirection)propertyDescriptor.GetValue(target);
                // 查找并设置当前选中项
                foreach (var item in combo.ItemsSource as System.Collections.IEnumerable)
                {
                    if (item is Avalonia.Controls.ComboBoxItem comboItem && (DisplayListSortDirection)comboItem.Tag == currentValue)
                    {
                        combo.SelectedItem = comboItem;
                        break;
                    }
                }
                return true;
            }

            return false;
        }
    }
}