using GF_Gereric;
using Griffins.Map.UI;

namespace GKG.Map.LoadUnloadFuncCtlMapCell
{
    /// <summary>
    /// 上下料图元分组类别定义类
    /// 负责在 Griffins 图元工具箱中定义“上下料”分类的显示名称与唯一标识。
    /// </summary>
    [MapCellKindCategory(LoadUnloadMapCellGroup.LoadUnloadMapCellGroupID_Str)]
    internal class LoadUnloadMapCellGroup : GriffinsPluginMngClass, IMapCellKindCategory
    {
        /// <summary>
        /// 上下料图元分组的全局唯一标识符 (GUID)
        /// </summary>
        internal const string LoadUnloadMapCellGroupID_Str = "{8F95A80C-BD44-4BB8-9B68-BDE755C13D4B}";

        #region IMapCellKindCategory 接口实现

        /// <summary>
        /// 获取图元在工具箱中显示的分类名称 (支持多语言资源映射)
        /// </summary>
        string IMapCellKindCategory.MapCellCategoryName => ResourceA.LoadUnload;

        #endregion
    }
}