using Avalonia.Controls;
using Avalonia.Threading;
using GKG.SubMM.Dispenser;
using Griffins.CompUI.WeighingBalance.CompUI_WeighingBalance.PageType.InitCfgPage.WeighingBalance.ViewModels;
using Griffins.CompUI.WeighingBalance.CompUI_WeighingBalance.PageType.InitCfgPage.WeighingBalance.Views;
using Griffins.Map.UI;
using Griffins.PF;
using System;

namespace Griffins.CompUI.WeighingBalance.CompUI_WeighingBalance.PageType.InitCfgPage.WeighingBalance
{
    internal class WeighingBalancePageTypeRunTimeCompUIView : IPageTypeRunTimeCompUIView
    {
        private ICompUIRunTimeCallBack callBack;
        private event EventHandler afterModified;

        private readonly WeighingBalanceInitCfgCompUIView view;

        private readonly WeighingBalanceInitCfgCompUIViewModel viewModel;

        private WeighingBalanceSubMachineModulesInitCfg currentModel;

        private bool isViewAttached;

        public WeighingBalancePageTypeRunTimeCompUIView(ICompUIRunTimeCallBack callBack)
        {
            this.callBack = callBack;
            view = new WeighingBalanceInitCfgCompUIView();
            viewModel = new WeighingBalanceInitCfgCompUIViewModel(false,this.callBack);
            view.DataContext = viewModel;

            view.AttachedToVisualTree += (_, __) =>
            {
                isViewAttached = true;
                ApplyDataToViewModel(currentModel ?? new WeighingBalanceSubMachineModulesInitCfg());
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

        public void SetData(WeighingBalanceSubMachineModulesInitCfg model)
        {
            currentModel = model;

            if (!isViewAttached)
            {
                return;
            }

            ApplyDataToViewModel(model);
        }

        private void ApplyDataToViewModel(WeighingBalanceSubMachineModulesInitCfg model)
        {
            void apply()
            {
                // 仅更新 ViewModel。不要清空/重设 DataContext，也不要对整个视图 InvalidateMeasure：
                // 否则会与 Avalonia DataGrid 的布局队列叠加，反复排队触发「Layout cycle detected」警告。
                viewModel.SetData(model);
            }

            if (Dispatcher.UIThread.CheckAccess())
            {
                apply();
                return;
            }

            Dispatcher.UIThread.InvokeAsync(apply, DispatcherPriority.Normal).GetAwaiter().GetResult();
        }

        public WeighingBalanceSubMachineModulesInitCfg GetData()
        {
            return viewModel.GetData();
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

        OpMngCellID[] IPageTypeRunTimeCompUIView.EditFuncMngCellIDs
        {
            get { return null; }
        }
    }
}
