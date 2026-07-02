using System;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using GF_Gereric;
using Griffins;

namespace GKG.Map.MapCell.Generic.Control.MapCell_DateInput
{
    [MapCellKindCategory(GenericMapCellGroup.GenericMapCellGroupID_Str)]
    [ControlMapCell("{4E767D2A-921B-43EC-8766-486FB2419103}")]
    internal class MapCellDateInputCtl_AMain : GriffinsPluginMngClass, IControlMapCell
    {
        private static Bitmap bitmap;

        static MapCellDateInputCtl_AMain()
        {
            try
            {
                Uri uri = new("avares://GKG.Map.MapCell.Generic/Assets/Images/ctr-DateInput.png");
                using var stream = AssetLoader.Open(uri);
                bitmap = new Bitmap(stream);
            }
            catch
            {
                bitmap = null;
            }
        }

        void IControlMapCell.Init(string pluginFileName)
        {
        }

        int IControlMapCell.DefaultWidth => 180;

        int IControlMapCell.DefaultHeight => 36;

        Bitmap IControlMapCell.Ico => bitmap;

        string IControlMapCell.MapCellKindName => "日期输入框";

        IControlMapCellDesigntime IControlMapCell.CreateMapCellDesigntime(MapObjID mapCellID, string mapCellName)
        {
            return new MapCellDateInputCtlDesigntime(mapCellID, mapCellName);
        }

        IControlMapCellObject IControlMapCell.CreateMapCellObj(MapObjID mapCellID, string mapCellName)
        {
            return new MapCellDateInputCtlObj(mapCellID, mapCellName);
        }
    }
}
