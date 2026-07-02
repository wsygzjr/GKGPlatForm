using Avalonia.Media.Imaging;
using Avalonia.Platform;
using GF_Gereric;
using Griffins;
using Griffins.Map.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace Griffins.Map.PressureFuncCtlMapCell
{
    [MapCellKindCategory(PressureMapCellGroup.PressureMapCellGroupID_Str)]
    [FunctionalMapCell("{4FD6F14F-325D-4FE1-998B-FC743722E545}")]
    class MapCellPressureCtl_AMain : GriffinsPluginMngClass, IFunctionalMapCell
	{
        private static Bitmap bitmap = null;
        static MapCellPressureCtl_AMain()
        {
            var assetLoader = AssetLoader.Open;
            // 路径格式：项目根命名空间;component/相对路径
            var uri = new Uri("avares://Griffins.Map.PressureFuncCtlMapCell/Assets/Images/ctr-Pressure.png");
            using (var stream = AssetLoader.Open(uri))
            {
                bitmap = new Bitmap(stream); // 加载为Bitmap
            }
        }
		#region IMapCell 成员

		void IFunctionalMapCell.Init(string pluginFileName)
        {
        }

        int IFunctionalMapCell.DefaultWidth
        {
            get { return 180; }
        }

        int IFunctionalMapCell.DefaultHeight
        {
            get { return 120; }
        }

        Bitmap IFunctionalMapCell.Ico
        {
            get { return bitmap; }
        }

        string IFunctionalMapCell.MapCellKindName
        {
            get { return ResourceA.Pressure; }
        }

		/// <summary>
		/// 创建图控元插件设计时接口实例
		/// </summary>
		/// <param name="mapCellID">图元ID</param>
		/// <param name="mapCellName">图元名称</param>
		/// <returns>图控元插件设计时接口实例</returns>
		IFunctionalMapCellDesigntime IFunctionalMapCell.CreateMapCellDesigntime(MapObjID mapCellID, string mapCellName)
        {
            return new MapCellPressureCtlDesigntime(mapCellID, mapCellName);
        }

		/// <summary>
		/// 创建图控元插件运行时接口实例
		/// </summary>
		/// <param name="mapCellID">图元ID</param>
		/// <param name="mapCellID">图元名称</param>
		/// <returns>图控元插件运行时接口实例</returns>
		IFunctionalMapCellObject IFunctionalMapCell.CreateMapCellObj(MapObjID mapCellID, string mapCellName)
        {
            return new MapCellPressureCtlObj(mapCellID, mapCellName);
        }

        #endregion
    }

}