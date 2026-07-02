using GF_Gereric;
using Griffins;
using Griffins.IOT;
using Griffins.Map;

namespace GKG.Map.UIDataObj.IconButton
{
    public class UIDataObjIconButton : GFPropObjBase
    {
        [GFProp(GfPropReadWrite.ReadWrite, "BackgroundColor", GriffinsBaseDataType.String)]
        public string BackgroundColor { get; set; } = "";

        [GFProp(GfPropReadWrite.ReadWrite, "BorderColor", GriffinsBaseDataType.String)]
        public string BorderColor { get; set; } = "";

        [GFProp(GfPropReadWrite.ReadWrite, "ForegroundColor", GriffinsBaseDataType.String)]
        public string ForegroundColor { get; set; } = "";

        [GFProp(GfPropReadWrite.ReadWrite)]
        public decimal Opacity { get; set; }

        [GFProp(GfPropReadWrite.ReadWrite)]
        public bool Visible { get; set; }

        [GFProp(GfPropReadWrite.ReadWrite, "ButtonText", GriffinsBaseDataType.String)]
        public string ButtonText { get; set; } = "";

    }
}
