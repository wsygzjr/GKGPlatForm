using Avalonia.Threading;
using GF_Gereric;
using GKG;
using GKG.MM;
using GKG.SubMM;
using GKG.SubMM.StorageDeviceModule;
using GKG.SubMM.TransportMechanismModule;
using GKG.UI;
using Griffins.ImeIOT;
using Griffins.Map.UI;
using Griffins.PF;
using Griffins.PF.RichClient;
using Newtonsoft.JsonG;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using BackendMaterialBoxPPCfg = GKG.SubMM.MaterialBoxSubMachineModulesPPCfg;

namespace Griffins.CompUI.MaterialBox.CompUI_MaterialBox.PageType.RecipeCfgPage.MaterialBoxConfig.ViewModels
{
    /// <summary>
    /// 料盒配方配置页视图模型，负责界面显示、运行时命令调用以及当前位置刷新。
    /// </summary>
    public class MaterialBoxConfigCompUIViewModel : ReactiveObject
    {
        /// <summary>移动到指定位置命令 ID。</summary>
        private const string CmdMoveTo = "MoveTo";
        /// <summary>Z 轴上移命令 ID。</summary>
        private const string CmdZMoveUp = "ZMoveUp";
        /// <summary>Z 轴下移命令 ID。</summary>
        private const string CmdZMoveDown = "ZMoveDown";
        /// <summary>Z 轴停止命令 ID。</summary>
        private const string CmdZAxisStop = "ZAxisStop";
        /// <summary>料盒夹紧命令 ID。</summary>
        private const string CmdMagazineClamp = "MagazineClamp";
        /// <summary>料盒张开命令 ID。</summary>
        private const string CmdMagazineUnclamp = "MagazineUnclamp";
        /// <summary>读取当前位置命令 ID。</summary>
        private const string CmdGetAxisPos = "GetAxisPos";

        /// <summary>上料上层料盒编号。</summary>
        private const int LoadUpperRack = 0;
        /// <summary>上料下层料盒编号。</summary>
        private const int LoadLowerRack = 1;
        /// <summary>下料上层料盒编号。</summary>
        private const int UnloadUpperRack = 2;
        /// <summary>下料下层料盒编号。</summary>
        private const int UnloadLowerRack = 3;

        /// <summary>上料轴默认显示名称。</summary>
        private const string FeedAxisName = "上料Z轴";
        /// <summary>下料轴默认显示名称。</summary>
        private const string UnloadAxisName = "下料Z轴";
        private const string LoadAxisInformType = "Load";
        private const string UnloadAxisInformType = "Unload";

        /// <summary>当前前端页面缓存的数据对象。</summary>
        private BackendMaterialBoxPPCfg _data = new BackendMaterialBoxPPCfg();
        /// <summary>宿主传入的运行时命令回调接口。</summary>
        private readonly ICompUIRunTimeCallBack? _callBack;
        /// <summary>记录当前仍处于连续运动状态的料盒编号。</summary>
        private readonly System.Collections.Generic.HashSet<int> _continuousMovingRacks = new();
        /// <summary>保护连续运动状态集合的线程锁。</summary>
        private readonly object _continuousMovingLock = new();

        /// <summary>页面任意字段发生修改时触发的通知事件。</summary>
        public event EventHandler? AfterModified;

        /// <summary>上料上层料盒参数视图模型。</summary>
        public MaterialBoxLayerViewModel LoadUpperLayerBox { get; } = new();
        /// <summary>上料下层料盒参数视图模型。</summary>
        public MaterialBoxLayerViewModel LoadLowerLayerBox { get; } = new();
        /// <summary>下料上层料盒参数视图模型。</summary>
        public MaterialBoxLayerViewModel UnloadUpperLayerBox { get; } = new();
        /// <summary>下料下层料盒参数视图模型。</summary>
        public MaterialBoxLayerViewModel UnloadLowerLayerBox { get; } = new();

        /// <summary>上料上层操作参数视图模型。</summary>
        public MaterialBoxInOutOperationViewModel LoadUpperInOutOperation { get; } = new(FeedAxisName);
        /// <summary>上料下层操作参数视图模型。</summary>
        public MaterialBoxInOutOperationViewModel LoadLowerInOutOperation { get; } = new(FeedAxisName);
        /// <summary>下料上层操作参数视图模型。</summary>
        public MaterialBoxInOutOperationViewModel UnloadUpperInOutOperation { get; } = new(UnloadAxisName);
        /// <summary>下料下层操作参数视图模型。</summary>
        public MaterialBoxInOutOperationViewModel UnloadLowerInOutOperation { get; } = new(UnloadAxisName);

