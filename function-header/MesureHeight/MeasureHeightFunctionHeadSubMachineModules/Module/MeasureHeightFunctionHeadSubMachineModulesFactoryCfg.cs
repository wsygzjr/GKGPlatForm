using GF_Gereric;

namespace GKG
{
    namespace SubMM
    {
        /// <summary>
        /// 出厂配置
        /// </summary>
        public class MeasureHeightFunctionHeadSubMachineModulesFactoryCfg
        {
            public MeasureHeightType MeasureHeightType { get; set; }
            public void CopyFrom(MeasureHeightFunctionHeadSubMachineModulesFactoryCfg source)
            {
                this.MeasureHeightType = source.MeasureHeightType;
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
}