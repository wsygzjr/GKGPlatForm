using GF_Gereric;
using GKG.SubMM;

namespace GKG
{
    namespace MM
    {
        public class LoadUnloadMachineModulesInitCfg
        {
            /// <summary>
            /// 连续空推次数阈值：当连续空推次数达到该值时，认为可能发生了推料堵塞等异常情况。
            /// </summary>
            public int ContinuousEmptyPushThreshold { get; set; } = 3;
            public void CopyFrom(LoadUnloadMachineModulesInitCfg source)
            {
                if (source == null)
                {
                    return;
                }
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
