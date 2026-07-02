using Griffins.ImeIOT;
using StandardSXLMachineModules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Griffins.StandardSXLMachineModules
{
    class MachineModulesCmdFactoryCFg : IMachineModulesCmdFactoryCFg
    {
        private readonly FactoryCfg factoryCfg;
        public MachineModulesCmdFactoryCFg() 
        {
            factoryCfg = new FactoryCfg();
        }

        /// <summary>
        /// 打开
        /// </summary>
        /// <param name="cfgInfo">老的配置参数，null表示缺省值</param>
        void IMachineModulesCmdFactoryCFg.Open(byte[] cfgInfo) 
        {
            factoryCfg.FromBytes(cfgInfo);
        }
        /// <summary>
        ///  关闭
        /// </summary>
        /// <returns>新的配置参数，null表示缺省值</returns>
        byte[] IMachineModulesCmdFactoryCFg.Close() 
        {
            return factoryCfg.ToBytes();
        }
        /// <summary>
        /// 执行配置命令
        /// </summary>
        /// <param name="cmdID">命令ID</param>
        /// <param name="cmdParam">命令参数</param>
        /// <returns>命令执行结果</returns>
        string IMachineModulesCmdFactoryCFg.ExecCfgCmd(string cmdID, string cmdParam) 
        {
            return string.Empty;
        }
    }
}
