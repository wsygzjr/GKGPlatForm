using GF_Gereric;
using Griffins.CompUI.Rail.CompUI_Rail.PageType.InitCfgPage.RailInit;
using GKG.MM;
using Griffins.ImeIOT;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;
using System;

namespace Griffins.CompUI.Rail.CompUI_Rail.PageType.InitCfgPage
{
    internal class InitCfgPageTypeRunTimeCompUI : PageTypeRunTimeCompUIBase
    {
        private IPageTypeRunTimeCompUIView _initView;
        private RailMachineModulesInitCfg _data;
        private bool _isApplyingViewData;

        protected override void _OnInit()
        {
            _data = new RailMachineModulesInitCfg();
        }

        protected override PageTypeID _GetPageTypeID()
        {
            return ImeIOTConst.InitCfgPage;
        }

        protected override IPageTypeRunTimeCompUIView _GetPageTypeCompUIView(string viewID)
        {
            if (viewID != InitCfgPageTypeConst.ViewID_RailInit)
                return null;

            if (_initView != null)
                _initView.AfterModified -= OnAfterModified;

            _initView = new RailInitPageTypeRunTimeCompUIView(CallBack);
            _initView.AfterModified += OnAfterModified;
            ApplyViewData(_data ?? new RailMachineModulesInitCfg());
            return _initView;
        }

        protected override void _SetData(byte[] data)
        {
            _data = data == null || data.Length == 0
                ? new RailMachineModulesInitCfg()
                : JsonObjConvert.FromJSonBytes<RailMachineModulesInitCfg>(data)
                    ?? new RailMachineModulesInitCfg();

            ApplyViewData(_data);
        }

        protected override byte[] _GetData()
        {
            if (_initView is RailInitPageTypeRunTimeCompUIView initView)
                _data = initView.GetData();

            return JsonObjConvert.ToJSonBytes(_data ?? new RailMachineModulesInitCfg());
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
                return;

            if (_initView is RailInitPageTypeRunTimeCompUIView initView)
                _data = initView.GetData();

            AfterDataModified?.Invoke(sender, e);
        }

        private void ApplyViewData(RailMachineModulesInitCfg data)
        {
            if (_initView is not RailInitPageTypeRunTimeCompUIView initView)
                return;

            _isApplyingViewData = true;
            try
            {
                initView.SetData(data);
            }
            finally
            {
                _isApplyingViewData = false;
            }
        }
    }
}
