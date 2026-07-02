using GKG.Map.LoadUnloadFuncCtlMapCell.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace GKG.Map.LoadUnloadFuncCtlMapCell.ViewModels
{
    /// <summary>
    /// 上下料图元主视图模型 (ViewModel)
    /// 统管所有容器与料盒的实时状态，负责 UI 逻辑处理，并通过事件向外抛出硬件执行意图。
    /// </summary>
    public class LoadUnloadViewModel : ReactiveObject
    {
        #region 字段依赖

        private readonly LoadUnloadPropertyModelEdit _propertyModelEdit;

        #endregion

        #region 数据集合

        /// <summary>
        /// 当前图元挂载的所有容器集合 (用于 UI 列表渲染)
        /// </summary>
        public ObservableCollection<StorageContainerViewModel> StorageContainers { get; } = new();

        #endregion

        #region 交互控制命令 (Command Bus - 供 View 绑定)

        /// <summary>
        /// 纯 UI 交互：一键展开/折叠所有容器
        /// </summary>
        public ReactiveCommand<Unit, Unit> ToggleAllContainersCommand { get; }

        /// <summary>
        /// 物理交互：夹紧指定料盒
        /// </summary>
        public ReactiveCommand<MagazineViewModel, Unit> ClampMagazineCommand { get; }

        /// <summary>
        /// 物理交互：松开指定料盒
        /// </summary>
        public ReactiveCommand<MagazineViewModel, Unit> UnclampMagazineCommand { get; }

        /// <summary>
        /// 物理交互：上料一次
        /// </summary>
        public ReactiveCommand<Unit, Unit> LoadCommand { get; }

        /// <summary>
        /// 物理交互：下料一次
        /// </summary>
        public ReactiveCommand<Unit, Unit> UnloadCommand { get; }

        #endregion

        #region 对外抛出的硬件指令请求事件 (供 Controller 订阅)

        /// <summary>
        /// 请求夹紧料盒：参数为 (ContainerName, MagazineName)
        /// </summary>
        public event Func<string, string, Task>? OnClampRequested;

        /// <summary>
        /// 请求松开料盒：参数为 (ContainerName, MagazineName)
        /// </summary>
        public event Func<string, string, Task>? OnUnclampRequested;

        /// <summary>
        /// 请求上料一次
        /// </summary>
        public event Func<Task>? OnLoadRequested;

        /// <summary>
        /// 请求下料一次
        /// </summary>
        public event Func<Task>? OnUnloadRequested;

        #endregion

        /// <summary>
        /// 实例化上下料视图模型
        /// </summary>
        /// <param name="propertyModelEdit">底层图元数据编辑模型</param>
        public LoadUnloadViewModel(LoadUnloadPropertyModelEdit propertyModelEdit)
        {
            _propertyModelEdit = propertyModelEdit ?? throw new ArgumentNullException(nameof(propertyModelEdit));

            // UI 纯视觉交互命令
            ToggleAllContainersCommand = ReactiveCommand.Create(ExecuteToggleAllContainers);

            // 利用 CreateFromTask 包装异步操作，实现硬件请求的物理级防抖锁定
            ClampMagazineCommand = ReactiveCommand.CreateFromTask<MagazineViewModel>(ExecuteClampMagazineAsync);
            UnclampMagazineCommand = ReactiveCommand.CreateFromTask<MagazineViewModel>(ExecuteUnclampMagazineAsync);
            LoadCommand = ReactiveCommand.CreateFromTask(ExecuteLoadAsync);
            UnloadCommand = ReactiveCommand.CreateFromTask(ExecuteUnloadAsync);

            // 全局命令异常流接管 (防止未捕获异常导致程序崩溃，统一输出日志)
            Observable.Merge(
                ClampMagazineCommand.ThrownExceptions,
                UnclampMagazineCommand.ThrownExceptions,
                LoadCommand.ThrownExceptions,
                UnloadCommand.ThrownExceptions
            )
            .ObserveOn(RxApp.MainThreadScheduler) // 确保在主线程操作
            .Subscribe(ex =>
            {
                // 统一异常提示出口，未来可接管至全局通知系统
                System.Diagnostics.Debug.WriteLine($"[UI命令异常流捕获]: {ex.Message}");
            });
        }

        #region 命令执行逻辑 (业务核心与防抖锁定)

        /// <summary>
        /// 切换所有容器的折叠/展开状态
        /// </summary>
        private void ExecuteToggleAllContainers()
        {
            if (!StorageContainers.Any()) return;

            // 交互反转逻辑：只要有任意容器展开，首要操作就是“全部折叠”
            bool targetState = !StorageContainers.Any(c => c.IsExpanded);

            foreach (var container in StorageContainers)
            {
                container.IsExpanded = targetState;
            }
        }

        /// <summary>
        /// 执行夹紧动作
        /// </summary>
        private async Task ExecuteClampMagazineAsync(MagazineViewModel mag)
        {
            if (mag == null || !mag.CanClamp) return;

            var parentContainer = StorageContainers.FirstOrDefault(c => c.Magazines.Contains(mag));
            string containerName = parentContainer?.Name ?? "未知容器";

            try
            {
                // 向图元对象抛出异步请求，并等待其真实执行完毕
                if (OnClampRequested != null)
                {
                    await OnClampRequested.Invoke(containerName, mag.Name);
                }

                // 物理级防抖锁定 (命令完成或失败后，强制 500ms 内按钮保持变灰状态，阻断连点)
                await Task.Delay(500);
            }
            catch (Exception ex)
            {
                throw new Exception($"夹紧指令下发失败 | 容器: {containerName}, 料盒: {mag.Name} | 原因: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 执行松开动作
        /// </summary>
        private async Task ExecuteUnclampMagazineAsync(MagazineViewModel mag)
        {
            if (mag == null || !mag.CanUnclamp) return;

            var parentContainer = StorageContainers.FirstOrDefault(c => c.Magazines.Contains(mag));
            string containerName = parentContainer?.Name ?? "未知容器";

            try
            {
                if (OnUnclampRequested != null)
                {
                    await OnUnclampRequested.Invoke(containerName, mag.Name);
                }

                await Task.Delay(500);
            }
            catch (Exception ex)
            {
                throw new Exception($"松开指令下发失败 | 容器: {containerName}, 料盒: {mag.Name} | 原因: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 执行上料一次动作
        /// </summary>
        private async Task ExecuteLoadAsync()
        {
            try
            {
                if (OnLoadRequested != null)
                {
                    await OnLoadRequested.Invoke();
                }

                await Task.Delay(500);
            }
            catch (Exception ex)
            {
                throw new Exception($"上料一次指令下发失败 | 原因: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 执行下料一次动作
        /// </summary>
        private async Task ExecuteUnloadAsync()
        {
            try
            {
                if (OnUnloadRequested != null)
                {
                    await OnUnloadRequested.Invoke();
                }

                await Task.Delay(500);
            }
            catch (Exception ex)
            {
                throw new Exception($"下料一次指令下发失败 | 原因: {ex.Message}", ex);
            }
        }

        #endregion

        #region 数据解析与 UI 状态同步

        /// <summary>
        /// 解析后端字典数据，并无损更新前端 UI 集合 
        /// (采用双级 Upsert 保证 UI 引用不丢失，防止闪烁)
        /// </summary>
        /// <param name="materialContainers">后端推送或拉取的容器状态字典</param>
        private void ExecuteDataSync(Dictionary<string, MaterialContainerStatus> materialContainers)
        {
            // 防护大门：判断字典是否为空
            if (materialContainers == null || materialContainers.Count == 0) return;

            try
            {
                foreach (var containerKvp in materialContainers)
                {
                    var backContainer = containerKvp.Value;
                    if (backContainer == null) continue;

                    // 一级 Upsert：同步 StorageContainerViewModel
                    var frontContainer = StorageContainers.FirstOrDefault(c => c.Name == backContainer.Name);
                    if (frontContainer == null)
                    {
                        frontContainer = new StorageContainerViewModel
                        {
                            Name = backContainer.Name,
                            IsExpanded = true
                        };
                        StorageContainers.Add(frontContainer);
                    }

                    if (backContainer.MaterialBoxes == null || backContainer.MaterialBoxes.Count == 0) continue;

                    foreach (var boxKvp in backContainer.MaterialBoxes)
                    {
                        var backBox = boxKvp.Value;
                        if (backBox == null) continue;

                        // 二级 Upsert：同步 MagazineViewModel
                        var frontBox = frontContainer.Magazines.FirstOrDefault(m => m.Name == backBox.Name);
                        if (frontBox == null)
                        {
                            frontBox = new MagazineViewModel { Name = backBox.Name };
                            frontContainer.Magazines.Add(frontBox);
                        }

                        // 同步料盒物理状态
                        frontBox.IsPresent = !backBox.IsEmpty;
                        frontBox.IsClamped = backBox.MaterialBoxCylinderStatus;

                        // 同步料盒内部槽位状态
                        if (frontBox.IsPresent && backBox.SlotStatusList != null && backBox.SlotStatusList.Count > 0)
                        {
                            // 核心架构适配：解析最新的字典嵌套列表结构 Dictionary<string, SlotStatuses>
                            // 使用 LINQ 的 SelectMany 将字典中所有的槽位集合平铺降维为一个完整的一维 slotList
                            var slotList = backBox.SlotStatusList.Values
                                .Where(v => v != null)
                                .SelectMany(v => v)
                                .ToList();

                            frontBox.Layers.Clear();

                            // 倒序渲染：底层在下 (Index 大)，高层在上 (Index 小)
                            for (int i = slotList.Count - 1; i >= 0; i--)
                            {
                                var backSlot = slotList[i];
                                if (backSlot == null) continue;

                                frontBox.Layers.Add(new LayerViewModel
                                {
                                    LayerIndex = i + 1,
                                    IsDisabled = (backSlot.MaterialStatus == MaterialStatus.Disable),
                                    IsOccupied = (backSlot.MaterialStatus == MaterialStatus.Full),
                                    IsEmpty = (backSlot.MaterialStatus == MaterialStatus.Empty)
                                });
                            }
                        }
                        else
                        {
                            // 防呆机制：如果料盒被拔出（不在位），将前端所有视觉槽位清空为“空槽”，消除视觉残留
                            foreach (var layer in frontBox.Layers)
                            {
                                layer.IsOccupied = false;
                                layer.IsEmpty = true;
                                layer.IsDisabled = false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"料盒状态解析与同步失败。原因：{ex.Message}");
            }
        }

        #endregion

        #region 公共 API：供外部 MapCellLoadUnloadCtlObj 调用

        /// <summary>
        /// 接收 MapCellLoadUnloadCtlObj 推送的模型状态，并安全地在 UI 线程执行同步
        /// </summary>
        /// <param name="materialContainers">需要同步的后端状态字典</param>
        public void SyncStatusFromBackend(Dictionary<string, MaterialContainerStatus> materialContainers)
        {
            RxApp.MainThreadScheduler.Schedule(() =>
            {
                ExecuteDataSync(materialContainers);
            });
        }

        #endregion
    }
}