        /// <summary>其它参数区域的视图模型。</summary>
        public MaterialBoxOtherParametersViewModel OtherParameters { get; } = new();
        public ReactiveCommand<Unit, Unit> LoadUpperCalculateDistanceCommand { get; }
        public ReactiveCommand<Unit, Unit> LoadLowerCalculateDistanceCommand { get; }
        public ReactiveCommand<Unit, Unit> UnloadUpperCalculateDistanceCommand { get; }
        public ReactiveCommand<Unit, Unit> UnloadLowerCalculateDistanceCommand { get; }
        public ReactiveCommand<Unit, Unit> LoadUpperTeachFirstCommand { get; }
        public ReactiveCommand<Unit, Unit> LoadLowerTeachFirstCommand { get; }
        public ReactiveCommand<Unit, Unit> UnloadUpperTeachFirstCommand { get; }
        public ReactiveCommand<Unit, Unit> UnloadLowerTeachFirstCommand { get; }
        public ReactiveCommand<Unit, Unit> LoadUpperTeachLastCommand { get; }
        public ReactiveCommand<Unit, Unit> LoadLowerTeachLastCommand { get; }
        public ReactiveCommand<Unit, Unit> UnloadUpperTeachLastCommand { get; }
        public ReactiveCommand<Unit, Unit> UnloadLowerTeachLastCommand { get; }
        public ReactiveCommand<Unit, Unit> LoadUpperMoveToCommand { get; }
        public ReactiveCommand<Unit, Unit> LoadLowerMoveToCommand { get; }
        public ReactiveCommand<Unit, Unit> UnloadUpperMoveToCommand { get; }
        public ReactiveCommand<Unit, Unit> UnloadLowerMoveToCommand { get; }
        public ReactiveCommand<Unit, Unit> LoadUpperMoveCommand { get; }
        public ReactiveCommand<Unit, Unit> LoadLowerMoveCommand { get; }
        public ReactiveCommand<Unit, Unit> UnloadUpperMoveCommand { get; }
        public ReactiveCommand<Unit, Unit> UnloadLowerMoveCommand { get; }
        public ReactiveCommand<Unit, Unit> LoadUpperMoveUpCommand { get; }
        public ReactiveCommand<Unit, Unit> LoadLowerMoveUpCommand { get; }
        public ReactiveCommand<Unit, Unit> UnloadUpperMoveUpCommand { get; }
        public ReactiveCommand<Unit, Unit> UnloadLowerMoveUpCommand { get; }
        public ReactiveCommand<Unit, Unit> LoadUpperAxisStopCommand { get; }
        public ReactiveCommand<Unit, Unit> LoadLowerAxisStopCommand { get; }
        public ReactiveCommand<Unit, Unit> UnloadUpperAxisStopCommand { get; }
        public ReactiveCommand<Unit, Unit> UnloadLowerAxisStopCommand { get; }
        public ReactiveCommand<Unit, Unit> LoadUpperMoveDownCommand { get; }
        public ReactiveCommand<Unit, Unit> LoadLowerMoveDownCommand { get; }
        public ReactiveCommand<Unit, Unit> UnloadUpperMoveDownCommand { get; }
        public ReactiveCommand<Unit, Unit> UnloadLowerMoveDownCommand { get; }
        public ReactiveCommand<Unit, Unit> LoadUpperClampCommand { get; }
        public ReactiveCommand<Unit, Unit> LoadLowerClampCommand { get; }
        public ReactiveCommand<Unit, Unit> UnloadUpperClampCommand { get; }
        public ReactiveCommand<Unit, Unit> UnloadLowerClampCommand { get; }
        public ReactiveCommand<Unit, Unit> LoadUpperReleaseCommand { get; }
        public ReactiveCommand<Unit, Unit> LoadLowerReleaseCommand { get; }
        public ReactiveCommand<Unit, Unit> UnloadUpperReleaseCommand { get; }
        public ReactiveCommand<Unit, Unit> UnloadLowerReleaseCommand { get; }
        /// <summary>创建配方配置页视图模型，并初始化所有命令与默认数据。</summary>
        public MaterialBoxConfigCompUIViewModel(ICompUIRunTimeCallBack? callBack = null)
        {
            _callBack = callBack;

            RegisterAxisStatusInformProcessDelegate();
            SubscribeNested();

            LoadUpperCalculateDistanceCommand = ReactiveCommand.Create(() => CalculateSpacing(LoadUpperLayerBox, LoadUpperRack));
            LoadLowerCalculateDistanceCommand = ReactiveCommand.Create(() => CalculateSpacing(LoadLowerLayerBox, LoadLowerRack));
            UnloadUpperCalculateDistanceCommand = ReactiveCommand.Create(() => CalculateSpacing(UnloadUpperLayerBox, UnloadUpperRack));
            UnloadLowerCalculateDistanceCommand = ReactiveCommand.Create(() => CalculateSpacing(UnloadLowerLayerBox, UnloadLowerRack));

            LoadUpperTeachFirstCommand = ReactiveCommand.Create(() => TeachFirstSlot(LoadUpperLayerBox, LoadUpperInOutOperation, LoadUpperRack, true));
            LoadLowerTeachFirstCommand = ReactiveCommand.Create(() => TeachFirstSlot(LoadLowerLayerBox, LoadLowerInOutOperation, LoadLowerRack, true));
            UnloadUpperTeachFirstCommand = ReactiveCommand.Create(() => TeachFirstSlot(UnloadUpperLayerBox, UnloadUpperInOutOperation, UnloadUpperRack, true));
            UnloadLowerTeachFirstCommand = ReactiveCommand.Create(() => TeachFirstSlot(UnloadLowerLayerBox, UnloadLowerInOutOperation, UnloadLowerRack, true));

            LoadUpperTeachLastCommand = ReactiveCommand.Create(() => TeachFirstSlot(LoadUpperLayerBox, LoadUpperInOutOperation, LoadUpperRack, false));
            LoadLowerTeachLastCommand = ReactiveCommand.Create(() => TeachFirstSlot(LoadLowerLayerBox, LoadLowerInOutOperation, LoadLowerRack, false));
            UnloadUpperTeachLastCommand = ReactiveCommand.Create(() => TeachFirstSlot(UnloadUpperLayerBox, UnloadUpperInOutOperation, UnloadUpperRack, false));
            UnloadLowerTeachLastCommand = ReactiveCommand.Create(() => TeachFirstSlot(UnloadLowerLayerBox, UnloadLowerInOutOperation, UnloadLowerRack, false));

            LoadUpperMoveToCommand = ReactiveCommand.CreateFromTask(async () => await MoveToSlot(LoadUpperLayerBox, LoadUpperInOutOperation, LoadUpperRack, true));
            LoadLowerMoveToCommand = ReactiveCommand.CreateFromTask(async () => await MoveToSlot(LoadLowerLayerBox, LoadLowerInOutOperation, LoadLowerRack, true));
            UnloadUpperMoveToCommand = ReactiveCommand.CreateFromTask(async () => await MoveToSlot(UnloadUpperLayerBox, UnloadUpperInOutOperation, UnloadUpperRack, true));
            UnloadLowerMoveToCommand = ReactiveCommand.CreateFromTask(async () => await MoveToSlot(UnloadLowerLayerBox, UnloadLowerInOutOperation, UnloadLowerRack, true));

            LoadUpperMoveCommand = ReactiveCommand.CreateFromTask(async () => await MoveToSlot(LoadUpperLayerBox, LoadUpperInOutOperation, LoadUpperRack, false));
            LoadLowerMoveCommand = ReactiveCommand.CreateFromTask(async () => await MoveToSlot(LoadLowerLayerBox, LoadLowerInOutOperation, LoadLowerRack, false));
            UnloadUpperMoveCommand = ReactiveCommand.CreateFromTask(async () => await MoveToSlot(UnloadUpperLayerBox, UnloadUpperInOutOperation, UnloadUpperRack, false));
            UnloadLowerMoveCommand = ReactiveCommand.CreateFromTask(async () => await MoveToSlot(UnloadLowerLayerBox, UnloadLowerInOutOperation, UnloadLowerRack, false));

            LoadUpperMoveUpCommand = ReactiveCommand.Create(() => MoveStep(LoadUpperInOutOperation, LoadUpperRack, true));
            LoadLowerMoveUpCommand = ReactiveCommand.Create(() => MoveStep(LoadLowerInOutOperation, LoadLowerRack, true));
            UnloadUpperMoveUpCommand = ReactiveCommand.Create(() => MoveStep(UnloadUpperInOutOperation, UnloadUpperRack, true));
            UnloadLowerMoveUpCommand = ReactiveCommand.Create(() => MoveStep(UnloadLowerInOutOperation, UnloadLowerRack, true));

            LoadUpperAxisStopCommand = ReactiveCommand.Create(() => StopAxis(LoadUpperInOutOperation, LoadUpperRack));
            LoadLowerAxisStopCommand = ReactiveCommand.Create(() => StopAxis(LoadLowerInOutOperation, LoadLowerRack));
            UnloadUpperAxisStopCommand = ReactiveCommand.Create(() => StopAxis(UnloadUpperInOutOperation, UnloadUpperRack));
            UnloadLowerAxisStopCommand = ReactiveCommand.Create(() => StopAxis(UnloadLowerInOutOperation, UnloadLowerRack));

            LoadUpperMoveDownCommand = ReactiveCommand.Create(() => MoveStep(LoadUpperInOutOperation, LoadUpperRack, false));
            LoadLowerMoveDownCommand = ReactiveCommand.Create(() => MoveStep(LoadLowerInOutOperation, LoadLowerRack, false));
            UnloadUpperMoveDownCommand = ReactiveCommand.Create(() => MoveStep(UnloadUpperInOutOperation, UnloadUpperRack, false));
            UnloadLowerMoveDownCommand = ReactiveCommand.Create(() => MoveStep(UnloadLowerInOutOperation, UnloadLowerRack, false));

            LoadUpperClampCommand = ReactiveCommand.Create(() => ControlClamp(LoadUpperRack, true));
            LoadLowerClampCommand = ReactiveCommand.Create(() => ControlClamp(LoadLowerRack, true));
            UnloadUpperClampCommand = ReactiveCommand.Create(() => ControlClamp(UnloadUpperRack, true));
            UnloadLowerClampCommand = ReactiveCommand.Create(() => ControlClamp(UnloadLowerRack, true));

            LoadUpperReleaseCommand = ReactiveCommand.Create(() => ControlClamp(LoadUpperRack, false));
            LoadLowerReleaseCommand = ReactiveCommand.Create(() => ControlClamp(LoadLowerRack, false));
            UnloadUpperReleaseCommand = ReactiveCommand.Create(() => ControlClamp(UnloadUpperRack, false));
            UnloadLowerReleaseCommand = ReactiveCommand.Create(() => ControlClamp(UnloadLowerRack, false));

            Init(new BackendMaterialBoxPPCfg());
        }
        ~MaterialBoxConfigCompUIViewModel()
        {
            UnRegisterAxisStatusInformProcessDelegate();
        }
        /// <summary>把前端配方数据加载到当前页面。</summary>
        public void Init(BackendMaterialBoxPPCfg data)
        {
            _data = data ?? new BackendMaterialBoxPPCfg();
            _data.LoadStorageDevice ??= new StorageDevicePPCfg();
            _data.UnloadStorageDevice ??= new StorageDevicePPCfg();
            _data.LoadTransportMechanism ??= new TransportMechanismPPCfg();
            _data.UnloadTransportMechanism ??= new TransportMechanismPPCfg();
            _data.FeedingPortPPCfg ??= new MaterialBoxSubMachineModules.FeedPort.FeedPortPPCfg();
            _data.ReceivePortPPCfg ??= new MaterialBoxSubMachineModules.FeedPort.FeedPortPPCfg();

            EnsureStorageCount(_data.LoadStorageDevice, 2);
            EnsureStorageCount(_data.UnloadStorageDevice, 2);

            LoadUpperLayerBox.Load(_data.LoadStorageDevice.Storages[LoadUpperRack]);
            LoadLowerLayerBox.Load(_data.LoadStorageDevice.Storages[LoadLowerRack]);
            UnloadUpperLayerBox.Load(_data.UnloadStorageDevice.Storages[0]);
            UnloadLowerLayerBox.Load(_data.UnloadStorageDevice.Storages[1]);

            LoadUpperInOutOperation.Load(_data.LoadTransportMechanism, FeedAxisName);
            LoadLowerInOutOperation.Load(_data.LoadTransportMechanism, FeedAxisName);
            UnloadUpperInOutOperation.Load(_data.UnloadTransportMechanism, UnloadAxisName);
            UnloadLowerInOutOperation.Load(_data.UnloadTransportMechanism, UnloadAxisName);

            OtherParameters.Load(_data);
            RefreshAllAxisPositions();
        }

