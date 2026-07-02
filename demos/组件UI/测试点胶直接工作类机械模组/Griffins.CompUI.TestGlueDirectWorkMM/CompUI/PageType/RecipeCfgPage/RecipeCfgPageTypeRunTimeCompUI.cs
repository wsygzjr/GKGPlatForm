using Griffins.CompUI.TestGlueDirectWorkMM.InitCfgPage;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage
{
	internal class RecipeCfgPageTypeRunTimeCompUI : PageTypeRunTimeCompUIBase
	{
        private IPageTypeRunTimeCompUIView testPageTypeRunTimeCompUIView;

        private IPageTypeRunTimeCompUIView test1PageTypeRunTimeCompUIView;

        private RecipeCfgPageTypeData _recipeCfgPageTypeData;

        private event EventHandler afterDataModified;
        /// <summary>
        /// 工艺参数与计算轨迹内部子页面运行时接口实现对象
        /// </summary>
        private PPCfgInnerSubPageRunTime pPCfgInnerSubPageRunTime = new();
        /// <summary>
        /// 初始化
        /// </summary>
        protected override void _OnInit()
		{
            _recipeCfgPageTypeData = new RecipeCfgPageTypeData();
			this.testPageTypeRunTimeCompUIView = new TestPageTypeRunTimeCompUIView(this.CallBack);
			this.test1PageTypeRunTimeCompUIView = new Test1PageTypeRunTimeCompUIView(this.CallBack);
			testPageTypeRunTimeCompUIView.AfterModified += afterModified;
			test1PageTypeRunTimeCompUIView.AfterModified += afterModified;

            //创建内部子页面实例
            pPCfgInnerSubPageRunTime.Init(base.CallBack);
            (pPCfgInnerSubPageRunTime as IInnerSubPageRunTime).AfterModified += afterModified; ;
        }

        private void afterModified(object sender, EventArgs e)
        {
            AfterDataModified?.Invoke(sender, e);
        }
        /// <summary>
        /// 页面类型ID
        /// </summary>
        protected override PageTypeID _GetPageTypeID() { return ImeIOTMapConst.PPCfgPage; }

        /// <summary>
        /// 获取界面ID对应的页面类型组件界面视图接口实例,null表示不存在界面ID对应的页面类型组件界面
        /// </summary>
        ///  <param name="viewID">界面ID</param>
        /// <returns>界面ID对应的页面类型组件界面视图接口实例</returns>
        protected override IPageTypeRunTimeCompUIView _GetPageTypeCompUIView(string viewID)
		{
			switch (viewID)
			{
				case RecipeCfgPageTypeConst.ViewID_Test:
					return testPageTypeRunTimeCompUIView;
				case RecipeCfgPageTypeConst.ViewID_Test1:
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
            _recipeCfgPageTypeData = GF_Gereric.JsonObjConvert.FromJSonBytes<RecipeCfgPageTypeData>(data);
			(testPageTypeRunTimeCompUIView as TestPageTypeRunTimeCompUIView).SetData(_recipeCfgPageTypeData.TestRecipeCfgCompUIModel);
			(test1PageTypeRunTimeCompUIView as Test1PageTypeRunTimeCompUIView).SetData(_recipeCfgPageTypeData.Test1RecipeCfgCompUIModel);

            (pPCfgInnerSubPageRunTime as IInnerSubPageRunTime).SetData(_recipeCfgPageTypeData.PPInnerSubPageCfgs);
        }
        /// <summary>
        /// 获取数据信息，null表示缺省值
        /// </summary>
        /// <returns>数据信息，null表示缺省值</returns>
        protected override byte[] _GetData()
		{
            _recipeCfgPageTypeData.TestRecipeCfgCompUIModel=(testPageTypeRunTimeCompUIView as TestPageTypeRunTimeCompUIView).GetData();
            _recipeCfgPageTypeData.Test1RecipeCfgCompUIModel = (test1PageTypeRunTimeCompUIView as Test1PageTypeRunTimeCompUIView).GetData();
            _recipeCfgPageTypeData.PPInnerSubPageCfgs = (pPCfgInnerSubPageRunTime as IInnerSubPageRunTime).GetData();

            return GF_Gereric.JsonObjConvert.ToJSonBytes(_recipeCfgPageTypeData);
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
        /// <summary>
        /// 创建内部子页面运行时接口
        /// </summary>
        /// <param name="subPageKindInfo">子页面种类信息</param>
        /// <returns>内部子页面运行时接口</returns>
        protected override ISubPageRunTime _GetSubPageRunTime(SubPageKindInfoBase subPageKindInfo)
		{
            InnerSubPageKindInfo innerSubPageKindInfo = (InnerSubPageKindInfo)subPageKindInfo;
            switch (innerSubPageKindInfo.InnerSubPageTypeID.ToString())
            {
                case PPCfgSubPageInfoDef.InnerSubPageTypeIDStr:
                    return pPCfgInnerSubPageRunTime;

                default:
                    throw new Exception("不存在对应内部子页面类型ID的内部子页面信息");
            }
        }
		public class RecipeCfgPageTypeData
        {
			public TestRecipeCfgCompUIModel TestRecipeCfgCompUIModel { get; set; }

			public Test1RecipeCfgCompUIModel Test1RecipeCfgCompUIModel { get; set; }
            /// <summary>
            /// 配方参数内部子页面配置信息
            /// </summary>
            public byte[] PPInnerSubPageCfgs { get; set; }
        }
	}
}
