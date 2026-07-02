using Griffins;
using Griffins.ImeIOT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GriffinsGeneralTestMM
{
    /// <summary>
    ///  子机械模组信息
    /// </summary>
    public class GenSubMMInfo
    {
        /// <summary>
        /// 子机械模组名称
        /// </summary>
        public string SubMMName { get; set; }

        /// <summary>
        ///  子机械模组的机械模组能力事件列表
        /// </summary>
        public GenCabilityEventDefInfoList CabilityEvents { get; set; }
        /// <summary>
        ///  子机械模组的机械模组能力方法列表
        /// </summary>
        public GenCabilityMethodDefInfoList CabilityMethods { get; set; }
        /// <summary>
        ///  子机械模组界面数据对象属性定义信息列表
        /// </summary>
        public GenUIDataObjPropDefInfo UIDataObjProps { get; set; }
        /// <summary>
        /// 子机械模组界面数据对象命令定义信息列表
        /// </summary>
        public ImeCompMethodDefInfoList UICommands { get; set; }
        /// <summary>
        ///  子机械模组的机械模组普通事件列表
        /// </summary>
        public GenNormalEventDefInfoList NormalEvents { get; set; }
        /// <summary>
        ///  子机械模组的机械模组普通方法列表
        /// </summary>
        public GenNormalMethodDefInfoList NormalMethods { get; set; }
        /// <summary>
        ///  控制面板定义信息列表
        /// </summary>
        public ControlPanelDefInfoList ControlPanels { get; set; }
        /// <summary>
        /// 子机械模组实例对象列表
        /// </summary>
        public SubMMObjInfoList SubMMObjInfos { get; set; }

		/// <summary>
		/// 设备属性列表，提供给设备模板定义时使用
		/// </summary>
		public DevicePropertyInfoList DeviceProps { get; set; }
        /// <summary>
        /// 界面数据对象属性交互接口
        /// </summary>
        public IUIDataObjPropExChange IUIDataObjPropExChange;
        /// <summary>
        ///  调整当前执行延迟时间
        /// </summary>
        /// <param name="execPercent">执行延迟时间百分比</param>
        public void AdjustCurDelyTime(int execPercent)
        {
            if (this.CabilityMethods != null)
            {
                foreach (var genMethodDefInfo in this.CabilityMethods)
                {
                    decimal curDelyTime = ((decimal)genMethodDefInfo.DelyTime * execPercent) / 100;
                    genMethodDefInfo.CurDelyTime = Convert.ToInt32(curDelyTime);
                }
            }
            if (this.NormalMethods != null)
            {
                foreach (var genMethodDefInfo in this.NormalMethods)
                {
                    decimal curDelyTime = ((decimal)genMethodDefInfo.DelyTime * execPercent) / 100;
                    genMethodDefInfo.CurDelyTime = Convert.ToInt32(curDelyTime);
                }
            }
        }

        /// <summary>
        /// 查找指定的能力方法ID对应的方法定义信息
        /// </summary>
        /// <param name="methodID">能力方法ID</param>
        /// <returns>指定的能力方法ID对应的方法定义信息</returns>
        public GenCabilityMethodDefInfo FindCabilityMethodDefInfo(string methodID)
        {
            if (this.CabilityMethods == null)
                return null;
            return this.CabilityMethods.Find(methodID);
        }

        /// <summary>
        /// 查找指定的普通方法ID对应的方法定义信息
        /// </summary>
        /// <param name="methodID">普通方法ID</param>
        /// <returns>指定的普通方法ID对应的方法定义信息</returns>
        public GenNormalMethodDefInfo FindNormalMethodDefInfo(string methodID)
        {
            if (this.NormalMethods == null)
                return null;
            return this.NormalMethods.Find(methodID);
        }

        /// <summary>
        ///  获取 ImeCompMethodDefInfoList
        /// </summary>
        /// <returns></returns>
        public ImeCompMethodDefInfoList GetImeCabilityMethodDefInfoList()
        {
            if (this.CabilityMethods == null)
                return new ImeCompMethodDefInfoList();
            return this.CabilityMethods.ToImeCabilityMethodDefInfoList();
        }

        /// <summary>
        ///  获取 ImeCompEventDefInfoList
        /// </summary>
        /// <returns></returns>
        public ImeCompEventDefInfoList GetImeCabilityEventDefInfoList()
        {
            if (this.CabilityEvents == null)
                return new ImeCompEventDefInfoList();
            return this.CabilityEvents.ToImeCabilityEventDefInfoList();
        }
    }

    /// <summary>
    ///  子机械模组设计时信息
    /// </summary>
    public class GenSubMMDesignTimeInfo
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="factoryCfgObjDefInfo">出厂配置参数对象定义信息</param>
        /// <param name="initCfgObjDefInfo">初始化参数对象定义信息</param>
        /// <param name="ppCfgObjDefInfo">配方参数对象定义信息</param>
        public GenSubMMDesignTimeInfo(GenCfgViewParamObjectDefInfoList factoryCfgObjDefInfo, GenCfgViewParamObjectDefInfoList initCfgObjDefInfo, GenCfgViewParamObjectDefInfoList ppCfgObjDefInfo)
        {
            this.FactoryCfgObjDefInfo = factoryCfgObjDefInfo;
            this.InitCfgObjDefInfo = initCfgObjDefInfo;
            this.PPCfgObjDefInfo = ppCfgObjDefInfo;
        }
        /// <summary>
		/// 出厂配置参数对象定义信息
		/// </summary>
		public GenCfgViewParamObjectDefInfoList FactoryCfgObjDefInfo { get; }
        /// <summary>
		/// 初始化参数对象定义信息
		/// </summary>
		public GenCfgViewParamObjectDefInfoList InitCfgObjDefInfo { get; }
        /// <summary>
		/// 配方参数对象定义信息
		/// </summary>
		public GenCfgViewParamObjectDefInfoList PPCfgObjDefInfo { get; }
    }
}
