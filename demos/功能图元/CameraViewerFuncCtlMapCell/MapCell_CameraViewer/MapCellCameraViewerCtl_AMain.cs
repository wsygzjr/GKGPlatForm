using Avalonia.Media.Imaging;
using Avalonia.Platform;
using GF_Gereric;
using Griffins.Map;
using Griffins.Map.UI;
using System;

namespace GKG.Map.CameraViewerFuncCtlMapCell
{
    [MapCellKindCategory(CameraViewerMapCellGroup.CameraViewerMapCellGroupID_Str)]
    [FunctionalMapCell("{56BC4EDA-5A9F-4042-8B99-5B3D3A8515C9}")]
    public class MapCellCameraViewerCtl_AMain : GriffinsPluginMngClass, IFunctionalMapCell
    {
        private static Bitmap bitmap = null;
        static MapCellCameraViewerCtl_AMain()
        {
            var assetLoader = AssetLoader.Open;
            // 路径格式：项目根命名空间;component/相对路径
            var uri = new Uri("avares://GKG.Map.CameraViewerFuncCtlMapCell/Assets/Images/ctr-Pressure.png");
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
            get { return 1050; }
        }

        int IFunctionalMapCell.DefaultHeight
        {
            get { return 649; }
        }

        Bitmap IFunctionalMapCell.Ico
        {
            get { return bitmap; }
        }

        string IFunctionalMapCell.MapCellKindName
        {
            get { return ResourceA.CameraViewer; }
        }

        /// <summary>
        /// 创建图控元插件设计时接口实例
        /// </summary>
        /// <param name="mapCellID">图元ID</param>
        /// <param name="mapCellName">图元名称</param>
        /// <returns>图控元插件设计时接口实例</returns>
        IFunctionalMapCellDesigntime IFunctionalMapCell.CreateMapCellDesigntime(MapObjID mapCellID, string mapCellName)
        {
            return new MapCellCameraViewerCtlDesigntime(mapCellID, mapCellName);
        }

        /// <summary>
        /// 创建图控元插件运行时接口实例
        /// </summary>
        /// <param name="mapCellID">图元ID</param>
        /// <param name="mapCellID">图元名称</param>
        /// <returns>图控元插件运行时接口实例</returns>
        IFunctionalMapCellObject IFunctionalMapCell.CreateMapCellObj(MapObjID mapCellID, string mapCellName)
        {
            return new MapCellCameraViewerCtlObj(mapCellID, mapCellName);
        }

        #endregion
    }

}
