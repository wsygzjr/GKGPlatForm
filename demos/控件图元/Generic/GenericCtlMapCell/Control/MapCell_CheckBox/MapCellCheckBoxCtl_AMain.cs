using Avalonia.Media.Imaging;
using Avalonia.Platform;
using GF_Gereric;
using Griffins;
using System;

namespace GKG.Map.MapCell.Generic.CheckBox
{
    [MapCellKindCategory(GenericMapCellGroup.GenericMapCellGroupID_Str)]
    [ControlMapCell("{2E9F2CF7-2A76-4C97-9C5E-1D6A7DAE35D8}")]
    class MapCellCheckBoxCtl_AMain : GriffinsPluginMngClass, IControlMapCell
    {
        private static Bitmap bitmap = null;
        static MapCellCheckBoxCtl_AMain()
        {
            try
            {
                var uri = new Uri("avares://GKG.Map.MapCell.Generic/Assets/Images/ctr-CheckBox.png");
                using (var stream = AssetLoader.Open(uri))
                {
                    bitmap = new Bitmap(stream);
                }
            }
            catch
            {
            }
        }

        #region IMapCell 成员

        void IControlMapCell.Init(string pluginFileName)
        {
        }

        int IControlMapCell.DefaultWidth
        {
            get { return 100; }
        }

        int IControlMapCell.DefaultHeight
        {
            get { return 30; }
        }

        Bitmap IControlMapCell.Ico
        {
            get { return bitmap; }
        }

        string IControlMapCell.MapCellKindName
        {
            get { return ResourceA.CheckBox; }
        }

        IControlMapCellDesigntime IControlMapCell.CreateMapCellDesigntime(MapObjID mapCellID, string mapCellName)
        {
            return new MapCellCheckBoxCtlDesigntime(mapCellID, mapCellName);
        }

        IControlMapCellObject IControlMapCell.CreateMapCellObj(MapObjID mapCellID, string mapCellName)
        {
            return new MapCellCheckBoxCtlObj(mapCellID, mapCellName);
        }

        #endregion
    }
}
