using GF_Gereric;
using Griffins;
using Griffins.Map;

namespace GKG.Map.UIDataObj.Station
{
    /// <summary>
    /// 瀹搞儰缍呴崝鐔诲厴閸ユ儳鍘撶€电懓绨查惃鍕櫕闂堛垺鏆熼幑顔碱嚠鐠?    /// </summary>
    public class UIDataObjStation : GFPropObjBase
    {
        [GFProp(GfPropReadWrite.ReadWrite)]
        public bool LeftHasBoard { get; set; }

        [GFProp(GfPropReadWrite.ReadWrite)]
        public bool RightHasBoard { get; set; }

        [GFProp(GfPropReadWrite.ReadWrite)]
        public bool ShowLeftSensorImage { get; set; }

        [GFProp(GfPropReadWrite.ReadWrite)]
        public bool ShowRightSensorImage { get; set; }

        [GFProp(GfPropReadWrite.ReadWrite)]
        public bool HasBoard { get; set; }
    }
}
