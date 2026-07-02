using System;
using System.Collections.Generic;
using System.Text;
using Griffins;
using GF_Gereric;
using Griffins.Map.UI;

namespace Griffins.Map.CtlMapCell.Generic
{
    [MapCellKindCategory(ContainerMapCellGroupID_Str)]
    class ContainerMapCellGroup : GriffinsPluginMngClass, IMapCellKindCategory
    {
        internal const string ContainerMapCellGroupID_Str = "{7BC19EFE-36E4-4EA0-97B8-454585BEB66D}";
        #region IMapCellKindCategory 成员

        string IMapCellKindCategory.MapCellCategoryName
        {
            get { return ResourceA.Container; }
        }

        #endregion
    }
}
