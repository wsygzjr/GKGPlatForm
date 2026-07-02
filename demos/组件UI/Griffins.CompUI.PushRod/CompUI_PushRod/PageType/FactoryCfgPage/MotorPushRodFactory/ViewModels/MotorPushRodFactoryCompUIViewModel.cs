using GKG.SubMM;
using GKG.UI;
using Griffins.Map.UI;
using ReactiveUI;
using System;

namespace Griffins.CompUI.PushRod.CompUI_PushRod.PageType.FactoryCfgPage.MotorPushRodFactory.ViewModels
{
    internal class MotorPushRodFactoryCompUIViewModel : ReactiveObject
    {
        public event EventHandler AfterModified;

        private object _viewTag;
        public object ViewTag
        {
            get => _viewTag;
            set => this.RaiseAndSetIfChanged(ref _viewTag, value);
        }

        private bool _readOnly;
        public bool ReadOnly
        {
            get => _readOnly;
            set => this.RaiseAndSetIfChanged(ref _readOnly, value);
        }

        public MotorPushRodFactoryCompUIViewModel(ICompUIRunTimeCallBack callBack)
        {
            ReadOnly = false;
        }

        public void SetData(MotorPushRodSubMachineModulesFactoryCfg data)
        {
        }

        public MotorPushRodSubMachineModulesFactoryCfg GetData()
        {
            return new MotorPushRodSubMachineModulesFactoryCfg();
        }
    }
}
