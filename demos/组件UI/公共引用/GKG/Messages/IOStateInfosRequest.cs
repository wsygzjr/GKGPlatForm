using Griffins;
using Griffins.PF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GKG
{
    public class IOStateInfosRequest : MutualInfoBase
    {
        /// <summary>
        /// 互斥消息种类ID。
        /// </summary>
        public static readonly GriffinsInfoKindID InfoKindID = new GriffinsInfoKindID("{C18EF8DF-75C3-4996-A723-5C0CCE6882EF}");

        /// <summary>
        /// 创建空消息实例，供反序列化使用。
        /// </summary>
        public IOStateInfosRequest()
        {

        }

        public IOStateInfosRequest(MotionControlCardType cardType)
        {
            this.CardType = cardType;
        }

        public MotionControlCardType CardType { get; set; } = MotionControlCardType.Normal;
    }
}
