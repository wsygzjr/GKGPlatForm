using Avalonia.Media.Imaging;
using Avalonia.Platform;
using GF_Gereric;
using GKG.Map.MapCell.Generic.Control.Lable;
using Griffins;
using System;

namespace GKG.Map.MapCell.Generic.Control.Lable
{
    [MapCellKindCategory(GenericMapCellGroup.GenericMapCellGroupID_Str)]
    [ControlMapCell("{8DEF6881-867E-44DF-9BF1-59005FA2B871}")]
    class MapCellLableCtl_AMain : GriffinsPluginMngClass, IControlMapCell
    {
        private static Bitmap bitmap = null;

        static MapCellLableCtl_AMain()
        {
            try
            {
                // 路径格式：项目根命名空间;component/相对路径
                var uri = new Uri("avares://GKG.Map.MapCell.Generic/Assets/Images/ctr-Lable.png");
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

        string IControlMapCell.MapCellKindName => Resource_Lable.Lable;

        IControlMapCellDesigntime IControlMapCell.CreateMapCellDesigntime(MapObjID mapCellID, string mapCellName)
        {
            return new MapCellLableCtlDesigntime(mapCellID, mapCellName);
        }

        IControlMapCellObject IControlMapCell.CreateMapCellObj(MapObjID mapCellID, string mapCellName)
        {
            return new MapCellLableCtlObj(mapCellID, mapCellName);
        }

        #endregion
    }
}