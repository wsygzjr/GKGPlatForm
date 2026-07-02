using Griffins.CompUI.AxisDebug.CompUI_AxisDebug.PageType.InitCfgPage.IOInDebugging.ViewModels;
using Griffins.CompUI.AxisDebug.CompUI_AxisDebug.PageType.InitCfgPage.IOInDebugging.Views;
using Griffins.Map.UI;
using Griffins.PF;
using System;

namespace Griffins.CompUI.AxisDebug.CompUI_AxisDebug.PageType.InitCfgPage.IOInDebugging
{
    internal class IOInDebuggingPageTypeRunTimeCompUIView : IPageTypeRunTimeCompUIView
    {
        private readonly IOInDebuggingCompUIView _view;
        private readonly IOInDebuggingCompUIViewModel _viewModel;

        private event EventHandler _afterModified;

        public IOInDebuggingPageTypeRunTimeCompUIView(ICompUIRunTimeCallBack callBack)
        {
            _view = new IOInDebuggingCompUIView();
            _viewModel = new IOInDebuggingCompUIViewModel();
            _view.DataContext = _viewModel;
        }

        public object View => _view;

        public OpMngCellID[] EditFuncMngCellIDs => null;

        public event EventHandler AfterModified
        {
            add => _afterModified += value;
            remove => _afterModified -= value;
        }

        public void SetReadOnly(bool readOnly)
        {
        }
    }
}
