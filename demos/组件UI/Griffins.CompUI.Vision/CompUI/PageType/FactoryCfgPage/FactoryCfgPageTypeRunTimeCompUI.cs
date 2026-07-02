using GF_Gereric;
using GKG.SubMM;
using Griffins.CompUI.Vision.CompUI.PageType.FactoryCfgPage.VisionFactoryCfg.Models;
using Griffins.CompUI.Vision.CompUI.PageType.InitCfgPage;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;

namespace Griffins.CompUI.Vision.CompUI.PageType.FactoryCfgPage
{
    internal class FactoryCfgPageTypeRunTimeCompUI : PageTypeRunTimeCompUIBase
    {
        private IPageTypeRunTimeCompUIView visionConfigView;

        private VisionSubMachineModulesFactoryCfg _data;

        protected override void _OnInit()
        {
            _data = new VisionSubMachineModulesFactoryCfg();
        }

        protected override PageTypeID _GetPageTypeID() { return PageTypeID.Parse("FactoryCfgPage"); }

        protected override IPageTypeRunTimeCompUIView _GetPageTypeCompUIView(string viewID)
        {
            if (viewID == FactoryCfgPageTypeConst.ViewID_Vision)
            {
                if (visionConfigView == null)
                {
                    visionConfigView = new VisionFactoryCfgPageTypeRunTimeCompUIView(this.CallBack);
                    visionConfigView.AfterModified += OnAfterModified;
                    (visionConfigView as VisionFactoryCfgPageTypeRunTimeCompUIView)?.SetData(_data);
                }
                return visionConfigView;
            }

            return null;
        }

        protected override void _SetData(byte[] data)
        {
            if (data == null)
            {
                return;
            }

            _data = JsonObjConvert.FromJSonBytes<VisionSubMachineModulesFactoryCfg>(data) ?? new VisionSubMachineModulesFactoryCfg();
            if (visionConfigView is VisionFactoryCfgPageTypeRunTimeCompUIView gc)
            {
                gc.SetData(_data);
            }
        }

        protected override byte[] _GetData()
        {
            if (visionConfigView is VisionFactoryCfgPageTypeRunTimeCompUIView gc)
            {
                _data = gc.GetData();
            }

            return JsonObjConvert.ToJSonBytes(_data);
        }

        protected override string _ExecViewCmd(string cmdID, string cmdParam)
        {
            return string.Empty;
        }

        private void OnAfterModified(object sender, EventArgs e)
        {
            if (visionConfigView is VisionFactoryCfgPageTypeRunTimeCompUIView gc)
            {
                _data = gc.GetData();
            }

            AfterDataModified?.Invoke(sender, e);
        }

        protected override ISubPageRunTime _GetSubPageRunTime(SubPageKindInfoBase subPageKindInfo)
        {
            return null;
        }

        protected override bool _CheckDataValid(out string[] inValidMsg)
        {
            inValidMsg = null;
            return true;
        }
    }
}
