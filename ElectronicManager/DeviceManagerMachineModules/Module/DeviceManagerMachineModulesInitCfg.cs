using GF_Gereric;
using GKG.SubMM;

namespace GKG
{
    namespace MM
    {
        public class DeviceManagerMachineModulesInitCfg
        {

            public void CopyFrom(DeviceManagerMachineModulesInitCfg source)
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
