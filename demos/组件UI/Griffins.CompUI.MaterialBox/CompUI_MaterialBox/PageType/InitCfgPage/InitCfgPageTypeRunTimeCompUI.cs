using GF_Gereric;
using GKG;
using Griffins.CompUI.MaterialBox.CompUI_MaterialBox.PageType.InitCfgPage.MaterialBoxInitConfig;
using Griffins.ImeIOT;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;
using MaterialBoxSubMachineModules.FeedPort;
using System;
using System.Collections.Generic;
using BackendMaterialBoxInitCfg = GKG.SubMM.MaterialBoxSubMachineModulesInitCfg;

namespace Griffins.CompUI.MaterialBox.CompUI_MaterialBox.PageType.InitCfgPage
{
    internal class InitCfgPageTypeRunTimeCompUI : PageTypeRunTimeCompUIBase
    {
        private BackendMaterialBoxInitCfg _data = new();
        private BackendMaterialBoxInitCfg _backendData = new();
        private MaterialBoxInitConfigPageTypeRunTimeCompUIView _view;
        private bool _isApplyingViewData;

        protected override void _OnInit()
        {
            _data = new BackendMaterialBoxInitCfg();
            _backendData = new BackendMaterialBoxInitCfg();
            ApplyFactoryCfgToInitData(_data);
        }

        protected override PageTypeID _GetPageTypeID()
        {
            return ImeIOTConst.InitCfgPage;
        }

        protected override IPageTypeRunTimeCompUIView _GetPageTypeCompUIView(string viewID)
        {
            if (viewID != InitCfgPageTypeConst.ViewID_MaterialBoxInitConfig)
            {
                return null;
            }

            if (_view == null)
            {
                _view = new MaterialBoxInitConfigPageTypeRunTimeCompUIView(this.CallBack);
                ApplyViewData(_data ?? new BackendMaterialBoxInitCfg());
                _view.AfterModified += (_, e) =>
                {
                    if (_isApplyingViewData)
                        return;

                    _data = _view.GetData();
                    _backendData = _data ?? new BackendMaterialBoxInitCfg();
                    AfterDataModified?.Invoke(this, e);
                };
            }

            return _view;
        }

        protected override void _SetData(byte[] data)
        {
            _backendData = new BackendMaterialBoxInitCfg();
            _backendData.FromBytes(data);

            _data.CopyFrom(_backendData);
            //_data = new BackendMaterialBoxInitCfg();
            ApplyFactoryCfgToInitData(_data);

            if (_view == null)
            {
                _view = new MaterialBoxInitConfigPageTypeRunTimeCompUIView(this.CallBack);
                ApplyViewData(_data ?? new BackendMaterialBoxInitCfg());
                _view.AfterModified += (_, e) =>
                {
                    if (_isApplyingViewData)
                        return;

                    _data = _view.GetData();
                    _backendData = _data ?? new BackendMaterialBoxInitCfg();
                    AfterDataModified?.Invoke(this, e);
                };
            }

            if (_view != null)
            {
                ApplyViewData(_data);
            }
        }

        protected override byte[] _GetData()
        {
            if (_view != null)
            {
                _data = _view.GetData();
            }

            ApplyFactoryCfgToInitData(_data);
            _backendData = _data ?? new BackendMaterialBoxInitCfg();
            return _backendData.ToBytes();
        }

        protected override string _ExecViewCmd(string cmdID, string cmdParam)
        {
            return string.Empty;
        }

        protected override ISubPageRunTime _GetSubPageRunTime(SubPageKindInfoBase subPageKindInfo)
        {
            return null;
        }

        protected override bool _CheckDataValid(out string[] inValidMsg)
        {
            inValidMsg = Array.Empty<string>();
            return true;
        }

        private static void ApplyFactoryCfgToInitData(BackendMaterialBoxInitCfg data)
        {
            if (data == null)
                return;

            var factoryCfg = MaterialBoxSharedState.FactoryCfg;
            data.FeedingPortInitCfg ??= new FeedPortInitCfg();
            data.ReceivePortInitCfg ??= new FeedPortInitCfg();

            EnsureGuidListSize(data.FeedingPortInitCfg.SensorIOGuids, factoryCfg.FeedingPortFactoryCfg?.PortCount ?? 0);
            EnsureGuidListSize(data.ReceivePortInitCfg.SensorIOGuids, factoryCfg.ReceivePortFactoryCfg?.PortCount ?? 0);
        }

        private void ApplyViewData(BackendMaterialBoxInitCfg data)
        {
            if (_view == null)
                return;

            _isApplyingViewData = true;
            try
            {
                _view.SetData(data);
            }
            finally
            {
                _isApplyingViewData = false;
            }
        }

        private static void EnsureGuidListSize(List<Guid> list, int size)
        {
            while (list.Count < size)
            {
                list.Add(Guid.Empty);
            }
        }
    }
}
