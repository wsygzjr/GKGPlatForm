using Avalonia.Media;
using Avalonia.Threading;
using GF_Gereric;
using GKG.Map.LogFuncCtlMapCell.View;
using GKG.Map.LogFuncCtlMapCell.ViewModel;
using Griffins;
using Griffins.Map;
using Griffins.Map.Cmd;
using Griffins.Map.UI;
using Griffins.PF;
using Griffins.UI2;
using Newtonsoft.JsonG;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace GKG.Map.LogFuncCtlMapCell
{
    /// <summary>
    /// 日志功能图元对象
    /// </summary>
    class MapCellLogCtlObj : FunctionalCellBase
    {
        #region 字段与属性

        private LogView logView;
        private LogViewModel logViewModel;

        [Browsable(false)]
        public LogPropertyModelEdit LogPropertyModelEdit => (PropertyEditModelBase as LogPropertyModelEdit)!;

        #endregion

        #region 构造与初始化

        static MapCellLogCtlObj()
        {
        }

        public MapCellLogCtlObj(MapObjID mapCellID, string mapCellName)
            : this(mapCellID, mapCellName, false)
        {
        }

        public MapCellLogCtlObj(MapObjID mapCellID, string mapCellName, bool designTime)
        {
            // 初始化基础模型
            PropertyEditModelBase = CreatePropertyModelEditBase();
            PropertyBindEditModelBase = CreatePropertyBindEditModelBase();
            EventBindEditModel = CreateEventBindEditModel();

            base.SetID(mapCellID);
            base.SetName(mapCellName);
            (this as IMapCellTypeBase).Name = ResourceA.Log;

            // 初始化视图与视图模型
            logView = new LogView();
            logViewModel = new LogViewModel(LogPropertyModelEdit);
            logView.DataContext = logViewModel;

            // 订阅查询历史日志事件
            logViewModel.OnQueryClicked = async () => { await ExeQueryClicked(); };

            // 注册图元属性、操作和操作原子
            RegisterPropertiesAndOprts();
        }

        // 注册
        private void RegisterPropertiesAndOprts()
        {
            //RegisterProperty(new MapObjPropertyInfo(MapObjPropEventConst.Prop_BackColor, MapObjPropEventConst.GetName(MapObjPropEventConst.Prop_BackColor), GriffinsBaseDataType.Integer, Guid.Empty, typeof(uint), true, true, new GriffinsBaseValue(Color.FromArgb(33, 0, 0, 0).ToColorString())));

            RegisterEvent(new MapObjEventInfo(MapObjPropEventConst.Event_MouseClick, MapObjPropEventConst.GetName(MapObjPropEventConst.Event_MouseClick), GriffinsBaseDataType.Object_Bytes, GraphMouseEventParam.Object_ID));

            RegisterOprtCellInfo(new MapOprtCellInfo(LogMapOprtCellConst.DisplayStyle_MapOprtCellID, ResourceA.DisplayStyle_MapOprtCellName));
            RegisterOprtCellInfo(new MapOprtCellInfo(LogMapOprtCellConst.NewLog_MapOprtCellID, ResourceA.NewLog_MapOprtCellName));

            RegisterProperty(new MapObjPropertyInfo(nameof(LogPropertyModelEdit.TextFont), ResourceA.TextFont, GriffinsBaseDataType.Object_Json, FontInfo.Object_ID, typeof(FontInfo), false, true, new GriffinsBaseValue(FontInfo.DefaultFont)));
            RegisterOprtInfo(new MapOprtInfo(nameof(LogPropertyModelEdit.TextFont), ResourceA.TextFont_MapOprtName, OprtExecKind.Normal, "", CreateMapOprtCellInstInfoList(LogMapOprtCellConst.DisplayStyle_MapOprtCellID)));

            RegisterProperty(new MapObjPropertyInfo(nameof(LogPropertyModelEdit.TextColor), ResourceA.TextColor, GriffinsBaseDataType.Integer, Guid.Empty, typeof(uint), true, true, new GriffinsBaseValue(Colors.Black.ToColorString())));
            RegisterOprtInfo(new MapOprtInfo(nameof(LogPropertyModelEdit.TextColor), ResourceA.TextColor_MapOprtName, OprtExecKind.Normal, "", CreateMapOprtCellInstInfoList(LogMapOprtCellConst.DisplayStyle_MapOprtCellID)));

            RegisterProperty(new MapObjPropertyInfo(nameof(LogPropertyModelEdit.BackColor), ResourceA.BackColor, GriffinsBaseDataType.Integer, Guid.Empty, typeof(uint), true, true, new GriffinsBaseValue(Color.FromArgb(33, 0, 0, 0).ToColorString())));
            RegisterOprtInfo(new MapOprtInfo(nameof(LogPropertyModelEdit.BackColor), ResourceA.BackColor_MapOprtName, OprtExecKind.Normal, "", CreateMapOprtCellInstInfoList(LogMapOprtCellConst.DisplayStyle_MapOprtCellID)));

            RegisterProperty(new MapObjPropertyInfo(nameof(LogPropertyModelEdit.NewLog), ResourceA.NewLog, GriffinsBaseDataType.Object_Json, LogItemModel.Object_ID, typeof(LogItemModel), false, true, new GriffinsBaseValue(new LogItemModel())));
            RegisterOprtInfo(new MapOprtInfo(nameof(LogPropertyModelEdit.NewLog), ResourceA.NewLog_MapOprtName, OprtExecKind.Normal, "", CreateMapOprtCellInstInfoList(LogMapOprtCellConst.NewLog_MapOprtCellID)));
        }

        // 创建单个操作原子实例
        private MapOprtCellInstInfoList CreateMapOprtCellInstInfoList(MapOprtCellID oprtCellID)
        {
            return CreateMapOprtCellInstInfoList(new[] { oprtCellID });
        }

        // 创建一组操作原子实例
        private MapOprtCellInstInfoList CreateMapOprtCellInstInfoList(IEnumerable<MapOprtCellID> oprtCellIDs)
        {
            var resultList = new MapOprtCellInstInfoList();

            if (oprtCellIDs != null && oprtCellIDs.Any())
            {
                resultList.AddRange(oprtCellIDs.Select(id => new MapOprtCellInstInfo()
                {
                    InstanceID = Guid.NewGuid(),
                    OprtCellID = id,
                    CfgInfo = null
                }));
            }

            return resultList;
        }

        #endregion

        #region 执行图元命令

        async Task ExeQueryClicked()
        {
            if (base.CallBack == null)
            {
                throw new InvalidOperationException("图元回调接口 (IFunctionalMapCellCallBack) 未初始化。");
            }

            var paramList = new GFBaseTypeParamValueList()
            {
                new GFBaseTypeParamValue("StartTime", new GriffinsBaseValue(logViewModel.SearchStartTime.DateTime)),
                new GFBaseTypeParamValue("EndTime", new GriffinsBaseValue(logViewModel.SearchEndTime.DateTime)),
                new GFBaseTypeParamValue("ModuleAlias", new GriffinsBaseValue(logViewModel.SearchModuleText ?? ""))
            };

            var (retCode, retVal) = await Task.Run(() =>
            {
                bool success = base.CallBack.ExecMapCellEvent(
                    EventCmdKind.MpCmdKind,
                    "QueryHistoryLog",
                    paramList,
                    out var tempRetVal);

                return (success, tempRetVal);
            });

            if (retCode == false) throw new TimeoutException("执行查询历史日志失败");
            if (retVal == null || retVal.Count == 0 || retVal[0].Value == null) throw new InvalidOperationException("查询结果为空");

            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                logViewModel.HistoricalLogs.Clear();

                if (retVal != null && retVal.Count > 0)
                {
                    foreach (var propValue in retVal)
                    {
                        if (propValue?.Value == null) continue;

                        LogItemModel newLog = new LogItemModel();
                        ((IGriffinsBaseValue)newLog).PopulateFromBaseValue(propValue.Value);

                        logViewModel.HistoricalLogs.Add(newLog);
                    }
                }
            });
        }

        #endregion

        #region 数据同步与更新

        /// <summary>
        /// 界面数据对象属性值 => 图元属性
        /// </summary>
        /// <param name="gFBaseTypePropValues">界面数据对象属性值列表</param>
        /// <returns></returns>
        protected override bool SetUIDataObjPropValues(GFBaseTypePropValueList gFBaseTypePropValues)
        {
            if (gFBaseTypePropValues == null || gFBaseTypePropValues.Count == 0) return false;

            bool setValueSuccess = true;
            foreach (GFBaseTypePropValue gFBaseTypePropValue in gFBaseTypePropValues)
            {
                if (gFBaseTypePropValue == null)
                {
                    setValueSuccess = false;
                    continue;
                }

                string propId = gFBaseTypePropValue.PropertyID.ToString();
                GriffinsBaseValue propValue = gFBaseTypePropValue.Value;

                if (string.IsNullOrWhiteSpace(propId) || propValue == null)
                {
                    setValueSuccess = false;
                    continue;
                }

                switch (propId)
                {
                    case "NewLog":
                        LogItemModel newLog = new LogItemModel();
                        ((IGriffinsBaseValue)newLog).PopulateFromBaseValue(propValue);
                        LogPropertyModelEdit.NewLog = newLog;
                        break;

                    default:
                        setValueSuccess &= LogPropertyModelEdit.SetPropertyValue(propId, propValue);
                        break;
                }
            }

            return setValueSuccess;
        }


        /// <summary>
        /// 图元属性值改变后的处理
        /// </summary>
        /// <param name="propertyID">属性ID</param>
        /// <param name="propertyValue">属性值</param>
        /// <remarks>
        /// 1.属性面板直接改变图元属性时，调用操作原子更新到VM
        ///   后端推送数据改变图元属性时，调用操作原子更新到VM
        /// 2.界面操作改变图元属性时，调用回调接口推到后端
        /// </remarks>
		protected override void AfterPropertyChanged(string propertyID, GriffinsBaseValue propertyValue)
        {
            CallBack?.ExecOprt(propertyID);

            if (LogPropertyModelEdit.IsRuning)
            {
                CallBack?.UpdateUIDataObjPropValues(new GFBaseTypePropValueList());
            }
        }

        public override GriffinsBaseValue ConvertPropertyValue(string propertyID, object propertyValue)
        {
            return null!;
        }

        #endregion

        #region 操作原子定义与执行

        /// <summary>
        /// 执行图元内部操作原子
        /// </summary>
        /// <param name="mapOprtCellInstInfo">图元内部操作原子信息</param>
        /// <returns>True:已经找到该操作原子并设置，false:没有该操作原子</returns>
        protected override bool ExecOprtCell(MapOprtCellInstInfo mapOprtCellInstInfo)
        {
            if (mapOprtCellInstInfo.OprtCellID == LogMapOprtCellConst.DisplayStyle_MapOprtCellID)
            {
                if (MapOprtCellExectorDict.TryGetValue(mapOprtCellInstInfo.InstanceID, out var mapOprtCellExector))
                {
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                else
                {
                    mapOprtCellExector = new DisplayStyleMapOprtCellExector();
                    mapOprtCellExector.Init(IMapOprtCellExectorCallBack);
                    MapOprtCellExectorDict.TryAdd(mapOprtCellInstInfo.InstanceID, mapOprtCellExector);
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                return true;
            }

            if (mapOprtCellInstInfo.OprtCellID == LogMapOprtCellConst.NewLog_MapOprtCellID)
            {
                if (MapOprtCellExectorDict.TryGetValue(mapOprtCellInstInfo.InstanceID, out var mapOprtCellExector))
                {
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                else
                {
                    mapOprtCellExector = new NewLogMapOprtCellExector();
                    mapOprtCellExector.Init(IMapOprtCellExectorCallBack);
                    MapOprtCellExectorDict.TryAdd(mapOprtCellInstInfo.InstanceID, mapOprtCellExector);
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                return true;
            }

            return base.ExecOprtCell(mapOprtCellInstInfo);
        }

        /// <summary>
        /// 显示样式操作原子执行器对象定义
        /// </summary>
        private class DisplayStyleMapOprtCellExector : IMapOprtCellExector
        {
            private IMapOprtCellExectorCallBack callBack = null!;

            public DisplayStyleMapOprtCellExector() { }

            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack) => this.callBack = callBack;

            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                if (callBack.GetMapCellVMObjInstance() is LogViewModel vm)
                {
                    PostToUI(() =>
                    {
                        vm.RefreshDisplayStyle();
                    });
                }
            }
        }

        /// <summary>
        /// 新日志操作原子执行器对象定义
        /// </summary>
        private class NewLogMapOprtCellExector : IMapOprtCellExector
        {
            private IMapOprtCellExectorCallBack callBack = null!;

            public NewLogMapOprtCellExector() { }

            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack) => this.callBack = callBack;

            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                if (callBack.GetMapCellVMObjInstance() is LogViewModel vm)
                {
                    PostToUI(() =>
                    {
                        vm.AppendRealTimeLog();
                    });
                }
            }
        }

        private static void PostToUI(Action action)
        {
            if (Dispatcher.UIThread.CheckAccess()) action();
            else Dispatcher.UIThread.Post(action);
        }

        #endregion

        #region 生命周期与数据持久化

        protected override void OnInit() => base.OnInit();

        public override void OnDispose()
        {
            logView.DataContext = null;
            logViewModel?.Dispose();
            base.OnDispose();
        }

        protected override void OnReadDrawInfoFromBytes(GriffinsXmlReader br)
        {
            base.OnReadDrawInfoFromBytes(br);
            var propertyEditModelBase = JsonObjConvert.FromJSon<LogPropertyModelEdit>(br.ReadString("PropertyEditModelBase"));
            (PropertyEditModelBase as LogPropertyModelEdit)!.CopyFrom(propertyEditModelBase);
            var propertyBindEditModelBase = JsonObjConvert.FromJSon<LogPropertyBindEditModel>(br.ReadString("PropertyBindEditModelBase"));
            (PropertyBindEditModelBase as LogPropertyBindEditModel)!.CopyFrom(propertyBindEditModelBase);
            var eventBindEditModel = System.Text.Json.JsonSerializer.Deserialize<EventBindEditModel>(br.ReadString("EventBindEditModel"));
            EventBindEditModel.CopyFrom(eventBindEditModel!);
        }

        protected override void OnWriteDrawInfoToBytes(GriffinsXmlWriter bw)
        {
            base.OnWriteDrawInfoToBytes(bw);
            bw.Write("PropertyEditModelBase", JsonObjConvert.ToJSon(PropertyEditModelBase));
            bw.Write("PropertyBindEditModelBase", JsonObjConvert.ToJSon(PropertyBindEditModelBase));
            bw.Write("EventBindEditModel", System.Text.Json.JsonSerializer.Serialize(EventBindEditModel));
        }

        protected override void OnCopyFrom(FunctionalCellBase source)
        {
            var src = source as MapCellLogCtlObj;
            base._CopyFrom(src);
            PropertyEditModelBase.CopyFrom(source.PropertyEditModelBase);
            PropertyBindEditModelBase.CopyFrom(source.PropertyBindEditModelBase);
            EventBindEditModel?.CopyFrom(source.EventBindEditModel);
        }

        protected override object OnGetView() => logView;

        protected override object OnGetViewModel() => logViewModel;

        public override PropertyEditModelBase CreatePropertyModelEditBase() => new LogPropertyModelEdit();

        public override PropertyBindEditModelBase CreatePropertyBindEditModelBase() => new LogPropertyBindEditModel();

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

        public override string ToString() => ResourceA.Log;

        #endregion
    }

    /// <summary>
    /// 日志图元属性
    /// </summary>
    [Serializable]
    [MapPropertyOrder]
    [CategoryPriority("图元信息", 1)]
    [CategoryPriority("外观样式", 2)]
    public class LogPropertyModelEdit : FunctionalCellPropertyModelEdit
    {
        public LogPropertyModelEdit()
        {
            TextFont = new FontInfo(FontManager.Current.DefaultFontFamily, 14.0, FontWeight.Normal, FontStyle.Normal);
        }

        private void textFont_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            // 通知PropertyGrid：嵌套对象的属性已变化
            RaisePropertyChanged(nameof(TextFont));
        }

        private FontInfo _textFont = FontInfo.DefaultFont;
        [DisplayName("文字字体")]
        [Category("外观样式")]
        [PropertySortOrder(1)]
        public FontInfo TextFont
        {
            get
            {
                return _textFont;
            }
            set
            {
                //SetProperty(ref _textFont, value);
                // 1. 旧实例：取消事件监听（避免内存泄漏+无效监听）
                if (_textFont != null)
                    _textFont.PropertyChanged -= textFont_PropertyChanged;

                // 2. 设置新值，并触发自身的PropertyChanged（关键）
                if (SetProperty(ref _textFont, value)) // 用基类的SetProperty，而非直接赋值
                {
                    // 3. 新实例：绑定事件监听
                    if (_textFont != null)
                        _textFont.PropertyChanged += textFont_PropertyChanged;
                }
            }
        }

        private Color _textColor = Colors.Black;
        [DisplayName("文本颜色")]
        [Category("外观样式")]
        [PropertySortOrder(2)]
        [JsonConverter(typeof(ColorConvert))]
        public Color TextColor
        {
            get => _textColor;
            set => SetProperty(ref _textColor, value);
        }

        private Color _backColor = Color.FromArgb(33, 0, 0, 0);
        /// <summary>
        /// 背景颜色
        /// </summary>
        [DisplayName("背景颜色")]
        [Category("外观样式")]
        [PropertySortOrder(3)]
        [JsonConverter(typeof(ColorConvert))]
        public Color BackColor
        {
            get
            {
                return _backColor;
            }
            set
            {
                SetProperty(ref _backColor, value);
            }
        }

        private LogItemModel _newLog = new LogItemModel();
        [Browsable(false)]
        public LogItemModel NewLog
        {
            get => _newLog;
            set => SetProperty(ref _newLog, value);
        }

        public override bool SetPropertyValue(string propertyID, GriffinsBaseValue propertyVal)
        {
            if (string.Compare(propertyID, nameof(NewLog)) == 0)
            {
                if (propertyVal is { })
                {
                    LogItemModel newLog = new LogItemModel();
                    ((IGriffinsBaseValue)newLog).PopulateFromBaseValue(propertyVal);
                    NewLog = newLog;
                }
                else
                {
                    NewLog = new LogItemModel();
                }
                return true;
            }
            if (string.Compare(propertyID, nameof(BackColor)) == 0)
            {
                if (propertyVal is { })
                {
                    var color1 = propertyVal.ToPrimitiveValue<string>();
                    BackColor = Color.Parse(color1);
                }
                else
                {
                    BackColor = Color.FromArgb(33, 0, 0, 0);
                }
                return true;
            }
            if (string.Compare(propertyID, nameof(TextColor)) == 0)
            {
                if (propertyVal is { })
                {
                    var color1 = propertyVal.ToPrimitiveValue<string>();
                    TextColor = Color.Parse(color1);
                }
                else
                {
                    TextColor = Colors.Black;
                }
                return true;
            }
            if (string.Compare(propertyID, nameof(TextFont)) == 0)
            {
                if (propertyVal is { })
                {
                    FontInfo fontInfo = new FontInfo();
                    ((IGriffinsBaseValue)fontInfo).PopulateFromBaseValue(propertyVal);
                    TextFont = fontInfo;
                }
                else
                {
                    TextFont = FontInfo.DefaultFont;
                }
                return true;
            }

            return base.SetPropertyValue(propertyID, propertyVal);
        }

        public void CopyFrom(LogPropertyModelEdit source)
        {
            base.CopyFrom(source);

            if (source.TextFont != null)
            {
                this.TextFont = new FontInfo(source.TextFont.FontFamily, source.TextFont.FontSize, source.TextFont.FontWeight, source.TextFont.FontStyle);
            }
            if (source.NewLog != null)
            {
                this.NewLog = new LogItemModel();
                this.NewLog.Time = source.NewLog.Time;
                this.NewLog.ModuleAlias = source.NewLog.ModuleAlias;
                this.NewLog.LogLevel = source.NewLog.LogLevel;
                this.NewLog.ErrorCode = source.NewLog.ErrorCode;
                this.NewLog.ThreadID = source.NewLog.ThreadID;
                this.NewLog.LogText = source.NewLog.LogText;
            }
            this.TextColor = source.TextColor;
            this.BackColor = source.BackColor;
        }
    }

    /// <summary>
    /// 日志图元绑定属性
    /// </summary>
    [Serializable]
    [MapPropertyOrder]
    [CategoryPriority("点位信息", 1)]
    [CategoryPriority("绑定信息", 2)]
    public class LogPropertyBindEditModel : FunctionalCellPropertyBindEditModel
    {
        private PropertyBindInfo _textFont = new PropertyBindInfo(GriffinsBaseDataType.Object_Json, FontInfo.Object_ID);
        private PropertyBindInfo _textColor = new PropertyBindInfo(GriffinsBaseDataType.Integer);
        private PropertyBindInfo _backColor = new PropertyBindInfo(GriffinsBaseDataType.Integer);
        private PropertyBindInfo _newLog = new PropertyBindInfo(GriffinsBaseDataType.Object_Json, LogItemModel.Object_ID);


        [DisplayName("文本字体")]
        [Category("绑定信息")]
        [PropertySortOrder(6)]
        [Browsable(false)]
        public PropertyBindInfo TextFont
        {
            get => _textFont;
            set => SetProperty(ref _textFont, value, "TextFont");
        }

        [DisplayName("文本颜色")]
        [Category("绑定信息")]
        [PropertySortOrder(7)]
        [Browsable(false)]
        public PropertyBindInfo TextColor
        {
            get => _textColor;
            set => SetProperty(ref _textColor, value, "TextColor");
        }

        [DisplayName("背景颜色")]
        [Category("绑定信息")]
        [PropertySortOrder(8)]
        [Browsable(false)]
        public PropertyBindInfo BackColor
        {
            get => _backColor;
            set => SetProperty(ref _backColor, value, "BackColor");
        }

        [DisplayName("新增日志")]
        [Category("绑定信息")]
        [PropertySortOrder(9)]
        [Browsable(false)]
        public PropertyBindInfo NewLog
        {
            get => _newLog;
            set => SetProperty(ref _newLog, value, "NewLog");
        }

        public void CopyFrom(LogPropertyBindEditModel source)
        {
            base.CopyFrom(source);
            this.TextFont = source.TextFont;
            this.TextColor = source.TextColor;
            this.BackColor = source.BackColor;
            this.NewLog = source.NewLog;
        }
    }

    /// <summary>
    /// 日志模型
    /// </summary>
    public class LogItemModel: IGriffinsBaseValue
    {
        public static readonly Guid Object_ID = new Guid("{8922617D-1F3E-4256-8578-9C3B907B76FC}");

        public string Time { get; set; } = string.Empty;
        public string ModuleAlias { get; set; } = string.Empty;
        public string LogLevel { get; set; } = string.Empty;
        public string ErrorCode { get; set; } = string.Empty;
        public string ThreadID { get; set; } = string.Empty;
        public string LogText { get; set; } = string.Empty;

        // UI 绑定的颜色属性
        [JsonIgnore]
        public IBrush RowColorBrush => LogLevel == "Error" || LogLevel == "Fatal" ? Brushes.LightCoral :
                                       LogLevel == "Warn" ? Brushes.LightGoldenrodYellow : Brushes.Transparent;

        #region IGriffinsBaseValue 实现

        [JsonIgnore] bool IGriffinsBaseValue.IsObject_Byte => false;

        Guid IGriffinsBaseValue.GetObject_ID() => Object_ID;

        void IGriffinsBaseValue.PopulateFromBaseValue(GriffinsBaseValue baseValue)
        {
            if (baseValue?.Val is ObjectValue_Json jsonValue && jsonValue.Object_ID == Object_ID)
            {
                var data = JsonConvert.DeserializeObject<LogItemModel>(jsonValue.JsonVal);
                if (data != null)
                {
                    this.Time = data.Time;
                    this.ModuleAlias = data.ModuleAlias;
                    this.LogLevel = data.LogLevel;
                    this.ErrorCode = data.ErrorCode;
                    this.ThreadID = data.ThreadID;
                    this.LogText = data.LogText;
                }
            }
        }

        GriffinsBaseValue IGriffinsBaseValue.ToBaseValue()
            => GriffinsBaseValue.Create(new ObjectValue_Json(Object_ID) { JsonVal = JsonConvert.SerializeObject(this) });

        #endregion
    }
}
