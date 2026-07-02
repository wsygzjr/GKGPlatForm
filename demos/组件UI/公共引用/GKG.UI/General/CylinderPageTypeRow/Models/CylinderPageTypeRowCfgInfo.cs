using Newtonsoft.JsonG.Linq;

namespace GKG.UI.General
{
    /// <summary>
    /// Cylinder page type row model
    /// </summary>
    public class CylinderPageTypeRowCfgInfo
    {
        public string ComboValue { get; set; } = string.Empty;

        public string ComboDisplayName { get; set; } = string.Empty;

        public string Text { get; set; } = string.Empty;

        public void CopyFrom(CylinderPageTypeRowCfgInfo src)
        {
            if (src == null)
            {
                return;
            }

            ComboValue = src.ComboValue;
            ComboDisplayName = src.ComboDisplayName;
            Text = src.Text;
        }

        public void FromJObject(JObject? jObject)
        {
            if (jObject == null)
            {
                return;
            }

            ComboValue = jObject["ComboValue"]?.Value<string>() ?? ComboValue;
            ComboDisplayName = jObject["ComboDisplayName"]?.Value<string>() ?? ComboDisplayName;
            Text = jObject["Text"]?.Value<string>() ?? Text;
        }

        public JObject ToJObject()
        {
            return new JObject
            {
                { "ComboValue", ComboValue },
                { "ComboDisplayName", ComboDisplayName },
                { "Text", Text },
            };
        }
    }
}
