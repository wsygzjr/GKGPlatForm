using Avalonia.Threading;
using FixingStructureSubMachineModules;
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
using System.Threading.Tasks;

namespace Griffins.CompUI.FixingStructure.CompUI_FixingStructure.PageType.RecipeCfgPage.AxisFixRecipeConfig.ViewModels
{
    /// <summary>
    /// 电机固定机构配方配置页视图模型，负责轨迹参数编辑、位置教导与轴运动控制。
    /// </summary>
    internal class AxisFixRecipeConfigCompUIViewModel : ReactiveObject
    {
        private AxisFixSubMachineModulesPPCfg _data = new();
        private readonly ICompUIRunTimeCallBack _callBack;
        private bool _isContinuousMoving;
        private bool _isApplyingData;

        /// <summary>页面内容修改后通知宿主。</summary>
        public event EventHandler AfterModified;

        public TextInputViewModel StartSpeedViewModel { get; }
        public TextInputViewModel MaxSpeedViewModel { get; }
        public TextInputViewModel AccelerationViewModel { get; }
        public TextInputViewModel DecelerationViewModel { get; }

        public TextInputViewModel FixingPositionViewModel { get; }
        public TextInputViewModel ReleaseFixingPositionViewModel { get; }
        public TextInputViewModel CurrentPositionViewModel { get; }

        public ReactiveCommand<Unit, Unit> TeachFixingPositionCommand { get; }
        public ReactiveCommand<Unit, Unit> TeachReleasePositionCommand { get; }
        public ReactiveCommand<Unit, Unit> MoveToFixingPositionCommand { get; }
        public ReactiveCommand<Unit, Unit> MoveToReleasePositionCommand { get; }
        public ReactiveCommand<Unit, Unit> MoveUpCommand { get; }
        public ReactiveCommand<Unit, Unit> MoveDownCommand { get; }
        public ReactiveCommand<Unit, Unit> AxisStopCommand { get; }

        private bool _readOnly;
        public bool ReadOnly
        {
            get => _readOnly;
            set
            {
                this.RaiseAndSetIfChanged(ref _readOnly, value);
                ApplyReadOnly();
            }
        }

        public AxisFixRecipeConfigCompUIViewModel(ICompUIRunTimeCallBack callBack)
        {
            _callBack = callBack;

            StartSpeedViewModel = CreateEditableTextInput();
            MaxSpeedViewModel = CreateEditableTextInput();
            AccelerationViewModel = CreateEditableTextInput();
            DecelerationViewModel = CreateEditableTextInput();
            FixingPositionViewModel = CreateEditableTextInput();
            ReleaseFixingPositionViewModel = CreateEditableTextInput();
            CurrentPositionViewModel = new TextInputViewModel { IsEnabled = false };

            TeachFixingPositionCommand = ReactiveCommand.Create(TeachFixingPosition);
            TeachReleasePositionCommand = ReactiveCommand.Create(TeachReleasePosition);
            MoveToFixingPositionCommand = ReactiveCommand.CreateFromTask(async () => await MoveToPosition(ParseDouble(FixingPositionViewModel.Text, _data.FixingPosition)));
            MoveToReleasePositionCommand = ReactiveCommand.CreateFromTask(async () => await MoveToPosition(ParseDouble(ReleaseFixingPositionViewModel.Text, _data.ReleaseFixingPosition)));
            MoveUpCommand = ReactiveCommand.Create(() => ContinueMove(true));
            MoveDownCommand = ReactiveCommand.Create(() => ContinueMove(false));
            AxisStopCommand = ReactiveCommand.Create(StopAxis);

            RegisterPositionInformProcessDelegate();
            ReadOnly = false;
            Init(new AxisFixSubMachineModulesPPCfg());
        }

        /// <summary>加载配方数据到界面。</summary>
        public void Init(AxisFixSubMachineModulesPPCfg data)
        {
            ApplyWithoutModified(() =>
            {
                _data = CloneData(data);
                FillTrajectory(_data.trajectoryParameters);
                FixingPositionViewModel.Text = _data.FixingPosition.ToString("0.###", CultureInfo.InvariantCulture);
                ReleaseFixingPositionViewModel.Text = _data.ReleaseFixingPosition.ToString("0.###", CultureInfo.InvariantCulture);
            });
        }

