using GF_Gereric;
using Griffins;
using Griffins.ImeIOT;

namespace GKG.Map.UIDataObjProcessDesignTime.ProductionInfo.Models
{
	/// <summary>
	/// 自定义界面数据对象配置信息
	/// </summary>
	public class UIDataObjProcessProductionInfoCfgInfo
	{
		/// <summary>
		/// 设备管理界面数据对象种类
		/// </summary>
        public static readonly ManagingPointKind DeviceManager_MPKind = ManagingPointKind.Parse(ImeIOTConst.DevMngMMStr);

		/// <summary>
		/// 设备管理机械模组实列别名
		/// </summary>
        public MMAlias DeviceManager_Alias { get; set; }

        public byte[] ToBytes() => JsonObjConvert.ToJSonBytes(this);

        public void FromBytes(byte[] data)
        {
            if (data != null) JsonObjConvert.PopulateObject(data, this);
        }
	}
}
