using GF_Gereric;
using Griffins.Map.UI;

namespace GKG.Map.ChartFuncCtlMapCell;

[MapCellKindCategory(ChartMapCellGroup.ChartMapCellGroupID_Str)]
class ChartMapCellGroup : GriffinsPluginMngClass, IMapCellKindCategory
{
    internal const string ChartMapCellGroupID_Str = "{01DC8866-AB13-460E-88F0-B49FE957E5B6}";

    #region IMapCellKindCategory 成员

    string IMapCellKindCategory.MapCellCategoryName
    {
        get { return ResourceA.Chart; }
    }

    #endregion
}

