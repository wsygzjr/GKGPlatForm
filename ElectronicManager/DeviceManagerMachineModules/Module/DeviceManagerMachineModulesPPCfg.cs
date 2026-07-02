using GF_Gereric;
using GKG.SubMM;

namespace GKG
{
    namespace MM
    {
        public class DeviceManagerMachineModulesPPCfg
        {
            /// <summary>
            /// 总上下料流程使用的有料信号持续检测时间。
            /// </summary>
            public double HasMaterialSignalSenseDuration { get; set; }

            /// <summary>
            /// 总上下料流程允许的连续空推次数。
            /// </summary>
            public int ContinuousEmptyPushCount { get; set; }

            public void CopyFrom(DeviceManagerMachineModulesPPCfg source)
            {
                if (source == null)
                {
                    return;
                }
                HasMaterialSignalSenseDuration = source.HasMaterialSignalSenseDuration;
                ContinuousEmptyPushCount = source.ContinuousEmptyPushCount;
            }

            public byte[] ToBytes()
            {
                return JsonObjConvert.ToJSonBytes(this);
            }

            public void FromBytes(byte[] data)
            {
                if (data != null && data.Length > 0)
                {
                    JsonObjConvert.PopulateObject(data, this);
                }
            }
        }
    }
}
