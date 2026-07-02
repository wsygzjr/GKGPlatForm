using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System;

namespace GKG.Map.ChartFuncCtlMapCell.View;

public partial class VerticalNumericUpDown : UserControl
{
    public VerticalNumericUpDown()
    {
        InitializeComponent();

        // 监听失去焦点事件
        InputBox.LostFocus += InputBox_LostFocus;

        // 初始显示
        InputBox.Text = Value.ToString();
    }

    #region 依赖属性定义 (Properties)

    // 在 Value 属性上挂载 Coerce 拦截器，控制最大最小值
    public static readonly StyledProperty<int> ValueProperty =
        AvaloniaProperty.Register<VerticalNumericUpDown, int>(
            nameof(Value),
            defaultValue: 0,
            coerce: CoerceValueHandler);

    public int Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public static readonly StyledProperty<int> MinimumProperty =
        AvaloniaProperty.Register<VerticalNumericUpDown, int>(nameof(Minimum), defaultValue: 0);

    public int Minimum
    {
        get => GetValue(MinimumProperty);
        set => SetValue(MinimumProperty, value);
    }

    public static readonly StyledProperty<int> MaximumProperty =
        AvaloniaProperty.Register<VerticalNumericUpDown, int>(nameof(Maximum), defaultValue: 100);

    public int Maximum
    {
        get => GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }

    #endregion

    #region 核心算法与事件处理 (Logic)

    // 属性强制约束：无论谁修改 Value，都不能越界
    private static int CoerceValueHandler(AvaloniaObject sender, int value)
    {
        if (sender is VerticalNumericUpDown control)
        {
            return Math.Clamp(value, control.Minimum, control.Maximum);
        }
        return value;
    }

    // 监听底层的 Value 变化，同步给文本框（处理外部代码修改 Value 的情况）
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == ValueProperty)
        {
            // 只有当用户没有在打字时，才去同步显示（防抖：避免打字时光标乱跳）
            if (InputBox != null && !InputBox.IsFocused)
            {
                InputBox.Text = Value.ToString();
            }
        }
    }

    // 极其温柔的输入拦截器，处理正在输入时的合法数字
    private void InputBox_TextChanged(object? sender, TextChangedEventArgs e)
    {
        if (InputBox != null && InputBox.IsFocused)
        {
            if (int.TryParse(InputBox.Text, out int parsed))
            {
                Value = parsed;

                if (Value != parsed)
                {
                    InputBox.Text = Value.ToString();
                    InputBox.CaretIndex = InputBox.Text.Length;
                }
            }
            // 正在输入时遇到空或者中文，什么都不做，等失去焦点时再算账
        }
    }

    // 点击向上按钮
    private void OnUpClick(object? sender, RoutedEventArgs e)
    {
        if (Value < Maximum) Value++;
    }

    // 点击向下按钮
    private void OnDownClick(object? sender, RoutedEventArgs e)
    {
        if (Value > Minimum) Value--;
    }

    // 失去焦点时，归零异常输入
    private void InputBox_LostFocus(object? sender, RoutedEventArgs e)
    {
        if (InputBox != null)
        {
            // 尝试解析当前的文本
            if (!int.TryParse(InputBox.Text, out int parsed))
            {
                // 如果解析失败（说明框被删空了，或者输入了纯中文/英文字母）
                // 直接赋 0！(不用担心越界，底层的 Coerce 会把它修正到 Minimum)
                Value = 0;
            }
            else
            {
                // 如果解析成功，确保 Value 是最新的
                Value = parsed;
            }

            // 无论刚才发生了什么，强行用底层干净的 Value 覆盖文本框
            InputBox.Text = Value.ToString();
        }
    }

    #endregion
}