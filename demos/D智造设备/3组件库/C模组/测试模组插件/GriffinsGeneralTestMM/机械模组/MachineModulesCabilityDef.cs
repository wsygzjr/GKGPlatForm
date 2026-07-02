using Griffins;
using Griffins.ImeIOT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GriffinsGeneralTestMM
{
	internal class MachineModulesCabilityDef: IMachineModulesCabilityDef
	{
        private GenMMInfo genMMInfo;
        private ImeCompMethodDefInfoList cabilityMethods;
        private ImeCompEventDefInfoList cabilityEvents;
        public MachineModulesCabilityDef(GenMMInfo genMMInfo)
        {
            this.genMMInfo = genMMInfo;
            this.cabilityMethods = this.genMMInfo.GetImeCabilityMethodDefInfoList();
            this.cabilityEvents = this.genMMInfo.GetImeCabilityEventDefInfoList();
        }

        /// <summary>
        /// 机械模组的机械模组能力事件列表
        /// </summary>
        ImeCompEventDefInfoList IMachineModulesCabilityDef.Events
        {
            get 
            {
                return this.cabilityEvents;
            }
        }
        /// <summary>
        /// 机械模组的机械模组能力方法列表
        /// </summary>
        ImeCompMethodDefInfoList IMachineModulesCabilityDef.Methods
		{
            get 
            {
                return this.cabilityMethods;
            }
        }

        /// <summary>
        /// 机械模组包含的子机械模组能力数据列表
        /// </summary>
		CompContainSubMMDataList IMachineModulesCabilityDef.SubMMs 
        {
            get 
            {
                return this.genMMInfo.SubMMs;
            }
        }

        /// <summary>
        /// 机械模组界面数据对象属性定义信息列表，如果不支持返回null
        /// </summary>
        ImeCompPropDefInfoList IMachineModulesCabilityDef.UIDataObjProps
        {
            get
            {
                if (this.genMMInfo.UIDataObjProps==null)
                    return null;
                else
                    return this.genMMInfo.UIDataObjProps.ToImeCompPropDefInfoes();
            }
        }
        /// <summary>
        /// 子机械模组界面数据对象命令定义信息列表，如果不支持返回null
        /// </summary>
        ImeCompMethodDefInfoList IMachineModulesCabilityDef.UICommands
        {
            get
            {
                return this.genMMInfo.UICommands;
            }
        }
        /// <summary>
		/// 创建机械模组定义信息服务接口实例
		/// </summary>
		/// <param name="alias">机械模组实例别名</param>
		/// <param name="factoryCfgInfo">出厂配置信息</param>
		/// <returns>机械模组定义信息服务接口实例</returns>
		IMachineModulesDefSvr IMachineModulesCabilityDef.CreateIMachineModulesDefSvr(MMAlias alias, byte[] factoryCfgInfo)
        {
            return new MachineModulesDefSvr(this.genMMInfo, factoryCfgInfo);
        }
    }

    /// <summary>
    /// 机械模组定义信息服务对象
    /// </summary>
    class MachineModulesDefSvr : IMachineModulesDefSvr
    {
        private GenMMInfo genMMInfo;
        private IMachineModulesDefSvrCallBack callBack;
        private byte[] factoryCfgInfo;
        public MachineModulesDefSvr(GenMMInfo genMMInfo, byte[] factoryCfgInfo)
        {
            this.factoryCfgInfo = factoryCfgInfo;
            this.genMMInfo = genMMInfo;
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="callBack">机械模组运行时回调接口</param>
        void IMachineModulesDefSvr.Init(IMachineModulesDefSvrCallBack callBack)
        {
            this.callBack = callBack;
        }
        /// <summary>
        /// 获取子界面数据对象项名称字典
        /// </summary>
        /// <param name="objInstPropPath">界面数据对象属性路径</param>
        /// <param name="factoryCfgInfo">子机械模组出厂配置信息</param>
        /// <returns>子界面数据对象项名称字典</returns>
        Dictionary<string, string> IMachineModulesDefSvr.GetSubUIProObjItemNames(ObjInstPropPath objInstPropPath)
        {
            //组件插件：可根据出厂配置信息的容器数量和callBack回调接口获取获取子机械模组实例名称，生成名称字典
            //模拟插件：可以写死固定的字典项数量，不需要从出厂配置数据获取字典项数量，如上下料容器数量
            return this.genMMInfo.IUIDataObjPropExChange.GetMMSubUIProObjItemNamesOfDefSvr(objInstPropPath, callBack);
        }
    }
}
