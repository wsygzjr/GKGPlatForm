using Griffins.ImeIOT;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;

namespace Griffins.CompUI.EletronicManager.CompUI_EletronicManager.PageType.ElectricalMngPage
{
    internal class ElectricalMngPageTypeDesignCompUI : PageTypeDesignCompUIbase
    {
        protected override PageTypeID _GetPageTypeID() { return ImeIOTConst.ElectricalMngCfgPage; }

        protected override PageTypeCompUIViewInfoList _GetPageTypeCompUIViewInfoes()
        {
            return new PageTypeCompUIViewInfoList();
        }

        protected override object _GetPageTypeCompUIView(string viewID)
        {
            return null;
        }

        protected override InnerSubPageTypeInfoList _GetInnerSubPageTypeInfoes()
        {
            InnerSubPageTypeInfoList innerSubPageTypeInfoes = new InnerSubPageTypeInfoList();

            innerSubPageTypeInfoes.Add(new InnerSubPageTypeInfo
            {
                ID = ControlCardSubPageInfoDef.InnerSubPageTypeID,
                Name = ControlCardSubPageInfoDef.InnerSubPageTypeName,
            });

            innerSubPageTypeInfoes.Add(new InnerSubPageTypeInfo
            {
                ID = AxisConfigSubPageInfoDef.InnerSubPageTypeID,
                Name = AxisConfigSubPageInfoDef.InnerSubPageTypeName,
            });

            innerSubPageTypeInfoes.Add(new InnerSubPageTypeInfo
            {
                ID = IODeviceSubPageInfoDef.InnerSubPageTypeID,
                Name = IODeviceSubPageInfoDef.InnerSubPageTypeName,
            });

            innerSubPageTypeInfoes.Add(new InnerSubPageTypeInfo
            {
                ID = AnalogIODeviceSubPageInfoDef.InnerSubPageTypeID,
                Name = AnalogIODeviceSubPageInfoDef.InnerSubPageTypeName,
            });

            return innerSubPageTypeInfoes;
        }

        protected override IInnerSubPageDesignTime _CreateDesignTime(InnerSubPageTypeID innerSubPageTypeID)
        {
            if (innerSubPageTypeID == ControlCardSubPageInfoDef.InnerSubPageTypeID)
                return new ControlCardInnerSubPageDesignTime();
            if (innerSubPageTypeID == AxisConfigSubPageInfoDef.InnerSubPageTypeID)
                return new AxisConfigInnerSubPageDesignTime();
            if (innerSubPageTypeID == IODeviceSubPageInfoDef.InnerSubPageTypeID)
                return new IODeviceInnerSubPageDesignTime();
            if (innerSubPageTypeID == AnalogIODeviceSubPageInfoDef.InnerSubPageTypeID)
                return new AnalogIODeviceInnerSubPageDesignTime();

            return null;
        }
    }
}
