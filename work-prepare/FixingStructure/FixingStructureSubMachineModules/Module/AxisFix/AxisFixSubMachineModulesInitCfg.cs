using GF_Gereric;
using System;

namespace GKG.SubMM
{
    /// <summary>
    /// 初始化配置
    /// </summary>
    public class AxisFixSubMachineModulesInitCfg
    {
        public Guid AxisBindingObjID { get; set; } = Guid.Empty;

        /// <summary>
        /// 复制
        /// </summary>
        public void CopyFrom(AxisFixSubMachineModulesInitCfg source)
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
