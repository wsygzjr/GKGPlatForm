using Griffins.CompUI.SL.DebugPage.View;
using Griffins.Map;
using Griffins.Map.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Griffins.CompUI.SL.DebugPage
{
	internal class DebugPageTypeDesignCompUI: IPageTypeDesignCompUI
	{
		private ICompUIRunTimeCallBack callBack;
		public DebugPageTypeDesignCompUI(ICompUIRunTimeCallBack callBack) 
		{
		     this.callBack = callBack;
		}
		/// <summary>
		/// 页面类型ID
		/// </summary>
		PageTypeID IPageTypeDesignCompUI.PageTypeID { get { return PageTypeID.Parse("DebugPage"); } }
		/// <summary>
		/// 获取组件实例所有的页面类型组件界面视图信息，null或个数为0表示没有对应的界面
		/// </summary>
		/// <returns>该机械模组所有的出厂配置参数配置界面信息列表</returns>
		PageTypeCompUIViewInfoList IPageTypeDesignCompUI.GetPageTypeCompUIViewInfoes()
		{			
			return new PageTypeCompUIViewInfoList()
			{
				new PageTypeCompUIViewInfo() { ViewID = DebugPageTypeConst.ViewID_Test, ViewName = ResourceA.ViewName_Test },
				new PageTypeCompUIViewInfo() { ViewID = DebugPageTypeConst.ViewID_Test1, ViewName = ResourceA.ViewzName_Test1 }
			};
		}
		/// <summary>
		/// 获取界面ID对应的页面类型组件界面视图接口实例,null表示不存在界面ID对应的页面类型组件界面
		/// </summary>
		///  <param name="viewID">界面ID</param>
		/// <returns>界面ID对应的页面类型组件界面视图接口实例</returns>
		object IPageTypeDesignCompUI.GetPageTypeCompUIView(string viewID)
		{
			switch (viewID)
			{
				case DebugPageTypeConst.ViewID_Test:
					return new TestDebugCompUIView();
				case DebugPageTypeConst.ViewID_Test1:
					return new Test1DebugCompUIView();
				default:

					return null;
			}
		}

        public InnerSubPageTypeInfoList GetInnerSubPageTypeInfoes()
        {
            throw new NotImplementedException();
        }

        public IInnerSubPageDesignTime CreateDesignTime(InnerSubPageTypeID innerSubPageTypeID)
        {
            throw new NotImplementedException();
        }

        public void Init(ICompUIDesignCallBack callBack)
        {
            throw new NotImplementedException();
        }
    }
}
