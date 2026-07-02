using System;
using System.ComponentModel;

namespace GKG.Map.MapCell.Generic.IconButton
{
    // 按钮专属的图标位置枚举
    public enum IconPlacementType
    {
        [Description("居左")] Left = 0,
        [Description("居右")] Right = 1,
        [Description("居上")] Top = 2,
        [Description("居下")] Bottom = 3
    }

    internal class IconButtonMapOprtCellConst
    {
        // 1. 数据与行为组 (文本, Base64图片, 启用状态等)
        public const string DataGroup_MapOprtCellIDStr = "{63581D64-FF5F-461C-841E-24EC01DEB365}";
        public static readonly MapOprtCellID DataGroup_MapOprtCellID = MapOprtCellID.Parse(DataGroup_MapOprtCellIDStr);

        // 2. 样式外观组 (颜色, 圆角, 透明度等)
        public const string StyleGroup_MapOprtCellIDStr = "{5903055E-E1B1-4BFB-8221-DD850C1B7BBF}";
        public static readonly MapOprtCellID StyleGroup_MapOprtCellID = MapOprtCellID.Parse(StyleGroup_MapOprtCellIDStr);

        // 3. 字体文本组 (字号, 加粗, 斜体等)
        public const string FontGroup_MapOprtCellIDStr = "{75747033-AF27-47EA-B3C6-2AD041D6EE3F}";
        public static readonly MapOprtCellID FontGroup_MapOprtCellID = MapOprtCellID.Parse(FontGroup_MapOprtCellIDStr);

        // 4. 布局位置组 (图标位置, 图标尺寸, 图文间距等)
        public const string LayoutGroup_MapOprtCellIDStr = "{AC2897E8-7F14-42D9-8212-B89F0953B57C}";
        public static readonly MapOprtCellID LayoutGroup_MapOprtCellID = MapOprtCellID.Parse(LayoutGroup_MapOprtCellIDStr);
    }
}