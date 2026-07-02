using Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.FuncHeadGroup.Models;
using Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.Views;
using Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.ViewModels;
using Griffins.ImeIOT.Map;
using Griffins.Map.UI;
using System;

namespace Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing
{
    /// <summary>
    /// 程序编辑内部子页面运行时接口实现对象
    /// </summary>
    internal sealed class RecipeEditingInnerSubPageRunTime : IInnerSubPageRunTime
    {
        private readonly RecipeEditingSubPageViewModel _recipeEditingSubPageViewModel;
        private EventHandler? afterDataModified;
        private ICompUIRunTimeCallBack? callBack;

        public RecipeEditingInnerSubPageRunTime()
        {
            _recipeEditingSubPageViewModel = new RecipeEditingSubPageViewModel();
            _recipeEditingSubPageViewModel.AfterModified += onAfterModified;
        }

        public void Init(ICompUIRunTimeCallBack callBack)
        {
            this.callBack = callBack;
            _recipeEditingSubPageViewModel.SetCallBack(callBack);
        }

        event EventHandler IInnerSubPageRunTime.AfterModified
        {
            add { afterDataModified += value; }
            remove { afterDataModified -= value; }
        }

        void IInnerSubPageRunTime.OnInit()
        {
        }

        void IInnerSubPageRunTime.SetData(byte[] data)
        {
            _recipeEditingSubPageViewModel.Init(data);
        }

        byte[] IInnerSubPageRunTime.GetData()
        {
            return _recipeEditingSubPageViewModel.CfgInfo ?? Array.Empty<byte>();
        }

        void ISubPageRunTime.Init(byte[] viewCfgInfo)
        {
        }

        object ISubPageRunTime.View
        {
            get
            {
                var recipeEditingSubPageView = new RecipeEditingSubPageView();
                recipeEditingSubPageView.DataContext = _recipeEditingSubPageViewModel;
                _recipeEditingSubPageViewModel.SetViewReference(recipeEditingSubPageView);
                return recipeEditingSubPageView;
            }
        }

        void ISubPageRunTime.SetViewCfgInfo(byte[] viewCfgInfo)
        {
        }

        private void onAfterModified(object? sender, EventArgs e)
        {
            afterDataModified?.Invoke(sender, e);
        }

        /// <summary>将功能头组数据同步到「方案配置」等区域（与程序编辑共用配方页数据）。</summary>
        internal void ApplyFuncHeadGroup(FuncHeadGroupModel? model)
        {
            _recipeEditingSubPageViewModel.RecipePlanManageCfgViewModel.ApplyFuncHeadGroup(model);
        }

        public bool CheckDataValid(out string[] inValidMsg)
        {
            inValidMsg = null;
            return true;
        }
    }
}
