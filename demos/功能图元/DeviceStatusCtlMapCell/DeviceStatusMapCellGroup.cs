using GF_Gereric;
using Griffins.Map;
using Griffins.Map.UI;

namespace GKG.Map.DeviceStatusFuncCtlMapCell
{
    /// <summary>
    /// 设备状态图元分组
    /// </summary>
    [MapCellKindCategory(DeviceStatusMapCellGroup.DeviceStatusMapCellGroupID_Str)]
    class DeviceStatusMapCellGroup : GriffinsPluginMngClass, IMapCellKindCategory
    {
        /// <summary>
        /// 图元分组ID
        /// </summary>
        internal const string DeviceStatusMapCellGroupID_Str = "{C7870C5B-8839-4467-84C9-96831B73F945}";

        #region IMapCellKindCategory 成员
        string IMapCellKindCategory.MapCellCategoryName
        {
            get { return "设备状态"; }
        }

        #endregion
    }
}
