using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Griffins.CompUI.ElectricalMngObj.FactoryCfgPage.ViewModels;
using ReactiveUI;
using System.Reactive.Linq;
using System;

namespace Griffins.CompUI.ElectricalMngObj.FactoryCfgPage.Views
{
    public partial class ControlCardEditWindow : Window
    {
        public ControlCardEditWindow()
        {
            InitializeComponent();
            // 监听 ViewModel 的 DialogResult 变化
            this.WhenAnyValue(x => x.DataContext!)
                .Where(ctx => ctx is ControlCardEditWindowViewModel)
                .Cast<ControlCardEditWindowViewModel>()
                .Subscribe(vm =>
                {
                    // 当 DialogResult 被设置时关闭窗口
                    vm.WhenAnyValue(x => x.DialogResult)
                        .Where(result => result.HasValue)
                        .Subscribe(result =>
                        {
                            Close(result!.Value); // 关闭并返回结果
                        });
                });

            //this.Loaded += onLoaded;
        }

        private void onLoaded(object? sender, RoutedEventArgs e)
        {
            if (DataContext is ControlCardEditWindowViewModel vm)
            {
                var controlCardKindModeltemp = vm.EditCopy.ControlCardKindModel.SelectedItem;
                vm.EditCopy.ControlCardKindModel.SelectedItem = null;
                vm.EditCopy.ControlCardKindModel.SelectedItem = controlCardKindModeltemp;

                var controlCardTypeModeltemp = vm.EditCopy.ControlCardTypeModel.SelectedItem;
                vm.EditCopy.ControlCardTypeModel.SelectedItem = null;
                vm.EditCopy.ControlCardTypeModel.SelectedItem = controlCardTypeModeltemp;
            }
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
