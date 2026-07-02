using System;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using GF_Gereric;
using Griffins.Map;
using Griffins.Map.UI;

namespace GKG.Map.DispensingViewFuncCtlMapCell
{
    [MapCellKindCategory(DispensingViewMapCellGroup.DispensingViewMapCellGroupID_Str)]
    [FunctionalMapCell("{AF985A3A-DB2B-4B0C-9B8F-A94F4B5F75D7}")]
    class MapCellDispensingViewCtl_AMain : GriffinsPluginMngClass, IFunctionalMapCell
    {
        private static Bitmap bitmap = null!;

        static MapCellDispensingViewCtl_AMain()
        {
            var uri = new Uri("avares://GKG.Map.DispensingViewFuncCtlMapCell/Assets/Images/DispensingView.png");

            using (var stream = AssetLoader.Open(uri))
            {
                bitmap = new Bitmap(stream);
            }
        }

        #region IFunctionalMapCell接口实现

        void IFunctionalMapCell.Init(string pluginFileName) { }

        int IFunctionalMapCell.DefaultWidth => 710;

        int IFunctionalMapCell.DefaultHeight => 460;

        Bitmap IFunctionalMapCell.Ico => bitmap;

        string IFunctionalMapCell.MapCellKindName => ResourceA.DispensingView;

        IFunctionalMapCellDesigntime IFunctionalMapCell.CreateMapCellDesigntime(MapObjID mapCellID, string mapCellName) 
            => new MapCellDispensingViewCtlDesigntime(mapCellID, mapCellName);

        IFunctionalMapCellObject IFunctionalMapCell.CreateMapCellObj(MapObjID mapCellID, string mapCellName) 
            => new MapCellDispensingViewCtlObj(mapCellID, mapCellName);

        #endregion
    }
}
