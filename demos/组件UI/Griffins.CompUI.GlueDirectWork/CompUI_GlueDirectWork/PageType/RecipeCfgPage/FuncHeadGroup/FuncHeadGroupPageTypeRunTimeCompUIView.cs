using Avalonia.Controls;
using Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.FuncHeadGroup.Models;
using Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.FuncHeadGroup.ViewModels;
using Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.FuncHeadGroup.Views;
using Griffins.Map.UI;
using Griffins.PF;
using System;

namespace Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.FuncHeadGroup
{
    internal sealed class FuncHeadGroupPageTypeRunTimeCompUIView : IPageTypeRunTimeCompUIView
    {
        private readonly ICompUIRunTimeCallBack callBack;
        private event EventHandler? afterModified;

        private readonly FuncHeadGroupView view;
        private readonly FuncHeadGroupViewModel viewModel;

        private FuncHeadGroupModel? currentFuncModel;
        private bool isViewAttached;

        public FuncHeadGroupPageTypeRunTimeCompUIView(ICompUIRunTimeCallBack callBack)
        {
            this.callBack = callBack;

            view = new FuncHeadGroupView();
            viewModel = new FuncHeadGroupViewModel();
            view.DataContext = viewModel;

            view.AttachedToVisualTree += (_, __) =>
            {
                isViewAttached = true;
                ApplyDataToViewModel(currentFuncModel ?? new FuncHeadGroupModel());
            };

            view.DetachedFromVisualTree += (_, __) =>
            {
                isViewAttached = false;
            };

            viewModel.AfterModified += (_, __) => afterModified?.Invoke(this, EventArgs.Empty);
        }

        object IPageTypeRunTimeCompUIView.View
        {
            get
            {
                RemoveViewFromParent();
                return view;
            }
        }

        void IPageTypeRunTimeCompUIView.SetReadOnly(bool readOnly)
        {
            viewModel.ReadOnly = readOnly;
        }

        public void SetData(FuncHeadGroupModel funcModel)
        {
            // 保存外部传入的数据，视图已挂载时立即刷新界面。
            currentFuncModel = funcModel;

            if (!isViewAttached)
            {
                return;
            }

            ApplyDataToViewModel(funcModel ?? new FuncHeadGroupModel());
        }

        public FuncHeadGroupModel GetData()
        {
            // 提取界面当前编辑结果，用于保存配方。
            return viewModel.GetData();
        }

        /// <summary>将模型应用到 ViewModel 并触发界面刷新（调用方需保证在 UI 线程调用）。</summary>
        private void ApplyDataToViewModel(FuncHeadGroupModel funcModel)
        {
            viewModel.SetData(funcModel);
            view.DataContext = null;
            view.DataContext = viewModel;
            view.InvalidateMeasure();
            view.InvalidateVisual();
        }

        /// <summary>切换页面宿主时移除旧父容器引用，避免重复挂载异常。</summary>
        private void RemoveViewFromParent()
        {
            if (view.Parent is Panel panelParent)
            {
                if (panelParent.Children.Contains(view))
                {
                    panelParent.Children.Remove(view);
                }
            }
            else if (view.Parent is ContentControl contentParent)
            {
                if (contentParent.Content == view)
                {
                    contentParent.Content = null;
                }
            }
        }

        event EventHandler IPageTypeRunTimeCompUIView.AfterModified
        {
            add { afterModified += value; }
            remove { afterModified -= value; }
        }

        OpMngCellID[] IPageTypeRunTimeCompUIView.EditFuncMngCellIDs => null;
    }
}
