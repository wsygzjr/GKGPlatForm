using Griffins.ImeIOT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualAnalysisPositionAdjustBasc
{
    public class BscCabilityDef: IBscCabilityDef
    {
        /// <summary>
        ///  机械模组的机械模组能力事件列表
        /// </summary>
        ImeCabilityEventDefInfoList IBscCabilityDef.Events 
        {
            get { return null; }
        }
        /// <summary>
        ///  机械模组的机械模组能力方法列表
        /// </summary>
        ImeCabilityMethodDefInfoList IBscCabilityDef.Methods
        {
            get { return null; }
        }
    }
}
