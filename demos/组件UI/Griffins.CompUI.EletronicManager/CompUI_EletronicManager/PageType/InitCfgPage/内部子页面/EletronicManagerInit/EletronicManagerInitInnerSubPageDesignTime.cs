using Griffins.Map.UI;
using System;

namespace Griffins.CompUI.EletronicManager.CompUI_EletronicManager.PageType.InitCfgPage
{
    public class EletronicManagerInitInnerSubPageDesignTime : IInnerSubPageDesignTime
    {
        public byte[] ViewCfgInfo => Array.Empty<byte>();

        event EventHandler ISubPageDesignTime.AfterModified
        {
            add { }
            remove { }
        }

        public void Edit(object ower)
        {
        }

        void ISubPageDesignTime.Init(byte[] viewCfgInfo)
        {
        }
    }
}
