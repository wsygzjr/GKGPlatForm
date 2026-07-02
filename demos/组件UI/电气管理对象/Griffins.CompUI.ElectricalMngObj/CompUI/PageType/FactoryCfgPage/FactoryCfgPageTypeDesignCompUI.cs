using Griffins.CompUI.ElectricalMngObj.FactoryCfgPage.View;
using Griffins.ImeIOT;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Griffins.CompUI.ElectricalMngObj.FactoryCfgPage
{
	internal class FactoryCfgPageTypeDesignCompUI: PageTypeDesignCompUIbase
	{
        /// <summary>
        /// 页面类型ID
        /// </summary>
        protected override PageTypeID _GetPageTypeID() { return ImeIOTConst.FactoryCfgPage; }
        /// <summary>
        /// 获取组件实例所有的页面类型组件界面视图信息，null或个数为0表示没有对应的界面
        /// </summary>
        /// <returns>该机械模组所有的出厂配置参数配置界面信息列表</returns>
        protected override  PageTypeCompUIViewInfoList _GetPageTypeCompUIViewInfoes()
		{			
			return new PageTypeCompUIViewInfoList()
			{
				new PageTypeCompUIViewInfo() { ViewID = FactoryCfgPageTypeConst.ViewID_Test, ViewName = ResourceA.ViewName_Test },
				new PageTypeCompUIViewInfo() { ViewID = FactoryCfgPageTypeConst.ViewID_Test1, ViewName = ResourceA.ViewzName_Test1 }
			};
		}
        /// <summary>
        /// 获取界面ID对应的页面类型组件界面视图接口实例,null表示不存在界面ID对应的页面类型组件界面
        /// </summary>
        ///  <param name="viewID">界面ID</param>
        /// <returns>界面ID对应的页面类型组件界面视图接口实例</returns>
        protected override object _GetPageTypeCompUIView(string viewID)
		{
			switch (viewID)
			{
				case FactoryCfgPageTypeConst.ViewID_Test:
					return new TestFactoryCfgCompUIView();
				case FactoryCfgPageTypeConst.ViewID_Test1:
					return new Test1FactoryCfgCompUIView();
				default:

					return null;
			}
		}
        /// <summary>
        /// 获取内部子页面类型信息列表
        /// </summary>
        /// <returns>内部子页面类型信息列表</returns>
        protected override  InnerSubPageTypeInfoList _GetInnerSubPageTypeInfoes()
		{
            InnerSubPageTypeInfoList innerSubPageTypeInfoes = new InnerSubPageTypeInfoList();

            InnerSubPageTypeInfo controlCardInnerSubPageTypeInfo = new InnerSubPageTypeInfo();
            controlCardInnerSubPageTypeInfo.ID = ControlCardSubPageInfoDef.InnerSubPageTypeID;
            controlCardInnerSubPageTypeInfo.Name = ControlCardSubPageInfoDef.InnerSubPageTypeName;
            innerSubPageTypeInfoes.Add(controlCardInnerSubPageTypeInfo);

            InnerSubPageTypeInfo iODeviceInnerSubPageTypeInfo = new InnerSubPageTypeInfo();
            iODeviceInnerSubPageTypeInfo.ID = IODeviceSubPageInfoDef.InnerSubPageTypeID;
            iODeviceInnerSubPageTypeInfo.Name = IODeviceSubPageInfoDef.InnerSubPageTypeName;
            innerSubPageTypeInfoes.Add(iODeviceInnerSubPageTypeInfo);

            return innerSubPageTypeInfoes;
        }
        /// <summary>
        /// 创建内部子页面设计时接口，未实现返回null
        /// </summary>
        /// <param name="innerSubPageTypeID">内部子页面类型ID</param>
        /// <returns>内部子页面设计时接口，未实现返回null</returns>
        protected override IInnerSubPageDesignTime _CreateDesignTime(InnerSubPageTypeID innerSubPageTypeID)
        {
            if (innerSubPageTypeID == ControlCardSubPageInfoDef.InnerSubPageTypeID)
                return new ControlCardInnerInnerSubPageDesignTime();
            else if (innerSubPageTypeID == IODeviceSubPageInfoDef.InnerSubPageTypeID)
                return null;
            else
                throw new Exception($"未知的内部子页面类型ID{innerSubPageTypeID}");
        }
    }
}
