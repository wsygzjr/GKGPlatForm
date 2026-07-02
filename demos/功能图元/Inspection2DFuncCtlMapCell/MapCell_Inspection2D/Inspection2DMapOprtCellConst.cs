using Griffins.Map;

namespace GKG.Map.Inspection2DFuncCtlMapCell
{
    /// <summary>
    /// 2D检测图元操作原子ID定义
    /// 定义所有2D检测图元的操作单元格ID常量
    /// </summary>
    public class Inspection2DMapOprtCellConst
    {

        /// <summary>
        /// 文本颜色操作单元格ID字符串
        /// </summary>
        public const string TextColor_MapOprtCellIDStr = "29C894F1-5E87-5F04-BFF5-92F5E3F06FB8";
        /// <summary>
        /// 文本颜色操作单元格ID
        /// </summary>
        public static readonly MapOprtCellID TextColor_MapOprtCellID = MapOprtCellID.Parse(TextColor_MapOprtCellIDStr);

        /// <summary>
        /// 背景颜色操作单元格ID字符串
        /// </summary>
        public const string BackColor_MapOprtCellIDStr = "F5FD3E4E-3C5C-5F70-9C7C-AB35B4D2C2CA";
        /// <summary>
        /// 背景颜色操作单元格ID
        /// </summary>
        public static readonly MapOprtCellID BackColor_MapOprtCellID = MapOprtCellID.Parse(BackColor_MapOprtCellIDStr);

        /// <summary>
        /// 文本字体操作单元格ID字符串
        /// </summary>
        public const string TextFont_MapOprtCellIDStr = "C6C88BAF-83B2-5B77-B8DF-5CAB3C9E6C1C";
        /// <summary>
        /// 文本字体操作单元格ID
        /// </summary>
        public static readonly MapOprtCellID TextFont_MapOprtCellID = MapOprtCellID.Parse(TextFont_MapOprtCellIDStr);

    }
}
