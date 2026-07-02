using Avalonia.Media;
using GF_Gereric;
using GKG.Map.AuxiliaryFunctionsFuncCtlMapCell.View;
using GKG.Map.AuxiliaryFunctionsFuncCtlMapCell.ViewModel;
using Griffins;
using Griffins.Map;
using Griffins.Map.Cmd;
using Griffins.Map.UI;
using Griffins.UI2;
using Newtonsoft.JsonG;
using System;
using System.ComponentModel;
using System.Linq;

namespace GKG.Map.AuxiliaryFunctionsFuncCtlMapCell
{
    class MapCellAuxiliaryFunctionsCtlObj : FunctionalCellBase
    {
        public const string Prop_PressureValue = "AuxiliaryFunctionsValue";

        private AuxiliaryFunctionsView view;

        private AuxiliaryFunctionsViewModel auxiliaryFunctionsViewModel;

        static MapCellAuxiliaryFunctionsCtlObj()
        {

        }
        public MapCellAuxiliaryFunctionsCtlObj(MapObjID mapCellID, string mapCellName)
            : this(mapCellID, mapCellName, false)
        {
        }

        public MapCellAuxiliaryFunctionsCtlObj(MapObjID mapCellID, string mapCellName, bool designTime)
        {
            PropertyEditModelBase = CreatePropertyModelEditBase();
            PropertyBindEditModelBase = CreatePropertyBindEditModelBase();
            EventBindEditModel = CreateEventBindEditModel();

            base.SetID(mapCellID);
            base.SetName(mapCellName);
            view = new AuxiliaryFunctionsView();

            RegisterProperty(new MapObjPropertyInfo(MapObjPropEventConst.Prop_BackColor, MapObjPropEventConst.GetName(MapObjPropEventConst.Prop_BackColor), GriffinsBaseDataType.Integer, Guid.Empty, typeof(uint), true, true, new GriffinsBaseValue(Color.FromArgb(33, 0, 0, 0).ToColorString())));
            RegisterProperty(new MapObjPropertyInfo(nameof(AuxiliaryFunctionsPropertyModelEdit.TextColor), MapObjPropEventConst.GetName(MapObjPropEventConst.Prop_TextColor), GriffinsBaseDataType.Integer, Guid.Empty, typeof(uint), true, true, new GriffinsBaseValue(Colors.Black.ToColorString())));
            RegisterProperty(new MapObjPropertyInfo(nameof(AuxiliaryFunctionsPropertyModelEdit.TextFont), ResourceA.TextFont, GriffinsBaseDataType.Object_Json, FontInfo.Object_ID, typeof(FontInfo), false, true, new GriffinsBaseValue(FontInfo.DefaultFont)));
            //RegisterProperty(new MapObjPropertyInfo(MapObjPropEventConst.Prop_Cursor, MapObjPropEventConst.GetName(MapObjPropEventConst.Prop_Cursor), MapCellPropDataType.Integer, Guid.Empty, typeof(int), true, true, new MapCellPropValue(CtlCellCursor.Default)));
            RegisterProperty(new MapObjPropertyInfo(Prop_PressureValue, ResourceA.PressureValue, GriffinsBaseDataType.Decimal, Guid.Empty, typeof(decimal), true, true, new GriffinsBaseValue(0)));
            
            RegisterEvent(new MapObjEventInfo(MapObjPropEventConst.Event_MouseClick, MapObjPropEventConst.GetName(MapObjPropEventConst.Event_MouseClick), GriffinsBaseDataType.Object_Bytes, GraphMouseEventParam.Object_ID));
            
            RegisterOprtCellInfo(new MapOprtCellInfo(AuxiliaryFunctionsMapOprtCellConst.TextColor_MapOprtCellID, ResourceA.TextColor_MapOprtCellName));
            RegisterOprtCellInfo(new MapOprtCellInfo(AuxiliaryFunctionsMapOprtCellConst.BackColor_MapOprtCellID, ResourceA.BackColor_MapOprtCellName));
            RegisterOprtCellInfo(new MapOprtCellInfo(AuxiliaryFunctionsMapOprtCellConst.PressureValue_MapOprtCellID, ResourceA.PressureValue_MapOprtCellName));
            RegisterOprtCellInfo(new MapOprtCellInfo(AuxiliaryFunctionsMapOprtCellConst.TextFont_MapOprtCellID, ResourceA.TextFont_MapOprtCellName));
            
            RegisterOprtInfo(new MapOprtInfo(nameof(AuxiliaryFunctionsPropertyModelEdit.TextColor), ResourceA.TextColor_MapOprtName, OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
            {
                new MapOprtCellInstInfo()
                {
                  InstanceID=Guid.NewGuid(),
                  OprtCellID=AuxiliaryFunctionsMapOprtCellConst.TextColor_MapOprtCellID,
                  CfgInfo=null
                }
            }));
            RegisterOprtInfo(new MapOprtInfo(nameof(AuxiliaryFunctionsPropertyModelEdit.BackColor), ResourceA.BackColor_MapOprtName, OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
            {
                new MapOprtCellInstInfo()
                {
                  InstanceID=Guid.NewGuid(),
                  OprtCellID=AuxiliaryFunctionsMapOprtCellConst.BackColor_MapOprtCellID,
                  CfgInfo=null
                }
            }));
            RegisterOprtInfo(new MapOprtInfo(nameof(AuxiliaryFunctionsPropertyModelEdit.PressureValue), ResourceA.PressureValue_MapOprtName, OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
            {
                new MapOprtCellInstInfo()
                {
                  InstanceID=Guid.NewGuid(),
                  OprtCellID=AuxiliaryFunctionsMapOprtCellConst.PressureValue_MapOprtCellID,
                  CfgInfo=null
                }
            }));
            RegisterOprtInfo(new MapOprtInfo(nameof(AuxiliaryFunctionsPropertyModelEdit.TextFont), ResourceA.TextFont_MapOprtName, OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
            {
                new MapOprtCellInstInfo()
                {
                  InstanceID=Guid.NewGuid(),
                  OprtCellID=AuxiliaryFunctionsMapOprtCellConst.TextFont_MapOprtCellID,
                  CfgInfo=null
                }
            }));

            

            (this as IMapCellTypeBase).Name = ResourceA.AuxiliaryFunctions;
            auxiliaryFunctionsViewModel = new AuxiliaryFunctionsViewModel(AuxiliaryFunctionsPropertyModelEdit, () => new MapCmdExector(base.CallBack.INorthSvrCommandExec), PropertyBindEditModelBase.MpNo);
            view.DataContext = auxiliaryFunctionsViewModel;
        }

