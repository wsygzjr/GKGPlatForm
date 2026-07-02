using Avalonia.Threading;
using GF_Gereric;
using GKG;
using GKG.SubMM;
using GKG.UI;
using Griffins.ImeIOT;
using Griffins.Map.UI;
using Griffins.PF;
using Griffins.PF.RichClient;
using ReactiveUI;
using System;
using System.Globalization;
using System.Reactive;

namespace Griffins.CompUI.PushRod.CompUI_PushRod.PageType.RecipeCfgPage.MotorPushRodRecipe.ViewModels
{
    internal class MotorPushRodRecipeCompUIViewModel : ReactiveObject
    {
        private const string PushRodAxisInformType = "PushRod";

        private MotorPushRodSubMachineModulesPPCfg _data = new();
        private double _currentPosition;
        private readonly ICompUIRunTimeCallBack _callBack;

        public TextInputViewModel CurrentValueViewModel { get; }
        public TextInputViewModel PushDistanceViewModel { get; }
        public TextInputViewModel PushAxisSpeedViewModel { get; }
        public TextInputViewModel PushAxisAccelerationViewModel { get; }
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
                var enabled = !_readOnly;
                PushDistanceViewModel.IsEnabled = enabled;
                PushAxisSpeedViewModel.IsEnabled = enabled;
                PushAxisAccelerationViewModel.IsEnabled = enabled;
            }
        }

        public MotorPushRodRecipeCompUIViewModel(ICompUIRunTimeCallBack callBack)
        {
            _callBack = callBack;
            CurrentValueViewModel = new TextInputViewModel { IsEnabled = false };
            PushDistanceViewModel = new TextInputViewModel();
            PushAxisSpeedViewModel = new TextInputViewModel();
            PushAxisAccelerationViewModel = new TextInputViewModel();

            PushDistanceViewModel.ValueChanged += (_, _) => AfterModified?.Invoke(this, EventArgs.Empty);
            PushAxisSpeedViewModel.ValueChanged += (_, _) => AfterModified?.Invoke(this, EventArgs.Empty);
            PushAxisAccelerationViewModel.ValueChanged += (_, _) => AfterModified?.Invoke(this, EventArgs.Empty);

            PushOnceCommand = ReactiveCommand.Create(() => ExecuteMoveCommand(PushRodSubMachineModulesConst.RtCmdPushOnce, true));
            PushRodForwardCommand = ReactiveCommand.Create(() => ExecuteMoveCommand(PushRodSubMachineModulesConst.RtCmdPusherForward, true));
            PushRodBackwardCommand = ReactiveCommand.Create(() => ExecuteMoveCommand(PushRodSubMachineModulesConst.RtCmdPusherBackward, true));

            RegisterAxisPositionInformProcessDelegate();
            ReadOnly = false;
            RefreshStatus();
        }

        public void Cleanup()
        {
            UnRegisterAxisPositionInformProcessDelegate();
        }

        public void SetData(MotorPushRodSubMachineModulesPPCfg data)
        {
            _data = data ?? new MotorPushRodSubMachineModulesPPCfg();
            CurrentValueViewModel.Text = _currentPosition.ToString("0.###", CultureInfo.InvariantCulture);
            PushDistanceViewModel.Text = _data.PushDistance.ToString(CultureInfo.InvariantCulture);
            PushAxisSpeedViewModel.Text = _data.PushAxisSpeed.ToString(CultureInfo.InvariantCulture);
            PushAxisAccelerationViewModel.Text = _data.PushAxisAcceleration.ToString(CultureInfo.InvariantCulture);
            RefreshStatus();
        }

        public MotorPushRodSubMachineModulesPPCfg GetData()
        {
            if (double.TryParse(CurrentValueViewModel.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var pos))
                _currentPosition = pos;

            _data.PushDistance = ParseDouble(PushDistanceViewModel.Text, _data.PushDistance);
            _data.PushAxisSpeed = ParseDouble(PushAxisSpeedViewModel.Text, _data.PushAxisSpeed);
            _data.PushAxisAcceleration = ParseDouble(PushAxisAccelerationViewModel.Text, _data.PushAxisAcceleration);
            return _data;
        }

        private void RegisterAxisPositionInformProcessDelegate()
        {
            ClientInfoProcessRegister.RegisterSvrInfoProcessDelegate<InformInfo_StatusChanged>(OnCompStatusChanged);
        }

        private void UnRegisterAxisPositionInformProcessDelegate()
        {
            ClientInfoProcessRegister.UnRegisterInfoProcessDelegate(
                InformInfo_StatusChanged.InfoKindID,
                OnCompStatusChanged);
        }

        private void OnCompStatusChanged(GriffinsInfoKindID infoKind, string infoNo, InformInfoBase info)
        {
            if (info is InformInfo_StatusChanged informInfo)
            {
                Dispatcher.UIThread.Post(() => ProcessAxisPositionChangedInformInfo(informInfo));
            }
        }

        private void ProcessAxisPositionChangedInformInfo(InformInfo_StatusChanged info)
        {
            if (string.IsNullOrWhiteSpace(info.Param))
                return;

            PushRodAxisStatus status;
            try
            {
                status = JsonObjConvert.FromJSon<PushRodAxisStatus>(info.Param);
            }
            catch
            {
                return;
            }

            if (status == null || status.Staus != 0)
                return;

            UpdateCurrentPosition(status.Position.ToString(CultureInfo.InvariantCulture));
        }

        private void ExecuteMoveCommand(string cmdId, bool includeDistance)
        {
            if (ReadOnly)
                return;

            var cmdParam = new GFBaseTypeParamValueList();
            var speed = ReadConfiguredDouble(PushAxisSpeedViewModel, _data.PushAxisSpeed);
            if (speed > 0)
            {
                cmdParam["MaxSpeed"] = new GriffinsBaseValue(speed.ToString(CultureInfo.InvariantCulture));
                var acc = ReadConfiguredDouble(PushAxisAccelerationViewModel, _data.PushAxisAcceleration);
                cmdParam["Acc"] = new GriffinsBaseValue(
                    Math.Max(1d, acc > 0 ? acc : speed / 10d).ToString(CultureInfo.InvariantCulture));
            }

            if (includeDistance)
            {
                var distance = ReadCurrentDistance();
                if (distance > 0)
                    cmdParam["PushDistance"] = new GriffinsBaseValue(distance.ToString(CultureInfo.InvariantCulture));
            }

            TryExecRuntimeCommand(cmdId, cmdParam);
            RefreshStatus();
            AfterModified?.Invoke(this, EventArgs.Empty);
        }

        private double ReadCurrentDistance()
        {
            if (double.TryParse(PushDistanceViewModel.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var val) && val > 0)
                return val;

            return _data.PushDistance;
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

        private void RefreshStatus()
        {
            var result = QueryRuntimeCommand(PushRodSubMachineModulesConst.RtCmdGetStatus, new GFBaseTypeParamValueList());
            var csv = TryGetResultData(result);
            if (string.IsNullOrWhiteSpace(csv))
                return;

            var parts = csv.Split(',');
            if (parts.Length == 0)
                return;

            UpdateCurrentPosition(parts[0].Trim());
        }

        private void UpdateCurrentPosition(string posText)
        {
            if (double.TryParse(posText, NumberStyles.Float, CultureInfo.InvariantCulture, out var pos))
            {
                _currentPosition = pos;
                var formatted = pos.ToString("0.###", CultureInfo.InvariantCulture);
                Dispatcher.UIThread.Post(() =>
                {
                    CurrentValueViewModel.Text = formatted;
                });
                return;
            }

            Dispatcher.UIThread.Post(() =>
            {
                CurrentValueViewModel.Text = posText;
            });
        }

        private GFBaseTypeParamValueList QueryRuntimeCommand(string cmdId, GFBaseTypeParamValueList cmdParam)
        {
            try
            {
                return _callBack?.ExecConfigSvrCtlCmd(cmdId, cmdParam) ?? new GFBaseTypeParamValueList();
            }
            catch
            {
                try
                {
                    return _callBack?.ExecConfigSvrCtlCmd(cmdId, cmdParam) ?? new GFBaseTypeParamValueList();
                }
                catch
                {
                    return new GFBaseTypeParamValueList();
                }
            }
        }

        private static string TryGetResultData(GFBaseTypeParamValueList result)
        {
            try
            {
                return result?["data"]?.ToString() ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        private static double ReadConfiguredDouble(TextInputViewModel viewModel, double fallback)
        {
            if (double.TryParse(viewModel.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var value) && value > 0)
                return value;

            return fallback;
        }

        private static double ParseDouble(string text, double fallback)
        {
            return double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var value)
                ? value
                : fallback;
        }
    }
}
