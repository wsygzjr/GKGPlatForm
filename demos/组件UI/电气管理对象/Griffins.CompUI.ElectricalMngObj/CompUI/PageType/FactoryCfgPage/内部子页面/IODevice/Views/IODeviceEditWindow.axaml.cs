using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ReactiveUI;
using Avalonia.Interactivity;
using System.Reactive.Linq;
using System;
using Griffins.CompUI.ElectricalMngObj.FactoryCfgPage.ViewModels;

namespace Griffins.CompUI.ElectricalMngObj.FactoryCfgPage.Views
{
    public partial class IODeviceEditWindow : Window
    {
        public IODeviceEditWindow()
        {
            InitializeComponent();
            // 监听 ViewModel 的 DialogResult 变化
            this.WhenAnyValue(x => x.DataContext!)
                .Where(ctx => ctx is IODeviceEditViewModel)
                .Cast<IODeviceEditViewModel>()
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

        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
