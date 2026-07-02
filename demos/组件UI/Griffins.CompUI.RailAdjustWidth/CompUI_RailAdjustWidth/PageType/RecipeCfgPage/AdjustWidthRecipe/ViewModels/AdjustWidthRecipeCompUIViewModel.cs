using Avalonia.Threading;
using GF_Gereric;
using GKG.SubMM;
using GKG.UI;
using Griffins.ImeIOT;
using Griffins.Map.UI;
using Griffins.PF;
using Griffins.PF.RichClient;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;

namespace Griffins.CompUI.RailAdjustWidth.CompUI_RailAdjustWidth.PageType.RecipeCfgPage.AdjustWidthRecipe.ViewModels
{
    internal class AdjustWidthRecipeCompUIViewModel : ReactiveObject
    {
        private static readonly (RailAdjustWidthAxis Axis, string DisplayName)[] FixRailOptions =
        {
            (RailAdjustWidthAxis.FrontRail, "前轨"),
            (RailAdjustWidthAxis.BackRail, "后轨"),
        };

        private readonly bool isDesign;
        private readonly ICompUIRunTimeCallBack callBack;

        private RailAdjustWidthSubMachineModulesPPCfg loadedData = new();
        private RailAdjustWidthSubMachineModulesFactoryCfg factoryCfg = new();
        private bool readOnly;
        private bool isApplyingData;
        private bool isContinuousMoving;

        public ComboxViewModel FixRailIDViewModel { get; } = CreateComboViewModel();
        public TextInputViewModel FixRailPositionViewModel { get; } = new TextInputViewModel();
        public TextInputViewModel WidthViewModel { get; } = new TextInputViewModel();
        public TextInputViewModel AdjustWidthSpeedViewModel { get; } = new TextInputViewModel();
        public TextInputViewModel AdjustWidthAccelerationViewModel { get; } = new TextInputViewModel();
        public TextInputViewModel FrontRailCurrentPositionViewModel { get; } = new TextInputViewModel { IsEnabled = false };
        public TextInputViewModel BackRailCurrentPositionViewModel { get; } = new TextInputViewModel { IsEnabled = false };

        public ReactiveCommand<Unit, Unit> TeachFixRailPositionCommand { get; }
        public ReactiveCommand<Unit, Unit> MoveToFixRailPositionCommand { get; }
        public ReactiveCommand<Unit, Unit> AdjustWidthCommand { get; }
        public ReactiveCommand<Unit, Unit> StopCommand { get; }
        public ReactiveCommand<Unit, Unit> GoHomeCommand { get; }

        public event EventHandler AfterModified;

        public bool ReadOnly
        {
            get => readOnly;
            set
            {
                this.RaiseAndSetIfChanged(ref readOnly, value);
                var editable = !readOnly;
                FixRailIDViewModel.IsEnabled = editable;
                FixRailPositionViewModel.IsEnabled = editable;
                WidthViewModel.IsEnabled = editable;
                AdjustWidthSpeedViewModel.IsEnabled = editable;
                AdjustWidthAccelerationViewModel.IsEnabled = editable;
                this.RaisePropertyChanged(nameof(CanOperateSelectedRail));
                this.RaisePropertyChanged(nameof(CanAdjustWidth));
            }
        }

        public bool CanOperateSelectedRail => !ReadOnly && (isDesign || IsSelectedRailMovable());

        public bool CanAdjustWidth => !ReadOnly;

        public bool IsFrontRailSelected => GetSelectedFixRailID() == RailAdjustWidthAxis.FrontRail;

        public bool IsBackRailSelected => GetSelectedFixRailID() == RailAdjustWidthAxis.BackRail;

        public string SelectedRailPositionLabel
            => IsFrontRailSelected ? "前轨当前位置(mm)" : "后轨当前位置(mm)";

        public TextInputViewModel SelectedRailCurrentPositionViewModel
            => IsFrontRailSelected
                ? FrontRailCurrentPositionViewModel
                : BackRailCurrentPositionViewModel;

        public AdjustWidthRecipeCompUIViewModel()
        {
            isDesign = true;
            SubscribeValueChanges();
            ApplyFixRailOptions(loadedData.FixRailID ?? RailAdjustWidthAxis.FrontRail);
            TeachFixRailPositionCommand = ReactiveCommand.Create(() => { });
            MoveToFixRailPositionCommand = ReactiveCommand.Create(() => { });
            AdjustWidthCommand = ReactiveCommand.Create(() => { });
            StopCommand = ReactiveCommand.Create(() => { });
            GoHomeCommand = ReactiveCommand.Create(() => { });
            ReadOnly = false;
            RaiseSelectedRailChanged();
        }

