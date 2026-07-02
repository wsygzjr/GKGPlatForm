using GF_Gereric;
using System;

namespace GKG
{
	namespace SubMM
	{
		public class RailCommunicateSubMachineModulesInitCfg
        {
			/// <summary>
			/// 上位机有板IO ID
			/// </summary>
			public Guid UpperMachineStateId { get; set; } = Guid.Empty;
            /// <summary>
            /// 下位机要板IO ID
            /// </summary>
            public Guid LowerMachineStateId { get; set; } = Guid.Empty;
            /// <summary>
            /// 本机有板IO ID
            /// </summary>
            public Guid MachineHasPanelId { get; set; } = Guid.Empty;
            /// <summary>
            /// 本机要板IO ID
            /// </summary>
            public Guid MachineNeedPanelId { get; set; } = Guid.Empty;
			
            public void CopyFrom(RailCommunicateSubMachineModulesInitCfg source)
			{
                if (source == null)
                    throw new ArgumentNullException(nameof(source), Resources.RailCommunicateInitCfgCopySourceNull);

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
