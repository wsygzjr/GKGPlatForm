using GF_Gereric;
using GKG.SubMM;
using Griffins.CompUI.FixingStructure.CompUI_FixingStructure.PageType.RecipeCfgPage.AxisFixRecipeConfig;
using Griffins.ImeIOT;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;
using System;

namespace Griffins.CompUI.FixingStructure.CompUI_FixingStructure.PageType.RecipeCfgPage
{
    /// <summary>
    /// 电机固定机构配方页运行态入口，负责配方前后端数据转换。
    /// </summary>
    internal class RecipeCfgPageTypeRunTimeCompUI : PageTypeRunTimeCompUIBase
    {
        private AxisFixSubMachineModulesPPCfg _data = new();
        private AxisFixSubMachineModulesPPCfg _backendData = new();
        private AxisFixRecipeConfigPageTypeRunTimeCompUIView _view;
        private bool _isApplyingViewData;

        protected override void _OnInit()
        {
            _data = new AxisFixSubMachineModulesPPCfg();
            _backendData = new AxisFixSubMachineModulesPPCfg();
        }

        protected override PageTypeID _GetPageTypeID()
        {
            return ImeIOTConst.PPCfgPage;
        }

        protected override IPageTypeRunTimeCompUIView _GetPageTypeCompUIView(string viewID)
        {
            if (viewID != RecipeCfgPageTypeConst.ViewID_AxisFixRecipeConfig)
            {
                return null;
            }

            if (_view == null)
            {
                _view = new AxisFixRecipeConfigPageTypeRunTimeCompUIView(this.CallBack);
                ApplyViewData(_data);
                _view.AfterModified += OnAfterModified;
            }

            return _view;
        }

        protected override void _SetData(byte[] data)
        {
            if (data == null || data.Length == 0)
            {
                _backendData = new AxisFixSubMachineModulesPPCfg();
                _data = new AxisFixSubMachineModulesPPCfg();
            }
            else
            {
                _backendData = JsonObjConvert.FromJSonBytes<AxisFixSubMachineModulesPPCfg>(data)
                    ?? new AxisFixSubMachineModulesPPCfg();
                _data.CopyFrom(_backendData);
            }

            if (_view == null)
            {
                _view = new AxisFixRecipeConfigPageTypeRunTimeCompUIView(this.CallBack);
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

            _backendData = _data ?? new AxisFixSubMachineModulesPPCfg();
            return JsonObjConvert.ToJSonBytes(_backendData) ?? Array.Empty<byte>();
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

        private void ApplyViewData(AxisFixSubMachineModulesPPCfg data)
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
