using GF_Gereric;
using GKG.SubMM;
using Griffins.CompUI.FixingStructure.CompUI_FixingStructure.PageType.InitCfgPage.AxisFixInitConfig;
using Griffins.ImeIOT;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;
using System;

namespace Griffins.CompUI.FixingStructure.CompUI_FixingStructure.PageType.InitCfgPage
{
    /// <summary>
    /// 电机固定机构初始化页运行态入口，负责初始化数据与视图的同步。
    /// </summary>
    internal class InitCfgPageTypeRunTimeCompUI : PageTypeRunTimeCompUIBase
    {
        private AxisFixSubMachineModulesInitCfg _data = new();
        private AxisFixSubMachineModulesInitCfg _backendData = new();
        private AxisFixInitConfigPageTypeRunTimeCompUIView _view;
        private bool _isApplyingViewData;

        protected override void _OnInit()
        {
            _data = new AxisFixSubMachineModulesInitCfg();
            _backendData = new AxisFixSubMachineModulesInitCfg();
        }

        protected override PageTypeID _GetPageTypeID()
        {
            return ImeIOTConst.InitCfgPage;
        }

        protected override IPageTypeRunTimeCompUIView _GetPageTypeCompUIView(string viewID)
        {
            if (viewID != InitCfgPageTypeConst.ViewID_AxisFixInitConfig)
            {
                return null;
            }

            if (_view == null)
            {
                _view = new AxisFixInitConfigPageTypeRunTimeCompUIView(this.CallBack);
                ApplyViewData(_data);
                _view.AfterModified += OnAfterModified;
            }

            return _view;
        }

        protected override void _SetData(byte[] data)
        {
            _backendData = new AxisFixSubMachineModulesInitCfg();
            if (data != null && data.Length > 0)
            {
                _backendData.FromBytes(data);
            }

            _data.CopyFrom(_backendData);

            if (_view == null)
            {
                _view = new AxisFixInitConfigPageTypeRunTimeCompUIView(this.CallBack);
                _view.AfterModified += OnAfterModified;
            }

            ApplyViewData(_data);
        }

        protected override byte[] _GetData()
        {
            if (_view != null)
            {
                _data = _view.GetData();
            }

            _backendData = _data ?? new AxisFixSubMachineModulesInitCfg();
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

        private void OnAfterModified(object sender, EventArgs e)
        {
            if (_isApplyingViewData)
            {
                return;
            }

            if (_view != null)
            {
                _data = _view.GetData();
                _backendData.CopyFrom(_data);
            }

            AfterDataModified?.Invoke(sender, e);
        }

        private void ApplyViewData(AxisFixSubMachineModulesInitCfg data)
        {
            if (_view == null)
            {
                return;
            }

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
    }
}
