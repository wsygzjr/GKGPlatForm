using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Avalonia.Threading;
using GF_Gereric;
using GKG.Map.MapCell.Generic.Control.MapCell_DateInput.MapOprtCellParamCfgView;
using GKG.Map.MapCell.Generic.Control.MapCell_DateInput.Objects;
using GKG.Map.MapCell.Generic.Control.MapCell_DateInput.ViewModels;
using GKG.Map.MapCell.Generic.Control.MapCell_DateInput.Views;
using Griffins;
using Newtonsoft.JsonG;
using PropertyModels.ComponentModel;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace GKG.Map.MapCell.Generic.Control.MapCell_DateInput
{
    internal class MapCellDateInputCtlObj : ControlCellBase
    {
        private EventBindEditModel _eventBindEditModel;
        private DateInputView view;
        private DateInputViewModel viewModel;

        public MapCellDateInputCtlObj(MapObjID mapCellID, string mapCellName)
            : this(mapCellID, mapCellName, false)
        {
        }

        public MapCellDateInputCtlObj(MapObjID mapCellID, string mapCellName, bool designTime)
        {
            PropertyEditModelBase = CreatePropertyModelEditBase();
            PropertyBindEditModelBase = CreatePropertyBindEditModelBase();
            EventBindEditModel = CreateEventBindEditModel();
            SetID(mapCellID);
            SetName(mapCellName);

            view = new DateInputView();

            RegisterProperty(new MapObjPropertyInfo(nameof(DateInputPropertyModelEdit.CommonInfo), "公共设置", MapCellPropDataType.Object_Json, DateInputCommonInfo.Object_ID, typeof(DateInputCommonInfo), false, true, new MapCellPropValue(DateInputCommonInfo.Default)));
            RegisterProperty(new MapObjPropertyInfo(nameof(DateInputCommonInfo.SelectedDate), "选中日期", MapCellPropDataType.String, Guid.Empty, typeof(string), false, true, new MapCellPropValue(string.Empty)));

            RegisterOprtCellInfo(new MapOprtCellInfo(DateInputMapOprtCellConst.CommonInfo_MapOprtCellID, "日期输入框公共设置操作原子", typeof(CommonInfoMapOprtCellParamCfgView)));
            RegisterOprtInfo(new MapOprtInfo(nameof(DateInputPropertyModelEdit.CommonInfo), "设置公共", OprtExecKind.Normal, "", new MapOprtCellInstInfoList { new MapOprtCellInstInfo { InstanceID = Guid.NewGuid(), OprtCellID = DateInputMapOprtCellConst.CommonInfo_MapOprtCellID, CfgInfo = null } }));

            (this as IMapCellTypeBase).Name = "日期输入框";

            viewModel = new DateInputViewModel(DateInputPropertyModelEdit);
            view.DataContext = viewModel;

            DateInputPropertyModelEdit.CommonInfo.PropertyChanged += OnCommonInfoPropertyChanged;
        }

        private void OnCommonInfoPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            ExecuteOprtByPropertyId(e?.PropertyName ?? nameof(DateInputPropertyModelEdit.CommonInfo));
        }

        [Browsable(false)]
        public DateInputPropertyModelEdit DateInputPropertyModelEdit => PropertyEditModelBase as DateInputPropertyModelEdit;

        public override PropertyEditModelBase CreatePropertyModelEditBase() => new DateInputPropertyModelEdit();

        public override PropertyBindEditModelBase CreatePropertyBindEditModelBase() => new DateInputPropertyBindEditModel();

        public override EventBindEditModel CreateEventBindEditModel()
        {
            _eventBindEditModel ??= new EventBindEditModel();
            return _eventBindEditModel;
        }

        protected override bool SetPropertyValue(GFBaseTypePropValueList gFBaseTypePropValues)
        {
            if (gFBaseTypePropValues == null)
                return false;

            foreach (GFBaseTypePropValue item in gFBaseTypePropValues)
            {
                string propertyId = item?.PropertyID.ToString() ?? string.Empty;
                if (string.IsNullOrWhiteSpace(propertyId))
                    continue;

                SetPropertyValue(propertyId, item.Value, true);
            }

            return true;
        }

        public override bool SetPropertyValue(string propertyID, MapCellPropValue propertyVal, bool isRuning)
        {
            DateInputPropertyModelEdit.IsRuning = isRuning;
            bool ok = DateInputPropertyModelEdit.SetPropertyValue(propertyID, propertyVal);
            if (ok)
            {
                viewModel.ReloadFromModel();
                ExecuteOprtByPropertyId(propertyID);
            }

            return ok;
        }

        private void ExecuteOprtByPropertyId(string propertyID)
        {
            if (!TryGetPrimaryOprtCellId(propertyID, out string normalizedOprtId, out MapOprtCellID primaryOprtCellId))
                return;

            TryExecuteOprtInfoById(normalizedOprtId, primaryOprtCellId);
        }

        private static bool TryGetPrimaryOprtCellId(string oprtId, out string normalizedOprtId, out MapOprtCellID oprtCellId)
        {
            normalizedOprtId = oprtId ?? string.Empty;
            oprtCellId = default;

            if (string.IsNullOrWhiteSpace(oprtId))
                return false;

            if (string.Equals(oprtId, nameof(DateInputCommonInfo.SelectedDate), StringComparison.Ordinal)
                || string.Equals(oprtId, nameof(DateInputPropertyModelEdit.CommonInfo), StringComparison.Ordinal))
            {
                normalizedOprtId = nameof(DateInputPropertyModelEdit.CommonInfo);
                oprtCellId = DateInputMapOprtCellConst.CommonInfo_MapOprtCellID;
                return true;
            }

            return false;
        }

        private bool TryExecuteOprtInfoById(string oprtId, MapOprtCellID primaryOprtCellId)
        {
            MapOprtCellInstInfo inst = new() { InstanceID = Guid.NewGuid(), OprtCellID = primaryOprtCellId, CfgInfo = null };
            return ExecOprtCell(inst);
        }

        protected override object OnGetView() => view;

        protected override object OnGetViewModel() => viewModel;

        public override void OnDispose()
        {
            DateInputPropertyModelEdit.CommonInfo.PropertyChanged -= OnCommonInfoPropertyChanged;

            view.DataContext = null;
            viewModel?.Dispose();
            viewModel = null;

            base.OnDispose();
        }

        protected override void OnCopyFrom(ControlCellBase source)
        {
            base._CopyFrom(source as MapCellDateInputCtlObj);
            if (source.PropertyEditModelBase is DateInputPropertyModelEdit propertyEditModel)
                DateInputPropertyModelEdit.CopyFrom(propertyEditModel);
            if (PropertyBindEditModelBase is DateInputPropertyBindEditModel selfBind && source.PropertyBindEditModelBase is DateInputPropertyBindEditModel srcBind)
                selfBind.CopyFrom(srcBind);
            else
                PropertyBindEditModelBase.CopyFrom(source.PropertyBindEditModelBase);

            EventBindEditModel.CopyFrom(source.EventBindEditModel);
            viewModel.ReloadFromModel();
        }

        protected override void OnReadDrawInfoFromBytes(GriffinsXmlReader br)
        {
            base.OnReadDrawInfoFromBytes(br);

            string propertyEditJson = br.ReadString("PropertyEditModelBase");
            if (!string.IsNullOrEmpty(propertyEditJson))
            {
                DateInputPropertyModelEdit propertyEditModel = JsonObjConvert.FromJSon<DateInputPropertyModelEdit>(propertyEditJson);
                DateInputPropertyModelEdit.CopyFrom(propertyEditModel);
            }

            string propertyBindJson = br.ReadString("PropertyBindEditModelBase");
            if (!string.IsNullOrEmpty(propertyBindJson))
            {
                DateInputPropertyBindEditModel propertyBindModel = JsonObjConvert.FromJSon<DateInputPropertyBindEditModel>(propertyBindJson);
                if (propertyBindModel != null && PropertyBindEditModelBase is DateInputPropertyBindEditModel selfBind)
                    selfBind.CopyFrom(propertyBindModel);
            }

            viewModel.ReloadFromModel();
        }

        protected override void OnWriteDrawInfoToBytes(GriffinsXmlWriter bw)
        {
            base.OnWriteDrawInfoToBytes(bw);
            bw.Write("PropertyEditModelBase", JsonObjConvert.ToJSon(PropertyEditModelBase));
            bw.Write("PropertyBindEditModelBase", JsonObjConvert.ToJSon(PropertyBindEditModelBase));
        }

        protected override bool ExecOprtCell(MapOprtCellInstInfo mapOprtCellInstInfo)
        {
            if (mapOprtCellInstInfo.OprtCellID == DateInputMapOprtCellConst.CommonInfo_MapOprtCellID)
                return ExecuteOprtCell<CommonInfoMapOprtCellExector>(mapOprtCellInstInfo);

            return base.ExecOprtCell(mapOprtCellInstInfo);
        }

        private bool ExecuteOprtCell<T>(MapOprtCellInstInfo mapOprtCellInstInfo) where T : IMapOprtCellExector, new()
        {
            if (!MapOprtCellExectorDict.TryGetValue(mapOprtCellInstInfo.InstanceID, out IMapOprtCellExector mapOprtCellExector))
            {
                mapOprtCellExector = new T();
                mapOprtCellExector.Init(IMapOprtCellExectorCallBack);
                MapOprtCellExectorDict.TryAdd(mapOprtCellInstInfo.InstanceID, mapOprtCellExector);
            }

            mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
            return true;
        }

        private static void PostToUI(Action action)
        {
            if (Dispatcher.UIThread.CheckAccess())
                action();
            else
                Dispatcher.UIThread.Post(action);
        }

        private class CommonInfoMapOprtCellExector : IMapOprtCellExector
        {
            private IMapOprtCellExectorCallBack callBack;

            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack) => this.callBack = callBack;

            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                if (callBack.GetMapCellVMObjInstance() is not DateInputViewModel vm)
                    return;

                if (cfg != null && cfg.Length > 0)
                {
                    CommonInfoMapOprtCellParamViewModel param = JsonSerializer.Deserialize<CommonInfoMapOprtCellParamViewModel>(Encoding.UTF8.GetString(cfg));
                    if (param != null)
                    {
                        PostToUI(() => vm.CommonInfo.SelectedDate = param.SelectedDate ?? string.Empty);
                        return;
                    }
                }

                MapCellPropValue val = callBack.GetMapCellPropValue(nameof(DateInputPropertyModelEdit.CommonInfo));
                if (val == null)
                    return;

                DateInputCommonInfo info = DeserializeObject<DateInputCommonInfo>(val);
                PostToUI(() => vm.CommonInfo.SelectedDate = info.SelectedDate);
            }
        }

        protected override void OnInit()
        {
            base.OnInit();
            viewModel.ReloadFromModel();
        }

        public override void OnZoomChanged()
        {
        }

        public override string ToString() => "日期输入框";

        private static T DeserializeObject<T>(MapCellPropValue val) where T : IMPPropObjectValue, new()
        {
            ObjectValue_Json objectValueJson = val.ToObjectValue_Json();
            GriffinsBaseValue griffinsBaseValue = GriffinsBaseValue.Create(objectValueJson);
            IMPPropObjectValue obj = new T();
            obj.PopulateFromBaseValue(griffinsBaseValue);
            return (T)obj;
        }

        public override MapCellPropValue ConvertPropertyValue(string propertyID, object propertyValue)
        {
            return null;
        }
    }
}