        private void clickExec()
        {
            EventCmdInfo? eventCmdInfo = EventBindEditModel.EventCmdInfos.FirstOrDefault
                (info => info.EventID == MapObjPropEventConst.Event_MouseClick);
            if (eventCmdInfo != null)
            {
                GFBaseTypeParamValueList cmdParam = null;
                if (!string.IsNullOrWhiteSpace(eventCmdInfo.CmdParam))
                {
                    cmdParam = new GFBaseTypeParamValueList();
                    cmdParam.FromJson(eventCmdInfo.CmdParam);
                }
                CallBack?.ExecMapCellEvent(eventCmdInfo.EventCmdKind, eventCmdInfo.CmdID, cmdParam, out GFBaseTypeParamValueList retVal);
            }
        }

        [Browsable(false)]
        public AuxiliaryFunctionsPropertyModelEdit AuxiliaryFunctionsPropertyModelEdit
        {
            get { return PropertyEditModelBase as AuxiliaryFunctionsPropertyModelEdit; }
        }

        public override bool SetPropertyValue(string propertyID, GriffinsBaseValue propertyVal, bool isRuning)
        {
            AuxiliaryFunctionsPropertyModelEdit.IsRuning = isRuning;
            return AuxiliaryFunctionsPropertyModelEdit.SetPropertyValue(propertyID, propertyVal);
        }

