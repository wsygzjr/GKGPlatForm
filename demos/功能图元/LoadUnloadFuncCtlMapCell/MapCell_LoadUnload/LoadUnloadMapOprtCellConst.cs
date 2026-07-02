using Griffins.Map;

namespace GKG.Map.LoadUnloadFuncCtlMapCell
{
    /// <summary>
    /// 上下料图元操作原子相关常量定义类
    /// </summary>
    internal static class LoadUnloadMapOprtCellConst
    {
        /// <summary>
        /// 容器数据组统一操作原子的 GUID 字符串
        /// </summary>
        public const string MaterialContainers_MapOprtCellIDStr = "{FBC81650-FBBD-4672-8170-BAA0F8C8FA17}";

        /// <summary>
        /// 容器数据组统一操作原子的强类型 ID 对象
        /// </summary>
        public static readonly MapOprtCellID MaterialContainers_MapOprtCellID = MapOprtCellID.Parse(MaterialContainers_MapOprtCellIDStr);
    }
}