        public AdjustWidthRecipeCompUIViewModel(ICompUIRunTimeCallBack callBack)
            : this()
        {
            isDesign = false;
            this.callBack = callBack;

            LoadFactoryParams();
            RegisterPositionInformProcessDelegate();

            TeachFixRailPositionCommand = ReactiveCommand.Create(TeachFixRailPosition);
            MoveToFixRailPositionCommand = ReactiveCommand.CreateFromTask(MoveToFixRailPositionAsync);
            AdjustWidthCommand = ReactiveCommand.Create(ExecuteAdjustWidth);
            StopCommand = ReactiveCommand.Create(StopSelectedRail);
            GoHomeCommand = ReactiveCommand.CreateFromTask(GoHomeSelectedRailAsync);

            FixRailIDViewModel.ValueChanged += (_, __) => RaiseSelectedRailChanged();
        }

        public void SetData(RailAdjustWidthSubMachineModulesPPCfg data)
        {
            ApplyWithoutModified(() =>
            {
                loadedData = CloneData(data);
                ApplyFixRailOptions(loadedData.FixRailID ?? RailAdjustWidthAxis.FrontRail);
                FixRailPositionViewModel.Text = (loadedData.FixRailPosition ?? 0)
                    .ToString(CultureInfo.InvariantCulture);
                WidthViewModel.Text = loadedData.Width.ToString(CultureInfo.InvariantCulture);
                AdjustWidthSpeedViewModel.Text = loadedData.AdjustWidthSpeed.ToString(CultureInfo.InvariantCulture);
                AdjustWidthAccelerationViewModel.Text = loadedData.AdjustWidthAcceleration.ToString(CultureInfo.InvariantCulture);
            });

            RaiseSelectedRailChanged();
        }

        public RailAdjustWidthSubMachineModulesPPCfg GetData()
        {
            var result = CloneData(loadedData) ?? new RailAdjustWidthSubMachineModulesPPCfg();
            result.FixRailID = GetSelectedFixRailID();

            if (double.TryParse(FixRailPositionViewModel.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var fixPos))
            {
                result.FixRailPosition = fixPos;
            }

            if (double.TryParse(WidthViewModel.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var width))
            {
                result.Width = width;
            }

            if (int.TryParse(AdjustWidthSpeedViewModel.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var speed))
            {
                result.AdjustWidthSpeed = speed;
            }

            if (int.TryParse(AdjustWidthAccelerationViewModel.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var acceleration))
            {
                result.AdjustWidthAcceleration = acceleration;
            }

            loadedData = CloneData(result);
            return result;
        }

        public void StartContinueMove(bool isForward)
        {
            if (!CanOperateSelectedRail)
            {
                return;
            }

            var cmdParam = CreateRailMotionParam(GetSelectedFixRailID());
            cmdParam["Direction"] = new GriffinsBaseValue(
                (isForward ? MoveDirection.Positive : MoveDirection.Negative).ToString());

            ExecuteRuntimeCommand(RailAdjustWidthSubMachineModulesConst.RailContinueMoveMethodID, cmdParam);
            isContinuousMoving = true;
        }

        public void StopSelectedRail()
        {
            if (isDesign || callBack == null)
            {
                return;
            }

            if (!isContinuousMoving && ReadOnly)
            {
                return;
            }

            var cmdParam = CreateRailParam(GetSelectedFixRailID());
            ExecuteRuntimeCommand(RailAdjustWidthSubMachineModulesConst.RtCmdStop, cmdParam);
            isContinuousMoving = false;
        }

        public void Cleanup()
        {
            UnregisterPositionInformProcessDelegate();
            if (isContinuousMoving)
            {
                StopSelectedRail();
            }
        }

        private void TeachFixRailPosition()
        {
            if (!CanOperateSelectedRail)
            {
                return;
            }

            FixRailPositionViewModel.Text = GetSelectedRailCurrentPositionText();
            NotifyDataModified();
        }

        private async Task MoveToFixRailPositionAsync()
        {
            if (!CanOperateSelectedRail)
            {
                return;
            }

            var position = ParseDouble(FixRailPositionViewModel.Text, loadedData.FixRailPosition ?? 0);
            var cmdParam = CreateRailMotionParam(GetSelectedFixRailID());
            cmdParam["Position"] = new GriffinsBaseValue(position.ToString(CultureInfo.InvariantCulture));

            isContinuousMoving = false;
            await Task.Run(() =>
            {
                ExecuteRuntimeCommand(RailAdjustWidthSubMachineModulesConst.RtCmdMoveTo, cmdParam);
            }).ConfigureAwait(true);
        }

