using GF_Gereric;
using Griffins.ImeIOT;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GriffinsGeneralTestMM
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class GenTestMMCompUIBase : CompUIBase
    {
        private GenMMInfo genMMInfo; 
        private GenMMDesignTimeInfo genMMDesignTimeInfo;
        public GenTestMMCompUIBase(GenMMInfo genMMInfo, GenMMDesignTimeInfo genMMDesignTimeInfo)
        {
            this.genMMInfo = genMMInfo;
            this.genMMDesignTimeInfo = genMMDesignTimeInfo;
        }

        /// <summary>
        /// 组件名称
        /// </summary>
        protected override string _GetCompName() {   return genMMInfo.MMName;   }

		/// <summary>
		/// 获取页面类型组件界面设计时接口实例
		/// </summary>
		/// <param name="pageTypeID">页面类型ID</param>
		/// <param name="objID">实现对象ID(只有一个实现对象为Guid.Empty)</param>
		/// <returns>页面类型组件界面设计时接口实例</returns>
		protected override IPageTypeDesignCompUI _GetPageTypeDesignCompUI(PageTypeID pageTypeID, Guid objID)
        { 
            if (pageTypeID == ImeIOTConst.FactoryCfgPage)
                return new CompUIDesignTime(pageTypeID, genMMDesignTimeInfo.FactoryCfgObjDefInfo);
            else if (pageTypeID == ImeIOTConst.InitCfgPage)
                return new CompUIDesignTime(pageTypeID, genMMDesignTimeInfo.InitCfgObjDefInfo);
            else if (pageTypeID == ImeIOTConst.PPCfgPage)
                return new CompUIDesignTime(pageTypeID, genMMDesignTimeInfo.PPCfgObjDefInfo);
            return null;
        }
		/// <summary>
		/// 获取页面类型组件界面运行时接口实例
		/// </summary>
		/// <param name="pageTypeID">页面类型ID</param>
		/// <param name="objID">实现对象ID(只有一个实现对象为Guid.Empty)</param>
		/// <returns>页面类型组件界面运行时接口实例</returns>
		protected override IPageTypeRunTimeCompUI _GetPageTypeRunTimeCompUI(PageTypeID pageTypeID, Guid objID)
        { 
            if (pageTypeID == ImeIOTConst.FactoryCfgPage)
                return new CompUIRunTime(pageTypeID, genMMDesignTimeInfo.FactoryCfgObjDefInfo);
            else if (pageTypeID == ImeIOTConst.InitCfgPage)
                return new CompUIRunTime(pageTypeID, genMMDesignTimeInfo.InitCfgObjDefInfo);
            else if (pageTypeID == ImeIOTConst.PPCfgPage)
                return new CompUIRunTime(pageTypeID, genMMDesignTimeInfo.PPCfgObjDefInfo);
            return null;
        }
        protected override IControlPanel _CreateControlPanel(Guid subMMObjID)
        {
            return new MainControlPanel(genMMInfo.ControlPanels);
        }
    }
}
