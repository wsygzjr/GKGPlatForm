using System;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using GF_Gereric;
using Griffins.Map;
using Griffins.Map.UI;

namespace GKG.Map.AuxiliaryFunctionsFuncCtlMapCell
{
    [MapCellKindCategory(AuxiliaryFunctionsMapCellGroup.AuxiliaryFunctionsMapCellGroupID_Str)]
    [FunctionalMapCell("{20281FE3-4D87-44D6-8D79-186DA3DDA694}")]
    class MapCellAuxiliaryFunctionsCtl_AMain : GriffinsPluginMngClass, IFunctionalMapCell
    {
        private static Bitmap bitmap = null;
        static MapCellAuxiliaryFunctionsCtl_AMain()
        {
            var assetLoader = AssetLoader.Open;
            // 路径格式：项目根命名空间;component/相对路径
            var uri = new Uri("avares://GKG.Map.AuxiliaryFunctionsFuncCtlMapCell/Assets/Images/ctr-Pressure.png");
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
            get { return ResourceA.AuxiliaryFunctions; }
        }

        /// <summary>
        /// 创建图控元插件设计时接口实例
        /// </summary>
        /// <param name="mapCellID">图元ID</param>
        /// <param name="mapCellName">图元名称</param>
        /// <returns>图控元插件设计时接口实例</returns>
        IFunctionalMapCellDesigntime IFunctionalMapCell.CreateMapCellDesigntime(MapObjID mapCellID, string mapCellName)
        {
            return new MapCellAuxiliaryFunctionsCtlDesigntime(mapCellID, mapCellName);
        }

        /// <summary>
        /// 创建图控元插件运行时接口实例
        /// </summary>
        /// <param name="mapCellID">图元ID</param>
        /// <param name="mapCellID">图元名称</param>
        /// <returns>图控元插件运行时接口实例</returns>
        IFunctionalMapCellObject IFunctionalMapCell.CreateMapCellObj(MapObjID mapCellID, string mapCellName)
        {
            return new MapCellAuxiliaryFunctionsCtlObj(mapCellID, mapCellName);
        }

        #endregion
    }

}