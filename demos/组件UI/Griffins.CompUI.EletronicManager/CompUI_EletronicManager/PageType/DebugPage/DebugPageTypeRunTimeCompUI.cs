using Griffins.ImeIOT;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;
using System;

namespace Griffins.CompUI.EletronicManager.CompUI_EletronicManager.PageType.DebugPage
{
    internal class DebugPageTypeRunTimeCompUI : PageTypeRunTimeCompUIBase
    {
        private readonly AxisDebugInnerSubPageRunTime _axisDebugInnerSubPageRunTime = new(AxisDebugInnerSubPageMode.AxisDebug);
        private readonly AxisDebugInnerSubPageRunTime _ioInInnerSubPageRunTime = new(AxisDebugInnerSubPageMode.IOIn);
        private byte[] _data = Array.Empty<byte>();

        protected override void _OnInit()
        {
            _axisDebugInnerSubPageRunTime.Init(base.CallBack);
            _ioInInnerSubPageRunTime.Init(base.CallBack);
        }

        protected override PageTypeID _GetPageTypeID() { return ImeIOTConst.DebugPage; }

        protected override IPageTypeRunTimeCompUIView _GetPageTypeCompUIView(string viewID)
        {
            return null;
        }

        protected override void _SetData(byte[] data)
        {
            _data = data ?? Array.Empty<byte>();
            (_axisDebugInnerSubPageRunTime as IInnerSubPageRunTime).SetData(_data);
            (_ioInInnerSubPageRunTime as IInnerSubPageRunTime).SetData(_data);
        }

        protected override byte[] _GetData()
        {
            return _data;
        }

        protected override string _ExecViewCmd(string cmdID, string cmdParam)
        {
            return string.Empty;
        }

        protected override ISubPageRunTime _GetSubPageRunTime(SubPageKindInfoBase subPageKindInfo)
        {
            InnerSubPageKindInfo innerSubPageKindInfo = (InnerSubPageKindInfo)subPageKindInfo;
            switch (innerSubPageKindInfo.InnerSubPageTypeID.ToString())
            {
                case AxisDebugSubPageInfoDef.AxisDebugInnerSubPageTypeIDStr:
                    return _axisDebugInnerSubPageRunTime;
                case AxisDebugSubPageInfoDef.IOInInnerSubPageTypeIDStr:
                    return _ioInInnerSubPageRunTime;
                default:
                    throw new Exception("不存在对应内部子页面类型ID的内部子页面信息");
            }
        }

        protected override bool _CheckDataValid(out string[] inValidMsg)
        {
            throw new NotImplementedException();
        }
    }
}
