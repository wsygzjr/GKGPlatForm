using GF_Gereric;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GKG
{
	namespace SubMM
	{
		/// <summary>
		/// 初始化配置
		/// </summary>
		public class VisionSubMachineModulesInitCfg
		{
            /// <summary>
            /// 绑定X轴ID，默认为"0"，如果不需要绑定X轴，可以设置为null或空字符串
            /// </summary>
            public Guid BindingAxisX { get; set; } = Guid.Empty;
            /// <summary>
            /// 绑定Y轴ID，默认为"1"，如果不需要绑定Y轴，可以设置为null或空字符串
            /// </summary>
            public Guid BindingAxisY { get; set; } = Guid.Empty;
            /// <summary>
            /// 绑定Z轴ID，默认为"2"，如果不需要绑定Z轴，可以设置为null或空字符串
            /// </summary>
            public Guid BindingAxisZ { get; set; } = Guid.Empty;
            /// <summary>
            /// 切换CCD/喷射控制IO的GUID
            /// </summary>
            public Guid ChangeCCDOrJetIOGuid { get; set; } = Guid.Empty;
            /// <summary>
            /// 触发CCD控制IO的GUID
            /// </summary>
            public Guid TriggerCCDIOGuid { get; set; } = Guid.Empty;
            /// <summary>
            /// 鼠标点击移动速度，单位mm/s
            /// </summary>
            public double MachineMoveSpeed { get; set; } = 10;
            /// <summary>
            /// 鼠标点击移动加速度，单位mm/s^2
            /// </summary>
            public double MachineMoveAcceleration { get; set; } = 100;

			/// <summary>
			/// 复制
			/// </summary>
			/// <param name="source">复制源</param>
			public void CopyFrom(VisionSubMachineModulesInitCfg source)
			{
				if (source == null)
					return;

				BindingAxisX = source.BindingAxisX;
				BindingAxisY = source.BindingAxisY;
				BindingAxisZ = source.BindingAxisZ;
				ChangeCCDOrJetIOGuid = source.ChangeCCDOrJetIOGuid;
				TriggerCCDIOGuid = source.TriggerCCDIOGuid;
				MachineMoveSpeed = source.MachineMoveSpeed;
				MachineMoveAcceleration = source.MachineMoveAcceleration;
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