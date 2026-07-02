using Avalonia.Media.Imaging;
using Avalonia.Platform;
using GF_Gereric;
using Griffins;
using System;

namespace GKG.Map.MapCell.Generic.ProgressBar
{
    [MapCellKindCategory(GenericMapCellGroup.GenericMapCellGroupID_Str)]
    [ControlMapCell("{4F6D8D21-42E2-4F9E-9F4E-87AA5A0B7D3C}")]
    class MapCellProgressBarCtl_AMain : GriffinsPluginMngClass, IControlMapCell
    {
        private static Bitmap bitmap = null;
        static MapCellProgressBarCtl_AMain()
        {
            try
            {
                var uri = new Uri("avares://GKG.Map.MapCell.Generic/Assets/Images/ctr-ProgressBar.png");
                using (var stream = AssetLoader.Open(uri))
                {
                    bitmap = new Bitmap(stream);
                }
            }
            catch
            {
            }
        }

        void IControlMapCell.Init(string pluginFileName)
        {
        }

        int IControlMapCell.DefaultWidth
        {
            get { return 160; }
        }

        int IControlMapCell.DefaultHeight
        {
            get { return 24; }
        }

        Bitmap IControlMapCell.Ico
        {
            get { return bitmap; }
        }

        string IControlMapCell.MapCellKindName
        {
            get { return ResourceA.ProgressBar; }
        }

        IControlMapCellDesigntime IControlMapCell.CreateMapCellDesigntime(MapObjID mapCellID, string mapCellName)
        {
            return new MapCellProgressBarCtlDesigntime(mapCellID, mapCellName);
        }

        IControlMapCellObject IControlMapCell.CreateMapCellObj(MapObjID mapCellID, string mapCellName)
        {
            return new MapCellProgressBarCtlObj(mapCellID, mapCellName);
        }
    }
}
