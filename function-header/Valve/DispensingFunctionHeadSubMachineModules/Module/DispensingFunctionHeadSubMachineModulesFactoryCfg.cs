using GF_Gereric;
using GKG.ElectronicControl.Dispenser;

namespace GKG.SubMM.Dispenser
{
    /// <summary>
    /// 出厂配置
    /// </summary>
    public class DispensingFunctionHeadSubMachineModulesFactoryCfg
    {
        /// <summary>
        /// 阀类型
        /// </summary>
        public ValveType ValveType {  get; set; }

        /// <summary>
        /// 供胶装置类型
        /// </summary>
        public GlueDispensingDeviceType GlueDispensingDeviceType { get; set; }

        /// <summary>
        /// 复制
        /// </summary>
        /// <param name="source">复制源</param>
        public void CopyFrom(DispensingFunctionHeadSubMachineModulesFactoryCfg source)
        {
            this.ValveType = source.ValveType;
            this.GlueDispensingDeviceType = source.GlueDispensingDeviceType;
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