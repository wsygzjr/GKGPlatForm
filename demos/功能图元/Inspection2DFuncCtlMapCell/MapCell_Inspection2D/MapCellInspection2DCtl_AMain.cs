using Avalonia.Media.Imaging;
using Avalonia.Platform;
using GF_Gereric;
using Griffins.Map;
using Griffins.Map.UI;
using System;

namespace GKG.Map.Inspection2DFuncCtlMapCell
{
    [MapCellKindCategory(Inspection2DMapCellGroup.Inspection2DMapCellGroupID_Str)]
    [FunctionalMapCell("{B5E3F2A5-2C3F-5F3C-97C6-3B857E8BA0F7}")]
    class MapCellInspection2DCtl_AMain : GriffinsPluginMngClass, IFunctionalMapCell
    {
        private static Bitmap bitmap = null!;

        static MapCellInspection2DCtl_AMain()
        {
            var uri = new Uri("avares://GKG.Map.Inspection2DFuncCtlMapCell/Assets/Images/Inspection2D.png");

            using (var stream = AssetLoader.Open(uri))
            {
                bitmap = new Bitmap(stream);
            }
        }

        #region IFunctionalMapCell 成员
        
        void IFunctionalMapCell.Init(string pluginFileName) { }

        int IFunctionalMapCell.DefaultWidth => 760;

        int IFunctionalMapCell.DefaultHeight => 410;

        Bitmap IFunctionalMapCell.Ico => bitmap;

        string IFunctionalMapCell.MapCellKindName => ResourceA.Inspection2D;

        IFunctionalMapCellDesigntime IFunctionalMapCell.CreateMapCellDesigntime(MapObjID mapCellID, string mapCellName)
            => new MapCellInspection2DCtlDesigntime(mapCellID, mapCellName);

        IFunctionalMapCellObject IFunctionalMapCell.CreateMapCellObj(MapObjID mapCellID, string mapCellName)
            => new MapCellInspection2DCtlObj(mapCellID, mapCellName);

        #endregion
    }
}
