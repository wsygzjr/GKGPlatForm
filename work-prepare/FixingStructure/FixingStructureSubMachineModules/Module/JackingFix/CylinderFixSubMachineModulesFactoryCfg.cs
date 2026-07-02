using GF_Gereric;
using System;

namespace GKG.SubMM
{
    /// <summary>
    /// 出厂配置
    /// </summary>
    public class CylinderFixSubMachineModulesFactoryCfg
    {

        /// <summary>
        /// 复制
        /// </summary>
        /// <param name="source">复制源</param>
        public void CopyFrom(CylinderFixSubMachineModulesFactoryCfg source)
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
