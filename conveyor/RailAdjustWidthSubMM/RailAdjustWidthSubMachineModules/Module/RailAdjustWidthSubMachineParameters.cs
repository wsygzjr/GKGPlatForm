using System;

namespace GKG
{
    namespace SubMM
    {
        public enum RailAdjustWidthAxis
        {
            FrontRail = 0,
            BackRail = 1,
        }
        public enum MoveDirection
        {
            /// <summary>
            /// 正向
            /// </summary>
            Positive = 0,
            /// <summary>
            /// 负向
            /// </summary>
            Negative = 1,
        }
        public class AdjustWidthParameters
        {
            /// <summary>
            /// 边轨ID，表示需要调整宽度的边轨的唯一标识符，通常是一个整数值，用于区分不同的边轨，确保调整宽度的命令能够正确地发送到指定的边轨上进行处理
            /// </summary>
            public int SideRailID { get; set; }
            /// <summary>
            /// 方向，表示调整宽度的方向，通常是一个整数值，0表示向内调整宽度，1表示向外调整宽度，根据实际需求进行设置，确保调整宽度的命令能够正确地解释和执行，实现预期的调整效果
            /// </summary>
            public int Direction { get; set; }
            //目标宽度，单位为mm，表示调整后的目标宽度，数值越大表示宽度越宽，数值越小表示宽度越窄，根据实际需求进行设置，确保调整宽度的命令能够正确地解释和执行，实现预期的调整效果，同时需要注意目标宽度的合理范围，避免设置过大或过小导致安全隐患
            public decimal TargetWidth { get; set; }
        }

        public class TranslationParameters
        {
            /// <summary>
            /// 边轨ID
            /// </summary>
            public RailAdjustWidthAxis RailID { get; set; } = RailAdjustWidthAxis.FrontRail;
            /// <summary>
            /// 移动方向
            /// </summary>
            public MoveDirection Direction { get; set; }
            /// <summary>
            /// 加速度
            /// </summary>
            public double Acceleration { get; set; }
            /// <summary>
            /// 速度
            /// </summary>
            public double Speed { get; set; }
        }
    }
}
