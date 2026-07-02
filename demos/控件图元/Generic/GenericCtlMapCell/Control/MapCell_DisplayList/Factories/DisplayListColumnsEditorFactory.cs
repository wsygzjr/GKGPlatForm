using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.PropertyGrid.Controls;
using Avalonia.PropertyGrid.Controls.Factories;
using Avalonia.VisualTree;
using GKG.Map.MapCell.Generic.Control.MapCell_DisplayList.Objects;
using GKG.Map.MapCell.Generic.Control.MapCell_DisplayList.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GKG.Map.MapCell.Generic.Control.MapCell_DisplayList.Factories
{
	/// <summary>
	/// DisplayList 的“列配置（Columns）”属性编辑器工厂。
	/// 
	/// 用于在 PropertyGrid 中把 Columns（ObservableCollection&lt;DisplayListColumnInfo&gt;）
	/// 从默认编辑方式替换为“打开配置窗口”的交互方式：
	/// - 显示当前列数
	/// - 点击按钮打开 DisplayListColumnsWindow 进行编辑
	/// - 确认后把新集合写回目标对象并触发属性变更
	/// </summary>
    public class DisplayListColumnsEditorFactory : AbstractCellEditFactory
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

            if (!typeof(ObservableCollection<DisplayListColumnInfo>).IsAssignableFrom(propertyDescriptor.PropertyType))
                return null;

            if (!string.Equals(propertyDescriptor.Name, nameof(DisplayListCommonInfo.Columns), StringComparison.Ordinal))
                return null;

            if (target is not DisplayListCommonInfo)
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
                Content = "添加列字段和列名",
                MinWidth = 90,
                Height = 26,
                Margin = new Thickness(-22, 0, 0, 0),
                Padding = new Thickness(8, 2)
            };

            UpdateDisplayText(displayText, propertyDescriptor.GetValue(target) as ObservableCollection<DisplayListColumnInfo>);

            editButton.Click += async (s, e) =>
            {
                // 从目标对象取出当前列配置，克隆一份给编辑窗口，避免取消时污染原数据
                var currentValue = propertyDescriptor.GetValue(target) as ObservableCollection<DisplayListColumnInfo> ?? new ObservableCollection<DisplayListColumnInfo>();
                var editCopy = CloneColumns(currentValue);

                var editorWindow = new DisplayListColumnsWindow
                {
                    Columns = editCopy,
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
                    // 确认后写回新集合，并更新显示文本
                    var newValue = new ObservableCollection<DisplayListColumnInfo>(editorWindow.Columns ?? new List<DisplayListColumnInfo>());
                    SetAndRaise(context, panel, newValue);
                    UpdateDisplayText(displayText, newValue);
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

            if (!typeof(ObservableCollection<DisplayListColumnInfo>).IsAssignableFrom(propertyDescriptor.PropertyType))
                return false;

            if (!string.Equals(propertyDescriptor.Name, nameof(DisplayListCommonInfo.Columns), StringComparison.Ordinal))
                return false;

            if (target is not DisplayListCommonInfo)
                return false;

            if (control is Avalonia.Controls.StackPanel panel && panel.Tag is Avalonia.Controls.TextBlock displayText)
            {
                UpdateDisplayText(displayText, propertyDescriptor.GetValue(target) as ObservableCollection<DisplayListColumnInfo>);
                return true;
            }

            return false;
        }

        private static List<DisplayListColumnInfo> CloneColumns(ObservableCollection<DisplayListColumnInfo> cols)
        {
            var list = new List<DisplayListColumnInfo>();
            if (cols == null) return list;

            foreach (var c in cols)
            {
                var nc = new DisplayListColumnInfo
                {
                    FieldID = c?.FieldID ?? string.Empty,
                    DisplayName = c?.DisplayName ?? string.Empty
                };
                list.Add(nc);
            }

            return list;
        }

        private static void UpdateDisplayText(Avalonia.Controls.TextBlock textBlock, ObservableCollection<DisplayListColumnInfo> cols)
        {
            int count = cols?.Count ?? 0;
            textBlock.Text = $"共{count} 列";
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
