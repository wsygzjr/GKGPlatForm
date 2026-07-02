
using GF_Gereric;
using Griffins;
using Griffins.Map;

namespace GKG.Map.UIDataObj.Communication
{
	/// <summary>
	/// 通信功能图元对应的界面数据对象
	/// </summary>
	public class UIDataObjCommunication : GFPropObjBase
	{
		/// <summary>
		/// 抬起时间（ms）
		/// </summary>
		[GFProp(GfPropReadWrite.ReadWrite)]
		public decimal RaiseTime { get; set; }

		/// <summary>
		/// 点胶时间（ms）
		/// </summary>
		[GFProp(GfPropReadWrite.ReadWrite)]
		public decimal DispenseTime { get; set; }

		/// <summary>
		/// 撞击时间（ms）
		/// </summary>
		[GFProp(GfPropReadWrite.ReadWrite)]
		public decimal ImpactTime { get; set; }

		/// <summary>
		/// 退胶时间（ms）
		/// </summary>
		[GFProp(GfPropReadWrite.ReadWrite)]
		public decimal RetractTime { get; set; }

		/// <summary>
		/// 电压比（%）
		/// </summary>
		[GFProp(GfPropReadWrite.ReadWrite)]
		public decimal VoltageRatio { get; set; }

		/// <summary>
		/// 画点次数
		/// </summary>
		[GFProp(GfPropReadWrite.ReadWrite)]
		public int DotCount { get; set; }

		/// <summary>
		/// 点胶模式
		/// </summary>
		[GFProp(GfPropReadWrite.ReadWrite)]
		public int DispenseMode { get; set; }

		/// <summary>
		/// 后停状态
		/// </summary>
		[GFProp(GfPropReadWrite.ReadWrite)]
		public bool AfterStop { get; set; }

		/// <summary>
		/// 总次数
		/// </summary>
		[GFProp(GfPropReadWrite.ReadWrite)]
		public int TotalCount { get; set; }
	}
}
