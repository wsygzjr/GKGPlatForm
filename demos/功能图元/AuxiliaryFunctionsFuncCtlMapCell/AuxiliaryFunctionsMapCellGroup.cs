using GF_Gereric;
using Griffins.Map.UI;

namespace GKG.Map.AuxiliaryFunctionsFuncCtlMapCell
{
    [MapCellKindCategory(AuxiliaryFunctionsMapCellGroup.AuxiliaryFunctionsMapCellGroupID_Str)]
    class AuxiliaryFunctionsMapCellGroup : GriffinsPluginMngClass, IMapCellKindCategory
    {
        internal const string AuxiliaryFunctionsMapCellGroupID_Str = "{2DD678D2-0356-4343-9313-0E26ABE68BB8}";
        #region IMapCellKindCategory 成员

        string IMapCellKindCategory.MapCellCategoryName
        {
            get { return ResourceA.AuxiliaryFunctions; }
        }

        #endregion
    }
}