        /// <summary>从界面收集配方数据，写回内部缓存以保证保存一致。</summary>
        public AxisFixSubMachineModulesPPCfg GetData()
        {
            var result = CloneData(_data);
            result.trajectoryParameters = BuildTrajectory(result.trajectoryParameters);
            result.FixingPosition = ParseDouble(FixingPositionViewModel.Text, result.FixingPosition);
            result.ReleaseFixingPosition = ParseDouble(ReleaseFixingPositionViewModel.Text, result.ReleaseFixingPosition);

            _data.CopyFrom(result);
            return result;
        }

        /// <summary>页面关闭时注销位置推送并停止连续运动。</summary>
        public void Cleanup()
        {
            UnregisterPositionInformProcessDelegate();
            if (_isContinuousMoving)
            {
                ExecuteRuntimeCommand(FixingStructureSubMachineModulesConst.RtCmdStop, new GFBaseTypeParamValueList());
                _isContinuousMoving = false;
            }
        }

        private TextInputViewModel CreateEditableTextInput()
        {
            var viewModel = new TextInputViewModel();
            viewModel.ValueChanged += (_, __) => RaiseAfterModified();
            return viewModel;
        }

        private void ApplyReadOnly()
        {
            var enabled = !_readOnly;
            StartSpeedViewModel.IsEnabled = enabled;
            MaxSpeedViewModel.IsEnabled = enabled;
            AccelerationViewModel.IsEnabled = enabled;
            DecelerationViewModel.IsEnabled = enabled;
            FixingPositionViewModel.IsEnabled = enabled;
            ReleaseFixingPositionViewModel.IsEnabled = enabled;
        }

        /// <summary>教导固定位置：将当前轴位置写入固定位置输入框。</summary>
        private void TeachFixingPosition()
        {
            FixingPositionViewModel.Text = CurrentPositionViewModel.Text ?? string.Empty;
            RaiseAfterModified();
        }

        /// <summary>教导松开位置：将当前轴位置写入松开位置输入框。</summary>
        private void TeachReleasePosition()
        {
            ReleaseFixingPositionViewModel.Text = CurrentPositionViewModel.Text ?? string.Empty;
            RaiseAfterModified();
        }

        ///// <summary>移动到固定位置。</summary>
        //private void MoveToFixingPosition()
        //{
        //    MoveToPosition(ParseDouble(FixingPositionViewModel.Text, _data.FixingPosition));
        //}

        ///// <summary>移动到松开位置。</summary>
        //private void MoveToReleasePosition()
        //{
        //    MoveToPosition(ParseDouble(ReleaseFixingPositionViewModel.Text, _data.ReleaseFixingPosition));
        //}

        /// <summary>调用后端 RtCmdMoveTo 执行定位运动，参数与 AxisFixSubMMCmdExecutor 一致。</summary>
        private async Task MoveToPosition(double position)
        {
            try
            {
                if (ReadOnly)
                {
                    return;
                }

                _data.trajectoryParameters = BuildTrajectory(_data.trajectoryParameters);
                var motion = _data.trajectoryParameters;
                var cmdParam = new GFBaseTypeParamValueList
                {
                    ["Speed"] = new GriffinsBaseValue(motion.MaxSpeed.ToString(CultureInfo.InvariantCulture)),
                    ["Acc"] = new GriffinsBaseValue(motion.Acceleration.ToString(CultureInfo.InvariantCulture)),
                    ["Position"] = new GriffinsBaseValue(position.ToString(CultureInfo.InvariantCulture)),
                };

                _isContinuousMoving = false;
                await Task.Run(() => ExecuteRuntimeCommand(FixingStructureSubMachineModulesConst.RtCmdMoveTo, cmdParam));
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"MoveToPosition 失败: {ex.Message}");
            }
            
        }

        /// <summary>调用后端 RtCmdContinueMove 执行连续上移/下移。</summary>
        private void ContinueMove(bool isUp)
        {
            if (ReadOnly)
            {
                return;
            }

            _data.trajectoryParameters = BuildTrajectory(_data.trajectoryParameters);
            var motion = _data.trajectoryParameters;
            var cmdParam = new GFBaseTypeParamValueList
            {
                ["Speed"] = new GriffinsBaseValue(motion.MaxSpeed.ToString(CultureInfo.InvariantCulture)),
                ["Acc"] = new GriffinsBaseValue(motion.Acceleration.ToString(CultureInfo.InvariantCulture)),
                ["Direction"] = new GriffinsBaseValue(isUp ? "true" : "false"),
            };

            ExecuteRuntimeCommand(FixingStructureSubMachineModulesConst.RtCmdContinueMove, cmdParam);
            _isContinuousMoving = true;
        }

