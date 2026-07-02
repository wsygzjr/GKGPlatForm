using Avalonia.Media;
using GKG.Map.MapCell.Generic.Control.MapCell_ComboBox.Models;
using Griffins;
using PropertyModels.ComponentModel;
using System;
using System.ComponentModel;

namespace GKG.Map.MapCell.Generic.Control.MapCell_ComboBox
{
    // 专门给右侧属性面板展示用的字体枚举
    public enum ComboBoxFontType
    {
        [Description("微软雅黑")] Microsoft_YaHei,
        [Description("宋体")] SimSun,
        [Description("黑体")] SimHei,
        [Description("楷体")] KaiTi,
        [Description("Arial")] Arial,
        [Description("Consolas")] Consolas
    }

    [Serializable]
    [MapPropertyOrder]
    [CategoryPriority("基础数据", 1)]
    [CategoryPriority("样式外观", 2)]
    [CategoryPriority("布局", 3)]
    public class ComboBoxPropertyModelEdit : ControlCellPropertyModelEdit
    {
        #region 1. 基础数据 (Data & Behavior)

        // 【核心修复】：去掉默认的 Item1 和 Item2，初始化为空！
        // 这样 JSON 引擎在读取时，就不会发生追加叠加的 Bug 了。
        private RunModeList _items = new RunModeList();
        private string _itemsUIText = "";
        private bool _isSyncingItems = false;

        private string _selectedItem = string.Empty;
        private string _placeholderText = string.Empty;
        private bool _isEditable;

        [Browsable(false)]
        public RunModeList Items
        {
            get => _items;
            set
            {
                if (SetProperty(ref _items, value))
                {
                    if (!_isSyncingItems)
                    {
                        _itemsUIText = string.Join(",", _items ?? new RunModeList());
                        RaisePropertyChanged(nameof(ItemsUI));
                    }
                }
            }
        }

        [DisplayName("下拉项集合")]
        [Category("基础数据")]
        [PropertySortOrder(1)]
        [System.Text.Json.Serialization.JsonIgnore]
        public string ItemsUI
        {
            get => _itemsUIText;
            set
            {
                if (_itemsUIText != value)
                {
                    _itemsUIText = value;
                    RaisePropertyChanged(nameof(ItemsUI));

                    _isSyncingItems = true;
                    try
                    {
                        var newItems = new RunModeList();
                        var separators = new[] { ",", "，", ";", "；", "|", "\r\n", "\n" };
                        foreach (var item in (value ?? "").Split(separators, StringSplitOptions.RemoveEmptyEntries))
                        {
                            var trimmed = item.Trim();
                            if (!string.IsNullOrEmpty(trimmed)) newItems.Add(trimmed);
                        }
                        Items = newItems;
                    }
                    finally
                    {
                        _isSyncingItems = false;
                    }
                }
            }
        }

        [DisplayName("当前选中项")]
        [Category("基础数据")]
        [PropertySortOrder(2)]
        public string SelectedItem
        {
            get => _selectedItem;
            set => SetProperty(ref _selectedItem, value);
        }

        [DisplayName("占位提示文案")]
        [Category("基础数据")]
        [PropertySortOrder(3)]
        public string PlaceholderText
        {
            get => _placeholderText;
            set => SetProperty(ref _placeholderText, value ?? string.Empty);
        }

        [DisplayName("允许输入")]
        [Category("基础数据")]
        [PropertySortOrder(4)]
        public bool IsEditable
        {
            get => _isEditable;
            set => SetProperty(ref _isEditable, value);
        }

        #endregion

        #region 2. 样式外观 (Style & Text)

        private Color _backgroundColor = Colors.White;
        private Color _foregroundColor = Colors.Black;
        private Color _borderBrush = Colors.Gray;

        // ------------------------------------------------------------------
        // 这三个属性对面板不可见，专门用来把 Color 变成安全的 Hex 字符串存入文件
        // ------------------------------------------------------------------
        private string _bgColorStr;
        [Browsable(false)]
        public string BgColorStr
        {
            get => _bgColorStr ?? _backgroundColor.ToString();
            set => _bgColorStr = value;
        }

        private string _fgColorStr;
        [Browsable(false)]
        public string FgColorStr
        {
            get => _fgColorStr ?? _foregroundColor.ToString();
            set => _fgColorStr = value;
        }

        private string _bdColorStr;
        [Browsable(false)]
        public string BdColorStr
        {
            get => _bdColorStr ?? _borderBrush.ToString();
            set => _bdColorStr = value;
        }
        // ------------------------------------------------------------------

        [DisplayName("背景色")]
        [Category("样式外观")]
        [PropertySortOrder(1)]
        [System.Text.Json.Serialization.JsonIgnore]
        public Color BackgroundColor
        {
            get => _backgroundColor;
            set => SetProperty(ref _backgroundColor, value);
        }

        [DisplayName("文字颜色")]
        [Category("样式外观")]
        [PropertySortOrder(2)]
        [System.Text.Json.Serialization.JsonIgnore]
        public Color ForegroundColor
        {
            get => _foregroundColor;
            set => SetProperty(ref _foregroundColor, value);
        }

        [DisplayName("边框颜色")]
        [Category("样式外观")]
        [PropertySortOrder(3)]
        [System.Text.Json.Serialization.JsonIgnore]
        public Color BorderBrush
        {
            get => _borderBrush;
            set => SetProperty(ref _borderBrush, value);
        }

        private string _borderThickness = "1";
        private int _fontSize = 14;
        private bool _isBold;
        private bool _isItalic;
        private string _fontFamily = "Microsoft YaHei";
        private double _opacity = 1.0;

