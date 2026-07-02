using Avalonia.Controls;
using GKG.SubMM;
using Griffins.CompUI.BasicRobot.CompUI_BasicRobot.PageType.InitCfgPage.MechanicalArm.ViewModels;
using Griffins.CompUI.BasicRobot.CompUI_BasicRobot.PageType.InitCfgPage.MechanicalArm.Views;
using Griffins.Map.UI;
using Griffins.PF;
using System;

namespace Griffins.CompUI.BasicRobot.CompUI_BasicRobot.PageType.InitCfgPage.MechanicalArm
{
    internal class MechanicalArmPageTypeRunTimeCompUIView : IPageTypeRunTimeCompUIView
    {
        private event EventHandler afterModified;

        private readonly MechanicalArmCompUIView view;

        private readonly MechanicalArmCompUIViewModel viewModel;

        public MechanicalArmPageTypeRunTimeCompUIView(ICompUIRunTimeCallBack callBack)
        {
            view = new MechanicalArmCompUIView();
            viewModel = new MechanicalArmCompUIViewModel(false);
            view.DataContext = viewModel;

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

        public void SetData(BasicRobotSubMachineModulesInitCfg model)
        {
            viewModel.SetData(model);
        }

        public BasicRobotSubMachineModulesInitCfg GetData()
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
