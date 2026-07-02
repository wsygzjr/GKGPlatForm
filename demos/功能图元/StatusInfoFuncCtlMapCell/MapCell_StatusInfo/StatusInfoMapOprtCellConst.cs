using Griffins.Map;

namespace GKG.Map.StatusInfoFuncCtlMapCell
{
    /// <summary>
    /// 状态信息图元操作原子ID定义
    /// 定义所有状态信息图元的操作单元格ID常量
    /// </summary>
    public class StatusInfoMapOprtCellConst
    {
        #region 基础操作原子

        /// <summary>
        /// 文本颜色操作单元格ID字符串
        /// </summary>
        public const string TextColor_MapOprtCellIDStr = "19C793E0-4D76-4E93-AEE4-91E4D2E95EA7";
        /// <summary>
        /// 文本颜色操作单元格ID
        /// </summary>
        public static readonly MapOprtCellID TextColor_MapOprtCellID = MapOprtCellID.Parse(TextColor_MapOprtCellIDStr);

        /// <summary>
        /// 背景颜色操作单元格ID字符串
        /// </summary>
        public const string BackColor_MapOprtCellIDStr = "E4FC2F3D-2B4B-4E6F-8B6B-9A24A3C1B1B9";
        /// <summary>
        /// 背景颜色操作单元格ID
        /// </summary>
        public static readonly MapOprtCellID BackColor_MapOprtCellID = MapOprtCellID.Parse(BackColor_MapOprtCellIDStr);

        /// <summary>
        /// 文本字体操作单元格ID字符串
        /// </summary>
        public const string TextFont_MapOprtCellIDStr = "B5B77A9E-72A1-4A66-A7CE-4B9A2B9D5B0B";
        /// <summary>
        /// 文本字体操作单元格ID
        /// </summary>
        public static readonly MapOprtCellID TextFont_MapOprtCellID = MapOprtCellID.Parse(TextFont_MapOprtCellIDStr);

        /// <summary>
        /// 双阀模式操作单元格ID字符串
        /// </summary>
        public const string IsDualValve_MapOprtCellIDStr = "D8E9F0A1-B2C3-4D5E-6F78-909A1B2C3D4E";
        /// <summary>
        /// 双阀模式操作单元格ID
        /// </summary>
        public static readonly MapOprtCellID IsDualValve_MapOprtCellID = MapOprtCellID.Parse(IsDualValve_MapOprtCellIDStr);

        #endregion

        #region 状态与时间

        /// <summary>
        /// 状态时间信息操作单元格ID字符串
        /// </summary>
        public const string StateTimeInfo_MapOprtCellIDStr = "17E1A83F-4D6F-49C0-9B89-2C7BFF47E5B7";
        /// <summary>
        /// 状态时间信息操作单元格ID
        /// </summary>
        public static readonly MapOprtCellID StateTimeInfo_MapOprtCellID = MapOprtCellID.Parse(StateTimeInfo_MapOprtCellIDStr);

        /// <summary>
        /// 状态信息操作单元格ID字符串
        /// </summary>
        public const string StateInfo_MapOprtCellIDStr = "E33AC66A-3D17-4DDE-A100-99BCF2093D1F";
        /// <summary>
        /// 状态信息操作单元格ID
        /// </summary>
        public static readonly MapOprtCellID StateInfo_MapOprtCellID = MapOprtCellID.Parse(StateInfo_MapOprtCellIDStr);

        /// <summary>
        /// 时间信息操作单元格ID字符串
        /// </summary>
        public const string TimeInfo_MapOprtCellIDStr = "2F0DDA93-2428-4C4C-9E20-83EA69DEDB73";
        /// <summary>
        /// 时间信息操作单元格ID
        /// </summary>
        public static readonly MapOprtCellID TimeInfo_MapOprtCellID = MapOprtCellID.Parse(TimeInfo_MapOprtCellIDStr);

        #endregion

        #region 动态操作原子示例

        /// <summary>
        /// 动态状态更新操作单元格ID字符串
        /// </summary>
        public const string DynamicStatusUpdate_MapOprtCellIDStr = "A1B2C3D4-E5F6-7890-ABCD-EF1234567890";
        /// <summary>
        /// 动态状态更新操作单元格ID
        /// </summary>
        public static readonly MapOprtCellID DynamicStatusUpdate_MapOprtCellID = MapOprtCellID.Parse(DynamicStatusUpdate_MapOprtCellIDStr);

        /// <summary>
        /// 动态时间更新操作单元格ID字符串
        /// </summary>
        public const string DynamicTimeUpdate_MapOprtCellIDStr = "B2C3D4E5-F6A7-8901-BCDE-F23456789012";
        /// <summary>
        /// 动态时间更新操作单元格ID
        /// </summary>
        public static readonly MapOprtCellID DynamicTimeUpdate_MapOprtCellID = MapOprtCellID.Parse(DynamicTimeUpdate_MapOprtCellIDStr);

        /// <summary>
        /// 动态颜色变化操作单元格ID字符串
        /// </summary>
        public const string DynamicColorChange_MapOprtCellIDStr = "C3D4E5F6-A7A8-9012-CDEF-345678901234";
        /// <summary>
        /// 动态颜色变化操作单元格ID
        /// </summary>
        public static readonly MapOprtCellID DynamicColorChange_MapOprtCellID = MapOprtCellID.Parse(DynamicColorChange_MapOprtCellIDStr);

        #endregion
    }
}
