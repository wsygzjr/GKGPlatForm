using Griffins;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;
using Griffins.PF;
using Griffins.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GriffinsGeneralTestMM
{
    class CompUIDesignTime : PageTypeDesignCompUIbase
    {
        private PageTypeID pageTypeID;
        private PageTypeCompUIViewInfoList cfgUIInfoes;
        private GenCfgViewParamObjectDefInfoList cfgViewParamObjectDefInfoes;
        public CompUIDesignTime(PageTypeID pageTypeID,GenCfgViewParamObjectDefInfoList cfgViewParamObjectDefInfoes)
        {
            this.pageTypeID = pageTypeID;
            this.cfgViewParamObjectDefInfoes = cfgViewParamObjectDefInfoes;
            this.cfgUIInfoes = new PageTypeCompUIViewInfoList();
            foreach(var item in cfgViewParamObjectDefInfoes)
            {
                this.cfgUIInfoes.Add(new PageTypeCompUIViewInfo
                {
                    ViewID = item.ViewID,
                    ViewName = item.ViewName,
                });
            }
        }
        /// <summary>
        /// 所属的页面类型ID
        /// </summary>
       protected override PageTypeID _GetPageTypeID() { return this.pageTypeID; }

        /// <summary>
        /// 获取组件实例所有的页面类型组件界面视图信息，null或个数为0表示没有对应的界面
        /// </summary>
        /// <returns>该机械模组所有的出厂配置参数配置界面信息列表</returns>
        protected override PageTypeCompUIViewInfoList _GetPageTypeCompUIViewInfoes()
        {
            return cfgUIInfoes;
        }

        /// <summary>
        /// 获取界面ID对应的页面类型组件界面视图接口实例,null表示不存在界面ID对应的页面类型组件界面
        /// </summary>
        ///  <param name="viewID">界面ID</param>
        /// <returns>界面ID对应的页面类型组件界面视图</returns>
        protected override object _GetPageTypeCompUIView(string viewID)
        {
            var cfgViewParamObjectDefInfo = this.cfgViewParamObjectDefInfoes.Find(viewID);
            if (cfgViewParamObjectDefInfo == null)
                return null;
            return getCompUIView(cfgViewParamObjectDefInfo);
        }
        private object getCompUIView(GenParamObjectDefInfo genObjectDefInfo)
        {
            //UctlTestMMCfgView uctlFactoryCfg = new UctlTestMMCfgView();
            //uctlFactoryCfg.Init(genObjectDefInfo);
            //uctlFactoryCfg.ReadOnly = true;
            //uctlFactoryCfg.CfgInfo = null;
            //return uctlFactoryCfg;

            var propValueListViewModel = new UIPropValueListViewModel();
            propValueListViewModel.CanEdit = false;
            GFUIPropDefInfoList uIPropDefInfoes = ConverterObj.ConvertToGFUIPropDefInfoList(genObjectDefInfo.ParamInfoes);
            propValueListViewModel.Init(new Griffins.GFBaseTypePropValueList(), uIPropDefInfoes);

            var uIPropValueListView = new UIPropValueListView();
            uIPropValueListView.DataContext = propValueListViewModel;
            propValueListViewModel.SetViewReference(uIPropValueListView);
            return uIPropValueListView;

        }
       
    }
}
