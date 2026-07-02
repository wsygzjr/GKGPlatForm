using Griffins;
using Griffins.ImeIOT;
using Griffins.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GriffinsGeneralTestMM
{

    #region 内部类型 
    // 内部事件定义信息（对应原InnerEventDefInfo）
    internal class InnerControlPanelDefInfo
    {
        /// <summary>
        ///  控制面板ID
        /// </summary>
        public string ControlPanelID { get; set; }
        /// <summary>
        ///  控制面板名称
        /// </summary>
        public string ControlPanelName { get; set; } 

        public override string ToString()
        {
            return string.IsNullOrEmpty(ControlPanelName) ? ControlPanelID.ToString() : ControlPanelName;
        }


        internal void CopyFrom(ControlPanelDefInfo info)
        {
            ControlPanelID = info.ControlPanelID;
            ControlPanelName = info.ControlPanelName; 
        }
    }

    /// <summary>
    /// 内部 控制面板列表 
    /// </summary>
    internal class InnerControlPanelDefInfoList : List<InnerControlPanelDefInfo>
    {
        public void CopyFrom(ControlPanelDefInfoList imeEventDefInfoes)
        {
            Clear();
            foreach (var genEvent in imeEventDefInfoes)
            {
                var innerEvent = new InnerControlPanelDefInfo();
                innerEvent.CopyFrom(genEvent);
                Add(innerEvent);
            }
        }
    }
    #endregion
}
