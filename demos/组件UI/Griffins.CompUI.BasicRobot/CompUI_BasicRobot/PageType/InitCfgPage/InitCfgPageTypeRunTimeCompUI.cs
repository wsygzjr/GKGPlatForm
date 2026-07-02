using GF_Gereric;
using GKG.SubMM;
using Griffins.CompUI.BasicRobot.CompUI_BasicRobot.PageType.InitCfgPage.MechanicalArm;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;
using System;

namespace Griffins.CompUI.BasicRobot.CompUI_BasicRobot.PageType.InitCfgPage
{
    internal class InitCfgPageTypeRunTimeCompUI : PageTypeRunTimeCompUIBase
    {
        private IPageTypeRunTimeCompUIView mechanicalArmView;

        private BasicRobotSubMachineModulesInitCfg data;

        protected override void _OnInit()
        {
            data = new BasicRobotSubMachineModulesInitCfg();
        }

        protected override PageTypeID _GetPageTypeID() { return PageTypeID.Parse("InitCfgPage"); }

        protected override IPageTypeRunTimeCompUIView _GetPageTypeCompUIView(string viewID)
        {
            if (viewID == InitCfgPageTypeConst.ViewID_MechanicalArm)
            {
                if (mechanicalArmView == null)
                {
                    mechanicalArmView = new MechanicalArmPageTypeRunTimeCompUIView(this.CallBack);
                    mechanicalArmView.AfterModified += OnAfterModified;
                }
                (mechanicalArmView as MechanicalArmPageTypeRunTimeCompUIView)?.SetData(data);
                return mechanicalArmView;
            }
            return null;
        }

        protected override void _SetData(byte[] dataBytes)
        {
            if (dataBytes == null) return;
            data = JsonObjConvert.FromJSonBytes<BasicRobotSubMachineModulesInitCfg>(dataBytes) ?? new BasicRobotSubMachineModulesInitCfg();
            (mechanicalArmView as MechanicalArmPageTypeRunTimeCompUIView)?.SetData(data);
        }

        protected override byte[] _GetData()
        {
            if (mechanicalArmView is MechanicalArmPageTypeRunTimeCompUIView view)
            {
                data = view.GetData();
            }
            return JsonObjConvert.ToJSonBytes(data);
        }

        protected override string _ExecViewCmd(string cmdID, string cmdParam)
        {
            return string.Empty;
        }

        private void OnAfterModified(object sender, EventArgs e)
        {
            if (mechanicalArmView is MechanicalArmPageTypeRunTimeCompUIView view)
            {
                data = view.GetData();
            }
            AfterDataModified?.Invoke(sender, e);
        }

        protected override ISubPageRunTime _GetSubPageRunTime(SubPageKindInfoBase subPageKindInfo)
        {
            return null;
        }

        protected override bool _CheckDataValid(out string[] inValidMsg)
        {
            inValidMsg=null;
            return true;
        }

        private class InitCfgPageTypeData
        {
            public BasicRobotSubMachineModulesInitCfg MechanicalArm { get; set; }

            public InitCfgPageTypeData()
            {
                MechanicalArm = new BasicRobotSubMachineModulesInitCfg();
            }
        }
    }
}
