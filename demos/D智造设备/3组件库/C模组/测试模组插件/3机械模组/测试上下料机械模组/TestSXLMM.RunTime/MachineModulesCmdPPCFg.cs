using Griffins.ImeIOT;
using StandardSXLMachineModules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Griffins.StandardSXLMachineModules
{
    class MachineModulesCmdPPCFg : IMachineModulesCmdPPCFg
    {
        private readonly PPCfg pPCfg;
        public MachineModulesCmdPPCFg()
        {
            pPCfg = new PPCfg();
        }
        /// <summary>
        /// 打开
        /// </summary>
        /// <param name="cfgInfo">老的配置参数，null表示缺省值</param>
        void IMachineModulesCmdPPCFg.Open(byte[] cfgInfo) 
        {
            pPCfg.FromBytes(cfgInfo);
        }
        /// <summary>
        ///  关闭
        /// </summary>
        /// <returns>新的配置参数，null表示缺省值</returns>
        byte[] IMachineModulesCmdPPCFg.Close() 
        {
            return pPCfg.ToBytes();
        }
        /// <summary>
        /// 执行配置命令
        /// </summary>
        /// <param name="cmdID">命令ID</param>
        /// <param name="cmdParam">命令参数</param>
        /// <returns>命令执行结果</returns>
        string IMachineModulesCmdPPCFg.ExecCfgCmd(string cmdID, string cmdParam) 
        {
            return string.Empty;
        }
    }
}
