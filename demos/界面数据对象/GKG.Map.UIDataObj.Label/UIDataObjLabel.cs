using GF_Gereric;
using Griffins;
using Griffins.IOT;
using Griffins.Map;

namespace GKG.Map.UIDataObj.Label
{
    public class UIDataObjLabel : GFPropObjBase
    {
        [GFProp(GfPropReadWrite.ReadWrite, "HasBoard", GriffinsBaseDataType.Bool)]
        public bool HasBoard { get; set; } = false;

        [GFProp(GfPropReadWrite.ReadWrite, "LeftSensorStatus", GriffinsBaseDataType.Bool)]
        public bool LeftSensorStatus { get; set; } = false;

        [GFProp(GfPropReadWrite.ReadWrite, "RightSensorStatus", GriffinsBaseDataType.Bool)]
        public bool RightSensorStatus { get; set; } = false;

        // µ²°å×´Ì¬£º0 = Stretch(Éì³ö), 1 = Retract(Ëõ»Ø), 2 = UnNormal(Òì³£ÉÁË¸)
        [GFProp(GfPropReadWrite.ReadWrite, "LeftJackingState", GriffinsBaseDataType.Integer)]
        public int LeftJackingState { get; set; } = 1;

        // µ²°å×´Ì¬£º0 = Stretch(Éì³ö), 1 = Retract(Ëõ»Ø), 2 = UnNormal(Òì³£ÉÁË¸)
        [GFProp(GfPropReadWrite.ReadWrite, "RightJackingState", GriffinsBaseDataType.Integer)]
        public int RightJackingState { get; set; } = 1;
    }
}
