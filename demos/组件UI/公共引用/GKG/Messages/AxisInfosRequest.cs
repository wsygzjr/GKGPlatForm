using Griffins;
using Griffins.PF;

namespace GKG
{
    /// <summary>
    /// AxisInfos 请求消息，获取轴列表信息
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    public class AxisInfosRequest : MutualInfoBase
    {
        /// <summary>
        /// 互斥消息种类ID。
        /// </summary>
        public static readonly GriffinsInfoKindID InfoKindID = new GriffinsInfoKindID("{559307B8-230D-441E-A2A1-59984FA5FD3E}");

        /// <summary>
        /// 创建空消息实例，供反序列化使用。
        /// </summary>
        public AxisInfosRequest()
        {

        }

        public AxisInfosRequest(MotionControlCardType cardType)
        {
            this.CardType = cardType;
        }

        public MotionControlCardType CardType { get; set; } = MotionControlCardType.Normal;
    }
}
