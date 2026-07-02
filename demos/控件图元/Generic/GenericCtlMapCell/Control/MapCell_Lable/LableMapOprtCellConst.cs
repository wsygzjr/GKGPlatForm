using System;

namespace GKG.Map.MapCell.Generic.Lable
{
    /// <summary>
    /// 标签图元操作原子常量定义
    /// </summary>
    internal class LableMapOprtCellConst
    {
        // 画笔信息操作原子ID
        public const string BrushInfo_MapOprtCellIDStr = "{B1A2C3D4-0001-4AC8-BF66-281412CDE101}";
        public static readonly MapOprtCellID BrushInfo_MapOprtCellID = MapOprtCellID.Parse(BrushInfo_MapOprtCellIDStr);

        // 外观信息操作原子ID
        public const string AppearanceInfo_MapOprtCellIDStr = "{62CB9A5F-95F4-4E22-8B8E-026B07531FA4}";
        public static readonly MapOprtCellID AppearanceInfo_MapOprtCellID = MapOprtCellID.Parse(AppearanceInfo_MapOprtCellIDStr);

        // 布局信息操作原子ID
        public const string LayoutInfo_MapOprtCellIDStr = "{41916B33-F121-4A7B-A204-A641B68BE06E}";
        public static readonly MapOprtCellID LayoutInfo_MapOprtCellID = MapOprtCellID.Parse(LayoutInfo_MapOprtCellIDStr);

        // 公共信息操作原子ID
        public const string CommonInfo_MapOprtCellIDStr = "{62AC0E74-C4CE-4C47-8A9A-A6BB8812E274}";
        public static readonly MapOprtCellID CommonInfo_MapOprtCellID = MapOprtCellID.Parse(CommonInfo_MapOprtCellIDStr);

        // 字体信息操作原子ID
        public const string FontInfo_MapOprtCellIDStr = "{DB38286A-800C-4400-9D81-085C982F5067}";
        public static readonly MapOprtCellID FontInfo_MapOprtCellID = MapOprtCellID.Parse(FontInfo_MapOprtCellIDStr);

        // 段落信息操作原子ID
        public const string ParagraphInfo_MapOprtCellIDStr = "{32C6C236-61D9-4F0B-BAE6-B245C7F0B806}";
        public static readonly MapOprtCellID ParagraphInfo_MapOprtCellID = MapOprtCellID.Parse(ParagraphInfo_MapOprtCellIDStr);
    }
}
