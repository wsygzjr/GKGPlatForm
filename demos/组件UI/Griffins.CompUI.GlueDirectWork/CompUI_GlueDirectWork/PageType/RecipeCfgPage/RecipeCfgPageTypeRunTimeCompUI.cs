using GF_Gereric;
using Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.DispensingTypeManage;
using Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.DispensingTypeManage.Models;
using Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.FuncHeadGroup;
using Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.FuncHeadGroup.Models;
using Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing;
using Griffins.ImeIOT;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;
using System;

namespace Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage
{
    internal class RecipeCfgPageTypeRunTimeCompUI : PageTypeRunTimeCompUIBase
    {
        private RecipeCfgPageTypeData _recipeCfgPageTypeData;

        private IPageTypeRunTimeCompUIView dispensingTypeManageView;

        private DispensingTypeManageModel dispensingTypeManageData;

        private IPageTypeRunTimeCompUIView funcHeadGroupView;

        private FuncHeadGroupModel funcHeadGroupData;

        /// <summary>
        /// 程序编辑内部子页面运行时接口实现对象
        /// </summary>
        private RecipeEditingInnerSubPageRunTime recipeEditingInnerSubPageRunTime = new();

        protected override void _OnInit()
        {
            _recipeCfgPageTypeData = new RecipeCfgPageTypeData();

            dispensingTypeManageData = new DispensingTypeManageModel();

            funcHeadGroupData = new FuncHeadGroupModel();

            //创建内部子页面实例
            recipeEditingInnerSubPageRunTime.Init(base.CallBack);
            (recipeEditingInnerSubPageRunTime as IInnerSubPageRunTime).AfterModified += OnAfterModified;
            recipeEditingInnerSubPageRunTime.ApplyFuncHeadGroup(funcHeadGroupData);
        }

        protected override PageTypeID _GetPageTypeID() { return ImeIOTConst.PPCfgPage; }

        protected override IPageTypeRunTimeCompUIView _GetPageTypeCompUIView(string viewID)
        {
            if (viewID == RecipeCfgPageTypeConst.ViewID_DispensingTypeManage)
            {
                if (dispensingTypeManageView != null)
                {
                    dispensingTypeManageView.AfterModified -= OnAfterModified;
                }

                dispensingTypeManageView = new DispensingTypeManagePageTypeRunTimeCompUIView(this.CallBack);
                dispensingTypeManageView.AfterModified += OnAfterModified;
                (dispensingTypeManageView as DispensingTypeManagePageTypeRunTimeCompUIView)?.SetData(dispensingTypeManageData ?? new DispensingTypeManageModel());
                return dispensingTypeManageView;
            }

            if (viewID == RecipeCfgPageTypeConst.ViewID_FuncHeadGroup)
            {
                if (funcHeadGroupView != null)
                {
                    if (funcHeadGroupView is FuncHeadGroupPageTypeRunTimeCompUIView oldView)
                    {
                        funcHeadGroupData = oldView.GetData() ?? new FuncHeadGroupModel();
                        recipeEditingInnerSubPageRunTime.ApplyFuncHeadGroup(funcHeadGroupData);
                    }
                    funcHeadGroupView.AfterModified -= OnAfterModified;
                }

                funcHeadGroupView = new FuncHeadGroupPageTypeRunTimeCompUIView(this.CallBack);
                funcHeadGroupView.AfterModified += OnAfterModified;
                (funcHeadGroupView as FuncHeadGroupPageTypeRunTimeCompUIView)?.SetData(
                    funcHeadGroupData ?? new FuncHeadGroupModel());
                return funcHeadGroupView;
            }

            return null;
        }

