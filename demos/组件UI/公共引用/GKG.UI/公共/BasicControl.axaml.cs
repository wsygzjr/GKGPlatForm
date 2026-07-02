using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;
using System.Reactive.Disposables;

namespace GKG.UI;

/// <summary>
/// 基础控件
/// </summary>
/// <typeparam name="TViewModel"></typeparam>
public partial class BasicControl<TViewModel> : ReactiveUserControl<TViewModel>
    where TViewModel : BasicControlViewModel, new() // 约束：必须有默认构造函数
{
    private TextBlock? _labelText;
    private Border? _border;
    private Panel? _contentControl;
    private BasicControlLayout? layoutControl;

    /// <summary>
    /// 标签文本依赖属性
    /// </summary>
    public static readonly StyledProperty<string> LabelNameProperty =
        AvaloniaProperty.Register<BasicControl, string>(
            nameof(LabelName),
            defaultValue: string.Empty,
            defaultBindingMode: BindingMode.TwoWay
        );

    /// <summary>
    /// 标签文本
    /// </summary>
    public string LabelName
    {
        get => GetValue(LabelNameProperty);
        set => SetValue(LabelNameProperty, value);
    }

    /// <summary>
    /// 宽度级别依赖属性
    /// </summary>
    public static readonly StyledProperty<string> WidthLevelProperty =
        AvaloniaProperty.Register<BasicControl, string>(
            nameof(WidthLevel),
            defaultValue: "Small",  // 默认宽度级别
            defaultBindingMode: BindingMode.TwoWay
        );

    public static readonly StyledProperty<bool> FixWidthProperty =
        AvaloniaProperty.Register<BasicControl, bool>(
            nameof(FixWidth),
            defaultValue: true,
            defaultBindingMode: BindingMode.TwoWay
        );

    /// <summary>
    /// 宽度级别
    /// </summary>
    public string WidthLevel
    {
        get => GetValue(WidthLevelProperty);
        set
        {
            SetValue(WidthLevelProperty, value);
        }
    }

    public bool FixWidth
    {
        get => GetValue(FixWidthProperty);
        set => SetValue(FixWidthProperty, value);
    }

    /// <summary>
    /// 是否显示 GKG 基础控件外层边框。
    /// </summary>
    public static readonly StyledProperty<bool> ShowOuterBorderProperty =
        AvaloniaProperty.Register<BasicControl, bool>(
            nameof(ShowOuterBorder),
            defaultValue: true,
            defaultBindingMode: BindingMode.TwoWay
        );

    /// <summary>
    /// 控制内部基础布局的外层边框，默认保持原有 GKG.UI 样式。
    /// </summary>
    public bool ShowOuterBorder
    {
        get => GetValue(ShowOuterBorderProperty);
        set => SetValue(ShowOuterBorderProperty, value);
    }

    /// <summary>
    /// 
    /// </summary>
    public TextBlock? labelText => _labelText;
    /// <summary>
    /// 
    /// </summary>
    public Border? border => _border;
    /// <summary>
    /// 
    /// </summary>
    public Panel? contentControl => _contentControl;

    /// <summary>
    /// 
    /// </summary>
    public BasicControl()
    {
        // 关键：加载 XAML 布局（必须调用，否则 XAML 中的控件无法被找到）
        InitializeComponent();
        InitializeBindings();
    }
    /// <summary>
    /// 自动生成的初始化方法（用于加载 XAML）
    /// </summary>
    protected void InitializeComponent()
    {
        // 基类 XAML 路径（指向 BasicControlLayout 的 XAML）
        var baseXamlUri = new Uri("avares://GKG.UI/公共/BasicControl.axaml");

        // 加载 XAML，实例化纯布局基类（BasicControlLayout）
        layoutControl = (BasicControlLayout)AvaloniaXamlLoader.Load(baseXamlUri);

        // 3. 将布局基类的控件映射到当前业务基类（关键步骤）
        _labelText = layoutControl.LabelTextControl;
        _border = layoutControl.BorderControl;
        _contentControl = layoutControl.ContentControl;
        // 4. 将布局基类的 Content 赋值给当前业务基类（复用布局）
        this.Content = layoutControl.Content;

        // 5. 同步布局基类的默认尺寸
        this.Width = layoutControl.Width;
        this.Height = layoutControl.Height;
    }
    private void InitializeBindings()
    {
        this.WhenActivated(disposables =>
        {
            //绑定 LabelName → 标签文本
            this.WhenAnyValue(x => x.LabelName)
                .Subscribe(label =>
                {
                    labelText!.Text = label;

                    // LabelName 为空时，不显示标签并且不占用左侧标签列宽
                    // 否则 DataGrid 等紧凑布局下右侧输入区会出现明显留白
                    var hasLabel = !string.IsNullOrWhiteSpace(label);
                    labelText.IsVisible = hasLabel;
                })
                .DisposeWith(disposables);

            // 绑定 WidthLevel → 调用 ViewModel.SetWidth
            this.WhenAnyValue(x => x.WidthLevel)
                .Subscribe(level => ViewModel?.SetWidth(level))
                .DisposeWith(disposables);

            this.WhenAnyValue(x => x.FixWidth, x => x.ViewModel!.TotalWidth)
                .Subscribe(tuple =>
                {
                    var (fixWidth, totalWidth) = tuple;

                    if (_border == null || layoutControl == null)
                        return;

                    var width = fixWidth && !double.IsNaN(totalWidth) ? totalWidth : double.NaN;
                    _border.Width = width;
                    layoutControl.BorderControl.Width = width;
                })
                .DisposeWith(disposables);

            this.WhenAnyValue(x => x.ShowOuterBorder)
                .Subscribe(ApplyOuterBorderState)
                .DisposeWith(disposables);

            // Label 宽度与列宽联动（当 LabelName 为空时强制不占位）
            this.WhenAnyValue(x => x.LabelName, x => x.ViewModel!.LabelWidth)
                .Subscribe(tuple =>
                {
                    var (label, labelWidth) = tuple;
                    var hasLabel = !string.IsNullOrWhiteSpace(label);

                    if (labelText != null)
                    {
                        labelText.Width = hasLabel && labelWidth.HasValue && !double.IsNaN(labelWidth.Value)
                            ? labelWidth.Value
                            : double.NaN;
                    }

                    if (layoutControl != null)
                    {
                        layoutControl.LabelColumn.Width = hasLabel
                            ? (labelWidth.HasValue && !double.IsNaN(labelWidth.Value)
                                ? new GridLength(labelWidth.Value, GridUnitType.Pixel)
                                : GridLength.Auto)
                            : new GridLength(0, GridUnitType.Pixel);
                    }
                })
                .DisposeWith(disposables);


            this.WhenAnyValue(
                   x => x._contentControl!.Bounds,    // 监听Bounds（包含实际尺寸）
                   x => x._contentControl!.IsVisible // 同时监听是否可见（过滤无效值）
               )
               .Subscribe(tuple =>
               {
                   var (bounds, isVisible) = tuple;
                   // 实际宽度 = bounds.Width；实际高度 = bounds.Height
                   double actualContentWidth = bounds.Width;

                   // 仅当控件可见且宽度有效时，同步给ViewModel
                   if (isVisible && actualContentWidth > 0)
                   {
                       ViewModel?.UpdateRightContentWidth(actualContentWidth);
                   }
                   else
                   {
                       ViewModel?.UpdateRightContentWidth(null); // 不可见时设为null
                   }
               })
               .DisposeWith(disposables);

        });

    }

    /// <summary>
    /// 应用外层边框显示状态；隐藏时只去掉 GKG 基础布局的外框，不影响内部输入控件本身的边框。
    /// </summary>
    /// <param name="showOuterBorder">是否显示外层边框</param>
    private void ApplyOuterBorderState(bool showOuterBorder)
    {
        if (_border == null)
        {
            return;
        }

        if (showOuterBorder)
        {
            _border.ClearValue(Border.BorderThicknessProperty);
            _border.ClearValue(Border.BackgroundProperty);
            _border.ClearValue(Border.MarginProperty);
            return;
        }

        _border.BorderThickness = new Thickness(0);
        _border.Background = null;
        _border.Margin = new Thickness(0);
    }

    /// <summary>
    /// 将控件添加到 contentPanel 中，并指定水平对齐方式
    /// </summary>
    /// <param name="content">要添加的控件</param>
    /// <param name="horizontalAlignment">水平对齐方式</param>
    protected void SetContent(object content, HorizontalAlignment horizontalAlignment)
    {
        if (content == null)
            throw new ArgumentNullException(nameof(content), "设置的控件不能为空");
        if (_contentControl == null)
            throw new Exception("右侧contentControl尚未初始化");

        _contentControl.Children.Clear();

        if (content is Control control)
        {
            control.HorizontalAlignment = horizontalAlignment;
            control.VerticalAlignment = VerticalAlignment.Center;
            _contentControl.Children.Add(control);
        }
        else
            throw new Exception("设置的控件为非控件");
    }

}

