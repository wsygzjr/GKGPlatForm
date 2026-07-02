using GF_Gereric;

namespace GKG.SubMM
{
    /// <summary>
    /// 初始化配置
    /// </summary>
    public class MotionCalculateSubMachineModulesInitCfg
    {
        /// <summary>
        /// 标定参数，相机与功能头的偏移
        /// </summary>
        public OffsetCalibrationResult[] offsetCalibrationResults;

        /// <summary>
        /// 多阀主轴定义
        /// </summary>
        public MultiValveSpindle multiValveSpindle;

        /// <summary>
        /// 副阀速度
        /// </summary>
        public double ViceSpeed;

        /// <summary>
        /// 非加工速度
        /// </summary>
        public double NonProcessingSpeed;

        /// <summary>
        /// 加速度
        /// </summary>
        public double Acceleration;

        /// <summary>
        /// 加工轨迹加速度
        /// </summary>
        public double ProcessingAcceleration;

        /// <summary>
        /// 通道列表
        /// </summary>
        public int[] Channels;

        /// <summary>
        /// 复制
        /// </summary>
        /// <param name="source">复制源</param>
        public void CopyFrom(MotionCalculateSubMachineModulesInitCfg source)
        {
        }

        /// <summary>
        /// 转为字节数组
        /// </summary>
        /// <returns>字节数组</returns>
        public byte[] ToBytes()
        {
            return JsonObjConvert.ToJSonBytes(this);
        }

        /// <summary>
        /// 从字节数组转换
        /// </summary>
        /// <param name="data">字节数组</param>
        public void FromBytes(byte[] data)
        {
            if (data != null && data.Length > 0)
                JsonObjConvert.PopulateObject(data, this);
        }
    }
}