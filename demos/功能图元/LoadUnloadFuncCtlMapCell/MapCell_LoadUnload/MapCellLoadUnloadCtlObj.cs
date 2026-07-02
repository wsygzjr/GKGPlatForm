using Avalonia.PropertyGrid.Controls;
using DynamicData.Binding;
using GF_Gereric;
using GKG.Map.LoadUnloadFuncCtlMapCell.Models;
using GKG.Map.LoadUnloadFuncCtlMapCell.ViewModels;
using GKG.Map.LoadUnloadFuncCtlMapCell.Views;
using Griffins;
using Griffins.Map;
using Griffins.Map.Cmd;
using Griffins.Map.UI;
using Griffins.UI2;
using PropertyModels.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace GKG.Map.LoadUnloadFuncCtlMapCell
{
    /// <summary>
    /// 上下料功能图元对象
    /// 负责打通 View, ViewModel, Model 与底层 Griffins Map 框架的数据流和命令下发。
    /// </summary>
    class MapCellLoadUnloadCtlObj : FunctionalCellBase
    {
        #region 字段与属性

        private LoadUnloadView view;
        private LoadUnloadViewModel viewModel = null!;

        /// <summary>
        /// 快速获取上下料属性编辑模型实例
        /// </summary>
        [Browsable(false)]
        public LoadUnloadPropertyModelEdit LoadUnloadPropertyModelEdit => (PropertyEditModelBase as LoadUnloadPropertyModelEdit)!;

        #endregion

        #region 构造与初始化

        /// <summary>
        /// 静态构造函数：处理类级别的全局、一次性初始化操作
        /// </summary>
        static MapCellLoadUnloadCtlObj() { }

        /// <summary>
        /// 运行时图元实例构造函数
        /// </summary>
        public MapCellLoadUnloadCtlObj(MapObjID mapCellID, string mapCellName) : this(mapCellID, mapCellName, false) { }

        /// <summary>
        /// 图元实例全参数构造函数 (区分设计时与运行时)
        /// </summary>
        public MapCellLoadUnloadCtlObj(MapObjID mapCellID, string mapCellName, bool designTime)
        {
            // 初始化基础模型
            PropertyEditModelBase = CreatePropertyModelEditBase();
            PropertyBindEditModelBase = CreatePropertyBindEditModelBase();
            EventBindEditModel = CreateEventBindEditModel();

            base.SetID(mapCellID);
            base.SetName(mapCellName);
            (this as IMapCellTypeBase).Name = ResourceA.LoadUnload;

            // 初始化视图
            view = new LoadUnloadView();

            // 注册向外部框架暴露的属性与操作
            RegisterPropertiesAndOprts();
        }

        /// <summary>
        /// 注册向 Griffins Map 框架暴露的图元属性与操作原子
        /// </summary>
        private void RegisterPropertiesAndOprts()
        {
            // 1. 注册原生交互事件 (鼠标点击等)
            RegisterEvent(new MapObjEventInfo(
                eventID: MapObjPropEventConst.Event_MouseClick,
                eventName: MapObjPropEventConst.GetName(MapObjPropEventConst.Event_MouseClick),
                eventParamValType: GriffinsBaseDataType.Object_Bytes,
                eventParamObjID: GraphMouseEventParam.Object_ID));

            // 2. 注册容器数据更新操作原子
            RegisterOprtCellInfo(new MapOprtCellInfo(
                oprtCellID: LoadUnloadMapOprtCellConst.MaterialContainers_MapOprtCellID,
                oprtCellName: ResourceA.MaterialContainers_MapOprtCellName));

            // 3. 注册复杂对象属性及关联原子绑定
            RegisterPropAndOprtBinding(
                nameof(LoadUnloadPropertyModelEdit.MaterialContainers),
                ResourceA.MaterialContainers,
                ResourceA.MaterialContainers_MapOprtName,
                GriffinsBaseDataType.Object_Bytes,
                GFBaseTypePropValueListDict.Object_ID,
                typeof(Dictionary<string, MaterialContainerStatus>),
                null);
        }

        /// <summary>
        /// 辅助方法：统一封装复杂属性与操作原子的注册逻辑，消除样板代码
        /// </summary>
        private void RegisterPropAndOprtBinding(string propertyID, string propertyName, string oprtName, GriffinsBaseDataType dataType, Guid valObjID, Type editorType, GriffinsBaseValue? defaultValue)
        {
            RegisterProperty(new MapObjPropertyInfo(
                propertyID: propertyID,
                propertyName: propertyName,
                propertyValType: dataType,
                propertyValObjID: valObjID,
                editorType: editorType,
                canGrouped: false,
                canBindVar: true,
                defaultValue: defaultValue));

            RegisterOprtInfo(new MapOprtInfo(
                propertyID: propertyID,
                oprtName: oprtName,
                oprtExecKind: OprtExecKind.Normal,
                script: string.Empty,
                mapOprtCellInstInfos: new MapOprtCellInstInfoList()
                {
                    new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = LoadUnloadMapOprtCellConst.MaterialContainers_MapOprtCellID, CfgInfo = null }
                }));
        }

        #endregion

        #region 数据同步与更新

        /// <summary>
        /// 外部或框架设置图元属性的入口
        /// </summary>
        public override bool SetPropertyValue(string propertyID, GriffinsBaseValue propertyVal, bool isRuning)
        {
            LoadUnloadPropertyModelEdit.IsRuning = isRuning;
            return LoadUnloadPropertyModelEdit.SetPropertyValue(propertyID, propertyVal);
        }

        /// <summary>
        /// 监听后端推送：解析界面数据对象属性值列表，映射到内部 Model 属性
        /// </summary>
        protected override bool SetUIDataObjPropValues(GFBaseTypePropValueList gFBaseTypePropValues)
        {
            if (gFBaseTypePropValues == null) return false;

            bool hasHandledAny = false;
            foreach (GFBaseTypePropValue prop in gFBaseTypePropValues)
            {
                if (prop == null || prop.Value == null) continue;

                string propId = prop.PropertyID.ToString();

                switch (propId)
                {
                    case "MaterialContainers":
                        hasHandledAny |= LoadUnloadPropertyModelEdit.SetPropertyValue(nameof(LoadUnloadPropertyModelEdit.MaterialContainers), prop.Value);
                        break;
                }
            }

            return hasHandledAny;
        }

        /// <summary>
        /// 当图元 Model 属性值改变后的处理逻辑
        /// </summary>
        protected override void AfterPropertyChanged(string propertyID, GriffinsBaseValue propertyValue)
        {
            base.AfterPropertyChanged(propertyID, propertyValue);

            // 通知框架操作原子，UI 刷新已由原子回调接管
            switch (propertyID)
            {
                case nameof(LoadUnloadPropertyModelEdit.MaterialContainers):
                    CallBack?.ExecOprt(propertyID);
                    break;
            }

            if (LoadUnloadPropertyModelEdit.IsRuning)
            {
                CallBack?.UpdateUIDataObjPropValues(new GFBaseTypePropValueList());
            }
        }

        /// <summary>
        /// 提供给框架的核心属性值获取与序列化封装接口
        /// </summary>
        public override GriffinsBaseValue ConvertPropertyValue(string propertyID, object propertyValue)
        {
            switch (propertyID)
            {
                case nameof(LoadUnloadPropertyModelEdit.MaterialContainers):
                    if (propertyValue is Dictionary<string, MaterialContainerStatus> materialContainers)
                    {
                        GFBaseTypePropValueListDict gFBaseTypePropValueListDict = new GFBaseTypePropValueListDict();
                        foreach (var kv in materialContainers)
                        {
                            gFBaseTypePropValueListDict.Add(kv.Key, kv.Value.ToGFBaseTypePropValues());
                        }
                        return gFBaseTypePropValueListDict.ToGriffinsBaseValue();
                    }
                    break;
            }

            return null!;
        }

        #endregion

        #region 操作原子定义与执行

        /// <summary>
        /// 拦截并执行操作原子
        /// </summary>
        protected override bool ExecOprtCell(MapOprtCellInstInfo mapOprtCellInstInfo)
        {
            if (mapOprtCellInstInfo == null) return false;

            if (mapOprtCellInstInfo.OprtCellID == LoadUnloadMapOprtCellConst.MaterialContainers_MapOprtCellID)
            {
                if (!MapOprtCellExectorDict.TryGetValue(mapOprtCellInstInfo.InstanceID, out var mapOprtCellExector))
                {
                    mapOprtCellExector = new MaterialContainersMapOprtCellExector();
                    mapOprtCellExector.Init(IMapOprtCellExectorCallBack);
                    MapOprtCellExectorDict.TryAdd(mapOprtCellInstInfo.InstanceID, mapOprtCellExector);
                }

                mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                return true;
            }

            return base.ExecOprtCell(mapOprtCellInstInfo);
        }

        /// <summary>
        /// 容器字典数据组操作原子执行器：负责将更新的数据集合推送到 ViewModel 进行深层比对 UI 刷新
        /// </summary>
        private class MaterialContainersMapOprtCellExector : IMapOprtCellExector
        {
            private IMapOprtCellExectorCallBack callBack = null!;

            public MaterialContainersMapOprtCellExector() { }

            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack) => this.callBack = callBack;

            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                object viewModel = callBack.GetMapCellVMObjInstance();
                if (viewModel is LoadUnloadViewModel loadUnloadViewModel)
                {
                    GriffinsBaseValue mapCellPropValue = callBack.GetMapCellPropValue(nameof(LoadUnloadPropertyModelEdit.MaterialContainers));
                    if (mapCellPropValue != null)
                    {
                        GFBaseTypePropValueListDict gFBaseTypePropValueListDict = new GFBaseTypePropValueListDict();
                        ((IGriffinsBaseValue)gFBaseTypePropValueListDict).PopulateFromBaseValue(mapCellPropValue);
                        Dictionary<string, MaterialContainerStatus> materialContainers = new Dictionary<string, MaterialContainerStatus>();
                        foreach (var kv in gFBaseTypePropValueListDict)
                        {
                            MaterialContainerStatus materialContainerStatus = GFPropObjBase.FromGFBaseTypePropValues<MaterialContainerStatus>(kv.Value);
                            materialContainers.Add(kv.Key, materialContainerStatus);
                        }

                        // 推送给 VM 同步
                        loadUnloadViewModel.SyncStatusFromBackend(materialContainers);
                    }
                }
            }
        }

        #endregion

        #region 执行图元下发命令 (对接 ViewModel 事件)

        private async Task ExeClampMagazineAsync(string containerName, string magName)
        {
            await ExecuteHardwareCommandAsync("StorageClose", new GFBaseTypeParamValueList
            {
                new GFBaseTypeParamValue("ContainerName", new GriffinsBaseValue(containerName)),
                new GFBaseTypeParamValue("MagName", new GriffinsBaseValue(magName))
            }, "执行料盒夹紧 (StorageClose) 动作失败。");
        }

        private async Task ExeUnclampMagazineAsync(string containerName, string magName)
        {
            await ExecuteHardwareCommandAsync("StorageOpen", new GFBaseTypeParamValueList
            {
                new GFBaseTypeParamValue("ContainerName", new GriffinsBaseValue(containerName)),
                new GFBaseTypeParamValue("MagName", new GriffinsBaseValue(magName))
            }, "执行料盒松开 (StorageOpen) 动作失败。");
        }

        private async Task ExeLoadAsync()
        {
            await ExecuteHardwareCommandAsync("LoadOnce", new GFBaseTypeParamValueList(), "后端拒绝或执行全局上料 (LoadOnce) 失败。");
        }

        private async Task ExeUnloadAsync()
        {
            await ExecuteHardwareCommandAsync("UnloadOnce", new GFBaseTypeParamValueList(), "后端拒绝或执行全局下料 (UnloadOnce) 失败。");
        }

        /// <summary>
        /// 🚀 核心底层硬件指令下发器 (应用 DRY 原则，提炼高度重复的网络请求与校验逻辑)
        /// </summary>
        private async Task ExecuteHardwareCommandAsync(string commandName, GFBaseTypeParamValueList parameters, string errorMsg)
        {
            var executor = GetCmdExecutor();
            string mpNo = PropertyBindEditModelBase?.MpNo ?? string.Empty;

            if (string.IsNullOrEmpty(mpNo))
            {
                throw new InvalidOperationException("未配置图元 MpNo，无法下发硬件指令。");
            }

            // 统一发起异步网络请求
            var result = await Task.Run(() => executor.ExecUIDataObjCommand(mpNo, commandName, parameters));

            // 统一结果与业务异常校验
            if (result == null || result["result"]?.ToString() == "-1")
            {
                throw new Exception(errorMsg);
            }
        }

        /// <summary>
        /// 安全获取当前环境下的命令执行器
        /// </summary>
        private MapCmdExector GetCmdExecutor()
        {
            if (base.CallBack?.INorthSvrCommandExec == null)
                throw new InvalidOperationException("服务器命令执行接口 (INorthSvrCommandExec) 未初始化。");

            return new MapCmdExector(base.CallBack.INorthSvrCommandExec);
        }

        #endregion

        #region 生命周期与数据持久化

        protected override void OnInit()
        {
            viewModel = new LoadUnloadViewModel(LoadUnloadPropertyModelEdit);

            // 核心架构解耦：在此处桥接 VM 纯净意图与底层物理执行器
            viewModel.OnClampRequested += ExeClampMagazineAsync;
            viewModel.OnUnclampRequested += ExeUnclampMagazineAsync;
            viewModel.OnLoadRequested += ExeLoadAsync;
            viewModel.OnUnloadRequested += ExeUnloadAsync;

            view.DataContext = viewModel;
        }

        public override void OnDispose()
        {
            // 严谨释放事件订阅，防止内存泄漏
            if (viewModel != null)
            {
                viewModel.OnClampRequested -= ExeClampMagazineAsync;
                viewModel.OnUnclampRequested -= ExeUnclampMagazineAsync;
                viewModel.OnLoadRequested -= ExeLoadAsync;
                viewModel.OnUnloadRequested -= ExeUnloadAsync;
            }

            if (view != null) view.DataContext = null;
            if (viewModel is IDisposable disposableVM) disposableVM.Dispose();

            viewModel = null!;
            view = null!;

            base.OnDispose();
        }

        /// <summary>
        /// 从 XML 中反序列化组态配置 (引入严密的空值防御，解决向下兼容性崩溃隐患)
        /// </summary>
        protected override void OnReadDrawInfoFromBytes(GriffinsXmlReader br)
        {
            base.OnReadDrawInfoFromBytes(br);

            // 统一使用 JsonObjConvert 并增加安全校验，移除感叹号 (!)
            var propertyEditModelBase = JsonObjConvert.FromJSon<LoadUnloadPropertyModelEdit>(br.ReadString("PropertyEditModelBase"));
            if (propertyEditModelBase != null)
            {
                ((LoadUnloadPropertyModelEdit)PropertyEditModelBase).CopyFrom(propertyEditModelBase);
            }

            var propertyBindEditModelBase = JsonObjConvert.FromJSon<LoadUnloadPropertyBindEditModel>(br.ReadString("PropertyBindEditModelBase"));
            if (propertyBindEditModelBase != null)
            {
                ((LoadUnloadPropertyBindEditModel)PropertyBindEditModelBase).CopyFrom(propertyBindEditModelBase);
            }

            var eventBindEditModel = JsonObjConvert.FromJSon<EventBindEditModel>(br.ReadString("EventBindEditModel"));
            if (eventBindEditModel != null)
            {
                EventBindEditModel?.CopyFrom(eventBindEditModel);
            }
        }

        /// <summary>
        /// 序列化保存至 XML (统一 JSON 引擎)
        /// </summary>
        protected override void OnWriteDrawInfoToBytes(GriffinsXmlWriter bw)
        {
            base.OnWriteDrawInfoToBytes(bw);
            bw.Write("PropertyEditModelBase", JsonObjConvert.ToJSon(PropertyEditModelBase));
            bw.Write("PropertyBindEditModelBase", JsonObjConvert.ToJSon(PropertyBindEditModelBase));
            bw.Write("EventBindEditModel", JsonObjConvert.ToJSon(EventBindEditModel));
        }

        protected override void OnCopyFrom(FunctionalCellBase source)
        {
            if (source is MapCellLoadUnloadCtlObj mapCellLoadUnloadCtlObj) base._CopyFrom(mapCellLoadUnloadCtlObj);
            PropertyEditModelBase.CopyFrom(source.PropertyEditModelBase);
            PropertyBindEditModelBase.CopyFrom(source.PropertyBindEditModelBase);
            EventBindEditModel?.CopyFrom(source.EventBindEditModel);
        }

        protected override object OnGetView() => view;

        protected override object OnGetViewModel() => viewModel;

        public override PropertyEditModelBase CreatePropertyModelEditBase() => new LoadUnloadPropertyModelEdit();

        public override PropertyBindEditModelBase CreatePropertyBindEditModelBase() => new LoadUnloadPropertyBindEditModel();

        public override EventBindEditModel CreateEventBindEditModel()
        {
            return new EventBindEditModel()
            {
                EventCmdInfos = new BindingList<EventCmdInfo>()
                {
                    new EventCmdInfo() { EventCmdKind = EventCmdKind.MpCmdKind, EventID = MapObjPropEventConst.Event_MouseClick },
                }
            };
        }

        public override string ToString() => ResourceA.LoadUnload;

        #endregion
    }

    /// <summary>
    /// 上下料图元属性模型 (配置项与数据承载)
    /// </summary>
    [Serializable]
    [MapPropertyOrder]
    [CategoryPriority("图元信息", 1)]
    public class LoadUnloadPropertyModelEdit : FunctionalCellPropertyModelEdit
    {
        public LoadUnloadPropertyModelEdit() { }

        private Dictionary<string, MaterialContainerStatus> materialContainers = new();

        /// <summary>
        /// 上下料图元内挂载的核心数据：物理容器状态字典
        /// </summary>
        [Browsable(false)]
        public Dictionary<string, MaterialContainerStatus> MaterialContainers
        {
            get => materialContainers;
            set => SetProperty(ref materialContainers, value);
        }

        public override bool SetPropertyValue(string propertyID, GriffinsBaseValue propertyVal)
        {
            if (propertyID == nameof(MaterialContainers))
            {
                if (propertyVal != null)
                {
                    GFBaseTypePropValueListDict gFBaseTypePropValueListDict = new GFBaseTypePropValueListDict();
                    ((IGriffinsBaseValue)gFBaseTypePropValueListDict).PopulateFromBaseValue(propertyVal);
                    Dictionary<string, MaterialContainerStatus> newMaterialContainers = new Dictionary<string, MaterialContainerStatus>();

                    foreach (var kv in gFBaseTypePropValueListDict)
                    {
                        MaterialContainerStatus materialContainerStatus = GFPropObjBase.FromGFBaseTypePropValues<MaterialContainerStatus>(kv.Value);
                        newMaterialContainers.Add(kv.Key, materialContainerStatus);
                    }
                    MaterialContainers = newMaterialContainers;
                }
                else
                {
                    MaterialContainers = new Dictionary<string, MaterialContainerStatus>();
                }
                return true;
            }

            return base.SetPropertyValue(propertyID, propertyVal);
        }

        /// <summary>
        /// 深度拷贝，保证组态阶段的绝对数据隔离
        /// </summary>
        public void CopyFrom(LoadUnloadPropertyModelEdit source)
        {
            base.CopyFrom(source);

            Dictionary<string, MaterialContainerStatus> newMaterialContainers = new Dictionary<string, MaterialContainerStatus>();
            if (source.MaterialContainers != null)
            {
                foreach (var kv in source.MaterialContainers)
                {
                    MaterialContainerStatus newMaterialContainerStatus = new MaterialContainerStatus();
                    newMaterialContainerStatus.CopyFrom(kv.Value);
                    newMaterialContainers.Add(kv.Key, newMaterialContainerStatus);
                }
            }
            this.MaterialContainers = newMaterialContainers;
        }
    }

    /// <summary>
    /// 上下料图元绑定属性
    /// </summary>
    [Serializable]
    [MapPropertyOrder]
    [CategoryPriority("点位信息", 1)]
    [CategoryPriority("绑定信息", 2)]
    public class LoadUnloadPropertyBindEditModel : FunctionalCellPropertyBindEditModel
    {
        public void CopyFrom(LoadUnloadPropertyBindEditModel source)
        {
            base.CopyFrom(source);
        }
    }
}