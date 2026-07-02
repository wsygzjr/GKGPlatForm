using System;

namespace GKG.Map.MapCell.Generic.Control.MapCell_ComboBox
{
    internal class ComboBoxMapOprtCellConst
    {
        // 1. 数据与行为组 (Items, SelectedItem, Placeholder 等)
        public const string DataGroup_MapOprtCellIDStr = "{D1A2B3C4-1111-4E5F-8A9B-0C1D2E3F4A5B}";
        public static readonly MapOprtCellID DataGroup_MapOprtCellID = MapOprtCellID.Parse(DataGroup_MapOprtCellIDStr);

        // 2. 样式外观组 (Colors, Opacity 等)
        public const string StyleGroup_MapOprtCellIDStr = "{E5F6A7B8-2222-4C5D-9E8F-1A2B3C4D5E6F}";
        public static readonly MapOprtCellID StyleGroup_MapOprtCellID = MapOprtCellID.Parse(StyleGroup_MapOprtCellIDStr);

        // 3. 字体文本组 (FontFamily, FontSize 等)
        public const string FontGroup_MapOprtCellIDStr = "{F9A0B1C2-3333-4D6E-A1B2-C3D4E5F6A7B8}";
        public static readonly MapOprtCellID FontGroup_MapOprtCellID = MapOprtCellID.Parse(FontGroup_MapOprtCellIDStr);

        // 4. 布局位置组 (Margin, Padding 等)
        public const string LayoutGroup_MapOprtCellIDStr = "{A3B4C5D6-4444-4E7F-B1C2-D3E4F5A6B7C8}";
        public static readonly MapOprtCellID LayoutGroup_MapOprtCellID = MapOprtCellID.Parse(LayoutGroup_MapOprtCellIDStr);
    }
}