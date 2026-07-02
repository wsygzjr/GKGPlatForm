using Griffins.CompUI.SL.DebugPage.Models;
using Griffins.Map;
using Griffins.Map.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Griffins.CompUI.SL.DebugPage
{
	internal class DebugPageTypeRunTimeCompUI : IPageTypeRunTimeCompUI
	{
		private ICompUIRunTimeCallBack callBack;

		private IPageTypeRunTimeCompUIView testPageTypeRunTimeCompUIView;

		private IPageTypeRunTimeCompUIView test1PageTypeRunTimeCompUIView;

		private DebugPageTypeData debugPageTypeData;

		private event EventHandler afterDataModified;
		public DebugPageTypeRunTimeCompUI(ICompUIRunTimeCallBack callBack)
		{
			debugPageTypeData = new DebugPageTypeData();
			this.callBack = callBack;
			this.testPageTypeRunTimeCompUIView=new TestPageTypeRunTimeCompUIView(this.callBack);
			this.test1PageTypeRunTimeCompUIView=new Test1PageTypeRunTimeCompUIView(this.callBack);
			testPageTypeRunTimeCompUIView.AfterModified += testPageTypeRunTimeCompUIView_AfterModified;
			test1PageTypeRunTimeCompUIView.AfterModified += test1PageTypeRunTimeCompUIView_AfterModified;
		}

		event EventHandler IPageTypeRunTimeCompUI.AfterDataModified
		{
			add
			{
				afterDataModified += value;
			}
			remove
			{
				afterDataModified -= value;
			}
		}

		private void testPageTypeRunTimeCompUIView_AfterModified(object sender, EventArgs e)
		{
			debugPageTypeData.TestDebugCompUIModel = (testPageTypeRunTimeCompUIView as TestPageTypeRunTimeCompUIView).GetData();
			afterDataModified?.Invoke(sender, e);
		}

		private void test1PageTypeRunTimeCompUIView_AfterModified(object sender, EventArgs e)
		{
			debugPageTypeData.Test1DebugCompUIModel = (test1PageTypeRunTimeCompUIView as Test1PageTypeRunTimeCompUIView).GetData();
			afterDataModified?.Invoke(sender, e);
		}

		/// <summary>
		/// 页面类型ID
		/// </summary>
		PageTypeID IPageTypeRunTimeCompUI.PageTypeID { get { return PageTypeID.Parse("DebugPage"); } }

		/// <summary>
		/// 获取界面ID对应的页面类型组件界面视图接口实例,null表示不存在界面ID对应的页面类型组件界面
		/// </summary>
		///  <param name="viewID">界面ID</param>
		/// <returns>界面ID对应的页面类型组件界面视图接口实例</returns>
		IPageTypeRunTimeCompUIView IPageTypeRunTimeCompUI.GetPageTypeCompUIView(string viewID)
		{
			switch (viewID)
			{
				case DebugPageTypeConst.ViewID_Test:
					return testPageTypeRunTimeCompUIView;
				case DebugPageTypeConst.ViewID_Test1:
					return test1PageTypeRunTimeCompUIView;
				default:

					return null;
			}
		}
		/// <summary>
		/// 设置数据信息
		/// </summary>
		/// <param name="data">数据信息，null表示缺省值</param>
		void IPageTypeRunTimeCompUI.SetData(byte[] data)
		{
			if (data == null)
			{
				return;
			}	
			debugPageTypeData = GF_Gereric.JsonObjConvert.FromJSonBytes<DebugPageTypeData>(data);
			(testPageTypeRunTimeCompUIView as TestPageTypeRunTimeCompUIView).SetData(debugPageTypeData.TestDebugCompUIModel);
			(test1PageTypeRunTimeCompUIView as Test1PageTypeRunTimeCompUIView).SetData(debugPageTypeData.Test1DebugCompUIModel);
		}
		/// <summary>
		/// 获取数据信息，null表示缺省值
		/// </summary>
		/// <returns>数据信息，null表示缺省值</returns>
		byte[] IPageTypeRunTimeCompUI.GetData()
		{			
			return GF_Gereric.JsonObjConvert.ToJSonBytes(debugPageTypeData);
		}
		/// <summary>
		///  执行界面命令
		///  说明：主要用于内部子页面和组件界面插件之间的数据交互，如：标定子页面为内部子页面，它需要从所有组件界面插件得到标定项，
		///  包括：标定项名称、对应的界面ID，然后自动产生对应的子页面。
		/// </summary>
		/// <param name="cmdID">命令ID</param>
		/// <param name="cmdParam">命令参数（和命令ID对应的Json字符串）</param>
		/// <returns>返回结果（和命令ID对应的Json字符串）</returns>
		string IPageTypeRunTimeCompUI.ExecViewCmd(string cmdID, string cmdParam)
		{
			return string.Empty;
		}

        public ISubPageRunTime GetSubPageRunTime(SubPageKindInfoBase subPageKindInfo)
        {
            throw new NotImplementedException();
        }

        public void Init(ICompUIRunTimeCallBack callBack)
        {
            throw new NotImplementedException();
        }

        private class DebugPageTypeData
		{
			public TestDebugCompUIModel TestDebugCompUIModel { get; set; }

			public Test1DebugCompUIModel Test1DebugCompUIModel { get; set; }
		}
	}
}
