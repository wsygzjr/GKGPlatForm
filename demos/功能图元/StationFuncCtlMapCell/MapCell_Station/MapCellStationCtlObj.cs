using Avalonia.Threading;
using GF_Gereric;
using GKG.Map.StationFuncCtlMapCell.Models;
using GKG.Map.StationFuncCtlMapCell.View;
using GKG.Map.StationFuncCtlMapCell.ViewModel;
using Griffins;
using Griffins.Map;
using Griffins.Map.Cmd;
using Griffins.Map.UI;
using Griffins.UI2;
using HarfBuzzSharp;
using Newtonsoft.JsonG;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace GKG.Map.StationFuncCtlMapCell
{
    /// <summary>
    /// 工位功能图元核心对象 (Controller)
    /// 负责打通 View, ViewModel, Model 与底层 Griffins Map 框架的数据流和命令下发。
    /// </summary>
    class MapCellStationCtlObj : FunctionalCellBase
    {
        #region 字段与属性

        private StationView view;
        private StationViewModel viewModel;

        /// <summary>
        /// 快速获取工位属性编辑模型实例 (Model)
        /// </summary>
        [Browsable(false)]
        public StationPropertyModelEdit StationPropertyModelEdit => (PropertyEditModelBase as StationPropertyModelEdit)!;

        #endregion

        #region 构造与初始化

        /// <summary>
        /// 静态构造函数
        /// 负责当前图元类的全局、一次性初始化操作（如静态资源的预加载、全局样式的映射等）。
        /// CLR 会保证在首次实例化该类或访问任何静态成员前，绝对线程安全地且仅执行一次。
        /// </summary>
        static MapCellStationCtlObj()
        {
            // TODO: 在此添加任何特定于此类全局范围的初始化逻辑
        }

        /// <summary>
        /// 运行时图元实例构造函数
        /// </summary>
        public MapCellStationCtlObj(MapObjID mapCellID, string mapCellName)
            : this(mapCellID, mapCellName, false)
        {
        }

        /// <summary>
        /// 图元实例全参数构造函数 (区分设计时与运行时)
        /// </summary>
        public MapCellStationCtlObj(MapObjID mapCellID, string mapCellName, bool designTime)
        {
            // 1. 初始化基础数据模型
            PropertyEditModelBase = CreatePropertyModelEditBase();
            PropertyBindEditModelBase = CreatePropertyBindEditModelBase();
            EventBindEditModel = CreateEventBindEditModel();

            base.SetID(mapCellID);
            base.SetName(mapCellName);
            (this as IMapCellTypeBase).Name = ResourceA.Station;

            // 2. 初始化视图 (View) 与视图模型 (ViewModel)
            view = new StationView();
            viewModel = new StationViewModel(StationPropertyModelEdit);
            view.DataContext = viewModel;

            // 3. 订阅气缸点击事件（防崩溃与异步下发，防止网络阻塞主 UI 线程）
            viewModel.OnLeftJackingClicked += () =>
            {
                System.Threading.Tasks.Task.Run(() =>
                {
                    try
                    {
                        ExeLeftJackingClicked();
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"左气缸控制异常: {ex.Message}");
                    }
                });
            };

            viewModel.OnRightJackingClicked += () =>
            {
                System.Threading.Tasks.Task.Run(() =>
                {
                    try
                    {
                        ExeRightJackingClicked();
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"右气缸控制异常: {ex.Message}");
                    }
                });
            };

            // 4. 注册向框架暴露的图元属性与操作原子
            RegisterPropertiesAndOprts();
        }

        /// <summary>
        /// 注册向 Griffins Map 框架暴露的图元属性与操作原子
        /// </summary>
        private void RegisterPropertiesAndOprts()
        {
            // 1. 注册总的数据组操作原子
            RegisterOprtCellInfo(new MapOprtCellInfo(StationMapOprtCellConst.DataGroup_MapOprtCellID, ResourceA.DataGroup_MapOprtCellName));

            // ==========================================
            // 2. 组态配置属性绑定
            // ==========================================
            RegisterPropAndOprtBinding(nameof(StationPropertyModelEdit.HasLeftJacking), ResourceA.HasLeftJacking, ResourceA.HasLeftJacking_MapOprtName, GriffinsBaseDataType.Bool, typeof(bool), new GriffinsBaseValue(true));
            RegisterPropAndOprtBinding(nameof(StationPropertyModelEdit.HasRightJacking), ResourceA.HasRightJacking, ResourceA.HasRightJacking_MapOprtName, GriffinsBaseDataType.Bool, typeof(bool), new GriffinsBaseValue(true));

            // ==========================================
            // 3. 运行时状态属性绑定 (挡板)
            // ==========================================
            RegisterPropAndOprtBinding(nameof(StationPropertyModelEdit.LeftJackingState), ResourceA.LeftJackingState, ResourceA.LeftJackingState_MapOprtName, GriffinsBaseDataType.Integer, typeof(int), new GriffinsBaseValue(StationPropertyModelEdit.STATE_RETRACT));
            RegisterPropAndOprtBinding(nameof(StationPropertyModelEdit.RightJackingState), ResourceA.RightJackingState, ResourceA.RightJackingState_MapOprtName, GriffinsBaseDataType.Integer, typeof(int), new GriffinsBaseValue(StationPropertyModelEdit.STATE_RETRACT));

            // ==========================================
            // 4. 运行时状态属性绑定 (传感器与料板)
            // ==========================================
            RegisterPropAndOprtBinding(nameof(StationPropertyModelEdit.LeftSensorStatus), ResourceA.LeftSensorStatus, ResourceA.LeftSensorStatus_MapOprtName, GriffinsBaseDataType.Bool, typeof(bool), new GriffinsBaseValue(false));
            RegisterPropAndOprtBinding(nameof(StationPropertyModelEdit.RightSensorStatus), ResourceA.RightSensorStatus, ResourceA.RightSensorStatus_MapOprtName, GriffinsBaseDataType.Bool, typeof(bool), new GriffinsBaseValue(false));
            RegisterPropAndOprtBinding(nameof(StationPropertyModelEdit.HasBoard), ResourceA.HasBoard, ResourceA.HasBoard_MapOprtName, GriffinsBaseDataType.Bool, typeof(bool), new GriffinsBaseValue(false));
        }

        /// <summary>
        /// 辅助方法：统一封装属性与操作原子的注册逻辑
        /// </summary>
        private void RegisterPropAndOprtBinding(string propertyName, string propertyDisplayName, string oprtDisplayName, GriffinsBaseDataType dataType, Type systemType, GriffinsBaseValue defaultValue)
        {
            // 注册图元属性
            RegisterProperty(new MapObjPropertyInfo(
                propertyName, propertyDisplayName, dataType, Guid.Empty, systemType, false, true, defaultValue));

            // 注册操作原子绑定
            RegisterOprtInfo(new MapOprtInfo(
                propertyName, oprtDisplayName, OprtExecKind.Normal, "", CreateMapOprtCellList(StationMapOprtCellConst.DataGroup_MapOprtCellID)));
        }

        /// <summary>
        /// 创建操作原子实例信息列表辅助方法
        /// </summary>
        private MapOprtCellInstInfoList CreateMapOprtCellList(MapOprtCellID oprtCellId)
            => new MapOprtCellInstInfoList() { new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = oprtCellId, CfgInfo = null } };

        #endregion

        #region 执行图元下发命令

        private void ExeLeftJackingClicked() => ExeCylinderCommand("LeftCylinder", StationPropertyModelEdit.LeftJackingState);
        private void ExeRightJackingClicked() => ExeCylinderCommand("RightCylinder", StationPropertyModelEdit.RightJackingState);

        /// <summary>
        /// 通用气缸控制逻辑
        /// </summary>
        /// <param name="cylinderPrefix">气缸指令前缀，如 "LeftCylinder"</param>
        /// <param name="currentState">当前气缸状态，用于取反下发</param>
        private void ExeCylinderCommand(string cylinderPrefix, int currentState)
        {
            // 1. 严格判空与环境校验
            if (base.CallBack?.INorthSvrCommandExec == null)
                throw new InvalidOperationException("服务器命令执行接口 (INorthSvrCommandExec) 未初始化。");
            if (PropertyBindEditModelBase == null)
                throw new InvalidOperationException("属性绑定编辑模型为空，无法获取图元点位编号。");
            if (StationPropertyModelEdit == null)
                throw new InvalidOperationException("图元属性模型未初始化。");

            // 2. 状态取反逻辑：当前处于伸出 (0) 或异常 (2) 状态时，执行缩回；否则执行伸出。
            bool isCurrentStretch = currentState == StationPropertyModelEdit.STATE_STRETCH || currentState == StationPropertyModelEdit.STATE_UNNORMAL;

            var paramList = new GFBaseTypeParamValueList();
            string actionSuffix = isCurrentStretch ? "Retract" : "Stretch";
            paramList.Add(new GFBaseTypeParamValue($"{cylinderPrefix}{actionSuffix}", new GriffinsBaseValue(true)));

            // 3. 执行异步下发命令
            var cmdExecutor = new MapCmdExector(base.CallBack.INorthSvrCommandExec);
            var result = cmdExecutor.ExecUIDataObjCommand(PropertyBindEditModelBase.MpNo, $"Control{cylinderPrefix}", paramList);

            // 4. 执行结果校验
            if (result == null)
                throw new Exception($"后端控制 {cylinderPrefix} 返回结果为空（网络已超时或发生底层错误）。");
        }

        #endregion

        #region 数据同步与更新

        /// <summary>
        /// 外部或框架设置图元属性的入口
        /// </summary>
        public override bool SetPropertyValue(string propertyID, GriffinsBaseValue propertyVal, bool isRuning)
        {
            StationPropertyModelEdit.IsRuning = isRuning;
            return StationPropertyModelEdit.SetPropertyValue(propertyID, propertyVal);
        }

        /// <summary>
        /// 监听后端推送：解析界面数据对象属性值列表，并无损映射到内部 Model 属性
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
                    case "IsHaveMaterial":
                        StationPropertyModelEdit.HasBoard = prop.Value.ToBool();
                        hasHandledAny = true;
                        break;
                    case "LeftSensorState":
                        StationPropertyModelEdit.LeftSensorStatus = prop.Value.ToBool();
                        hasHandledAny = true;
                        break;
                    case "RightSensorState":
                        StationPropertyModelEdit.RightSensorStatus = prop.Value.ToBool();
                        hasHandledAny = true;
                        break;
                    case "LeftCylinderState":
                        StationPropertyModelEdit.LeftJackingState = (int)prop.Value.ToInteger();
                        hasHandledAny = true;
                        break;
                    case "RightCylinderState":
                        StationPropertyModelEdit.RightJackingState = (int)prop.Value.ToInteger();
                        hasHandledAny = true;
                        break;
                }
            }

            return hasHandledAny;
        }

        /// <summary>
        /// 当图元 Model 属性值改变后的处理逻辑
        /// 触发条件：面板修改或后端推送导致数据变化
        /// </summary>
        protected override void AfterPropertyChanged(string propertyID, GriffinsBaseValue propertyValue)
        {
            base.AfterPropertyChanged(propertyID, propertyValue);

            // 通知框架操作原子，UI 刷新已由 ViewModel 主动监听，此处只负责向下通报
            switch (propertyID)
            {
                case nameof(StationPropertyModelEdit.HasLeftJacking):
                case nameof(StationPropertyModelEdit.HasRightJacking):
                case nameof(StationPropertyModelEdit.LeftJackingState):
                case nameof(StationPropertyModelEdit.RightJackingState):
                case nameof(StationPropertyModelEdit.LeftSensorStatus):
                case nameof(StationPropertyModelEdit.RightSensorStatus):
                case nameof(StationPropertyModelEdit.HasBoard):
                    CallBack?.ExecOprt(propertyID);
                    break;
            }

            if (StationPropertyModelEdit.IsRuning)
            {
                CallBack?.UpdateUIDataObjPropValues(new GFBaseTypePropValueList());
            }
        }

        /// <summary>
        /// 提供给框架的核心属性值获取接口
        /// </summary>
        public override GriffinsBaseValue ConvertPropertyValue(string propertyID, object propertyValue)
        {
            switch (propertyID)
            {
                case nameof(StationPropertyModelEdit.HasBoard):
                    return GriffinsBaseValue.Create(StationPropertyModelEdit.HasBoard);
                case nameof(StationPropertyModelEdit.LeftSensorStatus):
                    return GriffinsBaseValue.Create(StationPropertyModelEdit.LeftSensorStatus);
                case nameof(StationPropertyModelEdit.RightSensorStatus):
                    return GriffinsBaseValue.Create(StationPropertyModelEdit.RightSensorStatus);
                case nameof(StationPropertyModelEdit.LeftJackingState):
                    return GriffinsBaseValue.Create(StationPropertyModelEdit.LeftJackingState);
                case nameof(StationPropertyModelEdit.RightJackingState):
                    return GriffinsBaseValue.Create(StationPropertyModelEdit.RightJackingState);
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
            if (mapOprtCellInstInfo.OprtCellID == StationMapOprtCellConst.DataGroup_MapOprtCellID)
            {
                if (MapOprtCellExectorDict.TryGetValue(mapOprtCellInstInfo.InstanceID, out var mapOprtCellExector))
                {
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                else
                {
                    mapOprtCellExector = new DataGroupMapOprtCellExector();
                    mapOprtCellExector.Init(IMapOprtCellExectorCallBack);
                    MapOprtCellExectorDict.TryAdd(mapOprtCellInstInfo.InstanceID, mapOprtCellExector);
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                return true;
            }

            return base.ExecOprtCell(mapOprtCellInstInfo);
        }

        /// <summary>
        /// 数据组操作原子执行器
        /// (ViewModel 已通过 WhenAnyValue 主动侦听变化进行按需渲染，因此这里无需再强制刷新)
        /// </summary>
        private class DataGroupMapOprtCellExector : IMapOprtCellExector
        {
            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack) { }
            void IMapOprtCellExector.Exec(byte[] cfg) { }
        }

        #endregion

        #region 生命周期与数据持久化

        protected override void OnInit() => base.OnInit();

        /// <summary>
        /// 清理资源，释放绑定
        /// </summary>
        public override void OnDispose()
        {
            view.DataContext = null;
            viewModel?.Dispose();
            base.OnDispose();
        }

        /// <summary>
        /// 从 XML 中反序列化自身模型配置 (组态读取)
        /// </summary>
        protected override void OnReadDrawInfoFromBytes(GF_Gereric.GriffinsXmlReader br)
        {
            base.OnReadDrawInfoFromBytes(br);

            // 1. 读取并安全映射基础属性模型
            var propertyEditModelBase = JsonObjConvert.FromJSon<StationPropertyModelEdit>(br.ReadString("PropertyEditModelBase"));
            if (propertyEditModelBase != null)
            {
                (PropertyEditModelBase as StationPropertyModelEdit)?.CopyFrom(propertyEditModelBase);
            }

            // 2. 读取并安全映射属性绑定模型
            var propertyBindEditModelBase = JsonObjConvert.FromJSon<StationPropertyBindEditModel>(br.ReadString("PropertyBindEditModelBase"));
            if (propertyBindEditModelBase != null)
            {
                (PropertyBindEditModelBase as StationPropertyBindEditModel)?.CopyFrom(propertyBindEditModelBase);
            }

            // 3. 读取并安全映射事件绑定模型
            var eventBindEditModel = JsonObjConvert.FromJSon<EventBindEditModel>(br.ReadString("EventBindEditModel"));
            if (eventBindEditModel != null)
            {
                EventBindEditModel?.CopyFrom(eventBindEditModel);
            }
        }

        /// <summary>
        /// 序列化自身配置到 XML (组态保存)
        /// </summary>
        protected override void OnWriteDrawInfoToBytes(GF_Gereric.GriffinsXmlWriter bw)
        {
            base.OnWriteDrawInfoToBytes(bw);

            bw.Write("PropertyEditModelBase", JsonObjConvert.ToJSon(PropertyEditModelBase));
            bw.Write("PropertyBindEditModelBase", JsonObjConvert.ToJSon(PropertyBindEditModelBase));
            bw.Write("EventBindEditModel", JsonObjConvert.ToJSon(EventBindEditModel));
        }

        /// <summary>
        /// 对象拷贝 (支持组态环境复制粘贴)
        /// </summary>
        protected override void OnCopyFrom(FunctionalCellBase source)
        {
            var src = source as MapCellStationCtlObj;
            base._CopyFrom(src);
            PropertyEditModelBase.CopyFrom(source.PropertyEditModelBase);
            PropertyBindEditModelBase.CopyFrom(source.PropertyBindEditModelBase);
            EventBindEditModel?.CopyFrom(source.EventBindEditModel);
        }

        protected override object OnGetView() => view;

        protected override object OnGetViewModel() => viewModel;

        public override PropertyEditModelBase CreatePropertyModelEditBase() => new StationPropertyModelEdit();

        public override PropertyBindEditModelBase CreatePropertyBindEditModelBase() => new StationPropertyBindEditModel();

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

        public override string ToString() => ResourceA.Station;

        #endregion
    }

    /// <summary>
    /// 工位功能图元核心属性编辑器模型 (Model)
    /// 承载图元的配置与实时物理状态。
    /// </summary>
    [Serializable]
    [MapPropertyOrder]
    [CategoryPriority("图元信息", 1)]
    [CategoryPriority("挡板配置", 2)]
    public class StationPropertyModelEdit : FunctionalCellPropertyModelEdit
    {
        #region 常量定义 (消除魔法数字)

        /// <summary>气缸状态：伸出</summary>
        public const int STATE_STRETCH = 0;

        /// <summary>气缸状态：缩回</summary>
        public const int STATE_RETRACT = 1;

        /// <summary>气缸状态：异常</summary>
        public const int STATE_UNNORMAL = 2;

        #endregion

        #region 1. 组态时可见的配置属性

        private bool _hasLeftJacking = true;
        [DisplayName("配置左挡板")]
        [Category("挡板配置")]
        [PropertySortOrder(1)]
        [Description("设置当前工位是否包含左侧挡板")]
        public bool HasLeftJacking
        {
            get => _hasLeftJacking;
            set => SetProperty(ref _hasLeftJacking, value);
        }

        private bool _hasRightJacking = true;
        [DisplayName("配置右挡板")]
        [Category("挡板配置")]
        [PropertySortOrder(2)]
        [Description("设置当前工位是否包含右侧挡板")]
        public bool HasRightJacking
        {
            get => _hasRightJacking;
            set => SetProperty(ref _hasRightJacking, value);
        }

        #endregion

        #region 2. 仅供数据绑定的运行时属性 (面板隐藏)

        private bool _hasBoard;
        [Browsable(false)]
        public bool HasBoard
        {
            get => _hasBoard;
            set => SetProperty(ref _hasBoard, value);
        }

        private bool _leftSensorStatus;
        [Browsable(false)]
        public bool LeftSensorStatus
        {
            get => _leftSensorStatus;
            set => SetProperty(ref _leftSensorStatus, value);
        }

        private bool _rightSensorStatus;
        [Browsable(false)]
        public bool RightSensorStatus
        {
            get => _rightSensorStatus;
            set => SetProperty(ref _rightSensorStatus, value);
        }

        // 气缸状态初始默认设定为缩回
        private int _leftJackingState = STATE_RETRACT;
        [Browsable(false)]
        public int LeftJackingState
        {
            get => _leftJackingState;
            set => SetProperty(ref _leftJackingState, value);
        }

        private int _rightJackingState = STATE_RETRACT;
        [Browsable(false)]
        public int RightJackingState
        {
            get => _rightJackingState;
            set => SetProperty(ref _rightJackingState, value);
        }

        #endregion

        #region 核心方法

        /// <summary>
        /// 提供字符串到具体属性值的映射解析
        /// </summary>
        public override bool SetPropertyValue(string propertyID, GriffinsBaseValue propertyVal)
        {
            switch (propertyID)
            {
                case nameof(HasLeftJacking):
                    HasLeftJacking = propertyVal != null ? propertyVal.ToPrimitiveValue<bool>() : true;
                    return true;

                case nameof(HasRightJacking):
                    HasRightJacking = propertyVal != null ? propertyVal.ToPrimitiveValue<bool>() : true;
                    return true;

                case nameof(HasBoard):
                    HasBoard = propertyVal != null ? propertyVal.ToPrimitiveValue<bool>() : false;
                    return true;

                case nameof(LeftSensorStatus):
                    LeftSensorStatus = propertyVal != null ? propertyVal.ToPrimitiveValue<bool>() : false;
                    return true;

                case nameof(RightSensorStatus):
                    RightSensorStatus = propertyVal != null ? propertyVal.ToPrimitiveValue<bool>() : false;
                    return true;

                // 气缸数字型状态解析及默认值保护
                case nameof(LeftJackingState):
                    LeftJackingState = propertyVal != null ? propertyVal.ToPrimitiveValue<int>() : STATE_RETRACT;
                    return true;

                case nameof(RightJackingState):
                    RightJackingState = propertyVal != null ? propertyVal.ToPrimitiveValue<int>() : STATE_RETRACT;
                    return true;

                default:
                    // 如果都不匹配，交给基类处理
                    return base.SetPropertyValue(propertyID, propertyVal);
            }
        }

        /// <summary>
        /// 深拷贝克隆属性状态
        /// </summary>
        public void CopyFrom(StationPropertyModelEdit source)
        {
            base.CopyFrom(source);

            HasLeftJacking = source.HasLeftJacking;
            HasRightJacking = source.HasRightJacking;
            HasBoard = source.HasBoard;
            LeftSensorStatus = source.LeftSensorStatus;
            RightSensorStatus = source.RightSensorStatus;
            LeftJackingState = source.LeftJackingState;
            RightJackingState = source.RightJackingState;
        }

        #endregion
    }

    /// <summary>
    /// 工位功能图元属性绑定编辑模型
    /// 承载图元各属性与后端映射的配置信息。
    /// </summary>
    [Serializable]
    [MapPropertyOrder]
    [CategoryPriority("界面数据", 1)]
    [CategoryPriority("绑定信息", 2)]
    public class StationPropertyBindEditModel : FunctionalCellPropertyBindEditModel
    {
        public void CopyFrom(StationPropertyBindEditModel source) => base.CopyFrom(source);
    }
}