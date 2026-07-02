using Avalonia.Controls;
using GKG.SubMM.Dispenser;
using Griffins.CompUI.WeighingBalance.CompUI_WeighingBalance.PageType.FactoryCfgPage.WeighingBalanceFactory.ViewModels;
using Griffins.CompUI.WeighingBalance.CompUI_WeighingBalance.PageType.FactoryCfgPage.WeighingBalanceFactory.Views;
using Griffins.Map.UI;
using Griffins.PF;
using System;

namespace Griffins.CompUI.WeighingBalance.CompUI_WeighingBalance.PageType.FactoryCfgPage.WeighingBalanceFactory
{
    /// <summary>
    /// 称重出厂配置运行态视图。
    /// </summary>
    internal class WeighingBalanceFactoryPageTypeRunTimeCompUIView : IPageTypeRunTimeCompUIView
    {
        private readonly WeighingBalanceFactoryCompUIView view;
        private readonly WeighingBalanceFactoryCompUIViewModel viewModel;
        private WeighingBalanceSubMachineModulesFactoryCfg data;
        private event EventHandler afterModified;

        public WeighingBalanceFactoryPageTypeRunTimeCompUIView()
        {
            view = new WeighingBalanceFactoryCompUIView();
            viewModel = new WeighingBalanceFactoryCompUIViewModel();
            view.DataContext = viewModel;
            viewModel.AfterModified += (_, __) => afterModified?.Invoke(this, EventArgs.Empty);
        }

        public object View
        {
            get
            {
                RemoveViewFromParent();
                return view;
            }
        }

        public OpMngCellID[] EditFuncMngCellIDs => null;

        public event EventHandler AfterModified
        {
            add => afterModified += value;
            remove => afterModified -= value;
        }

        public void SetReadOnly(bool readOnly)
        {
            viewModel.ReadOnly = readOnly;
        }

        public void SetData(WeighingBalanceSubMachineModulesFactoryCfg data)
        {
            this.data = data ?? new WeighingBalanceSubMachineModulesFactoryCfg();
            viewModel.SetData(this.data);
        }

        public WeighingBalanceSubMachineModulesFactoryCfg GetData()
        {
            data = viewModel.GetData();
            return data;
        }

        private void RemoveViewFromParent()
        {
            if (view.Parent is Panel panelParent && panelParent.Children.Contains(view))
            {
                panelParent.Children.Remove(view);
            }
            else if (view.Parent is ContentControl contentParent && Equals(contentParent.Content, view))
            {
                contentParent.Content = null;
            }
        }
    }
}
