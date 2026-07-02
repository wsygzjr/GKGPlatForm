using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media;
using Avalonia.Layout;

namespace GKG.UI
{
    /// <summary>
    /// GroupBox组框自定义控件
    /// </summary>
    public class GroupBoxControl : Grid
    {
        private TextBlock _titleTextBlock;
        private ContentControl _contentControl; // 内容容器，绑定到依赖属性

        /// <summary>
        /// 标题文本依赖属性
        /// </summary>
        public static readonly StyledProperty<string> HeaderStrProperty =
            AvaloniaProperty.Register<GroupBoxControl, string>(
                nameof(HeaderStr),
                defaultValue: string.Empty,
                defaultBindingMode: BindingMode.TwoWay);

        /// <summary>
        /// GroupBox组框标题
        /// </summary>
        public string HeaderStr
        {
            get => GetValue(HeaderStrProperty);
            set => SetValue(HeaderStrProperty, value);
        }

        /// <summary>
        /// 动态内容依赖属性（替代固定的contentStackPanel，支持任意控件）
        /// </summary> 
        public static readonly StyledProperty<object> ContentAreaProperty =
            AvaloniaProperty.Register<GroupBoxControl, object>(
                nameof(ContentArea),
                defaultValue: new StackPanel { Spacing = 4 }, // 默认值保留原StackPanel
                defaultBindingMode: BindingMode.TwoWay);

        /// <summary>
        /// GroupBox的动态内容（可设置任意控件：StackPanel/Grid/Button等）
        /// </summary>
        public object ContentArea
        {
            get => GetValue(ContentAreaProperty);
            set => SetValue(ContentAreaProperty, value);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public GroupBoxControl() : base()
        {
            // 初始化标题 
            _titleTextBlock = new TextBlock
            {
                FontSize = 16,
                FontWeight = FontWeight.SemiBold,
                Foreground = new SolidColorBrush(Color.Parse("#BBE3F3")),
                Background = Brushes.White,
                Padding = new Thickness(8, 0),
                Margin = new Thickness(20, -7, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top
            };
            // 创建内容容器（ContentControl）
            _contentControl = new ContentControl
            {
                Margin = new Thickness(3, 3, 3, 3),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };

            InitializeControlLayout();  // 初始化布局（边框、内容容器）
            InitializeBindings();       // 绑定依赖属性
        }

        #region 初始化逻辑

        // 初始化整体布局（移除冗余的rootGrid，直接使用自身Grid）
        private void InitializeControlLayout()
        {
            // 根Grid的基础样式 
            this.Margin = new Thickness(5, 2, 5, 2);
            this.HorizontalAlignment = HorizontalAlignment.Stretch;  
            this.VerticalAlignment = VerticalAlignment.Stretch;   

            // 1. 添加底层边框
            var border = new Border
            {
                BorderBrush = Brushes.LightGray,
                BorderThickness = new Thickness(1.5),
                Padding = new Thickness(3, 0, 3, 3),
                CornerRadius = new CornerRadius(10),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };
            this.Children.Add(border);

            // 2. 添加标题TextBlock
            this.Children.Add(_titleTextBlock);

            // 3. 创建内容容器（ContentControl） 
            this.Children.Add(_contentControl);
        }

        // 绑定依赖属性（标题+动态内容）
        private void InitializeBindings()
        {
            // 绑定标题文本到HeaderStr
            _titleTextBlock.Bind(
                TextBlock.TextProperty,
                new Binding(nameof(HeaderStr)) { Source = this, Mode = BindingMode.TwoWay });

            // 绑定内容容器到ContentArea依赖属性（核心：动态内容的关键）
            _contentControl.Bind(
                ContentControl.ContentProperty,
                new Binding(nameof(ContentArea)) { Source = this, Mode = BindingMode.TwoWay });
        }
        #endregion
    }
}