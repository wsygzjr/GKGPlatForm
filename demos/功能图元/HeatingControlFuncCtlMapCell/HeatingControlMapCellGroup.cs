using GF_Gereric;
using Griffins.Map.UI;

namespace GKG.Map.HeatingControlFuncCtlMapCell
{
    /// <summary>
    /// 加热控制功能图元组类别定义类
    /// 定义加热控制图元的组别和基本信息
    /// </summary>
    [MapCellKindCategory(HeatingControlMapCellGroup.HeatingControlMapCellGroupID_Str)]
    class HeatingControlMapCellGroup : GriffinsPluginMngClass, IMapCellKindCategory
    {
        /// <summary>
        /// 加热控制图元组ID常量
        /// </summary>
        internal const string HeatingControlMapCellGroupID_Str = "{6A5F1B6C-6D2C-4DDE-9C88-24D54B569D92}";

        /// <summary>
        /// 获取图元类别名称
        /// </summary>
        string IMapCellKindCategory.MapCellCategoryName
        {
            get { return ResourceA.HeatingControl; }
        }
    }
}
