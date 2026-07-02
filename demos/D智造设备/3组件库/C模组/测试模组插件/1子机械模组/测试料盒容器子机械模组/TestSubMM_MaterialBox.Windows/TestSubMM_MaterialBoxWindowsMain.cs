using GF_Gereric;
using Griffins.ImeIOT;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;
using GriffinsGeneralTestMM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestSubMM_MaterialBox;

[assembly: Plugin(CompUIAttribute.PLUGINKIND_Str, "{9DDEBFC2-CE0B-4D81-B315-05627755F3B3}", "TestSubMM_MaterialBox.Windows")]

namespace TestSubMM_MaterialBox.Windows
{
    [CompUI(GenSubMMInfoDefine.SubMMModelStr, ImeIOTConst.CompType_SubMMStr)]
    class TestSubMM_MaterialBoxWindowsMain : GenTestSubMMCompUIBase
    {
        public TestSubMM_MaterialBoxWindowsMain():
            base(GenSubMMInfoDefine.GenSubMMInfo, GenSubMMInfoDefine.GenSubMMDesignTimeInfoDic)
        {
        }
    }
}
