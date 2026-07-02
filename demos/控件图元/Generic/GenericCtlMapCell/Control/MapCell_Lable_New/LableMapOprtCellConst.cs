using System;

namespace GKG.Map.MapCell.Generic.Control.Lable
{
    /// <summary>
    /// 标签图元操作原子常量定义 (标准4组)
    /// </summary>
    internal class LableMapOprtCellConst
    {
        // 1. 数据与行为组 (Text, IsEnabled, ToolTip等)
        public const string DataGroup_MapOprtCellIDStr = "{B4B8E579-157A-462E-BE53-3AAD72213780}";
        public static readonly MapOprtCellID DataGroup_MapOprtCellID = MapOprtCellID.Parse(DataGroup_MapOprtCellIDStr);

        // 2. 样式外观组 (Colors, Opacity, Border等)
        public const string StyleGroup_MapOprtCellIDStr = "{BF7B5337-6F10-4A12-B3B7-0748D996A9DF}";
        public static readonly MapOprtCellID StyleGroup_MapOprtCellID = MapOprtCellID.Parse(StyleGroup_MapOprtCellIDStr);

        // 3. 字体文本组 (FontFamily, FontSize, LineHeight等)
        public const string FontGroup_MapOprtCellIDStr = "{505750CC-B48E-4142-87F3-22BCB54F51DE}";
        public static readonly MapOprtCellID FontGroup_MapOprtCellID = MapOprtCellID.Parse(FontGroup_MapOprtCellIDStr);

        // 4. 布局位置组 (Margin, Padding, Alignment等)
        public const string LayoutGroup_MapOprtCellIDStr = "{483F3C4A-FD7C-4510-8F7D-02E3F54C1E25}";
        public static readonly MapOprtCellID LayoutGroup_MapOprtCellID = MapOprtCellID.Parse(LayoutGroup_MapOprtCellIDStr);
    }
}