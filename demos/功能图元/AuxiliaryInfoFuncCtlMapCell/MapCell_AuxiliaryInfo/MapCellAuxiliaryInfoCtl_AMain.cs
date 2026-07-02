using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using GF_Gereric;
using Griffins.Map;
using Griffins.Map.UI;
using System;

namespace GKG.Map.AuxiliaryInfoFuncCtlMapCell
{
    [MapCellKindCategory(AuxiliaryInfoMapCellGroup.AuxiliaryInfoMapCellGroupID_Str)]
    [FunctionalMapCell("{B7CFD19C-7B3D-4F1B-B4F5-0F1D2F450F07}")]
    class MapCellAuxiliaryInfoCtl_AMain : GriffinsPluginMngClass, IFunctionalMapCell
    {
        private static Bitmap bitmap = null;

        static MapCellAuxiliaryInfoCtl_AMain()
        {
            var assetLoader = AssetLoader.Open;
            // 路径格式：项目根命名空间;component/相对路径
            var uri = new Uri("avares://GKG.Map.AuxiliaryInfoFuncCtlMapCell/Assets/Images/AuxiliaryInfo.png");
            using (var stream = AssetLoader.Open(uri))
            {
                bitmap = new Bitmap(stream); // 加载为Bitmap
            }
        }

        void IFunctionalMapCell.Init(string pluginFileName)
        {
        }

        int IFunctionalMapCell.DefaultWidth
        {
            get { return 710; }
        }

        int IFunctionalMapCell.DefaultHeight
        {
            get { return 460; }
        }

        Bitmap IFunctionalMapCell.Ico
        {
            get { return bitmap; }
        }

        string IFunctionalMapCell.MapCellKindName
        {
            get { return ResourceA.AuxiliaryInfo; }
        }

        IFunctionalMapCellDesigntime IFunctionalMapCell.CreateMapCellDesigntime(MapObjID mapCellID, string mapCellName)
        {
            return new MapCellAuxiliaryInfoCtlDesigntime(mapCellID, mapCellName);
        }

        IFunctionalMapCellObject IFunctionalMapCell.CreateMapCellObj(MapObjID mapCellID, string mapCellName)
        {
            return new MapCellAuxiliaryInfoCtlObj(mapCellID, mapCellName);
        }
    }
}
