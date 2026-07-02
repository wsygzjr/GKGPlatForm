using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Griffins;
using Griffins.ImeIOT;
using System;

namespace GriffinsGeneralTestMM
{
    public partial class UctlCabilityEventesView : UserControl
    {
        // 暴露ViewModel供外部访问（可选）
        public UctlCabilityEventesViewModel _viewModel => (UctlCabilityEventesViewModel)DataContext!;

        #region  原控件的外部接口
        /// <summary>
        /// 初始化事件列表（转发ViewModel的Init方法）
        /// </summary>
        public void Init(GenCabilityEventDefInfoList eventDefInfoes)
        {
            _viewModel.Init(eventDefInfoes);
        }
        #endregion

        public UctlCabilityEventesView()
        {
            InitializeComponent();

            //  延迟到控件Loaded事件后传递View引用（此时控件已挂载到可视化树）
            Loaded += UctlCabilityEventesView_Loaded;

            //  DataContext变化时重新传递引用（防止DataContext后期赋值）
            DataContextChanged += UctlCabilityEventesView_DataContextChanged; 
        }

        private void UctlCabilityEventesView_DataContextChanged(object? sender, EventArgs e)
        {
            // DataContext更新时重新传递View引用
            if (DataContext is UctlCabilityEventesViewModel vm)
            {
                vm.SetViewReference(this);
            }
        }

        private void UctlCabilityEventesView_Loaded(object? sender, RoutedEventArgs e)
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