using Newtonsoft.JsonG.Linq;
using System;
using System.Collections.ObjectModel;

namespace Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.Models
{
    /// <summary>
    /// 脚本模型
    /// </summary>
    public class ScriptModel
    {
        /// <summary>
        /// 脚本名称
        /// </summary>
        public string ScriptName { get; set; } = "script0";

        /// <summary>
        /// 脚本结果
        /// </summary>
        public ScriptResultType ScriptResult { get; set; } = ScriptResultType.Default;

        /// <summary>
        /// 脚本参数配置
        /// </summary>
        public ScriptParamCfgInfo ScriptParamCfgInfo { get; set; } = new();

        /// <summary>
        /// 图像预处理配置
        /// </summary>
        public ImagePreProcessCfgInfo ImagePreProcessCfgInfo { get; set; } = new();

        /// <summary>
        /// 通过条件配置
        /// </summary>
        public PassConditionCfgInfo PassConditionCfgInfo { get; set; } = new();

        /// <summary>
        /// 检测逻辑
        /// </summary>
        public DetectionLogicType DetectionLogicType { get; set; } = DetectionLogicType.AllPass;


        /// <summary>
        /// 从 JObject 反序列化
        /// </summary>
        /// <param name="jObject">JSON 对象</param>
        public void FromJObject(JObject? jObject)
        {
            if (jObject == null) return;

            ScriptName = jObject["ScriptName"]?.ToString() ?? "script0";
            if (Enum.TryParse(jObject["ScriptResult"]?.ToString(), out ScriptResultType result))
            {
                ScriptResult = result;
            }

            if (jObject["ScriptParamCfgInfo"] is JObject scriptParamJObject)
            {
                ScriptParamCfgInfo ??= new ScriptParamCfgInfo();
                ScriptParamCfgInfo.FromJObject(scriptParamJObject);
            }

            if (jObject["ImagePreProcessCfgInfo"] is JObject imagePreProcessJObject)
            {
                ImagePreProcessCfgInfo ??= new ImagePreProcessCfgInfo();
                ImagePreProcessCfgInfo.FromJObject(imagePreProcessJObject);
            }

            if (jObject["PassConditionCfgInfo"] is JObject passConditionJObject)
            {
                PassConditionCfgInfo ??= new PassConditionCfgInfo();
                PassConditionCfgInfo.FromJObject(passConditionJObject);
            }

            DetectionLogicType = jObject["DetectionLogicType"] != null
                ? (DetectionLogicType)jObject["DetectionLogicType"].Value<int>()
                : DetectionLogicType;
        }

        /// <summary>
        /// 序列化为 JObject
        /// </summary>
        /// <returns>JSON 对象</returns>
        public JObject ToJObject()
        {
            var jObject = new JObject
            {
                { "ScriptName", ScriptName },
                { "ScriptResult", ScriptResult.ToString() },
                { "ScriptParamCfgInfo", ScriptParamCfgInfo.ToJObject() },
                { "ImagePreProcessCfgInfo", ImagePreProcessCfgInfo.ToJObject() },
                { "PassConditionCfgInfo", PassConditionCfgInfo.ToJObject() },
                { "DetectionLogicType", (int)DetectionLogicType },
            };

            return jObject;
        }
    }


    /// <summary>
    /// 脚本模型列表
    /// </summary>
    public class ScriptModelList : ObservableCollection<ScriptModel>
    {
        /// <summary>
        /// 从JArray反序列化
        /// </summary>
        /// <param name="jArray">JSON数组</param>
        public void FromJObject(JArray? jArray)
        {
            if (jArray == null) return;

            Clear();
            foreach (var itemObj in jArray)
            {
                if (itemObj is JObject itemJObject)
                {
                    var item = new ScriptModel();
                    item.FromJObject(itemJObject);
                    Add(item);
                }
            }
        }

        /// <summary>
        /// 序列化为JArray
        /// </summary>
        /// <returns>JSON数组</returns>
        public JArray ToJArray()
        {
            var jArray = new JArray();
            foreach (var item in this)
            {
                jArray.Add(item.ToJObject());
            }
            return jArray;
        }
    }
}
