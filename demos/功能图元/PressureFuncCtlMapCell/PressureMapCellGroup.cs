using GF_Gereric;
using Griffins.Map.UI;

namespace Griffins.Map.PressureFuncCtlMapCell
{
    [MapCellKindCategory(PressureMapCellGroup.PressureMapCellGroupID_Str)]
    class PressureMapCellGroup : GriffinsPluginMngClass, IMapCellKindCategory
    {
        internal const string PressureMapCellGroupID_Str = "{A8693B18-0917-495A-B647-4844510B4F5F}";
        #region IMapCellKindCategory 成员

        string IMapCellKindCategory.MapCellCategoryName
        {
            get { return ResourceA.Pressure; }
        }

        #endregion
    }
}
