using Newtonsoft.JsonG.Linq;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.Area;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.BasicParameters;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.GlueDispensingStyle;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.Plan;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.TemplateSubPage;
using System.Text;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models
{
    /// <summary>
    /// 配方参数配置信息
    /// </summary>
    public class ParametersAndCalculationCfgInfo
    {
        /// <summary>
        /// 基本参数配置信息
        /// </summary>
        public BasicParametersCfgInfo BasicParametersCfgInfo { set; get; }

        /// <summary>
        /// 点胶样式配置信息
        /// </summary>
        public DispensingStyleCfgInfo DispensingStyleCfgInfo { set; get; }

        /// <summary>
        /// 模板配置信息列表
        /// </summary>
        public TemplateCfgInfoList TemplateCfgInfoes { set; get; }

        /// <summary>
        /// 区域配置信息
        /// </summary>
        public AreaConfigInfo AreaConfigInfo { set; get; }

        /// <summary>
        /// 点胶方案配置信息
        /// </summary>
        public DispensingPlanConfigInfo PlanConfigInfo { set; get; }

        /// <summary>
        /// 基准点参数配置信息
        /// </summary>
        public DatumPointConfigInfo DatumPointConfigInfo { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ParametersAndCalculationCfgInfo()
        {
            BasicParametersCfgInfo = new BasicParametersCfgInfo();
            DispensingStyleCfgInfo = new DispensingStyleCfgInfo();
            TemplateCfgInfoes = new TemplateCfgInfoList();
            AreaConfigInfo = new AreaConfigInfo();
            PlanConfigInfo = new DispensingPlanConfigInfo();
            DatumPointConfigInfo = new DatumPointConfigInfo();
        }
        /// <summary>
        /// 序列化
        /// </summary>
        /// <returns></returns>
        public byte[] ToJsonBytes()
        {
            JObject jsonObj = this.ToJObject();
            string jsonStr = jsonObj.ToString();
            var jsonBytes= Encoding.UTF8.GetBytes(jsonStr);
            return jsonBytes;
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="bytes"></param>
        public void FromJSonBytes(byte[]bytes)
        {
            if (bytes == null)
                return;
            string jsonStr=Encoding.UTF8.GetString(bytes);
            JObject parseObj = JObject.Parse(jsonStr);
            this.FromJObject(parseObj);
        }

        /// <summary>
        /// 从JObject反序列化
        /// </summary>
        /// <param name="jObject">JSON对象</param>
        public void FromJObject(JObject? jObject)
        {
            if (jObject == null) return;

            //基本参数配置反序列化
            if (jObject["BasicParametersCfgInfo"] is JObject basicParamObj)
            {
                BasicParametersCfgInfo ??= new BasicParametersCfgInfo();
                BasicParametersCfgInfo.FromJObject(basicParamObj);
            }

            //点胶样式配置反序列化
            if (jObject["DispensingStyleCfgInfo"] is JObject dispensingStyleObj)
            {
                DispensingStyleCfgInfo ??= new DispensingStyleCfgInfo();
                DispensingStyleCfgInfo.FromJObject(dispensingStyleObj);
            }

            //模板配置列表反序列化
            if (jObject["TemplateCfgInfoes"] is JArray templateArray)
            {
                TemplateCfgInfoes ??= new TemplateCfgInfoList();
                TemplateCfgInfoes.FromJObject(templateArray);
            }

            //区域配置信息反序列化
            if (jObject["AreaConfigInfo"] is JObject areaObj)
            {
                AreaConfigInfo ??= new AreaConfigInfo();
                AreaConfigInfo.FromJObject(areaObj);
            }

            //点胶方案配置反序列化
            if (jObject["PlanConfigInfo"] is JObject planObj)
            {
                PlanConfigInfo ??= new DispensingPlanConfigInfo();
                PlanConfigInfo.FromJObject(planObj);
            }

            //基准点配置反序列化
            if (jObject["DatumPointConfigInfo"] is JObject datumPointObj)
            {
                DatumPointConfigInfo ??= new DatumPointConfigInfo();
                DatumPointConfigInfo.FromJObject(datumPointObj);
            }
        }

        /// <summary>
        /// 序列化为JObject
        /// </summary>
        /// <returns>JSON对象</returns>
        public JObject ToJObject()
        {
            var jObject = new JObject();

            //基本参数配置序列化
            jObject["BasicParametersCfgInfo"] = BasicParametersCfgInfo?.ToJObject() ?? new JObject();

            //点胶样式配置序列化
            jObject["DispensingStyleCfgInfo"] = DispensingStyleCfgInfo?.ToJObject() ?? new JObject();

            //模板配置列表序列化
            jObject["TemplateCfgInfoes"] = TemplateCfgInfoes?.ToJArray() ?? new JArray();

            //区域配置信息序列化
            jObject["AreaConfigInfo"] = AreaConfigInfo?.ToJObject() ?? new JObject();

            //点胶方案配置序列化
            jObject["PlanConfigInfo"] = PlanConfigInfo?.ToJObject() ?? new JObject();

            //基准点配置序列化
            jObject["DatumPointConfigInfo"] = DatumPointConfigInfo?.ToJObject() ?? new JObject();

            return jObject;
        }
    }

}

