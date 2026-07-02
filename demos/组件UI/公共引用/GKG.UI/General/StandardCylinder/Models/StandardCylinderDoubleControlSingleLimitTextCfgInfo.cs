using Newtonsoft.JsonG.Linq;

namespace GKG.UI.General
{
    public class StandardCylinderDoubleControlSingleLimitTextCfgInfo
    {
        public string OpenControlInput { get; set; } = string.Empty;

        public string CloseControlInput { get; set; } = string.Empty;

        public void CopyFrom(StandardCylinderDoubleControlSingleLimitTextCfgInfo src)
        {
            if (src == null)
            {
                return;
            }

            OpenControlInput = src.OpenControlInput;
            CloseControlInput = src.CloseControlInput;
        }

        public void FromJObject(JObject? jObject)
        {
            if (jObject == null)
            {
                return;
            }

            OpenControlInput = jObject["OpenControlInput"]?.Value<string>() ?? OpenControlInput;
            CloseControlInput = jObject["CloseControlInput"]?.Value<string>() ?? CloseControlInput;
        }

        public JObject ToJObject()
        {
            return new JObject
            {
                { "OpenControlInput", OpenControlInput },
                { "CloseControlInput", CloseControlInput },
            };
        }
    }
}
