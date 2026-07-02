using GF_Gereric;
using Griffins.Map.UI;

namespace GKG.Map.ProductionInformationFuncCtlMapCell
{
    /// <summary>
    /// 生产信息图元分组类别定义类
    /// 负责在 Griffins 图元分组中定义“生产信息”图元类别分组的显示名称与唯一标识。
    /// </summary>
    [MapCellKindCategory(ProductionInformationMapCellGroup.ProductionInformationMapCellGroupID_Str)]
    internal class ProductionInformationMapCellGroup : GriffinsPluginMngClass, IMapCellKindCategory
    {
        /// <summary>
        /// 生产信息图元分组的全局唯一标识符 (GUID)
        /// </summary>
        internal const string ProductionInformationMapCellGroupID_Str = "{A8693B18-0917-495A-B647-4844510B4F51}";

        #region IMapCellKindCategory 接口实现

        /// <summary>
        /// 获取图元在组态工具栏分组中显示的分类名称 (支持多语言资源映射)
        /// </summary>
        string IMapCellKindCategory.MapCellCategoryName => ResourceA.ProductionInformation;

        #endregion
    }
}