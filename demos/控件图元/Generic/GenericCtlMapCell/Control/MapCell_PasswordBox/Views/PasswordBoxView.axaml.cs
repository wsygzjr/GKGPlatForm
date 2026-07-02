using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Input;
using Avalonia.Media;
using GKG.Map.MapCell.Generic.Control.Lable;
using GKG.Map.MapCell.Generic.PasswordBox.ViewModel;
using System;
using System.ComponentModel;
using System.Globalization;

namespace GKG.Map.MapCell.Generic.PasswordBox.View;

public partial class PasswordBoxView : UserControl
{
    private PasswordBoxViewModel _viewModel;

    public PasswordBoxView()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }

    public PasswordBoxView(bool designTime) : this()
    {
    }

    private void OnDataContextChanged(object sender, EventArgs e)
    {
        if (DataContext is PasswordBoxViewModel vm)
        {
            _viewModel = vm;
            
            // 初始化绑定
            UpdateFromViewModel(vm);
            
            // 监听属性变化
            vm.PropertyChanged += OnViewModelPropertyChanged;
        }
    }

    private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (sender is PasswordBoxViewModel vm)
        {
            UpdateFromViewModel(vm);
        }
    }

    private void UpdateFromViewModel(PasswordBoxViewModel vm)
    {
            // 获取内部的 PasswordInput 控件
            var passwordInput = this.FindControl<TextBox>("PasswordInput");
            
            if (vm.LayoutInfo != null)
            {
                // 设置约束
                MinWidth = vm.LayoutInfo.MinWidth;
                MaxWidth = vm.LayoutInfo.MaxWidth;
                MinHeight = vm.LayoutInfo.MinHeight;
                MaxHeight = vm.LayoutInfo.MaxHeight;
                
                if (passwordInput != null)
                {
                    // 设置内部控件的水平对齐（相对于外层容器/绿点外边框）
                    passwordInput.HorizontalAlignment = vm.LayoutInfo.HorizontalAlignment switch
                    {
                        HorizontalAlignType.Left => Avalonia.Layout.HorizontalAlignment.Left,
                        HorizontalAlignType.Center => Avalonia.Layout.HorizontalAlignment.Center,
                        HorizontalAlignType.Right => Avalonia.Layout.HorizontalAlignment.Right,
                        HorizontalAlignType.Stretch => Avalonia.Layout.HorizontalAlignment.Stretch,
                        _ => Avalonia.Layout.HorizontalAlignment.Stretch
                    };
                    
                    // 设置内部控件的垂直对齐（相对于外层容器/绿点外边框）
                    passwordInput.VerticalAlignment = vm.LayoutInfo.VerticalAlignment switch
                    {
                        VerticalAlignType.Top => Avalonia.Layout.VerticalAlignment.Top,
                        VerticalAlignType.Center => Avalonia.Layout.VerticalAlignment.Center,
                        VerticalAlignType.Bottom => Avalonia.Layout.VerticalAlignment.Bottom,
                        VerticalAlignType.Stretch => Avalonia.Layout.VerticalAlignment.Stretch,
                        _ => Avalonia.Layout.VerticalAlignment.Stretch
                    };
                    // 设置内部控件的外边距
                    passwordInput.Margin = new Avalonia.Thickness(
                        vm.LayoutInfo.MarginLeft,
                        vm.LayoutInfo.MarginTop,
                        vm.LayoutInfo.MarginRight,
                        vm.LayoutInfo.MarginBottom);
                }
            }
            
            if (vm.AppearanceInfo != null)
            {
                Opacity = vm.AppearanceInfo.Opacity;
            }
            
            if (vm.CommonInfo != null)
            {
                Cursor = ConvertCursor(vm.CommonInfo.CursorType);
            }
            
            // 更新文本相关属性
            if (vm.TextInfo != null && passwordInput != null)
            {
                passwordInput.FontFamily = new FontFamily(ConvertFontFamilyType(vm.TextInfo.FontFamily));
                passwordInput.FontSize = vm.TextInfo.FontSize;
                passwordInput.FontWeight = vm.TextInfo.IsBold ? FontWeight.Bold : FontWeight.Normal;
                passwordInput.FontStyle = vm.TextInfo.IsItalic ? FontStyle.Italic : FontStyle.Normal;
            }
    }
    
    private static string ConvertFontFamilyType(FontFamilyType fontType)
    {
        return fontType switch
        {
            FontFamilyType.MicrosoftYaHei => "Microsoft YaHei",
            FontFamilyType.SimSun => "SimSun",
            FontFamilyType.SimHei => "SimHei",
            FontFamilyType.KaiTi => "KaiTi",
            FontFamilyType.FangSong => "FangSong",
            FontFamilyType.NSimSun => "NSimSun",
            FontFamilyType.Arial => "Arial",
            FontFamilyType.TimesNewRoman => "Times New Roman",
            FontFamilyType.Consolas => "Consolas",
            FontFamilyType.SegoeUI => "Segoe UI",
            _ => "Microsoft YaHei"
        };
    }

    private static Cursor ConvertCursor(CursorType cursorType)
    {
        return cursorType switch
        {
            CursorType.Arrow => new Cursor(StandardCursorType.Arrow),
            CursorType.Ibeam => new Cursor(StandardCursorType.Ibeam),
            CursorType.Wait => new Cursor(StandardCursorType.Wait),
            CursorType.Cross => new Cursor(StandardCursorType.Cross),
            CursorType.UpArrow => new Cursor(StandardCursorType.UpArrow),
            CursorType.SizeWestEast => new Cursor(StandardCursorType.SizeWestEast),
            CursorType.SizeNorthSouth => new Cursor(StandardCursorType.SizeNorthSouth),
            CursorType.SizeAll => new Cursor(StandardCursorType.SizeAll),
            CursorType.No => new Cursor(StandardCursorType.No),
            CursorType.Hand => new Cursor(StandardCursorType.Hand),
            CursorType.AppStarting => new Cursor(StandardCursorType.AppStarting),
            CursorType.Help => new Cursor(StandardCursorType.Help),
            _ => new Cursor(StandardCursorType.Ibeam)
        };
    }
}

