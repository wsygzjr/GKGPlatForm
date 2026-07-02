using GF_Gereric;
using Griffins.Map.UI;

namespace GKG.Map.StationFuncCtlMapCell
{
    /// <summary>
    /// 工位图元分组类别定义类
    /// 负责在图元工具箱中定义“工位”分类的显示名称与唯一标识。
    /// </summary>
    [MapCellKindCategory(StationMapCellGroup.StationMapCellGroupID_Str)]
    class StationMapCellGroup : GriffinsPluginMngClass, IMapCellKindCategory
    {
        /// <summary>
        /// 工位图元分组的全局唯一标识符
        /// </summary>
        internal const string StationMapCellGroupID_Str = "{E7A49A2A-8D2A-4D9A-8C62-2D8E9E78989A}";


        #region IMapCellKindCategory 成员

        /// <summary>
        /// 获取图元在工具箱中显示的分类名称
        /// </summary>
        string IMapCellKindCategory.MapCellCategoryName => ResourceA.Station;

        #endregion
    }
}
