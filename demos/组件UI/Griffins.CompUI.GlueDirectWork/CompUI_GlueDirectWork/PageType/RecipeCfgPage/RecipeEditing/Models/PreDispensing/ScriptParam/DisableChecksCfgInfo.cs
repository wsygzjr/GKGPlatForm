using Newtonsoft.JsonG.Linq;

namespace Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.Models
{
    /// <summary>
    /// 禁用检测配置信息
    /// </summary>
    public class DisableChecksCfgInfo : ScriptParamBase
    {
        /// <summary>
        /// 从 JObject 反序列化
        /// </summary>
        public override void FromJObject(JObject? jObject)
        {
            if (jObject == null) return;
        }

        /// <summary>
        /// 序列化为 JObject
        /// </summary>
        public override JObject ToJObject()
        {
            return new JObject
            {
            };
        }

        /// <summary>
        /// 复制数据
        /// </summary>
        public void CopyFrom(DisableChecksCfgInfo source)
        {
            if (source == null) return;
        }
    }
}
