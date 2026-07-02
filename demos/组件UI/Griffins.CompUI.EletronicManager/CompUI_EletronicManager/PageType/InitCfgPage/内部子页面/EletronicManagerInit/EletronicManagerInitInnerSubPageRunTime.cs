using GF_Gereric;
using GKG.SubMM;
using Griffins.CompUI.EletronicManager.CompUI_EletronicManager.PageType.InitCfgPage.EletronicManagerInit.ViewModels;
using Griffins.CompUI.EletronicManager.CompUI_EletronicManager.PageType.InitCfgPage.EletronicManagerInit.Views;
using Griffins.Map.UI;
using System;

namespace Griffins.CompUI.EletronicManager.CompUI_EletronicManager.PageType.InitCfgPage
{
    public class EletronicManagerInitInnerSubPageRunTime : IInnerSubPageRunTime
    {
        private ICompUIRunTimeCallBack _callBack;
        private EletronicManagerInitCompUIViewModel _viewModel;
        private EventHandler _afterDataModified;
        private bool _isApplyingViewData;
        private EletronicManagerSubMachineModulesInitCfg _data = new();

        public void Init(ICompUIRunTimeCallBack callBack)
        {
            _callBack = callBack;
            _viewModel = new EletronicManagerInitCompUIViewModel(callBack);
            _viewModel.AfterModified += OnViewModelAfterModified;
        }

        void ISubPageRunTime.Init(byte[] viewCfgInfo)
        {
        }

        object ISubPageRunTime.View
        {
            get
            {
                var view = new EletronicManagerInitCompUIView();
                if (_viewModel == null && _callBack != null)
                {
                    _viewModel = new EletronicManagerInitCompUIViewModel(_callBack);
                    _viewModel.AfterModified += OnViewModelAfterModified;
                }

                if (_viewModel != null)
                {
                    view.DataContext = _viewModel;
                    ApplyViewData(_data);
                }

                return view;
            }
        }

        void ISubPageRunTime.SetViewCfgInfo(byte[] viewCfgInfo)
        {
        }

        event EventHandler IInnerSubPageRunTime.AfterModified
        {
            add => _afterDataModified += value;
            remove => _afterDataModified -= value;
        }

        void IInnerSubPageRunTime.OnInit()
        {
        }

        void IInnerSubPageRunTime.SetData(byte[] data)
        {
            _data = data == null || data.Length == 0
                ? new EletronicManagerSubMachineModulesInitCfg()
                : JsonObjConvert.FromJSonBytes<EletronicManagerSubMachineModulesInitCfg>(data)
                    ?? new EletronicManagerSubMachineModulesInitCfg();

            ApplyViewData(_data);
        }

        byte[] IInnerSubPageRunTime.GetData()
        {
            if (_viewModel != null)
                _data = _viewModel.GetData();

            return JsonObjConvert.ToJSonBytes(_data ?? new EletronicManagerSubMachineModulesInitCfg());
        }

        public bool CheckDataValid(out string[] inValidMsg)
        {
            inValidMsg = Array.Empty<string>();
            return true;
        }

        private void ApplyViewData(EletronicManagerSubMachineModulesInitCfg data)
        {
            if (_viewModel == null)
                return;

            _isApplyingViewData = true;
            try
            {
                _viewModel.SetData(data);
            }
            finally
            {
                _isApplyingViewData = false;
            }
        }

        private void OnViewModelAfterModified(object sender, EventArgs e)
        {
            if (_isApplyingViewData)
                return;

            if (_viewModel != null)
                _data = _viewModel.GetData();

            _afterDataModified?.Invoke(sender, e);
        }
    }
}
