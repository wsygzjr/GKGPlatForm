using Avalonia.Controls;
using Avalonia.PropertyGrid.Controls;
using Avalonia.PropertyGrid.Controls.Factories;
using GKG.Map.MapCell.Generic.Control.MapCell_DisplayList.Objects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GKG.Map.MapCell.Generic.Control.MapCell_DisplayList.Factories
{
    public class DisplayListSortFieldEditorFactory : AbstractCellEditFactory
    {
        public override bool Accept(object accessToken)
        {
            return true;
        }

        public override int ImportPriority => 9999;

        public override Avalonia.Controls.Control HandleNewProperty(PropertyCellContext context)
        {
            if (context == null) return null;

            var propertyDescriptor = context.Property;
            var target = context.Target;

            if (propertyDescriptor?.PropertyType != typeof(string))
                return null;

            if (!string.Equals(propertyDescriptor.Name, nameof(DisplayListCommonInfo.SortField), StringComparison.Ordinal))
                return null;

            if (target is not DisplayListCommonInfo common)
                return null;

            var items = new ObservableCollection<string>();
            RefreshItems(common, items, propertyDescriptor.GetValue(target) as string);

            var combo = new Avalonia.Controls.ComboBox
            {
                ItemsSource = items,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
                MinWidth = 120
            };

            combo.Tag = (common, items);

            var curValue = propertyDescriptor.GetValue(target) as string ?? string.Empty;
            combo.SelectedItem = curValue;

            combo.SelectionChanged += (s, e) =>
            {
                var selected = combo.SelectedItem as string ?? string.Empty;
                SetAndRaise(context, combo, selected);
            };

            common.PropertyChanged += (s, e) =>
            {
                if (string.Equals(e.PropertyName, nameof(DisplayListCommonInfo.Columns), StringComparison.Ordinal))
                {
                    var keep = propertyDescriptor.GetValue(target) as string;
                    RefreshItems(common, items, keep);
                }
            };

            if (common.Columns != null)
            {
                common.Columns.CollectionChanged += (s, e) =>
                {
                    var keep = propertyDescriptor.GetValue(target) as string;
                    RefreshItems(common, items, keep);
                };
            }

            return combo;
        }

        public override bool HandlePropertyChanged(PropertyCellContext context)
        {
            if (context == null) return false;

            var propertyDescriptor = context.Property;
            var target = context.Target;
            var control = context.CellEdit as Avalonia.Controls.Control;

            if (propertyDescriptor?.PropertyType != typeof(string))
                return false;

            if (!string.Equals(propertyDescriptor.Name, nameof(DisplayListCommonInfo.SortField), StringComparison.Ordinal))
                return false;

            if (target is not DisplayListCommonInfo)
                return false;

            if (control is Avalonia.Controls.ComboBox combo)
            {
                var newValue = propertyDescriptor.GetValue(target) as string ?? string.Empty;
                if (!string.Equals(combo.SelectedItem as string, newValue, StringComparison.Ordinal))
                    combo.SelectedItem = newValue;
                return true;
            }

            return false;
        }

        private static void RefreshItems(DisplayListCommonInfo common, ObservableCollection<string> items, string keepValue)
        {
            if (items == null) return;

            var keep = keepValue ?? string.Empty;

            var set = new HashSet<string>(StringComparer.Ordinal);
            var list = new List<string>();

            list.Add(string.Empty);
            set.Add(string.Empty);

            if (common?.Columns != null)
            {
                foreach (var c in common.Columns)
                {
                    var id = c?.FieldID ?? string.Empty;
                    if (string.IsNullOrWhiteSpace(id))
                        continue;
                    if (set.Add(id))
                        list.Add(id);
                }
            }

            if (!set.Contains(keep))
                list.Add(keep);

            items.Clear();
            foreach (var it in list)
                items.Add(it);
        }
    }
}
