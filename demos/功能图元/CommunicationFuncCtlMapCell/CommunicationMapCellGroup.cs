using GF_Gereric;
using Griffins.Map.UI;

namespace GKG.Map.CommunicationFuncCtlMapCell
{
    [MapCellKindCategory(CommunicationMapCellGroup.CommunicationMapCellGroupID_Str)]
    class CommunicationMapCellGroup : GriffinsPluginMngClass, IMapCellKindCategory
    {
        internal const string CommunicationMapCellGroupID_Str = "{F1EB36E1-354D-4B87-B630-5261CFA46F3A}";

        string IMapCellKindCategory.MapCellCategoryName
        {
            get { return ResourceA.Communication; }
        }
    }
}
