using Griffins.ImeIOT;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;

namespace Griffins.CompUI.Calibration.PageType.InitCfgPage
{
    internal class InitCfgPageTypeDesignCompUI : PageTypeDesignCompUIbase
    {
        protected override PageTypeID _GetPageTypeID() { return ImeIOTConst.InitCfgPage; }

        protected override PageTypeCompUIViewInfoList _GetPageTypeCompUIViewInfoes()
        {
            return new PageTypeCompUIViewInfoList()
            {
                
            };
        }

        protected override object _GetPageTypeCompUIView(string viewID)
        {
            return viewID switch
            {
                _ => null,
            };
        }

        protected override InnerSubPageTypeInfoList _GetInnerSubPageTypeInfoes()
        {
            InnerSubPageTypeInfoList innerSubPageTypeInfoes = new InnerSubPageTypeInfoList();

            InnerSubPageTypeInfo calibrationInnerSubPageTypeInfo = new InnerSubPageTypeInfo();
            calibrationInnerSubPageTypeInfo.ID = new InnerSubPageTypeID(InitCfgPageTypeConst.InnerSubPageTypeIDStr_Calibration);
            calibrationInnerSubPageTypeInfo.Name = InitCfgPageTypeConst.InnerSubPageTypeName_Calibration;
            innerSubPageTypeInfoes.Add(calibrationInnerSubPageTypeInfo);

            return innerSubPageTypeInfoes;
        }
    }
}
