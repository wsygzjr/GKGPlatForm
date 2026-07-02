using GF_Gereric;
using Griffins.ImeIOT;
using GriffinsGeneralTestMM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: Plugin(SubMachineModulesMngAttribute.PLUGINKIND_Str, "{C54C397C-7353-4BEB-9DF5-D74C1E2B7518}", "TestSubMM_MaterialBox.RunTime")]

namespace TestSubMM_MaterialBox.RunTime
{
    [SubMachineModulesMngAttribute(GenSubMMInfoDefine.SubMMModelStr, "UpDown")]
    class TestSubMM_MaterialBoxRunTimeMain : GenTestSubMMRunTimeBase
    {
        public TestSubMM_MaterialBoxRunTimeMain() : base(GenSubMMInfoDefine.GenSubMMInfo)
        {
        }
    }
}
