using Avalonia.Controls;
using Griffins.CompUI.EletronicManager.CompUI_EletronicManager.PageType.ElectricalMngPage.IODevice.ViewModels;
using ReactiveUI;
using System;
using System.Reactive.Linq;

namespace Griffins.CompUI.EletronicManager.CompUI_EletronicManager.PageType.ElectricalMngPage.IODevice.Views
{
    public partial class IODeviceEditWindow : Window
    {
        private IDisposable? _resultSub;

        public IODeviceEditWindow()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object? sender, EventArgs e)
        {
            _resultSub?.Dispose();
            _resultSub = null;

            if (DataContext is IODeviceEditWindowViewModel vm)
            {
                _resultSub = vm.WhenAnyValue(x => x.DialogResult)
                    .Where(result => result.HasValue)
                    .Subscribe(result => Close(result!.Value));
            }
        }
    }
}
