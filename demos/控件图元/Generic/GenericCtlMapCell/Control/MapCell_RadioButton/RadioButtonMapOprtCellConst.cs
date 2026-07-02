using System;

namespace GKG.Map.MapCell.Generic.RadioButton
{
    /// <summary>
    /// 单选框图元操作原子常量定义
    /// </summary>
    internal class RadioButtonMapOprtCellConst
    {
        // 画笔信息操作原子ID
        public const string BrushInfo_MapOprtCellIDStr = "{C1A2B3D4-0001-4AC8-BF66-281412CDE201}";
        public static readonly MapOprtCellID BrushInfo_MapOprtCellID = MapOprtCellID.Parse(BrushInfo_MapOprtCellIDStr);

        // 外观信息操作原子ID
        public const string AppearanceInfo_MapOprtCellIDStr = "{C1A2B3D4-0002-4AC8-BF66-281412CDE202}";
        public static readonly MapOprtCellID AppearanceInfo_MapOprtCellID = MapOprtCellID.Parse(AppearanceInfo_MapOprtCellIDStr);

        // 布局信息操作原子ID
        public const string LayoutInfo_MapOprtCellIDStr = "{C1A2B3D4-0003-4AC8-BF66-281412CDE203}";
        public static readonly MapOprtCellID LayoutInfo_MapOprtCellID = MapOprtCellID.Parse(LayoutInfo_MapOprtCellIDStr);

        // 公共信息操作原子ID
        public const string CommonInfo_MapOprtCellIDStr = "{C1A2B3D4-0004-4AC8-BF66-281412CDE204}";
        public static readonly MapOprtCellID CommonInfo_MapOprtCellID = MapOprtCellID.Parse(CommonInfo_MapOprtCellIDStr);

        // 文本信息操作原子ID
        public const string TextInfo_MapOprtCellIDStr = "{C1A2B3D4-0005-4AC8-BF66-281412CDE205}";
        public static readonly MapOprtCellID TextInfo_MapOprtCellID = MapOprtCellID.Parse(TextInfo_MapOprtCellIDStr);
    }
}
