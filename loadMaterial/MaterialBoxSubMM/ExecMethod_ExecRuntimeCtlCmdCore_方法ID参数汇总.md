# MaterialBoxSubMMCmdExecutor：`ExecMethod` / `ExecRuntimeCtlCmdCore` 方法 ID 与参数 ID 汇总

## 1. 结论先看

### `ExecMethod(string methodID, GFBaseTypeParamValueList param)`
- 入口位置：`GKG.MaterialBoxSubMachineModules/MaterialBoxSubMMCmdExecutor.cs:813`
- 分发顺序：
  1. `TryExecMaterialBoxMethod`
  2. `TryExecStorageMethod`
  3. `TryExecTransportMethod`
  4. `TryExecFeedPortMethod`
- 如果都不匹配，返回错误：`未识别的方法: {methodID}`

### `ExecRuntimeCtlCmdCore(string cmdID, GFBaseTypeParamValueList cmdParam)`
- 入口位置：`GKG.MaterialBoxSubMachineModules/MaterialBoxSubMMCmdExecutor.cs:885`
- 固定返回字段：
  - `Result`
  - `errorMsg`
  - `data`
- 分发顺序：
  1. `TryExecMaterialBoxRuntimeCmd`
  2. `TryExecTransportRuntimeCmd`
  3. `TryExecStorageRuntimeCmd`
- 如果命令不匹配或执行异常，统一写入 `errorMsg`

---

## 2. `ExecMethod` 中用到的所有方法 ID 与参数 ID

> 方法 ID 的实际字符串定义来自：`MaterialBoxSubMachineModulesConst.cs`

| 分类 | 常量名 | 实际方法 ID | 参数 ID | 说明 |
|---|---|---|---|---|
| 料盒 | `UpfeedMoveNextSlotMethodID` | `UpfeedMoveNextSlot` | `MaterialRack`（可缺省，默认 0）, `ZAxisSelect`（可缺省，默认 0）, `MaxSpeed`（可选）, `Acc`（可选）, `acc`（兼容旧写法，可选）, `CurrentSlotIndex`（可选） | 内部会转调运行时命令 `MoveToNextSlot`；上料方向 |
| 料盒 | `DownfeedMoveNextSlotMethodID` | `DownfeedMoveNextSlot` | `MaterialRack`（可缺省，默认 2）, `ZAxisSelect`（可缺省，默认 1）, `MaxSpeed`（可选）, `Acc`（可选）, `acc`（兼容旧写法，可选）, `CurrentSlotIndex`（可选） | 内部会转调运行时命令 `MoveToNextSlot`；下料方向 |
| 料盒 | `GetMaterialContainerStatusMethodID` | `GetMaterialContainerStatus` | 无 | 返回四个料盒位的整体状态 |
| 储料 | `ControlCylinderMethodID` | `ControlCylinder` | `MaterialRack`（必填）, `CylinderIndex`（可选）, `bState`（必填） | `bState=true` 伸出，`false` 缩回 |
| 储料 | `StorageResetMethodID` | `StorageReset` | `MaterialRack`（必填）, `CylinderIndex`（可选） | 复位指定储料位 |
| 储料 | `StorageResetStateMethodID` | `StorageResetState` | `MaterialRack`（必填） | 重置料盒状态 |
| 储料 | `StorageOpenMethodID` | `StorageOpen` | `MaterialRack`（必填）, `CylinderIndex`（可选） | 换料盒/张开动作 |
| 储料 | `StorageCloseMethodID` | `StorageClose` | `MaterialRack`（必填）, `CylinderIndex`（可选） | 夹紧/关闭动作 |
| 储料 | `StorageGetCountMethodID` | `StorageGetCount` | `MaterialRack`（必填） | 返回储料位数量 |
| 储料 | `StorageGetCurrentSlotStateMethodID` | `StorageGetCurrentSlotState` | `MaterialRack`（必填） | 返回当前槽位状态 |
| 储料 | `StorageGetAvailableSlotsMethodID` | `StorageGetAvailableSlots` | `MaterialRack`（必填） | 返回可用槽位列表 |
| 储料 | `StorageGetSlotMaterialStateMethodID` | `StorageGetSlotMaterialState` | `MaterialRack`（必填）, `SlotIndex`（必填） | 查询指定槽位有/无料 |
| 储料 | `StorageSetSlotMaterialStateMethodID` | `StorageSetSlotMaterialState` | `MaterialRack`（必填）, `SlotIndex`（必填）, `HasMaterial`（必填） | 写指定槽位有/无料 |
| 储料 | `StorageGetInitialPositionMethodID` | `StorageGetInitialPosition` | `MaterialRack`（必填） | 获取料盒初始位置 |
| 运输 | `TransportMoveMethodID` | `TransportMove` | `ZAxisSelect` 或 `MaterialRack`（二选一用于选轴）, `Position` 或 `Pos`（目标位置，至少一个）, `MaxSpeed`（可选）, `Acc`（可选）, `acc`（兼容旧写法，可选） | 普通方法里的绝对移动 |
| 运输 | `TransportGetCurrentPositionMethodID` | `TransportGetCurrentPosition` | `ZAxisSelect` 或 `MaterialRack`（二选一） | 获取当前运输轴位置 |
| 运输 | `TransportGetMotionParametersMethodID` | `TransportGetMotionParameters` | `ZAxisSelect` 或 `MaterialRack`（二选一） | 获取当前运动参数 |
| 送料/接料口 | `GetMaterialStateMethodID` | `GetFeedPortMaterialState` | `Type`（必填）, `Index`（必填） | `Type` 选择送料口/接料口，`Index` 选择传感器索引 |

