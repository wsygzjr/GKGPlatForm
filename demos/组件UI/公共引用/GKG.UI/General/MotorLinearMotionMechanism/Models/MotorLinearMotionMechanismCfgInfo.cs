using Newtonsoft.JsonG.Linq;

namespace GKG.UI.General
{
    /// <summary>
    /// 电机型直线移动机构配置
    /// </summary>
    public class MotorLinearMotionMechanismCfgInfo
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
        public MotorMoveType MoveType { get; set; } = MotorMoveType.MoveToSpecifiedPosition;

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
            MoveType = jObject["MoveType"] != null
                ? (MotorMoveType)jObject["MoveType"].Value<int>()
                : MoveType;
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
                { "MoveType", (int)MoveType }
            };

            return obj;
        }

        /// <summary>
        /// 从另一个实例拷贝（浅拷贝）
        /// </summary>
        public void CopyFrom(MotorLinearMotionMechanismCfgInfo source)
        {
            if (source == null)
            {
                return;
            }

            PositionNumber = source.PositionNumber;
            CoordinateValue = source.CoordinateValue;
            MoveType = source.MoveType;
        }
    }
}