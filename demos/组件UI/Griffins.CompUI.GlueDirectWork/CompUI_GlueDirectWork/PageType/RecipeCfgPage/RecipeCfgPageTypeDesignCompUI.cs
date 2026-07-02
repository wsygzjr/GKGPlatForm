using Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.DispensingTypeManage.Views;
using Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.FuncHeadGroup.Views;
using Griffins.ImeIOT;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;

namespace Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage
{
    internal class RecipeCfgPageTypeDesignCompUI : PageTypeDesignCompUIbase
    {
        protected override PageTypeID _GetPageTypeID() { return ImeIOTConst.PPCfgPage; }

        protected override PageTypeCompUIViewInfoList _GetPageTypeCompUIViewInfoes()
        {
            return new PageTypeCompUIViewInfoList()
            {
                new PageTypeCompUIViewInfo() { ViewID = RecipeCfgPageTypeConst.ViewID_DispensingTypeManage, ViewName = "点胶类型管理" },
                new PageTypeCompUIViewInfo() { ViewID = RecipeCfgPageTypeConst.ViewID_FuncHeadGroup, ViewName = "默认功能头组" },
            };
        }

        protected override object _GetPageTypeCompUIView(string viewID)
        {
            return viewID switch
            {
                RecipeCfgPageTypeConst.ViewID_DispensingTypeManage => new DispensingTypeManageView(),
                RecipeCfgPageTypeConst.ViewID_FuncHeadGroup => new FuncHeadGroupView(),
                _ => null,
            };
        }

        protected override InnerSubPageTypeInfoList _GetInnerSubPageTypeInfoes()
        {
            InnerSubPageTypeInfoList innerSubPageTypeInfoes = new InnerSubPageTypeInfoList();

            InnerSubPageTypeInfo recipeEditingInnerSubPageTypeInfo = new InnerSubPageTypeInfo();
            recipeEditingInnerSubPageTypeInfo.ID = new InnerSubPageTypeID(RecipeCfgPageTypeConst.InnerSubPageTypeIDStr_RecipeEditing);
            recipeEditingInnerSubPageTypeInfo.Name = RecipeCfgPageTypeConst.InnerSubPageTypeName_RecipeEditing;
            innerSubPageTypeInfoes.Add(recipeEditingInnerSubPageTypeInfo);

            return innerSubPageTypeInfoes;
        }
    }
}
