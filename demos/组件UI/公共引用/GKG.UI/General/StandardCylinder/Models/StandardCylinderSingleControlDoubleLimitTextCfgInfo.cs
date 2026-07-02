using Newtonsoft.JsonG.Linq;

namespace GKG.UI.General
{
    public class StandardCylinderSingleControlDoubleLimitTextCfgInfo
    {
        public string ControlInput { get; set; } = string.Empty;

        public string UpperLimitInput { get; set; } = string.Empty;

        public string LowerLimitInput { get; set; } = string.Empty;

        public void CopyFrom(StandardCylinderSingleControlDoubleLimitTextCfgInfo src)
        {
            if (src == null)
            {
                return;
            }

            ControlInput = src.ControlInput;
            UpperLimitInput = src.UpperLimitInput;
            LowerLimitInput = src.LowerLimitInput;
        }

        public void FromJObject(JObject? jObject)
        {
            if (jObject == null)
            {
                return;
            }

            ControlInput = jObject["ControlInput"]?.Value<string>() ?? ControlInput;
            UpperLimitInput = jObject["UpperLimitInput"]?.Value<string>() ?? UpperLimitInput;
            LowerLimitInput = jObject["LowerLimitInput"]?.Value<string>() ?? LowerLimitInput;
        }

        public JObject ToJObject()
        {
            return new JObject
            {
                { "ControlInput", ControlInput },
                { "UpperLimitInput", UpperLimitInput },
                { "LowerLimitInput", LowerLimitInput },
            };
        }
    }
}
