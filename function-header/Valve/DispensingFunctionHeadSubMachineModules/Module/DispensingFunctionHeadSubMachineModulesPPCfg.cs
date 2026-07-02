using GF_Gereric;

namespace GKG.SubMM.Dispenser
{
    /// <summary>
    /// 配方配置
    /// </summary>
    public class DispensingFunctionHeadSubMachineModulesPPCfg
    {
        public byte[] glueAmountParams { get; set; } = new byte[0];
        public byte[] GKGPiezoValveFormulaParams { get; set; } = new byte[0];

        /// <summary>
        /// 复制
        /// </summary>
        /// <param name="source">复制源</param>
        public void CopyFrom(DispensingFunctionHeadSubMachineModulesPPCfg source)
        {
            this.glueAmountParams = source.glueAmountParams;
            this.GKGPiezoValveFormulaParams = source.GKGPiezoValveFormulaParams;
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