using System;

namespace GKG.Map.MapCell.Generic.CheckBox
{
    /// <summary>
    /// 复选框图元操作原子常量定义
    /// </summary>
    internal class CheckBoxMapOprtCellConst
    {
        // 画笔信息操作原子ID
        public const string BrushInfo_MapOprtCellIDStr = "{D7B4B728-DE0E-4D2F-B73F-6D2AF0DA1435}";
        public static readonly MapOprtCellID BrushInfo_MapOprtCellID = MapOprtCellID.Parse(BrushInfo_MapOprtCellIDStr);

        // 外观信息操作原子ID
        public const string AppearanceInfo_MapOprtCellIDStr = "{32BEB4A6-D670-44CF-A858-EDEE15EDFCDF}";
        public static readonly MapOprtCellID AppearanceInfo_MapOprtCellID = MapOprtCellID.Parse(AppearanceInfo_MapOprtCellIDStr);

        // 布局信息操作原子ID
        public const string LayoutInfo_MapOprtCellIDStr = "{3ACBD75F-229D-4182-8150-13BC3D1BBFEE}";
        public static readonly MapOprtCellID LayoutInfo_MapOprtCellID = MapOprtCellID.Parse(LayoutInfo_MapOprtCellIDStr);

        // 公共信息操作原子ID
        public const string CommonInfo_MapOprtCellIDStr = "{7989D1C3-629B-4D9D-85EF-24CBF0AB1B91}";
        public static readonly MapOprtCellID CommonInfo_MapOprtCellID = MapOprtCellID.Parse(CommonInfo_MapOprtCellIDStr);

        // 文本信息操作原子ID
        public const string TextInfo_MapOprtCellIDStr = "{55D75915-6FD1-42CA-AB7A-9896C5B558B6}";
        public static readonly MapOprtCellID TextInfo_MapOprtCellID = MapOprtCellID.Parse(TextInfo_MapOprtCellIDStr);
    }
}