### `ExecMethod` 相关补充

1. `MaterialRack` 取值含义：
   - `0` = 上料上层
   - `1` = 上料下层
   - `2` = 下料上层
   - `3` = 下料下层

2. `ZAxisSelect` 取值含义：
   - `0` = 上料 Z 轴
   - `1` = 下料 Z 轴

3. 轴选择优先级：
   - 先读 `ZAxisSelect`
   - 如果没有，再尝试由 `MaterialRack` 推导

4. 关于 `Acc` / `acc`：
   - 代码里存在大小写混用：
     - `ResolveTransportAcceleration(...)` 读的是 `Acc`
     - `TryCreateTransportRequestContext(...)` 读的是 `acc`
   - 所以从“代码实际行为”看，两种写法都被用到了；如果要统一协议，建议后续只保留一种。

---

## 3. `ExecRuntimeCtlCmdCore` 中用到的所有运行时命令 ID 与参数 ID

| 分类 | 常量名 | 实际命令 ID | 参数 ID | 说明 |
|---|---|---|---|---|
| 料盒运行时 | `RtCmdMoveToFirstSlot` | `RtCmdMoveToFirstSlot` | `MaterialRack`（必填）, `ZAxisSelect` 或 `MaterialRack`（选轴）, `MaxSpeed`（可选）, `Acc`（可选）, `acc`（兼容旧写法，可选） | 移动到首槽位置 |
| 料盒运行时 | `RtCmdMoveToLastSlot` | `RtCmdMoveToLastSlot` | `MaterialRack`（必填）, `ZAxisSelect` 或 `MaterialRack`（选轴）, `MaxSpeed`（可选）, `Acc`（可选）, `acc`（兼容旧写法，可选） | 移动到末槽位置 |
| 料盒运行时 | `RtCmdMoveToNextSlot` | `MoveToNextSlot` | `MaterialRack`（必填）, `ZAxisSelect` 或 `MaterialRack`（选轴）, `MaxSpeed`（可选）, `Acc`（可选）, `acc`（兼容旧写法，可选）, `CurrentSlotIndex`（可选） | 找下一个符合条件的槽位并移动；`CurrentSlotIndex` 传入后会做 `-1` 处理 |
| 料盒运行时 | `RtCmdResetMaterialBoxState` | `ResetMaterialBoxState` | `MaterialRack`（必填） | 重置料盒状态 |
| 料盒运行时 | `RtCmdMoveToInitialPosition` | `MoveToInitialPosition` | `MaterialRack`（必填）, `ZAxisSelect` 或 `MaterialRack`（选轴）, `MaxSpeed`（可选）, `Acc`（可选）, `acc`（兼容旧写法，可选） | 回料盒初始位置 |
| 运输运行时 | `RtCmdMoveTo` | `MoveTo` | `MaterialRack`（必填）, `ZAxisSelect` 或 `MaterialRack`（选轴）, `SlotIndex` 或 `Pos` 或 `Position`（目标，三选一）, `MaxSpeed`（可选）, `Acc`（可选）, `acc`（兼容旧写法，可选） | 目标解析优先级：`SlotIndex` > `Pos` > `Position` |
| 运输运行时 | `RtCmdMagazineMotion` | `MagazineMotion` | `ZAxisSelect` 或 `MaterialRack`（选轴）, `Direction`（必填）, `MaxSpeed`（可选）, `Acc`（可选）, `acc`（兼容旧写法，可选） | 连续运动，方向由 `Direction` 正负决定 |
| 运输运行时 | `RtCmdZAxisStop` | `ZAxisStop` | `ZAxisSelect` 或 `MaterialRack`（选轴） | 停止当前连续运动 |
| 运输运行时 | `RtCmdZMoveUp` | `ZMoveUp` | `ZAxisSelect` 或 `MaterialRack`（选轴）, `MaxSpeed`（必填）, `Acc`（可选）, `Step`（可选） | `Step>0` 为相对位移，否则连续向上运动 |
| 运输运行时 | `RtCmdZMoveDown` | `ZMoveDown` | `ZAxisSelect` 或 `MaterialRack`（选轴）, `MaxSpeed`（必填）, `Acc`（可选）, `Step`（可选） | `Step>0` 为相对位移，否则连续向下运动 |
| 运输运行时 | `RtCmdGetAxisInfos` | `GetAxisInfos` | 无 | 返回全部轴信息 JSON |
| 运输运行时 | `RtCmdGetAxisPos` | `GetAxisPos` | `ZAxisSelect` 或 `MaterialRack`（选轴） | 返回当前轴位置 |
| 储料运行时 | `RtCmdMagazineClamp` | `MagazineClamp` | `MaterialRack`（必填）, `CylinderIndex`（可选） | 夹紧料盒 |
| 储料运行时 | `RtCmdMagazineUnclamp` | `MagazineUnclamp` | `MaterialRack`（必填）, `CylinderIndex`（可选） | 松开料盒 |
| 储料运行时 | `RtCmdGetIOInfos` | `GetIOInfos` | `MaterialRack`（可选） | 不传则返回全部 rack 关联 IO；传了则按 rack 过滤 |

