using Griffins.ImeIOT;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;

namespace Griffins.CompUI.EletronicManager.CompUI_EletronicManager.PageType.InitCfgPage
{
    internal class InitCfgPageTypeDesignCompUI : PageTypeDesignCompUIbase
    {
        protected override PageTypeID _GetPageTypeID()
        {
            return ImeIOTConst.InitCfgPage;
        }

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
            return new InnerSubPageTypeInfoList
            {
                new InnerSubPageTypeInfo
                {
                    ID = EletronicManagerInitSubPageInfoDef.InnerSubPageTypeID,
                    Name = EletronicManagerInitSubPageInfoDef.InnerSubPageTypeName,
                },
            };
        }

        protected override IInnerSubPageDesignTime _CreateDesignTime(InnerSubPageTypeID innerSubPageTypeID)
        {
            if (innerSubPageTypeID == EletronicManagerInitSubPageInfoDef.InnerSubPageTypeID)
                return new EletronicManagerInitInnerSubPageDesignTime();

            return null;
        }
    }
}
