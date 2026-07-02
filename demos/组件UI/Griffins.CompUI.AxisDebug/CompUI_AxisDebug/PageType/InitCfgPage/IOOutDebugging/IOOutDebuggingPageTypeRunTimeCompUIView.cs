using Griffins.CompUI.AxisDebug.CompUI_AxisDebug.PageType.InitCfgPage.IOOutDebugging.ViewModels;
using Griffins.CompUI.AxisDebug.CompUI_AxisDebug.PageType.InitCfgPage.IOOutDebugging.Views;
using Griffins.Map.UI;
using Griffins.PF;
using System;

namespace Griffins.CompUI.AxisDebug.CompUI_AxisDebug.PageType.InitCfgPage.IOOutDebugging
{
    internal class IOOutDebuggingPageTypeRunTimeCompUIView : IPageTypeRunTimeCompUIView
    {
        private readonly IOOutDebuggingCompUIView _view;
        private readonly IOOutDebuggingCompUIViewModel _viewModel;

        private event EventHandler _afterModified;

        public IOOutDebuggingPageTypeRunTimeCompUIView(ICompUIRunTimeCallBack callBack)
        {
            _view = new IOOutDebuggingCompUIView();
            _viewModel = new IOOutDebuggingCompUIViewModel();
            _view.DataContext = _viewModel;
            _viewModel.AfterModified += (s, e) => _afterModified?.Invoke(this, e);
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