### `ExecRuntimeCtlCmdCore` 相关补充

1. 统一返回格式固定为：
   - `Result`
   - `errorMsg`
   - `data`

2. `MoveTo` 与普通方法 `TransportMove` 的目标参数优先级不同：
   - `ExecRuntimeCtlCmdCore -> MoveTo`：`SlotIndex` > `Pos` > `Position`
   - `ExecMethod -> TransportMove`：`Position` > `Pos`

3. `CurrentSlotIndex` 的处理有一个细节：
   - 代码中会先读取传入值，再执行 `providedIndex - 1`
   - 也就是说它更像是“界面 1 基下标”转“内部 0 基下标”

---

## 4. 汇总后的“参数 ID 总表”

下面是这两个入口及其下游分支里实际出现过的参数 ID（只统计入参，不统计返回字段）：

- `MaterialRack`
- `ZAxisSelect`
- `MaxSpeed`
- `Acc`
- `acc`
- `CurrentSlotIndex`
- `CylinderIndex`
- `bState`
- `SlotIndex`
- `HasMaterial`
- `Position`
- `Pos`
- `Direction`
- `Step`
- `Type`
- `Index`

---

## 5. 源码定位

### 核心入口
- `GKG.MaterialBoxSubMachineModules/MaterialBoxSubMMCmdExecutor.cs:813` → `ExecMethod(...)`
- `GKG.MaterialBoxSubMachineModules/MaterialBoxSubMMCmdExecutor.cs:885` → `ExecRuntimeCtlCmdCore(...)`

### 方法 ID / 命令 ID 常量定义
- `MaterialBoxSubMachineModules/MaterialBoxSubMachineModulesConst.cs:83-122`
- `MaterialBoxSubMachineModules/MaterialBoxSubMachineModulesConst.cs:170-198`

---

## 6. 额外观察

这个文件里“方法 ID / 运行时命令 ID”已经比较清楚，但“参数 ID”有两个值得注意的地方：

1. **加速度参数大小写不统一**
   - 有的地方读 `Acc`
   - 有的地方读 `acc`
   - 这会让前端/调用方容易踩坑

2. **轴选择既能用 `ZAxisSelect`，也能用 `MaterialRack` 推导**
   - 这提升了兼容性
   - 但也意味着协议文档最好明确“优先级”和“推荐传法”

如果你要，我下一步可以直接继续帮你把这份文档再整理成：
- **更适合给前端/联调用的接口表**，或者
- **Excel/CSV 风格的字段清单**。