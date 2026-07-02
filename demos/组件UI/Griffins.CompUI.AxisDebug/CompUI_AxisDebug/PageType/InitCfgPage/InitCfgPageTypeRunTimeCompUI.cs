using GF_Gereric;
using Griffins.Map;
using Griffins.Map.UI;
using System;
using Griffins.CompUI.AxisDebug.CompUI_AxisDebug.PageType.InitCfgPage.AxisDebugging;
using Griffins.CompUI.AxisDebug.CompUI_AxisDebug.PageType.InitCfgPage.IOInDebugging;
using Griffins.CompUI.AxisDebug.CompUI_AxisDebug.PageType.InitCfgPage.IOOutDebugging;
using Griffins.ImeIOT.Map;

namespace Griffins.CompUI.AxisDebug.CompUI_AxisDebug.PageType.InitCfgPage
{
    internal class InitCfgPageTypeRunTimeCompUI : PageTypeRunTimeCompUIBase
    {
        private IPageTypeRunTimeCompUIView axisDebuggingView;

        private IPageTypeRunTimeCompUIView ioInDebuggingView;

        private IPageTypeRunTimeCompUIView ioOutDebuggingView;

        protected override void _OnInit()
        {
        }

        protected override PageTypeID _GetPageTypeID() { return PageTypeID.Parse("InitCfgPage"); }

        protected override IPageTypeRunTimeCompUIView _GetPageTypeCompUIView(string viewID)
        {
            if (viewID == InitCfgPageTypeConst.ViewID_AxisDebugging)
            {
                if (axisDebuggingView != null)
                {
                    axisDebuggingView.AfterModified -= OnAfterModified;
                }

                axisDebuggingView = new AxisDebuggingPageTypeRunTimeCompUIView(this.CallBack);
                axisDebuggingView.AfterModified += OnAfterModified;
                return axisDebuggingView;
            }

            if (viewID == InitCfgPageTypeConst.ViewID_IOInDebugging)
            {
                if (ioInDebuggingView != null)
                {
                    ioInDebuggingView.AfterModified -= OnAfterModified;
                }

                ioInDebuggingView = new IOInDebuggingPageTypeRunTimeCompUIView(this.CallBack);
                ioInDebuggingView.AfterModified += OnAfterModified;
                return ioInDebuggingView;
            }

            if (viewID == InitCfgPageTypeConst.ViewID_IOOutDebugging)
            {
                if (ioOutDebuggingView != null)
                {
                    ioOutDebuggingView.AfterModified -= OnAfterModified;
                }

                ioOutDebuggingView = new IOOutDebuggingPageTypeRunTimeCompUIView(this.CallBack);
                ioOutDebuggingView.AfterModified += OnAfterModified;
                return ioOutDebuggingView;
            }

            return null;
        }

        protected override void _SetData(byte[] data)
        {
        }

        protected override byte[] _GetData()
        {
            return JsonObjConvert.ToJSonBytes(new byte[0]);
        }

        protected override string _ExecViewCmd(string cmdID, string cmdParam)
        {
            return string.Empty;
        }

        protected override ISubPageRunTime _GetSubPageRunTime(SubPageKindInfoBase subPageKindInfo)
        {
            return null;
        }

        private void OnAfterModified(object sender, EventArgs e)
        {
            AfterDataModified?.Invoke(sender, e);
        }

        protected override bool _CheckDataValid(out string[] inValidMsg)
        {
            throw new NotImplementedException();
        }
    }
}
