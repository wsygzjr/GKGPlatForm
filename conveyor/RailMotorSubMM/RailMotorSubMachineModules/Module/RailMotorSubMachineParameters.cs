using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GKG
{
    namespace SubMM
    {
        public enum ERailMotionMode
        {
            /// <summary>
            /// 独占
            /// </summary>
            ExclusiveMode,
            /// <summary>
            /// 共享
            /// </summary>
            ShareMode,
        }

        public class ContinueMoveParameters
        {
            /// <summary>
            /// 加速度，单位为毫米每秒^2
            /// </summary>
            public double Acceleration { get; set; }
            /// <summary>
            /// 连续运动的速度，单位为毫米每秒
            /// </summary>
            public double Speed { get; set; }
            /// <summary>
            /// 连续运动的方向，true表示正向，false表示反向
            /// </summary>
            public bool Direction { get; set; }
        }

        public class RelativeMoveParameters
        {
            /// <summary>
            /// 相对运动的距离，单位为毫米。正值表示沿正向移动，负值表示沿反向移动。
            /// </summary>
            public double RelativeDistance { get; set; }
            /// <summary>
            /// 相对运动的加速度，单位为毫米每秒^2
            /// </summary>
            public double Acceleration { get; set; }
            /// <summary>
            /// 相对运动的速度，单位为毫米每秒
            /// </summary>
            public double Speed { get; set; }
            /// <summary>
            /// 相对运动的方向，true表示正向，false表示反向
            /// </summary>
            public bool Direction { get; set; }
        }
    }
}
