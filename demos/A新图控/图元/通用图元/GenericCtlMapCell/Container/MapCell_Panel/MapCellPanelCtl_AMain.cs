using Avalonia.Media.Imaging;
using Avalonia.Platform;
using GF_Gereric;
using Griffins;
using Griffins.Map.CtlMapCell.Generic.Container;
using Griffins.Map.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace Griffins.Map.CtlMapCell.Generic.TextButton
{
    [MapCellKindCategory(ContainerMapCellGroup.ContainerMapCellGroupID_Str)]
    [ControlMapCell("{1A5D9816-880D-42DA-A36B-9979BA50259B}")]
    class MapCellPanelCtl_AMain : GriffinsPluginMngClass, IControlMapCell
    {
        private static Bitmap bitmap = null;
        static MapCellPanelCtl_AMain()
        {
            var assetLoader = AssetLoader.Open;
            // 路径格式：项目根命名空间;component/相对路径
            var uri = new Uri("avares://Griffins.Map.CtlMapCell.Generic/Assets/Images/ctr-Panel.png");
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
            get { return 200; }
        }

        int IControlMapCell.DefaultHeight
        {
            get { return 200; }
        }

        Bitmap IControlMapCell.Ico
        {
            get { return bitmap; }
        }

        string IControlMapCell.MapCellKindName
        {
            get { return ResourceA.PanelContainer; }
        }

        /// <summary>
        /// 创建图控元插件设计时接口实例
        /// </summary>
        /// <param name="mapCellID">图元ID</param>
        /// <param name="mapCellName">图元名称</param>
        /// <returns>图控元插件设计时接口实例</returns>
        IControlMapCellDesigntime IControlMapCell.CreateMapCellDesigntime(MapObjID mapCellID, string mapCellName)
        {
            return new MapCellPanelCtlDesigntime(mapCellID, mapCellName);
        }

        /// <summary>
        /// 创建图控元插件运行时接口实例
        /// </summary>
        /// <param name="mapCellID">图元ID</param>
        /// <param name="mapCellID">图元名称</param>
        /// <returns>图控元插件运行时接口实例</returns>
        IControlMapCellObject IControlMapCell.CreateMapCellObj(MapObjID mapCellID, string mapCellName)
        {
            return new MapCellPanelCtlObj(mapCellID, mapCellName);
        }

        #endregion
    }

}