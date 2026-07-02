using Avalonia.Threading;
using GF_Gereric;
using Griffins.ImeIOT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GriffinsGeneralTestMM
{
    /// <summary>
    ///  子机械模组运行时插件接口实现对象基类
    /// </summary>
    public abstract class GenTestSubMMRunTimeBase : GriffinsPluginMngClass, ISubMachineModulesRunTimePlugin
    {
        private GenSubMMInfo genSubMMInfo;
        public GenTestSubMMRunTimeBase(GenSubMMInfo genSubMMInfo)
        {
            this.genSubMMInfo = genSubMMInfo;
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="pluginPath">插件路径</param>
        void ISubMachineModulesRunTimePlugin.Init(string pluginPath)
        {
           
        }
        /// <summary>
        /// 机械模组名称
        /// </summary>
        string ISubMachineModulesMng.SubMMName { get { return genSubMMInfo.SubMMName; } }

		SubMMObjInfoList ISubMachineModulesMng.SubMMObjInfos 
        {
            get 
            {
                return genSubMMInfo.SubMMObjInfos;
			}
        }

		ISubMachineModulesCabilityDef ISubMachineModulesMng.CretaeCabilityDef()
        {
            return TestSubMMMain.CretaeSubMMCabilityDef(genSubMMInfo);
        }

        ///// <summary>
        ///// 创建子机械模组配置接口实例
        ///// </summary>
        ///// <param name="alias">子机械模组实例别名</param>
        ///// <param name="subMMObjID">子机械模组实现对象ID</param>
        ///// <returns>子机械模组配置接口实例</returns>
        //ISubMachineModulesConfig ISubMachineModulesMng.CretaeSubMMConfig(SubMMAlias alias, Guid subMMObjID)
        //{
        //   return TestSubMMMain.CreateSubMMConfig(genSubMMInfo, alias);
        //}

        /// <summary>
        /// 创建子机械模组（复合子机械模组）命令执行对象接口实例
        /// </summary>
        /// <param name="alias">子机械模组实例别名</param>
        /// <param name="subMMObjID">子机械模组实现对象ID</param>
        /// <param name="factoryCfgInfo">子机械模组出厂配置信息</param>
        /// <returns>子机械模组（复合子机械模组）命令执行对象接口实例</returns>
        ISubMMCmdExecutor ISubMachineModulesMng.CreateSubMMCmdExecutor(SubMMAlias alias, Guid subMMObjID, byte[] factoryCfgInfo)
        {
            return _CreateSubMMCmdExecutor(alias, subMMObjID,factoryCfgInfo);
        }
        protected virtual ISubMMCmdExecutor _CreateSubMMCmdExecutor(SubMMAlias alias, Guid subMMObjID, byte[] factoryCfgInfo)
        {
            return Dispatcher.UIThread.Invoke(() =>
            {
                TestSubMMMain.Init();
                return TestSubMMMain.CreateSubMMCmdExecutor(genSubMMInfo, alias);
            });
        }
    }
}
