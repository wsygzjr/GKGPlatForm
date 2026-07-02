using GF_Gereric;
using Griffins.IOT;
using Griffins.Map;

namespace Griffins.Map.UIDataObj.SL
{
	public class UIDataObjSL: GFPropObjBase
    {
        [GFProp(GfPropReadWrite.ReadWrite)]
		public int Param1 { get; set; }

        [GFProp(GfPropReadWrite.ReadWrite, "Param_2")]
        public decimal Param2 { get; set; }

        [GFProp(GfPropReadWrite.ReadWrite, "Param_3", GriffinsBaseDataType.String, "{131F5283-8AB7-47C6-BE94-4E8B0580AC8F}")]
        public string Param3 { get; set; } = "";

        [GFProp(GfPropReadWrite.ReadWrite, "Param_4")]
        public bool Param4 { get; set; }

        [GFProp(GfPropReadWrite.ReadWrite)]
        public int RemainingCount { get; set; }
    }
}
