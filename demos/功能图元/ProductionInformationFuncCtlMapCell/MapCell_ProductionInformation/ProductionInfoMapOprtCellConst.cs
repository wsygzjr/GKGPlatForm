using Griffins.Map;

namespace GKG.Map.ProductionInformationFuncCtlMapCell
{
    /// <summary>
    /// 生产信息图元操作原子常量定义类
    /// 声明为静态类以防止被意外实例化，专门用于存储全局唯一的 ID 配置。
    /// </summary>
    internal static class ProductionInfoMapOprtCellConst
    {
        /// <summary>
        /// 数据组统一操作原子的 GUID 字符串
        /// </summary>
        public const string Datas_MapOprtCellIDStr = "{A7E2A0B4-17D0-4471-8C4D-E6B330636015}";

        /// <summary>
        /// 数据组统一操作原子的强类型 ID 对象
        /// (供图元在 RegisterOprtInfo 时注册)
        /// </summary>
        public static readonly MapOprtCellID Datas_MapOprtCellID = MapOprtCellID.Parse(Datas_MapOprtCellIDStr);
    }
}