using System;

namespace GKG.Map.MapCell.Generic.IconButton
{
    /// <summary>
    /// 图标按钮图元操作原子常量定义
    /// </summary>
    internal class IconButtonMapOprtCellConst
    {
        // 画笔信息操作原子ID
        public const string BrushInfo_MapOprtCellIDStr = "{C1B2D3E4-0001-4AC8-BF66-281412CDE101}";
        public static readonly MapOprtCellID BrushInfo_MapOprtCellID = MapOprtCellID.Parse(BrushInfo_MapOprtCellIDStr);

        // 外观信息操作原子ID
        public const string AppearanceInfo_MapOprtCellIDStr = "{C1B2D3E4-0002-4AC8-BF66-281412CDE102}";
        public static readonly MapOprtCellID AppearanceInfo_MapOprtCellID = MapOprtCellID.Parse(AppearanceInfo_MapOprtCellIDStr);

        // 布局信息操作原子ID
        public const string LayoutInfo_MapOprtCellIDStr = "{C1B2D3E4-0003-4AC8-BF66-281412CDE103}";
        public static readonly MapOprtCellID LayoutInfo_MapOprtCellID = MapOprtCellID.Parse(LayoutInfo_MapOprtCellIDStr);

        // 公共信息操作原子ID
        public const string CommonInfo_MapOprtCellIDStr = "{C1B2D3E4-0004-4AC8-BF66-281412CDE104}";
        public static readonly MapOprtCellID CommonInfo_MapOprtCellID = MapOprtCellID.Parse(CommonInfo_MapOprtCellIDStr);

        // 字体信息操作原子ID
        public const string FontInfo_MapOprtCellIDStr = "{C1B2D3E4-0005-4AC8-BF66-281412CDE105}";
        public static readonly MapOprtCellID FontInfo_MapOprtCellID = MapOprtCellID.Parse(FontInfo_MapOprtCellIDStr);

        // 段落信息操作原子ID
        public const string ParagraphInfo_MapOprtCellIDStr = "{C1B2D3E4-0006-4AC8-BF66-281412CDE106}";
        public static readonly MapOprtCellID ParagraphInfo_MapOprtCellID = MapOprtCellID.Parse(ParagraphInfo_MapOprtCellIDStr);

        // 杂项信息操作原子ID
        public const string MiscInfo_MapOprtCellIDStr = "{C1B2D3E4-0007-4AC8-BF66-281412CDE107}";
        public static readonly MapOprtCellID MiscInfo_MapOprtCellID = MapOprtCellID.Parse(MiscInfo_MapOprtCellIDStr);
    }
}