        /// <summary>从当前页面收集最新的前端配方数据。</summary>
        public BackendMaterialBoxPPCfg GetData()
        {
            _data ??= new BackendMaterialBoxPPCfg();
            _data.LoadStorageDevice ??= new StorageDevicePPCfg();
            _data.UnloadStorageDevice ??= new StorageDevicePPCfg();
            _data.LoadTransportMechanism ??= new TransportMechanismPPCfg();
            _data.UnloadTransportMechanism ??= new TransportMechanismPPCfg();
            _data.FeedingPortPPCfg ??= new MaterialBoxSubMachineModules.FeedPort.FeedPortPPCfg();
            _data.ReceivePortPPCfg ??= new MaterialBoxSubMachineModules.FeedPort.FeedPortPPCfg();

            EnsureStorageCount(_data.LoadStorageDevice, 2);
            EnsureStorageCount(_data.UnloadStorageDevice, 2);

            _data.LoadStorageDevice.Storages[LoadUpperRack] = LoadUpperLayerBox.ToModel();
            _data.LoadStorageDevice.Storages[LoadLowerRack] = LoadLowerLayerBox.ToModel();
            _data.UnloadStorageDevice.Storages[0] = UnloadUpperLayerBox.ToModel();
            _data.UnloadStorageDevice.Storages[1] = UnloadLowerLayerBox.ToModel();

            LoadUpperInOutOperation.ApplyTo(_data.LoadTransportMechanism);
            UnloadUpperInOutOperation.ApplyTo(_data.UnloadTransportMechanism);
            OtherParameters.ApplyTo(_data);

            return _data;
        }

