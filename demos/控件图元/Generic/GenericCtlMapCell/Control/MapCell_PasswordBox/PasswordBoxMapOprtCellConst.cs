using System;

namespace GKG.Map.MapCell.Generic.PasswordBox
{
    /// <summary>
    /// 密码输入框图元操作原子常量定义
    /// </summary>
    internal class PasswordBoxMapOprtCellConst
    {
        // 画笔信息操作原子ID
        public const string BrushInfo_MapOprtCellIDStr = "{B1C2D3E4-1111-4AC8-BF66-281412CDE101}";
        public static readonly MapOprtCellID BrushInfo_MapOprtCellID = MapOprtCellID.Parse(BrushInfo_MapOprtCellIDStr);

        // 外观信息操作原子ID
        public const string AppearanceInfo_MapOprtCellIDStr = "{B1C2D3E4-2222-4AC8-BF66-281412CDE102}";
        public static readonly MapOprtCellID AppearanceInfo_MapOprtCellID = MapOprtCellID.Parse(AppearanceInfo_MapOprtCellIDStr);

        // 布局信息操作原子ID
        public const string LayoutInfo_MapOprtCellIDStr = "{B1C2D3E4-3333-4AC8-BF66-281412CDE103}";
        public static readonly MapOprtCellID LayoutInfo_MapOprtCellID = MapOprtCellID.Parse(LayoutInfo_MapOprtCellIDStr);

        // 公共信息操作原子ID
        public const string CommonInfo_MapOprtCellIDStr = "{B1C2D3E4-4444-4AC8-BF66-281412CDE104}";
        public static readonly MapOprtCellID CommonInfo_MapOprtCellID = MapOprtCellID.Parse(CommonInfo_MapOprtCellIDStr);

        // 文本信息操作原子ID
        public const string TextInfo_MapOprtCellIDStr = "{B1C2D3E4-5555-4AC8-BF66-281412CDE105}";
        public static readonly MapOprtCellID TextInfo_MapOprtCellID = MapOprtCellID.Parse(TextInfo_MapOprtCellIDStr);
    }
}
