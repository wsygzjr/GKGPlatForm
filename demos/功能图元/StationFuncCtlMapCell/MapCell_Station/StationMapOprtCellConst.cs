using Griffins.Map;

namespace GKG.Map.StationFuncCtlMapCell
{
    /// <summary>
    /// 工位图元操作原子相关常量定义
    /// </summary>
    internal static class StationMapOprtCellConst
    {
        /// <summary>
        /// 数据组统一操作原子的 GUID 字符串
        /// </summary>
        public const string DataGroup_MapOprtCellIDStr = "{5531355D-4BA3-4110-B031-A73F4D4A2C6C}";

        /// <summary>
        /// 数据组统一操作原子的强类型 ID 对象
        /// </summary>
        public static readonly MapOprtCellID DataGroup_MapOprtCellID = MapOprtCellID.Parse(DataGroup_MapOprtCellIDStr);
    }
}
