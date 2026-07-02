using GF_Gereric;
using System;

namespace GKG.SubMM
{
    /// <summary>
    /// 初始化配置
    /// </summary>
    public class CylinderFixSubMachineModulesInitCfg
    {
        /// <summary>
        /// 固定气缸初始化参数。
        /// </summary>
        public CylinderInitParameters FixingCylinderParams { get; set; } = new CylinderInitParameters();

        /// <summary>
        /// 复制
        /// </summary>
        public void CopyFrom(CylinderFixSubMachineModulesInitCfg source)
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