        protected override void _SetData(byte[] data)
        {
            if (data == null)
            {
                return;
            }

            _recipeCfgPageTypeData = JsonObjConvert.FromJSonBytes<RecipeCfgPageTypeData>(data);
            (recipeEditingInnerSubPageRunTime as IInnerSubPageRunTime).SetData(_recipeCfgPageTypeData.RecipeEditingInnerSubPageCfgs);

            dispensingTypeManageData = JsonObjConvert.FromJSonBytes<DispensingTypeManageModel>(_recipeCfgPageTypeData.DispensingTypeManageCfgs) ?? new DispensingTypeManageModel();
            if (dispensingTypeManageView is DispensingTypeManagePageTypeRunTimeCompUIView view)
            {
                view.SetData(dispensingTypeManageData);
            }

            funcHeadGroupData = JsonObjConvert.FromJSonBytes<FuncHeadGroupModel>(_recipeCfgPageTypeData.FuncHeadGroupCfgs) ?? new FuncHeadGroupModel();
            if (funcHeadGroupView is FuncHeadGroupPageTypeRunTimeCompUIView fgView)
            {
                fgView.SetData(funcHeadGroupData);
            }

            recipeEditingInnerSubPageRunTime.ApplyFuncHeadGroup(funcHeadGroupData);
        }

        protected override byte[] _GetData()
        {
            _recipeCfgPageTypeData.RecipeEditingInnerSubPageCfgs = (recipeEditingInnerSubPageRunTime as IInnerSubPageRunTime).GetData();

            if (dispensingTypeManageView is DispensingTypeManagePageTypeRunTimeCompUIView view)
            {
                dispensingTypeManageData = view.GetData();
            }
            _recipeCfgPageTypeData.DispensingTypeManageCfgs = JsonObjConvert.ToJSonBytes(dispensingTypeManageData ?? new DispensingTypeManageModel());

            if (funcHeadGroupView is FuncHeadGroupPageTypeRunTimeCompUIView fgView)
            {
                funcHeadGroupData = fgView.GetData();
            }
            _recipeCfgPageTypeData.FuncHeadGroupCfgs = JsonObjConvert.ToJSonBytes(funcHeadGroupData ?? new FuncHeadGroupModel());

            return JsonObjConvert.ToJSonBytes(_recipeCfgPageTypeData);
        }

        protected override string _ExecViewCmd(string cmdID, string cmdParam)
        {
            return string.Empty;
        }

        private void OnAfterModified(object sender, EventArgs e)
        {
            if (dispensingTypeManageView is DispensingTypeManagePageTypeRunTimeCompUIView dView)
            {
                dispensingTypeManageData = dView.GetData();
            }

            if (funcHeadGroupView is FuncHeadGroupPageTypeRunTimeCompUIView fView)
            {
                funcHeadGroupData = fView.GetData();
            }

            recipeEditingInnerSubPageRunTime.ApplyFuncHeadGroup(funcHeadGroupData);

            AfterDataModified?.Invoke(sender, e);
        }

        protected override ISubPageRunTime _GetSubPageRunTime(SubPageKindInfoBase subPageKindInfo)
        {
            InnerSubPageKindInfo innerSubPageKindInfo = (InnerSubPageKindInfo)subPageKindInfo;
            switch (innerSubPageKindInfo.InnerSubPageTypeID.ToString())
            {
                //配方编辑内部子页面
                case RecipeCfgPageTypeConst.InnerSubPageTypeIDStr_RecipeEditing:
                    return recipeEditingInnerSubPageRunTime;

                default:
                    throw new Exception("不存在对应内部子页面类型ID的内部子页面信息");
            }
        }

        protected override bool _CheckDataValid(out string[] inValidMsg)
        {
            inValidMsg = null;
            return true;
        }

        public class RecipeCfgPageTypeData
        {
            /// <summary>
            /// 配方编辑内部子页面配置信息
            /// </summary>
            public byte[] RecipeEditingInnerSubPageCfgs { get; set; } = Array.Empty<byte>();

            public byte[] DispensingTypeManageCfgs { get; set; } = Array.Empty<byte>();

            public byte[] FuncHeadGroupCfgs { get; set; } = Array.Empty<byte>();
        }
    }
}
