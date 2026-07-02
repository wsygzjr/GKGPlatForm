using GF_Gereric;
using RailAdjustWidthSubMachineModules.Properties;
using System;

namespace GKG
{
	namespace SubMM
	{
		public class RailAdjustWidthSubMachineModulesInitCfg
		{
            /// <summary>
            /// 是否启用碰撞检测，启用后会在调整宽度过程中检测轨道与周围环境的碰撞情况，若发生碰撞则停止调整并发出警告信号
            /// </summary>
            public bool IsEnableCrashDetection { get; set; }
            /// <summary>
            /// 调宽速度，单位为mm/s，表示轨道调整宽度的速度，数值越大调整越快，但可能会增加碰撞风险，建议根据实际情况进行设置
            /// </summary>
            public int AdjustWidthSpeed { get; set; } = 20;
            /// <summary>
            /// 加速度
            /// </summary>
            public int AdjustWidthAcceleration { get; set; } = 200;
            /// <summary>
            /// 是否启用最大距离调整宽度功能，启用后会根据前后轨道的最大距离限制调整宽度，确保调整后的宽度不会超过设定的最大距离，避免轨道过宽导致安全隐患
            /// </summary>
            public bool IsEnableMaxDistanceAdjustWidth { get; set; }
            /// <summary>
            /// 前轨轴向输入绑定对象ID，若前轨可移动则需要绑定一个轴向输入对象，用于接收调整宽度的命令和参数，若前轨不可移动则该属性无效
            /// </summary>
            public Guid FrontRailAxisBindingObjID { get; set; } = Guid.Empty;
            /// <summary>
            /// 后轨轴向输入绑定对象ID，若后轨可移动则需要绑定一个轴向输入对象，用于接收调整宽度的命令和参数，若后轨不可移动则该属性无效
            /// </summary>
            public Guid BackRailAxisBindingObjID { get; set; } = Guid.Empty;

            public void CopyFrom(RailAdjustWidthSubMachineModulesInitCfg source)
			{
                if (source == null)
                    throw new ArgumentNullException(nameof(source), Resources.RailAdjustWidthInitCfgCopySourceNull);

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
