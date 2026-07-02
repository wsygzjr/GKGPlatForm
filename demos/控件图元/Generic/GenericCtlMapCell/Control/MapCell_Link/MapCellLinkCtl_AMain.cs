using System;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using GF_Gereric;
using Griffins;

namespace GKG.Map.MapCell.Generic.Link
{
    [MapCellKindCategory(GenericMapCellGroup.GenericMapCellGroupID_Str)]
    [ControlMapCell("{D0362D08-9AB5-4AE8-BB26-79FBA5588113}")]
    class MapCellLinkCtl_AMain : GriffinsPluginMngClass, IControlMapCell
    {
        private static Bitmap bitmap;

        static MapCellLinkCtl_AMain()
        {
            try
            {
                var uri = new Uri("avares://GKG.Map.MapCell.Generic/Assets/Images/ctr-TextButton.png");
                using var stream = AssetLoader.Open(uri);
                bitmap = new Bitmap(stream);
            }
            catch
            {
            }
        }

        void IControlMapCell.Init(string pluginFileName)
        {
        }

        int IControlMapCell.DefaultWidth => 160;

        int IControlMapCell.DefaultHeight => 36;

        Bitmap IControlMapCell.Ico => bitmap;

        string IControlMapCell.MapCellKindName => ResourceA.ResourceManager.GetString("Link", ResourceA.Culture) ?? "链接";

        IControlMapCellDesigntime IControlMapCell.CreateMapCellDesigntime(MapObjID mapCellID, string mapCellName)
        {
            return new MapCellLinkCtlDesigntime(mapCellID, mapCellName);
        }

        IControlMapCellObject IControlMapCell.CreateMapCellObj(MapObjID mapCellID, string mapCellName)
        {
            return new MapCellLinkCtlObj(mapCellID, mapCellName);
        }
    }
}
