using Avalonia.Controls;
using GKG.UI;
using Griffins.Map.UI;
using ReactiveUI;
using System;

namespace Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.ViewModels
{
    internal sealed class RecipeEditingSubPageViewModel : ReactiveObject
    {
        private Control? _viewReference;

        private ICompUIRunTimeCallBack? _callBack;

        private byte[]? _cfgInfo;

        public event EventHandler? AfterModified;

        public GlueDispensingStyleCfgViewModel GlueDispensingStyleCfgViewModel { get; }

        public PreDispensingViewModel PreDispensingViewModel { get; }

        public ProgramEditingViewModel ProgramEditingViewModel { get; }

        public RecipePlanManageCfgViewModel RecipePlanManageCfgViewModel { get; }

        public RecipeEditingSubPageViewModel()
        {
            GlueDispensingStyleCfgViewModel = new GlueDispensingStyleCfgViewModel();
            PreDispensingViewModel = new PreDispensingViewModel();
            ProgramEditingViewModel = new ProgramEditingViewModel();
            RecipePlanManageCfgViewModel = new RecipePlanManageCfgViewModel(
                ProgramEditingViewModel.AreaEditingWorkspaceViewModel,
                ProgramEditingViewModel.Templates);
        }

        public RecipeEditingSubPageViewModel(ICompUIRunTimeCallBack? callBack)
        {
            _callBack = callBack;
            GlueDispensingStyleCfgViewModel = new GlueDispensingStyleCfgViewModel();
            PreDispensingViewModel = new PreDispensingViewModel();
            ProgramEditingViewModel = new ProgramEditingViewModel();
            RecipePlanManageCfgViewModel = new RecipePlanManageCfgViewModel(
                ProgramEditingViewModel.AreaEditingWorkspaceViewModel,
                ProgramEditingViewModel.Templates);
            PreDispensingViewModel.SetCallBack(callBack);
        }

        public void SetCallBack(ICompUIRunTimeCallBack? callBack)
        {
            _callBack = callBack;
            PreDispensingViewModel.SetCallBack(callBack);
        }

        public void SetViewReference(Control view)
        {
            _viewReference = view;
        }

        public void Init(byte[]? cfgInfo)
        {
            _cfgInfo = cfgInfo;
            AfterModified?.Invoke(this, EventArgs.Empty);
        }

        public byte[]? CfgInfo
        {
            get => _cfgInfo;
            set
            {
                if (!ReferenceEquals(_cfgInfo, value))
                {
                    _cfgInfo = value;
                    AfterModified?.Invoke(this, EventArgs.Empty);
                }
            }
        }
    }
}