        /// <summary>订阅子视图模型的属性变化事件，统一触发页面修改通知。</summary>
        private void SubscribeNested()
        {
            LoadUpperLayerBox.PropertyChanged += (_, _) => RaiseModified();
            LoadLowerLayerBox.PropertyChanged += (_, _) => RaiseModified();
            UnloadUpperLayerBox.PropertyChanged += (_, _) => RaiseModified();
            UnloadLowerLayerBox.PropertyChanged += (_, _) => RaiseModified();

            LoadUpperInOutOperation.PropertyChanged += (_, _) => RaiseModified();
            LoadLowerInOutOperation.PropertyChanged += (_, _) => RaiseModified();
            UnloadUpperInOutOperation.PropertyChanged += (_, _) => RaiseModified();
            UnloadLowerInOutOperation.PropertyChanged += (_, _) => RaiseModified();

            OtherParameters.PropertyChanged += (_, _) => RaiseModified();
        }

        private static void EnsureStorageCount(StorageDevicePPCfg storageDevice, int count)
        {
            storageDevice.Storages ??= new List<StorageRecipeParameters>();
            while (storageDevice.Storages.Count < count)
            {
                storageDevice.Storages.Add(new StorageRecipeParameters());
            }

            if (storageDevice.Storages.Count > count)
            {
                storageDevice.Storages = storageDevice.Storages.Take(count).ToList();
            }
        }

        private static HashSet<int> ParseDisabledSlotIndexes(string? indexesText, string? countText, int slotCount)
        {
            var result = new HashSet<int>();
            if (slotCount <= 0)
                return result;

            foreach (var token in (indexesText ?? string.Empty).Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                if (int.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out var slotNo) && slotNo >= 1 && slotNo <= slotCount)
                    result.Add(slotNo);
            }

            if (result.Count > 0)
                return result;

            var disabledCount = Math.Max(0, ParseInt(countText, 0));
            for (var i = 1; i <= Math.Min(disabledCount, slotCount); i++)
                result.Add(i);

            return result;
        }

        /// <summary>触发页面已修改事件。</summary>
        private void RaiseModified() => AfterModified?.Invoke(this, EventArgs.Empty);

        /// <summary>根据首槽与末槽高度重新计算槽间距。</summary>
        private void CalculateSpacing(MaterialBoxLayerViewModel layer, int rack)
        {
            var slotNums = ParseInt(layer.SlotCountViewModel.Text, 0);
            var first = ParseDouble(layer.FirstSlotPcbHeightMmViewModel.Text, 0);
            var last = ParseDouble(layer.LastSlotPcbHeightMmViewModel.Text, first);
            var pitch = slotNums > 1 ? Math.Abs(first - last) / (slotNums - 1) : 0;
            layer.SlotPitchMmViewModel.Text = pitch.ToString("0.###", CultureInfo.InvariantCulture);
            RaiseModified();
        }

        /// <summary>读取当前位置，并把它写入首槽高度或末槽高度输入框。</summary>
        private void TeachFirstSlot(MaterialBoxLayerViewModel layer, MaterialBoxInOutOperationViewModel operation, int rack, bool isFirstSlot)
        {
            RefreshAxisPosition(operation, rack);
            var currentPosition = operation.CurrentPositionViewModel.Text ?? string.Empty;
            if (isFirstSlot)
                layer.FirstSlotPcbHeightMmViewModel.Text = currentPosition;
            else
                layer.LastSlotPcbHeightMmViewModel.Text = currentPosition;

            RaiseModified();
        }

