using Avalonia.Media;
using GF_Gereric;
using GKG.Map.ChartFuncCtlMapCell.View;
using GKG.Map.ChartFuncCtlMapCell.ViewModel;
using Griffins;
using Griffins.Map;
using Griffins.Map.Cmd;
using Griffins.Map.UI;
using Griffins.PF;
using Griffins.UI2;
using HarfBuzzSharp;
using PropertyModels.ComponentModel;
using ReactiveUI;
using System;
using System.ComponentModel;
using System.Linq;
using System.Text.Json;

namespace GKG.Map.ChartFuncCtlMapCell
{

    class MapCellChartCtlObj : FunctionalCellBase
    {
        private ChartView view;

        private ChartViewModel chartViewModel;

        static MapCellChartCtlObj()
        {

        }
        public MapCellChartCtlObj(MapObjID mapCellID, string mapCellName)
            : this(mapCellID, mapCellName, false)
        {

        }

        public MapCellChartCtlObj(MapObjID mapCellID, string mapCellName, bool designTime)
        {
            PropertyEditModelBase = CreatePropertyModelEditBase();
            PropertyBindEditModelBase = CreatePropertyBindEditModelBase();
            EventBindEditModel = CreateEventBindEditModel();

            base.SetID(mapCellID);
            base.SetName(mapCellName);

            // 注册编辑属性
            RegisterProperty(new MapObjPropertyInfo(nameof(ChartPropertyModelEdit.ChartDatas), ResourceA.ChartDatas, GriffinsBaseDataType.Object_Json, ChartDatas.Object_ID, typeof(ChartDatas), false, true, new GriffinsBaseValue(ChartDatas.DefaultEmpty)));
            
            // 注册操作原子
            RegisterOprtCellInfo(new MapOprtCellInfo(ChartMapOprtCellConst.ChartDatas_MapOprtCellID, ResourceA.ChartDatas_MapOprtCellName));

            // 注册操作
            RegisterOprtInfo(new MapOprtInfo(nameof(ChartPropertyModelEdit.ChartDatas), ResourceA.ChartDatas_MapOprtName, OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
            {
                new MapOprtCellInstInfo()
                {
                  InstanceID=Guid.NewGuid(),
                  OprtCellID=ChartMapOprtCellConst.ChartDatas_MapOprtCellID,
                  CfgInfo=null
                }
            }));

            // 注册事件
            RegisterEvent(new MapObjEventInfo(MapObjPropEventConst.Event_MouseClick, MapObjPropEventConst.GetName(MapObjPropEventConst.Event_MouseClick), GriffinsBaseDataType.Object_Bytes, GraphMouseEventParam.Object_ID));

            (this as IMapCellTypeBase).Name = ResourceA.Chart;

            view = new ChartView();
            chartViewModel = new ChartViewModel(ChartPropertyModelEdit, () => new MapCmdExector(base.CallBack.INorthSvrCommandExec), PropertyBindEditModelBase.MpNo);
            view.DataContext = chartViewModel;
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
        public ChartPropertyModelEdit ChartPropertyModelEdit
        {
            get { return PropertyEditModelBase as ChartPropertyModelEdit; }
        }

        public override bool SetPropertyValue(string propertyID, GriffinsBaseValue propertyVal, bool isRuning)
        {
            ChartPropertyModelEdit.IsRuning = isRuning;
            return ChartPropertyModelEdit.SetPropertyValue(propertyID, propertyVal);
        }

        /// <summary>
        /// 执行图元内部操作原子
        /// </summary>
        /// <param name="mapOprtCellInstInfo">图元内部操作原子信息</param>
        /// <returns>True:已经找到该操作原子并设置，false:没有该操作原子</returns>
        protected override bool ExecOprtCell(MapOprtCellInstInfo mapOprtCellInstInfo)
        {
            if (mapOprtCellInstInfo.OprtCellID == ChartMapOprtCellConst.ChartDatas_MapOprtCellID)
            {
                if (MapOprtCellExectorDict.TryGetValue(mapOprtCellInstInfo.InstanceID, out IMapOprtCellExector mapOprtCellExector))
                {
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                else
                {
                    mapOprtCellExector = new ChartDatasMapOprtCellExector(this);
                    mapOprtCellExector.Init(IMapOprtCellExectorCallBack);
                    MapOprtCellExectorDict.TryAdd(mapOprtCellInstInfo.InstanceID, mapOprtCellExector);
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                return true;
            }

            return base.ExecOprtCell(mapOprtCellInstInfo);
        }

        /// <summary>
        /// 从字节流中读画图信息（必须先调用基类的OnReadDrawInfoFromBytes，必须保证写入数据和读出数据的顺序一致）
        /// </summary>
        /// <param name="br">字节流读取对象</param>
        protected override void OnReadDrawInfoFromBytes(GriffinsXmlReader br)
        {
            base.OnReadDrawInfoFromBytes(br);
            var propertyEditModelBase = JsonObjConvert.FromJSon<ChartPropertyModelEdit>(br.ReadString("PropertyEditModelBase"));
            (PropertyEditModelBase as ChartPropertyModelEdit).CopyFrom(propertyEditModelBase);
            var propertyBindEditModelBase = JsonObjConvert.FromJSon<ChartPropertyBindEditModel>(br.ReadString("PropertyBindEditModelBase"));
            (PropertyBindEditModelBase as ChartPropertyBindEditModel).CopyFrom(propertyBindEditModelBase);
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
            MapCellChartCtlObj mapCellDeviceStatusIndicatorCtlObj = (source as MapCellChartCtlObj);
            base._CopyFrom(mapCellDeviceStatusIndicatorCtlObj);
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
            return chartViewModel;
        }

        /// <summary>
        /// 创建图元属性编辑模型对象
        /// </summary>
        /// <returns>图元属性编辑模型对象</returns>
        public override PropertyEditModelBase CreatePropertyModelEditBase()
        {
            return new ChartPropertyModelEdit();
        }

        /// <summary>
        /// 创建图元属性绑定编辑模型对象
        /// </summary>
        /// <returns>图元属性绑定编辑模型对象</returns>
        public override PropertyBindEditModelBase CreatePropertyBindEditModelBase()
        {
            return new ChartPropertyBindEditModel();
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
        }

        public override string ToString()
        {
            return "图表";
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
                string propId = gFBaseTypePropValue.PropertyID.ToString();
                GriffinsBaseValue propValue = gFBaseTypePropValue.Value;

                if (propValue == null) return false;

                switch (propId)
                {
                    case "ChartDatas":
                        ((IGriffinsBaseValue)ChartPropertyModelEdit.ChartDatas).PopulateFromBaseValue(propValue);
                        break;

                    default:
                        break;
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

            switch (propertyID)
            {
                case nameof(ChartPropertyModelEdit.ChartDatas):
                    CallBack?.ExecOprt(nameof(ChartPropertyModelEdit.ChartDatas));
                    break;
            }

            if (!ChartPropertyModelEdit.IsRuning)
            {
                //需要在此次将图元属性值转换为界面数据对象属性值
                CallBack?.UpdateUIDataObjPropValues(new GFBaseTypePropValueList());
            }
        }

        public override GriffinsBaseValue ConvertPropertyValue(string propertyID, object propertyValue)
        {
            return null!;
        }

        #region 操作原子执行对象

        /// <summary>
        /// 图表数据操作原子执行对象
        /// </summary>
        private class ChartDatasMapOprtCellExector : IMapOprtCellExector
        {
            private MapCellChartCtlObj mapCellChartCtlObj;

            private IMapOprtCellExectorCallBack callBack;

            public ChartDatasMapOprtCellExector(MapCellChartCtlObj mapCellChartCtlObj)
            {
                this.mapCellChartCtlObj = mapCellChartCtlObj;
            }

            #region  IMapOprtCellExector 成员

            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack)
            {
                this.callBack = callBack;
            }

            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                object viewModel = callBack.GetMapCellVMObjInstance();
                if (viewModel != null && viewModel is ChartViewModel chartViewModel)
                {

                    GriffinsBaseValue valueMapCellPropValue = callBack.GetMapCellPropValue(nameof(ChartPropertyModelEdit.ChartDatas));

                    if (valueMapCellPropValue is not null)
                    {

                        ChartDatas chartDatas = new ChartDatas();
                        ((IGriffinsBaseValue)chartDatas).PopulateFromBaseValue(valueMapCellPropValue);


                        chartViewModel.AddNewData(chartDatas.Time, chartDatas.Value);
                        chartViewModel.Cpk = chartDatas.Cpk;
                        chartViewModel.UpperLimit = chartDatas.UpperLimit;
                        chartViewModel.LowerLimit = chartDatas.LowerLimit;

                    }
                }
            }

            #endregion
        }
        #endregion
    }


    /// <summary>
    /// 图表数据对象
    /// </summary>
    public class ChartDatas : MiniReactiveObject, IJsonValueConvert, IGriffinsBaseValue
    {
        /// <summary>对象ID</summary>
        public static readonly Guid Object_ID = new Guid("{BDA30552-1DC0-4DE1-A5DE-BEB6B1D96811}");

        public static readonly ChartDatas DefaultEmpty = new ChartDatas();

        #region 属性字段


        private string _type;
        public string Type { get => _type; set => SetProperty(ref _type, value); }

        private DateTime _time;
        public DateTime Time { get => _time; set => SetProperty(ref _time, value); }

        private double _value;
        public double Value { get => _value; set => SetProperty(ref _value, value); }

        private double _cpk;
        public double Cpk { get => _cpk; set => SetProperty(ref _cpk, value); }

        private double _upperLimit;
        public double UpperLimit { get => _upperLimit; set => SetProperty(ref _upperLimit, value); }

        private double _lowerLimit;
        public double LowerLimit { get => _lowerLimit; set => SetProperty(ref _lowerLimit, value); }


        #endregion

        #region 构造函数

        public ChartDatas()
        {
            Type = string.Empty;
            Time = DateTime.Today;
            Value = 0;
            Cpk = 0;
            UpperLimit = 0;
            LowerLimit = 0;
        }

        #endregion

        #region IGriffinsBaseValue 成员

        bool IGriffinsBaseValue.IsObject_Byte => false;

        Guid IGriffinsBaseValue.GetObject_ID()
        {
            return Object_ID;
        }

        GriffinsBaseValue IGriffinsBaseValue.ToBaseValue()
        {
            ObjectValue_Json objectValue_Json = new ObjectValue_Json(Object_ID);
            objectValue_Json.JsonVal = toJson();
            return GriffinsBaseValue.Create(objectValue_Json);
        }

        void IGriffinsBaseValue.PopulateFromBaseValue(GriffinsBaseValue baseValue)
        {
            if ((object)baseValue != null && baseValue.Val != null)
            {
                if (!(baseValue.Val is ObjectValue_Json))
                {
                    throw new Exception("对象值不是ChartDatas转换的");
                }

                if ((baseValue.Val as ObjectValue_Json).Object_ID != Object_ID)
                {
                    throw new Exception("对象值不是ChartDatas转换的");
                }

                fromJson((baseValue.Val as ObjectValue_Json).JsonVal);
            }
        }

        #endregion

        #region IJsonValueConvert 成员

        void IJsonValueConvert.FromJsonDataObject(string jsonDataObject)
        {
            fromJson(jsonDataObject);
        }

        string IJsonValueConvert.ToJsonDataObject()
        {
            return toJson();
        }

        #endregion

        #region 序列化与反序列化

        private class SaveModel
        {
            public string Type { get; set; }
            public DateTime Time { get; set; }
            public double Value { get; set; }
            public double Cpk { get; set; }
            public double UpperLimit { get; set; }
            public double LowerLimit { get; set; }
        }

        private string toJson()
        {
            SaveModel saveModel = new SaveModel()
            {
                Type = this.Type,
                Time = this.Time,
                Value = this.Value,
                Cpk = this.Cpk,
                UpperLimit = this.UpperLimit,
                LowerLimit = this.LowerLimit
            };

            return JsonSerializer.Serialize(saveModel);
        }

        private void fromJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) return;

            try
            {
                var data = JsonSerializer.Deserialize<SaveModel>(json);
                if (data != null)
                {
                    this.Type = data.Type;
                    this.Time = data.Time;
                    this.Value = data.Value;
                    this.Cpk = data.Cpk;
                    this.UpperLimit = data.UpperLimit;
                    this.LowerLimit = data.LowerLimit;
                }
            }
            catch (JsonException)
            {
                // 遇到被破坏的 JSON 数据不报错，维持当前界面的默认值
            }
        }