        private void ExecuteAdjustWidth()
        {
            if (ReadOnly)
            {
                return;
            }

            ExecuteRuntimeCommand(
                RailAdjustWidthSubMachineModulesConst.RailAdjustWidthMethodID,
                new GFBaseTypeParamValueList());
            isContinuousMoving = false;
        }

        private async Task GoHomeSelectedRailAsync()
        {
            if (!CanOperateSelectedRail)
            {
                return;
            }

            var cmdParam = CreateRailParam(GetSelectedFixRailID());
            isContinuousMoving = false;
            await Task.Run(() =>
            {
                ExecuteRuntimeCommand(RailAdjustWidthSubMachineModulesConst.RtGoHome, cmdParam);
            }).ConfigureAwait(true);
        }

        private void LoadFactoryParams()
        {
            if (callBack == null)
            {
                return;
            }

            try
            {
                var result = callBack.ExecConfigSvrCtlCmd(
                    RailAdjustWidthSubMachineModulesConst.RtCmdGetFactoryParams,
                    new GFBaseTypeParamValueList());

                var raw = result?["Result"]?.ToStringVal();
                if (string.IsNullOrWhiteSpace(raw))
                {
                    return;
                }

                factoryCfg = JsonObjConvert.FromJSon<RailAdjustWidthSubMachineModulesFactoryCfg>(raw)
                    ?? new RailAdjustWidthSubMachineModulesFactoryCfg();
            }
            catch
            {
                factoryCfg = new RailAdjustWidthSubMachineModulesFactoryCfg();
            }
        }

        private void RegisterPositionInformProcessDelegate()
        {
            ClientInfoProcessRegister.RegisterSvrInfoProcessDelegate<InformInfo_StatusChanged>(OnCompStatusChanged);
        }

        private void UnregisterPositionInformProcessDelegate()
        {
            ClientInfoProcessRegister.UnRegisterInfoProcessDelegate(
                InformInfo_StatusChanged.InfoKindID,
                OnCompStatusChanged);
        }

        private void OnCompStatusChanged(GriffinsInfoKindID infoKind, string infoNo, InformInfoBase info)
        {
            if (info is InformInfo_StatusChanged informInfo)
            {
                Dispatcher.UIThread.Post(() => ProcessPositionChangedInform(informInfo));
            }
        }

        private void ProcessPositionChangedInform(InformInfo_StatusChanged info)
        {
            if (info == null || string.IsNullOrWhiteSpace(info.CompType) || string.IsNullOrWhiteSpace(info.Param))
            {
                return;
            }

            var positionText = FormatPositionText(info.Param);
            if (string.Equals(
                    info.CompType,
                    RailAdjustWidthSubMachineModulesConst.FrontRailPosition,
                    StringComparison.Ordinal))
            {
                FrontRailCurrentPositionViewModel.Text = positionText;
                return;
            }

            if (string.Equals(
                    info.CompType,
                    RailAdjustWidthSubMachineModulesConst.BackRailPosition,
                    StringComparison.Ordinal))
            {
                BackRailCurrentPositionViewModel.Text = positionText;
            }
        }

        private string GetSelectedRailCurrentPositionText()
        {
            return GetSelectedFixRailID() == RailAdjustWidthAxis.FrontRail
                ? FrontRailCurrentPositionViewModel.Text ?? string.Empty
                : BackRailCurrentPositionViewModel.Text ?? string.Empty;
        }

        private bool IsSelectedRailMovable()
        {
            return GetSelectedFixRailID() switch
            {
                RailAdjustWidthAxis.FrontRail => factoryCfg.FrontRailIsMovable,
                RailAdjustWidthAxis.BackRail => factoryCfg.BackRailIsMovable,
                _ => false,
            };
        }

        private GFBaseTypeParamValueList CreateRailParam(RailAdjustWidthAxis railId)
        {
            return new GFBaseTypeParamValueList
            {
                ["RailID"] = new GriffinsBaseValue(railId.ToString()),
            };
        }

        private GFBaseTypeParamValueList CreateRailMotionParam(RailAdjustWidthAxis railId)
        {
            var cmdParam = CreateRailParam(railId);
            cmdParam["Speed"] = new GriffinsBaseValue(GetAdjustWidthSpeed().ToString(CultureInfo.InvariantCulture));
            cmdParam["Acceleration"] = new GriffinsBaseValue(GetAdjustWidthAcceleration().ToString(CultureInfo.InvariantCulture));
            return cmdParam;
        }

