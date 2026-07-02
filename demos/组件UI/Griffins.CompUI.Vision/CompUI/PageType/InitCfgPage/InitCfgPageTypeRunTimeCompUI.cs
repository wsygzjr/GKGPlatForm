using GF_Gereric;
using GKG.Vision;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;

namespace Griffins.CompUI.Vision.CompUI.PageType.InitCfgPage
{
    internal class InitCfgPageTypeRunTimeCompUI : PageTypeRunTimeCompUIBase
    {
        private IPageTypeRunTimeCompUIView visionConfigView;



        private byte[] _data;

        protected override void _OnInit()
        {
            _data = new byte[0];

        }

        protected override PageTypeID _GetPageTypeID() { return PageTypeID.Parse("InitCfgPage"); }

        protected override IPageTypeRunTimeCompUIView _GetPageTypeCompUIView(string viewID)
        {
            if (viewID == InitCfgPageTypeConst.ViewID_Vision)
            {
                if (visionConfigView == null)
                {
                    visionConfigView = new VisionConfigPageTypeRunTimeCompUIView(this.CallBack);
                    visionConfigView.AfterModified += OnAfterModified;
                    (visionConfigView as VisionConfigPageTypeRunTimeCompUIView)?.SetData(_data);
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
            _data = data;
            if (visionConfigView is VisionConfigPageTypeRunTimeCompUIView gc)
            {
                gc.SetData(_data);
            }
        }

        protected override byte[] _GetData()
        {
            if (visionConfigView is VisionConfigPageTypeRunTimeCompUIView gc)
            {
                _data = gc.GetData();
            }

            return _data;
        }

        protected override string _ExecViewCmd(string cmdID, string cmdParam)
        {
            return string.Empty;
        }

        private void OnAfterModified(object sender, EventArgs e)
        {
            if (visionConfigView is VisionConfigPageTypeRunTimeCompUIView gc)
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
