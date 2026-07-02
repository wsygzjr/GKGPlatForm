using Avalonia.Controls;
using Griffins.CompUI.EletronicManager.CompUI_EletronicManager.PageType.ElectricalMngPage.ControlCard.ViewModels;
using ReactiveUI;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Griffins.CompUI.EletronicManager.CompUI_EletronicManager.PageType.ElectricalMngPage.ControlCard.Views
{
    public partial class ControlCardEditWindow : Window
    {
        private IDisposable? _resultSub;

        public ControlCardEditWindow()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object? sender, EventArgs e)
        {
            _resultSub?.Dispose();
            _resultSub = null;

            if (DataContext is ControlCardEditWindowViewModel vm)
            {
                _resultSub = vm.WhenAnyValue(x => x.DialogResult)
                    .Where(result => result.HasValue)
                    .Subscribe(result => Close(result!.Value));
            }
        }
    }
}
