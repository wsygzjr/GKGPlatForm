using Griffins.CompUI.TestGlueDirectWorkMM.InitCfgPage.Models;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Griffins.CompUI.TestGlueDirectWorkMM.InitCfgPage
{
	internal class InitCfgPageTypeRunTimeCompUI : PageTypeRunTimeCompUIBase
	{
        private IPageTypeRunTimeCompUIView testPageTypeRunTimeCompUIView;

        private IPageTypeRunTimeCompUIView test1PageTypeRunTimeCompUIView;

        private InitCfgPageTypeData _initCfgPageTypeData;

        /// <summary>
        /// 标定内部子页面运行时接口实现对象
        /// </summary>
        private CalibrationInnerSubPageRunTime calibrationInnerSubPageRunTime=new ();
        
        /// <summary>
        /// 初始化
        /// </summary>
        protected override void _OnInit()
		{
            _initCfgPageTypeData = new InitCfgPageTypeData();
			this.testPageTypeRunTimeCompUIView = new TestPageTypeRunTimeCompUIView(CallBack);
			this.test1PageTypeRunTimeCompUIView = new Test1PageTypeRunTimeCompUIView(CallBack);
			testPageTypeRunTimeCompUIView.AfterModified += afterModified;
			test1PageTypeRunTimeCompUIView.AfterModified += afterModified;

            //创建内部子页面实例
            calibrationInnerSubPageRunTime.Init(base.CallBack);
            (calibrationInnerSubPageRunTime as IInnerSubPageRunTime).AfterModified += afterModified; ;
        }

        private void afterModified(object sender, EventArgs e)
        {
            AfterDataModified?.Invoke(sender, e);
        }
        /// <summary>
        /// 页面类型ID
        /// </summary>
        protected override PageTypeID _GetPageTypeID() { return ImeIOTMapConst.InitCfgPage; }

        /// <summary>
        /// 获取界面ID对应的页面类型组件界面视图接口实例,null表示不存在界面ID对应的页面类型组件界面
        /// </summary>
        ///  <param name="viewID">界面ID</param>
        /// <returns>界面ID对应的页面类型组件界面视图接口实例</returns>
        protected override  IPageTypeRunTimeCompUIView _GetPageTypeCompUIView(string viewID)
		{
			switch (viewID)
			{
				case InitCfgPageTypeConst.ViewID_Test:
					return testPageTypeRunTimeCompUIView;
				case InitCfgPageTypeConst.ViewID_Test1:
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
            _initCfgPageTypeData = GF_Gereric.JsonObjConvert.FromJSonBytes<InitCfgPageTypeData>(data);
			(testPageTypeRunTimeCompUIView as TestPageTypeRunTimeCompUIView).SetData(_initCfgPageTypeData.TestInitCfgCompUIModel);
			(test1PageTypeRunTimeCompUIView as Test1PageTypeRunTimeCompUIView).SetData(_initCfgPageTypeData.Test1InitCfgCompUIModel);

			(calibrationInnerSubPageRunTime as IInnerSubPageRunTime).SetData(_initCfgPageTypeData.CalibrationInnerSubPageCfgs);

        }
        /// <summary>
        /// 获取数据信息，null表示缺省值
        /// </summary>
        /// <returns>数据信息，null表示缺省值</returns>
        protected override byte[] _GetData()
		{
            _initCfgPageTypeData.TestInitCfgCompUIModel=(testPageTypeRunTimeCompUIView as TestPageTypeRunTimeCompUIView).GetData();
            _initCfgPageTypeData.Test1InitCfgCompUIModel = (test1PageTypeRunTimeCompUIView as Test1PageTypeRunTimeCompUIView).GetData();
            _initCfgPageTypeData.CalibrationInnerSubPageCfgs = (calibrationInnerSubPageRunTime as IInnerSubPageRunTime).GetData();

            return GF_Gereric.JsonObjConvert.ToJSonBytes(_initCfgPageTypeData);
		}
        /// <summary>
        ///  执行界面命令
        ///  说明：主要用于内部子页面和组件界面插件之间的数据交互，如：标定子页面为内部子页面，它需要从所有组件界面插件得到标定项，
        ///  包括：标定项名称、对应的界面ID，然后自动产生对应的子页面。
        /// </summary>
        /// <param name="cmdID">命令ID</param>
        /// <param name="cmdParam">命令参数（和命令ID对应的Json字符串）</param>
        /// <returns>返回结果（和命令ID对应的Json字符串）</returns>
        protected override  string _ExecViewCmd(string cmdID, string cmdParam)
		{
			return string.Empty;
		}
        /// <summary>
        /// 创建内部子页面运行时接口
        /// </summary>
        /// <param name="subPageKindInfo">子页面种类信息</param>
        /// <returns>内部子页面运行时接口</returns>
        protected override  ISubPageRunTime _GetSubPageRunTime(SubPageKindInfoBase subPageKindInfo)
		{
			InnerSubPageKindInfo innerSubPageKindInfo = (InnerSubPageKindInfo)subPageKindInfo;
			switch (innerSubPageKindInfo.InnerSubPageTypeID.ToString())
			{
				//标定内部子页面
				case CalibrationSubPageInfoDef.InnerSubPageTypeIDStr:
                    return calibrationInnerSubPageRunTime;
			
				default:
					throw new Exception("不存在对应内部子页面类型ID的内部子页面信息");
			}

		}
		public class InitCfgPageTypeData
		{
			public TestInitCfgCompUIModel TestInitCfgCompUIModel { get; set; }

			public Test1InitCfgCompUIModel Test1InitCfgCompUIModel { get; set; }
			/// <summary>
			/// 标定内部子页面配置信息
			/// </summary>
            public byte[] CalibrationInnerSubPageCfgs { get; set; }
        }
	}
}
