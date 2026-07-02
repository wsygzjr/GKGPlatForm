using GF_Gereric;
using Griffins.ImeIOT;
using GriffinsGeneralTestMM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: Plugin(SubMachineModulesMngAttribute.PLUGINKIND_Str, "{CA9234EF-FA9B-493C-8E7A-CCA886BD71D0}", "TestSubMM_PushRod.RunTime")]

namespace TestSubMM_PushRod.RunTime
{
    [SubMachineModulesMngAttribute(GenSubMMInfoDefine.SubMMModelStr, "UpDown")]
    //[PluginSupportCompany("GKG")]
    class TestSubMM_PushRodRunTimeMain : GenTestSubMMRunTimeBase
    {
        public TestSubMM_PushRodRunTimeMain() : base(GenSubMMInfoDefine.GenSubMMInfo)
        {
        }
    }
}
