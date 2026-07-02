using Griffins.CompUI.EletronicManager.CompUI_EletronicManager.PageType.ElectricalMngPage.ControlCard.ViewModels;
using Griffins.CompUI.EletronicManager.CompUI_EletronicManager.PageType.ElectricalMngPage.ViewModels;
using Griffins.ImeIOT;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;
using System;

namespace Griffins.CompUI.EletronicManager.CompUI_EletronicManager.PageType.ElectricalMngPage
{
    internal class ElectricalMngPageTypeRunTimeCompUI : PageTypeRunTimeCompUIBase
    {
        private readonly ElectricalMngPageViewModel _sharedViewModel = new();
        private readonly ControlCardInnerSubPageRunTime _controlCardInnerSubPageRunTime;
        private readonly AxisConfigInnerSubPageRunTime _axisConfigInnerSubPageRunTime;
        private readonly IODeviceInnerSubPageRunTime _ioDeviceInnerSubPageRunTime;
        private readonly AnalogIODeviceInnerSubPageRunTime _analogIODeviceInnerSubPageRunTime;

        public ElectricalMngPageTypeRunTimeCompUI()
        {
            _controlCardInnerSubPageRunTime = new ControlCardInnerSubPageRunTime(_sharedViewModel);
            _axisConfigInnerSubPageRunTime = new AxisConfigInnerSubPageRunTime(_sharedViewModel);
            _ioDeviceInnerSubPageRunTime = new IODeviceInnerSubPageRunTime(_sharedViewModel);
            _analogIODeviceInnerSubPageRunTime = new AnalogIODeviceInnerSubPageRunTime(_sharedViewModel);
        }

        protected override void _OnInit()
        {
            _controlCardInnerSubPageRunTime.Init(base.CallBack);
            _axisConfigInnerSubPageRunTime.Init(base.CallBack);
            _ioDeviceInnerSubPageRunTime.Init(base.CallBack);
            _analogIODeviceInnerSubPageRunTime.Init(base.CallBack);
        }

        protected override PageTypeID _GetPageTypeID()
        {
            return ImeIOTConst.ElectricalMngCfgPage;
        }

        protected override IPageTypeRunTimeCompUIView _GetPageTypeCompUIView(string viewID)
        {
            return null;
        }

        protected override void _SetData(byte[] data)
        {
            if (data == null)
                return;

            _sharedViewModel.Init(data);
        }

        protected override byte[] _GetData()
        {
            var data = _sharedViewModel.GetData();
            return data;
        }

        protected override string _ExecViewCmd(string cmdID, string cmdParam)
        {
            return string.Empty;
        }

        protected override ISubPageRunTime _GetSubPageRunTime(SubPageKindInfoBase subPageKindInfo)
        {
            var innerSubPageKindInfo = (InnerSubPageKindInfo)subPageKindInfo;
            switch (innerSubPageKindInfo.InnerSubPageTypeID.ToString())
            {
                case ControlCardSubPageInfoDef.InnerSubPageTypeIDStr:
                    return _controlCardInnerSubPageRunTime;
                case AxisConfigSubPageInfoDef.InnerSubPageTypeIDStr:
                    return _axisConfigInnerSubPageRunTime;
                case IODeviceSubPageInfoDef.InnerSubPageTypeIDStr:
                    return _ioDeviceInnerSubPageRunTime;
                case AnalogIODeviceSubPageInfoDef.InnerSubPageTypeIDStr:
                    return _analogIODeviceInnerSubPageRunTime;
                default:
                    throw new Exception("Unknown inner sub page type.");
            }
        }

        protected override bool _CheckDataValid(out string[] inValidMsg)
        {
            inValidMsg = Array.Empty<string>();
            return true;
        }
    }
}
