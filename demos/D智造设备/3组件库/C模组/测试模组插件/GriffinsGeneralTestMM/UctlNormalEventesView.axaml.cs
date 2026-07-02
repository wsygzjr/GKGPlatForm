using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Griffins;
using Griffins.ImeIOT;
using System;

namespace GriffinsGeneralTestMM
{
    public partial class UctlNormalEventesView : UserControl
    {
        private readonly UctlNormalEventesViewModel _viewModel;

        public UctlNormalEventesView()
        {
            InitializeComponent();
            _viewModel = new UctlNormalEventesViewModel();
            DataContext = _viewModel;

            //  延迟到控件Loaded事件后传递View引用（此时控件已挂载到可视化树）
            Loaded += UctlNormalEventesView_Loaded;

            //  DataContext变化时重新传递引用（防止DataContext后期赋值）
            DataContextChanged += UctlNormalEventesView_DataContextChanged;
        }

        private void UctlNormalEventesView_DataContextChanged(object? sender, EventArgs e)
        {
            // DataContext更新时重新传递View引用
            if (DataContext is UctlNormalEventesViewModel vm)
            {
                vm.SetViewReference(this);
            }
        }

        private void UctlNormalEventesView_Loaded(object? sender, RoutedEventArgs e)
        {
            // 控件加载完成后传递View引用（此时可视化树已构建）
            if (_viewModel != null)
            {
                _viewModel.SetViewReference(this);
            }
        }
         
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        } 
    }
}