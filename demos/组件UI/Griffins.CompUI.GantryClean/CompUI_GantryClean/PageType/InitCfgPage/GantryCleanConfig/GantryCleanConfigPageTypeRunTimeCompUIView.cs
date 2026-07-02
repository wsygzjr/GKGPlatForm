using Griffins.CompUI.GantryClean.CompUI_GantryClean.PageType.InitCfgPage.GantryCleanConfig.ViewModels;
using Griffins.CompUI.GantryClean.CompUI_GantryClean.PageType.InitCfgPage.GantryCleanConfig.Views;
using GKG.SubMM;
using Griffins.Map.UI;
using System;
using Griffins.PF;
using Avalonia.Controls;

namespace Griffins.CompUI.GantryClean.CompUI_GantryClean.PageType.InitCfgPage.GantryCleanConfig
{
    internal class GantryCleanConfigPageTypeRunTimeCompUIView : IPageTypeRunTimeCompUIView
    {
        private readonly ICompUIRunTimeCallBack _callBack;
        private readonly GantryCleanConfigCompUIView _view;
        private readonly GantryCleanConfigCompUIViewModel _viewModel;

        private event EventHandler _afterModified;

        public GantryCleanConfigPageTypeRunTimeCompUIView(ICompUIRunTimeCallBack callBack)
        {
            _callBack = callBack;
            _view = new GantryCleanConfigCompUIView();
            _viewModel = new GantryCleanConfigCompUIViewModel();
            _view.DataContext = _viewModel;

            _viewModel.AfterModified += (s, e) => _afterModified?.Invoke(this, e);
        }

        public object View
        {
            get
            {
                RemoveViewFromParent();
                return _view;
            }
        }

        private void RemoveViewFromParent()
        {
            if (_view == null) return;

            if (_view.Parent is Panel panelParent)
            {
                if (panelParent.Children.Contains(_view))
                {
                    panelParent.Children.Remove(_view);
                }
            }
            else if (_view.Parent is ContentControl contentParent)
            {
                if (contentParent.Content == _view)
                {
                    contentParent.Content = null;
                }
            }
        }

        public OpMngCellID[] EditFuncMngCellIDs => null;

        public event EventHandler AfterModified
        {
            add => _afterModified += value;
            remove => _afterModified -= value;
        }

        public void SetReadOnly(bool readOnly)
        {
        }

        public void SetData(CleanParameters data)
        {
            _viewModel.Init(data);
        }

        public CleanParameters GetData()
        {
            return _viewModel.GetData();
        }
    }
}