        /// <summary>
        /// 执行图元内部操作原子
        /// </summary>
        /// <param name="mapOprtCellInstInfo">图元内部操作原子信息</param>
        /// <returns>True:已经找到该操作原子并设置，false:没有该操作原子</returns>
        protected override bool ExecOprtCell(MapOprtCellInstInfo mapOprtCellInstInfo)
        {
            if (mapOprtCellInstInfo.OprtCellID == AuxiliaryFunctionsMapOprtCellConst.TextColor_MapOprtCellID)
            {
                if (MapOprtCellExectorDict.TryGetValue(mapOprtCellInstInfo.InstanceID, out IMapOprtCellExector mapOprtCellExector))
                {
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                else
                {
                    mapOprtCellExector = new TextColorMapOprtCellExector(this);
                    mapOprtCellExector.Init(IMapOprtCellExectorCallBack);
                    MapOprtCellExectorDict.TryAdd(mapOprtCellInstInfo.InstanceID, mapOprtCellExector);
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                return true;
            }
            if (mapOprtCellInstInfo.OprtCellID == AuxiliaryFunctionsMapOprtCellConst.BackColor_MapOprtCellID)
            {
                if (MapOprtCellExectorDict.TryGetValue(mapOprtCellInstInfo.InstanceID, out IMapOprtCellExector mapOprtCellExector))
                {
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                else
                {
                    mapOprtCellExector = new BackColorMapOprtCellExector(this);
                    mapOprtCellExector.Init(IMapOprtCellExectorCallBack);
                    MapOprtCellExectorDict.TryAdd(mapOprtCellInstInfo.InstanceID, mapOprtCellExector);
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                return true;
            }
            if (mapOprtCellInstInfo.OprtCellID == AuxiliaryFunctionsMapOprtCellConst.PressureValue_MapOprtCellID)
            {
                if (MapOprtCellExectorDict.TryGetValue(mapOprtCellInstInfo.InstanceID, out IMapOprtCellExector mapOprtCellExector))
                {
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                else
                {
                    mapOprtCellExector = new PressureValueMapOprtCellExector(this);
                    mapOprtCellExector.Init(IMapOprtCellExectorCallBack);
                    MapOprtCellExectorDict.TryAdd(mapOprtCellInstInfo.InstanceID, mapOprtCellExector);
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                return true;
            }
            if (mapOprtCellInstInfo.OprtCellID == AuxiliaryFunctionsMapOprtCellConst.TextFont_MapOprtCellID)
            {
                if (MapOprtCellExectorDict.TryGetValue(mapOprtCellInstInfo.InstanceID, out IMapOprtCellExector mapOprtCellExector))
                {
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                else
                {
                    mapOprtCellExector = new TextFontMapOprtCellExector(this);
                    mapOprtCellExector.Init(IMapOprtCellExectorCallBack);
                    MapOprtCellExectorDict.TryAdd(mapOprtCellInstInfo.InstanceID, mapOprtCellExector);
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                return true;
            }
            return base.ExecOprtCell(mapOprtCellInstInfo);
        }

        /// <summary>
        /// 设置界面数据对象属性值
        /// </summary>
        /// <param name="gFBaseTypePropValues">界面数据对象属性值列表</param>
        /// <returns>true:已执行，false:未执行</returns>
        protected override bool SetUIDataObjPropValues(GFBaseTypePropValueList gFBaseTypePropValues)
        {
            foreach (GFBaseTypePropValue gFBaseTypePropValue in gFBaseTypePropValues)
            {
                //在这里需要将界面数据对象属性值转换为图元属性值
                if (string.Compare(gFBaseTypePropValue.PropertyID.ToString(), "AAA") == 0)
                {
                    //PressurePropertyModelEdit.BackColor=
                }
            }
            return true;
        }

        /// <summary>
        /// 属性值改变后需要做的处理，如：需要执行什么操作，如果是在View改变数据，调用回调接口将数据写到后端
        /// </summary>
        /// <param name="propertyID"></param>
        /// <param name="propertyValue"></param>
        protected override void AfterPropertyChanged(string propertyID, GriffinsBaseValue propertyValue)
        {
            base.AfterPropertyChanged(propertyID, propertyValue);
            if (string.Compare(propertyID, nameof(AuxiliaryFunctionsPropertyModelEdit.TextColor)) == 0)
            {
                CallBack?.ExecOprt(nameof(AuxiliaryFunctionsPropertyModelEdit.TextColor));
            }
            if (string.Compare(propertyID, nameof(AuxiliaryFunctionsPropertyModelEdit.BackColor)) == 0)
            {
                CallBack?.ExecOprt(nameof(AuxiliaryFunctionsPropertyModelEdit.BackColor));
            }
            if (string.Compare(propertyID, nameof(AuxiliaryFunctionsPropertyModelEdit.PressureValue)) == 0)
            {
                CallBack?.ExecOprt(nameof(AuxiliaryFunctionsPropertyModelEdit.PressureValue));
            }
            if (string.Compare(propertyID, nameof(AuxiliaryFunctionsPropertyModelEdit.TextFont)) == 0)
            {
                CallBack?.ExecOprt(nameof(AuxiliaryFunctionsPropertyModelEdit.TextFont));
            }
            if (!AuxiliaryFunctionsPropertyModelEdit.IsRuning)
            {
                //需要在此次将图元属性值转换为界面数据对象属性值
                CallBack?.UpdateUIDataObjPropValues(new GFBaseTypePropValueList());
            }
        }

        /// <summary>
        /// 从字节流中读画图信息（必须先调用基类的OnReadDrawInfoFromBytes，必须保证写入数据和读出数据的顺序一致）
        /// </summary>
        /// <param name="br">字节流读取对象</param>
        protected override void OnReadDrawInfoFromBytes(GriffinsXmlReader br)
        {
            base.OnReadDrawInfoFromBytes(br);
            var propertyEditModelBase = JsonObjConvert.FromJSon<AuxiliaryFunctionsPropertyModelEdit>(br.ReadString("PropertyEditModelBase"));
            (PropertyEditModelBase as AuxiliaryFunctionsPropertyModelEdit).CopyFrom(propertyEditModelBase);
            var propertyBindEditModelBase = JsonObjConvert.FromJSon<AuxiliaryFunctionsPropertyBindEditModel>(br.ReadString("PropertyBindEditModelBase"));
            (PropertyBindEditModelBase as AuxiliaryFunctionsPropertyBindEditModel).CopyFrom(propertyBindEditModelBase);
            var eventBindEditModel = System.Text.Json.JsonSerializer.Deserialize<EventBindEditModel>(br.ReadString("EventBindEditModel"));
            EventBindEditModel.CopyFrom(eventBindEditModel);
        }

        /// <summary>
        /// 当把画图信息写入到字节流中（必须先调用基类的OnWriteDrawInfoToBytes，必须保证写入数据和读出数据的顺序一致）
        /// </summary>
        /// <param name="bw">字节流写入对象</param>
        protected override void OnWriteDrawInfoToBytes(GriffinsXmlWriter bw)
        {
            base.OnWriteDrawInfoToBytes(bw);
            bw.Write("PropertyEditModelBase", JsonObjConvert.ToJSon(PropertyEditModelBase));
            bw.Write("PropertyBindEditModelBase", JsonObjConvert.ToJSon(PropertyBindEditModelBase));
            bw.Write("EventBindEditModel", System.Text.Json.JsonSerializer.Serialize(EventBindEditModel));
        }

        protected override void OnCopyFrom(FunctionalCellBase source)
        {
            MapCellAuxiliaryFunctionsCtlObj mapCellPressureCtlObj = (source as MapCellAuxiliaryFunctionsCtlObj);
            base._CopyFrom(mapCellPressureCtlObj);
            (PropertyEditModelBase).CopyFrom(source.PropertyEditModelBase);
            (PropertyBindEditModelBase).CopyFrom(source.PropertyBindEditModelBase);
            EventBindEditModel.CopyFrom(source.EventBindEditModel);
        }

        /// <summary>
        /// 初始化时
        /// </summary>
        protected override void OnInit()
        {

        }

        protected override object OnGetView()
        {
                return view;
        }

        protected override object OnGetViewModel()
        {
            return auxiliaryFunctionsViewModel;
        }

        /// <summary>
        /// 创建图元属性编辑模型对象
        /// </summary>
        /// <returns>图元属性编辑模型对象</returns>
        public override PropertyEditModelBase CreatePropertyModelEditBase()
        {
            return new AuxiliaryFunctionsPropertyModelEdit();
        }

        /// <summary>
        /// 创建图元属性绑定编辑模型对象
        /// </summary>
        /// <returns>图元属性绑定编辑模型对象</returns>
        public override PropertyBindEditModelBase CreatePropertyBindEditModelBase()
        {
            //气压值绑定到上料管理点的剩余数量属性
            var auxiliaryFunctionsPropertyBindEditModel = new AuxiliaryFunctionsPropertyBindEditModel();
            return auxiliaryFunctionsPropertyBindEditModel;
        }

        /// <summary>
        /// 创建图元事件绑定编辑模型对象
        /// </summary>
        /// <returns>图元事件绑定编辑模型对象</returns>
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

        public override void OnZoomChanged()
        {
            SetButtonTextFont();
        }

        internal void SetButtonTextFont()
        {
            double size = base.CallBack?.Calc?.CalcZoomVal((decimal)this.AuxiliaryFunctionsPropertyModelEdit.TextFont.FontSize) ?? this.AuxiliaryFunctionsPropertyModelEdit.TextFont.FontSize;
            if (size < 2)
                size = 2;
            FontInfo font = new FontInfo(this.AuxiliaryFunctionsPropertyModelEdit.TextFont.FontFamily, size, this.AuxiliaryFunctionsPropertyModelEdit.TextFont.FontWeight, this.AuxiliaryFunctionsPropertyModelEdit.TextFont.FontStyle);
            //this.auxiliaryFunctionsViewModel.TextFont = font;
        }

        public override string ToString()
        {
            return "辅助功能";
        }

        #region 操作原子执行对象

        /// <summary>
        /// 文本颜色操作原子执行对象
        /// </summary>
        private class TextColorMapOprtCellExector : IMapOprtCellExector
        {
            private MapCellAuxiliaryFunctionsCtlObj mapCellPressureCtlObj;

            private IMapOprtCellExectorCallBack callBack;

            public TextColorMapOprtCellExector(MapCellAuxiliaryFunctionsCtlObj mapCellPressureCtlObj)
            {
                this.mapCellPressureCtlObj = mapCellPressureCtlObj;
            }
            #region  IMapOprtCellExector 成员

            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack)
            {
                this.callBack = callBack;
            }

            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                object viewModel = callBack.GetMapCellVMObjInstance();
                if (viewModel != null && viewModel is AuxiliaryFunctionsViewModel pressureViewModel)
                {
                    GriffinsBaseValue mapCellPropValue = callBack.GetMapCellPropValue(nameof(AuxiliaryFunctionsPropertyModelEdit.TextColor));
                    if (mapCellPropValue is { })
                    {
                        var color = mapCellPropValue.ToPrimitiveValue<string>();
                        //pressureViewModel.TextColor = Color.Parse(color);
                    }
                }
            }

            #endregion
        }

        /// <summary>
        /// 背景颜色操作原子执行对象
        /// </summary>
        private class BackColorMapOprtCellExector : IMapOprtCellExector
        {
            private MapCellAuxiliaryFunctionsCtlObj mapCellPressureCtlObj;

            private IMapOprtCellExectorCallBack callBack;

            public BackColorMapOprtCellExector(MapCellAuxiliaryFunctionsCtlObj mapCellPressureCtlObj)
            {
                this.mapCellPressureCtlObj = mapCellPressureCtlObj;
            }

            #region  IMapOprtCellExector 成员

            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack)
            {
                this.callBack = callBack;
            }

            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                object viewModel = callBack.GetMapCellVMObjInstance();
                if (viewModel != null && viewModel is AuxiliaryFunctionsViewModel pressureViewModel)
                {
                    GriffinsBaseValue mapCellPropValue = callBack.GetMapCellPropValue(nameof(AuxiliaryFunctionsPropertyModelEdit.BackColor));
                    if (mapCellPropValue is { })
                    {
                        var color = mapCellPropValue.ToPrimitiveValue<string>();
                        //pressureViewModel.BackColor = Color.Parse(color);
                    }
                }
            }

            #endregion
        }

        /// <summary>
        /// 气压值操作原子执行对象
        /// </summary>
        private class PressureValueMapOprtCellExector : IMapOprtCellExector
        {
            private MapCellAuxiliaryFunctionsCtlObj mapCellPressureCtlObj;

            private IMapOprtCellExectorCallBack callBack;


            public PressureValueMapOprtCellExector(MapCellAuxiliaryFunctionsCtlObj mapCellPressureCtlObj)
            {
                this.mapCellPressureCtlObj = mapCellPressureCtlObj;
            }
            #region  IMapOprtCellExector 成员

            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack)
            {
                this.callBack = callBack;
            }

            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                if (cfg != null)
                {
                    //paramViewModel.FromBytes(cfg);
                }
                object viewModel = callBack.GetMapCellVMObjInstance();
                if (viewModel != null && viewModel is AuxiliaryFunctionsViewModel pressureViewModel)
                {
                    GriffinsBaseValue mapCellPropValue = callBack.GetMapCellPropValue(nameof(AuxiliaryFunctionsPropertyModelEdit.PressureValue));
                    if (mapCellPropValue is { })
                    {
                        
                    }
                }
            }

            #endregion
        }

        /// <summary>
        /// 文本字体操作原子执行对象
        /// </summary>
        private class TextFontMapOprtCellExector : IMapOprtCellExector
        {
            private MapCellAuxiliaryFunctionsCtlObj mapCellPressureCtlObj;

            private IMapOprtCellExectorCallBack callBack;

            public TextFontMapOprtCellExector(MapCellAuxiliaryFunctionsCtlObj mapCellPressureCtlObj)
            {
                this.mapCellPressureCtlObj = mapCellPressureCtlObj;
            }
            #region  IMapOprtCellExector 成员

            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack)
            {
                this.callBack = callBack;
            }

            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                object viewModel = callBack.GetMapCellVMObjInstance();
                if (viewModel != null && viewModel is AuxiliaryFunctionsViewModel pressureViewModel)
                {
                    GriffinsBaseValue mapCellPropValue = callBack.GetMapCellPropValue(nameof(AuxiliaryFunctionsPropertyModelEdit.TextFont));
                    if (mapCellPropValue is { })
                    {
                        ObjectValue_Json objectValue_Json = mapCellPropValue.ToObjectValue_Json();
                        GriffinsBaseValue griffinsBaseValue = GriffinsBaseValue.Create(objectValue_Json);
                        IGriffinsBaseValue iMPPropObjectValue = new FontInfo();
                        iMPPropObjectValue.PopulateFromBaseValue(griffinsBaseValue);
                        //pressureViewModel.TextFont = (FontInfo)iMPPropObjectValue;
                    }
                }
            }

            #endregion
        }

