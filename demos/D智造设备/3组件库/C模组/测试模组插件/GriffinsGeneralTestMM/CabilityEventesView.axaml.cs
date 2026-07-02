using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Griffins;
using Griffins.ImeIOT;

namespace GriffinsGeneralTestMM
{
    public partial class CabilityEventesView : UserControl
    {
        private readonly CabilityEventesViewModel _viewModel;

        public CabilityEventesView()
        {
            InitializeComponent();
            _viewModel = new CabilityEventesViewModel();
            DataContext = _viewModel;
        }

        private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

        #region 转发ViewModel的方法
        public void Init(GenCabilityEventDefInfoList? eventDefInfoes) => _viewModel.Init(eventDefInfoes);
        #endregion

        #region 转发ViewModel的属性 
        public bool ReadOnly
        {
            get => _viewModel.ReadOnly;
            set => _viewModel.ReadOnly = value;
        }
        #endregion

        #region 转发ViewModel的事件
        public event ImeCabilityEventHandler? CabilityEvent
        {
            add => _viewModel.CabilityEvent += value;
            remove => _viewModel.CabilityEvent -= value;
        }
        #endregion
    }
}