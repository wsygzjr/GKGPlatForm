using GF_Gereric;
using System;

namespace GKG
{
	namespace SubMM
	{
		public enum RequestPanelMode
		{
			/// <summary>
			/// 只要
			/// </summary>
			RequestOnly = 0,
			/// <summary>
			/// 先要后有
			/// </summary>
			RequestBeforeHave = 1,
			/// <summary>
			/// 先有后要
			/// </summary>
			HaveBeforeRequest = 2,
		}
		public class RailCommunicateSubMachineModulesPPCfg
		{
			/// <summary>
			/// 要板模式
			/// </summary>
			public RequestPanelMode RequestPanelMode { get; set; } = RequestPanelMode.RequestOnly;
            public void CopyFrom(RailCommunicateSubMachineModulesPPCfg source)
			{
                if (source == null)
                    throw new ArgumentNullException(nameof(source), Resources.RailCommunicatePPCfgCopySourceNull);

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
