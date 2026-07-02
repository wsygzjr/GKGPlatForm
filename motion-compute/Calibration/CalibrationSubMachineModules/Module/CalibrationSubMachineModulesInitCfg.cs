using GF_Gereric;
using System.Collections.Concurrent;

namespace GKG.SubMM
{
    /// <summary>
    /// 初始化配置
    /// </summary>
    public class CalibrationSubMachineModulesInitCfg
    {
        /// <summary>
        /// 标定结果
        /// </summary>
        public ConcurrentBag<CalibrationResults> calibrationResults = new ConcurrentBag<CalibrationResults>();

        /// <summary>
        /// 阀功能头实例别名
        /// </summary>
        public string ValveID { get; set; } = "";

        /// <summary>
        /// 复制
        /// </summary>
        /// <param name="source">复制源</param>

        /// <summary>
        /// XY运动参数
        /// </summary>
        public NonProcessingTrajectoryParameters XYMoveParameters { get; set; } = new NonProcessingTrajectoryParameters();
        /// <summary>
        /// Z运动参数
        /// </summary>
        public NonProcessingTrajectoryParameters ZMoveParameters { get; set; } = new NonProcessingTrajectoryParameters();

        /// <summary>
        /// 安全高度(mm)
        /// </summary>
        public double SafetyHeight { get; set; } = 5.0;

        public void CopyFrom(CalibrationSubMachineModulesInitCfg source)
        {
            if (source != null)
            {
                this.calibrationResults = new ConcurrentBag<CalibrationResults>(source.calibrationResults);
                this.ValveID = source.ValveID;
                this.XYMoveParameters = source.XYMoveParameters;
                this.ZMoveParameters = source.ZMoveParameters;
                this.SafetyHeight = source.SafetyHeight;
            }
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