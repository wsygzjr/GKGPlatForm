using System;

namespace GKG.Map.MapCell.Generic.Control.MapCell_ImageGroup
{
    /// <summary>
    /// 图片组图元操作原子常量定义
    /// </summary>
    internal class ImageGroupMapOprtCellConst
    {
        // 外观信息操作原子ID
        public const string AppearanceInfo_MapOprtCellIDStr = "{D3C4E5F6-0001-4AC8-BF66-481612EEF301}";
        public static readonly MapOprtCellID AppearanceInfo_MapOprtCellID = MapOprtCellID.Parse(AppearanceInfo_MapOprtCellIDStr);

        // 公共信息操作原子ID
        public const string CommonInfo_MapOprtCellIDStr = "{D3C4E5F6-0002-4AC8-BF66-481612EEF302}";
        public static readonly MapOprtCellID CommonInfo_MapOprtCellID = MapOprtCellID.Parse(CommonInfo_MapOprtCellIDStr);

        // 布局信息操作原子ID
        public const string LayoutInfo_MapOprtCellIDStr = "{D3C4E5F6-0003-4AC8-BF66-481612EEF303}";
        public static readonly MapOprtCellID LayoutInfo_MapOprtCellID = MapOprtCellID.Parse(LayoutInfo_MapOprtCellIDStr);

        // 当前图片索引操作原子ID（独立操作，用于运行时切换图片）
        public const string CurrentIndex_MapOprtCellIDStr = "{D3C4E5F6-0004-4AC8-BF66-481612EEF304}";
        public static readonly MapOprtCellID CurrentIndex_MapOprtCellID = MapOprtCellID.Parse(CurrentIndex_MapOprtCellIDStr);
    }
}
