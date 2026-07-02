using GF_Gereric;

namespace DispenserMachineModules
{
    /// <summary>
    /// 初始化配置
    /// </summary>
    public class DispenserMachineModulesInitCfg
    {
        /// <summary>
        /// XY运行速度(mm/s)
        /// </summary>
        public double XYSpeed { get; set; } = 100.0;
        /// <summary>
        /// XY加速度(mm/s²)
        /// </summary>
        public double XYAcc { get; set; } = 1000.0;
        /// <summary>
        /// XY运行速度(mm/s)
        /// </summary>
        public double ZSpeed { get; set; } = 50.0;
        /// <summary>
        /// XY加速度(mm/s²)
        /// </summary>
        public double ZAcc { get; set; } = 500.0;
        /// <summary>
        /// 安全高度(mm)
        /// </summary>
        public double SafetyHeight { get; set; } = 5.0;
        /// <summary>
        /// 复制
        /// </summary>
        /// <param name="source">复制源</param>
        public void CopyFrom(DispenserMachineModulesInitCfg source)
        {
            XYSpeed = source.XYSpeed;
            XYAcc = source.XYAcc;
            ZSpeed = source.ZSpeed;
            ZAcc = source.ZAcc;
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