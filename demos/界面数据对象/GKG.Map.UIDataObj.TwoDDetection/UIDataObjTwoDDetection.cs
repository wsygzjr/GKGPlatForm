using GF_Gereric;
using Griffins;
using Griffins.IOT;
using Griffins.Map;

namespace GKG.Map.UIDataObj.TwoDDetection
{
	public class UIDataObjTwoDDetection : GFPropObjBase
    {
        [GFProp(GfPropReadWrite.ReadOnly, "FeedbackInfo")]
		public string FeedbackInfo { get; set; }
    }
}