        /// <summary>调用后端 RtCmdStop 停止轴运动。</summary>
        private void StopAxis()
        {
            if (ReadOnly)
            {
                return;
            }

            ExecuteRuntimeCommand(FixingStructureSubMachineModulesConst.RtCmdStop, new GFBaseTypeParamValueList());
            _isContinuousMoving = false;
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

        /// <summary>接收后端 SendToMapTmlStateChanged 推送的实时位置。</summary>
        private void OnCompStatusChanged(GriffinsInfoKindID infoKind, string infoNo, InformInfoBase info)
        {
            if (info is InformInfo_StatusChanged informInfo)
            {
                Dispatcher.UIThread.Post(() => ProcessPositionChangedInform(informInfo));
            }
        }

        private void ProcessPositionChangedInform(InformInfo_StatusChanged info)
        {
            if (string.IsNullOrWhiteSpace(info.Param))
            {
                return;
            }

            SendToMapTml message;
            try
            {
                message = JsonObjConvert.FromJSon<SendToMapTml>(info.Param);
            }
            catch
            {
                return;
            }

            if (message == null ||
                !string.Equals(message.MessageKind, "PositionChanged", StringComparison.Ordinal))
            {
                return;
            }

            UpdateCurrentPosition(message.Data);
        }

        private void UpdateCurrentPosition(string positionText)
        {
            if (string.IsNullOrWhiteSpace(positionText))
            {
                return;
            }

            if (double.TryParse(positionText, NumberStyles.Float, CultureInfo.InvariantCulture, out var position))
            {
                positionText = position.ToString("0.###", CultureInfo.InvariantCulture);
            }

            CurrentPositionViewModel.Text = positionText;
        }

        private void FillTrajectory(NonProcessingTrajectoryParameters trajectory)
        {
            StartSpeedViewModel.Text = trajectory.StartSpeed.ToString("0.###", CultureInfo.InvariantCulture);
            MaxSpeedViewModel.Text = trajectory.MaxSpeed.ToString("0.###", CultureInfo.InvariantCulture);
            AccelerationViewModel.Text = trajectory.Acceleration.ToString("0.###", CultureInfo.InvariantCulture);
            DecelerationViewModel.Text = trajectory.Deceleration.ToString("0.###", CultureInfo.InvariantCulture);
        }

        private NonProcessingTrajectoryParameters BuildTrajectory(NonProcessingTrajectoryParameters current)
        {
            current.StartSpeed = ParseDouble(StartSpeedViewModel.Text, current.StartSpeed);
            current.MaxSpeed = ParseDouble(MaxSpeedViewModel.Text, current.MaxSpeed);
            current.Acceleration = ParseDouble(AccelerationViewModel.Text, current.Acceleration);
            current.Deceleration = ParseDouble(DecelerationViewModel.Text, current.Deceleration);
            return current;
        }

        private GFBaseTypeParamValueList ExecuteRuntimeCommand(string cmdId, GFBaseTypeParamValueList cmdParam)
        {
            GFBaseTypeParamValueList result;
            try
            {
                result = _callBack?.ExecConfigSvrCtlCmd(cmdId, cmdParam) ?? new GFBaseTypeParamValueList();
            }
            catch
            {
                try
                {
                    result = _callBack?.ExecConfigSvrCtlCmd(cmdId, cmdParam) ?? new GFBaseTypeParamValueList();
                }
                catch
                {
                    result = new GFBaseTypeParamValueList();
                }
            }

            return result;
        }

        private void ApplyWithoutModified(Action action)
        {
            _isApplyingData = true;
            try
            {
                action?.Invoke();
            }
            finally
            {
                _isApplyingData = false;
            }
        }

        private void RaiseAfterModified()
        {
            if (_isApplyingData)
            {
                return;
            }

            AfterModified?.Invoke(this, EventArgs.Empty);
        }

        private static AxisFixSubMachineModulesPPCfg CloneData(AxisFixSubMachineModulesPPCfg data)
        {
            if (data == null)
            {
                return new AxisFixSubMachineModulesPPCfg();
            }

            var cloned = new AxisFixSubMachineModulesPPCfg();
            cloned.CopyFrom(data);
            return cloned;
        }

        private static double ParseDouble(string text, double defaultValue)
        {
            return double.TryParse(text, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var value)
                ? value
                : defaultValue;
        }
    }
}
