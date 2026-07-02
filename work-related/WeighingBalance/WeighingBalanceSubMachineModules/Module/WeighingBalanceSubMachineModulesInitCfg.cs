using GF_Gereric;
using System.Collections.Concurrent;

namespace GKG.SubMM.Dispenser
{
    /// <summary>
    /// 初始化配置
    /// </summary>
    public class WeighingBalanceSubMachineModulesInitCfg
    {
        /// <summary>
        /// 称重动作参数
        /// </summary>
        public WeighingActionParameters WeighingActionParameters { get; set; }
        ///// <summary>
        ///// 功能头偏移校准结果数组
        ///// </summary>
        //public OffsetCalibrationResult[] OffsetCalibrationResults { get; set; }

        /// <summary>
        /// X轴绑定
        /// </summary>
        public Guid XAxisBindingGuid { get; set; }

        /// <summary>
        /// Y轴绑定
        /// </summary>
        public Guid YAxisBindingGuid { get; set; }

        /// <summary>
        /// Z轴绑定
        /// </summary>
        public Guid ZAxisBindingGuid { get; set; }

        /// <summary>
        /// 阀实例ID数组
        /// </summary>
        public Dictionary<string, string> ValveID { get; set; }

        /// <summary>
        /// 天平初始化参数
        /// </summary>
        public byte[] WeighingBalanceInitParams { get; set; }

        /// <summary>
        /// 复制
        /// </summary>
        /// <param name="source">复制源</param>
        public void CopyFrom(WeighingBalanceSubMachineModulesInitCfg source)
        {
            if (source == null)
                return;
            this.WeighingBalanceInitParams = source.WeighingBalanceInitParams;
            //this.WeighingParameters = source.WeighingParameters;
            this.XAxisBindingGuid = source.XAxisBindingGuid;
            this.YAxisBindingGuid = source.YAxisBindingGuid;
            this.ZAxisBindingGuid = source.ZAxisBindingGuid;
            this.ValveID = source.ValveID;
            //this.OffsetCalibrationResults = source.OffsetCalibrationResults;
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