using GF_Gereric;
using Griffins.Map.UI;
using System;

namespace Griffins.CompUI.EletronicManager.CompUI_EletronicManager.PageType.ElectricalMngPage
{
    /// <summary>
    /// IO 设备配置内部子页面设计时接口实现对象
    /// </summary>
    public class IODeviceInnerSubPageDesignTime : IInnerSubPageDesignTime
    {
        /// <summary>
        /// 界面参数数据改变事件
        /// </summary>
        event EventHandler ISubPageDesignTime.AfterModified
        {
            add { }
            remove { }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="viewCfgInfo">内部子页面界面配置信息</param>
        void ISubPageDesignTime.Init(byte[] viewCfgInfo)
        {
            // 设计时初始化
        }


        public byte[] ViewCfgInfo => Array.Empty<byte>();





        public void Edit(object ower)
        {
        }
    }
}
