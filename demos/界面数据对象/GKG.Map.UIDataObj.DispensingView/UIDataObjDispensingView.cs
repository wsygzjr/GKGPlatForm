using GF_Gereric;
using Griffins;
using Griffins.Map;

namespace GKG.Map.UIDataObj.DispensingView
{
    /// <summary>
    /// 点胶视图功能图元对应的界面数据对象
    /// </summary>
    public class UIDataObjDispensingView : GFPropObjBase
    {
        [GFProp(GfPropReadWrite.ReadWrite)]
        public string? ImageUri { get; set; }
    }
}
