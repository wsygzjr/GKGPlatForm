using GF_Gereric;
using Griffins.Map.UI;


namespace GKG.Map.StatusInfoFuncCtlMapCell
{
    /// <summary>
    /// 状态信息图元分组
    /// </summary>
    [MapCellKindCategory(StatusInfoMapCellGroup.StatusInfoMapCellGroupID_Str)]
    class StatusInfoMapCellGroup : GriffinsPluginMngClass, IMapCellKindCategory
    {
        /// <summary>
        /// 图元分组ID
        /// </summary>
        internal const string StatusInfoMapCellGroupID_Str = "{6D4D2BAA-39B0-4F98-86DA-4A5F84273F92}";

        #region IMapCellKindCategory 成员
        /// <summary>
        /// 获取图元类别名称
        /// </summary>
        string IMapCellKindCategory.MapCellCategoryName
        {
            get { return ResourceA.StatusInfo; }
        }

        #endregion
    }
}
