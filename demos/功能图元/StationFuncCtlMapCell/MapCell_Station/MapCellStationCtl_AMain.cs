using Avalonia.Media.Imaging;
using Avalonia.Platform;
using GF_Gereric;
using Griffins;
using Griffins.Map;
using Griffins.Map.UI;
using System;

namespace GKG.Map.StationFuncCtlMapCell
{
    /// <summary>
    /// 工位功能图元主入口类
    /// 提供图元基本信息和实例创建入口。
    /// </summary>
    [MapCellKindCategory(StationMapCellGroup.StationMapCellGroupID_Str)]
    [FunctionalMapCell("{6C1A9343-0B1B-4A37-9E6A-3E8C23B7D0F1}")]
    class MapCellStationCtl_AMain : GriffinsPluginMngClass, IFunctionalMapCell
    {
        private static Bitmap bitmap = null!;

        /// <summary>
        /// 静态构造函数加载图元显示图标
        /// </summary>
        static MapCellStationCtl_AMain()
        {
            try
            {
                using var stream = AssetLoader.Open(new Uri("avares://GKG.Map.StationFuncCtlMapCell/Assets/Images/ctr-Station.png"));
                bitmap = new Bitmap(stream);
            }
            catch
            {
                // 防止因资源丢失导致插件无法加载 
            }
        }

        #region IFunctionalMapCell 成员

        void IFunctionalMapCell.Init(string pluginFileName) { }

        /// <summary>
        /// 图元拖拽到画布上的默认宽度
        /// </summary>
        int IFunctionalMapCell.DefaultWidth => 240;

        /// <summary>
        /// 图元拖拽到画布上的默认高度
        /// </summary>
        int IFunctionalMapCell.DefaultHeight => 80;

        /// <summary>
        /// 图元在工具箱中显示的图标
        /// </summary>
        Bitmap IFunctionalMapCell.Ico => bitmap;

        /// <summary>
        /// 图元种类名称
        /// </summary>
        string IFunctionalMapCell.MapCellKindName => ResourceA.Station;

        IFunctionalMapCellDesigntime IFunctionalMapCell.CreateMapCellDesigntime(MapObjID mapCellID, string mapCellName)
            => new MapCellStationCtlDesigntime(mapCellID, mapCellName);

        IFunctionalMapCellObject IFunctionalMapCell.CreateMapCellObj(MapObjID mapCellID, string mapCellName)
            => new MapCellStationCtlObj(mapCellID, mapCellName);

        #endregion
    }
}