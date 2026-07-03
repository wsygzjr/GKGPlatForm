using GKG.ElectronicControl;

namespace GKG
{
    namespace MotionControl
    {
        /// <summary>
        /// Robot 规划上下文。
        /// 当前先承载设备、功能头、坐标系与扩展参数，后续可继续扩展为机构学/工装/TCP 上下文。
        /// </summary>
        public class RobotPlanContext
        {
            /// <summary>
            /// 机器人实例ID。
            /// </summary>
            public string? RobotId { get; set; }

            /// <summary>
            /// 功能头ID。
            /// </summary>
            public string? FunctionHeadId { get; set; }

            /// <summary>
            /// 坐标系编号。
            /// </summary>
            public int CoordinateSystemId { get; set; }

            /// <summary>
            /// 轴绑定信息。Key=逻辑轴。
            /// </summary>
            public Dictionary<int, AxisBinding> AxisBindingPairs { get; set; } = new Dictionary<int, AxisBinding>();

            /// <summary>
            /// 底层运控卡实例（可选）。
            /// </summary>
            public IMotionControlBase? MotionControl { get; set; }

            /// <summary>
            /// 扩展二进制参数。
            /// </summary>
            public byte[]? ExtendedParameters { get; set; }

            /// <summary>
            /// 预留：TCP/偏移参数。
            /// </summary>
            public Dictionary<string, Point3D> TcpOffsets { get; set; } = new Dictionary<string, Point3D>();
        }
    }
}
