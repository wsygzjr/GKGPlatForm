using Newtonsoft.JsonG.Linq;
using System.Collections.Generic;

namespace Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.Models
{

    /// <summary>
    /// 脚本参数基类
    /// </summary>
    public class ScriptParamBase
    {
        /// <summary>
        /// 从JObject反序列化
        /// </summary>
        public virtual void FromJObject(JObject? jObject)
        {
            if (jObject == null) return;
        }

        /// <summary>
        /// 序列化为JObject
        /// </summary>
        public virtual JObject ToJObject()
        {
            return new JObject();
        }

        public virtual void CopyFrom(ScriptParamBase source)
        {
            if (source == null) return;
        }

        public ScriptParamBase()
        {
        }
    }

    /// <summary>
    /// 脚本参数配置信息
    /// </summary>
    public class ScriptParamCfgInfo
    {
        /// <summary>
        /// 脚本参数类型
        /// </summary>
        public ScriptParamType ParamType { get; set; } = ScriptParamType.MissingGlueDetection;

        /// <summary>
        /// 当前脚本参数配置
        /// </summary>
        public ScriptParamBase? ParamCfg { get; set; } = new MissingGlueDetectionCfgInfo();

        /// <summary>
        /// 从 JObject 反序列化
        /// </summary>
        public void FromJObject(JObject? jObject)
        {
            if (jObject == null) return;

            if (jObject["ParamType"] != null)
            {
                ParamType = (ScriptParamType)(jObject["ParamType"]?.Value<int>() ?? (int)ScriptParamType.MissingGlueDetection);
            }

            switch (ParamType)
            {
                case ScriptParamType.MissingGlueDetection:
                    var missing = new MissingGlueDetectionCfgInfo();
                    missing.FromJObject(jObject["MissingGlueDetectionCfgInfo"] as JObject);
                    ParamCfg = missing;
                    break;
                case ScriptParamType.MaxGlueAreaDetection:
                    var maxArea = new MaxGlueAreaDetectionCfgInfo();
                    maxArea.FromJObject(jObject["MaxGlueAreaDetectionCfgInfo"] as JObject);
                    ParamCfg = maxArea;
                    break;
                case ScriptParamType.GlueDiameterCountOffset:
                    var diameter = new GlueDiameterCountOffsetCfgInfo();
                    diameter.FromJObject(jObject["GlueDiameterCountOffsetCfgInfo"] as JObject);
                    ParamCfg = diameter;
                    break;
                case ScriptParamType.DisableChecks:
                    var disable = new DisableChecksCfgInfo();
                    disable.FromJObject(jObject["DisableChecksCfgInfo"] as JObject);
                    ParamCfg = disable;
                    break;
                case ScriptParamType.TotalAreaDetection:
                    var totalArea = new TotalAreaDetectionCfgInfo();
                    totalArea.FromJObject(jObject["TotalAreaDetectionCfgInfo"] as JObject);
                    ParamCfg = totalArea;
                    break;
                case ScriptParamType.GlueCountDetection:
                    var count = new GlueCountDetectionCfgInfo();
                    count.FromJObject(jObject["GlueCountDetectionCfgInfo"] as JObject);
                    ParamCfg = count;
                    break;
                default:
                    ParamCfg = new MissingGlueDetectionCfgInfo();
                    break;
            }
        }

        /// <summary>
        /// 序列化为 JObject
        /// </summary>
        public JObject ToJObject()
        {
            var jObject = new JObject
            {
                { "ParamType", (int)ParamType }
            };

            if (ParamCfg == null) return jObject;

            switch (ParamType)
            {
                case ScriptParamType.MissingGlueDetection:
                    if (ParamCfg is MissingGlueDetectionCfgInfo)
                        jObject["MissingGlueDetectionCfgInfo"] = ParamCfg.ToJObject();
                    break;
                case ScriptParamType.MaxGlueAreaDetection:
                    if (ParamCfg is MaxGlueAreaDetectionCfgInfo)
                        jObject["MaxGlueAreaDetectionCfgInfo"] = ParamCfg.ToJObject();
                    break;
                case ScriptParamType.GlueDiameterCountOffset:
                    if (ParamCfg is GlueDiameterCountOffsetCfgInfo)
                        jObject["GlueDiameterCountOffsetCfgInfo"] = ParamCfg.ToJObject();
                    break;
                case ScriptParamType.DisableChecks:
                    if (ParamCfg is DisableChecksCfgInfo)
                        jObject["DisableChecksCfgInfo"] = ParamCfg.ToJObject();
                    break;
                case ScriptParamType.TotalAreaDetection:
                    if (ParamCfg is TotalAreaDetectionCfgInfo)
                        jObject["TotalAreaDetectionCfgInfo"] = ParamCfg.ToJObject();
                    break;
                case ScriptParamType.GlueCountDetection:
                    if (ParamCfg is GlueCountDetectionCfgInfo)
                        jObject["GlueCountDetectionCfgInfo"] = ParamCfg.ToJObject();
                    break;
            }

            return jObject;
        }

        /// <summary>
        /// 复制数据
        /// </summary>
        public void CopyFrom(ScriptParamCfgInfo source)
        {
            if (source == null) return;

            ParamType = source.ParamType;

            switch (ParamType)
            {
                case ScriptParamType.MissingGlueDetection:
                    var missing = new MissingGlueDetectionCfgInfo();
                    if (source.ParamCfg is MissingGlueDetectionCfgInfo sMissing)
                        missing.CopyFrom(sMissing);
                    ParamCfg = missing;
                    break;
                case ScriptParamType.MaxGlueAreaDetection:
                    var maxArea = new MaxGlueAreaDetectionCfgInfo();
                    if (source.ParamCfg is MaxGlueAreaDetectionCfgInfo sMaxArea)
                        maxArea.CopyFrom(sMaxArea);
                    ParamCfg = maxArea;
                    break;
                case ScriptParamType.GlueDiameterCountOffset:
                    var diameter = new GlueDiameterCountOffsetCfgInfo();
                    if (source.ParamCfg is GlueDiameterCountOffsetCfgInfo sDiameter)
                        diameter.CopyFrom(sDiameter);
                    ParamCfg = diameter;
                    break;
                case ScriptParamType.DisableChecks:
                    var disable = new DisableChecksCfgInfo();
                    if (source.ParamCfg is DisableChecksCfgInfo sDisable)
                        disable.CopyFrom(sDisable);
                    ParamCfg = disable;
                    break;
                case ScriptParamType.TotalAreaDetection:
                    var totalArea = new TotalAreaDetectionCfgInfo();
                    if (source.ParamCfg is TotalAreaDetectionCfgInfo sTotalArea)
                        totalArea.CopyFrom(sTotalArea);
                    ParamCfg = totalArea;
                    break;
                case ScriptParamType.GlueCountDetection:
                    var count = new GlueCountDetectionCfgInfo();
                    if (source.ParamCfg is GlueCountDetectionCfgInfo sCount)
                        count.CopyFrom(sCount);
                    ParamCfg = count;
                    break;
            }
        }
    }

    /// <summary>
    /// 脚本参数配置信息列表
    /// </summary>
    public class ScriptParamCfgInfoList : List<ScriptParamCfgInfo>
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
                    var item = new ScriptParamCfgInfo();
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