/// <summary>
/// 基础控件-用于加载布局
/// </summary>
public partial class BasicControl : BasicControl<BasicControlViewModel>
{
}
/// <summary>
/// 基础控件的布局
/// </summary>
public partial class BasicControlLayout : ReactiveUserControl<BasicControlViewModel>
{
    /// <summary>
    /// 
    /// </summary>
    public BasicControlLayout()
    {
    }
    /// <summary>
    /// 
    /// </summary>
    public TextBlock LabelTextControl => this.FindControl<TextBlock>("labelText")!;
    /// <summary>
    /// 
    /// </summary>
    public Border BorderControl => this.FindControl<Border>("borderName")!;
    /// <summary>
    /// 
    /// </summary>
    public Panel ContentControl => this.FindControl<Panel>("contentControl")!;
    /// <summary>
    /// 
    /// </summary>
    public Grid MainGrid => this.FindControl<Grid>("MainGrid")
      ?? throw new Exception("未找到 x:Name 为 MainGrid 的 Grid 控件");

    /// <summary>
    /// 2. 通过索引 0 获取第一列（Label 所在列），完全避开 Name 属性
    /// </summary>
    public ColumnDefinition LabelColumn => MainGrid.ColumnDefinitions[0]
        ?? throw new Exception("Grid 第一列不存在");

}

