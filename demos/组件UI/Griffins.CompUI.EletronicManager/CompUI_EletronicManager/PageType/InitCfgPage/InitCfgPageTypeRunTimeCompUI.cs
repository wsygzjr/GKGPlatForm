using GF_Gereric;
using GKG.SubMM;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;
using System;

namespace Griffins.CompUI.EletronicManager.CompUI_EletronicManager.PageType.InitCfgPage
{
    internal class InitCfgPageTypeRunTimeCompUI : PageTypeRunTimeCompUIBase
    {
        private readonly EletronicManagerInitInnerSubPageRunTime _initInnerSubPageRunTime = new();
        private EletronicManagerSubMachineModulesInitCfg _data = new();

        protected override void _OnInit()
        {
            _data = new EletronicManagerSubMachineModulesInitCfg();
            _initInnerSubPageRunTime.Init(CallBack);
            ((IInnerSubPageRunTime)_initInnerSubPageRunTime).AfterModified += OnAfterModified;
        }

        protected override PageTypeID _GetPageTypeID()
        {
            return ImeIOTMapConst.InitCfgPage;
        }

        protected override IPageTypeRunTimeCompUIView _GetPageTypeCompUIView(string viewID)
        {
            return null;
        }

        protected override void _SetData(byte[] data)
        {
            _data = data == null || data.Length == 0
                ? new EletronicManagerSubMachineModulesInitCfg()
                : JsonObjConvert.FromJSonBytes<EletronicManagerSubMachineModulesInitCfg>(data)
                    ?? new EletronicManagerSubMachineModulesInitCfg();

            ((IInnerSubPageRunTime)_initInnerSubPageRunTime).SetData(JsonObjConvert.ToJSonBytes(_data));
        }

        protected override byte[] _GetData()
        {
            var raw = ((IInnerSubPageRunTime)_initInnerSubPageRunTime).GetData();
            _data = raw == null || raw.Length == 0
                ? new EletronicManagerSubMachineModulesInitCfg()
                : JsonObjConvert.FromJSonBytes<EletronicManagerSubMachineModulesInitCfg>(raw)
                    ?? new EletronicManagerSubMachineModulesInitCfg();

            return JsonObjConvert.ToJSonBytes(_data);
        }

        protected override string _ExecViewCmd(string cmdID, string cmdParam)
        {
            return string.Empty;
        }

        protected override ISubPageRunTime _GetSubPageRunTime(SubPageKindInfoBase subPageKindInfo)
        {
            var innerSubPageKindInfo = (InnerSubPageKindInfo)subPageKindInfo;
            if (innerSubPageKindInfo.InnerSubPageTypeID.ToString() == EletronicManagerInitSubPageInfoDef.InnerSubPageTypeIDStr)
                return _initInnerSubPageRunTime;

            throw new Exception("不存在对应内部子页面类型ID的内部子页面信息");
        }

        protected override bool _CheckDataValid(out string[] inValidMsg)
        {
            inValidMsg = Array.Empty<string>();
            return true;
        }

        private void OnAfterModified(object sender, EventArgs e)
        {
            var raw = ((IInnerSubPageRunTime)_initInnerSubPageRunTime).GetData();
            _data = raw == null || raw.Length == 0
                ? new EletronicManagerSubMachineModulesInitCfg()
                : JsonObjConvert.FromJSonBytes<EletronicManagerSubMachineModulesInitCfg>(raw)
                    ?? new EletronicManagerSubMachineModulesInitCfg();

            AfterDataModified?.Invoke(sender, e);
        }
    }
}
