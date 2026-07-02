using System;

namespace GKG.Map.MapCell.Generic.Stepper
{
    /// <summary>
    /// 步进器图元操作原子常量定义
    /// </summary>
    internal class StepperMapOprtCellConst
    {
        // 画笔信息操作原子ID
        public const string BrushInfo_MapOprtCellIDStr = "{C1A2B3D4-1111-4AC8-BF66-281412CDE211}";
        public static readonly MapOprtCellID BrushInfo_MapOprtCellID = MapOprtCellID.Parse(BrushInfo_MapOprtCellIDStr);

        // 外观信息操作原子ID
        public const string AppearanceInfo_MapOprtCellIDStr = "{C1A2B3D4-2222-4AC8-BF66-281412CDE212}";
        public static readonly MapOprtCellID AppearanceInfo_MapOprtCellID = MapOprtCellID.Parse(AppearanceInfo_MapOprtCellIDStr);

        // 布局信息操作原子ID
        public const string LayoutInfo_MapOprtCellIDStr = "{C1A2B3D4-3333-4AC8-BF66-281412CDE213}";
        public static readonly MapOprtCellID LayoutInfo_MapOprtCellID = MapOprtCellID.Parse(LayoutInfo_MapOprtCellIDStr);

        // 公共信息操作原子ID
        public const string CommonInfo_MapOprtCellIDStr = "{C1A2B3D4-4444-4AC8-BF66-281412CDE214}";
        public static readonly MapOprtCellID CommonInfo_MapOprtCellID = MapOprtCellID.Parse(CommonInfo_MapOprtCellIDStr);

        // 文本信息操作原子ID
        public const string TextInfo_MapOprtCellIDStr = "{C1A2B3D4-5555-4AC8-BF66-281412CDE215}";
        public static readonly MapOprtCellID TextInfo_MapOprtCellID = MapOprtCellID.Parse(TextInfo_MapOprtCellIDStr);

        // 宽度操作原子ID
        public const string Width_MapOprtCellIDStr = "{C1A2B3D4-6666-4AC8-BF66-281412CDE216}";
        public static readonly MapOprtCellID Width_MapOprtCellID = MapOprtCellID.Parse(Width_MapOprtCellIDStr);

        // 高度操作原子ID
        public const string Height_MapOprtCellIDStr = "{C1A2B3D4-7777-4AC8-BF66-281412CDE217}";
        public static readonly MapOprtCellID Height_MapOprtCellID = MapOprtCellID.Parse(Height_MapOprtCellIDStr);
    }
}
