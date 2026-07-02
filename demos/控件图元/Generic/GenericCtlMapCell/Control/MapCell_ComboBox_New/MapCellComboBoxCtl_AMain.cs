using Avalonia.Media.Imaging;
using Avalonia.Platform;
using GF_Gereric;
using Griffins;
using GKG.Map.MapCell.Generic.GroupPanel;
using System;

namespace GKG.Map.MapCell.Generic.Control.MapCell_ComboBox
{
    [MapCellKindCategory(GenericMapCellGroup.GenericMapCellGroupID_Str)]
    [ControlMapCell("{A7E8B4A2-9F3C-4D2D-9B13-1B1B7B3F9C01}")]
    class MapCellComboBoxCtl_AMain : GriffinsPluginMngClass, IControlMapCell
    {
        private static Bitmap bitmap;

        static MapCellComboBoxCtl_AMain()
        {
            try
            {
                var uri = new Uri("avares://GKG.Map.MapCell.Generic/Assets/Images/ctr-ComboBox.png");
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
            // 预留：插件初始化回调（本控件暂无额外初始化逻辑）
        }

        int IControlMapCell.DefaultWidth => 180;

        int IControlMapCell.DefaultHeight => 36;

        Bitmap IControlMapCell.Ico => bitmap;

        string IControlMapCell.MapCellKindName => "下拉框new";

        IControlMapCellDesigntime IControlMapCell.CreateMapCellDesigntime(MapObjID mapCellID, string mapCellName)
        {
            return new MapCellComboBoxCtlDesigntime(mapCellID, mapCellName);
        }

        IControlMapCellObject IControlMapCell.CreateMapCellObj(MapObjID mapCellID, string mapCellName)
        {
            return new MapCellComboBoxCtlObj(mapCellID, mapCellName);
        }
    }
}
