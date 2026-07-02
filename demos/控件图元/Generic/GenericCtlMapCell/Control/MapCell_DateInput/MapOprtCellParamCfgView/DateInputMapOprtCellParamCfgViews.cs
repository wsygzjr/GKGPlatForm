using System.Text;
using System.Text.Json;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using ReactiveUI;

namespace GKG.Map.MapCell.Generic.Control.MapCell_DateInput.MapOprtCellParamCfgView
{
    internal static class DateInputOprtCellCfgJson
    {
        internal static byte[] ToBytes<T>(T vm) => Encoding.UTF8.GetBytes(JsonSerializer.Serialize(vm));

        internal static T FromBytes<T>(byte[] data)
        {
            if (data == null || data.Length == 0)
                return default;

            return JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(data));
        }
    }

    internal sealed class CommonInfoMapOprtCellParamViewModel : ReactiveObject
    {
        private string _selectedDate = string.Empty;

        public string SelectedDate
        {
            get => _selectedDate;
            set => this.RaiseAndSetIfChanged(ref _selectedDate, value);
        }

        public void FromBytes(byte[] data)
        {
            CommonInfoMapOprtCellParamViewModel temp = DateInputOprtCellCfgJson.FromBytes<CommonInfoMapOprtCellParamViewModel>(data);
            if (temp == null)
                return;

            SelectedDate = temp.SelectedDate ?? string.Empty;
        }

        public byte[] ToBytes() => DateInputOprtCellCfgJson.ToBytes(this);
    }

    internal sealed class CommonInfoMapOprtCellParamView : UserControl
    {
        public CommonInfoMapOprtCellParamView()
        {
            TextBox textBox = new();
            textBox.Bind(TextBox.TextProperty, new Binding(nameof(CommonInfoMapOprtCellParamViewModel.SelectedDate)) { Mode = BindingMode.TwoWay });

            Content = new StackPanel
            {
                Margin = new Thickness(8),
                Spacing = 8,
                Children =
                {
                    new TextBlock { Text = "选中日期" },
                    textBox
                }
            };
        }
    }

    internal sealed class CommonInfoMapOprtCellParamCfgView : IMapOprtCellParamCfgView
    {
        private readonly CommonInfoMapOprtCellParamViewModel _vm = new();
        private readonly CommonInfoMapOprtCellParamView _view;

        public CommonInfoMapOprtCellParamCfgView()
        {
            _view = new CommonInfoMapOprtCellParamView { DataContext = _vm };
        }

        object IMapOprtCellParamCfgView.View => _view;

        void IMapOprtCellParamCfgView.SetData(byte[] data) => _vm.FromBytes(data);

        byte[] IMapOprtCellParamCfgView.GetData() => _vm.ToBytes();
    }
}
