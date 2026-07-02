using Griffins.ImeIOT;
using StandardPrintingMachineModules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Griffins.StandardPrintingMachineModules
{
	class MachineModulesCmdInitCFg: IMachineModulesCmdInitCFg
    {
        private readonly InitCfg initCfg;
        public MachineModulesCmdInitCFg() 
        {
            initCfg = new InitCfg();
        }
        /// <summary>
        /// 打开
        /// </summary>
        /// <param name="cfgInfo">老的配置参数，null表示缺省值</param>
        void IMachineModulesCmdInitCFg.Open(byte[] cfgInfo) 
        {
            initCfg.FromBytes(cfgInfo);
        }
        /// <summary>
        ///  关闭
        /// </summary>
        /// <returns>新的配置参数，null表示缺省值</returns>
        byte[] IMachineModulesCmdInitCFg.Close() 
        {
            return initCfg.ToBytes();
        }
        /// <summary>
        /// 执行配置命令
        /// </summary>
        /// <param name="cmdID">命令ID</param>
        /// <param name="cmdParam">命令参数</param>
        /// <returns>命令执行结果</returns>
        string IMachineModulesCmdInitCFg.ExecCfgCmd(string cmdID, string cmdParam) 
        {
            return string.Empty;
        }
    }
}
