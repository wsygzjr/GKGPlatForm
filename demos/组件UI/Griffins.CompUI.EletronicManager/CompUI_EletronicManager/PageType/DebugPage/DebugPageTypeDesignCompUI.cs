using Griffins.ImeIOT;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;

namespace Griffins.CompUI.EletronicManager.CompUI_EletronicManager.PageType.DebugPage
{
    internal class DebugPageTypeDesignCompUI : PageTypeDesignCompUIbase
    {
        protected override PageTypeID _GetPageTypeID() { return ImeIOTConst.DebugPage; }

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

            InnerSubPageTypeInfo axisDebugInnerSubPageTypeInfo = new InnerSubPageTypeInfo();
            axisDebugInnerSubPageTypeInfo.ID = AxisDebugSubPageInfoDef.AxisDebugInnerSubPageTypeID;
            axisDebugInnerSubPageTypeInfo.Name = AxisDebugSubPageInfoDef.AxisDebugInnerSubPageTypeName;
            innerSubPageTypeInfoes.Add(axisDebugInnerSubPageTypeInfo);

            InnerSubPageTypeInfo ioInInnerSubPageTypeInfo = new InnerSubPageTypeInfo();
            ioInInnerSubPageTypeInfo.ID = AxisDebugSubPageInfoDef.IOInInnerSubPageTypeID;
            ioInInnerSubPageTypeInfo.Name = AxisDebugSubPageInfoDef.IOInInnerSubPageTypeName;
            innerSubPageTypeInfoes.Add(ioInInnerSubPageTypeInfo);

            return innerSubPageTypeInfoes;
        }

        protected override IInnerSubPageDesignTime _CreateDesignTime(InnerSubPageTypeID innerSubPageTypeID)
        {
            if (innerSubPageTypeID == AxisDebugSubPageInfoDef.AxisDebugInnerSubPageTypeID ||
                innerSubPageTypeID == AxisDebugSubPageInfoDef.IOInInnerSubPageTypeID)
                return new AxisDebugInnerSubPageDesignTime();

            return null;
        }
    }
}