public class ColorToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Color color)
            return new SolidColorBrush(color);
        return new SolidColorBrush(Colors.Transparent);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is SolidColorBrush brush)
            return brush.Color;
        return Colors.Transparent;
    }
}

public class PasswordCharConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool isVisible && isVisible)
            return '\0';
        return '●';
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is char c && c == '\0';
    }
}

public class BorderThicknessConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is PasswordBoxAppearanceInfo info)
            return new Thickness(info.BorderThicknessLeft, info.BorderThicknessTop, info.BorderThicknessRight, info.BorderThicknessBottom);
        return new Thickness(1);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return null;
    }
}

public class BoolToFontWeightConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool isBold && isBold)
            return FontWeight.Bold;
        return FontWeight.Normal;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is FontWeight fw && fw == FontWeight.Bold;
    }
}

public class BoolToFontStyleConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool isItalic && isItalic)
            return FontStyle.Italic;
        return FontStyle.Normal;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is FontStyle fs && fs == FontStyle.Italic;
    }
}

public class FontFamilyTypeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is FontFamilyType fontType)
        {
            return fontType switch
            {
                FontFamilyType.MicrosoftYaHei => "Microsoft YaHei",
                FontFamilyType.SimSun => "SimSun",
                FontFamilyType.SimHei => "SimHei",
                FontFamilyType.KaiTi => "KaiTi",
                FontFamilyType.FangSong => "FangSong",
                FontFamilyType.NSimSun => "NSimSun",
                FontFamilyType.Arial => "Arial",
                FontFamilyType.TimesNewRoman => "Times New Roman",
                FontFamilyType.Consolas => "Consolas",
                FontFamilyType.SegoeUI => "Segoe UI",
                _ => "Microsoft YaHei"
            };
        }
        return "Microsoft YaHei";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return FontFamilyType.MicrosoftYaHei;
    }
}
