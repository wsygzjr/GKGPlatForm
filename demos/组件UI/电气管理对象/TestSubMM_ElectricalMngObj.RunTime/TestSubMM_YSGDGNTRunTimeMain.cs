using GF_Gereric;
using Griffins.ImeIOT;
using GriffinsGeneralTestMM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: Plugin(SubMachineModulesMngAttribute.PLUGINKIND_Str, "{1B1607B6-6A41-48C0-8EB4-9A398317AD39}", "TestSubMM_ElectricalMngObj.RunTime")]

namespace TestSubMM_ElectricalMngObj.RunTime
{
    [SubMachineModulesMngAttribute(GenSubMMInfoDefine.SubMMModelStr)]
    //[PluginSupportCompany("GKG")]
    class TestSubMM_ElectricalMngObjRunTimeMain : GenTestSubMMRunTimeBase
    {
        public TestSubMM_ElectricalMngObjRunTimeMain() : base(GenSubMMInfoDefine.GenSubMMInfo)
        {
        }
    }
}
