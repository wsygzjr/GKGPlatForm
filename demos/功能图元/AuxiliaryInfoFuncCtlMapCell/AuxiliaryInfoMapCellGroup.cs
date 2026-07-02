using GF_Gereric;
using Griffins.Map.UI;

namespace GKG.Map.AuxiliaryInfoFuncCtlMapCell
{
    [MapCellKindCategory(AuxiliaryInfoMapCellGroup.AuxiliaryInfoMapCellGroupID_Str)]
    class AuxiliaryInfoMapCellGroup : GriffinsPluginMngClass, IMapCellKindCategory
    {
        internal const string AuxiliaryInfoMapCellGroupID_Str = "{3B4FBB2E-4A9B-4B63-BB7C-6AAEC08C8FD9}";

        string IMapCellKindCategory.MapCellCategoryName
        {
            get { return ResourceA.AuxiliaryInfo; }
        }
    }
}
