using System;
using System.Collections.Generic;
using System.Text;
using Griffins;
using GF_Gereric;

namespace GKG.Map.MapCell.Generic
{
    [MapCellKindCategory(GenericMapCellGroup.GenericMapCellGroupID_Str)]
    class GenericMapCellGroup : GriffinsPluginMngClass, IMapCellKindCategory
    {
        internal const string GenericMapCellGroupID_Str = "{F7A7680F-BB34-4475-9A32-57D7610C728C}";
        #region IMapCellKindCategory 成员

        string IMapCellKindCategory.MapCellCategoryName
        {
            get { return ResourceA.Generic; }
        }

        #endregion
    }
}
