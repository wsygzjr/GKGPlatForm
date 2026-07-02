using Newtonsoft.JsonG.Linq;

namespace GKG.UI.General
{
    /// <summary>
    /// 气缸型移动机构配置
    /// </summary>
    public class CylinderMotionMechanismCfgInfo
    {
        /// <summary>
        /// 伸缩状态
        /// </summary>
        public bool ExtendStatus { get; set; } = false;

        /// <summary>
        /// 位置编号
        /// </summary>
        public int PositionNumber { get; set; } = 1;

        /// <summary>
        /// 从 JObject 反序列化
        /// </summary>
        public void FromJObject(JObject? jObject)
        {
            if (jObject == null)
            {
                return;
            }

            ExtendStatus = jObject["ExtendStatus"]?.Value<bool>() ?? ExtendStatus;
            PositionNumber = jObject["PositionNumber"]?.Value<int>() ?? PositionNumber;
        }

        /// <summary>
        /// 序列化为 JObject
        /// </summary>
        public JObject ToJObject()
        {
            var obj = new JObject
            {
                { "PositionNumber", ExtendStatus },
                { "CoordinateValue", PositionNumber },
            };

            return obj;
        }

        /// <summary>
        /// 从另一个实例拷贝
        /// </summary>
        public void CopyFrom(CylinderMotionMechanismCfgInfo source)
        {
            if (source == null)
            {
                return;
            }

            ExtendStatus = source.ExtendStatus;
            PositionNumber = source.PositionNumber;
        }
    }
}
