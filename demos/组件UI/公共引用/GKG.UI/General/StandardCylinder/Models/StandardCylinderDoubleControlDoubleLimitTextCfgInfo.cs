using Newtonsoft.JsonG.Linq;

namespace GKG.UI.General
{
    public class StandardCylinderDoubleControlDoubleLimitTextCfgInfo
    {
        public string OpenControlInput { get; set; } = string.Empty;

        public string CloseControlInput { get; set; } = string.Empty;

        public string UpperLimitInput { get; set; } = string.Empty;

        public string LowerLimitInput { get; set; } = string.Empty;

        public void CopyFrom(StandardCylinderDoubleControlDoubleLimitTextCfgInfo src)
        {
            if (src == null)
            {
                return;
            }

            OpenControlInput = src.OpenControlInput;
            CloseControlInput = src.CloseControlInput;
            UpperLimitInput = src.UpperLimitInput;
            LowerLimitInput = src.LowerLimitInput;
        }

        public void FromJObject(JObject? jObject)
        {
            if (jObject == null)
            {
                return;
            }

            OpenControlInput = jObject["OpenControlInput"]?.Value<string>() ?? OpenControlInput;
            CloseControlInput = jObject["CloseControlInput"]?.Value<string>() ?? CloseControlInput;
            UpperLimitInput = jObject["UpperLimitInput"]?.Value<string>() ?? UpperLimitInput;
            LowerLimitInput = jObject["LowerLimitInput"]?.Value<string>() ?? LowerLimitInput;
        }

        public JObject ToJObject()
        {
            return new JObject
            {
                { "OpenControlInput", OpenControlInput },
                { "CloseControlInput", CloseControlInput },
                { "UpperLimitInput", UpperLimitInput },
                { "LowerLimitInput", LowerLimitInput },
            };
        }
    }
}
