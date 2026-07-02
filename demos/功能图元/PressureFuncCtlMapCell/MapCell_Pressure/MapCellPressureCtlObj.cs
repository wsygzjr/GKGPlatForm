using Avalonia.Media;
using GF_Gereric;
using Griffins.Map.PressureFuncCtlMapCell.MapCell_Pressure;
using Griffins.Map.PressureFuncCtlMapCell.View;
using Griffins.Map.PressureFuncCtlMapCell.ViewModel;
using Griffins.Map.UI;
using Griffins.UI2;
using Newtonsoft.JsonG;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Griffins.Map.PressureFuncCtlMapCell
{
    class MapCellPressureCtlObj : FunctionalCellBase
    {
        /// <summary>
        /// 上料管理点种类下的属性：剩余数量
        /// </summary>
        private const string _mPPropertyID_RemainingCountStr = "RemainingCount";


        public const string Prop_PressureValue = "PressureValue";

        private PressureView view;

        private PressureHView view1;

        private PressureViewModel pressureViewModel;

        static MapCellPressureCtlObj()
        {

        }
        public MapCellPressureCtlObj(MapObjID mapCellID, string mapCellName)
            : this(mapCellID, mapCellName, false)
        {
        }

        public MapCellPressureCtlObj(MapObjID mapCellID, string mapCellName, bool designTime)
        {
            PropertyEditModelBase = CreatePropertyModelEditBase();
            PropertyBindEditModelBase = CreatePropertyBindEditModelBase();
            EventBindEditModel = CreateEventBindEditModel();
            base.SetID(mapCellID);
            base.SetName(mapCellName);
            view = new PressureView();
            view1 = new PressureHView();
            RegisterProperty(new MapObjPropertyInfo(MapObjPropEventConst.Prop_BackColor, MapObjPropEventConst.GetName(MapObjPropEventConst.Prop_BackColor), GriffinsBaseDataType.Integer, Guid.Empty, typeof(uint), true, true, new GriffinsBaseValue(Color.FromArgb(33, 0, 0, 0).ToColorString())));
            RegisterProperty(new MapObjPropertyInfo(nameof(PressurePropertyModelEdit.TextColor), MapObjPropEventConst.GetName(MapObjPropEventConst.Prop_TextColor), GriffinsBaseDataType.Integer, Guid.Empty, typeof(uint), true, true, new GriffinsBaseValue(Colors.Black.ToColorString())));
            RegisterProperty(new MapObjPropertyInfo(nameof(PressurePropertyModelEdit.TextFont), ResourceA.TextFont, GriffinsBaseDataType.Object_Json, FontInfo.Object_ID, typeof(FontInfo), false, true, new GriffinsBaseValue(FontInfo.DefaultFont)));
            //RegisterProperty(new MapObjPropertyInfo(MapObjPropEventConst.Prop_Cursor, MapObjPropEventConst.GetName(MapObjPropEventConst.Prop_Cursor), MapCellPropDataType.Integer, Guid.Empty, typeof(int), true, true, new MapCellPropValue(CtlCellCursor.Default)));
            RegisterProperty(new MapObjPropertyInfo(Prop_PressureValue, ResourceA.PressureValue, GriffinsBaseDataType.Decimal, Guid.Empty, typeof(decimal), true, true, new GriffinsBaseValue(0)));
            RegisterEvent(new MapObjEventInfo(MapObjPropEventConst.Event_MouseClick, MapObjPropEventConst.GetName(MapObjPropEventConst.Event_MouseClick), GriffinsBaseDataType.Object_Bytes, GraphMouseEventParam.Object_ID));
            RegisterOprtCellInfo(new MapOprtCellInfo(PressureMapOprtCellConst.TextColor_MapOprtCellID, ResourceA.TextColor_MapOprtCellName));
            RegisterOprtCellInfo(new MapOprtCellInfo(PressureMapOprtCellConst.BackColor_MapOprtCellID, ResourceA.BackColor_MapOprtCellName));
            RegisterOprtCellInfo(new MapOprtCellInfo(PressureMapOprtCellConst.PressureValue_MapOprtCellID, ResourceA.PressureValue_MapOprtCellName, typeof(MapOprtCellParamCfgView)));
            RegisterOprtCellInfo(new MapOprtCellInfo(PressureMapOprtCellConst.TextFont_MapOprtCellID, ResourceA.TextFont_MapOprtCellName));
            RegisterOprtInfo(new MapOprtInfo(nameof(PressurePropertyModelEdit.TextColor), ResourceA.TextColor_MapOprtName, OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
            {
                new MapOprtCellInstInfo()
                {
                  InstanceID=Guid.NewGuid(),
                  OprtCellID=PressureMapOprtCellConst.TextColor_MapOprtCellID,
                  CfgInfo=null
                }
            }));
            RegisterOprtInfo(new MapOprtInfo(nameof(PressurePropertyModelEdit.BackColor), ResourceA.BackColor_MapOprtName, OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
            {
                new MapOprtCellInstInfo()
                {
                  InstanceID=Guid.NewGuid(),
                  OprtCellID=PressureMapOprtCellConst.BackColor_MapOprtCellID,
                  CfgInfo=null
                }
            }));
            RegisterOprtInfo(new MapOprtInfo(nameof(PressurePropertyModelEdit.PressureValue), ResourceA.PressureValue_MapOprtName, OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
            {
                new MapOprtCellInstInfo()
                {
                  InstanceID=Guid.NewGuid(),
                  OprtCellID=PressureMapOprtCellConst.PressureValue_MapOprtCellID,
                  CfgInfo=null
                }
            }));
            RegisterOprtInfo(new MapOprtInfo(nameof(PressurePropertyModelEdit.TextFont), ResourceA.TextFont_MapOprtName, OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
            {
                new MapOprtCellInstInfo()
                {
                  InstanceID=Guid.NewGuid(),
                  OprtCellID=PressureMapOprtCellConst.TextFont_MapOprtCellID,
                  CfgInfo=null
                }
            }));
            (this as IMapCellTypeBase).Name = ResourceA.Pressure;
            pressureViewModel = new PressureViewModel(PressurePropertyModelEdit, clickExec);
            view.DataContext = pressureViewModel;
            view1.DataContext = pressureViewModel;
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
        public PressurePropertyModelEdit PressurePropertyModelEdit
        {
            get { return PropertyEditModelBase as PressurePropertyModelEdit; }
        }

        public override bool SetPropertyValue(string propertyID, GriffinsBaseValue propertyVal, bool isRuning)
        {
            PressurePropertyModelEdit.IsRuning = isRuning;
            return PressurePropertyModelEdit.SetPropertyValue(propertyID, propertyVal);
        }

        /// <summary>
        /// 执行图元内部操作原子
        /// </summary>
        /// <param name="mapOprtCellInstInfo">图元内部操作原子信息</param>
        /// <returns>True:已经找到该操作原子并设置，false:没有该操作原子</returns>
        protected override bool ExecOprtCell(MapOprtCellInstInfo mapOprtCellInstInfo)
        {
            if (mapOprtCellInstInfo.OprtCellID == PressureMapOprtCellConst.TextColor_MapOprtCellID)
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
            if (mapOprtCellInstInfo.OprtCellID == PressureMapOprtCellConst.BackColor_MapOprtCellID)
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
            if (mapOprtCellInstInfo.OprtCellID == PressureMapOprtCellConst.PressureValue_MapOprtCellID)
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
            if (mapOprtCellInstInfo.OprtCellID == PressureMapOprtCellConst.TextFont_MapOprtCellID)
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
            if (string.Compare(propertyID, nameof(PressurePropertyModelEdit.TextColor)) == 0)
            {
                CallBack?.ExecOprt(nameof(PressurePropertyModelEdit.TextColor));
            }
            if (string.Compare(propertyID, nameof(PressurePropertyModelEdit.BackColor)) == 0)
            {
                CallBack?.ExecOprt(nameof(PressurePropertyModelEdit.BackColor));
            }
            if (string.Compare(propertyID, nameof(PressurePropertyModelEdit.PressureValue)) == 0)
            {
                CallBack?.ExecOprt(nameof(PressurePropertyModelEdit.PressureValue));
            }
            if (string.Compare(propertyID, nameof(PressurePropertyModelEdit.TextFont)) == 0)
            {
                CallBack?.ExecOprt(nameof(PressurePropertyModelEdit.TextFont));
            }
            if (!PressurePropertyModelEdit.IsRuning)
            {
                //需要在此次将图元属性值转换为界面数据对象属性值
                CallBack?.UpdateUIDataObjPropValues(new GFBaseTypePropValueList());
            }
        }

        public override GriffinsBaseValue ConvertPropertyValue(string propertyID, object propertyValue)
        {
            return null!;
        }

        /// <summary>
        /// 从字节流中读画图信息（必须先调用基类的OnReadDrawInfoFromBytes，必须保证写入数据和读出数据的顺序一致）
        /// </summary>
        /// <param name="br">字节流读取对象</param>
        protected override void OnReadDrawInfoFromBytes(GriffinsXmlReader br)
        {
            base.OnReadDrawInfoFromBytes(br);
            var propertyEditModelBase = JsonObjConvert.FromJSon<PressurePropertyModelEdit>(br.ReadString("PropertyEditModelBase"));
            (PropertyEditModelBase as PressurePropertyModelEdit).CopyFrom(propertyEditModelBase);
            var propertyBindEditModelBase = JsonObjConvert.FromJSon<PressurePropertyBindEditModel>(br.ReadString("PropertyBindEditModelBase"));
            (PropertyBindEditModelBase as PressurePropertyBindEditModel).CopyFrom(propertyBindEditModelBase);
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
            MapCellPressureCtlObj mapCellPressureCtlObj = (source as MapCellPressureCtlObj);
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
            if (PropertyEditModelBase.SelectStyle == ResourceA.Style1)
            {
                return view;
            }
            else
            {
                return view1;
            }
        }

        protected override object OnGetViewModel()
        {
            return pressureViewModel;
        }

        /// <summary>
        /// 创建图元属性编辑模型对象
        /// </summary>
        /// <returns>图元属性编辑模型对象</returns>
        public override PropertyEditModelBase CreatePropertyModelEditBase()
        {
            return new PressurePropertyModelEdit();
        }

        /// <summary>
        /// 创建图元属性绑定编辑模型对象
        /// </summary>
        /// <returns>图元属性绑定编辑模型对象</returns>
        public override PropertyBindEditModelBase CreatePropertyBindEditModelBase()
        {
            //气压值绑定到上料管理点的剩余数量属性
            var pressurePropertyBindEditModel = new PressurePropertyBindEditModel();
            pressurePropertyBindEditModel.PressureValue = _mPPropertyID_RemainingCountStr;
            return pressurePropertyBindEditModel;
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
            double size = base.CallBack?.Calc?.CalcZoomVal((decimal)this.PressurePropertyModelEdit.TextFont.FontSize) ?? this.PressurePropertyModelEdit.TextFont.FontSize;
            if (size < 2)
                size = 2;
            FontInfo font = new FontInfo(this.PressurePropertyModelEdit.TextFont.FontFamily, size, this.PressurePropertyModelEdit.TextFont.FontWeight, this.PressurePropertyModelEdit.TextFont.FontStyle);
            this.pressureViewModel.TextFont = font;
        }

        public override string ToString()
        {
            return "气压";
        }

        #region 操作原子执行对象

        /// <summary>
        /// 文本颜色操作原子执行对象
        /// </summary>
        private class TextColorMapOprtCellExector : IMapOprtCellExector
        {
            private MapCellPressureCtlObj mapCellPressureCtlObj;

            private IMapOprtCellExectorCallBack callBack;

            public TextColorMapOprtCellExector(MapCellPressureCtlObj mapCellPressureCtlObj)
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
                if (viewModel != null && viewModel is PressureViewModel pressureViewModel)
                {
                    GriffinsBaseValue mapCellPropValue = callBack.GetMapCellPropValue(nameof(PressurePropertyModelEdit.TextColor));
                    if (mapCellPropValue is { })
                    {
                        var color = mapCellPropValue.ToPrimitiveValue<string>();
                        pressureViewModel.TextColor = Color.Parse(color);
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
            private MapCellPressureCtlObj mapCellPressureCtlObj;

            private IMapOprtCellExectorCallBack callBack;

            public BackColorMapOprtCellExector(MapCellPressureCtlObj mapCellPressureCtlObj)
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
                if (viewModel != null && viewModel is PressureViewModel pressureViewModel)
                {
                    GriffinsBaseValue mapCellPropValue = callBack.GetMapCellPropValue(nameof(PressurePropertyModelEdit.BackColor));
                    if (mapCellPropValue is { })
                    {
                        var color = mapCellPropValue.ToPrimitiveValue<string>();
                        pressureViewModel.BackColor = Color.Parse(color);
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
            private MapCellPressureCtlObj mapCellPressureCtlObj;

            private IMapOprtCellExectorCallBack callBack;

            private PressureValueMapOprtCellParamViewModel paramViewModel;

            public PressureValueMapOprtCellExector(MapCellPressureCtlObj mapCellPressureCtlObj)
            {
                this.mapCellPressureCtlObj = mapCellPressureCtlObj;
                paramViewModel = new PressureValueMapOprtCellParamViewModel();
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
                    paramViewModel.FromBytes(cfg);
                }
                object viewModel = callBack.GetMapCellVMObjInstance();
                if (viewModel != null && viewModel is PressureViewModel pressureViewModel)
                {
                    GriffinsBaseValue mapCellPropValue = callBack.GetMapCellPropValue(nameof(PressurePropertyModelEdit.PressureValue));
                    if (mapCellPropValue is { })
                    {
                        if (paramViewModel.ShowUnit)
                        {
                            pressureViewModel.PressureValue = mapCellPropValue.ToPrimitiveValue<decimal>() + paramViewModel.Unit;
                        }
                        else
                        {
                            pressureViewModel.PressureValue = mapCellPropValue.ToPrimitiveValue<decimal>().ToString();
                        }
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
            private MapCellPressureCtlObj mapCellPressureCtlObj;

            private IMapOprtCellExectorCallBack callBack;

            public TextFontMapOprtCellExector(MapCellPressureCtlObj mapCellPressureCtlObj)
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
                if (viewModel != null && viewModel is PressureViewModel pressureViewModel)
                {
                    GriffinsBaseValue mapCellPropValue = callBack.GetMapCellPropValue(nameof(PressurePropertyModelEdit.TextFont));
                    if (mapCellPropValue is { })
                    {
                        ObjectValue_Json objectValue_Json = mapCellPropValue.ToObjectValue_Json();
                        GriffinsBaseValue griffinsBaseValue = GriffinsBaseValue.Create(objectValue_Json);
                        IGriffinsBaseValue iMPPropObjectValue = new FontInfo();
                        iMPPropObjectValue.PopulateFromBaseValue(griffinsBaseValue);
                        pressureViewModel.TextFont = (FontInfo)iMPPropObjectValue;
                    }
                }
            }

            #endregion
        }

        #endregion
    }

    /// <summary>
    /// 按钮属性编辑模型对象
    /// </summary>
    [Serializable]
    [MapPropertyOrder]
    [CategoryPriority("图元信息", 1)]
    public class PressurePropertyModelEdit : FunctionalCellPropertyModelEdit
    {
        public PressurePropertyModelEdit()
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
        public void CopyFrom(PressurePropertyModelEdit source)
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
    /// 按钮图元属性绑定编辑模型对象，可由图元继承
    /// </summary>
    [Serializable]
    [MapPropertyOrder]
    [CategoryPriority("点位信息", 1)]
    [CategoryPriority("绑定信息", 2)]
    public class PressurePropertyBindEditModel : FunctionalCellPropertyBindEditModel
    {
        private string _textFont = "TextFont";
        /// <summary>
        /// 文本颜色
        /// </summary>
        [DisplayName("文本字体")]
        [Category("绑定信息")]
        [PropertySortOrder(7)]
        [Browsable(false)]
        public string TextFont
        {
            get
            {
                return _textFont;
            }
            set
            {
                SetProperty(ref _textFont, value);
            }
        }
        private string _textColor = "TextColor";
        /// <summary>
        /// 文本颜色
        /// </summary>
        [DisplayName("文本颜色")]
        [Category("绑定信息")]
        [PropertySortOrder(8)]
        [Browsable(false)]
        public string TextColor
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

        private string _backColor = "BackColor";
        /// <summary>
        /// 文本颜色
        /// </summary>
        [DisplayName("背景颜色")]
        [Category("绑定信息")]
        [PropertySortOrder(9)]
        [Browsable(false)]
        public string BackColor
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

        private string _pressureValue = "PressureValue";
        /// <summary>
        /// 气压值
        /// </summary>
        [DisplayName("气压值")]
        [Category("绑定信息")]
        [PropertySortOrder(10)]
        [Browsable(false)]
        public string PressureValue
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
        /// 从来源实例复制字段到本实例
        /// </summary>
        /// <param name="source">来源实例</param>
        public void CopyFrom(PressurePropertyBindEditModel source)
        {
            base.CopyFrom(source);
            this.TextFont = source.TextFont;
            this.TextColor = source.TextColor;
            this.BackColor = source.BackColor;
            this.PressureValue = source.PressureValue;
        }
    }
}
