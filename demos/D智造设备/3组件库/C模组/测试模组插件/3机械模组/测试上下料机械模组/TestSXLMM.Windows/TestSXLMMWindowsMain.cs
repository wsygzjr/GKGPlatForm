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

[assembly: Plugin(CompUIAttribute.PLUGINKIND_Str, "{1FCE9AC9-4CBA-4BCA-A684-2AF755641F9A}", "TestSXLMM.Windows")]

namespace TestSXLMM.Windows
{
    /// <summary>
    /// 标准上下料机械模组
    /// </summary>
    [CompUI(GenMMInfoDefine.MMNumberStr, ImeIOTConst.CompType_MMStr)]
    //[PluginSupportCompany("GKG")]
    public class TestSXLMMWindowsMain : GenTestMMCompUIBase
    {
        public TestSXLMMWindowsMain() 
            : base(GenMMInfoDefine.GenMMInfo, GenMMInfoDefine.GenMMDesignTimeInfo)
        {
        }
    }
}
