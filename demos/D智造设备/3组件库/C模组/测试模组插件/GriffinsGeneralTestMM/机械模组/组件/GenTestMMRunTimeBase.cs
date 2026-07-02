using Avalonia.Threading;
using GF_Gereric;
using Griffins;
using Griffins.ImeIOT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GriffinsGeneralTestMM
{
    /// <summary>
    ///  机械模组运行时插件接口实现对象基类
    /// </summary>
    public abstract class GenTestMMRunTimeBase : GriffinsPluginMngClass, IMachineModulesPlugin
    {
        private GenMMInfo genMMInfo;

        public GenTestMMRunTimeBase(GenMMInfo genMMInfo)
        {
            this.genMMInfo = genMMInfo;
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="pluginPath">插件路径</param>
        void IMachineModulesPlugin.Init(string pluginPath)
        {
           
        }
        /// <summary>
        /// 机械模组名称
        /// </summary>
        string IMachineModulesMng.MMName { get { return genMMInfo.MMName; } }

        IMachineModulesCabilityDef IMachineModulesMng.CretaeCabilityDef()
        {
            return TestMMMain.CretaeMMCabilityDef(genMMInfo);
        }
		/// <summary>
		/// 创建机械模组命令执行对象接口实例
		/// </summary>
		/// <param name="alias">机械模组实例别名</param>
		/// <param name="factoryCfgInfo">机械模组出厂配置信息</param>
		/// <returns>机械模组命令执行对象接口实例</returns>
		IMMCmdExecutor IMachineModulesMng.CreateMMCmdExecutor(MMAlias alias, byte[] factoryCfgInfo)
        {
            return _CreateMMCmdExecutor(alias, factoryCfgInfo);
        }
        protected virtual IMMCmdExecutor _CreateMMCmdExecutor(MMAlias alias, byte[] factoryCfgInfo)
        {
            return Dispatcher.UIThread.Invoke(() =>
            {
                TestMMMain.Init();
                return TestMMMain.CreateMMCmdExecutor(genMMInfo, alias);
            });
        }
    }
}
