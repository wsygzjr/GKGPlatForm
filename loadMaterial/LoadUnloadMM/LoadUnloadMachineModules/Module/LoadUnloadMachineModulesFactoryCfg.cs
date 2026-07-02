using GF_Gereric;

namespace GKG
{
    namespace MM
    {
        public class LoadUnloadMachineModulesFactoryCfg
        {
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
