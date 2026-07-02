using Avalonia.Media.Imaging;
using Avalonia.Platform;
using GF_Gereric;
using Griffins;
using System;

namespace GKG.Map.MapCell.Generic.RadioButton
{
    [MapCellKindCategory(GenericMapCellGroup.GenericMapCellGroupID_Str)]
    [ControlMapCell("{9DEF6881-867E-44DF-9BF1-59005FA2B872}")]
    class MapCellRadioButtonCtl_AMain : GriffinsPluginMngClass, IControlMapCell
    {
        private static Bitmap bitmap = null;
        static MapCellRadioButtonCtl_AMain()
        {
            try
            {
                var uri = new Uri("avares://GKG.Map.MapCell.Generic/Assets/Images/ctr-RadioButton.png");
                using (var stream = AssetLoader.Open(uri))
                {
                    bitmap = new Bitmap(stream);
                }
            }
            catch
            {
                // 如果图标不存在，使用默认图标
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
            get { return ResourceA.RadioButton; }
        }

        IControlMapCellDesigntime IControlMapCell.CreateMapCellDesigntime(MapObjID mapCellID, string mapCellName)
        {
            return new MapCellRadioButtonCtlDesigntime(mapCellID, mapCellName);
        }

        IControlMapCellObject IControlMapCell.CreateMapCellObj(MapObjID mapCellID, string mapCellName)
        {
            return new MapCellRadioButtonCtlObj(mapCellID, mapCellName);
        }

        #endregion
    }
}
