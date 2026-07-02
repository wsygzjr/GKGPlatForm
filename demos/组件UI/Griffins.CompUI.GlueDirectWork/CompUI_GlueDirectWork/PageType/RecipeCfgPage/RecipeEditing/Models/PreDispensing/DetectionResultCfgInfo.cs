using Newtonsoft.JsonG.Linq;

namespace Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.Models
{
    /// <summary>
    /// 检测结果配置信息
    /// </summary>
    public class DetectionResultCfgInfo
    {
        /// <summary>
        /// 脚本参数列表
        /// </summary>
        public ScriptModelList ScriptModelList { get; set; } = new();

        /// <summary>
        /// 从 JObject 反序列化
        /// </summary>
        public void FromJObject(JObject? jObject)
        {
            if (jObject == null) return;

            if (jObject["ScriptParamCfgInfoList"] is JArray ScriptParamArray)
            {
                ScriptModelList ??= new();
                ScriptModelList.FromJObject(ScriptParamArray);
            }
        }

        /// <summary>
        /// 序列化为 JObject
        /// </summary>
        public JObject ToJObject()
        {
            return new JObject
            {
                { "ScriptParamCfgInfoList", ScriptModelList.ToJArray() }
            };
        }

        /// <summary>
        /// 复制数据
        /// </summary>
        public void CopyFrom(DetectionResultCfgInfo source)
        {
            if (source == null) return;

            ScriptModelList = new();
        }
    }
}