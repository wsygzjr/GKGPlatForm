using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.PropertyGrid.Controls;
using Avalonia.PropertyGrid.Controls.Factories;
using Avalonia.VisualTree;
using GF_Gereric;
using GKG.Map.MapCell.Generic.Control.MapCell_ImageGroup.Objects;
using GKG.Map.MapCell.Generic.Control.MapCell_ImageGroup.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GKG.Map.MapCell.Generic.Control.MapCell_ImageGroup.Factories
{
    public class ImageGroupImageSourcesEditorFactory : AbstractCellEditFactory
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

            if (propertyDescriptor?.PropertyType == null)
                return null;

            if (!typeof(List<BitmapData>).IsAssignableFrom(propertyDescriptor.PropertyType))
                return null;

            if (!string.Equals(propertyDescriptor.Name, nameof(ImageGroupCommonInfo.ImageSources), StringComparison.Ordinal))
                return null;

            if (target is not ImageGroupCommonInfo)
                return null;

            var panel = new Avalonia.Controls.StackPanel
            {
                Orientation = Avalonia.Layout.Orientation.Horizontal,
                Spacing = 5,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch
            };

            var displayText = new Avalonia.Controls.TextBlock
            {
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                MinWidth = 120
            };

            var editButton = new Avalonia.Controls.Button
            {
                Content = "编辑图片列表",
                MinWidth = 90,
                Height = 26,
                Margin = new Thickness(2, 0, 0, 0),
                Padding = new Thickness(8, 2)
            };

            UpdateDisplayText(displayText, propertyDescriptor.GetValue(target) as List<BitmapData>);

            editButton.Click += async (s, e) =>
            {
                var currentValue = propertyDescriptor.GetValue(target) as List<BitmapData> ?? new List<BitmapData>();
                var editCopy = new List<BitmapData>(currentValue);

                var editorWindow = new ImageGroupImagesWindow
                {
                    ImageSources = editCopy,
                    CurrentIndex = (target as ImageGroupCommonInfo)?.CurrentIndex ?? 0,
                    HostPropertyGrid = FindAncestorPropertyGrid(editButton)
                };

                Avalonia.Controls.Window parentWindow = GetParentWindow(editButton);

                if (parentWindow != null)
                {
                    await editorWindow.ShowDialog(parentWindow);
                }
                else
                {
                    editorWindow.Show();
                    var tcs = new System.Threading.Tasks.TaskCompletionSource<bool>();
                    editorWindow.Closed += (_, __) => tcs.SetResult(true);
                    await tcs.Task;
                }

                if (editorWindow.DialogResult)
                {
                    var newValue = editorWindow.ImageSources;
                    SetAndRaise(context, panel, newValue);
                    UpdateDisplayText(displayText, newValue);

                    if (target is ImageGroupCommonInfo common)
                    {
                        common.CurrentIndex = editorWindow.CurrentIndex;
                    }
                }
            };

            panel.Children.Add(displayText);
            panel.Children.Add(editButton);
            panel.Tag = displayText;

            return panel;
        }

        public override bool HandlePropertyChanged(PropertyCellContext context)
        {
            if (context == null) return false;

            var propertyDescriptor = context.Property;
            var target = context.Target;
            var control = context.CellEdit as Avalonia.Controls.Control;

            if (propertyDescriptor?.PropertyType == null)
                return false;

            if (!typeof(List<BitmapData>).IsAssignableFrom(propertyDescriptor.PropertyType))
                return false;

            if (!string.Equals(propertyDescriptor.Name, nameof(ImageGroupCommonInfo.ImageSources), StringComparison.Ordinal))
                return false;

            if (control is Avalonia.Controls.StackPanel panel && panel.Tag is Avalonia.Controls.TextBlock displayText)
            {
                UpdateDisplayText(displayText, propertyDescriptor.GetValue(target) as List<BitmapData>);
                return true;
            }

            return false;
        }

        private static void UpdateDisplayText(Avalonia.Controls.TextBlock textBlock, List<BitmapData> images)
        {
            int count = images?.Count ?? 0;
            textBlock.Text = $"{count} 张图片";
        }

        private static Avalonia.Controls.Window GetParentWindow(Avalonia.Controls.Control control)
        {
            var visual = control as Visual;
            while (visual != null)
            {
                if (visual is Avalonia.Controls.Window window)
                    return window;
                visual = visual.GetVisualParent() as Visual;
            }

            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                return desktop.MainWindow;
            }

            return null;
        }

        private static PropertyGrid FindAncestorPropertyGrid(Avalonia.Controls.Control control)
        {
            var visual = control as Visual;
            while (visual != null)
            {
                if (visual is PropertyGrid pg)
                    return pg;
                visual = visual.GetVisualParent() as Visual;
            }

            return null;
        }
    }
}
