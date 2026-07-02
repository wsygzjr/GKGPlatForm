using GF_Gereric;
using Griffins.Map;
using Griffins.Map.UI;

namespace GKG.Map.DataMonitorFuncCtlMapCell
{
    [MapCellKindCategory(DataMonitorMapCellGroup.DataMonitorMapCellGroupID_Str)]
    public class DataMonitorMapCellGroup : GriffinsPluginMngClass, IMapCellKindCategory
    {
        public const string DataMonitorMapCellGroupID_Str = "{8DD5AD15-0F58-4047-A6C1-99C323D4AB9E}";

        string IMapCellKindCategory.MapCellCategoryName
        {
            get { return ResourceA.DataMonitor; }
        }
    }
}
