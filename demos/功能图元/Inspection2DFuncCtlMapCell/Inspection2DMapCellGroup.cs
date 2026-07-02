using GF_Gereric;
using Griffins.Map.UI;


namespace GKG.Map.Inspection2DFuncCtlMapCell
{
    [MapCellKindCategory(Inspection2DMapCellGroup.Inspection2DMapCellGroupID_Str)]
    internal class Inspection2DMapCellGroup : GriffinsPluginMngClass, IMapCellKindCategory
    {
        internal const string Inspection2DMapCellGroupID_Str = "{7E5E3CBB-4AC1-5F09-97EB-4C967F9CB1F8}";

        #region IMapCellKindCategory 成员
        
        string IMapCellKindCategory.MapCellCategoryName => ResourceA.Inspection2D;

        #endregion
    }
}
