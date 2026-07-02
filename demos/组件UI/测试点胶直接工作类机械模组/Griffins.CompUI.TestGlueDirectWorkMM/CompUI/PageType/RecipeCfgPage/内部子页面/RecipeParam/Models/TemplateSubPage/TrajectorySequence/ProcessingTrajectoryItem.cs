using Newtonsoft.JsonG.Linq;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.TemplateSubPage.TrajectorySequence
{
    /// <summary>
    /// 加工轨迹项
    /// </summary>
    public class ProcessingTrajectoryItem: EntityBase
    {
        /// <summary>
        /// 计算轨迹类型
        /// </summary>
        public CalculateTrajectoryType CalculateTrajectoryType { get; set; }

        /// <summary>
        /// 计算轨迹实例（根据轨迹类型动态创建）
        /// </summary>
        public CalculateTrajectoryBase CalculateTrajectory { get; set; }

        /// <summary>
        /// 工艺参数（动态类型，存储具体加工参数）
        /// </summary>
        public object ProcessParameters { get; set; }

        /// <summary>
        /// 从JObject反序列化
        /// </summary>
        /// <param name="jObject">JSON对象</param>
        public new void FromJObject(JObject? jObject)
        {
            if (jObject == null) return;
            base.FromJObject(jObject);  
            if (jObject["CalculateTrajectoryType"] != null)
                CalculateTrajectoryType = (CalculateTrajectoryType)jObject["CalculateTrajectoryType"].Value<int>();

            if (jObject["CalculateTrajectory"] is JObject trajectoryObj)
            {
                CalculateTrajectory ??= CalculateTrajectoryType switch
                {
                    CalculateTrajectoryType.Point => new PointCalculateTrajectory(),
                    CalculateTrajectoryType.Line => new LineCalculateTrajectory(),
                    _ => new CalculateTrajectoryBase()
                };
                CalculateTrajectory.FromJObject(trajectoryObj);
            }

            if (jObject["ProcessParameters"] != null)
                ProcessParameters = jObject["ProcessParameters"].ToObject<object>();
        }

        /// <summary>
        /// 序列化为JObject
        /// </summary>
        /// <returns>JSON对象</returns>
        public new JObject ToJObject()
        {
            var jObject = new JObject();
            jObject["CalculateTrajectoryType"] = (int)CalculateTrajectoryType;
            jObject["CalculateTrajectory"] = CalculateTrajectory?.ToJObject() ?? new JObject();
            jObject["ProcessParameters"] = JToken.FromObject(ProcessParameters);
            return jObject;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ProcessingTrajectoryItem()
        {
            CalculateTrajectory = new CalculateTrajectoryBase();
            ProcessParameters = new object();
        }
    }
}