using System;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using GF_Gereric;
using Griffins;

namespace GKG.Map.MapCell.Generic.Control.MapCell_Calendar
{
    [MapCellKindCategory(GenericMapCellGroup.GenericMapCellGroupID_Str)]
    [ControlMapCell("{6DBC7438-014D-4F47-B406-457C88803FDD}")]
    internal class MapCellCalendarCtl_AMain : GriffinsPluginMngClass, IControlMapCell
    {
        private static Bitmap? bitmap;

        static MapCellCalendarCtl_AMain()
        {
            try
            {
                Uri uri = new("avares://GKG.Map.MapCell.Generic/Assets/Images/ctr-Calendar.png");
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

        int IControlMapCell.DefaultWidth => 280;

        int IControlMapCell.DefaultHeight => 280;

        Bitmap? IControlMapCell.Ico => bitmap;

        string IControlMapCell.MapCellKindName => "日历";

        IControlMapCellDesigntime IControlMapCell.CreateMapCellDesigntime(MapObjID mapCellID, string mapCellName)
        {
            return new MapCellCalendarCtlDesigntime(mapCellID, mapCellName);
        }

        IControlMapCellObject IControlMapCell.CreateMapCellObj(MapObjID mapCellID, string mapCellName)
        {
            return new MapCellCalendarCtlObj(mapCellID, mapCellName);
        }
    }
}
