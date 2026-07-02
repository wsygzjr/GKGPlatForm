using System;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using GF_Gereric;
using Griffins.Map;
using Griffins.Map.UI;

namespace GKG.Map.CommunicationFuncCtlMapCell
{
    [MapCellKindCategory(CommunicationMapCellGroup.CommunicationMapCellGroupID_Str)]
    [FunctionalMapCell("{C508D61E-5B7E-4BA5-8112-910BD11A8CC9}")]
    class MapCellCommunicationCtl_AMain : GriffinsPluginMngClass, IFunctionalMapCell
    {
        private static Bitmap bitmap = null;

        static MapCellCommunicationCtl_AMain()
        {
            var uri = new Uri("avares://GKG.Map.CommunicationFuncCtlMapCell/Assets/Images/ctr-Communication.png");
            using (var stream = AssetLoader.Open(uri))
            {
                bitmap = new Bitmap(stream);
            }
        }

        void IFunctionalMapCell.Init(string pluginFileName)
        {
        }

        int IFunctionalMapCell.DefaultWidth => 500;

        int IFunctionalMapCell.DefaultHeight => 650;

        Bitmap IFunctionalMapCell.Ico => bitmap;

        string IFunctionalMapCell.MapCellKindName => ResourceA.Communication;

        IFunctionalMapCellDesigntime IFunctionalMapCell.CreateMapCellDesigntime(MapObjID mapCellID, string mapCellName)
        {
            return new MapCellCommunicationCtlDesigntime(mapCellID, mapCellName);
        }

        IFunctionalMapCellObject IFunctionalMapCell.CreateMapCellObj(MapObjID mapCellID, string mapCellName)
        {
            return new MapCellCommunicationCtlObj(mapCellID, mapCellName);
        }
    }
}
