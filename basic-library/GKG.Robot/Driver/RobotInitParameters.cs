using GF_Gereric;

namespace GKG
{
    namespace MotionControl
    {
        /// <summary>
        /// 机器人驱动初始化参数。
        /// </summary>
        public class RobotInitParameters
        {
            public string DriverName { get; set; } = RobotDriverNames.BasicRobot;

            public MotionControlCardType MotionControlCardType { get; set; } = MotionControlCardType.Normal;

            /// <summary>
            /// 轴绑定列表（逻辑轴 -> 物理轴 + 运控卡GUID）。
            /// 放在核心参数层，避免上层依赖具体 Robot 插件参数模型。
            /// </summary>
            public AxisBinding[] AxisBindings { get; set; } = Array.Empty<AxisBinding>();

            public byte[]? DriverInitParameters { get; set; }

            public static RobotInitParameters? TryFromBytes(byte[]? data)
            {
                if (data == null || data.Length == 0)
                    return null;

                try
                {
                    var parsed = JsonObjConvert.FromJSonBytes<RobotInitParameters>(data);
                    if (parsed == null)
                        return null;

                    if (string.IsNullOrWhiteSpace(parsed.DriverName))
                    {
                        parsed.DriverName = RobotDriverNames.BasicRobot;
                    }

                    return parsed;
                }
                catch
                {
                    return null;
                }
            }
        }
    }
}
