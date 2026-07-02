
using GF_Gereric;
using Griffins.Map.UI;

namespace GKG.Map.LogFuncCtlMapCell
{
    [MapCellKindCategory(LogMapCellGroup.LogMapCellGroupID_Str)]
    class LogMapCellGroup : GriffinsPluginMngClass, IMapCellKindCategory
    {
        internal const string LogMapCellGroupID_Str = "{C13A8C14-02B4-4E37-98D0-4D7C1DBE6F5A}";

        string IMapCellKindCategory.MapCellCategoryName
        {
            get { return ResourceA.Log; }
        }
    }
}
