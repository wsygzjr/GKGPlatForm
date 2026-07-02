using GF_Gereric;
using Griffins.Map.UI;

namespace GKG.Map.DispensingViewFuncCtlMapCell
{
    [MapCellKindCategory(DispensingViewMapCellGroup.DispensingViewMapCellGroupID_Str)]
    internal class DispensingViewMapCellGroup : GriffinsPluginMngClass, IMapCellKindCategory
    {
        internal const string DispensingViewMapCellGroupID_Str = "{B39C3D9E-6D9D-4E21-8D5E-7D7CBA6D55C1}";

        string IMapCellKindCategory.MapCellCategoryName => ResourceA.DispensingView;
    }
}
