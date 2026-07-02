
using System;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using GF_Gereric;
using Griffins.Map;
using Griffins.Map.UI;

namespace GKG.Map.LogFuncCtlMapCell
{
    [MapCellKindCategory(LogMapCellGroup.LogMapCellGroupID_Str)]
    [FunctionalMapCell("{27D6A186-0E63-4E46-8E64-56E2E716F722}")]
    class MapCellLogCtl_AMain : GriffinsPluginMngClass, IFunctionalMapCell
    {
        private static Bitmap bitmap = null;

        static MapCellLogCtl_AMain()
        {
            var uri = new Uri("avares://GKG.Map.LogFuncCtlMapCell/Assets/Log.png");
            using (var stream = AssetLoader.Open(uri))
            {
                bitmap = new Bitmap(stream);
            }
        }

        void IFunctionalMapCell.Init(string pluginFileName)
        {
        }

        int IFunctionalMapCell.DefaultWidth
        {
            get { return 760; }
        }

        int IFunctionalMapCell.DefaultHeight
        {
            get { return 410; }
        }

        Bitmap IFunctionalMapCell.Ico
        {
            get { return bitmap; }
        }

        string IFunctionalMapCell.MapCellKindName
        {
            get { return ResourceA.Log; }
        }

        IFunctionalMapCellDesigntime IFunctionalMapCell.CreateMapCellDesigntime(MapObjID mapCellID, string mapCellName)
        {
            return new MapCellLogCtlDesigntime(mapCellID, mapCellName);
        }

        IFunctionalMapCellObject IFunctionalMapCell.CreateMapCellObj(MapObjID mapCellID, string mapCellName)
        {
            return new MapCellLogCtlObj(mapCellID, mapCellName);
        }
    }
}
