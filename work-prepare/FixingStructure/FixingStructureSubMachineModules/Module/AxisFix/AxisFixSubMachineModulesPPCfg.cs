using GF_Gereric;
using System;

namespace GKG.SubMM
{
    /// <summary>
    /// 配方配置
    /// </summary>
    public class AxisFixSubMachineModulesPPCfg
    {
        /// <summary>
        /// 移动参数
        /// </summary>
        public NonProcessingTrajectoryParameters trajectoryParameters { get; set; } = new NonProcessingTrajectoryParameters();

        /// <summary>
        /// 固定位置
        /// </summary>
        public double FixingPosition { get; set; } = 0;
        /// <summary>
        /// 松开位置
        /// </summary>
        public double ReleaseFixingPosition { get; set; } = 0;

        /// <summary>
        /// 复制
        /// </summary>
        public void CopyFrom(AxisFixSubMachineModulesPPCfg source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            JsonObjConvert.PopulateObject(source.ToBytes(), this);
        }

        /// <summary>
        /// 转为字节数组
        /// </summary>
        public byte[] ToBytes()
        {
            return JsonObjConvert.ToJSonBytes(this);
        }

        /// <summary>
        /// 从字节数组转换
        /// </summary>
        public void FromBytes(byte[] data)
        {
            if (data != null && data.Length > 0)
                JsonObjConvert.PopulateObject(data, this);
        }
    }
}
