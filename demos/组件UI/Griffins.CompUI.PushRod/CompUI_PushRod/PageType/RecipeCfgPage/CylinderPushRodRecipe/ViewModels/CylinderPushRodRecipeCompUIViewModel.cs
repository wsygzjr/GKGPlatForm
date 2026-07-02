using GF_Gereric;
using GKG;
using GKG.SubMM;
using GKG.UI;
using Griffins.Map.UI;
using ReactiveUI;
using System;
using System.Reactive;

namespace Griffins.CompUI.PushRod.CompUI_PushRod.PageType.RecipeCfgPage.CylinderPushRodRecipe.ViewModels
{
    internal class CylinderPushRodRecipeCompUIViewModel : ReactiveObject
    {
        private CylinderPushRodSubMachineModulesPPCfg _data = new();
        private readonly ICompUIRunTimeCallBack _callBack;

        public ReactiveCommand<Unit, Unit> PushOnceCommand { get; }
        public ReactiveCommand<Unit, Unit> PushRodForwardCommand { get; }
        public ReactiveCommand<Unit, Unit> PushRodBackwardCommand { get; }

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
            set
            {
                this.RaiseAndSetIfChanged(ref _readOnly, value);
            }
        }

        public CylinderPushRodRecipeCompUIViewModel(ICompUIRunTimeCallBack callBack)
        {
            _callBack = callBack;
            PushOnceCommand = ReactiveCommand.Create(() => ExecuteCommand(PushRodSubMachineModulesConst.RtCmdPushOnce));
            PushRodForwardCommand = ReactiveCommand.Create(() => ExecuteCommand(PushRodSubMachineModulesConst.RtCmdPusherForward));
            PushRodBackwardCommand = ReactiveCommand.Create(() => ExecuteCommand(PushRodSubMachineModulesConst.RtCmdPusherBackward));

            ReadOnly = false;
        }

        public void SetData(CylinderPushRodSubMachineModulesPPCfg data)
        {
            _data = data ?? new CylinderPushRodSubMachineModulesPPCfg();
        }

        public CylinderPushRodSubMachineModulesPPCfg GetData()
        {
            return _data;
        }

        private void ExecuteCommand(string cmdId)
        {
            if (ReadOnly)
                return;

            var cmdParam = new GFBaseTypeParamValueList();
            TryExecRuntimeCommand(cmdId, cmdParam);
            AfterModified?.Invoke(this, EventArgs.Empty);
        }

        private void TryExecRuntimeCommand(string cmdId, GFBaseTypeParamValueList cmdParam)
        {
            try
            {
                _callBack?.ExecConfigSvrCtlCmd(cmdId, cmdParam);
            }
            catch
            {
                try
                {
                    _callBack?.ExecConfigSvrCtlCmd(cmdId, cmdParam);
                }
                catch
                {
                }
            }
        }
    }
}
