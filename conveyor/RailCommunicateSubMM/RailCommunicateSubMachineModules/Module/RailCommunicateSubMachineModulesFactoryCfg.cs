using GF_Gereric;
using System;

namespace GKG
{
	namespace SubMM
	{
		public class RailCommunicateSubMachineModulesFactoryCfg
		{
			public void CopyFrom(RailCommunicateSubMachineModulesFactoryCfg source)
			{
                if (source == null)
                    throw new ArgumentNullException(nameof(source), Resources.RailCommunicateFactoryCfgCopySourceNull);

                JsonObjConvert.PopulateObject(source.ToBytes(), this);
			}
			public byte[] ToBytes()
			{
				return JsonObjConvert.ToJSonBytes(this);
			}
			public void FromBytes(byte[] data)
			{
				if (data != null && data.Length > 0)
					JsonObjConvert.PopulateObject(data, this);
			}
		}
	}
}