        private int GetAdjustWidthSpeed()
        {
            if (int.TryParse(AdjustWidthSpeedViewModel.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var speed) && speed > 0)
            {
                return speed;
            }

            return loadedData.AdjustWidthSpeed > 0 ? loadedData.AdjustWidthSpeed : 20;
        }

        private int GetAdjustWidthAcceleration()
        {
            if (int.TryParse(AdjustWidthAccelerationViewModel.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var acceleration) && acceleration > 0)
            {
                return acceleration;
            }

            return loadedData.AdjustWidthAcceleration > 0 ? loadedData.AdjustWidthAcceleration : 200;
        }

        private GFBaseTypeParamValueList ExecuteRuntimeCommand(string cmdId, GFBaseTypeParamValueList cmdParam)
        {
            GFBaseTypeParamValueList result;
            try
            {
                result = callBack?.ExecConfigSvrCtlCmd(cmdId, cmdParam) ?? new GFBaseTypeParamValueList();
            }
            catch
            {
                try
                {
                    result = callBack?.ExecConfigSvrCtlCmd(cmdId, cmdParam) ?? new GFBaseTypeParamValueList();
                }
                catch
                {
                    result = new GFBaseTypeParamValueList();
                }
            }

            return result;
        }

        private void RaiseSelectedRailChanged()
        {
            this.RaisePropertyChanged(nameof(IsFrontRailSelected));
            this.RaisePropertyChanged(nameof(IsBackRailSelected));
            this.RaisePropertyChanged(nameof(SelectedRailPositionLabel));
            this.RaisePropertyChanged(nameof(SelectedRailCurrentPositionViewModel));
            this.RaisePropertyChanged(nameof(CanOperateSelectedRail));
        }

        private void SubscribeValueChanges()
        {
            FixRailIDViewModel.ValueChanged += (_, __) =>
            {
                RaiseSelectedRailChanged();
                NotifyDataModified();
            };
            FixRailPositionViewModel.ValueChanged += (_, __) => NotifyDataModified();
            WidthViewModel.ValueChanged += (_, __) => NotifyDataModified();
            AdjustWidthSpeedViewModel.ValueChanged += (_, __) => NotifyDataModified();
            AdjustWidthAccelerationViewModel.ValueChanged += (_, __) => NotifyDataModified();
        }

        private void ApplyFixRailOptions(RailAdjustWidthAxis selectedFixRail)
        {
            var items = FixRailIDViewModel.ItemsSource as ObservableCollection<ComBoxItem>;
            items?.Clear();
            if (items == null)
            {
                return;
            }

            foreach (var option in FixRailOptions)
            {
                items.Add(new ComBoxItem
                {
                    Value = option.Axis,
                    DisplayName = option.DisplayName,
                });
            }

            var target = items.FirstOrDefault(item => item.Value is RailAdjustWidthAxis axis && axis == selectedFixRail)
                ?? items.FirstOrDefault();
            FixRailIDViewModel.SelectedItem = target;
        }

        private RailAdjustWidthAxis GetSelectedFixRailID()
        {
            if (FixRailIDViewModel.SelectedItem is ComBoxItem item && item.Value is RailAdjustWidthAxis axis)
            {
                return axis;
            }

            return loadedData.FixRailID ?? RailAdjustWidthAxis.FrontRail;
        }

        private void NotifyDataModified()
        {
            if (isApplyingData)
            {
                return;
            }

            AfterModified?.Invoke(this, EventArgs.Empty);
        }

        private void ApplyWithoutModified(Action action)
        {
            isApplyingData = true;
            try
            {
                action();
            }
            finally
            {
                isApplyingData = false;
            }
        }

        private static string FormatPositionText(string positionText)
        {
            if (double.TryParse(positionText, NumberStyles.Float, CultureInfo.InvariantCulture, out var position))
            {
                return position.ToString("0.###", CultureInfo.InvariantCulture);
            }

            return positionText;
        }

        private static double ParseDouble(string text, double defaultValue)
        {
            return double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var value)
                ? value
                : defaultValue;
        }

        private static ComboxViewModel CreateComboViewModel()
        {
            return new ComboxViewModel
            {
                DisplayMemberPath = nameof(ComBoxItem.DisplayName),
                ItemsSource = new ObservableCollection<ComBoxItem>(),
            };
        }

        private static RailAdjustWidthSubMachineModulesPPCfg CloneData(RailAdjustWidthSubMachineModulesPPCfg data)
        {
            if (data == null)
            {
                return new RailAdjustWidthSubMachineModulesPPCfg();
            }

            var clone = new RailAdjustWidthSubMachineModulesPPCfg();
            clone.CopyFrom(data);
            return clone;
        }
    }
}
