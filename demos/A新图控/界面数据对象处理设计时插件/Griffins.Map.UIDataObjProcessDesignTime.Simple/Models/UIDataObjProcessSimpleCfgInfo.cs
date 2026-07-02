using GF_Gereric;
using Griffins.ImeIOT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Griffins.Map.UIDataObjProcessDesignTime.Simple.Models
{
	/// <summary>
	/// 自定义界面数据对象配置信息
	/// </summary>
	internal class UIDataObjProcessSimpleCfgInfo
	{
		/// <summary>
		/// 印刷机械模组界面数据对象种类
		/// </summary>
		public static readonly ManagingPointKind YS_MPKind = ManagingPointKind.Parse("test_ys");
		/// <summary>
		/// 备料机械模组界面数据对象种类
		/// </summary>
		public static readonly ManagingPointKind BL_MPKind = ManagingPointKind.Parse("test_bl");
		/// <summary>
		/// 印刷机械模组实例别名
		/// </summary>
		public MMAlias YS_Alias { get; set; }
		/// <summary>
		/// 备料机械模组实例别名
		/// </summary>
		public MMAlias BL_Alias { get; set; }

		/// <summary>
		/// 转为字节数组
		/// </summary>
		/// <returns>字节数组</returns>
		public byte[] ToBytes()
		{
			return JsonObjConvert.ToJSonBytes(this);
		}

		/// <summary>
		/// 从字节数组转为对象
		/// </summary>
		/// <param name="data"></param>
		public void FromBytes(byte[] data)
		{
			if (data == null)
			{
				return;
			}
			JsonObjConvert.PopulateObject(data, this);
		}
	}
}
