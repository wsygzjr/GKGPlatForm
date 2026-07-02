using Avalonia.Media.Imaging;
using Avalonia.Platform;
using GF_Gereric;
using Griffins;
using System;

namespace GKG.Map.MapCell.Generic.IconButton
{
    [MapCellKindCategory(GenericMapCellGroup.GenericMapCellGroupID_Str)]
    [ControlMapCell("{A4854EFC-22EC-48D1-AA71-556F0C24F714}")]
    class MapCellIconButtonCtl_AMain : GriffinsPluginMngClass, IControlMapCell
    {
        private static Bitmap bitmap = null;

        static MapCellIconButtonCtl_AMain()
        {
            try
            {
                var uri = new Uri("avares://GKG.Map.MapCell.Generic/Assets/Images/ctr-IconButton.png");
                using var stream = AssetLoader.Open(uri);
                bitmap = new Bitmap(stream);
            }
            catch
            {
                bitmap = null;
            }
        }

        #region IMapCell 成员

        void IControlMapCell.Init(string pluginFileName)
        {
        }

        int IControlMapCell.DefaultWidth => 100;

        int IControlMapCell.DefaultHeight => 40;

        Bitmap IControlMapCell.Ico => bitmap;

        string IControlMapCell.MapCellKindName => Resource_IconButton.IconButton;

        IControlMapCellDesigntime IControlMapCell.CreateMapCellDesigntime(MapObjID mapCellID, string mapCellName)
        {
            return new MapCellIconButtonCtlDesigntime(mapCellID, mapCellName);
        }

        IControlMapCellObject IControlMapCell.CreateMapCellObj(MapObjID mapCellID, string mapCellName)
        {
            return new MapCellIconButtonCtlObj(mapCellID, mapCellName);
        }

        #endregion
    }
}