        #endregion
    }


    /// <summary>
    /// 图元属性编辑模型对象
    /// </summary>
    [Serializable]
    [MapPropertyOrder]
    [CategoryPriority("图元信息", 1)]
    public class ChartPropertyModelEdit : FunctionalCellPropertyModelEdit
    {
        public ChartPropertyModelEdit()
        {
            ChartDatas = ChartDatas.DefaultEmpty;
            ChartDatas.PropertyChanged += chartDatas_PropertyChanged;
        }

        private void chartDatas_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(ChartDatas));
        }

        private ChartDatas _chartDatas = ChartDatas.DefaultEmpty;
        [DisplayName("图表数据")]
        [Category("图元信息")]
        [PropertySortOrder(11)]
        [Browsable(false)]
        public ChartDatas ChartDatas
        {
            get => _chartDatas;
            set
            {
                if (_chartDatas != null)
                {
                    _chartDatas.PropertyChanged -= chartDatas_PropertyChanged;
                }

                if (SetProperty(ref _chartDatas, value))
                {
                    if (_chartDatas != null)
                    {
                        _chartDatas.PropertyChanged += chartDatas_PropertyChanged;
                    }
                }
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
            switch (propertyID)
            {
                case nameof(ChartDatas):
                    if (propertyVal == null)
                    {
                        ChartDatas = ChartDatas.DefaultEmpty;
                    }
                    else
                    {
                        ChartDatas = new ChartDatas();
                        ((IGriffinsBaseValue)ChartDatas).PopulateFromBaseValue(propertyVal);
                    }
                    return true;

                default:
                    break;
            }

            return base.SetPropertyValue(propertyID, propertyVal);
        }

        /// <summary>
        /// 从来源实例复制字段到本实例
        /// </summary>
        /// <param name="source">来源实例</param>
        public void CopyFrom(ChartPropertyModelEdit source)
        {
            base.CopyFrom(source);

            if (source.ChartDatas != null)
            {
                this.ChartDatas = new ChartDatas()
                {
                    Type = source.ChartDatas.Type,
                    Time = source.ChartDatas.Time,
                    Value = source.ChartDatas.Value,
                    Cpk = source.ChartDatas.Cpk,
                    UpperLimit = source.ChartDatas.UpperLimit,
                    LowerLimit = source.ChartDatas.LowerLimit
                };
            }
        }
    }


    /// <summary>
    /// 图元属性绑定编辑模型对象，可由图元继承
    /// </summary>
    [Serializable]
    [MapPropertyOrder]
    [CategoryPriority("点位信息", 1)]
    [CategoryPriority("绑定信息", 2)]
    public class ChartPropertyBindEditModel : FunctionalCellPropertyBindEditModel
    {
        /// <summary>
        /// 从来源实例复制字段到本实例
        /// </summary>
        /// <param name="source">来源实例</param>
        public void CopyFrom(ChartPropertyBindEditModel source)
        {
            base.CopyFrom(source);
        }
    }

}