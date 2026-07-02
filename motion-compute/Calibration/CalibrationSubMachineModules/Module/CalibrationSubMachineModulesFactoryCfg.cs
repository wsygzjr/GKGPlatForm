using GF_Gereric;
using System.Collections.Concurrent;

namespace GKG.SubMM
{
    /// <summary>
    /// 出厂配置
    /// </summary>
    public class CalibrationSubMachineModulesFactoryCfg
    {
        public ConcurrentBag<Calibration> calibrations = new ConcurrentBag<Calibration>();
        public ConcurrentBag<CalibrationInitParams> calibrationInitParams = new ConcurrentBag<CalibrationInitParams>();

        /// <param name="source">复制源</param>
        public void CopyFrom(CalibrationSubMachineModulesFactoryCfg source)
        {
            if (source != null)
            {
                this.calibrations = new ConcurrentBag<Calibration>(source.calibrations);
                this.calibrationInitParams = new ConcurrentBag<CalibrationInitParams>(source.calibrationInitParams);
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