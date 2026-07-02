using Newtonsoft.JsonG.Linq;

namespace GKG.UI.General
{
    /// <summary>
    /// 电机型旋转移动机构配置
    /// </summary>
    public class MotorRotateMotionMechanismCfgInfo
    {
        /// <summary>
        /// 位置编号
        /// </summary>
        public int PositionNumber { get; set; } = 1;

        /// <summary>
        /// 坐标值
        /// </summary>
        public decimal CoordinateValue { get; set; } = 0m;

        /// <summary>
        /// 移动类型
        /// </summary>
        public MotorRotateMoveType MotorRotateMoveType { get; set; } = MotorRotateMoveType.MoveToSpecifiedPosition;

        /// <summary>
        /// 从 JObject 反序列化
        /// </summary>
        public void FromJObject(JObject? jObject)
        {
            if (jObject == null)
            {
                return;
            }

            PositionNumber = jObject["PositionNumber"]?.Value<int>() ?? PositionNumber;
            CoordinateValue = jObject["CoordinateValue"]?.Value<decimal>() ?? CoordinateValue;
            MotorRotateMoveType = jObject["MotorRotateMoveType"] != null
                ? (MotorRotateMoveType)jObject["MotorRotateMoveType"].Value<int>()
                : MotorRotateMoveType;
        }

        /// <summary>
        /// 序列化为 JObject
        /// </summary>
        public JObject ToJObject()
        {
            var obj = new JObject
            {
                { "PositionNumber", PositionNumber },
                { "CoordinateValue", CoordinateValue },
                { "MotorRotateMoveType", (int)MotorRotateMoveType }
            };

            return obj;
        }

        /// <summary>
        /// 从另一个实例拷贝（浅拷贝）
        /// </summary>
        public void CopyFrom(MotorRotateMotionMechanismCfgInfo source)
        {
            if (source == null)
            {
                return;
            }

            PositionNumber = source.PositionNumber;
            CoordinateValue = source.CoordinateValue;
            MotorRotateMoveType = source.MotorRotateMoveType;
        }
    }
}