        #endregion
    }

    /// <summary>
    /// 辅助功能图元属性编辑模型对象
    /// </summary>
    [Serializable]
    [MapPropertyOrder]
    [CategoryPriority("图元信息", 1)]
    public class AuxiliaryFunctionsPropertyModelEdit : FunctionalCellPropertyModelEdit
    {
        public AuxiliaryFunctionsPropertyModelEdit()
        {
            TextFont = new FontInfo(FontManager.Current.DefaultFontFamily, 14.0, FontWeight.Normal, FontStyle.Normal);
            //TextFont.PropertyChanged += textFont_PropertyChanged;
            Styles.Add(ResourceA.Style1);
            Styles.Add(ResourceA.Style2);
        }

        private void textFont_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // 通知PropertyGrid：嵌套对象的属性已变化
            RaisePropertyChanged(nameof(TextFont));
        }

        private FontInfo _textFont;// = FontInfo.DefaultFont;
        [DisplayName("文字字体")]
        [Category("图元信息")]
        [PropertySortOrder(15)]
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
        /// <summary>
        /// 文本颜色
        /// </summary>
        [DisplayName("文本颜色")]
        [Category("图元信息")]
        [PropertySortOrder(16)]
        [JsonConverter(typeof(ColorConvert))]
        public Color TextColor
        {
            get
            {
                return _textColor;
            }
            set
            {
                SetProperty(ref _textColor, value);
            }
        }

