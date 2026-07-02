using Griffins;
using Griffins.ImeIOT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GriffinsGeneralTestMM
{
	internal class SubMachineModulesCabilityDef : ISubMachineModulesCabilityDef
    {
        private GenSubMMInfo genSubMMInfo;
        private ImeCompMethodDefInfoList cabilityMethods;
        private ImeCompEventDefInfoList cabilityEvents;
        public SubMachineModulesCabilityDef(GenSubMMInfo genSubMMInfo)
        {
            this.genSubMMInfo = genSubMMInfo;
            this.cabilityMethods = this.genSubMMInfo.GetImeCabilityMethodDefInfoList();
            this.cabilityEvents = this.genSubMMInfo.GetImeCabilityEventDefInfoList();
        }

        /// <summary>
        /// 机械模组的机械模组能力事件列表
        /// </summary>
        ImeCompEventDefInfoList ISubMachineModulesCabilityDef.Events
        {
            get 
            {
                return this.cabilityEvents;
            }
        }
        /// <summary>
        /// 机械模组的机械模组能力方法列表
        /// </summary>
        ImeCompMethodDefInfoList ISubMachineModulesCabilityDef.Methods
		{
            get 
            {
                return this.cabilityMethods;
            }
        }

        /// <summary>
        /// 子机械模组界面数据对象属性定义信息列表，如果不支持返回null
        /// </summary>
        ImeCompPropDefInfoList ISubMachineModulesCabilityDef.UIDataObjProps
        {
            get
            {
                if (this.genSubMMInfo.UIDataObjProps == null)
                    return null;
                else
                    return this.genSubMMInfo.UIDataObjProps.ToImeCompPropDefInfoes();
            }
        }
        /// <summary>
        /// 子机械模组界面数据对象命令定义信息列表，如果不支持返回null
        /// </summary>
        ImeCompMethodDefInfoList ISubMachineModulesCabilityDef.UICommands 
        {
            get
            {
                return this.genSubMMInfo.UICommands;
            }
        }

		/// <summary>
		/// 设备属性列表，提供给设备模板定义时使用
		/// </summary>
		DevicePropertyInfoList ISubMachineModulesCabilityDef.DeviceProps 
        {
			get
			{
				return this.genSubMMInfo.DeviceProps;
			}
		}
        /// <summary>
        /// 创建子机械模组定义信息服务接口实例
        /// </summary>
        /// <param name="alias">子机械模组实例别名</param>
        /// <param name="factoryCfgInfo">出厂配置信息</param>
        /// <param name="devicePropValues">设备属性值列表</param>
        /// <returns>子机械模组定义信息服务接口实例</returns>
        ISubMachineModulesDefSvr ISubMachineModulesCabilityDef.CreateISubMachineModulesDefSvr(SubMMAlias alias, byte[] factoryCfgInfo, GFBaseTypePropValueList devicePropValues)
        {
            return new SubMachineModulesDefSvr(this.genSubMMInfo, factoryCfgInfo,devicePropValues);
        }

        /// <summary>
        /// 子机械模组定义信息服务对象
        /// </summary>
        class SubMachineModulesDefSvr : ISubMachineModulesDefSvr
        {
            private GenSubMMInfo genSubMMInfo;
            private ISubMachineModulesDefSvrCallBack callBack;
            private byte[] factoryCfgInfo;
            private GFBaseTypePropValueList devicePropValues;
            public SubMachineModulesDefSvr(GenSubMMInfo genSubMMInfo, byte[] factoryCfgInfo, GFBaseTypePropValueList devicePropValues)
            {
                this.genSubMMInfo = genSubMMInfo;
                this.factoryCfgInfo = factoryCfgInfo;
                this.devicePropValues = devicePropValues;
            }
            /// <summary>
            /// 初始化
            /// </summary>
            /// <param name="callBack">机械模组运行时回调接口</param>
            void ISubMachineModulesDefSvr.Init(ISubMachineModulesDefSvrCallBack callBack)
            {
                this.callBack = callBack;
            }
            /// <summary>
            /// 获取子界面数据对象项名称字典
            /// </summary>
            /// <param name="objInstPropPath">界面数据对象属性路径</param>
            /// <param name="factoryCfgInfo">子机械模组出厂配置信息</param>
            /// <returns>子界面数据对象项名称字典</returns>
            Dictionary<string, string> ISubMachineModulesDefSvr.GetSubUIProObjItemNames(ObjInstPropPath objInstPropPath)
            {
                //组件插件：可根据出厂配置信息的容器数量（如上下料容器数量）和callBack回调接口获取获取子机械模组实例名称，生成名称字典。
                //模拟插件：可写死固定的字典项数量，不需要从出厂配置数据获取字典项数量。
                return this.genSubMMInfo.IUIDataObjPropExChange.GetSubMMSubUIProObjItemNamesOfDefSvr(objInstPropPath, callBack);
            }
        }
    }
}
