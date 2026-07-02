using System;

namespace GKG.Map.MapCell.Generic.Control.MapCell_Image
{
    /// <summary>
    /// 图片图元操作原子常量定义
    /// </summary>
    internal class ImageMapOprtCellConst
    {
        // 外观信息操作原子ID
        public const string AppearanceInfo_MapOprtCellIDStr = "{C2B3D4E5-0001-4AC8-BF66-381512DEF201}";
        public static readonly MapOprtCellID AppearanceInfo_MapOprtCellID = MapOprtCellID.Parse(AppearanceInfo_MapOprtCellIDStr);

        // 公共信息操作原子ID
        public const string CommonInfo_MapOprtCellIDStr = "{C2B3D4E5-0002-4AC8-BF66-381512DEF202}";
        public static readonly MapOprtCellID CommonInfo_MapOprtCellID = MapOprtCellID.Parse(CommonInfo_MapOprtCellIDStr);

        // 布局信息操作原子ID
        public const string LayoutInfo_MapOprtCellIDStr = "{C2B3D4E5-0003-4AC8-BF66-381512DEF203}";
        public static readonly MapOprtCellID LayoutInfo_MapOprtCellID = MapOprtCellID.Parse(LayoutInfo_MapOprtCellIDStr);
    }
}