/// <summary>
/// 基础控件VM
/// </summary>
public class BasicControlViewModel : ReactiveObject
{
    /// <summary>
    /// 统一的值变更事件（参数：旧值，新值）
    /// </summary>
    public event EventHandler<ValueChangedEventArgs>? ValueChanged;

    /// <summary>
    /// 触发值变更事件
    /// </summary>
    /// <param name="oldValue">旧值</param>
    /// <param name="newValue">新值</param>
    protected virtual void OnValueChanged(object? oldValue, object? newValue)
    {
        ValueChanged?.Invoke(this, new ValueChangedEventArgs(oldValue, newValue));
    }
    private double _totalWidth = 200;
    /// <summary>
    /// 控件总宽度
    /// </summary>
    public double TotalWidth
    {
        get => _totalWidth;
        private set => this.RaiseAndSetIfChanged(ref _totalWidth, value);
    }

    private double? _labelWidth = 100;
    /// <summary>
    /// 标签宽度
    /// </summary>
    public double? LabelWidth
    {
        get => _labelWidth;
        private set => this.RaiseAndSetIfChanged(ref _labelWidth, value);
    }

    private double? _rightContentWidth; 
    /// <summary>
    /// 右侧区域宽度
    /// </summary>
    public double? RightContentWidth
    {
        get => _rightContentWidth;
        private set => this.RaiseAndSetIfChanged(ref _rightContentWidth, value);
    }

    /// <summary>
    /// 设置右侧区域宽度,仅界面控件调用
    /// </summary>
    /// <param name="width"></param>
    internal void UpdateRightContentWidth(double? width)
    {
        RightContentWidth = width;
    }
    /// <summary>
    /// 
    /// </summary>
    public BasicControlViewModel()
    {
    }

    /// <summary>
    /// 根据宽度级别更新宽度
    /// </summary>
    public void SetWidth(string level)
    {
        var baseControlCfg = ConfigurationManager.BaseControlCfgs.Find(o => o.TotalWidthLevel == level);
        TotalWidth = baseControlCfg != null ? baseControlCfg.TotalWidth : 200;
        LabelWidth = baseControlCfg != null ? baseControlCfg.LableTotal : 100;

        this.RaisePropertyChanged(nameof(TotalWidth));
        this.RaisePropertyChanged(nameof(LabelWidth));

    }
}


/// <summary>
/// 通用值变更事件参数（传递旧值和新值）
/// </summary>
public class ValueChangedEventArgs : EventArgs
{
    /// <summary>
    /// 旧值
    /// </summary>
    public object? OldValue { get; }
    /// <summary>
    /// 
    /// </summary>
    public object? NewValue { get; }

    /// <summary>
    /// 新值
    /// </summary>
    public ValueChangedEventArgs(object? oldValue, object? newValue)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }
}

