using Avalonia;
using System.Text;
using System.Text.Json;
using Avalonia.Controls;
using Avalonia.Layout;
using ReactiveUI;

namespace GKG.Map.MapCell.Generic.Link.MapOprtCellParamCfgView
{
    internal static class LinkOprtCellCfgJson
    {
        internal static byte[] ToBytes<T>(T vm) => Encoding.UTF8.GetBytes(JsonSerializer.Serialize(vm));

        internal static T? FromBytes<T>(byte[] data)
        {
            if (data == null || data.Length == 0)
                return default;

            return JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(data));
        }
    }

    internal sealed class BrushInfoMapOprtCellParamViewModel : ReactiveObject
    {
        private string _textColor = "#FF0000FF";
        private string _hoverTextColor = "#FFFF0000";

        public string TextColor
        {
            get => _textColor;
            set => this.RaiseAndSetIfChanged(ref _textColor, value);
        }

        public string HoverTextColor
        {
            get => _hoverTextColor;
            set => this.RaiseAndSetIfChanged(ref _hoverTextColor, value);
        }

        public void FromBytes(byte[] data)
        {
            var temp = LinkOprtCellCfgJson.FromBytes<BrushInfoMapOprtCellParamViewModel>(data);
            if (temp == null)
                return;

            TextColor = temp.TextColor ?? TextColor;
            HoverTextColor = temp.HoverTextColor ?? HoverTextColor;
        }

        public byte[] ToBytes() => LinkOprtCellCfgJson.ToBytes(this);
    }

    internal sealed class CommonInfoMapOprtCellParamViewModel : ReactiveObject
    {
        private string _linkText = string.Empty;
        private string _address = string.Empty;

        public string LinkText
        {
            get => _linkText;
            set => this.RaiseAndSetIfChanged(ref _linkText, value);
        }

        public string Address
        {
            get => _address;
            set => this.RaiseAndSetIfChanged(ref _address, value);
        }

        public void FromBytes(byte[] data)
        {
            var temp = LinkOprtCellCfgJson.FromBytes<CommonInfoMapOprtCellParamViewModel>(data);
            if (temp == null)
                return;

            LinkText = temp.LinkText ?? LinkText;
            Address = temp.Address ?? Address;
        }

        public byte[] ToBytes() => LinkOprtCellCfgJson.ToBytes(this);
    }

    internal sealed class BrushInfoMapOprtCellParamView : UserControl
    {
        public BrushInfoMapOprtCellParamView()
        {
            Content = new StackPanel
            {
                Margin = new Thickness(8),
                Spacing = 8,
                Children =
                {
                    CreateLabeledTextBox("文本颜色", nameof(BrushInfoMapOprtCellParamViewModel.TextColor)),
                    CreateLabeledTextBox("鼠标悬停颜色", nameof(BrushInfoMapOprtCellParamViewModel.HoverTextColor))
                }
            };
        }

        private static Avalonia.Controls.Control CreateLabeledTextBox(string title, string propertyName)
        {
            var textBox = new TextBox();
            textBox.Bind(TextBox.TextProperty, new Avalonia.Data.Binding(propertyName));
            return new StackPanel
            {
                Spacing = 4,
                Children =
                {
                    new TextBlock { Text = title },
                    textBox
                }
            };
        }
    }

    internal sealed class CommonInfoMapOprtCellParamView : UserControl
    {
        public CommonInfoMapOprtCellParamView()
        {
            Content = new StackPanel
            {
                Margin = new Thickness(8),
                Spacing = 8,
                Children =
                {
                    CreateLabeledTextBox("链接文本", nameof(CommonInfoMapOprtCellParamViewModel.LinkText)),
                    CreateLabeledTextBox("跳转地址", nameof(CommonInfoMapOprtCellParamViewModel.Address))
                }
            };
        }

        private static Avalonia.Controls.Control CreateLabeledTextBox(string title, string propertyName)
        {
            var textBox = new TextBox();
            textBox.Bind(TextBox.TextProperty, new Avalonia.Data.Binding(propertyName));
            return new StackPanel
            {
                Spacing = 4,
                Children =
                {
                    new TextBlock { Text = title },
                    textBox
                }
            };
        }
    }

    internal sealed class BrushInfoMapOprtCellParamCfgView : IMapOprtCellParamCfgView
    {
        private readonly BrushInfoMapOprtCellParamViewModel _vm = new();
        private readonly BrushInfoMapOprtCellParamView _view;

        public BrushInfoMapOprtCellParamCfgView()
        {
            _view = new BrushInfoMapOprtCellParamView { DataContext = _vm };
        }

        public object View => _view;
        public void SetData(byte[] data) => _vm.FromBytes(data);
        public byte[] GetData() => _vm.ToBytes();

        object IMapOprtCellParamCfgView.View => View;
        void IMapOprtCellParamCfgView.SetData(byte[] data) => SetData(data);
        byte[] IMapOprtCellParamCfgView.GetData() => GetData();
    }

    internal sealed class CommonInfoMapOprtCellParamCfgView : IMapOprtCellParamCfgView
    {
        private readonly CommonInfoMapOprtCellParamViewModel _vm = new();
        private readonly CommonInfoMapOprtCellParamView _view;

        public CommonInfoMapOprtCellParamCfgView()
        {
            _view = new CommonInfoMapOprtCellParamView { DataContext = _vm };
        }

        public object View => _view;
        public void SetData(byte[] data) => _vm.FromBytes(data);
        public byte[] GetData() => _vm.ToBytes();

        object IMapOprtCellParamCfgView.View => View;
        void IMapOprtCellParamCfgView.SetData(byte[] data) => SetData(data);
        byte[] IMapOprtCellParamCfgView.GetData() => GetData();
    }
}
