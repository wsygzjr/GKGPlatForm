using GF_Gereric;
using Griffins.Map.UI;
using System;

namespace Griffins.CompUI.EletronicManager.CompUI_EletronicManager.PageType.ElectricalMngPage
{
    /// <summary>
    /// 运控卡配置内部子页面设计时实现对象
    /// </summary>
    public class ControlCardInnerSubPageDesignTime : IInnerSubPageDesignTime
    {
        public byte[] ViewCfgInfo => Array.Empty<byte>();

        /// <summary>
        /// 界面参数数据变更事件
        /// </summary>
        event EventHandler ISubPageDesignTime.AfterModified
        {
            add { }
            remove { }
        }

        public void Edit(object ower)
        {
            // 设计时编辑未实现：保持 no-op，禁止抛异常影响运行时 UI
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="viewCfgInfo">页面设计配置数据字节数组</param>
        void ISubPageDesignTime.Init(byte[] viewCfgInfo)
        {
            // 设计时无需特殊初始化
        }
    }
}
