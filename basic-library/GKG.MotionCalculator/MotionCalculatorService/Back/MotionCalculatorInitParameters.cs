using GF_Gereric;

namespace GKG
{
    namespace MotionControl
    {
        /// <summary>
        /// 运动计算服务初始化参数。
        /// 可选地携带具体驱动初始化参数。
        /// </summary>
        public class MotionCalculatorInitParameters
        {
            /// <summary>
            /// 驱动名称。
            /// </summary>
            public string DriverName { get; set; } = MotionCalculatorDriverNames.XYZ_xyz;

            /// <summary>
            /// 传递给驱动的初始化参数（可选）。
            /// 若为空则驱动可直接使用原始 initParam。
            /// </summary>
            public byte[]? DriverInitParameters { get; set; }

            public static MotionCalculatorInitParameters? TryFromBytes(byte[]? data)
            {
                if (data == null || data.Length == 0)
                    return null;

                try
                {
                    var parsed = JsonObjConvert.FromJSonBytes<MotionCalculatorInitParameters>(data);
                    if (parsed == null)
                        return null;

                    if (string.IsNullOrWhiteSpace(parsed.DriverName))
                    {
                        parsed.DriverName = MotionCalculatorDriverNames.XYZ_xyz;
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
