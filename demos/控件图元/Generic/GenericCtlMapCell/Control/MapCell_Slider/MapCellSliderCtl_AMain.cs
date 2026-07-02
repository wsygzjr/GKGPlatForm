using Avalonia.Media.Imaging;
using Avalonia.Platform;
using GF_Gereric;
using Griffins;
using GKG.Map.MapCell.Generic.Control.MapCell_Slider;
using System;

namespace GKG.Map.MapCell.Generic.Control.MapCell_Slider
{
    [MapCellKindCategory(GenericMapCellGroup.GenericMapCellGroupID_Str)]
    [ControlMapCell("{A4854EFC-22EC-48D1-AA71-556F0C24F715}")]
    class MapCellSliderCtl_AMain : GriffinsPluginMngClass, IControlMapCell
    {
        private static Bitmap bitmap = null;

        static MapCellSliderCtl_AMain()
        {
            try
            {
                var assetLoader = AssetLoader.Open;
                // 路径格式：项目根命名空间;component/相对路径
                var uri = new Uri("avares://GKG.Map.MapCell.Generic/Assets/Images/ctr-Slider.png");
                using (var stream = AssetLoader.Open(uri))
                {
                    bitmap = new Bitmap(stream); // 加载为Bitmap
                }
            }
            catch (Exception ex)
            {
            }
        }
        #region IMapCell 成员

        void IControlMapCell.Init(string pluginFileName)
        {
        }

        int IControlMapCell.DefaultWidth
        {
            get { return 150; }
        }

        int IControlMapCell.DefaultHeight
        {
            get { return 50; }
        }

        Bitmap IControlMapCell.Ico
        {
            get { return bitmap; }
        }

        string IControlMapCell.MapCellKindName
        {
            get { return ResourceA.Slider; }
        }

        /// <summary>
        /// 创建图控元插件设计时接口实例
        /// </summary>
        /// <param name="mapCellID">图元ID</param>
        /// <param name="mapCellName">图元名称</param>
        /// <returns>图控元插件设计时接口实例</returns>
        IControlMapCellDesigntime IControlMapCell.CreateMapCellDesigntime(MapObjID mapCellID, string mapCellName)
        {
            return new MapCellSliderCtlDesigntime(mapCellID, mapCellName);
        }

        /// <summary>
        /// 创建图控元插件运行时接口实例
        /// </summary>
        /// <param name="mapCellID">图元ID</param>
        /// <param name="mapCellName">图元名称</param>
        /// <returns>图控元插件运行时接口实例</returns>
        IControlMapCellObject IControlMapCell.CreateMapCellObj(MapObjID mapCellID, string mapCellName)
        {
            return new MapCellSliderCtlObj(mapCellID, mapCellName);
        }

        #endregion
    }
}
