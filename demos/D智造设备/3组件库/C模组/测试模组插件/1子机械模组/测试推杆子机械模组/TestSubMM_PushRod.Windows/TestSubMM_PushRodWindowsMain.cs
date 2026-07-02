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
using TestSubMM_PushRod;

[assembly: Plugin(CompUIAttribute.PLUGINKIND_Str, "{CA9234EF-FA9B-493C-8E7A-CCA886BD71D0}", "TestSubMM_PushRod.Windows")]

namespace TestSubMM_PushRod.Windows
{
    [CompUI(GenSubMMInfoDefine.SubMMModelStr, ImeIOTConst.CompType_SubMMStr)]
    class TestSubMM_PushRodWindowsMain : GenTestSubMMCompUIBase
    {
        public TestSubMM_PushRodWindowsMain():
            base(GenSubMMInfoDefine.GenSubMMInfo, GenSubMMInfoDefine.GenSubMMDesignTimeInfoDic)
        {
        }
    }
}
