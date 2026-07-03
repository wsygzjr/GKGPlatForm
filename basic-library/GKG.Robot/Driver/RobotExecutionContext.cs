using GKG.ElectronicControl;

namespace GKG
{
    namespace MotionControl
    {
        /// <summary>
        /// Robot 执行上下文。
        /// 用于描述本次运控指令序列执行所依赖的运行时环境。
        /// </summary>
        public class RobotExecutionContext
        {
            public string? RobotId { get; set; }

            public string? FunctionHeadId { get; set; }

            public int CoordinateSystemId { get; set; }

            public bool IsSimulation { get; set; }

            public bool EnablePokaYoke { get; set; } = true;

            public bool AutoInsertSafeMove { get; set; }

            public Dictionary<string, Point3D> TcpOffsets { get; set; } = new Dictionary<string, Point3D>();

            public Dictionary<int, AxisBinding> AxisBindingPairs { get; set; } = new Dictionary<int, AxisBinding>();

            public IMotionControlBase? MotionControl { get; set; }

            public byte[]? ExtendedParameters { get; set; }
        }
    }
}
