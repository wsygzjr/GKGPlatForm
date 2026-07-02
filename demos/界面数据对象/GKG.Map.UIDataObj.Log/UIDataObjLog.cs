using GF_Gereric;
using Griffins;
using Griffins.Map;

namespace GKG.Map.UIDataObj.Log
{
    public class UIDataObjLog : GFPropObjBase
    {
        [GFProp(GfPropReadWrite.ReadWrite, "RunningLog", GriffinsBaseDataType.String)]
        public string? RunningLog { get; set; }

        [GFProp(GfPropReadWrite.ReadWrite, "AlarmLog", GriffinsBaseDataType.String)]
        public string? AlarmLog { get; set; }
    }
}
