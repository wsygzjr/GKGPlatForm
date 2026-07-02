using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Griffins;
using Griffins.UI;
using System;

namespace GriffinsGeneralTestMM
{
    public partial class UctlParamListView : UserControl
    {
        private UctlParamListViewModel _viewModel;
        public UctlParamListView()
        {
            InitializeComponent();
            _viewModel = new UctlParamListViewModel();
            DataContext = _viewModel;

            // 对接标准值变化事件
            _viewModel.AfterParamValModified += (s, e) =>
                AfterParamValModified?.Invoke(s, e);

            _viewModel.InitTestData();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public void Init(GenParamObjectDefInfo genObjectDefInfo)
        {
            _viewModel.Init(genObjectDefInfo);
        }

        // 保持你原有对外事件名不变（上层不用改）
        public event ParamValChangedEventHandler AfterParamValModified;

        public bool ReadOnly
        {
            get => _viewModel.ReadOnly;
            set => _viewModel.ReadOnly = value;
        }

        public GFBaseTypeParamValueList ParamValues
        {
            get => _viewModel.ParamValues;
            set => _viewModel.ParamValues = value;
        }
    }
}