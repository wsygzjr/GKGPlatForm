using Avalonia.Controls;
using GF_Gereric;
using Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.DispensingTypeManage.Models;
using Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.DispensingTypeManage.ViewModels;
using Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.DispensingTypeManage.Views;
using Griffins.Map.UI;
using Griffins.PF;
using System;

namespace Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.DispensingTypeManage
{
    internal sealed class DispensingTypeManagePageTypeRunTimeCompUIView : IPageTypeRunTimeCompUIView
    {
        private readonly ICompUIRunTimeCallBack callBack;
        private event EventHandler? afterModified;

        private readonly DispensingTypeManageView view;
        private readonly DispensingTypeManageViewModel viewModel;

        private DispensingTypeManageModel? currentModel;
        private bool isViewAttached;

        public DispensingTypeManagePageTypeRunTimeCompUIView(ICompUIRunTimeCallBack callBack)
        {
            this.callBack = callBack;

            view = new DispensingTypeManageView();
            viewModel = new DispensingTypeManageViewModel();
            view.DataContext = viewModel;

            view.AttachedToVisualTree += (_, __) =>
            {
                isViewAttached = true;
                ApplyDataToViewModel(currentModel ?? new DispensingTypeManageModel());
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

        public OpMngCellID[] EditFuncMngCellIDs => throw new NotImplementedException();

        void IPageTypeRunTimeCompUIView.SetReadOnly(bool readOnly)
        {
            viewModel.ReadOnly = readOnly;
        }

        public void SetData(DispensingTypeManageModel model)
        {
            currentModel = model;

            if (!isViewAttached)
            {
                return;
            }

            ApplyDataToViewModel(model);
        }

        public DispensingTypeManageModel GetData()
        {
            return viewModel.GetData();
        }

        private void ApplyDataToViewModel(DispensingTypeManageModel model)
        {
            viewModel.SetData(model);
            view.DataContext = null;
            view.DataContext = viewModel;
            view.InvalidateMeasure();
            view.InvalidateVisual();
        }

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