        private Color _backColor = Color.FromArgb(33, 0, 0, 0);
        /// <summary>
        /// 背景颜色
        /// </summary>
        [DisplayName("背景颜色")]
        [Category("图元信息")]
        [PropertySortOrder(17)]
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

        private decimal _pressureValue = 0;
        /// <summary>
        /// 图片定位方式
        /// </summary>
        [DisplayName("气压值")]
        [Category("图元信息")]
        [PropertySortOrder(18)]
        public decimal PressureValue
        {
            get
            {
                return _pressureValue;
            }
            set
            {
                SetProperty(ref _pressureValue, value);
            }
        }

        /// <summary>
        /// 设置属性值
        /// </summary>
        /// <param name="propertyID">属性ID</param>
        /// <param name="propertyVal">属性值</param>
        /// <returns>True:已经找到该属性并设置，false:没有该属性</returns>
        public override bool SetPropertyValue(string propertyID, GriffinsBaseValue propertyVal)
        {
            if (string.Compare(propertyID, nameof(PressureValue)) == 0)
            {
                if (propertyVal is { })
                {
                    decimal imageSizeMode = propertyVal.ToPrimitiveValue<decimal>();
                    PressureValue = imageSizeMode;
                }
                else
                {
                    PressureValue = 0;
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
                    ObjectValue_Json objectValue_Json = propertyVal.ToObjectValue_Json();
                    GriffinsBaseValue griffinsBaseValue = GriffinsBaseValue.Create(objectValue_Json);
                    IGriffinsBaseValue iMPPropObjectValue = new FontInfo();
                    iMPPropObjectValue.PopulateFromBaseValue(griffinsBaseValue);
                    TextFont = (FontInfo)iMPPropObjectValue;
                }
                else
                {
                    TextFont = FontInfo.DefaultFont;
                }
                return true;
            }
            //注意最后一定要对在没有找到图元定义的属性调用其父类的SetPropertyValue
            return base.SetPropertyValue(propertyID, propertyVal);
        }

        /// <summary>
        /// 从来源实例复制字段到本实例
        /// </summary>
        /// <param name="source">来源实例</param>
        public void CopyFrom(AuxiliaryFunctionsPropertyModelEdit source)
        {
            base.CopyFrom(source);
            if (source.TextFont != null)
            {
                this.TextFont = new FontInfo(source.TextFont.FontFamily, source.TextFont.FontSize, source.TextFont.FontWeight, source.TextFont.FontStyle);
            }
            this.TextColor = source.TextColor;
            this.BackColor = source.BackColor;
            this.PressureValue = source.PressureValue;
        }
    }

    /// <summary>
    /// 辅助功能图元属性绑定编辑模型对象，可由图元继承
    /// </summary>
    [Serializable]
    [MapPropertyOrder]
    [CategoryPriority("点位信息", 1)]
    [CategoryPriority("绑定信息", 2)]
    public class AuxiliaryFunctionsPropertyBindEditModel : FunctionalCellPropertyBindEditModel
    {
        
        /// <summary>
        /// 从来源实例复制字段到本实例
        /// </summary>
        /// <param name="source">来源实例</param>
        public void CopyFrom(AuxiliaryFunctionsPropertyBindEditModel source)
        {
            base.CopyFrom(source);
        }
    }
}
