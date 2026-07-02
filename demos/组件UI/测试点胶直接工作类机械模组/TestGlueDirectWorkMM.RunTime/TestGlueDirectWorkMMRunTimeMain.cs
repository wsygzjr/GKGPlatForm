using GF_Gereric;
using Griffins.ImeIOT;
using GriffinsGeneralTestMM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestGlueDirectWorkMM;

[assembly: Plugin(MachineModulesMngAttribute.PLUGINKIND_Str, "{F181523A-521F-4D6F-9DF7-98088CCDB6A8}", "TestGlueDirectWorkMM.RunTime")]

namespace TestGlueDirectWorkMM.RunTime
{
    /// <summary>
    /// 标准上下料机械模组
    /// </summary>
    [MachineModulesMng(GenMMInfoDefine.MMNumberStr)]
    //[PluginSupportCompany("GKG")]
    public class TestGlueDirectWorkMMRunTimeMain : GenTestMMRunTimeBase
    {
        public TestGlueDirectWorkMMRunTimeMain():base(GenMMInfoDefine.GenMMInfo)
        {
        }
       
    }
}