        [DisplayName("边框厚度")]
        [Category("样式外观")]
        [PropertySortOrder(4)]
        public string BorderThickness
        {
            get => _borderThickness;
            set => SetProperty(ref _borderThickness, value ?? "1");
        }

        [DisplayName("字号")]
        [Category("样式外观")]
        [PropertySortOrder(5)]
        public int FontSize
        {
            get => _fontSize;
            set => SetProperty(ref _fontSize, Math.Max(1, value));
        }

        [Browsable(false)]
        public string FontFamily
        {
            get => _fontFamily;
            set
            {
                if (SetProperty(ref _fontFamily, value ?? "Microsoft YaHei")) RaisePropertyChanged(nameof(FontFamilyUI));
            }
        }

        [DisplayName("字体族")]
        [Category("样式外观")]
        [PropertySortOrder(6)]
        [System.Text.Json.Serialization.JsonIgnore]
        public ComboBoxFontType FontFamilyUI
        {
            get
            {
                return _fontFamily switch
                {
                    "SimSun" => ComboBoxFontType.SimSun,
                    "SimHei" => ComboBoxFontType.SimHei,
                    "KaiTi" => ComboBoxFontType.KaiTi,
                    "Arial" => ComboBoxFontType.Arial,
                    "Consolas" => ComboBoxFontType.Consolas,
                    _ => ComboBoxFontType.Microsoft_YaHei,
                };
            }
            set
            {
                string fontName = value switch
                {
                    ComboBoxFontType.SimSun => "SimSun",
                    ComboBoxFontType.SimHei => "SimHei",
                    ComboBoxFontType.KaiTi => "KaiTi",
                    ComboBoxFontType.Arial => "Arial",
                    ComboBoxFontType.Consolas => "Consolas",
                    _ => "Microsoft YaHei",
                };
                FontFamily = fontName;
            }
        }

        [DisplayName("加粗")]
        [Category("样式外观")]
        [PropertySortOrder(7)]
        public bool IsBold
        {
            get => _isBold;
            set => SetProperty(ref _isBold, value);
        }

        [DisplayName("斜体")]
        [Category("样式外观")]
        [PropertySortOrder(8)]
        public bool IsItalic
        {
            get => _isItalic;
            set => SetProperty(ref _isItalic, value);
        }

        [DisplayName("透明度(0-1)")]
        [Category("样式外观")]
        [PropertySortOrder(9)]
        public double Opacity
        {
            get => _opacity;
            set => SetProperty(ref _opacity, Math.Max(0.0, Math.Min(1.0, value)));
        }

        #endregion

        #region 3. 布局 (Layout)

        private string _margin = "0";
        private string _padding = "2";
        private double _maxDropDownHeight = 300;

        [DisplayName("外边距")]
        [Category("布局")]
        [PropertySortOrder(1)]
        public string Margin
        {
            get => _margin;
            set => SetProperty(ref _margin, value ?? "0");
        }

        [DisplayName("内边距")]
        [Category("布局")]
        [PropertySortOrder(2)]
        public string Padding
        {
            get => _padding;
            set => SetProperty(ref _padding, value ?? "2");
        }

        [DisplayName("下拉框最大高度")]
        [Category("布局")]
        [PropertySortOrder(3)]
        public double MaxDropDownHeight
        {
            get => _maxDropDownHeight;
            set => SetProperty(ref _maxDropDownHeight, Math.Max(0, value));
        }

        #endregion

        #region 反序列化兼容与拷贝

        public override bool SetPropertyValue(string propertyID, MapCellPropValue propertyVal)
        {
            if (string.Compare(propertyID, nameof(Items)) == 0)
            {
                if (propertyVal?.Val is ObjectValue_Json ov)
                {
                    var newList = new RunModeList();
                    ((IGriffinsBaseValue)newList).PopulateFromBaseValue(new GriffinsBaseValue(ov));
                    Items = newList;
                }
                return true;
            }
            if (string.Compare(propertyID, nameof(SelectedItem)) == 0)
            {
                SelectedItem = propertyVal?.ToPrimitiveValue<string>() ?? string.Empty;
                return true;
            }
            return base.SetPropertyValue(propertyID, propertyVal);
        }

        // 统一的安全颜色解析提取器
        private Color SafeParseColor(string hex, Color fallback)
        {
            if (!string.IsNullOrEmpty(hex) && Color.TryParse(hex, out var c)) return c;
            return fallback;
        }

        public void CopyFrom(ComboBoxPropertyModelEdit source)
        {
            if (source == null) return;
            base.CopyFrom(source);

            // 1. 基础数据
            var newItems = new RunModeList();
            if (source.Items != null) newItems.AddRange(source.Items);
            Items = newItems;

            SelectedItem = source.SelectedItem;
            PlaceholderText = source.PlaceholderText;
            IsEditable = source.IsEditable;

            // 2. 样式外观（从最安全的字符串通道解析出真实颜色）
            BackgroundColor = SafeParseColor(source.BgColorStr, source.BackgroundColor);
            ForegroundColor = SafeParseColor(source.FgColorStr, source.ForegroundColor);
            BorderBrush = SafeParseColor(source.BdColorStr, source.BorderBrush);

            BorderThickness = source.BorderThickness;
            FontSize = source.FontSize;
            FontFamily = source.FontFamily;
            IsBold = source.IsBold;
            IsItalic = source.IsItalic;
            Opacity = source.Opacity;

            // 3. 布局
            Margin = source.Margin;
            Padding = source.Padding;
            MaxDropDownHeight = source.MaxDropDownHeight;
        }
        #endregion
    }
}