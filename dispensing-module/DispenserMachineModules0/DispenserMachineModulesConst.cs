using Griffins.ImeIOT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DispenserMachineModules
{
	public class DispenserMachineModulesConst
	{
		public const string MMName = "点胶机械模组";

		public const string MMNumberStr = "Dispenser";

		public readonly static MMNumber MMNumber = MMNumber.Parse(MMNumberStr);
		/// <summary>
		/// 开始能力方法ID(异步)
		/// </summary>
		public const string StartMethodID = "Start";
		/// <summary>
		/// 开始完成能力事件ID
		/// </summary>
		public static readonly string StartFinishedEventID = ImeMethodDefInfo.GetFinishedEventID(StartMethodID);
		/// <summary>
		/// 结束能力方法ID(异步)
		/// </summary>
		public const string EndMethodID = "End";
		/// <summary>
		/// 结束完成能力事件ID
		/// </summary>
		public static readonly string EndFinishedEventID = ImeMethodDefInfo.GetFinishedEventID(EndMethodID);
		/// <summary>
		/// 移动能力方法ID(异步)
		/// </summary>
		public const string MoveMethodID = "Move";
		/// <summary>
		/// 移动完成能力事件ID
		/// </summary>
		public static readonly string MoveFinishedEventID = ImeMethodDefInfo.GetFinishedEventID(MoveMethodID);

		/// <summary>
		/// 机械模组能力事件定义信息列表
		/// 不是和能力方法匹配的其他能力事件
		/// </summary>
		public static readonly ImeCabilityEventDefInfoList MMCabilityEventes = new ImeCabilityEventDefInfoList()
		{

		};

		/// <summary>
		/// 机械模组能力方法定义信息列表
		/// 约定能力方法产生的能力事件，ID保持一致
		/// </summary>
		public static readonly ImeCabilityMethodDefInfoList MMCabilityMethodes = new ImeCabilityMethodDefInfoList()
		{
			new ImeCabilityMethodDefInfo(StartMethodID,"开始",new ImeParamObjPropInfoList(),new ImeParamObjPropInfoList()),
			new ImeCabilityMethodDefInfo(EndMethodID,"结束",new ImeParamObjPropInfoList(),new ImeParamObjPropInfoList()),
			new ImeCabilityMethodDefInfo(MoveMethodID,"移动",new ImeParamObjPropInfoList(),new ImeParamObjPropInfoList()),
		};

		/// <summary>
		/// Y移动龙门型号
		/// </summary>
		public readonly static SubMMModel YLongMenSubMMModel = SubMMModel.Parse("YLongMen");
		/// <summary>
		/// Y移动龙门内部别名
		/// </summary>
		public readonly static InnerAlias YLongMen_Alias = InnerAlias.Parse("ylongmen01");

	}
}
