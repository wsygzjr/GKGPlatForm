using Avalonia.Media.Imaging;
using Avalonia.Platform;
using GF_Gereric;
using Griffins;
using System;

namespace GKG.Map.MapCell.Generic.Control.MapCell_TextBox
{
    [MapCellKindCategory(GenericMapCellGroup.GenericMapCellGroupID_Str)]
    [ControlMapCell("{6A6B8B1C-8DD0-4D20-9B28-8E7B4CE6F2B4}")]
    /// <summary>
    /// 文本输入框控件插件入口
    /// </summary>
    class MapCellTextBoxCtl_AMain : GriffinsPluginMngClass, IControlMapCell
    {
        private static Bitmap bitmap;

        static MapCellTextBoxCtl_AMain()
        {
            // 加载控件图标（用于控件面板显示）；失败时允许为空
            try
            {
                var uri = new Uri("avares://GKG.Map.MapCell.Generic/Assets/Images/ctr-TextBox.png");
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

        string IControlMapCell.MapCellKindName => "文本输入框";

        IControlMapCellDesigntime IControlMapCell.CreateMapCellDesigntime(MapObjID mapCellID, string mapCellName)
        {
            // 设计时对象：用于设计器中设置默认配色等
            return new MapCellTextBoxCtlDesigntime(mapCellID, mapCellName);
        }

        IControlMapCellObject IControlMapCell.CreateMapCellObj(MapObjID mapCellID, string mapCellName)
        {
            // 运行时对象：真正参与渲染/交互/序列化的控件对象
            return new MapCellTextBoxCtlObj(mapCellID, mapCellName);
        }
    }
}
