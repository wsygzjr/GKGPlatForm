using Avalonia.Controls;
using GKG.SubMM;
using GKG.Vision;
using Griffins.CompUI.Vision.CompUI.PageType.FactoryCfgPage.VisionFactoryCfg.Models;
using Griffins.CompUI.Vision.CompUI.PageType.FactoryCfgPage.VisionFactoryCfg.ViewModels;
using Griffins.CompUI.Vision.CompUI.PageType.FactoryCfgPage.VisionFactoryCfg.Views;
using Griffins.Map.UI;
using Griffins.PF;

namespace Griffins.CompUI.Vision.CompUI.PageType.FactoryCfgPage
{
    internal class VisionFactoryCfgPageTypeRunTimeCompUIView : IPageTypeRunTimeCompUIView
    {
        private readonly ICompUIRunTimeCallBack _callBack;
        private readonly UserControl _view;
        private readonly VisionFactoryCfgViewModel _viewModel;
        private event EventHandler _afterModified;

        public VisionFactoryCfgPageTypeRunTimeCompUIView(ICompUIRunTimeCallBack callBack)
        {
            _callBack = callBack;
            _view = new VisionFactoryCfgView();
            _viewModel = new VisionFactoryCfgViewModel();
            _view.DataContext = _viewModel;

            _viewModel.AfterModified += (_, __) => _afterModified?.Invoke(this, EventArgs.Empty);
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

        public void SetData(VisionSubMachineModulesFactoryCfg model)
        {
            _viewModel.SetData(model);
        }

        public VisionSubMachineModulesFactoryCfg GetData()
        {
            return _viewModel.GetData();
        }
    }
}
