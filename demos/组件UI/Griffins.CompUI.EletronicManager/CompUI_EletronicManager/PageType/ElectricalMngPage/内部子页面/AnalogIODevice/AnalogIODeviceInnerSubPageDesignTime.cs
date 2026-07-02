using Griffins.Map.UI;
using System;

namespace Griffins.CompUI.EletronicManager.CompUI_EletronicManager.PageType.ElectricalMngPage
{
    public class AnalogIODeviceInnerSubPageDesignTime : IInnerSubPageDesignTime
    {
        public byte[] ViewCfgInfo => Array.Empty<byte>();

        event EventHandler ISubPageDesignTime.AfterModified
        {
            add { }
            remove { }
        }

        void ISubPageDesignTime.Init(byte[] viewCfgInfo)
        {
        }

        public void Edit(object ower)
        {
        }
    }
}
