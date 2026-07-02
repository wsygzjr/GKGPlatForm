using GF_Gereric;
using GKG.ElectronicControl;
using GKG.SubMM.StorageDeviceModule;
using GKG.ElectronicControl.General;
using Griffins.PF.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace GKG.MaterialBoxSubMachineModules.Common
{
    /// <summary>
    /// 气缸事件参数，携带气缸索引
    /// </summary>
    public class CylinderEventArgs : EventArgs
    {
        public int CylinderIndex { get; private set; }

        public CylinderEventArgs(int cylinderIndex)
        {
            CylinderIndex = cylinderIndex;
        }
    }

    /// <summary>
    /// 储料装置事件参数，携带储料位索引
    /// </summary>
    public class StorageDeviceEventArgs : EventArgs
    {
        public int materialBoxIndex { get; private set; }

        public StorageDeviceEventArgs(int materialBoxIndex)
        {
            materialBoxIndex = materialBoxIndex;
        }
    }

    /// <summary>
    /// 储料装置气缸事件参数，同时携带储料位和气缸索引
    /// </summary>
    public class StorageDeviceCylinderEventArgs : CylinderEventArgs
    {
        public int materialBoxIndex { get; private set; }

        public StorageDeviceCylinderEventArgs(int materialBoxIndex, int cylinderIndex)
            : base(cylinderIndex)
        {
            materialBoxIndex = materialBoxIndex;
        }
    }

    /// <summary>
    /// 料槽
    /// </summary>
    public class Slot
    {
        /// <summary>
        /// 索引
        /// </summary>
        public double Index { get; set; }
        /// <summary>
        /// 位置
        /// </summary>
        public double Position { get; set; }
        /// <summary>
        /// 是否为空料槽
        /// </summary>
        public bool IsEmpty { get; set; } = false;
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnabled { get; set; } = true;
    }

    /// <summary>
    /// 料盒
    /// </summary>
    public class MaterialBox
    {
        /// <summary>
        /// 初始化参数
        /// </summary>
        private StorageMechanismInitCfg initCfg;

        /// <summary>
        /// 配方参数
        /// </summary>
        private StorageRecipeParameters ppCfg;

        /// <summary>
        /// IO 实例列表，按初始化参数中 IO Guid 列表顺序对应
        /// </summary>
        private List<IBaseStateIO> stateIOList;

        /// <summary>
        /// 气缸列表
        /// </summary>
        private List<IBaseCylinder> cylinderList;

        /// <summary>
        /// 气缸伸出完成事件，携带气缸索引
        /// </summary>
        public event EventHandler<CylinderEventArgs> StretchFinished;

        /// <summary>
        /// 气缸缩回完成事件，携带气缸索引
        /// </summary>
        public event EventHandler<CylinderEventArgs> RetractFinished;

        /// <summary>
        /// 索引
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// 储料位名称
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 当前储料位下的全部槽位运行时数据
        /// </summary>
        private List<Slot> Slots { get; set; } = new List<Slot>();

        /// <summary>
        /// 当前正在处理的槽位索引
        /// </summary>
        public int CurrentSlotIndex { get; set; }


        public MaterialBox(string name)
        {
            initCfg = new StorageMechanismInitCfg();
            Name = name ?? string.Empty;
        }

        /// <summary>
        /// 根据初始化参数绑定 IO 与气缸实例
        /// </summary>
        public void Init(StorageMechanismInitCfg initCfg, Func<List<Guid>,List<IBaseStateIO>> getIOStateList)
        {
            DetachCylinderEvents();
            this.initCfg = initCfg ?? new StorageMechanismInitCfg();
            stateIOList = new List<IBaseStateIO>();
            cylinderList = new List<IBaseCylinder>();
            foreach (CylinderInitParameters item in this.initCfg.CylinderInitParameters)
            {
                List<Guid> ioGuidList = item?.IOStateGuidList != null
                    ? new List<Guid>(item.IOStateGuidList)
                    : new List<Guid>();
                IBaseCylinder cylinder = CylinderFactory.CreateCylinder(item.eCylinderType);
                cylinder.SetStateIOInstanceList(getIOStateList(item.IOStateGuidList));
                cylinder.Init(JsonObjConvert.ToJSonBytes(item));
                cylinderList.Add(cylinder);
            }
            stateIOList.AddRange(getIOStateList(initCfg.SenserIOGuids));
            AttachCylinderEvents();
        }

        /// <summary>
        /// 应用储料装置配方
        /// </summary>
        public void ApplyRecipe(StorageRecipeParameters ppCfg)
        {
            this.ppCfg = ppCfg ?? new StorageRecipeParameters();
            Slots = new List<Slot>();
            double spacing = (ppCfg.LastSlotPosition - ppCfg.FirstSlotPosition) / (ppCfg.SlotCount <= 0 ? 1 : ppCfg.SlotCount);
            for (int i = 0; i < ppCfg.SlotCount; i++)
            {
                Slots.Add(new Slot
                {
                    Index = i,
                    Position = ppCfg.FirstSlotPosition + i * spacing,
                    IsEmpty = initCfg.IsFeeding ? false : true,
                    IsEnabled = ppCfg.SlotList != null && i < ppCfg.SlotList.Count ? ppCfg.SlotList[i]?.IsEnabled ?? true : true
                });
            }
        }

        /// <summary>
        /// 绑定气缸事件
        /// </summary>
        private void AttachCylinderEvents()
        {
            if (cylinderList == null)
                return;

            foreach (IBaseCylinder cylinder in cylinderList)
            {
                if (cylinder == null)
                    continue;

                cylinder.StretchFinished -= OnCylinderStretchFinished;
                cylinder.RetractFinished -= OnCylinderRetractFinished;
                cylinder.StretchFinished += OnCylinderStretchFinished;
                cylinder.RetractFinished += OnCylinderRetractFinished;
            }
        }
        /// <summary>
        /// 解绑气缸事件
        /// </summary>
        private void DetachCylinderEvents()
        {
            if (cylinderList == null)
                return;

            foreach (IBaseCylinder cylinder in cylinderList)
            {
                if (cylinder == null)
                    continue;

                cylinder.StretchFinished -= OnCylinderStretchFinished;
                cylinder.RetractFinished -= OnCylinderRetractFinished;
            }
        }

        /// <summary>
        /// 气缸伸出完成事件处理，抛出携带气缸索引的 StretchFinished 事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCylinderStretchFinished(object sender, EventArgs e)
        {
            StretchFinished?.Invoke(this, new CylinderEventArgs(GetCylinderIndex(sender)));
        }
        /// <summary>
        /// 气缸缩回完成事件处理，抛出携带气缸索引的 RetractFinished 事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCylinderRetractFinished(object sender, EventArgs e)
        {
            RetractFinished?.Invoke(this, new CylinderEventArgs(GetCylinderIndex(sender)));
        }
        /// <summary>
        /// 获取气缸索引，未找到返回 -1
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        private int GetCylinderIndex(object sender)
        {
            IBaseCylinder cylinder = sender as IBaseCylinder;
            if (cylinder == null || cylinderList == null)
                return -1;

            return cylinderList.IndexOf(cylinder);
        }

        /// <summary>
        /// 凭索引获取气缸实例，索引无效时返回 null
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private IBaseCylinder GetCylinderByIndex(int index)
        {
            if (cylinderList == null || index < 0 || index >= cylinderList.Count)
                return null;

            return cylinderList[index];
        }

        /// <summary>
        /// 按索引控制气缸伸出，并在 CylinderDelay 超时内等待到位
        /// </summary>
        public void Stretch(int index)
        {
            IBaseCylinder cylinder = GetCylinderByIndex(index);
            if (cylinder == null)
                return;

            cylinder.Stretch();
        }

        /// <summary>
        /// 按索引控制气缸缩回，并在 CylinderDelay 超时内等待到位
        /// </summary>
        public void Retract(int index)
        {
            IBaseCylinder cylinder = GetCylinderByIndex(index);
            if (cylinder == null)
                return;

            cylinder.Retract();
        }

        /// <summary>
        /// 读取指定气缸当前位置状态
        /// </summary>
        public ECylinderPosType GetCylinderPosType(int index)
        {
            return GetCylinderByIndex(index)?.CylinderPosType ?? ECylinderPosType.Retract;
        }

        private IBaseStateIO GetStateIOByIndex(int index)
        {
            if (stateIOList == null || index < 0 || index >= stateIOList.Count)
                return null;

            return stateIOList[index];
        }

        /// <summary>
        /// 按索引读取状态 IO
        /// </summary>
        public bool ReadStateIO(int index)
        {
            IBaseStateIO stateIO = GetStateIOByIndex(index);
            return stateIO != null && stateIO.Read();
        }

        /// <summary>
        /// 设置指定槽位的有料状态，供内部调用和外部按接口语义直接调用；索引无效或储料位未初始化时返回 false
        /// </summary>
        /// <param name="slotIndex"></param>
        /// <param name="hasMaterial"></param>
        /// <returns></returns>
        public bool SetSlotMaterialState(int slotIndex, bool hasMaterial)
        {
            if (Slots.Count < slotIndex || Slots[slotIndex] == null)
                return false;
            Slots[slotIndex].IsEmpty = !hasMaterial;
            return true;
        }

        public Slot GetSlot(int slotIndex)
        {
            if (Slots.Count < slotIndex || Slots[slotIndex] == null)
                return null;
            return Slots[slotIndex];
        }

        public List<Slot> GetAllSlots()
        {
            return Slots;
        }
        public int GetSlotCount()
        {
            return Slots?.Count ?? 0;
        }
    }
    
    /// <summary>
    /// 储料装置
    /// </summary>
    public class StorageDevice
    {
        /// <summary>
        /// 储料装置名称
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 出厂参数
        /// </summary>
        private readonly StorageDeviceFactoryCfg factoryCfg;

        /// <summary>
        /// 初始化参数
        /// </summary>
        private StorageDeviceInitCfg initCfg;

        /// <summary>
        /// 当前生效配方
        /// </summary>
        private StorageDevicePPCfg ppCfg;

        /// <summary>
        /// 储料装置下各储料位的运行时数据
        /// </summary>
        private List<MaterialBox> materialBoxes;

        /// <summary>
        /// 余料不足预警事件，携带储料位索引
        /// </summary>
        public event EventHandler<StorageDeviceEventArgs> MaterialWarning;

        /// <summary>
        /// 储料位无料事件，携带储料位索引
        /// </summary>
        public event EventHandler<StorageDeviceEventArgs> MaterialEmpty;

        /// <summary>
        /// 储料位气缸伸出完成事件，携带储料位和气缸索引
        /// </summary>
        public event EventHandler<StorageDeviceCylinderEventArgs> StretchFinished;

        /// <summary>
        /// 储料位气缸缩回完成事件，携带储料位和气缸索引
        /// </summary>
        public event EventHandler<StorageDeviceCylinderEventArgs> RetractFinished;

        private int runtimeMaterialIndex = 0;
        private int runtimeSlotIndex = 0;
        /// <summary>
        /// 根据出厂数量和名称创建运行时储料位与机构
        /// <summary>
        public StorageDevice(StorageDeviceFactoryCfg factoryCfg, string name)
        {
            this.factoryCfg = factoryCfg ?? new StorageDeviceFactoryCfg();
            Name = name ?? string.Empty;
            initCfg = new StorageDeviceInitCfg();
            ppCfg = new StorageDevicePPCfg();
            materialBoxes = new List<MaterialBox>();
            int count = this.factoryCfg.StorageCount > 0 ? this.factoryCfg.StorageCount : 2;
            for (int i = 0; i < count; i++)
            {
                MaterialBox materialBox = new MaterialBox(GetStorageBoxName(i));
                materialBox.StretchFinished += OnStorageMechanismStretchFinished;
                materialBox.RetractFinished += OnStorageMechanismRetractFinished;
                materialBoxes.Add(materialBox);
            }
        }

        private string GetStorageBoxName(int materialBoxIndex)
        {
            return materialBoxIndex switch
            {
                0 => "UpperRack",
                1 => "LowerRack",
                _ => $"Rack{materialBoxIndex + 1}"
            };
        }

        /// <summary>
        /// 按初始化参数完成储料机构绑定
        /// </summary>
        public void Init(StorageDeviceInitCfg initCfg, Func<List<Guid>, List<IBaseStateIO>> getIOStateList)
        {
            this.initCfg = initCfg ?? new StorageDeviceInitCfg();
            List<StorageMechanismInitCfg> mechanismCfg = this.initCfg.StorageMechanism ?? new List<StorageMechanismInitCfg>();
            for(int i = 0;i < factoryCfg.StorageCount; i++)
            {
                materialBoxes[i].Init(mechanismCfg[i], getIOStateList);
            }
        }

        /// <summary>
        /// 应用储料装置配方
        /// </summary>
        public void ApplyRecipe(StorageDevicePPCfg ppCfg)
        {
            this.ppCfg = ppCfg ?? new StorageDevicePPCfg();
            for (int i = 0; i < materialBoxes.Count; i++)
            {
                StorageRecipeParameters storageRecipe = GetRecipeParameters(i);
                if (storageRecipe != null)
                {
                    materialBoxes[i].ApplyRecipe(storageRecipe);
                }
            }
        }

        /// <summary>
        /// 按索引获取单个储料位运行时数据
        /// </summary>
        public MaterialBox GetStorageBox(int materialBoxIndex)
        {
            if (materialBoxIndex < 0 || materialBoxIndex >= materialBoxes.Count)
                return null;

            return materialBoxes[materialBoxIndex];
        }

        /// <summary>
        /// 按索引获取单个储料位配方
        /// </summary>
        private StorageRecipeParameters GetRecipeParameters(int materialBoxIndex)
        {
            if (materialBoxIndex < 0 || ppCfg?.Storages == null || materialBoxIndex >= ppCfg.Storages.Count)
                return null;

            return ppCfg.Storages[materialBoxIndex];
        }

        /// <summary>
        /// 读取储料装置当前配置的储料位数量
        /// </summary>
        public int GetStorageCount()
        {
            return factoryCfg?.StorageCount ?? 0;
        }

        /// <summary>
        /// 读取指定储料位的指定状态 IO
        /// </summary>
        public bool ReadStateIO(int materialBoxIndex, int stateIoIndex)
        {
            return GetMaterialBox(materialBoxIndex)?.ReadStateIO(stateIoIndex) ?? false;
        }

        /// <summary>控制指定储料位的指定气缸伸出。</summary>
        public void Stretch(int materialBoxIndex, int cylinderIndex)
        {
            GetMaterialBox(materialBoxIndex)?.Stretch(cylinderIndex);
        }

        /// <summary>控制指定储料位的指定气缸缩回。</summary>
        public void Retract(int materialBoxIndex, int cylinderIndex)
        {
            GetMaterialBox(materialBoxIndex)?.Retract(cylinderIndex);
        }

        /// <summary>读取指定储料位的指定气缸位置状态。</summary>
        public ECylinderPosType GetCylinderPosType(int materialBoxIndex, int cylinderIndex)
        {
            return GetMaterialBox(materialBoxIndex)?.GetCylinderPosType(cylinderIndex) ?? ECylinderPosType.Retract;
        }


        /// <summary>调整指定槽位的有料状态，供外部按接口语义直接调用。</summary>
        public void AdjustSlotMaterialState(int materialBoxIndex, int slotIndex, bool hasMaterial)
        {
            SetSlotMaterialState(materialBoxIndex, slotIndex, hasMaterial);
        }

        /// <summary>调整当前槽位的有料状态，供外部按接口语义直接调用。</summary>
        public void AdjustCurrentSlotMaterialState(bool hasMaterial)
        {
            if (runtimeMaterialIndex >= materialBoxes.Count)
            {
                return;
            }
            if(runtimeSlotIndex >= materialBoxes[runtimeMaterialIndex].GetSlotCount())
            {
                return;
            }
            SetSlotMaterialState(runtimeMaterialIndex, runtimeSlotIndex, hasMaterial);
        }


        /// <summary>读取指定槽位当前是否有料。</summary>
        public Slot GetSlot(int materialBoxIndex, int slotIndex)
        {
            MaterialBox materialBox = GetStorageBox(materialBoxIndex);
            Slot slot = materialBox.GetSlot(slotIndex);
            return slot;
        }

        /// <summary>读取指定槽位当前是否有料。</summary>
        public Slot GetFirstSlot(int materialBoxIndex)
        {
            MaterialBox materialBox = GetStorageBox(materialBoxIndex);
            Slot slot = materialBox.GetSlot(0);
            return slot;
        }

        /// <summary>读取指定槽位当前是否有料。</summary>
        public Slot GetLastSlot(int materialBoxIndex)
        {
            MaterialBox materialBox = GetStorageBox(materialBoxIndex);
            Slot slot = materialBox.GetSlot(materialBox.GetSlotCount() - 1);
            return slot;
        }


        /// <summary>读取指定槽位当前是否有料。</summary>
        public List<Slot> GetAllSlots(int materialBoxIndex)
        {
            MaterialBox materialBox = GetStorageBox(materialBoxIndex);
            return materialBox.GetAllSlots();
        }

        /// <summary>设置指定槽位当前是否有料。</summary>
        public void SetSlotMaterialState(int materialBoxIndex, int slotIndex, bool hasMaterial)
        {
            MaterialBox storageBox = GetStorageBox(materialBoxIndex);
            storageBox.SetSlotMaterialState(slotIndex, hasMaterial);
        }
        /// <summary>设置指定槽位当前是否有料。</summary>
        public void SetSlotMaterialState(int materialBoxIndex, bool hasMaterial)
        {
            MaterialBox storageBox = GetStorageBox(materialBoxIndex);
            for(int i = 0;i< storageBox.GetSlotCount(); i++)
            {
                storageBox.SetSlotMaterialState(i, hasMaterial);
            }
        }

        /// <summary>查找指定储料位的下一个匹配有料状态的可用槽位，找到后同步当前槽位索引。</summary>
        public bool TryMoveToNextOKSlot(bool hasMaterial, out Slot slot)
        {
            if(runtimeMaterialIndex >= materialBoxes.Count)
            {
                RaiseMaterialEmpty(materialBoxes.Count - 1);
                slot = null;
                return false;
            }
            bool hasMaterialBox = false;
            foreach(var box in materialBoxes)
            {
                if (box.ReadStateIO(0) == true)
                {
                    hasMaterialBox = true;
                }
            }
            if(hasMaterialBox == false)
            {
                RaiseMaterialEmpty(materialBoxes.Count - 1);
                slot = null;
                string box = hasMaterial ? "上料" : "下料";
                throw new Exception($"{box}无料盒");
            }
            for (int i = runtimeMaterialIndex; i < materialBoxes.Count; i++)
            {
                MaterialBox materialBox = materialBoxes[i];
                if(materialBox.ReadStateIO(0) == false)
                {
                    runtimeSlotIndex = 0;
                    continue;
                }
                if (runtimeSlotIndex >= materialBox.GetSlotCount())
                {
                    runtimeSlotIndex = 0;
                    runtimeMaterialIndex++;
                }
                for (int j = runtimeSlotIndex; j < materialBox.GetSlotCount(); j++)
                {
                    Slot currentSlot;
                    if (j + 1 < materialBox.GetSlotCount())
                    {
                        runtimeSlotIndex = j + 1;
                        currentSlot = materialBox.GetSlot(j + 1);
                        if (currentSlot.IsEnabled && currentSlot.IsEmpty != hasMaterial)
                        {
                            slot = currentSlot;
                            return true;
                        }
                    }
                    else
                    {
                        RaiseMaterialEmpty(runtimeMaterialIndex);
                        break;
                    }
                }
            }
            RaiseMaterialEmpty(materialBoxes.Count - 1);
            slot = null;
            return false;
        }

        public bool TryMoveToCurrentOKSlot(bool hasMaterial, out Slot slot)
        {
            if (runtimeMaterialIndex >= materialBoxes.Count)
            {
                RaiseMaterialEmpty(materialBoxes.Count - 1);
                slot = null;
                return false;
            }
            MaterialBox materialBox = materialBoxes[runtimeMaterialIndex];
            if (materialBox.ReadStateIO(0) == false)
            {
                return TryMoveToNextOKSlot(hasMaterial, out slot);
            }
            if (runtimeSlotIndex < materialBox.GetSlotCount())
            {
                slot = materialBox.GetSlot(runtimeSlotIndex);
                return true;
            }
            else
            {
                RaiseMaterialEmpty(materialBoxes.Count - 1);
                slot = null;
                return false;
            }
        }

        /// <summary>输出指定储料位的初始位置。</summary>
        public double GetInitialPosition(int materialBoxIndex)
        {
            StorageRecipeParameters recipe = GetRecipeParameters(materialBoxIndex);
            if (recipe?.InitialPositionList == null || materialBoxIndex < 0 || materialBoxIndex >= recipe.InitialPositionList.Count)
                return 0;

            return recipe.InitialPositionList[materialBoxIndex];
        }
        public string GetMaterialBoxName(int materialBoxIndex)
        {
            if (materialBoxIndex < 0 || materialBoxIndex >= materialBoxes.Count)
                return null;

            return materialBoxes[materialBoxIndex].Name;
        }
        /// <summary>主动抛出余料预警事件。</summary>
        public void RaiseMaterialWarning(int materialBoxIndex)
        {
            MaterialWarning?.Invoke(this, new StorageDeviceEventArgs(materialBoxIndex));
        }

        /// <summary>主动抛出缺料事件。</summary>
        public void RaiseMaterialEmpty(int materialBoxIndex)
        {
            MaterialEmpty?.Invoke(this, new StorageDeviceEventArgs(materialBoxIndex));
        }

        private MaterialBox GetMaterialBox(int materialBoxIndex)
        {
            if (materialBoxIndex < 0 || materialBoxIndex >= materialBoxes.Count)
                return null;

            return materialBoxes[materialBoxIndex];
        }

        private void OnStorageMechanismStretchFinished(object sender, CylinderEventArgs e)
        {
            int materialBoxIndex = materialBoxes.IndexOf(sender as MaterialBox);
            StretchFinished?.Invoke(this, new StorageDeviceCylinderEventArgs(materialBoxIndex, e?.CylinderIndex ?? -1));
        }

        private void OnStorageMechanismRetractFinished(object sender, CylinderEventArgs e)
        {
            int materialBoxIndex = materialBoxes.IndexOf(sender as MaterialBox);
            RetractFinished?.Invoke(this, new StorageDeviceCylinderEventArgs(materialBoxIndex, e?.CylinderIndex ?? -1));
        }
    }
}
