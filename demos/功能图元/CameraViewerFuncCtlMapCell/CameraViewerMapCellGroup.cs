using GF_Gereric;
using Griffins.Map.UI;

namespace GKG.Map.CameraViewerFuncCtlMapCell
{
    [MapCellKindCategory(CameraViewerMapCellGroup.CameraViewerMapCellGroupID_Str)]
    class CameraViewerMapCellGroup : GriffinsPluginMngClass, IMapCellKindCategory
    {
        internal const string CameraViewerMapCellGroupID_Str = "{5FC7F3DC-CEC6-4B79-8715-135D9C00DC00}";
        #region IMapCellKindCategory 成员

        string IMapCellKindCategory.MapCellCategoryName
        {
            get { return ResourceA.CameraViewer; }
        }

        #endregion
    }
}
