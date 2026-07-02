using Griffins.CompUI.TestGlueDirectWorkMM.FactoryCfgPage.Models;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Griffins.CompUI.TestGlueDirectWorkMM.FactoryCfgPage
{
	internal class FactoryCfgPageTypeRunTimeCompUI : PageTypeRunTimeCompUIBase
	{
        private IPageTypeRunTimeCompUIView testPageTypeRunTimeCompUIView;

        private IPageTypeRunTimeCompUIView test1PageTypeRunTimeCompUIView;

        private FactoryCfgPageTypeData factoryCfgPageTypeData;

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void _OnInit()
		{
            factoryCfgPageTypeData = new FactoryCfgPageTypeData();
			this.testPageTypeRunTimeCompUIView = new TestPageTypeRunTimeCompUIView(CallBack);
			this.test1PageTypeRunTimeCompUIView = new Test1PageTypeRunTimeCompUIView(CallBack);
			testPageTypeRunTimeCompUIView.AfterModified += afterModified;
			test1PageTypeRunTimeCompUIView.AfterModified += afterModified;

        }

        private void afterModified(object sender, EventArgs e)
        {
            AfterDataModified?.Invoke(sender, e);
        }
        /// <summary>
        /// 页面类型ID
        /// </summary>
        protected override PageTypeID _GetPageTypeID() { return ImeIOTMapConst.FactoryCfgPage; }

        /// <summary>
        /// 获取界面ID对应的页面类型组件界面视图接口实例,null表示不存在界面ID对应的页面类型组件界面
        /// </summary>
        ///  <param name="viewID">界面ID</param>
        /// <returns>界面ID对应的页面类型组件界面视图接口实例</returns>
        protected override IPageTypeRunTimeCompUIView _GetPageTypeCompUIView(string viewID)
		{
			switch (viewID)
			{
				case FactoryCfgPageTypeConst.ViewID_Test:
					return testPageTypeRunTimeCompUIView;
				case FactoryCfgPageTypeConst.ViewID_Test1:
					return test1PageTypeRunTimeCompUIView;
				default:

					return null;
			}
		}
        /// <summary>
        /// 设置数据信息
        /// </summary>
        /// <param name="data">数据信息，null表示缺省值</param>
        protected override void _SetData(byte[] data)
		{
			if (data == null)
			{
				return;
			}
            factoryCfgPageTypeData = GF_Gereric.JsonObjConvert.FromJSonBytes<FactoryCfgPageTypeData>(data);
			(testPageTypeRunTimeCompUIView as TestPageTypeRunTimeCompUIView).SetData(factoryCfgPageTypeData.TestFactoryCfgCompUIModel);
			(test1PageTypeRunTimeCompUIView as Test1PageTypeRunTimeCompUIView).SetData(factoryCfgPageTypeData.Test1FactoryCfgCompUIModel);
        }
        /// <summary>
        /// 获取数据信息，null表示缺省值
        /// </summary>
        /// <returns>数据信息，null表示缺省值</returns>
        protected override byte[] _GetData()
		{
            factoryCfgPageTypeData.TestFactoryCfgCompUIModel=(testPageTypeRunTimeCompUIView as TestPageTypeRunTimeCompUIView).GetData();
            factoryCfgPageTypeData.Test1FactoryCfgCompUIModel = (test1PageTypeRunTimeCompUIView as Test1PageTypeRunTimeCompUIView).GetData();

            return GF_Gereric.JsonObjConvert.ToJSonBytes(factoryCfgPageTypeData);
		}
        /// <summary>
        ///  执行界面命令
        ///  说明：主要用于内部子页面和组件界面插件之间的数据交互，如：标定子页面为内部子页面，它需要从所有组件界面插件得到标定项，
        ///  包括：标定项名称、对应的界面ID，然后自动产生对应的子页面。
        /// </summary>
        /// <param name="cmdID">命令ID</param>
        /// <param name="cmdParam">命令参数（和命令ID对应的Json字符串）</param>
        /// <returns>返回结果（和命令ID对应的Json字符串）</returns>
        protected override string _ExecViewCmd(string cmdID, string cmdParam)
		{
			return string.Empty;
		}
       
		public class FactoryCfgPageTypeData
		{
			public TestFactoryCfgCompUIModel TestFactoryCfgCompUIModel { get; set; }

			public Test1FactoryCfgCompUIModel Test1FactoryCfgCompUIModel { get; set; }
        }
	}
}
