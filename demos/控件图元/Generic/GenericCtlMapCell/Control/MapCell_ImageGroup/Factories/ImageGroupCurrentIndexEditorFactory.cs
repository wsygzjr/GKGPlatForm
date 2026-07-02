using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.PropertyGrid.Controls;
using Avalonia.PropertyGrid.Controls.Factories;
using Avalonia.VisualTree;
using GKG.Map.MapCell.Generic.Control.MapCell_ImageGroup.Objects;
using GKG.Map.MapCell.Generic.Control.MapCell_ImageGroup.UI;
using System;

namespace GKG.Map.MapCell.Generic.Control.MapCell_ImageGroup.Factories
{
    public class ImageGroupCurrentIndexEditorFactory : AbstractCellEditFactory
    {
        public override bool Accept(object accessToken)
        {
            return true;
        }

        public override int ImportPriority => 9998;

        public override Avalonia.Controls.Control HandleNewProperty(PropertyCellContext context)
        {
            if (context == null) return null;

            var propertyDescriptor = context.Property;
            var target = context.Target;

            if (propertyDescriptor?.PropertyType == null)
                return null;

            if (propertyDescriptor.PropertyType != typeof(int))
                return null;

            if (!string.Equals(propertyDescriptor.Name, nameof(ImageGroupCommonInfo.CurrentIndex), StringComparison.Ordinal))
                return null;

            // 仅处理顶层 ImageGroupPropertyModelEdit.CurrentIndex
            if (target is not GKG.Map.MapCell.Generic.Control.MapCell_ImageGroup.ImageGroupPropertyModelEdit)
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
                Content = "弹窗设置",
                MinWidth = 90,
                Height = 26,
                Margin = new Thickness(2, 0, 0, 0),
                Padding = new Thickness(8, 2)
            };

            UpdateDisplayText(displayText, (int)(propertyDescriptor.GetValue(target) ?? 0));

            editButton.Click += async (s, e) =>
            {
                var (images, currentIndex) = GetImagesAndIndex(target);

                var editorWindow = new ImageGroupImagesWindow
                {
                    ImageSources = images,
                    CurrentIndex = currentIndex,
                    IndexOnly = true,
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
                    var newIndex = editorWindow.CurrentIndex;
                    SetAndRaise(context, panel, newIndex);
                    UpdateDisplayText(displayText, newIndex);
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

            if (propertyDescriptor?.PropertyType != typeof(int))
                return false;

            if (!string.Equals(propertyDescriptor.Name, nameof(ImageGroupCommonInfo.CurrentIndex), StringComparison.Ordinal))
                return false;

            if (target is not GKG.Map.MapCell.Generic.Control.MapCell_ImageGroup.ImageGroupPropertyModelEdit)
                return false;

            if (control is Avalonia.Controls.StackPanel panel && panel.Tag is Avalonia.Controls.TextBlock displayText)
            {
                UpdateDisplayText(displayText, (int)(propertyDescriptor.GetValue(target) ?? 0));
                return true;
            }

            return false;
        }

        private static void UpdateDisplayText(Avalonia.Controls.TextBlock textBlock, int index)
        {
            textBlock.Text = $"{index}";
        }

        private static (System.Collections.Generic.List<BitmapData> images, int currentIndex) GetImagesAndIndex(object target)
        {
            if (target is GKG.Map.MapCell.Generic.Control.MapCell_ImageGroup.ImageGroupPropertyModelEdit model)
            {
                var images = model.CommonInfo?.ImageSources ?? new System.Collections.Generic.List<BitmapData>();
                return (new System.Collections.Generic.List<BitmapData>(images), model.CurrentIndex);
            }

            return (new System.Collections.Generic.List<BitmapData>(), 0);
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