        /// <summary>按首槽或末槽位置执行定位运动，并刷新当前位置显示。</summary>
        private async Task MoveToSlot(MaterialBoxLayerViewModel layer, MaterialBoxInOutOperationViewModel operation, int rack, bool firstSlot)
        {
            try
            {
                var param = CreateMotionParam(rack);
                var targetText = firstSlot
                    ? layer.FirstSlotPcbHeightMmViewModel.Text
                    : layer.LastSlotPcbHeightMmViewModel.Text;

                var target = ParseDouble(targetText, 0);
                param["Pos"] = new GriffinsBaseValue(target.ToString(CultureInfo.InvariantCulture));
                param["Position"] = new GriffinsBaseValue(target.ToString(CultureInfo.InvariantCulture));
                await Task.Run(() => ExecuteRuntimeCommand(CmdMoveTo, param));
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"MoveToSlotAsync 失败: {ex.Message}");
            }
        }

        /// <summary>执行上移或下移动作，由后端位置事件实时刷新界面。</summary>
        private void MoveStep(MaterialBoxInOutOperationViewModel operation, int rack, bool isUp)
        {
            var param = CreateMotionParam(rack);
            var step = operation.EnableStepDistance
                ? ParseDouble(operation.StepDistanceMmViewModel.Text, 1)
                : -1;
            param["Step"] = new GriffinsBaseValue(step.ToString(CultureInfo.InvariantCulture));
            ExecuteRuntimeCommand(isUp ? CmdZMoveUp : CmdZMoveDown, param);

            if (operation.EnableStepDistance)
            {
                ClearContinuousMovingRack(rack);
                return;
            }

            MarkContinuousMovingRack(rack);
        }

        /// <summary>停止当前料盒对应的 Z 轴，并刷新当前位置。</summary>
        private void StopAxis(MaterialBoxInOutOperationViewModel operation, int rack)
        {
            ExecuteRuntimeCommand(CmdZAxisStop, CreateZAxisParam(rack));
            ClearContinuousMovingRack(rack);
            RefreshAxisPosition(operation, rack);
        }

        /// <summary>控制指定料盒执行夹紧或张开动作。</summary>
        private void ControlClamp(int rack, bool clamp)
        {
            ExecuteRuntimeCommand(clamp ? CmdMagazineClamp : CmdMagazineUnclamp, CreateCommonParam(rack));
        }

        /// <summary>读取后端当前位置并刷新到界面显示。</summary>
        private void RefreshAxisPosition(MaterialBoxInOutOperationViewModel operation, int rack)
        {
            if (!TryReadAxisPositionText(rack, out string positionText))
                return;

            UpdateAxisPositionText(operation, positionText);
        }

        /// <summary>
        /// 从运行时命令中读取当前位置显示文本。
        /// </summary>
        private bool TryReadAxisPositionText(int rack, out string positionText)
        {
            positionText = string.Empty;

            var result = ExecuteRuntimeCommand(CmdGetAxisPos, CreateZAxisParam(rack));
            var data = TryGetString(result, "data");
            if (string.IsNullOrWhiteSpace(data))
                return false;

            var parts = data.Split(',');
            if (parts.Length < 1)
                return false;

            positionText = parts[0].Trim();
            return !string.IsNullOrWhiteSpace(positionText);
        }

        /// <summary>
        /// 更新界面上的当前位置。
        /// </summary>
        private void UpdateAxisPositionText(MaterialBoxInOutOperationViewModel operation, string positionText)
        {
            Dispatcher.UIThread.Post(() =>
            {
                operation.CurrentPositionViewModel.Text = positionText;
            });
        }

        /// <summary>
        /// 页面关闭时取消所有位置轮询，并停止仍处于连续运动状态的轴。
        /// </summary>
        public void Cleanup()
        {
            int[] continuousRacks;
            lock (_continuousMovingLock)
            {
                continuousRacks = System.Linq.Enumerable.ToArray(_continuousMovingRacks);
                _continuousMovingRacks.Clear();
            }

            foreach (int rack in continuousRacks)
                ExecuteRuntimeCommand(CmdZAxisStop, CreateZAxisParam(rack));
        }

        /// <summary>
        /// 记录当前料盒已经进入未勾选步距时的连续运动状态。
        /// </summary>
        private void MarkContinuousMovingRack(int rack)
        {
            lock (_continuousMovingLock)
            {
                _continuousMovingRacks.Add(rack);
            }
        }

        /// <summary>
        /// 清理当前料盒的连续运动状态记录。
        /// </summary>
        private void ClearContinuousMovingRack(int rack)
        {
            lock (_continuousMovingLock)
            {
                _continuousMovingRacks.Remove(rack);
            }
        }

        private void RegisterAxisStatusInformProcessDelegate()
        {
            ClientInfoProcessRegister.RegisterSvrInfoProcessDelegate<InformInfo_StatusChanged>(OnCompStatusChanged);
        }

        private void UnRegisterAxisStatusInformProcessDelegate()
        {
            ClientInfoProcessRegister.UnRegisterInfoProcessDelegate(
                InformInfo_StatusChanged.InfoKindID,
                OnCompStatusChanged);
        }

        private void OnCompStatusChanged(GriffinsInfoKindID infoKind, string infoNo, InformInfoBase info)
        {
            if (info is InformInfo_StatusChanged informInfo)
            {
                Dispatcher.UIThread.Post(() => ProcessBackendStatusChange(informInfo));
            }
        }

        private void ProcessBackendStatusChange(InformInfo_StatusChanged info)
        {
            if (string.IsNullOrWhiteSpace(info.Param))
                return;

            MaterialBoxAxisStatus status;
            try
            {
                status = JsonConvert.DeserializeObject<MaterialBoxAxisStatus>(info.Param);
            }
            catch
            {
                return;
            }

            if (status == null || status.Staus != 0)
                return;

            if (string.Equals(status.Type, LoadAxisInformType, StringComparison.Ordinal))
            {
                RefreshAxisPositionGroup(
                    LoadUpperRack,
                    LoadUpperInOutOperation,
                    LoadLowerInOutOperation,
                    status.Position);
                return;
            }

            if (string.Equals(status.Type, UnloadAxisInformType, StringComparison.Ordinal))
            {
                RefreshAxisPositionGroup(
                    UnloadUpperRack,
                    UnloadUpperInOutOperation,
                    UnloadLowerInOutOperation,
                    status.Position);
            }
        }

        private void RefreshAllAxisPositions()
        {
            RefreshAxisPositionGroup(LoadUpperRack, LoadUpperInOutOperation, LoadLowerInOutOperation);
            RefreshAxisPositionGroup(UnloadUpperRack, UnloadUpperInOutOperation, UnloadLowerInOutOperation);
        }

        private void RefreshAxisPositionGroup(
            int rack,
            MaterialBoxInOutOperationViewModel first,
            MaterialBoxInOutOperationViewModel second,
            double? informPosition = null)
        {
            string positionText;
            if (informPosition.HasValue)
                positionText = informPosition.Value.ToString("0.###", CultureInfo.InvariantCulture);
            else if (!TryReadAxisPositionText(rack, out positionText))
                return;

            UpdateAxisPositionGroup(first, second, positionText);
        }

        private void UpdateAxisPositionGroup(
            MaterialBoxInOutOperationViewModel first,
            MaterialBoxInOutOperationViewModel second,
            string positionText)
        {
            UpdateAxisPositionText(first, positionText);
            UpdateAxisPositionText(second, positionText);
        }

        /// <summary>创建通用运行时命令参数，包含料盒编号、轴选择与夹料气缸索引。</summary>
        private GFBaseTypeParamValueList CreateCommonParam(int rack)
        {
            var param = new GFBaseTypeParamValueList();
            param["MaterialRack"] = new GriffinsBaseValue(rack.ToString(CultureInfo.InvariantCulture));
            param["ZAxisSelect"] = new GriffinsBaseValue(GetZAxisSelect(rack).ToString(CultureInfo.InvariantCulture));
            param["CylinderIndex"] = new GriffinsBaseValue(GetCylinderIndex(rack).ToString(CultureInfo.InvariantCulture));
            return param;
        }

        /// <summary>创建仅针对 Z 轴相关命令的参数对象。</summary>
        private GFBaseTypeParamValueList CreateZAxisParam(int rack)
        {
            var param = new GFBaseTypeParamValueList();
            param["MaterialRack"] = new GriffinsBaseValue(rack.ToString(CultureInfo.InvariantCulture));
            param["ZAxisSelect"] = new GriffinsBaseValue(GetZAxisSelect(rack).ToString(CultureInfo.InvariantCulture));
            return param;
        }

        /// <summary>创建运动命令参数，补充速度与加速度配置。</summary>
        private GFBaseTypeParamValueList CreateMotionParam(int rack)
        {
            var param = CreateCommonParam(rack);
            var isLoadRack = IsLoadRack(rack);
            var speedText = isLoadRack
                ? OtherParameters.UpperZAxisSpeedMmSViewModel.Text
                : OtherParameters.LowerZAxisSpeedMmSViewModel.Text;
            var accelerationText = isLoadRack
                ? OtherParameters.UpperZAxisAccelerationMmSSViewModel.Text
                : OtherParameters.LowerZAxisAccelerationMmSSViewModel.Text;

            var speed = ParseDouble(speedText, 1);
            var acc = ParseDouble(accelerationText, Math.Max(1.0, speed / 10.0));
            param["MaxSpeed"] = new GriffinsBaseValue(speed.ToString(CultureInfo.InvariantCulture));
            param["Acc"] = new GriffinsBaseValue(acc.ToString(CultureInfo.InvariantCulture));
            return param;
        }

        /// <summary>根据料盒编号计算前后端约定的 ZAxisSelect 值。</summary>
        private static int GetZAxisSelect(int rack) => IsLoadRack(rack) ? 0 : 1;

        /// <summary>根据料盒编号计算储料位内的夹料气缸索引（上层=0，下层=1）。</summary>
        private static int GetCylinderIndex(int rack)
        {
            return rack == LoadUpperRack || rack == UnloadUpperRack ? 0 : 1;
        }

        /// <summary>判断当前料盒编号是否属于上料区域。</summary>
        private static bool IsLoadRack(int rack) => rack == LoadUpperRack || rack == LoadLowerRack;

        /// <summary>执行运行时命令，优先走普通控制命令，失败后回退到配置服务命令。</summary>
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

            RaiseModified();
            return result;
        }

        /// <summary>安全读取命令返回值中的字符串字段。</summary>
        private static string TryGetString(GFBaseTypeParamValueList param, string key)
        {
            try
            {
                return param?[key]?.ToString() ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>把文本安全转换成整数，非法内容返回默认值。</summary>
        private static int ParseInt(string? text, int defaultValue)
        {
            if (int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value))
                return value;

            return defaultValue;
        }

        /// <summary>把文本安全转换成浮点数，非法内容返回默认值。</summary>
        private static double ParseDouble(string? text, double defaultValue)
        {
            if (double.TryParse(text, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var value))
                return value;

            return defaultValue;
        }
    }

    /// <summary>单层料盒参数的视图模型。</summary>
    public class MaterialBoxLayerViewModel : ReactiveObject
    {
        private string _disabledSlotIndexes = string.Empty;

        public TextInputViewModel SlotCountViewModel { get; } = new();
        public TextInputViewModel SlotPitchMmViewModel { get; } = new();
        public TextInputViewModel FirstSlotPcbHeightMmViewModel { get; } = new();
        public TextInputViewModel LastSlotPcbHeightMmViewModel { get; } = new();
        public TextInputViewModel DisabledSlotCountViewModel { get; } = new();
        public TextInputViewModel AlarmSlotCountViewModel { get; } = new();

        public string DisabledSlotIndexes
        {
            get => _disabledSlotIndexes;
            set => this.RaiseAndSetIfChanged(ref _disabledSlotIndexes, value ?? string.Empty);
        }

        public void Load(StorageRecipeParameters model)
        {
            model ??= new StorageRecipeParameters();
            var disabledSlots = model.SlotList?
                .Select((slot, index) => new { slot, SlotNo = index + 1 })
                .Where(x => x.slot != null && !x.slot.IsEnabled)
                .Select(x => x.SlotNo)
                .ToList() ?? new List<int>();

            SlotCountViewModel.Text = Math.Max(0, model.SlotCount).ToString(CultureInfo.InvariantCulture);
            SlotPitchMmViewModel.Text = model.SlotSpacing.ToString("0.###", CultureInfo.InvariantCulture);
            FirstSlotPcbHeightMmViewModel.Text = model.FirstSlotPosition.ToString("0.###", CultureInfo.InvariantCulture);
            LastSlotPcbHeightMmViewModel.Text = model.LastSlotPosition.ToString("0.###", CultureInfo.InvariantCulture);
            DisabledSlotCountViewModel.Text = disabledSlots.Count.ToString(CultureInfo.InvariantCulture);
            DisabledSlotIndexes = string.Join(",", disabledSlots);
            AlarmSlotCountViewModel.Text = Math.Max(0, model.SlotWarningCount).ToString(CultureInfo.InvariantCulture);
        }

        public StorageRecipeParameters ToModel()
        {
            var slotCount = ParseInt(SlotCountViewModel.Text, 0);
            var first = ParseDouble(FirstSlotPcbHeightMmViewModel.Text, 0);
            var last = ParseDouble(LastSlotPcbHeightMmViewModel.Text, 0);
            var spacing = ParseDouble(SlotPitchMmViewModel.Text, 0);
            var warningCount = ParseInt(AlarmSlotCountViewModel.Text, 0);
            var disabledSlots = ParseDisabledSlotIndexes(DisabledSlotIndexes, DisabledSlotCountViewModel.Text, slotCount);
            var slotList = new List<StorageSlotRecipeParameters>();

            for (var i = 0; i < slotCount; i++)
            {
                slotList.Add(new StorageSlotRecipeParameters
                {
                    IsEnabled = !disabledSlots.Contains(i + 1)
                });
            }

            return new StorageRecipeParameters
            {
                SlotCount = slotCount,
                SlotSpacing = spacing,
                FirstSlotPosition = first,
                LastSlotPosition = last,
                SlotWarningCount = warningCount,
                SlotList = slotList,
            };
        }

        private static int ParseInt(string? text, int defaultValue)
        {
            return int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value)
                ? value
                : defaultValue;
        }

        private static double ParseDouble(string? text, double defaultValue)
        {
            return double.TryParse(text, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var value)
                ? value
                : defaultValue;
        }

        private static HashSet<int> ParseDisabledSlotIndexes(string? indexesText, string? countText, int slotCount)
        {
            var result = new HashSet<int>();
            if (slotCount <= 0)
                return result;

            foreach (var token in (indexesText ?? string.Empty).Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                if (int.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out var slotNo) && slotNo >= 1 && slotNo <= slotCount)
                    result.Add(slotNo);
            }

            if (result.Count > 0)
                return result;

            var disabledCount = Math.Max(0, ParseInt(countText, 0));
            for (var i = 1; i <= Math.Min(disabledCount, slotCount); i++)
                result.Add(i);

            return result;
        }
    }

    /// <summary>单个进出料操作区域的视图模型。</summary>
    public class MaterialBoxInOutOperationViewModel : ReactiveObject
    {
        public ComboxViewModel AxisNameViewModel { get; } = new();
        public TextInputViewModel CurrentPositionViewModel { get; } = new() { IsEnabled = false };
        public TextInputViewModel StepDistanceMmViewModel { get; } = new();

        private bool _enableStepDistance;

        public bool EnableStepDistance
        {
            get => _enableStepDistance;
            set => this.RaiseAndSetIfChanged(ref _enableStepDistance, value);
        }

        public MaterialBoxInOutOperationViewModel(string defaultAxisName)
        {
            AxisNameViewModel.DisplayMemberPath = string.Empty;
            AxisNameViewModel.SelectedItem = null;
        }

        public void InitializeAxisOptions(IReadOnlyList<string> axisOptions) => AxisNameViewModel.ItemsSource = axisOptions;

        public void Load(TransportMechanismPPCfg model, string defaultAxisName)
        {
            model ??= new TransportMechanismPPCfg();
            AxisNameViewModel.SelectedItem = defaultAxisName;
            CurrentPositionViewModel.Text ??= string.Empty;
            EnableStepDistance = model.UseStepDistance;
            StepDistanceMmViewModel.Text = model.StepDistance.ToString("0.###", CultureInfo.InvariantCulture);
        }

        public void ApplyTo(TransportMechanismPPCfg model)
        {
            model ??= new TransportMechanismPPCfg();
            model.UseStepDistance = EnableStepDistance;
            model.StepDistance = ParseDouble(StepDistanceMmViewModel.Text, 0.5);
        }

        private static double ParseDouble(string? text, double defaultValue)
        {
            return double.TryParse(text, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var value)
                ? value
                : defaultValue;
        }
    }

    /// <summary>其它工艺参数区域的视图模型。</summary>
    public class MaterialBoxOtherParametersViewModel : ReactiveObject
    {
        public TextInputViewModel UpperZAxisSpeedMmSViewModel { get; } = new();
        public TextInputViewModel UpperZAxisAccelerationMmSSViewModel { get; } = new();
        public TextInputViewModel LowerZAxisSpeedMmSViewModel { get; } = new();
        public TextInputViewModel LowerZAxisAccelerationMmSSViewModel { get; } = new();
        public TextInputViewModel PushRodAxisSpeedMmSViewModel { get; } = new();
        public TextInputViewModel FeedDetectTimeMsViewModel { get; } = new();
        public TextInputViewModel MaterialInPlaceSenseTimeMsViewModel { get; } = new();
        public TextInputViewModel PushDistanceMmViewModel { get; } = new();
        public TextInputViewModel MaterialClampSpeedMmSViewModel { get; } = new();
        public TextInputViewModel ContinuousPushCountViewModel { get; } = new();
        public TextInputViewModel SolenoidValveDelayMsViewModel { get; } = new();
        public TextInputViewModel SensorResponseTimeMsViewModel { get; } = new();
        public TextInputViewModel ManualOperationSpeedMmSViewModel { get; } = new();
        public TextInputViewModel CylinderSignalDetectTimeMsViewModel { get; } = new();
        public TextInputViewModel PushRodBackHomeTimeoutMsViewModel { get; } = new();
        public TextInputViewModel SignalDetectTimeMsViewModel { get; } = new();
        public TextInputViewModel PushMaterialSignalDetectTimeMsViewModel { get; } = new();
        public TextInputViewModel PushAxisSpeedMmSViewModel { get; } = new();
        public TextInputViewModel PushAxisMoveTimeMsViewModel { get; } = new();

        public void Load(BackendMaterialBoxPPCfg model)
        {
            model ??= new BackendMaterialBoxPPCfg();
            model.LoadTransportMechanism ??= new TransportMechanismPPCfg();
            model.UnloadTransportMechanism ??= new TransportMechanismPPCfg();
            model.FeedingPortPPCfg ??= new MaterialBoxSubMachineModules.FeedPort.FeedPortPPCfg();
            model.ReceivePortPPCfg ??= new MaterialBoxSubMachineModules.FeedPort.FeedPortPPCfg();
            var loadAxisMotion = model.LoadTransportMechanism.AxisMotionParameters;
            var unloadAxisMotion = model.UnloadTransportMechanism.AxisMotionParameters;

            UpperZAxisSpeedMmSViewModel.Text = loadAxisMotion.MaxSpeed.ToString("0.###", CultureInfo.InvariantCulture);
            UpperZAxisAccelerationMmSSViewModel.Text = loadAxisMotion.Acceleration.ToString("0.###", CultureInfo.InvariantCulture);
            LowerZAxisSpeedMmSViewModel.Text = unloadAxisMotion.MaxSpeed.ToString("0.###", CultureInfo.InvariantCulture);
            LowerZAxisAccelerationMmSSViewModel.Text = unloadAxisMotion.Acceleration.ToString("0.###", CultureInfo.InvariantCulture);
            MaterialInPlaceSenseTimeMsViewModel.Text = (model.FeedingPortPPCfg.MaterialArrivedSenseTime > 0 ? model.FeedingPortPPCfg.MaterialArrivedSenseTime : model.ReceivePortPPCfg.MaterialArrivedSenseTime).ToString("0.###", CultureInfo.InvariantCulture);
        }

        public void ApplyTo(BackendMaterialBoxPPCfg model)
        {
            model ??= new BackendMaterialBoxPPCfg();
            model.LoadTransportMechanism ??= new TransportMechanismPPCfg();
            model.UnloadTransportMechanism ??= new TransportMechanismPPCfg();
            model.FeedingPortPPCfg ??= new MaterialBoxSubMachineModules.FeedPort.FeedPortPPCfg();
            model.ReceivePortPPCfg ??= new MaterialBoxSubMachineModules.FeedPort.FeedPortPPCfg();
            var loadAxisMotion = model.LoadTransportMechanism.AxisMotionParameters;
            var unloadAxisMotion = model.UnloadTransportMechanism.AxisMotionParameters;

            loadAxisMotion.MaxSpeed = ParseDouble(UpperZAxisSpeedMmSViewModel.Text, 0);
            loadAxisMotion.Acceleration = ParseDouble(UpperZAxisAccelerationMmSSViewModel.Text, 0);
            unloadAxisMotion.MaxSpeed = ParseDouble(LowerZAxisSpeedMmSViewModel.Text, 0);
            unloadAxisMotion.Acceleration = ParseDouble(LowerZAxisAccelerationMmSSViewModel.Text, 0);

            model.LoadTransportMechanism.AxisMotionParameters = loadAxisMotion;
            model.UnloadTransportMechanism.AxisMotionParameters = unloadAxisMotion;

            var materialSenseTime = ParseDouble(MaterialInPlaceSenseTimeMsViewModel.Text, 0);
            model.FeedingPortPPCfg.MaterialArrivedSenseTime = materialSenseTime;
            model.ReceivePortPPCfg.MaterialArrivedSenseTime = materialSenseTime;
        }

        private static double ParseDouble(string? text, double defaultValue)
        {
            return double.TryParse(text, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var value)
                ? value
                : defaultValue;
        }
    }

}

