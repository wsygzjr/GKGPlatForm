using Newtonsoft.JsonG.Linq;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.TemplateSubPage.TrajectorySequence
{
    /// <summary>
    /// 平移参数
    /// </summary>
    public class TranslationParam
    {
        /// <summary>
        /// X轴平移量
        /// </summary>
        public decimal X { get; set; }

        /// <summary>
        /// Y轴平移量
        /// </summary>
        public decimal Y { get; set; }

        /// <summary>
        /// Z轴平移量
        /// </summary>
        public decimal Z { get; set; }

        /// <summary>
        /// 从JObject反序列化
        /// </summary>
        /// <param name="jObject">JSON对象</param>
        public void FromJObject(JObject? jObject)
        {
            if (jObject == null) return;

            if (jObject["X"] != null)
                X = jObject["X"].Value<decimal>();
            if (jObject["Y"] != null)
                Y = jObject["Y"].Value<decimal>();
            if (jObject["Z"] != null)
                Z = jObject["Z"].Value<decimal>();
        }

        /// <summary>
        /// 序列化为JObject
        /// </summary>
        /// <returns>JSON对象</returns>
        public JObject ToJObject()
        {
            var jObject = new JObject();
            jObject["X"] = X;
            jObject["Y"] = Y;
            jObject["Z"] = Z;
            return jObject;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public TranslationParam()
        {
        }
    }
}