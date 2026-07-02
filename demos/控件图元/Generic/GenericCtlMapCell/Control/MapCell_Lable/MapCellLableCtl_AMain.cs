using Avalonia.Media.Imaging;
using Avalonia.Platform;
using GF_Gereric;
using Griffins;
using System;
using System.Collections.Generic;
using System.Text;

namespace GKG.Map.MapCell.Generic.Lable
{
    [MapCellKindCategory(GenericMapCellGroup.GenericMapCellGroupID_Str)]
    [ControlMapCell("{8DEF6881-867E-44DF-9BF1-59005FA2B871}")]
    class MapCellLableCtl_AMain : GriffinsPluginMngClass, IControlMapCell
    {
        private static Bitmap bitmap = null;
        static MapCellLableCtl_AMain()
        {
            var assetLoader = AssetLoader.Open;
            // 路径格式：项目根命名空间;component/相对路径
            var uri = new Uri("avares://GKG.Map.MapCell.Generic/Assets/Images/ctr-Lable.png");
            using (var stream = AssetLoader.Open(uri))
            {
                bitmap = new Bitmap(stream); // 加载为Bitmap
            }
        }
		#region IMapCell 成员

		void IControlMapCell.Init(string pluginFileName)
        {
        }

        int IControlMapCell.DefaultWidth
        {
            get { return 100; }
        }

        int IControlMapCell.DefaultHeight
        {
            get { return 40; }
        }

        Bitmap IControlMapCell.Ico
        {
            get { return bitmap; }
        }

        string IControlMapCell.MapCellKindName
        {
            get { return ResourceA.Lable; }
        }

        /// <summary>
        /// 创建图控元插件设计时接口实例
        /// </summary>
        /// <param name="mapCellID">图元ID</param>
        /// <param name="mapCellName">图元名称</param>
        /// <returns>图控元插件设计时接口实例</returns>
        IControlMapCellDesigntime IControlMapCell.CreateMapCellDesigntime(MapObjID mapCellID, string mapCellName)
        {
            return new MapCellLableCtlDesigntime(mapCellID, mapCellName);
        }

        /// <summary>
        /// 创建图控元插件运行时接口实例
        /// </summary>
        /// <param name="mapCellID">图元ID</param>
        /// <param name="mapCellID">图元名称</param>
        /// <returns>图控元插件运行时接口实例</returns>
        IControlMapCellObject IControlMapCell.CreateMapCellObj(MapObjID mapCellID, string mapCellName)
        {
            return new MapCellLableCtlObj(mapCellID, mapCellName);
        }

        #endregion
    }

}