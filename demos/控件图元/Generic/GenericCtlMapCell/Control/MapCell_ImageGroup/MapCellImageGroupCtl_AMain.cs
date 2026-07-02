using Avalonia.Media.Imaging;
using Avalonia.Platform;
using GF_Gereric;
using Griffins;
using GKG.Map.MapCell.Generic.Control.MapCell_ImageGroup.Factories;
using System;

namespace GKG.Map.MapCell.Generic.Control.MapCell_ImageGroup
{
    [MapCellKindCategory(GenericMapCellGroup.GenericMapCellGroupID_Str)]
    [ControlMapCell("{D3C4E5F6-978F-55EF-ACF2-69106FB3C983}")]
    class MapCellImageGroupCtl_AMain : GriffinsPluginMngClass, IControlMapCell
    {
        private static Bitmap bitmap = null;

        static MapCellImageGroupCtl_AMain()
        {
            try
            {
                ImageGroupEditorFactoryRegistration.RegisterGlobal();
                var uri = new Uri("avares://GKG.Map.MapCell.Generic/Assets/Images/ctr-ImageGroup.png");
                using (var stream = AssetLoader.Open(uri))
                {
                    bitmap = new Bitmap(stream);
                }
            }
            catch
            {
                // 如果图标不存在，使用null
                bitmap = null;
            }
        }

        #region IControlMapCell 成员

        void IControlMapCell.Init(string pluginFileName)
        {
            ImageGroupEditorFactoryRegistration.RegisterGlobal();
        }

        int IControlMapCell.DefaultWidth => 100;

        int IControlMapCell.DefaultHeight => 100;

        Bitmap IControlMapCell.Ico => bitmap;

        string IControlMapCell.MapCellKindName => ResourceA.ImageGroupMapCell;

        IControlMapCellDesigntime IControlMapCell.CreateMapCellDesigntime(MapObjID mapCellID, string mapCellName)
        {
            return new MapCellImageGroupCtlDesigntime(mapCellID, mapCellName);
        }

        IControlMapCellObject IControlMapCell.CreateMapCellObj(MapObjID mapCellID, string mapCellName)
        {
            return new MapCellImageGroupCtlObj(mapCellID, mapCellName);
        }

        #endregion
    }
}
