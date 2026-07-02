using Newtonsoft.JsonG.Linq;

namespace GKG.UI.General
{
    public class StandardCylinderSingleControlSingleLimitTextCfgInfo
    {
        public string ControlInput { get; set; } = string.Empty;

        public string LimitInput { get; set; } = string.Empty;

        public void CopyFrom(StandardCylinderSingleControlSingleLimitTextCfgInfo src)
        {
            if (src == null)
            {
                return;
            }

            ControlInput = src.ControlInput;
            LimitInput = src.LimitInput;
        }

        public void FromJObject(JObject? jObject)
        {
            if (jObject == null)
            {
                return;
            }

            ControlInput = jObject["ControlInput"]?.Value<string>() ?? ControlInput;
            LimitInput = jObject["LimitInput"]?.Value<string>() ?? LimitInput;
        }

        public JObject ToJObject()
        {
            return new JObject
            {
                { "ControlInput", ControlInput },
                { "LimitInput", LimitInput },
            };
        }
    }
}
