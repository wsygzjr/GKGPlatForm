using Avalonia.Media;
using GF_Gereric;
using GKG.Map.HeatingControlFuncCtlMapCell.View;
using GKG.Map.HeatingControlFuncCtlMapCell.ViewModel;
using Griffins;
using Griffins.Map;
using Griffins.Map.UI;
using Griffins.UI2;
using Newtonsoft.JsonG;
using PropertyModels.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text.Json;

namespace GKG.Map.HeatingControlFuncCtlMapCell
{
    class MapCellHeatingControlCtlObj : FunctionalCellBase
    {
        private HeatingControlView view;
        private HeatingControlViewModel heatingControlViewModel;

        static MapCellHeatingControlCtlObj()
        {

        }
        /// <summary>
        /// 构造函数 - 设计时
        /// </summary>
        /// <param name="mapCellID">图元ID</param>
        /// <param name="mapCellName">图元名称</param>
        public MapCellHeatingControlCtlObj(MapObjID mapCellID, string mapCellName)
            : this(mapCellID, mapCellName, false)
        {
        }

        /// <summary>
        /// 构造函数 - 运行时/设计时
        /// </summary>
        /// <param name="mapCellID">图元ID</param>
        /// <param name="mapCellName">图元名称</param>
        /// <param name="designTime">是否为设计时</param>
        public MapCellHeatingControlCtlObj(MapObjID mapCellID, string mapCellName, bool designTime)
        {
            PropertyEditModelBase = CreatePropertyModelEditBase();
            PropertyBindEditModelBase = CreatePropertyBindEditModelBase();
            EventBindEditModel = CreateEventBindEditModel();

            base.SetID(mapCellID);
            base.SetName(mapCellName);

            view = new HeatingControlView();

            RegisterProperty(new MapObjPropertyInfo(nameof(HeatingControlPropertyModelEdit.BackColor), MapObjPropEventConst.GetName(MapObjPropEventConst.Prop_BackColor), GriffinsBaseDataType.Integer, Guid.Empty, typeof(uint), true, true, new GriffinsBaseValue(Color.FromArgb(1, 0, 0, 0).ToColorString())));
            RegisterProperty(new MapObjPropertyInfo(nameof(HeatingControlPropertyModelEdit.TextColor), MapObjPropEventConst.GetName(MapObjPropEventConst.Prop_TextColor), GriffinsBaseDataType.Integer, Guid.Empty, typeof(uint), true, true, new GriffinsBaseValue(Colors.Black.ToColorString())));
            RegisterProperty(new MapObjPropertyInfo(nameof(HeatingControlPropertyModelEdit.TextFont), ResourceA.TextFont, GriffinsBaseDataType.Object_Json, FontInfo.Object_ID, typeof(FontInfo), true, true, new GriffinsBaseValue(FontInfo.DefaultFont)));

            // 注册双阀模式属性
            RegisterProperty(new MapObjPropertyInfo(nameof(HeatingControlPropertyModelEdit.IsDualValve), "双阀模式", GriffinsBaseDataType.Bool, Guid.Empty, typeof(bool), true, true, new GriffinsBaseValue(false)));

            // 注册双轨模式属性
            RegisterProperty(new MapObjPropertyInfo(nameof(HeatingControlPropertyModelEdit.IsDualTrack), "双轨模式", GriffinsBaseDataType.Bool, Guid.Empty, typeof(bool), true, true, new GriffinsBaseValue(false)));

            // 注册每个加热模块属性
            RegisterProperty(new MapObjPropertyInfo(nameof(HeatingControlPropertyModelEdit.RightValveDispensingHead), "右阀点胶头", GriffinsBaseDataType.Object_Json, HeatingModuleInfo.Object_ID, typeof(HeatingModuleInfo), true, true, new GriffinsBaseValue(HeatingControlPropertyModelEdit.RightValveDispensingHead)));
            RegisterProperty(new MapObjPropertyInfo(nameof(HeatingControlPropertyModelEdit.RightValveCartridgeHeating), "右阀胶筒加热", GriffinsBaseDataType.Object_Json, HeatingModuleInfo.Object_ID, typeof(HeatingModuleInfo), true, true, new GriffinsBaseValue(HeatingControlPropertyModelEdit.RightValveCartridgeHeating)));
            RegisterProperty(new MapObjPropertyInfo(nameof(HeatingControlPropertyModelEdit.LeftValveDispensingHead), "左阀点胶头", GriffinsBaseDataType.Object_Json, HeatingModuleInfo.Object_ID, typeof(HeatingModuleInfo), true, true, new GriffinsBaseValue(HeatingControlPropertyModelEdit.LeftValveDispensingHead)));
            RegisterProperty(new MapObjPropertyInfo(nameof(HeatingControlPropertyModelEdit.LeftValveCartridgeHeating), "左阀胶筒加热", GriffinsBaseDataType.Object_Json, HeatingModuleInfo.Object_ID, typeof(HeatingModuleInfo), true, true, new GriffinsBaseValue(HeatingControlPropertyModelEdit.LeftValveCartridgeHeating)));
            RegisterProperty(new MapObjPropertyInfo(nameof(HeatingControlPropertyModelEdit.ARailPreheatLeft), "A轨预热-左", GriffinsBaseDataType.Object_Json, HeatingModuleInfo.Object_ID, typeof(HeatingModuleInfo), true, true, new GriffinsBaseValue(HeatingControlPropertyModelEdit.ARailPreheatLeft)));
            RegisterProperty(new MapObjPropertyInfo(nameof(HeatingControlPropertyModelEdit.ARailPreheatLeft2), "A轨预热-左(2)", GriffinsBaseDataType.Object_Json, HeatingModuleInfo.Object_ID, typeof(HeatingModuleInfo), true, true, new GriffinsBaseValue(HeatingControlPropertyModelEdit.ARailPreheatLeft2)));
            RegisterProperty(new MapObjPropertyInfo(nameof(HeatingControlPropertyModelEdit.ARailGlueBoardStationMiddle), "A轨点胶工位-中", GriffinsBaseDataType.Object_Json, HeatingModuleInfo.Object_ID, typeof(HeatingModuleInfo), true, true, new GriffinsBaseValue(HeatingControlPropertyModelEdit.ARailGlueBoardStationMiddle)));
            RegisterProperty(new MapObjPropertyInfo(nameof(HeatingControlPropertyModelEdit.ARailDispensingStationMiddle2), "A轨点胶工位-中(2)", GriffinsBaseDataType.Object_Json, HeatingModuleInfo.Object_ID, typeof(HeatingModuleInfo), true, true, new GriffinsBaseValue(HeatingControlPropertyModelEdit.ARailDispensingStationMiddle2)));
            RegisterProperty(new MapObjPropertyInfo(nameof(HeatingControlPropertyModelEdit.ARailPreheatRight), "A轨预热-右", GriffinsBaseDataType.Object_Json, HeatingModuleInfo.Object_ID, typeof(HeatingModuleInfo), true, true, new GriffinsBaseValue(HeatingControlPropertyModelEdit.ARailPreheatRight)));
            RegisterProperty(new MapObjPropertyInfo(nameof(HeatingControlPropertyModelEdit.ARailPreheatRight2), "A轨预热-右(2)", GriffinsBaseDataType.Object_Json, HeatingModuleInfo.Object_ID, typeof(HeatingModuleInfo), true, true, new GriffinsBaseValue(HeatingControlPropertyModelEdit.ARailPreheatRight2)));
            RegisterProperty(new MapObjPropertyInfo(nameof(HeatingControlPropertyModelEdit.BRailPreheatLeft), "B轨预热-左", GriffinsBaseDataType.Object_Json, HeatingModuleInfo.Object_ID, typeof(HeatingModuleInfo), true, true, new GriffinsBaseValue(HeatingControlPropertyModelEdit.BRailPreheatLeft)));
            RegisterProperty(new MapObjPropertyInfo(nameof(HeatingControlPropertyModelEdit.BRailPreheatLeft2), "B轨预热-左(2)", GriffinsBaseDataType.Object_Json, HeatingModuleInfo.Object_ID, typeof(HeatingModuleInfo), true, true, new GriffinsBaseValue(HeatingControlPropertyModelEdit.BRailPreheatLeft2)));
            RegisterProperty(new MapObjPropertyInfo(nameof(HeatingControlPropertyModelEdit.BRailGlueBoardStationMiddle), "B轨点胶工位-中", GriffinsBaseDataType.Object_Json, HeatingModuleInfo.Object_ID, typeof(HeatingModuleInfo), true, true, new GriffinsBaseValue(HeatingControlPropertyModelEdit.BRailGlueBoardStationMiddle)));
            RegisterProperty(new MapObjPropertyInfo(nameof(HeatingControlPropertyModelEdit.BRailDispensingStationMiddle2), "B轨点胶工位-中(2)", GriffinsBaseDataType.Object_Json, HeatingModuleInfo.Object_ID, typeof(HeatingModuleInfo), true, true, new GriffinsBaseValue(HeatingControlPropertyModelEdit.BRailDispensingStationMiddle2)));
            RegisterProperty(new MapObjPropertyInfo(nameof(HeatingControlPropertyModelEdit.BRailPreheatRight), "B轨预热-右", GriffinsBaseDataType.Object_Json, HeatingModuleInfo.Object_ID, typeof(HeatingModuleInfo), true, true, new GriffinsBaseValue(HeatingControlPropertyModelEdit.BRailPreheatRight)));
            RegisterProperty(new MapObjPropertyInfo(nameof(HeatingControlPropertyModelEdit.BRailPreheatRight2), "B轨预热-右(2)", GriffinsBaseDataType.Object_Json, HeatingModuleInfo.Object_ID, typeof(HeatingModuleInfo), true, true, new GriffinsBaseValue(HeatingControlPropertyModelEdit.BRailPreheatRight2)));

            RegisterEvent(new MapObjEventInfo(MapObjPropEventConst.Event_MouseClick, MapObjPropEventConst.GetName(MapObjPropEventConst.Event_MouseClick), GriffinsBaseDataType.Object_Bytes, GraphMouseEventParam.Object_ID));

            RegisterOprtCellInfo(new MapOprtCellInfo(HeatingControlMapOprtCellConst.TextColor_MapOprtCellID, ResourceA.TextColor_MapOprtCellName));
            RegisterOprtCellInfo(new MapOprtCellInfo(HeatingControlMapOprtCellConst.BackColor_MapOprtCellID, ResourceA.BackColor_MapOprtCellName));
            RegisterOprtCellInfo(new MapOprtCellInfo(HeatingControlMapOprtCellConst.TextFont_MapOprtCellID, ResourceA.TextFont_MapOprtCellName));

            RegisterOprtCellInfo(new MapOprtCellInfo(HeatingControlMapOprtCellConst.IsDualValve_MapOprtCellID, "双阀模式操作原子"));
            RegisterOprtCellInfo(new MapOprtCellInfo(HeatingControlMapOprtCellConst.IsDualTrack_MapOprtCellID, "双轨模式操作原子"));

            RegisterOprtCellInfo(new MapOprtCellInfo(HeatingControlMapOprtCellConst.RightValveDispensingHead_MapOprtCellID, "右阀点胶头操作原子"));
            RegisterOprtCellInfo(new MapOprtCellInfo(HeatingControlMapOprtCellConst.RightValveCartridgeHeating_MapOprtCellID, "右阀胶筒加热操作原子"));
            RegisterOprtCellInfo(new MapOprtCellInfo(HeatingControlMapOprtCellConst.LeftValveDispensingHead_MapOprtCellID, "左阀点胶头操作原子"));
            RegisterOprtCellInfo(new MapOprtCellInfo(HeatingControlMapOprtCellConst.LeftValveCartridgeHeating_MapOprtCellID, "左阀胶筒加热操作原子"));

            RegisterOprtCellInfo(new MapOprtCellInfo(HeatingControlMapOprtCellConst.ARailPreheatLeft_MapOprtCellID, "A轨预热-左操作原子"));
            RegisterOprtCellInfo(new MapOprtCellInfo(HeatingControlMapOprtCellConst.ARailPreheatLeft2_MapOprtCellID, "A轨预热-左(2)操作原子"));
            RegisterOprtCellInfo(new MapOprtCellInfo(HeatingControlMapOprtCellConst.ARailGlueBoardStationMiddle_MapOprtCellID, "A轨胶板工位-中操作原子"));
            RegisterOprtCellInfo(new MapOprtCellInfo(HeatingControlMapOprtCellConst.ARailDispensingStationMiddle2_MapOprtCellID, "A轨点胶工位-中(2)操作原子"));
            RegisterOprtCellInfo(new MapOprtCellInfo(HeatingControlMapOprtCellConst.ARailPreheatRight_MapOprtCellID, "A轨预热-右操作原子"));
            RegisterOprtCellInfo(new MapOprtCellInfo(HeatingControlMapOprtCellConst.ARailPreheatRight2_MapOprtCellID, "A轨预热-右(2)操作原子"));

            RegisterOprtCellInfo(new MapOprtCellInfo(HeatingControlMapOprtCellConst.BRailPreheatLeft_MapOprtCellID, "B轨预热-左操作原子"));
            RegisterOprtCellInfo(new MapOprtCellInfo(HeatingControlMapOprtCellConst.BRailPreheatLeft2_MapOprtCellID, "B轨预热-左(2)操作原子"));
            RegisterOprtCellInfo(new MapOprtCellInfo(HeatingControlMapOprtCellConst.BRailGlueBoardStationMiddle_MapOprtCellID, "B轨胶板工位-中操作原子"));
            RegisterOprtCellInfo(new MapOprtCellInfo(HeatingControlMapOprtCellConst.BRailDispensingStationMiddle2_MapOprtCellID, "B轨点胶工位-中(2)操作原子"));
            RegisterOprtCellInfo(new MapOprtCellInfo(HeatingControlMapOprtCellConst.BRailPreheatRight_MapOprtCellID, "B轨预热-右操作原子"));
            RegisterOprtCellInfo(new MapOprtCellInfo(HeatingControlMapOprtCellConst.BRailPreheatRight2_MapOprtCellID, "B轨预热-右(2)操作原子"));

            RegisterOprtInfo(new MapOprtInfo(nameof(HeatingControlPropertyModelEdit.TextColor), ResourceA.TextColor_MapOprtName, OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
            {
                new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = HeatingControlMapOprtCellConst.TextColor_MapOprtCellID, CfgInfo = null }
            }));

            RegisterOprtInfo(new MapOprtInfo(nameof(HeatingControlPropertyModelEdit.BackColor), ResourceA.BackColor_MapOprtName, OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
            {
                new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = HeatingControlMapOprtCellConst.BackColor_MapOprtCellID, CfgInfo = null }
            }));

            RegisterOprtInfo(new MapOprtInfo(nameof(HeatingControlPropertyModelEdit.TextFont), ResourceA.TextFont_MapOprtName, OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
            {
                new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = HeatingControlMapOprtCellConst.TextFont_MapOprtCellID, CfgInfo = null }
            }));

            // 为每个加热模块属性注册操作信息
            RegisterOprtInfo(new MapOprtInfo(nameof(HeatingControlPropertyModelEdit.RightValveDispensingHead), "右阀点胶头", OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
            {
                new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = HeatingControlMapOprtCellConst.RightValveDispensingHead_MapOprtCellID, CfgInfo = null }
            }));

            RegisterOprtInfo(new MapOprtInfo(nameof(HeatingControlPropertyModelEdit.RightValveCartridgeHeating), "右阀胶筒加热", OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
            {
                new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = HeatingControlMapOprtCellConst.RightValveCartridgeHeating_MapOprtCellID, CfgInfo = null }
            }));

            RegisterOprtInfo(new MapOprtInfo(nameof(HeatingControlPropertyModelEdit.LeftValveDispensingHead), "左阀点胶头", OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
            {
                new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = HeatingControlMapOprtCellConst.LeftValveDispensingHead_MapOprtCellID, CfgInfo = null }
            }));

            RegisterOprtInfo(new MapOprtInfo(nameof(HeatingControlPropertyModelEdit.LeftValveCartridgeHeating), "左阀胶筒加热", OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
            {
                new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = HeatingControlMapOprtCellConst.LeftValveCartridgeHeating_MapOprtCellID, CfgInfo = null }
            }));

            RegisterOprtInfo(new MapOprtInfo(nameof(HeatingControlPropertyModelEdit.ARailPreheatLeft), "A轨预热-左", OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
            {
                new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = HeatingControlMapOprtCellConst.ARailPreheatLeft_MapOprtCellID, CfgInfo = null }
            }));

            RegisterOprtInfo(new MapOprtInfo(nameof(HeatingControlPropertyModelEdit.ARailPreheatLeft2), "A轨预热-左(2)", OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
            {
                new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = HeatingControlMapOprtCellConst.ARailPreheatLeft2_MapOprtCellID, CfgInfo = null }
            }));

            RegisterOprtInfo(new MapOprtInfo(nameof(HeatingControlPropertyModelEdit.ARailGlueBoardStationMiddle), "A轨点胶工位-中", OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
            {
                new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = HeatingControlMapOprtCellConst.ARailGlueBoardStationMiddle_MapOprtCellID, CfgInfo = null }
            }));

            RegisterOprtInfo(new MapOprtInfo(nameof(HeatingControlPropertyModelEdit.ARailDispensingStationMiddle2), "A轨点胶工位-中(2)", OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
            {
                new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = HeatingControlMapOprtCellConst.ARailDispensingStationMiddle2_MapOprtCellID, CfgInfo = null }
            }));

            RegisterOprtInfo(new MapOprtInfo(nameof(HeatingControlPropertyModelEdit.ARailPreheatRight), "A轨预热-右", OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
            {
                new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = HeatingControlMapOprtCellConst.ARailPreheatRight_MapOprtCellID, CfgInfo = null }
            }));

            RegisterOprtInfo(new MapOprtInfo(nameof(HeatingControlPropertyModelEdit.ARailPreheatRight2), "A轨预热-右(2)", OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
            {
                new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = HeatingControlMapOprtCellConst.ARailPreheatRight2_MapOprtCellID, CfgInfo = null }
            }));

            RegisterOprtInfo(new MapOprtInfo(nameof(HeatingControlPropertyModelEdit.BRailPreheatLeft), "B轨预热-左", OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
            {
                new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = HeatingControlMapOprtCellConst.BRailPreheatLeft_MapOprtCellID, CfgInfo = null }
            }));

            RegisterOprtInfo(new MapOprtInfo(nameof(HeatingControlPropertyModelEdit.BRailPreheatLeft2), "B轨预热-左(2)", OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
            {
                new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = HeatingControlMapOprtCellConst.BRailPreheatLeft2_MapOprtCellID, CfgInfo = null }
            }));

            RegisterOprtInfo(new MapOprtInfo(nameof(HeatingControlPropertyModelEdit.BRailGlueBoardStationMiddle), "B轨点胶工位-中", OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
            {
                new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = HeatingControlMapOprtCellConst.BRailGlueBoardStationMiddle_MapOprtCellID, CfgInfo = null }
            }));

            RegisterOprtInfo(new MapOprtInfo(nameof(HeatingControlPropertyModelEdit.BRailDispensingStationMiddle2), "B轨点胶工位-中(2)", OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
            {
                new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = HeatingControlMapOprtCellConst.BRailDispensingStationMiddle2_MapOprtCellID, CfgInfo = null }
            }));

            RegisterOprtInfo(new MapOprtInfo(nameof(HeatingControlPropertyModelEdit.BRailPreheatRight), "B轨预热-右", OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
            {
                new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = HeatingControlMapOprtCellConst.BRailPreheatRight_MapOprtCellID, CfgInfo = null }
            }));

            RegisterOprtInfo(new MapOprtInfo(nameof(HeatingControlPropertyModelEdit.BRailPreheatRight2), "B轨预热-右(2)", OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
            {
                new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = HeatingControlMapOprtCellConst.BRailPreheatRight2_MapOprtCellID, CfgInfo = null }
            }));

            // 注册双阀模式属性操作信息
            RegisterOprtInfo(new MapOprtInfo(nameof(HeatingControlPropertyModelEdit.IsDualValve), "双阀模式", OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
            {
                new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = HeatingControlMapOprtCellConst.IsDualValve_MapOprtCellID, CfgInfo = null }
            }));

            // 注册双轨模式属性操作信息
            RegisterOprtInfo(new MapOprtInfo(nameof(HeatingControlPropertyModelEdit.IsDualTrack), "双轨模式", OprtExecKind.Normal, "", new MapOprtCellInstInfoList()
            {
                new MapOprtCellInstInfo() { InstanceID = Guid.NewGuid(), OprtCellID = HeatingControlMapOprtCellConst.IsDualTrack_MapOprtCellID, CfgInfo = null }
            }));

            (this as IMapCellTypeBase).Name = ResourceA.HeatingControl;
            heatingControlViewModel = new HeatingControlViewModel(HeatingControlPropertyModelEdit, clickExec);
            view.DataContext = heatingControlViewModel;
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
        public HeatingControlPropertyModelEdit HeatingControlPropertyModelEdit
        {
            get { return PropertyEditModelBase as HeatingControlPropertyModelEdit; }
        }

        [Browsable(false)]
        public HeatingControlPropertyBindEditModel HeatingControlPropertyBindEditModel
        {
            get { return PropertyBindEditModelBase as HeatingControlPropertyBindEditModel; }
        }

        public override bool SetPropertyValue(string propertyID, GriffinsBaseValue propertyVal, bool isRuning)
        {
            HeatingControlPropertyModelEdit.IsRuning = isRuning;
            return HeatingControlPropertyModelEdit.SetPropertyValue(propertyID, propertyVal);
        }

        /// <summary>
        /// 执行图元内部操作原子
        /// </summary>
        /// <param name="mapOprtCellInstInfo">图元内部操作原子信息</param>
        /// <returns>True:已经找到该操作原子并设置，false:没有该操作原子</returns>
        protected override bool ExecOprtCell(MapOprtCellInstInfo mapOprtCellInstInfo)
        {
            if (mapOprtCellInstInfo.OprtCellID == HeatingControlMapOprtCellConst.TextColor_MapOprtCellID)
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
            if (mapOprtCellInstInfo.OprtCellID == HeatingControlMapOprtCellConst.BackColor_MapOprtCellID)
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
            if (mapOprtCellInstInfo.OprtCellID == HeatingControlMapOprtCellConst.TextFont_MapOprtCellID)
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
            if (mapOprtCellInstInfo.OprtCellID == HeatingControlMapOprtCellConst.IsDualValve_MapOprtCellID)
            {
                if (MapOprtCellExectorDict.TryGetValue(mapOprtCellInstInfo.InstanceID, out IMapOprtCellExector mapOprtCellExector))
                {
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                else
                {
                    mapOprtCellExector = new IsDualValveMapOprtCellExector(this);
                    mapOprtCellExector.Init(IMapOprtCellExectorCallBack);
                    MapOprtCellExectorDict.TryAdd(mapOprtCellInstInfo.InstanceID, mapOprtCellExector);
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                return true;
            }
            if (mapOprtCellInstInfo.OprtCellID == HeatingControlMapOprtCellConst.IsDualTrack_MapOprtCellID)
            {
                if (MapOprtCellExectorDict.TryGetValue(mapOprtCellInstInfo.InstanceID, out IMapOprtCellExector mapOprtCellExector))
                {
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                else
                {
                    mapOprtCellExector = new IsDualTrackMapOprtCellExector(this);
                    mapOprtCellExector.Init(IMapOprtCellExectorCallBack);
                    MapOprtCellExectorDict.TryAdd(mapOprtCellInstInfo.InstanceID, mapOprtCellExector);
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                return true;
            }
            if (mapOprtCellInstInfo.OprtCellID == HeatingControlMapOprtCellConst.RightValveDispensingHead_MapOprtCellID)
            {
                if (MapOprtCellExectorDict.TryGetValue(mapOprtCellInstInfo.InstanceID, out IMapOprtCellExector mapOprtCellExector))
                {
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                else
                {
                    mapOprtCellExector = new HeatingModuleMapOprtCellExector(this, nameof(HeatingControlPropertyModelEdit.RightValveDispensingHead));
                    mapOprtCellExector.Init(IMapOprtCellExectorCallBack);
                    MapOprtCellExectorDict.TryAdd(mapOprtCellInstInfo.InstanceID, mapOprtCellExector);
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                return true;
            }
            if (mapOprtCellInstInfo.OprtCellID == HeatingControlMapOprtCellConst.RightValveCartridgeHeating_MapOprtCellID)
            {
                if (MapOprtCellExectorDict.TryGetValue(mapOprtCellInstInfo.InstanceID, out IMapOprtCellExector mapOprtCellExector))
                {
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                else
                {
                    mapOprtCellExector = new HeatingModuleMapOprtCellExector(this, nameof(HeatingControlPropertyModelEdit.RightValveCartridgeHeating));
                    mapOprtCellExector.Init(IMapOprtCellExectorCallBack);
                    MapOprtCellExectorDict.TryAdd(mapOprtCellInstInfo.InstanceID, mapOprtCellExector);
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                return true;
            }
            if (mapOprtCellInstInfo.OprtCellID == HeatingControlMapOprtCellConst.LeftValveDispensingHead_MapOprtCellID)
            {
                if (MapOprtCellExectorDict.TryGetValue(mapOprtCellInstInfo.InstanceID, out IMapOprtCellExector mapOprtCellExector))
                {
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                else
                {
                    mapOprtCellExector = new HeatingModuleMapOprtCellExector(this, nameof(HeatingControlPropertyModelEdit.LeftValveDispensingHead));
                    mapOprtCellExector.Init(IMapOprtCellExectorCallBack);
                    MapOprtCellExectorDict.TryAdd(mapOprtCellInstInfo.InstanceID, mapOprtCellExector);
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                return true;
            }
            if (mapOprtCellInstInfo.OprtCellID == HeatingControlMapOprtCellConst.LeftValveCartridgeHeating_MapOprtCellID)
            {
                if (MapOprtCellExectorDict.TryGetValue(mapOprtCellInstInfo.InstanceID, out IMapOprtCellExector mapOprtCellExector))
                {
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                else
                {
                    mapOprtCellExector = new HeatingModuleMapOprtCellExector(this, nameof(HeatingControlPropertyModelEdit.LeftValveCartridgeHeating));
                    mapOprtCellExector.Init(IMapOprtCellExectorCallBack);
                    MapOprtCellExectorDict.TryAdd(mapOprtCellInstInfo.InstanceID, mapOprtCellExector);
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                return true;
            }
            if (mapOprtCellInstInfo.OprtCellID == HeatingControlMapOprtCellConst.ARailPreheatLeft_MapOprtCellID)
            {
                if (MapOprtCellExectorDict.TryGetValue(mapOprtCellInstInfo.InstanceID, out IMapOprtCellExector mapOprtCellExector))
                {
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                else
                {
                    mapOprtCellExector = new HeatingModuleMapOprtCellExector(this, nameof(HeatingControlPropertyModelEdit.ARailPreheatLeft));
                    mapOprtCellExector.Init(IMapOprtCellExectorCallBack);
                    MapOprtCellExectorDict.TryAdd(mapOprtCellInstInfo.InstanceID, mapOprtCellExector);
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                return true;
            }
            if (mapOprtCellInstInfo.OprtCellID == HeatingControlMapOprtCellConst.ARailPreheatLeft2_MapOprtCellID)
            {
                if (MapOprtCellExectorDict.TryGetValue(mapOprtCellInstInfo.InstanceID, out IMapOprtCellExector mapOprtCellExector))
                {
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                else
                {
                    mapOprtCellExector = new HeatingModuleMapOprtCellExector(this, nameof(HeatingControlPropertyModelEdit.ARailPreheatLeft2));
                    mapOprtCellExector.Init(IMapOprtCellExectorCallBack);
                    MapOprtCellExectorDict.TryAdd(mapOprtCellInstInfo.InstanceID, mapOprtCellExector);
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                return true;
            }
            if (mapOprtCellInstInfo.OprtCellID == HeatingControlMapOprtCellConst.ARailGlueBoardStationMiddle_MapOprtCellID)
            {
                if (MapOprtCellExectorDict.TryGetValue(mapOprtCellInstInfo.InstanceID, out IMapOprtCellExector mapOprtCellExector))
                {
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                else
                {
                    mapOprtCellExector = new HeatingModuleMapOprtCellExector(this, nameof(HeatingControlPropertyModelEdit.ARailGlueBoardStationMiddle));
                    mapOprtCellExector.Init(IMapOprtCellExectorCallBack);
                    MapOprtCellExectorDict.TryAdd(mapOprtCellInstInfo.InstanceID, mapOprtCellExector);
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                return true;
            }
            if (mapOprtCellInstInfo.OprtCellID == HeatingControlMapOprtCellConst.ARailDispensingStationMiddle2_MapOprtCellID)
            {
                if (MapOprtCellExectorDict.TryGetValue(mapOprtCellInstInfo.InstanceID, out IMapOprtCellExector mapOprtCellExector))
                {
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                else
                {
                    mapOprtCellExector = new HeatingModuleMapOprtCellExector(this, nameof(HeatingControlPropertyModelEdit.ARailDispensingStationMiddle2));
                    mapOprtCellExector.Init(IMapOprtCellExectorCallBack);
                    MapOprtCellExectorDict.TryAdd(mapOprtCellInstInfo.InstanceID, mapOprtCellExector);
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                return true;
            }
            if (mapOprtCellInstInfo.OprtCellID == HeatingControlMapOprtCellConst.ARailPreheatRight_MapOprtCellID)
            {
                if (MapOprtCellExectorDict.TryGetValue(mapOprtCellInstInfo.InstanceID, out IMapOprtCellExector mapOprtCellExector))
                {
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                else
                {
                    mapOprtCellExector = new HeatingModuleMapOprtCellExector(this, nameof(HeatingControlPropertyModelEdit.ARailPreheatRight));
                    mapOprtCellExector.Init(IMapOprtCellExectorCallBack);
                    MapOprtCellExectorDict.TryAdd(mapOprtCellInstInfo.InstanceID, mapOprtCellExector);
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                return true;
            }
            if (mapOprtCellInstInfo.OprtCellID == HeatingControlMapOprtCellConst.ARailPreheatRight2_MapOprtCellID)
            {
                if (MapOprtCellExectorDict.TryGetValue(mapOprtCellInstInfo.InstanceID, out IMapOprtCellExector mapOprtCellExector))
                {
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                else
                {
                    mapOprtCellExector = new HeatingModuleMapOprtCellExector(this, nameof(HeatingControlPropertyModelEdit.ARailPreheatRight2));
                    mapOprtCellExector.Init(IMapOprtCellExectorCallBack);
                    MapOprtCellExectorDict.TryAdd(mapOprtCellInstInfo.InstanceID, mapOprtCellExector);
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                return true;
            }
            if (mapOprtCellInstInfo.OprtCellID == HeatingControlMapOprtCellConst.BRailPreheatLeft_MapOprtCellID)
            {
                if (MapOprtCellExectorDict.TryGetValue(mapOprtCellInstInfo.InstanceID, out IMapOprtCellExector mapOprtCellExector))
                {
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                else
                {
                    mapOprtCellExector = new HeatingModuleMapOprtCellExector(this, nameof(HeatingControlPropertyModelEdit.BRailPreheatLeft));
                    mapOprtCellExector.Init(IMapOprtCellExectorCallBack);
                    MapOprtCellExectorDict.TryAdd(mapOprtCellInstInfo.InstanceID, mapOprtCellExector);
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                return true;
            }
            if (mapOprtCellInstInfo.OprtCellID == HeatingControlMapOprtCellConst.BRailPreheatLeft2_MapOprtCellID)
            {
                if (MapOprtCellExectorDict.TryGetValue(mapOprtCellInstInfo.InstanceID, out IMapOprtCellExector mapOprtCellExector))
                {
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                else
                {
                    mapOprtCellExector = new HeatingModuleMapOprtCellExector(this, nameof(HeatingControlPropertyModelEdit.BRailPreheatLeft2));
                    mapOprtCellExector.Init(IMapOprtCellExectorCallBack);
                    MapOprtCellExectorDict.TryAdd(mapOprtCellInstInfo.InstanceID, mapOprtCellExector);
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                return true;
            }
            if (mapOprtCellInstInfo.OprtCellID == HeatingControlMapOprtCellConst.BRailGlueBoardStationMiddle_MapOprtCellID)
            {
                if (MapOprtCellExectorDict.TryGetValue(mapOprtCellInstInfo.InstanceID, out IMapOprtCellExector mapOprtCellExector))
                {
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                else
                {
                    mapOprtCellExector = new HeatingModuleMapOprtCellExector(this, nameof(HeatingControlPropertyModelEdit.BRailGlueBoardStationMiddle));
                    mapOprtCellExector.Init(IMapOprtCellExectorCallBack);
                    MapOprtCellExectorDict.TryAdd(mapOprtCellInstInfo.InstanceID, mapOprtCellExector);
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                return true;
            }
            if (mapOprtCellInstInfo.OprtCellID == HeatingControlMapOprtCellConst.BRailDispensingStationMiddle2_MapOprtCellID)
            {
                if (MapOprtCellExectorDict.TryGetValue(mapOprtCellInstInfo.InstanceID, out IMapOprtCellExector mapOprtCellExector))
                {
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                else
                {
                    mapOprtCellExector = new HeatingModuleMapOprtCellExector(this, nameof(HeatingControlPropertyModelEdit.BRailDispensingStationMiddle2));
                    mapOprtCellExector.Init(IMapOprtCellExectorCallBack);
                    MapOprtCellExectorDict.TryAdd(mapOprtCellInstInfo.InstanceID, mapOprtCellExector);
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                return true;
            }
            if (mapOprtCellInstInfo.OprtCellID == HeatingControlMapOprtCellConst.BRailPreheatRight_MapOprtCellID)
            {
                if (MapOprtCellExectorDict.TryGetValue(mapOprtCellInstInfo.InstanceID, out IMapOprtCellExector mapOprtCellExector))
                {
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                else
                {
                    mapOprtCellExector = new HeatingModuleMapOprtCellExector(this, nameof(HeatingControlPropertyModelEdit.BRailPreheatRight));
                    mapOprtCellExector.Init(IMapOprtCellExectorCallBack);
                    MapOprtCellExectorDict.TryAdd(mapOprtCellInstInfo.InstanceID, mapOprtCellExector);
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                return true;
            }
            if (mapOprtCellInstInfo.OprtCellID == HeatingControlMapOprtCellConst.BRailPreheatRight2_MapOprtCellID)
            {
                if (MapOprtCellExectorDict.TryGetValue(mapOprtCellInstInfo.InstanceID, out IMapOprtCellExector mapOprtCellExector))
                {
                    mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                }
                else
                {
                    mapOprtCellExector = new HeatingModuleMapOprtCellExector(this, nameof(HeatingControlPropertyModelEdit.BRailPreheatRight2));
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
                    if (gFBaseTypePropValue == null)
                        continue;

                    string propId = gFBaseTypePropValue.PropertyID.ToString();
                    if (string.IsNullOrWhiteSpace(propId))
                        continue;

                    int dotIndex = propId.LastIndexOf('.');
                    if (dotIndex >= 0 && dotIndex < propId.Length - 1)
                        propId = propId.Substring(dotIndex + 1);

                    if (TryApplyHeatingModuleSubProperty(propId, gFBaseTypePropValue.Value))
                    continue;

                if (string.Compare(propId, nameof(HeatingControlPropertyModelEdit.TextColor)) == 0)
                    {
                        SetPropertyValue(nameof(HeatingControlPropertyModelEdit.TextColor), gFBaseTypePropValue.Value, true);
                        continue;
                    }
                    if (string.Compare(propId, nameof(HeatingControlPropertyModelEdit.BackColor)) == 0)
                    {
                        SetPropertyValue(nameof(HeatingControlPropertyModelEdit.BackColor), gFBaseTypePropValue.Value, true);
                        continue;
                    }
                    if (string.Compare(propId, nameof(HeatingControlPropertyModelEdit.TextFont)) == 0)
                    {
                        SetPropertyValue(nameof(HeatingControlPropertyModelEdit.TextFont), gFBaseTypePropValue.Value, true);
                        continue;
                    }
                    if (string.Compare(propId, nameof(HeatingControlPropertyModelEdit.IsDualValve)) == 0)
                    {
                        if (gFBaseTypePropValue.Value == null)
                            continue;
                        SetPropertyValue(nameof(HeatingControlPropertyModelEdit.IsDualValve), gFBaseTypePropValue.Value, true);
                        heatingControlViewModel.IsDualValve = ParseBool(gFBaseTypePropValue.Value);
                        heatingControlViewModel.UpdateHeatingModules();
                        continue;
                    }
                    if (string.Compare(propId, nameof(HeatingControlPropertyModelEdit.IsDualTrack)) == 0)
                    {
                        if (gFBaseTypePropValue.Value == null)
                            continue;
                        SetPropertyValue(nameof(HeatingControlPropertyModelEdit.IsDualTrack), gFBaseTypePropValue.Value, true);
                        heatingControlViewModel.IsDualTrack = ParseBool(gFBaseTypePropValue.Value);
                        heatingControlViewModel.UpdateHeatingModules();
                        continue;
                    }

                    if (string.Compare(propId, nameof(HeatingControlPropertyModelEdit.RightValveDispensingHead)) == 0)
                    {
                        SetPropertyValue(nameof(HeatingControlPropertyModelEdit.RightValveDispensingHead), gFBaseTypePropValue.Value, true);
                        heatingControlViewModel.SetHeatingModule(nameof(HeatingControlPropertyModelEdit.RightValveDispensingHead), HeatingControlPropertyModelEdit.RightValveDispensingHead);
                        continue;
                    }
                    if (string.Compare(propId, nameof(HeatingControlPropertyModelEdit.RightValveCartridgeHeating)) == 0)
                    {
                        SetPropertyValue(nameof(HeatingControlPropertyModelEdit.RightValveCartridgeHeating), gFBaseTypePropValue.Value, true);
                        heatingControlViewModel.SetHeatingModule(nameof(HeatingControlPropertyModelEdit.RightValveCartridgeHeating), HeatingControlPropertyModelEdit.RightValveCartridgeHeating);
                        continue;
                    }
                    if (string.Compare(propId, nameof(HeatingControlPropertyModelEdit.LeftValveDispensingHead)) == 0)
                    {
                        SetPropertyValue(nameof(HeatingControlPropertyModelEdit.LeftValveDispensingHead), gFBaseTypePropValue.Value, true);
                        heatingControlViewModel.SetHeatingModule(nameof(HeatingControlPropertyModelEdit.LeftValveDispensingHead), HeatingControlPropertyModelEdit.LeftValveDispensingHead);
                        continue;
                    }
                    if (string.Compare(propId, nameof(HeatingControlPropertyModelEdit.LeftValveCartridgeHeating)) == 0)
                    {
                        SetPropertyValue(nameof(HeatingControlPropertyModelEdit.LeftValveCartridgeHeating), gFBaseTypePropValue.Value, true);
                        heatingControlViewModel.SetHeatingModule(nameof(HeatingControlPropertyModelEdit.LeftValveCartridgeHeating), HeatingControlPropertyModelEdit.LeftValveCartridgeHeating);
                        continue;
                    }

                    if (string.Compare(propId, nameof(HeatingControlPropertyModelEdit.ARailPreheatLeft)) == 0)
                    {
                        SetPropertyValue(nameof(HeatingControlPropertyModelEdit.ARailPreheatLeft), gFBaseTypePropValue.Value, true);
                        heatingControlViewModel.SetHeatingModule(nameof(HeatingControlPropertyModelEdit.ARailPreheatLeft), HeatingControlPropertyModelEdit.ARailPreheatLeft);
                        continue;
                    }
                    if (string.Compare(propId, nameof(HeatingControlPropertyModelEdit.ARailPreheatLeft2)) == 0)
                    {
                        SetPropertyValue(nameof(HeatingControlPropertyModelEdit.ARailPreheatLeft2), gFBaseTypePropValue.Value, true);
                        heatingControlViewModel.SetHeatingModule(nameof(HeatingControlPropertyModelEdit.ARailPreheatLeft2), HeatingControlPropertyModelEdit.ARailPreheatLeft2);
                        continue;
                    }
                    if (string.Compare(propId, nameof(HeatingControlPropertyModelEdit.ARailGlueBoardStationMiddle)) == 0)
                    {
                        SetPropertyValue(nameof(HeatingControlPropertyModelEdit.ARailGlueBoardStationMiddle), gFBaseTypePropValue.Value, true);
                        heatingControlViewModel.SetHeatingModule(nameof(HeatingControlPropertyModelEdit.ARailGlueBoardStationMiddle), HeatingControlPropertyModelEdit.ARailGlueBoardStationMiddle);
                        continue;
                    }
                    if (string.Compare(propId, nameof(HeatingControlPropertyModelEdit.ARailDispensingStationMiddle2)) == 0)
                    {
                        SetPropertyValue(nameof(HeatingControlPropertyModelEdit.ARailDispensingStationMiddle2), gFBaseTypePropValue.Value, true);
                        heatingControlViewModel.SetHeatingModule(nameof(HeatingControlPropertyModelEdit.ARailDispensingStationMiddle2), HeatingControlPropertyModelEdit.ARailDispensingStationMiddle2);
                        continue;
                    }
                    if (string.Compare(propId, nameof(HeatingControlPropertyModelEdit.ARailPreheatRight)) == 0)
                    {
                        SetPropertyValue(nameof(HeatingControlPropertyModelEdit.ARailPreheatRight), gFBaseTypePropValue.Value, true);
                        heatingControlViewModel.SetHeatingModule(nameof(HeatingControlPropertyModelEdit.ARailPreheatRight), HeatingControlPropertyModelEdit.ARailPreheatRight);
                        continue;
                    }
                    if (string.Compare(propId, nameof(HeatingControlPropertyModelEdit.ARailPreheatRight2)) == 0)
                    {
                        SetPropertyValue(nameof(HeatingControlPropertyModelEdit.ARailPreheatRight2), gFBaseTypePropValue.Value, true);
                        heatingControlViewModel.SetHeatingModule(nameof(HeatingControlPropertyModelEdit.ARailPreheatRight2), HeatingControlPropertyModelEdit.ARailPreheatRight2);
                        continue;
                    }

                    if (string.Compare(propId, nameof(HeatingControlPropertyModelEdit.BRailPreheatLeft)) == 0)
                    {
                        SetPropertyValue(nameof(HeatingControlPropertyModelEdit.BRailPreheatLeft), gFBaseTypePropValue.Value, true);
                        heatingControlViewModel.SetHeatingModule(nameof(HeatingControlPropertyModelEdit.BRailPreheatLeft), HeatingControlPropertyModelEdit.BRailPreheatLeft);
                        continue;
                    }
                    if (string.Compare(propId, nameof(HeatingControlPropertyModelEdit.BRailPreheatLeft2)) == 0)
                    {
                        SetPropertyValue(nameof(HeatingControlPropertyModelEdit.BRailPreheatLeft2), gFBaseTypePropValue.Value, true);
                        heatingControlViewModel.SetHeatingModule(nameof(HeatingControlPropertyModelEdit.BRailPreheatLeft2), HeatingControlPropertyModelEdit.BRailPreheatLeft2);
                        continue;
                    }
                    if (string.Compare(propId, nameof(HeatingControlPropertyModelEdit.BRailGlueBoardStationMiddle)) == 0)
                    {
                        SetPropertyValue(nameof(HeatingControlPropertyModelEdit.BRailGlueBoardStationMiddle), gFBaseTypePropValue.Value, true);
                        heatingControlViewModel.SetHeatingModule(nameof(HeatingControlPropertyModelEdit.BRailGlueBoardStationMiddle), HeatingControlPropertyModelEdit.BRailGlueBoardStationMiddle);
                        continue;
                    }
                    if (string.Compare(propId, nameof(HeatingControlPropertyModelEdit.BRailDispensingStationMiddle2)) == 0)
                    {
                        SetPropertyValue(nameof(HeatingControlPropertyModelEdit.BRailDispensingStationMiddle2), gFBaseTypePropValue.Value, true);
                        heatingControlViewModel.SetHeatingModule(nameof(HeatingControlPropertyModelEdit.BRailDispensingStationMiddle2), HeatingControlPropertyModelEdit.BRailDispensingStationMiddle2);
                        continue;
                    }
                    if (string.Compare(propId, nameof(HeatingControlPropertyModelEdit.BRailPreheatRight)) == 0)
                    {
                        SetPropertyValue(nameof(HeatingControlPropertyModelEdit.BRailPreheatRight), gFBaseTypePropValue.Value, true);
                        heatingControlViewModel.SetHeatingModule(nameof(HeatingControlPropertyModelEdit.BRailPreheatRight), HeatingControlPropertyModelEdit.BRailPreheatRight);
                        continue;
                    }
                    if (string.Compare(propId, nameof(HeatingControlPropertyModelEdit.BRailPreheatRight2)) == 0)
                    {
                        SetPropertyValue(nameof(HeatingControlPropertyModelEdit.BRailPreheatRight2), gFBaseTypePropValue.Value, true);
                        heatingControlViewModel.SetHeatingModule(nameof(HeatingControlPropertyModelEdit.BRailPreheatRight2), HeatingControlPropertyModelEdit.BRailPreheatRight2);
                        continue;
                    }
                }
                return true;
            }

        private bool TryApplyHeatingModuleSubProperty(string propId, GriffinsBaseValue value)
        {
            try
            {
                int idx = propId.IndexOf('_');
                if (idx <= 0 || idx >= propId.Length - 1)
                    return false;

                string modulePropId = propId.Substring(0, idx);
                string subPropId = propId.Substring(idx + 1);
                if (string.IsNullOrWhiteSpace(modulePropId) || string.IsNullOrWhiteSpace(subPropId))
                    return false;

                var moduleProp = typeof(HeatingControlPropertyModelEdit).GetProperty(modulePropId);
                if (moduleProp == null || moduleProp.PropertyType != typeof(HeatingModuleInfo))
                    return false;

                var moduleObj = moduleProp.GetValue(HeatingControlPropertyModelEdit) as HeatingModuleInfo;
                if (moduleObj == null)
                {
                    moduleObj = new HeatingModuleInfo();
                    moduleProp.SetValue(HeatingControlPropertyModelEdit, moduleObj);
                }

                var subProp = typeof(HeatingModuleInfo).GetProperty(subPropId);
                if (subProp == null || !subProp.CanWrite)
                    return false;

                object typedVal;
                if (subProp.PropertyType == typeof(bool))
                    typedVal = value == null ? default(bool) : value.ToPrimitiveValue<bool>();
                else if (subProp.PropertyType == typeof(int))
                    typedVal = value == null ? default(int) : value.ToPrimitiveValue<int>();
                else if (subProp.PropertyType == typeof(string))
                    typedVal = value == null ? string.Empty : value.ToPrimitiveValue<string>();
                else
                    return false;

                object currentVal = subProp.GetValue(moduleObj);
                if (Equals(currentVal, typedVal))
                    return true;

                subProp.SetValue(moduleObj, typedVal);

                if (moduleObj is IGriffinsBaseValue objAsBaseValue)
                    SetPropertyValue(modulePropId, objAsBaseValue.ToBaseValue(), true);
                else
                    SetPropertyValue(modulePropId, new GriffinsBaseValue(moduleObj), true);

                heatingControlViewModel.SetHeatingModule(modulePropId, moduleObj);
                return true;
            }
            catch
            {
                return false;
            }
        }

        protected override void AfterPropertyChanged(string propertyID, GriffinsBaseValue propertyValue)
        {
            base.AfterPropertyChanged(propertyID, propertyValue);

            if (string.Compare(propertyID, nameof(HeatingControlPropertyModelEdit.TextColor)) == 0)
            {
                CallBack?.ExecOprt(nameof(HeatingControlPropertyModelEdit.TextColor));
            }
            if (string.Compare(propertyID, nameof(HeatingControlPropertyModelEdit.BackColor)) == 0)
            {
                CallBack?.ExecOprt(nameof(HeatingControlPropertyModelEdit.BackColor));
            }
            if (string.Compare(propertyID, nameof(HeatingControlPropertyModelEdit.TextFont)) == 0)
            {
                CallBack?.ExecOprt(nameof(HeatingControlPropertyModelEdit.TextFont));
            }
            if (string.Compare(propertyID, nameof(HeatingControlPropertyModelEdit.IsDualValve)) == 0)
            {
                    CallBack?.ExecOprt(nameof(HeatingControlPropertyModelEdit.IsDualValve));
            }
            if (string.Compare(propertyID, nameof(HeatingControlPropertyModelEdit.IsDualTrack)) == 0)
            {
                    CallBack?.ExecOprt(nameof(HeatingControlPropertyModelEdit.IsDualTrack));
            }

            if (string.Compare(propertyID, nameof(HeatingControlPropertyModelEdit.RightValveDispensingHead)) == 0)
            {
                CallBack?.ExecOprt(nameof(HeatingControlPropertyModelEdit.RightValveDispensingHead));
            }
            if (string.Compare(propertyID, nameof(HeatingControlPropertyModelEdit.RightValveCartridgeHeating)) == 0)
            {
                CallBack?.ExecOprt(nameof(HeatingControlPropertyModelEdit.RightValveCartridgeHeating));
            }
            if (string.Compare(propertyID, nameof(HeatingControlPropertyModelEdit.LeftValveDispensingHead)) == 0)
            {
                CallBack?.ExecOprt(nameof(HeatingControlPropertyModelEdit.LeftValveDispensingHead));
            }
            if (string.Compare(propertyID, nameof(HeatingControlPropertyModelEdit.LeftValveCartridgeHeating)) == 0)
            {
                CallBack?.ExecOprt(nameof(HeatingControlPropertyModelEdit.LeftValveCartridgeHeating));
            }
            if (string.Compare(propertyID, nameof(HeatingControlPropertyModelEdit.ARailPreheatLeft)) == 0)
            {
                CallBack?.ExecOprt(nameof(HeatingControlPropertyModelEdit.ARailPreheatLeft));
            }
            if (string.Compare(propertyID, nameof(HeatingControlPropertyModelEdit.ARailPreheatLeft2)) == 0)
            {
                CallBack?.ExecOprt(nameof(HeatingControlPropertyModelEdit.ARailPreheatLeft2));
            }
            if (string.Compare(propertyID, nameof(HeatingControlPropertyModelEdit.ARailGlueBoardStationMiddle)) == 0)
            {
                CallBack?.ExecOprt(nameof(HeatingControlPropertyModelEdit.ARailGlueBoardStationMiddle));
            }
            if (string.Compare(propertyID, nameof(HeatingControlPropertyModelEdit.ARailDispensingStationMiddle2)) == 0)
            {
                CallBack?.ExecOprt(nameof(HeatingControlPropertyModelEdit.ARailDispensingStationMiddle2));
            }
            if (string.Compare(propertyID, nameof(HeatingControlPropertyModelEdit.ARailPreheatRight)) == 0)
            {
                CallBack?.ExecOprt(nameof(HeatingControlPropertyModelEdit.ARailPreheatRight));
            }
            if (string.Compare(propertyID, nameof(HeatingControlPropertyModelEdit.ARailPreheatRight2)) == 0)
            {
                CallBack?.ExecOprt(nameof(HeatingControlPropertyModelEdit.ARailPreheatRight2));
            }
            if (string.Compare(propertyID, nameof(HeatingControlPropertyModelEdit.BRailPreheatLeft)) == 0)
            {
                CallBack?.ExecOprt(nameof(HeatingControlPropertyModelEdit.BRailPreheatLeft));
            }
            if (string.Compare(propertyID, nameof(HeatingControlPropertyModelEdit.BRailPreheatLeft2)) == 0)
            {
                CallBack?.ExecOprt(nameof(HeatingControlPropertyModelEdit.BRailPreheatLeft2));
            }
            if (string.Compare(propertyID, nameof(HeatingControlPropertyModelEdit.BRailGlueBoardStationMiddle)) == 0)
            {
                CallBack?.ExecOprt(nameof(HeatingControlPropertyModelEdit.BRailGlueBoardStationMiddle));
            }
            if (string.Compare(propertyID, nameof(HeatingControlPropertyModelEdit.BRailDispensingStationMiddle2)) == 0)
            {
                CallBack?.ExecOprt(nameof(HeatingControlPropertyModelEdit.BRailDispensingStationMiddle2));
            }
            if (string.Compare(propertyID, nameof(HeatingControlPropertyModelEdit.BRailPreheatRight)) == 0)
            {
                CallBack?.ExecOprt(nameof(HeatingControlPropertyModelEdit.BRailPreheatRight));
            }
            if (string.Compare(propertyID, nameof(HeatingControlPropertyModelEdit.BRailPreheatRight2)) == 0)
            {
                CallBack?.ExecOprt(nameof(HeatingControlPropertyModelEdit.BRailPreheatRight2));
            }

            if (!HeatingControlPropertyModelEdit.IsRuning)
            {
                GFBaseTypePropValueList gFBaseTypePropValues = new GFBaseTypePropValueList();

                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(HeatingControlPropertyModelEdit.TextColor)), new GriffinsBaseValue(HeatingControlPropertyModelEdit.TextColor.ToColorString())));
                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(HeatingControlPropertyModelEdit.BackColor)), new GriffinsBaseValue(HeatingControlPropertyModelEdit.BackColor.ToColorString())));
                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(HeatingControlPropertyModelEdit.TextFont)), new GriffinsBaseValue(HeatingControlPropertyModelEdit.TextFont)));
                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(HeatingControlPropertyModelEdit.IsDualValve)), new GriffinsBaseValue(HeatingControlPropertyModelEdit.IsDualValve)));
                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(HeatingControlPropertyModelEdit.IsDualTrack)), new GriffinsBaseValue(HeatingControlPropertyModelEdit.IsDualTrack)));

                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(HeatingControlPropertyModelEdit.RightValveDispensingHead)), new GriffinsBaseValue(HeatingControlPropertyModelEdit.RightValveDispensingHead)));
                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(HeatingControlPropertyModelEdit.RightValveCartridgeHeating)), new GriffinsBaseValue(HeatingControlPropertyModelEdit.RightValveCartridgeHeating)));
                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(HeatingControlPropertyModelEdit.LeftValveDispensingHead)), new GriffinsBaseValue(HeatingControlPropertyModelEdit.LeftValveDispensingHead)));
                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(HeatingControlPropertyModelEdit.LeftValveCartridgeHeating)), new GriffinsBaseValue(HeatingControlPropertyModelEdit.LeftValveCartridgeHeating)));
                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(HeatingControlPropertyModelEdit.ARailPreheatLeft)), new GriffinsBaseValue(HeatingControlPropertyModelEdit.ARailPreheatLeft)));
                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(HeatingControlPropertyModelEdit.ARailPreheatLeft2)), new GriffinsBaseValue(HeatingControlPropertyModelEdit.ARailPreheatLeft2)));
                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(HeatingControlPropertyModelEdit.ARailGlueBoardStationMiddle)), new GriffinsBaseValue(HeatingControlPropertyModelEdit.ARailGlueBoardStationMiddle)));
                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(HeatingControlPropertyModelEdit.ARailDispensingStationMiddle2)), new GriffinsBaseValue(HeatingControlPropertyModelEdit.ARailDispensingStationMiddle2)));
                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(HeatingControlPropertyModelEdit.ARailPreheatRight)), new GriffinsBaseValue(HeatingControlPropertyModelEdit.ARailPreheatRight)));
                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(HeatingControlPropertyModelEdit.ARailPreheatRight2)), new GriffinsBaseValue(HeatingControlPropertyModelEdit.ARailPreheatRight2)));
                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(HeatingControlPropertyModelEdit.BRailPreheatLeft)), new GriffinsBaseValue(HeatingControlPropertyModelEdit.BRailPreheatLeft)));
                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(HeatingControlPropertyModelEdit.BRailPreheatLeft2)), new GriffinsBaseValue(HeatingControlPropertyModelEdit.BRailPreheatLeft2)));
                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(HeatingControlPropertyModelEdit.BRailGlueBoardStationMiddle)), new GriffinsBaseValue(HeatingControlPropertyModelEdit.BRailGlueBoardStationMiddle)));
                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(HeatingControlPropertyModelEdit.BRailDispensingStationMiddle2)), new GriffinsBaseValue(HeatingControlPropertyModelEdit.BRailDispensingStationMiddle2)));
                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(HeatingControlPropertyModelEdit.BRailPreheatRight)), new GriffinsBaseValue(HeatingControlPropertyModelEdit.BRailPreheatRight)));
                gFBaseTypePropValues.Add(new GFBaseTypePropValue(new MPPropertyID(nameof(HeatingControlPropertyModelEdit.BRailPreheatRight2)), new GriffinsBaseValue(HeatingControlPropertyModelEdit.BRailPreheatRight2)));

                CallBack?.UpdateUIDataObjPropValues(gFBaseTypePropValues);
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
            var propertyEditModelBase = JsonObjConvert.FromJSon<HeatingControlPropertyModelEdit>(br.ReadString("PropertyEditModelBase"));
            (PropertyEditModelBase as HeatingControlPropertyModelEdit).CopyFrom(propertyEditModelBase);
            var propertyBindEditModelBase = JsonObjConvert.FromJSon<HeatingControlPropertyBindEditModel>(br.ReadString("PropertyBindEditModelBase"));
            (PropertyBindEditModelBase as HeatingControlPropertyBindEditModel).CopyFrom(propertyBindEditModelBase);
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
            MapCellHeatingControlCtlObj mapCellHeatingControlCtlObj = (source as MapCellHeatingControlCtlObj);
            base._CopyFrom(mapCellHeatingControlCtlObj);
            (PropertyEditModelBase).CopyFrom(source.PropertyEditModelBase);
            (PropertyBindEditModelBase).CopyFrom(source.PropertyBindEditModelBase);
            EventBindEditModel.CopyFrom(source.EventBindEditModel);
        }

        /// <summary>
        /// 初始化时
        /// </summary>
        protected override void OnInit()
        {
            base.OnInit();

            CallBack?.ExecOprt(nameof(HeatingControlPropertyModelEdit.TextColor));
            CallBack?.ExecOprt(nameof(HeatingControlPropertyModelEdit.BackColor));
            CallBack?.ExecOprt(nameof(HeatingControlPropertyModelEdit.TextFont));
            CallBack?.ExecOprt(nameof(HeatingControlPropertyModelEdit.IsDualValve));
            CallBack?.ExecOprt(nameof(HeatingControlPropertyModelEdit.IsDualTrack));

            CallBack?.ExecOprt(nameof(HeatingControlPropertyModelEdit.RightValveDispensingHead));
            CallBack?.ExecOprt(nameof(HeatingControlPropertyModelEdit.RightValveCartridgeHeating));
            CallBack?.ExecOprt(nameof(HeatingControlPropertyModelEdit.LeftValveDispensingHead));
            CallBack?.ExecOprt(nameof(HeatingControlPropertyModelEdit.LeftValveCartridgeHeating));

            CallBack?.ExecOprt(nameof(HeatingControlPropertyModelEdit.ARailPreheatLeft));
            CallBack?.ExecOprt(nameof(HeatingControlPropertyModelEdit.ARailPreheatLeft2));
            CallBack?.ExecOprt(nameof(HeatingControlPropertyModelEdit.ARailGlueBoardStationMiddle));
            CallBack?.ExecOprt(nameof(HeatingControlPropertyModelEdit.ARailDispensingStationMiddle2));
            CallBack?.ExecOprt(nameof(HeatingControlPropertyModelEdit.ARailPreheatRight));
            CallBack?.ExecOprt(nameof(HeatingControlPropertyModelEdit.ARailPreheatRight2));

            CallBack?.ExecOprt(nameof(HeatingControlPropertyModelEdit.BRailPreheatLeft));
            CallBack?.ExecOprt(nameof(HeatingControlPropertyModelEdit.BRailPreheatLeft2));
            CallBack?.ExecOprt(nameof(HeatingControlPropertyModelEdit.BRailGlueBoardStationMiddle));
            CallBack?.ExecOprt(nameof(HeatingControlPropertyModelEdit.BRailDispensingStationMiddle2));
            CallBack?.ExecOprt(nameof(HeatingControlPropertyModelEdit.BRailPreheatRight));
            CallBack?.ExecOprt(nameof(HeatingControlPropertyModelEdit.BRailPreheatRight2));
        }

        /// <summary>
        /// 获取视图
        /// </summary>
        /// <returns>视图对象</returns>
        protected override object OnGetView()
        {
            return view;
        }

        protected override object OnGetViewModel()
        {
            return heatingControlViewModel;
        }

        /// <summary>
        /// 创建图元属性编辑模型对象
        /// </summary>
        /// <returns>图元属性编辑模型对象</returns>
        public override PropertyEditModelBase CreatePropertyModelEditBase()
        {
            return new HeatingControlPropertyModelEdit();
        }

        /// <summary>
        /// 创建图元属性绑定编辑模型对象
        /// </summary>
        /// <returns>图元属性绑定编辑模型对象</returns>
        public override PropertyBindEditModelBase CreatePropertyBindEditModelBase()
        {
            var m = new HeatingControlPropertyBindEditModel();
            ApplyDefaultTestBindings(m);
            return m;
        }

        private static void ApplyDefaultTestBindings(HeatingControlPropertyBindEditModel m)
        {
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

        public void SetButtonTextFont()
        {
            double size = base.CallBack?.Calc?.CalcZoomVal((decimal)this.HeatingControlPropertyModelEdit.TextFont.FontSize) ?? this.HeatingControlPropertyModelEdit.TextFont.FontSize;
            if (size < 2)
                size = 2;
            FontInfo font = new FontInfo(this.HeatingControlPropertyModelEdit.TextFont.FontFamily, size, this.HeatingControlPropertyModelEdit.TextFont.FontWeight, this.HeatingControlPropertyModelEdit.TextFont.FontStyle);
            this.heatingControlViewModel.TextFont = font;
        }
        public override string ToString()
        {
            return "加热控制";
        }

        #region 操作原子执行对象
        private class TextFontMapOprtCellExector : IMapOprtCellExector
        {
            private MapCellHeatingControlCtlObj mapCellHeatingControlCtlObj;
            private IMapOprtCellExectorCallBack callBack;

            public TextFontMapOprtCellExector(MapCellHeatingControlCtlObj mapCellHeatingControlCtlObj)
            {
                this.mapCellHeatingControlCtlObj = mapCellHeatingControlCtlObj;
            }

            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack)
            {
                this.callBack = callBack;
            }

            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                object viewModel = callBack.GetMapCellVMObjInstance();
                if (viewModel != null && viewModel is HeatingControlViewModel heatingControlViewModel)
                {
                    GriffinsBaseValue mapCellPropValue = callBack.GetMapCellPropValue(nameof(HeatingControlPropertyModelEdit.TextFont));
                    if (mapCellPropValue is { })
                    {
                        ObjectValue_Json objectValue_Json = mapCellPropValue.ToObjectValue_Json();
                        GriffinsBaseValue griffinsBaseValue = GriffinsBaseValue.Create(objectValue_Json);
                        IGriffinsBaseValue iMPPropObjectValue = new FontInfo();
                        iMPPropObjectValue.PopulateFromBaseValue(griffinsBaseValue);
                        heatingControlViewModel.TextFont = (FontInfo)iMPPropObjectValue;
                    }
                }
            }
        }

        private class TextColorMapOprtCellExector : IMapOprtCellExector
        {
            private MapCellHeatingControlCtlObj mapCellHeatingControlCtlObj;
            private IMapOprtCellExectorCallBack callBack;

            public TextColorMapOprtCellExector(MapCellHeatingControlCtlObj mapCellHeatingControlCtlObj)
            {
                this.mapCellHeatingControlCtlObj = mapCellHeatingControlCtlObj;
            }

            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack)
            {
                this.callBack = callBack;
            }

            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                object viewModel = callBack.GetMapCellVMObjInstance();
                if (viewModel != null && viewModel is HeatingControlViewModel heatingControlViewModel)
                {
                    GriffinsBaseValue mapCellPropValue = callBack.GetMapCellPropValue(nameof(HeatingControlPropertyModelEdit.TextColor));
                    if (mapCellPropValue is { })
                    {
                        var color = mapCellPropValue.ToPrimitiveValue<string>();
                        heatingControlViewModel.TextColor = Color.Parse(color);
                    }
                }
            }
        }

        private class BackColorMapOprtCellExector : IMapOprtCellExector
        {
            private MapCellHeatingControlCtlObj mapCellHeatingControlCtlObj;
            private IMapOprtCellExectorCallBack callBack;

            public BackColorMapOprtCellExector(MapCellHeatingControlCtlObj mapCellHeatingControlCtlObj)
            {
                this.mapCellHeatingControlCtlObj = mapCellHeatingControlCtlObj;
            }

            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack)
            {
                this.callBack = callBack;
            }

            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                object viewModel = callBack.GetMapCellVMObjInstance();
                if (viewModel != null && viewModel is HeatingControlViewModel heatingControlViewModel)
                {
                    GriffinsBaseValue mapCellPropValue = callBack.GetMapCellPropValue(nameof(HeatingControlPropertyModelEdit.BackColor));
                    if (mapCellPropValue is { })
                    {
                        var color = mapCellPropValue.ToPrimitiveValue<string>();
                        heatingControlViewModel.BackColor = Color.Parse(color);
                    }
                }
            }
        }

        private class IsDualValveMapOprtCellExector : IMapOprtCellExector
        {
            private MapCellHeatingControlCtlObj mapCellHeatingControlCtlObj;
            private IMapOprtCellExectorCallBack callBack;

            public IsDualValveMapOprtCellExector(MapCellHeatingControlCtlObj mapCellHeatingControlCtlObj)
            {
                this.mapCellHeatingControlCtlObj = mapCellHeatingControlCtlObj;
            }

            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack)
            {
                this.callBack = callBack;
            }

            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                object viewModel = callBack.GetMapCellVMObjInstance();
                if (viewModel != null && viewModel is HeatingControlViewModel heatingControlViewModel)
                {
                    GriffinsBaseValue mapCellPropValue = callBack.GetMapCellPropValue(nameof(HeatingControlPropertyModelEdit.IsDualValve));
                    if (mapCellPropValue is { })
                    {
                        heatingControlViewModel.IsDualValve = ParseBool(mapCellPropValue);
                    }
                }
            }
        }

        private class IsDualTrackMapOprtCellExector : IMapOprtCellExector
        {
            private MapCellHeatingControlCtlObj mapCellHeatingControlCtlObj;
            private IMapOprtCellExectorCallBack callBack;

            public IsDualTrackMapOprtCellExector(MapCellHeatingControlCtlObj mapCellHeatingControlCtlObj)
            {
                this.mapCellHeatingControlCtlObj = mapCellHeatingControlCtlObj;
            }

            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack)
            {
                this.callBack = callBack;
            }

            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                object viewModel = callBack.GetMapCellVMObjInstance();
                if (viewModel != null && viewModel is HeatingControlViewModel heatingControlViewModel)
                {
                    GriffinsBaseValue mapCellPropValue = callBack.GetMapCellPropValue(nameof(HeatingControlPropertyModelEdit.IsDualTrack));
                    if (mapCellPropValue is { })
                    {
                        heatingControlViewModel.IsDualTrack = ParseBool(mapCellPropValue);
                    }
                }
            }
        }

        private class HeatingModuleMapOprtCellExector : IMapOprtCellExector
        {
            private MapCellHeatingControlCtlObj mapCellHeatingControlCtlObj;
            private string modulePropName;
            private IMapOprtCellExectorCallBack callBack;

            public HeatingModuleMapOprtCellExector(MapCellHeatingControlCtlObj mapCellHeatingControlCtlObj, string modulePropName)
            {
                this.mapCellHeatingControlCtlObj = mapCellHeatingControlCtlObj;
                this.modulePropName = modulePropName;
            }

            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack)
            {
                this.callBack = callBack;
            }

            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                object viewModel = callBack.GetMapCellVMObjInstance();
                if (viewModel != null && viewModel is HeatingControlViewModel heatingControlViewModel)
                {
                    GriffinsBaseValue mapCellPropValue = callBack.GetMapCellPropValue(modulePropName);
                    if (mapCellPropValue is { })
                    {
                        ObjectValue_Json objectValue_Json = mapCellPropValue.ToObjectValue_Json();
                        GriffinsBaseValue griffinsBaseValue = GriffinsBaseValue.Create(objectValue_Json);
                        IGriffinsBaseValue iMPPropObjectValue = new HeatingModuleInfo();
                        iMPPropObjectValue.PopulateFromBaseValue(griffinsBaseValue);
                        heatingControlViewModel.SetHeatingModule(modulePropName, (HeatingModuleInfo)iMPPropObjectValue);
                    }
                }
            }
        }
        #endregion

        private static bool ParseBool(GriffinsBaseValue v)
        {
            if (v == null)
                return false;

            try
            {
                return v.ToPrimitiveValue<bool>();
            }
            catch
            {
            }

            try
            {
                var i = v.ToPrimitiveValue<int>();
                return i != 0;
            }
            catch
            {
            }

            string str = null;
            try
            {
                str = v.ToPrimitiveValue<string>();
            }
            catch
            {
                str = null;
            }

            if (string.IsNullOrWhiteSpace(str))
                return false;

            if (bool.TryParse(str, out var b))
                return b;

            if (int.TryParse(str, out var i2))
                return i2 != 0;

            return string.Equals(str.Trim(), "Y", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(str.Trim(), "Yes", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(str.Trim(), "On", StringComparison.OrdinalIgnoreCase);
        }
    }

    /// <summary>
    /// 加热控制属性编辑模型对象
    /// </summary>
    [Serializable]
    [MapPropertyOrder]
    [CategoryPriority("图元信息", 1)]
    [CategoryPriority("加热控制", 2)]
    public class HeatingControlPropertyModelEdit : FunctionalCellPropertyModelEdit
    {
        public HeatingControlPropertyModelEdit()
        {
            TextFont.PropertyChanged += textFont_PropertyChanged;

            _rightValveDispensingHead_PropertyChanged = (s, e) => { RaisePropertyChanged(nameof(RightValveDispensingHead)); };
            _rightValveCartridgeHeating_PropertyChanged = (s, e) => { RaisePropertyChanged(nameof(RightValveCartridgeHeating)); };
            _leftValveDispensingHead_PropertyChanged = (s, e) => { RaisePropertyChanged(nameof(LeftValveDispensingHead)); };
            _leftValveCartridgeHeating_PropertyChanged = (s, e) => { RaisePropertyChanged(nameof(LeftValveCartridgeHeating)); };

            _aRailPreheatLeft_PropertyChanged = (s, e) => { RaisePropertyChanged(nameof(ARailPreheatLeft)); };
            _aRailPreheatLeft2_PropertyChanged = (s, e) => { RaisePropertyChanged(nameof(ARailPreheatLeft2)); };
            _aRailGlueBoardStationMiddle_PropertyChanged = (s, e) => { RaisePropertyChanged(nameof(ARailGlueBoardStationMiddle)); };
            _aRailDispensingStationMiddle2_PropertyChanged = (s, e) => { RaisePropertyChanged(nameof(ARailDispensingStationMiddle2)); };
            _aRailPreheatRight_PropertyChanged = (s, e) => { RaisePropertyChanged(nameof(ARailPreheatRight)); };
            _aRailPreheatRight2_PropertyChanged = (s, e) => { RaisePropertyChanged(nameof(ARailPreheatRight2)); };

            _bRailPreheatLeft_PropertyChanged = (s, e) => { RaisePropertyChanged(nameof(BRailPreheatLeft)); };
            _bRailPreheatLeft2_PropertyChanged = (s, e) => { RaisePropertyChanged(nameof(BRailPreheatLeft2)); };
            _bRailGlueBoardStationMiddle_PropertyChanged = (s, e) => { RaisePropertyChanged(nameof(BRailGlueBoardStationMiddle)); };
            _bRailDispensingStationMiddle2_PropertyChanged = (s, e) => { RaisePropertyChanged(nameof(BRailDispensingStationMiddle2)); };
            _bRailPreheatRight_PropertyChanged = (s, e) => { RaisePropertyChanged(nameof(BRailPreheatRight)); };
            _bRailPreheatRight2_PropertyChanged = (s, e) => { RaisePropertyChanged(nameof(BRailPreheatRight2)); };

            InitializeHeatingModules();
        }

        private void InitializeHeatingModules()
        {
            RightValveDispensingHead = new HeatingModuleInfo { Switch = true, ModuleName = "右阀点胶头", CurrentTemperature = 0, SetTemperature = 35, TemperatureRange = 7, IdleCloseInterval = 9999, PreheatTime = 100, DetectionEnabled = false, WorkEnabled = false, DelayTime = 3 };
            RightValveCartridgeHeating = new HeatingModuleInfo { Switch = true, ModuleName = "右阀胶筒加热", CurrentTemperature = 0, SetTemperature = 45, TemperatureRange = 5, IdleCloseInterval = 999, PreheatTime = 10, DetectionEnabled = false, WorkEnabled = false, DelayTime = 3 };
            LeftValveDispensingHead = new HeatingModuleInfo { Switch = true, ModuleName = "左阀点胶头", CurrentTemperature = 0, SetTemperature = 35, TemperatureRange = 5, IdleCloseInterval = 9999, PreheatTime = 100, DetectionEnabled = false, WorkEnabled = false, DelayTime = 3 };
            LeftValveCartridgeHeating = new HeatingModuleInfo { Switch = true, ModuleName = "左阀胶筒加热", CurrentTemperature = 0, SetTemperature = 43, TemperatureRange = 5, IdleCloseInterval = 999, PreheatTime = 10, DetectionEnabled = false, WorkEnabled = false, DelayTime = 3 };
            ARailPreheatLeft = new HeatingModuleInfo { Switch = true, ModuleName = "A轨预热-左", CurrentTemperature = 0, SetTemperature = 30, TemperatureRange = 10, IdleCloseInterval = 0, PreheatTime = 3, HeatingTime = 3, DetectionEnabled = false, WorkEnabled = false, DelayTime = 3 };
            ARailPreheatLeft2 = new HeatingModuleInfo { Switch = true, ModuleName = "A轨预热-左(2)", CurrentTemperature = 0, SetTemperature = 30, TemperatureRange = 3, IdleCloseInterval = 0, PreheatTime = 3, HeatingTime = 3, DetectionEnabled = false, WorkEnabled = false, DelayTime = 3 };
            ARailGlueBoardStationMiddle = new HeatingModuleInfo { Switch = true, ModuleName = "A轨胶板工位-中", CurrentTemperature = 0, SetTemperature = 30, TemperatureRange = 10, IdleCloseInterval = 0, PreheatTime = 3, HeatingTime = 3, DetectionEnabled = false, WorkEnabled = false, DelayTime = 3 };
            ARailDispensingStationMiddle2 = new HeatingModuleInfo { Switch = true, ModuleName = "A轨点胶工位-中(2)", CurrentTemperature = 0, SetTemperature = 30, TemperatureRange = 3, IdleCloseInterval = 0, PreheatTime = 3, HeatingTime = 3, DetectionEnabled = false, WorkEnabled = false, DelayTime = 3 };
            ARailPreheatRight = new HeatingModuleInfo { Switch = true, ModuleName = "A轨预热-右", CurrentTemperature = 0, SetTemperature = 30, TemperatureRange = 10, IdleCloseInterval = 0, PreheatTime = 3, HeatingTime = 3, DetectionEnabled = false, WorkEnabled = false, DelayTime = 3 };
            ARailPreheatRight2 = new HeatingModuleInfo { Switch = true, ModuleName = "A轨预热-右(2)", CurrentTemperature = 0, SetTemperature = 30, TemperatureRange = 3, IdleCloseInterval = 0, PreheatTime = 3, HeatingTime = 3, DetectionEnabled = false, WorkEnabled = false, DelayTime = 3 };
            BRailPreheatLeft = new HeatingModuleInfo { Switch = true, ModuleName = "B轨预热-左", CurrentTemperature = 0, SetTemperature = 30, TemperatureRange = 10, IdleCloseInterval = 0, PreheatTime = 3, HeatingTime = 3, DetectionEnabled = false, WorkEnabled = false, DelayTime = 3 };
            BRailPreheatLeft2 = new HeatingModuleInfo { Switch = true, ModuleName = "B轨预热-左(2)", CurrentTemperature = 0, SetTemperature = 30, TemperatureRange = 3, IdleCloseInterval = 0, PreheatTime = 3, HeatingTime = 3, DetectionEnabled = false, WorkEnabled = false, DelayTime = 3 };
            BRailGlueBoardStationMiddle = new HeatingModuleInfo { Switch = true, ModuleName = "B轨胶板工位-中", CurrentTemperature = 0, SetTemperature = 30, TemperatureRange = 10, IdleCloseInterval = 0, PreheatTime = 3, HeatingTime = 3, DetectionEnabled = false, WorkEnabled = false, DelayTime = 3 };
            BRailDispensingStationMiddle2 = new HeatingModuleInfo { Switch = true, ModuleName = "B轨点胶工位-中(2)", CurrentTemperature = 0, SetTemperature = 30, TemperatureRange = 3, IdleCloseInterval = 0, PreheatTime = 3, HeatingTime = 3, DetectionEnabled = false, WorkEnabled = false, DelayTime = 3 };
            BRailPreheatRight = new HeatingModuleInfo { Switch = true, ModuleName = "B轨预热-右", CurrentTemperature = 0, SetTemperature = 30, TemperatureRange = 10, IdleCloseInterval = 0, PreheatTime = 5, HeatingTime = 3, DetectionEnabled = false, WorkEnabled = false, DelayTime = 3 };
            BRailPreheatRight2 = new HeatingModuleInfo { Switch = true, ModuleName = "B轨预热-右(2)", CurrentTemperature = 0, SetTemperature = 30, TemperatureRange = 3, IdleCloseInterval = 0, PreheatTime = 3, HeatingTime = 3, DetectionEnabled = false, WorkEnabled = false, DelayTime = 3 };
        }

        private void textFont_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(TextFont));
        }

        private FontInfo _textFont = new FontInfo(FontInfo.DefaultFont.FontFamily, 16, FontInfo.DefaultFont.FontWeight, FontInfo.DefaultFont.FontStyle);
        [DisplayName("文字字体")]
        [Category("图元信息")]
        [PropertySortOrder(8)]
        [Browsable(false)]
        public FontInfo TextFont
        {
            get { return _textFont; }
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
        [Category("图元信息")]
        [PropertySortOrder(9)]
        [JsonConverter(typeof(ColorConvert))]
        [Browsable(false)]
        public Color TextColor
        {
            get { return _textColor; }
            set { SetProperty(ref _textColor, value); }
        }

        private Color _backColor = Colors.White;
        [DisplayName("背景颜色")]
        [Category("图元信息")]
        [PropertySortOrder(10)]
        [JsonConverter(typeof(ColorConvert))]
        [Browsable(false)]
        public Color BackColor
        {
            get { return _backColor; }
            set { SetProperty(ref _backColor, value); }
        }

        private bool _isDualValve;
        [DisplayName("双阀模式")]
        [Category("加热控制")]
        [PropertySortOrder(11)]
        [Browsable(false)]
        public bool IsDualValve
        {
            get { return _isDualValve; }
            set { SetProperty(ref _isDualValve, value); }
        }

        private bool _isDualTrack;
        [DisplayName("双轨模式")]
        [Category("加热控制")]
        [PropertySortOrder(12)]
        [Browsable(false)]
        public bool IsDualTrack
        {
            get { return _isDualTrack; }
            set { SetProperty(ref _isDualTrack, value); }
        }

        private HeatingModuleInfo _rightValveDispensingHead;
        private readonly PropertyChangedEventHandler _rightValveDispensingHead_PropertyChanged;
        [DisplayName("右阀点胶头")]
        [Category("加热控制")]
        [PropertySortOrder(13)]
        [Browsable(false)]
        public HeatingModuleInfo RightValveDispensingHead
        {
            get { return _rightValveDispensingHead; }
            set
            {
                if (_rightValveDispensingHead != null)
                    _rightValveDispensingHead.PropertyChanged -= _rightValveDispensingHead_PropertyChanged;
                if (SetProperty(ref _rightValveDispensingHead, value))
                {
                    if (_rightValveDispensingHead != null)
                    {
                        _rightValveDispensingHead.PropertyChanged += _rightValveDispensingHead_PropertyChanged;
                    }
                }
            }
        }

        private HeatingModuleInfo _rightValveCartridgeHeating;
        private readonly PropertyChangedEventHandler _rightValveCartridgeHeating_PropertyChanged;
        [DisplayName("右阀胶筒加热")]
        [Category("加热控制")]
        [PropertySortOrder(14)]
        [Browsable(false)]
        public HeatingModuleInfo RightValveCartridgeHeating
        {
            get { return _rightValveCartridgeHeating; }
            set
            {
                if (_rightValveCartridgeHeating != null)
                    _rightValveCartridgeHeating.PropertyChanged -= _rightValveCartridgeHeating_PropertyChanged;
                if (SetProperty(ref _rightValveCartridgeHeating, value))
                {
                    if (_rightValveCartridgeHeating != null)
                    {
                        _rightValveCartridgeHeating.PropertyChanged += _rightValveCartridgeHeating_PropertyChanged;
                    }
                }
            }
        }

        private HeatingModuleInfo _leftValveDispensingHead;
        private readonly PropertyChangedEventHandler _leftValveDispensingHead_PropertyChanged;
        [DisplayName("左阀点胶头")]
        [Category("加热控制")]
        [PropertySortOrder(15)]
        [Browsable(false)]
        public HeatingModuleInfo LeftValveDispensingHead
        {
            get { return _leftValveDispensingHead; }
            set
            {
                if (_leftValveDispensingHead != null)
                    _leftValveDispensingHead.PropertyChanged -= _leftValveDispensingHead_PropertyChanged;
                if (SetProperty(ref _leftValveDispensingHead, value))
                {
                    if (_leftValveDispensingHead != null)
                    {
                        _leftValveDispensingHead.PropertyChanged += _leftValveDispensingHead_PropertyChanged;
                    }
                }
            }
        }

        private HeatingModuleInfo _leftValveCartridgeHeating;
        private readonly PropertyChangedEventHandler _leftValveCartridgeHeating_PropertyChanged;
        [DisplayName("左阀胶筒加热")]
        [Category("加热控制")]
        [PropertySortOrder(16)]
        [Browsable(false)]
        public HeatingModuleInfo LeftValveCartridgeHeating
        {
            get { return _leftValveCartridgeHeating; }
            set
            {
                if (_leftValveCartridgeHeating != null)
                    _leftValveCartridgeHeating.PropertyChanged -= _leftValveCartridgeHeating_PropertyChanged;
                if (SetProperty(ref _leftValveCartridgeHeating, value))
                {
                    if (_leftValveCartridgeHeating != null)
                    {
                        _leftValveCartridgeHeating.PropertyChanged += _leftValveCartridgeHeating_PropertyChanged;
                    }
                }
            }
        }

        private HeatingModuleInfo _aRailPreheatLeft;
        private readonly PropertyChangedEventHandler _aRailPreheatLeft_PropertyChanged;
        [DisplayName("A轨预热-左")]
        [Category("加热控制")]
        [PropertySortOrder(17)]
        [Browsable(false)]
        public HeatingModuleInfo ARailPreheatLeft
        {
            get { return _aRailPreheatLeft; }
            set
            {
                if (_aRailPreheatLeft != null)
                    _aRailPreheatLeft.PropertyChanged -= _aRailPreheatLeft_PropertyChanged;
                if (SetProperty(ref _aRailPreheatLeft, value))
                {
                    if (_aRailPreheatLeft != null)
                    {
                        _aRailPreheatLeft.PropertyChanged += _aRailPreheatLeft_PropertyChanged;
                    }
                }
            }
        }

        private HeatingModuleInfo _aRailPreheatLeft2;
        private readonly PropertyChangedEventHandler _aRailPreheatLeft2_PropertyChanged;
        [DisplayName("A轨预热-左(2)")]
        [Category("加热控制")]
        [PropertySortOrder(18)]
        [Browsable(false)]
        public HeatingModuleInfo ARailPreheatLeft2
        {
            get { return _aRailPreheatLeft2; }
            set
            {
                if (_aRailPreheatLeft2 != null)
                    _aRailPreheatLeft2.PropertyChanged -= _aRailPreheatLeft2_PropertyChanged;
                if (SetProperty(ref _aRailPreheatLeft2, value))
                {
                    if (_aRailPreheatLeft2 != null)
                    {
                        _aRailPreheatLeft2.PropertyChanged += _aRailPreheatLeft2_PropertyChanged;
                    }
                }
            }
        }

        private HeatingModuleInfo _aRailGlueBoardStationMiddle;
        private readonly PropertyChangedEventHandler _aRailGlueBoardStationMiddle_PropertyChanged;
        [DisplayName("A轨胶板工位-中")]
        [Category("加热控制")]
        [PropertySortOrder(19)]
        [Browsable(false)]
        public HeatingModuleInfo ARailGlueBoardStationMiddle
        {
            get { return _aRailGlueBoardStationMiddle; }
            set
            {
                if (_aRailGlueBoardStationMiddle != null)
                    _aRailGlueBoardStationMiddle.PropertyChanged -= _aRailGlueBoardStationMiddle_PropertyChanged;
                if (SetProperty(ref _aRailGlueBoardStationMiddle, value))
                {
                    if (_aRailGlueBoardStationMiddle != null)
                    {
                        _aRailGlueBoardStationMiddle.PropertyChanged += _aRailGlueBoardStationMiddle_PropertyChanged;
                    }
                }
            }
        }

        private HeatingModuleInfo _aRailDispensingStationMiddle2;
        private readonly PropertyChangedEventHandler _aRailDispensingStationMiddle2_PropertyChanged;
        [DisplayName("A轨点胶工位-中(2)")]
        [Category("加热控制")]
        [PropertySortOrder(20)]
        [Browsable(false)]
        public HeatingModuleInfo ARailDispensingStationMiddle2
        {
            get { return _aRailDispensingStationMiddle2; }
            set
            {
                if (_aRailDispensingStationMiddle2 != null)
                    _aRailDispensingStationMiddle2.PropertyChanged -= _aRailDispensingStationMiddle2_PropertyChanged;
                if (SetProperty(ref _aRailDispensingStationMiddle2, value))
                {
                    if (_aRailDispensingStationMiddle2 != null)
                    {
                        _aRailDispensingStationMiddle2.PropertyChanged += _aRailDispensingStationMiddle2_PropertyChanged;
                    }
                }
            }
        }

        private HeatingModuleInfo _aRailPreheatRight;
        private readonly PropertyChangedEventHandler _aRailPreheatRight_PropertyChanged;
        [DisplayName("A轨预热-右")]
        [Category("加热控制")]
        [PropertySortOrder(21)]
        [Browsable(false)]
        public HeatingModuleInfo ARailPreheatRight
        {
            get { return _aRailPreheatRight; }
            set
            {
                if (_aRailPreheatRight != null)
                    _aRailPreheatRight.PropertyChanged -= _aRailPreheatRight_PropertyChanged;
                if (SetProperty(ref _aRailPreheatRight, value))
                {
                    if (_aRailPreheatRight != null)
                    {
                        _aRailPreheatRight.PropertyChanged += _aRailPreheatRight_PropertyChanged;
                    }
                }
            }
        }

        private HeatingModuleInfo _aRailPreheatRight2;
        private readonly PropertyChangedEventHandler _aRailPreheatRight2_PropertyChanged;
        [DisplayName("A轨预热-右(2)")]
        [Category("加热控制")]
        [PropertySortOrder(22)]
        [Browsable(false)]
        public HeatingModuleInfo ARailPreheatRight2
        {
            get { return _aRailPreheatRight2; }
            set
            {
                if (_aRailPreheatRight2 != null)
                    _aRailPreheatRight2.PropertyChanged -= _aRailPreheatRight2_PropertyChanged;
                if (SetProperty(ref _aRailPreheatRight2, value))
                {
                    if (_aRailPreheatRight2 != null)
                    {
                        _aRailPreheatRight2.PropertyChanged += _aRailPreheatRight2_PropertyChanged;
                    }
                }
            }
        }

        private HeatingModuleInfo _bRailPreheatLeft;
        private readonly PropertyChangedEventHandler _bRailPreheatLeft_PropertyChanged;
        [DisplayName("B轨预热-左")]
        [Category("加热控制")]
        [PropertySortOrder(23)]
        [Browsable(false)]
        public HeatingModuleInfo BRailPreheatLeft
        {
            get { return _bRailPreheatLeft; }
            set
            {
                if (_bRailPreheatLeft != null)
                    _bRailPreheatLeft.PropertyChanged -= _bRailPreheatLeft_PropertyChanged;
                if (SetProperty(ref _bRailPreheatLeft, value))
                {
                    if (_bRailPreheatLeft != null)
                    {
                        _bRailPreheatLeft.PropertyChanged += _bRailPreheatLeft_PropertyChanged;
                    }
                }
            }
        }

        private HeatingModuleInfo _bRailPreheatLeft2;
        private readonly PropertyChangedEventHandler _bRailPreheatLeft2_PropertyChanged;
        [DisplayName("B轨预热-左(2)")]
        [Category("加热控制")]
        [PropertySortOrder(24)]
        [Browsable(false)]
        public HeatingModuleInfo BRailPreheatLeft2
        {
            get { return _bRailPreheatLeft2; }
            set
            {
                if (_bRailPreheatLeft2 != null)
                    _bRailPreheatLeft2.PropertyChanged -= _bRailPreheatLeft2_PropertyChanged;
                if (SetProperty(ref _bRailPreheatLeft2, value))
                {
                    if (_bRailPreheatLeft2 != null)
                    {
                        _bRailPreheatLeft2.PropertyChanged += _bRailPreheatLeft2_PropertyChanged;
                    }
                }
            }
        }

        private HeatingModuleInfo _bRailGlueBoardStationMiddle;
        private readonly PropertyChangedEventHandler _bRailGlueBoardStationMiddle_PropertyChanged;
        [DisplayName("B轨胶板工位-中")]
        [Category("加热控制")]
        [PropertySortOrder(25)]
        [Browsable(false)]
        public HeatingModuleInfo BRailGlueBoardStationMiddle
        {
            get { return _bRailGlueBoardStationMiddle; }
            set
            {
                if (_bRailGlueBoardStationMiddle != null)
                    _bRailGlueBoardStationMiddle.PropertyChanged -= _bRailGlueBoardStationMiddle_PropertyChanged;
                if (SetProperty(ref _bRailGlueBoardStationMiddle, value))
                {
                    if (_bRailGlueBoardStationMiddle != null)
                    {
                        _bRailGlueBoardStationMiddle.PropertyChanged += _bRailGlueBoardStationMiddle_PropertyChanged;
                    }
                }
            }
        }

        private HeatingModuleInfo _bRailDispensingStationMiddle2;
        private readonly PropertyChangedEventHandler _bRailDispensingStationMiddle2_PropertyChanged;
        [DisplayName("B轨点胶工位-中(2)")]
        [Category("加热控制")]
        [PropertySortOrder(26)]
        [Browsable(false)]
        public HeatingModuleInfo BRailDispensingStationMiddle2
        {
            get { return _bRailDispensingStationMiddle2; }
            set
            {
                if (_bRailDispensingStationMiddle2 != null)
                    _bRailDispensingStationMiddle2.PropertyChanged -= _bRailDispensingStationMiddle2_PropertyChanged;
                if (SetProperty(ref _bRailDispensingStationMiddle2, value))
                {
                    if (_bRailDispensingStationMiddle2 != null)
                    {
                        _bRailDispensingStationMiddle2.PropertyChanged += _bRailDispensingStationMiddle2_PropertyChanged;
                    }
                }
            }
        }

        private HeatingModuleInfo _bRailPreheatRight;
        private readonly PropertyChangedEventHandler _bRailPreheatRight_PropertyChanged;
        [DisplayName("B轨预热-右")]
        [Category("加热控制")]
        [PropertySortOrder(27)]
        [Browsable(false)]
        public HeatingModuleInfo BRailPreheatRight
        {
            get { return _bRailPreheatRight; }
            set
            {
                if (_bRailPreheatRight != null)
                    _bRailPreheatRight.PropertyChanged -= _bRailPreheatRight_PropertyChanged;
                if (SetProperty(ref _bRailPreheatRight, value))
                {
                    if (_bRailPreheatRight != null)
                    {
                        _bRailPreheatRight.PropertyChanged += _bRailPreheatRight_PropertyChanged;
                    }
                }
            }
        }

        private HeatingModuleInfo _bRailPreheatRight2;
        private readonly PropertyChangedEventHandler _bRailPreheatRight2_PropertyChanged;
        [DisplayName("B轨预热-右(2)")]
        [Category("加热控制")]
        [PropertySortOrder(28)]
        [Browsable(false)]
        public HeatingModuleInfo BRailPreheatRight2
        {
            get { return _bRailPreheatRight2; }
            set
            {
                if (_bRailPreheatRight2 != null)
                    _bRailPreheatRight2.PropertyChanged -= _bRailPreheatRight2_PropertyChanged;
                if (SetProperty(ref _bRailPreheatRight2, value))
                {
                    if (_bRailPreheatRight2 != null)
                    {
                        _bRailPreheatRight2.PropertyChanged += _bRailPreheatRight2_PropertyChanged;
                    }
                }
            }
        }

        [Browsable(false)]
        public List<HeatingModuleInfo> HeatingModules
        {
            get
            {
                var modules = new List<HeatingModuleInfo>
                {
                    RightValveDispensingHead,
                    RightValveCartridgeHeating,
                };

                if (IsDualValve)
                {
                    modules.Add(LeftValveDispensingHead);
                    modules.Add(LeftValveCartridgeHeating);
                }

                modules.Add(ARailPreheatLeft);
                modules.Add(ARailPreheatLeft2);
                modules.Add(ARailGlueBoardStationMiddle);
                modules.Add(ARailDispensingStationMiddle2);
                modules.Add(ARailPreheatRight);
                modules.Add(ARailPreheatRight2);

                if (IsDualTrack)
                {
                    modules.Add(BRailPreheatLeft);
                    modules.Add(BRailPreheatLeft2);
                    modules.Add(BRailGlueBoardStationMiddle);
                    modules.Add(BRailDispensingStationMiddle2);
                    modules.Add(BRailPreheatRight);
                    modules.Add(BRailPreheatRight2);
                }

                return modules;
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

            if (string.Compare(propertyID, nameof(IsDualValve)) == 0)
            {
                if (propertyVal is { })
                {
                    IsDualValve = propertyVal.ToPrimitiveValue<bool>();
                }
                else
                {
                    IsDualValve = false;
                }
                return true;
            }

            if (string.Compare(propertyID, nameof(IsDualTrack)) == 0)
            {
                if (propertyVal is { })
                {
                    IsDualTrack = propertyVal.ToPrimitiveValue<bool>();
                }
                else
                {
                    IsDualTrack = false;
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

            if (string.Compare(propertyID, nameof(RightValveDispensingHead)) == 0)
            {
                if (propertyVal is { })
                {
                    ObjectValue_Json objectValue_Json = propertyVal.ToObjectValue_Json();
                    GriffinsBaseValue griffinsBaseValue = GriffinsBaseValue.Create(objectValue_Json);
                    IGriffinsBaseValue iMPPropObjectValue = new HeatingModuleInfo();
                    iMPPropObjectValue.PopulateFromBaseValue(griffinsBaseValue);
                    RightValveDispensingHead = (HeatingModuleInfo)iMPPropObjectValue;
                }
                else
                {
                    RightValveDispensingHead = new HeatingModuleInfo();
                }
                return true;
            }

            if (string.Compare(propertyID, nameof(RightValveCartridgeHeating)) == 0)
            {
                if (propertyVal is { })
                {
                    ObjectValue_Json objectValue_Json = propertyVal.ToObjectValue_Json();
                    GriffinsBaseValue griffinsBaseValue = GriffinsBaseValue.Create(objectValue_Json);
                    IGriffinsBaseValue iMPPropObjectValue = new HeatingModuleInfo();
                    iMPPropObjectValue.PopulateFromBaseValue(griffinsBaseValue);
                    RightValveCartridgeHeating = (HeatingModuleInfo)iMPPropObjectValue;
                }
                else
                {
                    RightValveCartridgeHeating = new HeatingModuleInfo();
                }
                return true;
            }

            if (string.Compare(propertyID, nameof(LeftValveDispensingHead)) == 0)
            {
                if (propertyVal is { })
                {
                    ObjectValue_Json objectValue_Json = propertyVal.ToObjectValue_Json();
                    GriffinsBaseValue griffinsBaseValue = GriffinsBaseValue.Create(objectValue_Json);
                    IGriffinsBaseValue iMPPropObjectValue = new HeatingModuleInfo();
                    iMPPropObjectValue.PopulateFromBaseValue(griffinsBaseValue);
                    LeftValveDispensingHead = (HeatingModuleInfo)iMPPropObjectValue;
                }
                else
                {
                    LeftValveDispensingHead = new HeatingModuleInfo();
                }
                return true;
            }

            if (string.Compare(propertyID, nameof(LeftValveCartridgeHeating)) == 0)
            {
                if (propertyVal is { })
                {
                    ObjectValue_Json objectValue_Json = propertyVal.ToObjectValue_Json();
                    GriffinsBaseValue griffinsBaseValue = GriffinsBaseValue.Create(objectValue_Json);
                    IGriffinsBaseValue iMPPropObjectValue = new HeatingModuleInfo();
                    iMPPropObjectValue.PopulateFromBaseValue(griffinsBaseValue);
                    LeftValveCartridgeHeating = (HeatingModuleInfo)iMPPropObjectValue;
                }
                else
                {
                    LeftValveCartridgeHeating = new HeatingModuleInfo();
                }
                return true;
            }

            if (string.Compare(propertyID, nameof(ARailPreheatLeft)) == 0)
            {
                if (propertyVal is { })
                {
                    ObjectValue_Json objectValue_Json = propertyVal.ToObjectValue_Json();
                    GriffinsBaseValue griffinsBaseValue = GriffinsBaseValue.Create(objectValue_Json);
                    IGriffinsBaseValue iMPPropObjectValue = new HeatingModuleInfo();
                    iMPPropObjectValue.PopulateFromBaseValue(griffinsBaseValue);
                    ARailPreheatLeft = (HeatingModuleInfo)iMPPropObjectValue;
                }
                else
                {
                    ARailPreheatLeft = new HeatingModuleInfo();
                }
                return true;
            }

            if (string.Compare(propertyID, nameof(ARailPreheatLeft2)) == 0)
            {
                if (propertyVal is { })
                {
                    ObjectValue_Json objectValue_Json = propertyVal.ToObjectValue_Json();
                    GriffinsBaseValue griffinsBaseValue = GriffinsBaseValue.Create(objectValue_Json);
                    IGriffinsBaseValue iMPPropObjectValue = new HeatingModuleInfo();
                    iMPPropObjectValue.PopulateFromBaseValue(griffinsBaseValue);
                    ARailPreheatLeft2 = (HeatingModuleInfo)iMPPropObjectValue;
                }
                else
                {
                    ARailPreheatLeft2 = new HeatingModuleInfo();
                }
                return true;
            }

            if (string.Compare(propertyID, nameof(ARailGlueBoardStationMiddle)) == 0)
            {
                if (propertyVal is { })
                {
                    ObjectValue_Json objectValue_Json = propertyVal.ToObjectValue_Json();
                    GriffinsBaseValue griffinsBaseValue = GriffinsBaseValue.Create(objectValue_Json);
                    IGriffinsBaseValue iMPPropObjectValue = new HeatingModuleInfo();
                    iMPPropObjectValue.PopulateFromBaseValue(griffinsBaseValue);
                    ARailGlueBoardStationMiddle = (HeatingModuleInfo)iMPPropObjectValue;
                }
                else
                {
                    ARailGlueBoardStationMiddle = new HeatingModuleInfo();
                }
                return true;
            }

            if (string.Compare(propertyID, nameof(ARailDispensingStationMiddle2)) == 0)
            {
                if (propertyVal is { })
                {
                    ObjectValue_Json objectValue_Json = propertyVal.ToObjectValue_Json();
                    GriffinsBaseValue griffinsBaseValue = GriffinsBaseValue.Create(objectValue_Json);
                    IGriffinsBaseValue iMPPropObjectValue = new HeatingModuleInfo();
                    iMPPropObjectValue.PopulateFromBaseValue(griffinsBaseValue);
                    ARailDispensingStationMiddle2 = (HeatingModuleInfo)iMPPropObjectValue;
                }
                else
                {
                    ARailDispensingStationMiddle2 = new HeatingModuleInfo();
                }
                return true;
            }

            if (string.Compare(propertyID, nameof(ARailPreheatRight)) == 0)
            {
                if (propertyVal is { })
                {
                    ObjectValue_Json objectValue_Json = propertyVal.ToObjectValue_Json();
                    GriffinsBaseValue griffinsBaseValue = GriffinsBaseValue.Create(objectValue_Json);
                    IGriffinsBaseValue iMPPropObjectValue = new HeatingModuleInfo();
                    iMPPropObjectValue.PopulateFromBaseValue(griffinsBaseValue);
                    ARailPreheatRight = (HeatingModuleInfo)iMPPropObjectValue;
                }
                else
                {
                    ARailPreheatRight = new HeatingModuleInfo();
                }
                return true;
            }

            if (string.Compare(propertyID, nameof(ARailPreheatRight2)) == 0)
            {
                if (propertyVal is { })
                {
                    ObjectValue_Json objectValue_Json = propertyVal.ToObjectValue_Json();
                    GriffinsBaseValue griffinsBaseValue = GriffinsBaseValue.Create(objectValue_Json);
                    IGriffinsBaseValue iMPPropObjectValue = new HeatingModuleInfo();
                    iMPPropObjectValue.PopulateFromBaseValue(griffinsBaseValue);
                    ARailPreheatRight2 = (HeatingModuleInfo)iMPPropObjectValue;
                }
                else
                {
                    ARailPreheatRight2 = new HeatingModuleInfo();
                }
                return true;
            }

            if (string.Compare(propertyID, nameof(BRailPreheatLeft)) == 0)
            {
                if (propertyVal is { })
                {
                    ObjectValue_Json objectValue_Json = propertyVal.ToObjectValue_Json();
                    GriffinsBaseValue griffinsBaseValue = GriffinsBaseValue.Create(objectValue_Json);
                    IGriffinsBaseValue iMPPropObjectValue = new HeatingModuleInfo();
                    iMPPropObjectValue.PopulateFromBaseValue(griffinsBaseValue);
                    BRailPreheatLeft = (HeatingModuleInfo)iMPPropObjectValue;
                }
                else
                {
                    BRailPreheatLeft = new HeatingModuleInfo();
                }
                return true;
            }

            if (string.Compare(propertyID, nameof(BRailPreheatLeft2)) == 0)
            {
                if (propertyVal is { })
                {
                    ObjectValue_Json objectValue_Json = propertyVal.ToObjectValue_Json();
                    GriffinsBaseValue griffinsBaseValue = GriffinsBaseValue.Create(objectValue_Json);
                    IGriffinsBaseValue iMPPropObjectValue = new HeatingModuleInfo();
                    iMPPropObjectValue.PopulateFromBaseValue(griffinsBaseValue);
                    BRailPreheatLeft2 = (HeatingModuleInfo)iMPPropObjectValue;
                }
                else
                {
                    BRailPreheatLeft2 = new HeatingModuleInfo();
                }
                return true;
            }

            if (string.Compare(propertyID, nameof(BRailGlueBoardStationMiddle)) == 0)
            {
                if (propertyVal is { })
                {
                    ObjectValue_Json objectValue_Json = propertyVal.ToObjectValue_Json();
                    GriffinsBaseValue griffinsBaseValue = GriffinsBaseValue.Create(objectValue_Json);
                    IGriffinsBaseValue iMPPropObjectValue = new HeatingModuleInfo();
                    iMPPropObjectValue.PopulateFromBaseValue(griffinsBaseValue);
                    BRailGlueBoardStationMiddle = (HeatingModuleInfo)iMPPropObjectValue;
                }
                else
                {
                    BRailGlueBoardStationMiddle = new HeatingModuleInfo();
                }
                return true;
            }

            if (string.Compare(propertyID, nameof(BRailDispensingStationMiddle2)) == 0)
            {
                if (propertyVal is { })
                {
                    ObjectValue_Json objectValue_Json = propertyVal.ToObjectValue_Json();
                    GriffinsBaseValue griffinsBaseValue = GriffinsBaseValue.Create(objectValue_Json);
                    IGriffinsBaseValue iMPPropObjectValue = new HeatingModuleInfo();
                    iMPPropObjectValue.PopulateFromBaseValue(griffinsBaseValue);
                    BRailDispensingStationMiddle2 = (HeatingModuleInfo)iMPPropObjectValue;
                }
                else
                {
                    BRailDispensingStationMiddle2 = new HeatingModuleInfo();
                }
                return true;
            }

            if (string.Compare(propertyID, nameof(BRailPreheatRight)) == 0)
            {
                if (propertyVal is { })
                {
                    ObjectValue_Json objectValue_Json = propertyVal.ToObjectValue_Json();
                    GriffinsBaseValue griffinsBaseValue = GriffinsBaseValue.Create(objectValue_Json);
                    IGriffinsBaseValue iMPPropObjectValue = new HeatingModuleInfo();
                    iMPPropObjectValue.PopulateFromBaseValue(griffinsBaseValue);
                    BRailPreheatRight = (HeatingModuleInfo)iMPPropObjectValue;
                }
                else
                {
                    BRailPreheatRight = new HeatingModuleInfo();
                }
                return true;
            }

            if (string.Compare(propertyID, nameof(BRailPreheatRight2)) == 0)
            {
                if (propertyVal is { })
                {
                    ObjectValue_Json objectValue_Json = propertyVal.ToObjectValue_Json();
                    GriffinsBaseValue griffinsBaseValue = GriffinsBaseValue.Create(objectValue_Json);
                    IGriffinsBaseValue iMPPropObjectValue = new HeatingModuleInfo();
                    iMPPropObjectValue.PopulateFromBaseValue(griffinsBaseValue);
                    BRailPreheatRight2 = (HeatingModuleInfo)iMPPropObjectValue;
                }
                else
                {
                    BRailPreheatRight2 = new HeatingModuleInfo();
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
        public void CopyFrom(HeatingControlPropertyModelEdit source)
        {
            base.CopyFrom(source);

            if (source.TextFont != null)
            {
                this.TextFont = new FontInfo(source.TextFont.FontFamily, source.TextFont.FontSize, source.TextFont.FontWeight, source.TextFont.FontStyle);
            }
            this.TextColor = source.TextColor;
            this.BackColor = source.BackColor;
            this.IsDualValve = source.IsDualValve;
            this.IsDualTrack = source.IsDualTrack;

            // 复制所有加热模块属性
            if (source.RightValveDispensingHead != null)
            {
                this.RightValveDispensingHead = new HeatingModuleInfo
                {
                    Switch = source.RightValveDispensingHead.Switch,
                    ModuleName = source.RightValveDispensingHead.ModuleName,
                    CurrentTemperature = source.RightValveDispensingHead.CurrentTemperature,
                    SetTemperature = source.RightValveDispensingHead.SetTemperature,
                    TemperatureRange = source.RightValveDispensingHead.TemperatureRange,
                    IdleCloseInterval = source.RightValveDispensingHead.IdleCloseInterval,
                    PreheatTime = source.RightValveDispensingHead.PreheatTime,
                    HeatingTime = source.RightValveDispensingHead.HeatingTime,
                    DetectionEnabled = source.RightValveDispensingHead.DetectionEnabled,
                    WorkEnabled = source.RightValveDispensingHead.WorkEnabled,
                    DelayTime = source.RightValveDispensingHead.DelayTime
                };
            }

            if (source.RightValveCartridgeHeating != null)
            {
                this.RightValveCartridgeHeating = new HeatingModuleInfo
                {
                    Switch = source.RightValveCartridgeHeating.Switch,
                    ModuleName = source.RightValveCartridgeHeating.ModuleName,
                    CurrentTemperature = source.RightValveCartridgeHeating.CurrentTemperature,
                    SetTemperature = source.RightValveCartridgeHeating.SetTemperature,
                    TemperatureRange = source.RightValveCartridgeHeating.TemperatureRange,
                    IdleCloseInterval = source.RightValveCartridgeHeating.IdleCloseInterval,
                    PreheatTime = source.RightValveCartridgeHeating.PreheatTime,
                    HeatingTime = source.RightValveCartridgeHeating.HeatingTime,
                    DetectionEnabled = source.RightValveCartridgeHeating.DetectionEnabled,
                    WorkEnabled = source.RightValveCartridgeHeating.WorkEnabled,
                    DelayTime = source.RightValveCartridgeHeating.DelayTime
                };
            }

            if (source.LeftValveDispensingHead != null)
            {
                this.LeftValveDispensingHead = new HeatingModuleInfo
                {
                    Switch = source.LeftValveDispensingHead.Switch,
                    ModuleName = source.LeftValveDispensingHead.ModuleName,
                    CurrentTemperature = source.LeftValveDispensingHead.CurrentTemperature,
                    SetTemperature = source.LeftValveDispensingHead.SetTemperature,
                    TemperatureRange = source.LeftValveDispensingHead.TemperatureRange,
                    IdleCloseInterval = source.LeftValveDispensingHead.IdleCloseInterval,
                    PreheatTime = source.LeftValveDispensingHead.PreheatTime,
                    HeatingTime = source.LeftValveDispensingHead.HeatingTime,
                    DetectionEnabled = source.LeftValveDispensingHead.DetectionEnabled,
                    WorkEnabled = source.LeftValveDispensingHead.WorkEnabled,
                    DelayTime = source.LeftValveDispensingHead.DelayTime
                };
            }

            if (source.LeftValveCartridgeHeating != null)
            {
                this.LeftValveCartridgeHeating = new HeatingModuleInfo
                {
                    Switch = source.LeftValveCartridgeHeating.Switch,
                    ModuleName = source.LeftValveCartridgeHeating.ModuleName,
                    CurrentTemperature = source.LeftValveCartridgeHeating.CurrentTemperature,
                    SetTemperature = source.LeftValveCartridgeHeating.SetTemperature,
                    TemperatureRange = source.LeftValveCartridgeHeating.TemperatureRange,
                    IdleCloseInterval = source.LeftValveCartridgeHeating.IdleCloseInterval,
                    PreheatTime = source.LeftValveCartridgeHeating.PreheatTime,
                    HeatingTime = source.LeftValveCartridgeHeating.HeatingTime,
                    DetectionEnabled = source.LeftValveCartridgeHeating.DetectionEnabled,
                    WorkEnabled = source.LeftValveCartridgeHeating.WorkEnabled,
                    DelayTime = source.LeftValveCartridgeHeating.DelayTime
                };
            }

            if (source.ARailPreheatLeft != null)
            {
                this.ARailPreheatLeft = new HeatingModuleInfo
                {
                    Switch = source.ARailPreheatLeft.Switch,
                    ModuleName = source.ARailPreheatLeft.ModuleName,
                    CurrentTemperature = source.ARailPreheatLeft.CurrentTemperature,
                    SetTemperature = source.ARailPreheatLeft.SetTemperature,
                    TemperatureRange = source.ARailPreheatLeft.TemperatureRange,
                    IdleCloseInterval = source.ARailPreheatLeft.IdleCloseInterval,
                    PreheatTime = source.ARailPreheatLeft.PreheatTime,
                    HeatingTime = source.ARailPreheatLeft.HeatingTime,
                    DetectionEnabled = source.ARailPreheatLeft.DetectionEnabled,
                    WorkEnabled = source.ARailPreheatLeft.WorkEnabled,
                    DelayTime = source.ARailPreheatLeft.DelayTime
                };
            }

            if (source.ARailPreheatLeft2 != null)
            {
                this.ARailPreheatLeft2 = new HeatingModuleInfo
                {
                    Switch = source.ARailPreheatLeft2.Switch,
                    ModuleName = source.ARailPreheatLeft2.ModuleName,
                    CurrentTemperature = source.ARailPreheatLeft2.CurrentTemperature,
                    SetTemperature = source.ARailPreheatLeft2.SetTemperature,
                    TemperatureRange = source.ARailPreheatLeft2.TemperatureRange,
                    IdleCloseInterval = source.ARailPreheatLeft2.IdleCloseInterval,
                    PreheatTime = source.ARailPreheatLeft2.PreheatTime,
                    HeatingTime = source.ARailPreheatLeft2.HeatingTime,
                    DetectionEnabled = source.ARailPreheatLeft2.DetectionEnabled,
                    WorkEnabled = source.ARailPreheatLeft2.WorkEnabled,
                    DelayTime = source.ARailPreheatLeft2.DelayTime
                };
            }

            if (source.ARailGlueBoardStationMiddle != null)
            {
                this.ARailGlueBoardStationMiddle = new HeatingModuleInfo
                {
                    Switch = source.ARailGlueBoardStationMiddle.Switch,
                    ModuleName = source.ARailGlueBoardStationMiddle.ModuleName,
                    CurrentTemperature = source.ARailGlueBoardStationMiddle.CurrentTemperature,
                    SetTemperature = source.ARailGlueBoardStationMiddle.SetTemperature,
                    TemperatureRange = source.ARailGlueBoardStationMiddle.TemperatureRange,
                    IdleCloseInterval = source.ARailGlueBoardStationMiddle.IdleCloseInterval,
                    PreheatTime = source.ARailGlueBoardStationMiddle.PreheatTime,
                    HeatingTime = source.ARailGlueBoardStationMiddle.HeatingTime,
                    DetectionEnabled = source.ARailGlueBoardStationMiddle.DetectionEnabled,
                    WorkEnabled = source.ARailGlueBoardStationMiddle.WorkEnabled,
                    DelayTime = source.ARailGlueBoardStationMiddle.DelayTime
                };
            }

            if (source.ARailDispensingStationMiddle2 != null)
            {
                this.ARailDispensingStationMiddle2 = new HeatingModuleInfo
                {
                    Switch = source.ARailDispensingStationMiddle2.Switch,
                    ModuleName = source.ARailDispensingStationMiddle2.ModuleName,
                    CurrentTemperature = source.ARailDispensingStationMiddle2.CurrentTemperature,
                    SetTemperature = source.ARailDispensingStationMiddle2.SetTemperature,
                    TemperatureRange = source.ARailDispensingStationMiddle2.TemperatureRange,
                    IdleCloseInterval = source.ARailDispensingStationMiddle2.IdleCloseInterval,
                    PreheatTime = source.ARailDispensingStationMiddle2.PreheatTime,
                    HeatingTime = source.ARailDispensingStationMiddle2.HeatingTime,
                    DetectionEnabled = source.ARailDispensingStationMiddle2.DetectionEnabled,
                    WorkEnabled = source.ARailDispensingStationMiddle2.WorkEnabled,
                    DelayTime = source.ARailDispensingStationMiddle2.DelayTime
                };
            }

            if (source.ARailPreheatRight != null)
            {
                this.ARailPreheatRight = new HeatingModuleInfo
                {
                    Switch = source.ARailPreheatRight.Switch,
                    ModuleName = source.ARailPreheatRight.ModuleName,
                    CurrentTemperature = source.ARailPreheatRight.CurrentTemperature,
                    SetTemperature = source.ARailPreheatRight.SetTemperature,
                    TemperatureRange = source.ARailPreheatRight.TemperatureRange,
                    IdleCloseInterval = source.ARailPreheatRight.IdleCloseInterval,
                    PreheatTime = source.ARailPreheatRight.PreheatTime,
                    HeatingTime = source.ARailPreheatRight.HeatingTime,
                    DetectionEnabled = source.ARailPreheatRight.DetectionEnabled,
                    WorkEnabled = source.ARailPreheatRight.WorkEnabled,
                    DelayTime = source.ARailPreheatRight.DelayTime
                };
            }

            if (source.ARailPreheatRight2 != null)
            {
                this.ARailPreheatRight2 = new HeatingModuleInfo
                {
                    Switch = source.ARailPreheatRight2.Switch,
                    ModuleName = source.ARailPreheatRight2.ModuleName,
                    CurrentTemperature = source.ARailPreheatRight2.CurrentTemperature,
                    SetTemperature = source.ARailPreheatRight2.SetTemperature,
                    TemperatureRange = source.ARailPreheatRight2.TemperatureRange,
                    IdleCloseInterval = source.ARailPreheatRight2.IdleCloseInterval,
                    PreheatTime = source.ARailPreheatRight2.PreheatTime,
                    HeatingTime = source.ARailPreheatRight2.HeatingTime,
                    DetectionEnabled = source.ARailPreheatRight2.DetectionEnabled,
                    WorkEnabled = source.ARailPreheatRight2.WorkEnabled,
                    DelayTime = source.ARailPreheatRight2.DelayTime
                };
            }

            if (source.BRailPreheatLeft != null)
            {
                this.BRailPreheatLeft = new HeatingModuleInfo
                {
                    Switch = source.BRailPreheatLeft.Switch,
                    ModuleName = source.BRailPreheatLeft.ModuleName,
                    CurrentTemperature = source.BRailPreheatLeft.CurrentTemperature,
                    SetTemperature = source.BRailPreheatLeft.SetTemperature,
                    TemperatureRange = source.BRailPreheatLeft.TemperatureRange,
                    IdleCloseInterval = source.BRailPreheatLeft.IdleCloseInterval,
                    PreheatTime = source.BRailPreheatLeft.PreheatTime,
                    HeatingTime = source.BRailPreheatLeft.HeatingTime,
                    DetectionEnabled = source.BRailPreheatLeft.DetectionEnabled,
                    WorkEnabled = source.BRailPreheatLeft.WorkEnabled,
                    DelayTime = source.BRailPreheatLeft.DelayTime
                };
            }

            if (source.BRailPreheatLeft2 != null)
            {
                this.BRailPreheatLeft2 = new HeatingModuleInfo
                {
                    Switch = source.BRailPreheatLeft2.Switch,
                    ModuleName = source.BRailPreheatLeft2.ModuleName,
                    CurrentTemperature = source.BRailPreheatLeft2.CurrentTemperature,
                    SetTemperature = source.BRailPreheatLeft2.SetTemperature,
                    TemperatureRange = source.BRailPreheatLeft2.TemperatureRange,
                    IdleCloseInterval = source.BRailPreheatLeft2.IdleCloseInterval,
                    PreheatTime = source.BRailPreheatLeft2.PreheatTime,
                    HeatingTime = source.BRailPreheatLeft2.HeatingTime,
                    DetectionEnabled = source.BRailPreheatLeft2.DetectionEnabled,
                    WorkEnabled = source.BRailPreheatLeft2.WorkEnabled,
                    DelayTime = source.BRailPreheatLeft2.DelayTime
                };
            }

            if (source.BRailGlueBoardStationMiddle != null)
            {
                this.BRailGlueBoardStationMiddle = new HeatingModuleInfo
                {
                    Switch = source.BRailGlueBoardStationMiddle.Switch,
                    ModuleName = source.BRailGlueBoardStationMiddle.ModuleName,
                    CurrentTemperature = source.BRailGlueBoardStationMiddle.CurrentTemperature,
                    SetTemperature = source.BRailGlueBoardStationMiddle.SetTemperature,
                    TemperatureRange = source.BRailGlueBoardStationMiddle.TemperatureRange,
                    IdleCloseInterval = source.BRailGlueBoardStationMiddle.IdleCloseInterval,
                    PreheatTime = source.BRailGlueBoardStationMiddle.PreheatTime,
                    HeatingTime = source.BRailGlueBoardStationMiddle.HeatingTime,
                    DetectionEnabled = source.BRailGlueBoardStationMiddle.DetectionEnabled,
                    WorkEnabled = source.BRailGlueBoardStationMiddle.WorkEnabled,
                    DelayTime = source.BRailGlueBoardStationMiddle.DelayTime
                };
            }

            if (source.BRailDispensingStationMiddle2 != null)
            {
                this.BRailDispensingStationMiddle2 = new HeatingModuleInfo
                {
                    Switch = source.BRailDispensingStationMiddle2.Switch,
                    ModuleName = source.BRailDispensingStationMiddle2.ModuleName,
                    CurrentTemperature = source.BRailDispensingStationMiddle2.CurrentTemperature,
                    SetTemperature = source.BRailDispensingStationMiddle2.SetTemperature,
                    TemperatureRange = source.BRailDispensingStationMiddle2.TemperatureRange,
                    IdleCloseInterval = source.BRailDispensingStationMiddle2.IdleCloseInterval,
                    PreheatTime = source.BRailDispensingStationMiddle2.PreheatTime,
                    HeatingTime = source.BRailDispensingStationMiddle2.HeatingTime,
                    DetectionEnabled = source.BRailDispensingStationMiddle2.DetectionEnabled,
                    WorkEnabled = source.BRailDispensingStationMiddle2.WorkEnabled,
                    DelayTime = source.BRailDispensingStationMiddle2.DelayTime
                };
            }

            if (source.BRailPreheatRight != null)
            {
                this.BRailPreheatRight = new HeatingModuleInfo
                {
                    Switch = source.BRailPreheatRight.Switch,
                    ModuleName = source.BRailPreheatRight.ModuleName,
                    CurrentTemperature = source.BRailPreheatRight.CurrentTemperature,
                    SetTemperature = source.BRailPreheatRight.SetTemperature,
                    TemperatureRange = source.BRailPreheatRight.TemperatureRange,
                    IdleCloseInterval = source.BRailPreheatRight.IdleCloseInterval,
                    PreheatTime = source.BRailPreheatRight.PreheatTime,
                    HeatingTime = source.BRailPreheatRight.HeatingTime,
                    DetectionEnabled = source.BRailPreheatRight.DetectionEnabled,
                    WorkEnabled = source.BRailPreheatRight.WorkEnabled,
                    DelayTime = source.BRailPreheatRight.DelayTime
                };
            }

            if (source.BRailPreheatRight2 != null)
            {
                this.BRailPreheatRight2 = new HeatingModuleInfo
                {
                    Switch = source.BRailPreheatRight2.Switch,
                    ModuleName = source.BRailPreheatRight2.ModuleName,
                    CurrentTemperature = source.BRailPreheatRight2.CurrentTemperature,
                    SetTemperature = source.BRailPreheatRight2.SetTemperature,
                    TemperatureRange = source.BRailPreheatRight2.TemperatureRange,
                    IdleCloseInterval = source.BRailPreheatRight2.IdleCloseInterval,
                    PreheatTime = source.BRailPreheatRight2.PreheatTime,
                    HeatingTime = source.BRailPreheatRight2.HeatingTime,
                    DetectionEnabled = source.BRailPreheatRight2.DetectionEnabled,
                    WorkEnabled = source.BRailPreheatRight2.WorkEnabled,
                    DelayTime = source.BRailPreheatRight2.DelayTime
                };
            }
        }
    }

    /// <summary>
    /// 按钮图元属性绑定编辑模型对象，可由图元继承
    /// </summary>
    [Serializable]
    [MapPropertyOrder]
    [CategoryPriority("点位信息", 1)]
    [CategoryPriority("绑定信息", 2)]
    public class HeatingControlPropertyBindEditModel : FunctionalCellPropertyBindEditModel
    {
        private string _textFont = nameof(HeatingControlPropertyModelEdit.TextFont);
        [DisplayName("文本字体")]
        [Category("绑定信息")]
        [PropertySortOrder(20)]
        [Browsable(false)]
        public string TextFont { get { return _textFont; } set { SetProperty(ref _textFont, value); } }

        private string _textColor = nameof(HeatingControlPropertyModelEdit.TextColor);
        [DisplayName("文本颜色")]
        [Category("绑定信息")]
        [PropertySortOrder(21)]
        [Browsable(false)]
        public string TextColor { get { return _textColor; } set { SetProperty(ref _textColor, value); } }

        private string _backColor = nameof(HeatingControlPropertyModelEdit.BackColor);
        [DisplayName("背景颜色")]
        [Category("绑定信息")]
        [PropertySortOrder(22)]
        [Browsable(false)]
        public string BackColor { get { return _backColor; } set { SetProperty(ref _backColor, value); } }

        private string _isDualValve = nameof(HeatingControlPropertyModelEdit.IsDualValve);
        [DisplayName("双阀模式")]
        [Category("绑定信息")]
        [PropertySortOrder(23)]
        [Browsable(false)]
        public string IsDualValve { get { return _isDualValve; } set { SetProperty(ref _isDualValve, value); } }

        private string _isDualTrack = nameof(HeatingControlPropertyModelEdit.IsDualTrack);
        [DisplayName("双轨模式")]
        [Category("绑定信息")]
        [PropertySortOrder(24)]
        [Browsable(false)]
        public string IsDualTrack { get { return _isDualTrack; } set { SetProperty(ref _isDualTrack, value); } }

        private string _rightValveDispensingHead = nameof(HeatingControlPropertyModelEdit.RightValveDispensingHead);
        [DisplayName("右阀点胶头")]
        [Category("绑定信息")]
        [PropertySortOrder(25)]
        [Browsable(false)]
        public string RightValveDispensingHead { get { return _rightValveDispensingHead; } set { SetProperty(ref _rightValveDispensingHead, value); } }

        private string _rightValveDispensingHead_Switch = "RightValveDispensingHead_Switch";
        [DisplayName("右阀点胶头_关/开")]
        [Category("绑定信息")]
        [PropertySortOrder(31)]
        [Browsable(false)]
        public string RightValveDispensingHead_Switch { get { return _rightValveDispensingHead_Switch; } set { SetProperty(ref _rightValveDispensingHead_Switch, value); } }

        private string _rightValveDispensingHead_CurrentTemperature = "RightValveDispensingHead_CurrentTemperature";
        [DisplayName("右阀点胶头_当前温度")]
        [Category("绑定信息")]
        [PropertySortOrder(41)]
        [Browsable(false)]
        public string RightValveDispensingHead_CurrentTemperature { get { return _rightValveDispensingHead_CurrentTemperature; } set { SetProperty(ref _rightValveDispensingHead_CurrentTemperature, value); } }

        private string _rightValveDispensingHead_SetTemperature = "RightValveDispensingHead_SetTemperature";
        [DisplayName("右阀点胶头_设定温度")]
        [Category("绑定信息")]
        [PropertySortOrder(42)]
        [Browsable(false)]
        public string RightValveDispensingHead_SetTemperature { get { return _rightValveDispensingHead_SetTemperature; } set { SetProperty(ref _rightValveDispensingHead_SetTemperature, value); } }

        private string _rightValveDispensingHead_TemperatureRange = "RightValveDispensingHead_TemperatureRange";
        [DisplayName("右阀点胶头_温度范围")]
        [Category("绑定信息")]
        [PropertySortOrder(73)]
        [Browsable(false)]
        public string RightValveDispensingHead_TemperatureRange { get { return _rightValveDispensingHead_TemperatureRange; } set { SetProperty(ref _rightValveDispensingHead_TemperatureRange, value); } }

        private string _rightValveDispensingHead_IdleCloseInterval = "RightValveDispensingHead_IdleCloseInterval";
        [DisplayName("右阀点胶头_空闲关闭间隔")]
        [Category("绑定信息")]
        [PropertySortOrder(74)]
        [Browsable(false)]
        public string RightValveDispensingHead_IdleCloseInterval { get { return _rightValveDispensingHead_IdleCloseInterval; } set { SetProperty(ref _rightValveDispensingHead_IdleCloseInterval, value); } }

        private string _rightValveDispensingHead_PreheatTime = "RightValveDispensingHead_PreheatTime";
        [DisplayName("右阀点胶头_预热时间")]
        [Category("绑定信息")]
        [PropertySortOrder(75)]
        [Browsable(false)]
        public string RightValveDispensingHead_PreheatTime { get { return _rightValveDispensingHead_PreheatTime; } set { SetProperty(ref _rightValveDispensingHead_PreheatTime, value); } }

        private string _rightValveDispensingHead_HeatingTime = "RightValveDispensingHead_HeatingTime";
        [DisplayName("右阀点胶头_加热时间")]
        [Category("绑定信息")]
        [PropertySortOrder(76)]
        [Browsable(false)]
        public string RightValveDispensingHead_HeatingTime { get { return _rightValveDispensingHead_HeatingTime; } set { SetProperty(ref _rightValveDispensingHead_HeatingTime, value); } }

        private string _rightValveDispensingHead_DetectionEnabled = "RightValveDispensingHead_DetectionEnabled";
        [DisplayName("右阀点胶头_检测开启")]
        [Category("绑定信息")]
        [PropertySortOrder(77)]
        [Browsable(false)]
        public string RightValveDispensingHead_DetectionEnabled { get { return _rightValveDispensingHead_DetectionEnabled; } set { SetProperty(ref _rightValveDispensingHead_DetectionEnabled, value); } }

        private string _rightValveDispensingHead_WorkEnabled = "RightValveDispensingHead_WorkEnabled";
        [DisplayName("右阀点胶头_工作开启")]
        [Category("绑定信息")]
        [PropertySortOrder(78)]
        [Browsable(false)]
        public string RightValveDispensingHead_WorkEnabled { get { return _rightValveDispensingHead_WorkEnabled; } set { SetProperty(ref _rightValveDispensingHead_WorkEnabled, value); } }

        private string _rightValveDispensingHead_DelayTime = "RightValveDispensingHead_DelayTime";
        [DisplayName("右阀点胶头_延迟时间")]
        [Category("绑定信息")]
        [PropertySortOrder(79)]
        [Browsable(false)]
        public string RightValveDispensingHead_DelayTime { get { return _rightValveDispensingHead_DelayTime; } set { SetProperty(ref _rightValveDispensingHead_DelayTime, value); } }

        private string _rightValveCartridgeHeating = nameof(HeatingControlPropertyModelEdit.RightValveCartridgeHeating);
        [DisplayName("右阀胶筒加热")]
        [Category("绑定信息")]
        [PropertySortOrder(26)]
        [Browsable(false)]
        public string RightValveCartridgeHeating { get { return _rightValveCartridgeHeating; } set { SetProperty(ref _rightValveCartridgeHeating, value); } }

        private string _rightValveCartridgeHeating_Switch = "RightValveCartridgeHeating_Switch";
        [DisplayName("右阀胶筒加热_关/开")]
        [Category("绑定信息")]
        [PropertySortOrder(32)]
        [Browsable(false)]
        public string RightValveCartridgeHeating_Switch { get { return _rightValveCartridgeHeating_Switch; } set { SetProperty(ref _rightValveCartridgeHeating_Switch, value); } }

        private string _rightValveCartridgeHeating_CurrentTemperature = "RightValveCartridgeHeating_CurrentTemperature";
        [DisplayName("右阀胶筒加热_当前温度")]
        [Category("绑定信息")]
        [PropertySortOrder(43)]
        [Browsable(false)]
        public string RightValveCartridgeHeating_CurrentTemperature { get { return _rightValveCartridgeHeating_CurrentTemperature; } set { SetProperty(ref _rightValveCartridgeHeating_CurrentTemperature, value); } }

        private string _rightValveCartridgeHeating_SetTemperature = "RightValveCartridgeHeating_SetTemperature";
        [DisplayName("右阀胶筒加热_设定温度")]
        [Category("绑定信息")]
        [PropertySortOrder(44)]
        [Browsable(false)]
        public string RightValveCartridgeHeating_SetTemperature { get { return _rightValveCartridgeHeating_SetTemperature; } set { SetProperty(ref _rightValveCartridgeHeating_SetTemperature, value); } }

        private string _rightValveCartridgeHeating_TemperatureRange = "RightValveCartridgeHeating_TemperatureRange";
        [DisplayName("右阀胶筒加热_温度范围")]
        [Category("绑定信息")]
        [PropertySortOrder(80)]
        [Browsable(false)]
        public string RightValveCartridgeHeating_TemperatureRange { get { return _rightValveCartridgeHeating_TemperatureRange; } set { SetProperty(ref _rightValveCartridgeHeating_TemperatureRange, value); } }

        private string _rightValveCartridgeHeating_IdleCloseInterval = "RightValveCartridgeHeating_IdleCloseInterval";
        [DisplayName("右阀胶筒加热_空闲关闭间隔")]
        [Category("绑定信息")]
        [PropertySortOrder(81)]
        [Browsable(false)]
        public string RightValveCartridgeHeating_IdleCloseInterval { get { return _rightValveCartridgeHeating_IdleCloseInterval; } set { SetProperty(ref _rightValveCartridgeHeating_IdleCloseInterval, value); } }

        private string _rightValveCartridgeHeating_PreheatTime = "RightValveCartridgeHeating_PreheatTime";
        [DisplayName("右阀胶筒加热_预热时间")]
        [Category("绑定信息")]
        [PropertySortOrder(82)]
        [Browsable(false)]
        public string RightValveCartridgeHeating_PreheatTime { get { return _rightValveCartridgeHeating_PreheatTime; } set { SetProperty(ref _rightValveCartridgeHeating_PreheatTime, value); } }

        private string _rightValveCartridgeHeating_HeatingTime = "RightValveCartridgeHeating_HeatingTime";
        [DisplayName("右阀胶筒加热_加热时间")]
        [Category("绑定信息")]
        [PropertySortOrder(83)]
        [Browsable(false)]
        public string RightValveCartridgeHeating_HeatingTime { get { return _rightValveCartridgeHeating_HeatingTime; } set { SetProperty(ref _rightValveCartridgeHeating_HeatingTime, value); } }

        private string _rightValveCartridgeHeating_DetectionEnabled = "RightValveCartridgeHeating_DetectionEnabled";
        [DisplayName("右阀胶筒加热_检测开启")]
        [Category("绑定信息")]
        [PropertySortOrder(84)]
        [Browsable(false)]
        public string RightValveCartridgeHeating_DetectionEnabled { get { return _rightValveCartridgeHeating_DetectionEnabled; } set { SetProperty(ref _rightValveCartridgeHeating_DetectionEnabled, value); } }

        private string _rightValveCartridgeHeating_WorkEnabled = "RightValveCartridgeHeating_WorkEnabled";
        [DisplayName("右阀胶筒加热_工作开启")]
        [Category("绑定信息")]
        [PropertySortOrder(85)]
        [Browsable(false)]
        public string RightValveCartridgeHeating_WorkEnabled { get { return _rightValveCartridgeHeating_WorkEnabled; } set { SetProperty(ref _rightValveCartridgeHeating_WorkEnabled, value); } }

        private string _rightValveCartridgeHeating_DelayTime = "RightValveCartridgeHeating_DelayTime";
        [DisplayName("右阀胶筒加热_延迟时间")]
        [Category("绑定信息")]
        [PropertySortOrder(86)]
        [Browsable(false)]
        public string RightValveCartridgeHeating_DelayTime { get { return _rightValveCartridgeHeating_DelayTime; } set { SetProperty(ref _rightValveCartridgeHeating_DelayTime, value); } }

        private string _leftValveDispensingHead = nameof(HeatingControlPropertyModelEdit.LeftValveDispensingHead);
        [DisplayName("左阀点胶头")]
        [Category("绑定信息")]
        [PropertySortOrder(27)]
        [Browsable(false)]
        public string LeftValveDispensingHead { get { return _leftValveDispensingHead; } set { SetProperty(ref _leftValveDispensingHead, value); } }

        private string _leftValveDispensingHead_Switch = "LeftValveDispensingHead_Switch";
        [DisplayName("左阀点胶头_关/开")]
        [Category("绑定信息")]
        [PropertySortOrder(33)]
        [Browsable(false)]
        public string LeftValveDispensingHead_Switch { get { return _leftValveDispensingHead_Switch; } set { SetProperty(ref _leftValveDispensingHead_Switch, value); } }

        private string _leftValveDispensingHead_CurrentTemperature = "LeftValveDispensingHead_CurrentTemperature";
        [DisplayName("左阀点胶头_当前温度")]
        [Category("绑定信息")]
        [PropertySortOrder(45)]
        [Browsable(false)]
        public string LeftValveDispensingHead_CurrentTemperature { get { return _leftValveDispensingHead_CurrentTemperature; } set { SetProperty(ref _leftValveDispensingHead_CurrentTemperature, value); } }

        private string _leftValveDispensingHead_SetTemperature = "LeftValveDispensingHead_SetTemperature";
        [DisplayName("左阀点胶头_设定温度")]
        [Category("绑定信息")]
        [PropertySortOrder(46)]
        [Browsable(false)]
        public string LeftValveDispensingHead_SetTemperature { get { return _leftValveDispensingHead_SetTemperature; } set { SetProperty(ref _leftValveDispensingHead_SetTemperature, value); } }

        private string _leftValveDispensingHead_TemperatureRange = "LeftValveDispensingHead_TemperatureRange";
        [DisplayName("左阀点胶头_温度范围")]
        [Category("绑定信息")]
        [PropertySortOrder(87)]
        [Browsable(false)]
        public string LeftValveDispensingHead_TemperatureRange { get { return _leftValveDispensingHead_TemperatureRange; } set { SetProperty(ref _leftValveDispensingHead_TemperatureRange, value); } }

        private string _leftValveDispensingHead_IdleCloseInterval = "LeftValveDispensingHead_IdleCloseInterval";
        [DisplayName("左阀点胶头_空闲关闭间隔")]
        [Category("绑定信息")]
        [PropertySortOrder(88)]
        [Browsable(false)]
        public string LeftValveDispensingHead_IdleCloseInterval { get { return _leftValveDispensingHead_IdleCloseInterval; } set { SetProperty(ref _leftValveDispensingHead_IdleCloseInterval, value); } }

        private string _leftValveDispensingHead_PreheatTime = "LeftValveDispensingHead_PreheatTime";
        [DisplayName("左阀点胶头_预热时间")]
        [Category("绑定信息")]
        [PropertySortOrder(89)]
        [Browsable(false)]
        public string LeftValveDispensingHead_PreheatTime { get { return _leftValveDispensingHead_PreheatTime; } set { SetProperty(ref _leftValveDispensingHead_PreheatTime, value); } }

        private string _leftValveDispensingHead_HeatingTime = "LeftValveDispensingHead_HeatingTime";
        [DisplayName("左阀点胶头_加热时间")]
        [Category("绑定信息")]
        [PropertySortOrder(90)]
        [Browsable(false)]
        public string LeftValveDispensingHead_HeatingTime { get { return _leftValveDispensingHead_HeatingTime; } set { SetProperty(ref _leftValveDispensingHead_HeatingTime, value); } }

        private string _leftValveDispensingHead_DetectionEnabled = "LeftValveDispensingHead_DetectionEnabled";
        [DisplayName("左阀点胶头_检测开启")]
        [Category("绑定信息")]
        [PropertySortOrder(91)]
        [Browsable(false)]
        public string LeftValveDispensingHead_DetectionEnabled { get { return _leftValveDispensingHead_DetectionEnabled; } set { SetProperty(ref _leftValveDispensingHead_DetectionEnabled, value); } }

        private string _leftValveDispensingHead_WorkEnabled = "LeftValveDispensingHead_WorkEnabled";
        [DisplayName("左阀点胶头_工作开启")]
        [Category("绑定信息")]
        [PropertySortOrder(92)]
        [Browsable(false)]
        public string LeftValveDispensingHead_WorkEnabled { get { return _leftValveDispensingHead_WorkEnabled; } set { SetProperty(ref _leftValveDispensingHead_WorkEnabled, value); } }

        private string _leftValveDispensingHead_DelayTime = "LeftValveDispensingHead_DelayTime";
        [DisplayName("左阀点胶头_延迟时间")]
        [Category("绑定信息")]
        [PropertySortOrder(93)]
        [Browsable(false)]
        public string LeftValveDispensingHead_DelayTime { get { return _leftValveDispensingHead_DelayTime; } set { SetProperty(ref _leftValveDispensingHead_DelayTime, value); } }

        private string _leftValveCartridgeHeating = nameof(HeatingControlPropertyModelEdit.LeftValveCartridgeHeating);
        [DisplayName("左阀胶筒加热")]
        [Category("绑定信息")]
        [PropertySortOrder(28)]
        [Browsable(false)]
        public string LeftValveCartridgeHeating { get { return _leftValveCartridgeHeating; } set { SetProperty(ref _leftValveCartridgeHeating, value); } }

        private string _leftValveCartridgeHeating_Switch = "LeftValveCartridgeHeating_Switch";
        [DisplayName("左阀胶筒加热_关/开")]
        [Category("绑定信息")]
        [PropertySortOrder(34)]
        [Browsable(false)]
        public string LeftValveCartridgeHeating_Switch { get { return _leftValveCartridgeHeating_Switch; } set { SetProperty(ref _leftValveCartridgeHeating_Switch, value); } }

        private string _leftValveCartridgeHeating_CurrentTemperature = "LeftValveCartridgeHeating_CurrentTemperature";
        [DisplayName("左阀胶筒加热_当前温度")]
        [Category("绑定信息")]
        [PropertySortOrder(47)]
        [Browsable(false)]
        public string LeftValveCartridgeHeating_CurrentTemperature { get { return _leftValveCartridgeHeating_CurrentTemperature; } set { SetProperty(ref _leftValveCartridgeHeating_CurrentTemperature, value); } }

        private string _leftValveCartridgeHeating_SetTemperature = "LeftValveCartridgeHeating_SetTemperature";
        [DisplayName("左阀胶筒加热_设定温度")]
        [Category("绑定信息")]
        [PropertySortOrder(48)]
        [Browsable(false)]
        public string LeftValveCartridgeHeating_SetTemperature { get { return _leftValveCartridgeHeating_SetTemperature; } set { SetProperty(ref _leftValveCartridgeHeating_SetTemperature, value); } }

        private string _leftValveCartridgeHeating_TemperatureRange = "LeftValveCartridgeHeating_TemperatureRange";
        [DisplayName("左阀胶筒加热_温度范围")]
        [Category("绑定信息")]
        [PropertySortOrder(94)]
        [Browsable(false)]
        public string LeftValveCartridgeHeating_TemperatureRange { get { return _leftValveCartridgeHeating_TemperatureRange; } set { SetProperty(ref _leftValveCartridgeHeating_TemperatureRange, value); } }

        private string _leftValveCartridgeHeating_IdleCloseInterval = "LeftValveCartridgeHeating_IdleCloseInterval";
        [DisplayName("左阀胶筒加热_空闲关闭间隔")]
        [Category("绑定信息")]
        [PropertySortOrder(95)]
        [Browsable(false)]
        public string LeftValveCartridgeHeating_IdleCloseInterval { get { return _leftValveCartridgeHeating_IdleCloseInterval; } set { SetProperty(ref _leftValveCartridgeHeating_IdleCloseInterval, value); } }

        private string _leftValveCartridgeHeating_PreheatTime = "LeftValveCartridgeHeating_PreheatTime";
        [DisplayName("左阀胶筒加热_预热时间")]
        [Category("绑定信息")]
        [PropertySortOrder(96)]
        [Browsable(false)]
        public string LeftValveCartridgeHeating_PreheatTime { get { return _leftValveCartridgeHeating_PreheatTime; } set { SetProperty(ref _leftValveCartridgeHeating_PreheatTime, value); } }

        private string _leftValveCartridgeHeating_HeatingTime = "LeftValveCartridgeHeating_HeatingTime";
        [DisplayName("左阀胶筒加热_加热时间")]
        [Category("绑定信息")]
        [PropertySortOrder(97)]
        [Browsable(false)]
        public string LeftValveCartridgeHeating_HeatingTime { get { return _leftValveCartridgeHeating_HeatingTime; } set { SetProperty(ref _leftValveCartridgeHeating_HeatingTime, value); } }

        private string _leftValveCartridgeHeating_DetectionEnabled = "LeftValveCartridgeHeating_DetectionEnabled";
        [DisplayName("左阀胶筒加热_检测开启")]
        [Category("绑定信息")]
        [PropertySortOrder(98)]
        [Browsable(false)]
        public string LeftValveCartridgeHeating_DetectionEnabled { get { return _leftValveCartridgeHeating_DetectionEnabled; } set { SetProperty(ref _leftValveCartridgeHeating_DetectionEnabled, value); } }

        private string _leftValveCartridgeHeating_WorkEnabled = "LeftValveCartridgeHeating_WorkEnabled";
        [DisplayName("左阀胶筒加热_工作开启")]
        [Category("绑定信息")]
        [PropertySortOrder(99)]
        [Browsable(false)]
        public string LeftValveCartridgeHeating_WorkEnabled { get { return _leftValveCartridgeHeating_WorkEnabled; } set { SetProperty(ref _leftValveCartridgeHeating_WorkEnabled, value); } }

        private string _leftValveCartridgeHeating_DelayTime = "LeftValveCartridgeHeating_DelayTime";
        [DisplayName("左阀胶筒加热_延迟时间")]
        [Category("绑定信息")]
        [PropertySortOrder(100)]
        [Browsable(false)]
        public string LeftValveCartridgeHeating_DelayTime { get { return _leftValveCartridgeHeating_DelayTime; } set { SetProperty(ref _leftValveCartridgeHeating_DelayTime, value); } }

        private string _aRailPreheatLeft = nameof(HeatingControlPropertyModelEdit.ARailPreheatLeft);
        [DisplayName("A轨预热-左")]
        [Category("绑定信息")]
        [PropertySortOrder(29)]
        [Browsable(false)]
        public string ARailPreheatLeft { get { return _aRailPreheatLeft; } set { SetProperty(ref _aRailPreheatLeft, value); } }

        private string _aRailPreheatLeft_Switch = "ARailPreheatLeft_Switch";
        [DisplayName("A轨预热-左_关/开")]
        [Category("绑定信息")]
        [PropertySortOrder(101)]
        [Browsable(false)]
        public string ARailPreheatLeft_Switch { get { return _aRailPreheatLeft_Switch; } set { SetProperty(ref _aRailPreheatLeft_Switch, value); } }

        private string _aRailPreheatLeft_CurrentTemperature = "ARailPreheatLeft_CurrentTemperature";
        [DisplayName("A轨预热-左_当前温度")]
        [Category("绑定信息")]
        [PropertySortOrder(49)]
        [Browsable(false)]
        public string ARailPreheatLeft_CurrentTemperature { get { return _aRailPreheatLeft_CurrentTemperature; } set { SetProperty(ref _aRailPreheatLeft_CurrentTemperature, value); } }

        private string _aRailPreheatLeft_SetTemperature = "ARailPreheatLeft_SetTemperature";
        [DisplayName("A轨预热-左_设定温度")]
        [Category("绑定信息")]
        [PropertySortOrder(50)]
        [Browsable(false)]
        public string ARailPreheatLeft_SetTemperature { get { return _aRailPreheatLeft_SetTemperature; } set { SetProperty(ref _aRailPreheatLeft_SetTemperature, value); } }

        private string _aRailPreheatLeft_TemperatureRange = "ARailPreheatLeft_TemperatureRange";
        [DisplayName("A轨预热-左_温度范围")]
        [Category("绑定信息")]
        [PropertySortOrder(102)]
        [Browsable(false)]
        public string ARailPreheatLeft_TemperatureRange { get { return _aRailPreheatLeft_TemperatureRange; } set { SetProperty(ref _aRailPreheatLeft_TemperatureRange, value); } }

        private string _aRailPreheatLeft_IdleCloseInterval = "ARailPreheatLeft_IdleCloseInterval";
        [DisplayName("A轨预热-左_空闲关闭间隔")]
        [Category("绑定信息")]
        [PropertySortOrder(103)]
        [Browsable(false)]
        public string ARailPreheatLeft_IdleCloseInterval { get { return _aRailPreheatLeft_IdleCloseInterval; } set { SetProperty(ref _aRailPreheatLeft_IdleCloseInterval, value); } }

        private string _aRailPreheatLeft_PreheatTime = "ARailPreheatLeft_PreheatTime";
        [DisplayName("A轨预热-左_预热时间")]
        [Category("绑定信息")]
        [PropertySortOrder(104)]
        [Browsable(false)]
        public string ARailPreheatLeft_PreheatTime { get { return _aRailPreheatLeft_PreheatTime; } set { SetProperty(ref _aRailPreheatLeft_PreheatTime, value); } }

        private string _aRailPreheatLeft_HeatingTime = "ARailPreheatLeft_HeatingTime";
        [DisplayName("A轨预热-左_加热时间")]
        [Category("绑定信息")]
        [PropertySortOrder(105)]
        [Browsable(false)]
        public string ARailPreheatLeft_HeatingTime { get { return _aRailPreheatLeft_HeatingTime; } set { SetProperty(ref _aRailPreheatLeft_HeatingTime, value); } }

        private string _aRailPreheatLeft_DetectionEnabled = "ARailPreheatLeft_DetectionEnabled";
        [DisplayName("A轨预热-左_检测开启")]
        [Category("绑定信息")]
        [PropertySortOrder(106)]
        [Browsable(false)]
        public string ARailPreheatLeft_DetectionEnabled { get { return _aRailPreheatLeft_DetectionEnabled; } set { SetProperty(ref _aRailPreheatLeft_DetectionEnabled, value); } }

        private string _aRailPreheatLeft_WorkEnabled = "ARailPreheatLeft_WorkEnabled";
        [DisplayName("A轨预热-左_工作开启")]
        [Category("绑定信息")]
        [PropertySortOrder(107)]
        [Browsable(false)]
        public string ARailPreheatLeft_WorkEnabled { get { return _aRailPreheatLeft_WorkEnabled; } set { SetProperty(ref _aRailPreheatLeft_WorkEnabled, value); } }

        private string _aRailPreheatLeft_DelayTime = "ARailPreheatLeft_DelayTime";
        [DisplayName("A轨预热-左_延迟时间")]
        [Category("绑定信息")]
        [PropertySortOrder(108)]
        [Browsable(false)]
        public string ARailPreheatLeft_DelayTime { get { return _aRailPreheatLeft_DelayTime; } set { SetProperty(ref _aRailPreheatLeft_DelayTime, value); } }

        private string _aRailPreheatLeft2 = nameof(HeatingControlPropertyModelEdit.ARailPreheatLeft2);
        [DisplayName("A轨预热-左(2)")]
        [Category("绑定信息")]
        [PropertySortOrder(30)]
        [Browsable(false)]
        public string ARailPreheatLeft2 { get { return _aRailPreheatLeft2; } set { SetProperty(ref _aRailPreheatLeft2, value); } }

        private string _aRailPreheatLeft2_Switch = "ARailPreheatLeft2_Switch";
        [DisplayName("A轨预热-左(2)_关/开")]
        [Category("绑定信息")]
        [PropertySortOrder(109)]
        [Browsable(false)]
        public string ARailPreheatLeft2_Switch { get { return _aRailPreheatLeft2_Switch; } set { SetProperty(ref _aRailPreheatLeft2_Switch, value); } }

        private string _aRailPreheatLeft2_CurrentTemperature = "ARailPreheatLeft2_CurrentTemperature";
        [DisplayName("A轨预热-左(2)_当前温度")]
        [Category("绑定信息")]
        [PropertySortOrder(51)]
        [Browsable(false)]
        public string ARailPreheatLeft2_CurrentTemperature { get { return _aRailPreheatLeft2_CurrentTemperature; } set { SetProperty(ref _aRailPreheatLeft2_CurrentTemperature, value); } }

        private string _aRailPreheatLeft2_SetTemperature = "ARailPreheatLeft2_SetTemperature";
        [DisplayName("A轨预热-左(2)_设定温度")]
        [Category("绑定信息")]
        [PropertySortOrder(52)]
        [Browsable(false)]
        public string ARailPreheatLeft2_SetTemperature { get { return _aRailPreheatLeft2_SetTemperature; } set { SetProperty(ref _aRailPreheatLeft2_SetTemperature, value); } }

        private string _aRailPreheatLeft2_TemperatureRange = "ARailPreheatLeft2_TemperatureRange";
        [DisplayName("A轨预热-左(2)_温度范围")]
        [Category("绑定信息")]
        [PropertySortOrder(110)]
        [Browsable(false)]
        public string ARailPreheatLeft2_TemperatureRange { get { return _aRailPreheatLeft2_TemperatureRange; } set { SetProperty(ref _aRailPreheatLeft2_TemperatureRange, value); } }

        private string _aRailPreheatLeft2_IdleCloseInterval = "ARailPreheatLeft2_IdleCloseInterval";
        [DisplayName("A轨预热-左(2)_空闲关闭间隔")]
        [Category("绑定信息")]
        [PropertySortOrder(111)]
        [Browsable(false)]
        public string ARailPreheatLeft2_IdleCloseInterval { get { return _aRailPreheatLeft2_IdleCloseInterval; } set { SetProperty(ref _aRailPreheatLeft2_IdleCloseInterval, value); } }

        private string _aRailPreheatLeft2_PreheatTime = "ARailPreheatLeft2_PreheatTime";
        [DisplayName("A轨预热-左(2)_预热时间")]
        [Category("绑定信息")]
        [PropertySortOrder(112)]
        [Browsable(false)]
        public string ARailPreheatLeft2_PreheatTime { get { return _aRailPreheatLeft2_PreheatTime; } set { SetProperty(ref _aRailPreheatLeft2_PreheatTime, value); } }

        private string _aRailPreheatLeft2_HeatingTime = "ARailPreheatLeft2_HeatingTime";
        [DisplayName("A轨预热-左(2)_加热时间")]
        [Category("绑定信息")]
        [PropertySortOrder(113)]
        [Browsable(false)]
        public string ARailPreheatLeft2_HeatingTime { get { return _aRailPreheatLeft2_HeatingTime; } set { SetProperty(ref _aRailPreheatLeft2_HeatingTime, value); } }

        private string _aRailPreheatLeft2_DetectionEnabled = "ARailPreheatLeft2_DetectionEnabled";
        [DisplayName("A轨预热-左(2)_检测开启")]
        [Category("绑定信息")]
        [PropertySortOrder(114)]
        [Browsable(false)]
        public string ARailPreheatLeft2_DetectionEnabled { get { return _aRailPreheatLeft2_DetectionEnabled; } set { SetProperty(ref _aRailPreheatLeft2_DetectionEnabled, value); } }

        private string _aRailPreheatLeft2_WorkEnabled = "ARailPreheatLeft2_WorkEnabled";
        [DisplayName("A轨预热-左(2)_工作开启")]
        [Category("绑定信息")]
        [PropertySortOrder(115)]
        [Browsable(false)]
        public string ARailPreheatLeft2_WorkEnabled { get { return _aRailPreheatLeft2_WorkEnabled; } set { SetProperty(ref _aRailPreheatLeft2_WorkEnabled, value); } }

        private string _aRailPreheatLeft2_DelayTime = "ARailPreheatLeft2_DelayTime";
        [DisplayName("A轨预热-左(2)_延迟时间")]
        [Category("绑定信息")]
        [PropertySortOrder(116)]
        [Browsable(false)]
        public string ARailPreheatLeft2_DelayTime { get { return _aRailPreheatLeft2_DelayTime; } set { SetProperty(ref _aRailPreheatLeft2_DelayTime, value); } }

        private string _aRailGlueBoardStationMiddle = nameof(HeatingControlPropertyModelEdit.ARailGlueBoardStationMiddle);
        [DisplayName("A轨胶板工位-中")]
        [Category("绑定信息")]
        [PropertySortOrder(31)]
        [Browsable(false)]
        public string ARailGlueBoardStationMiddle { get { return _aRailGlueBoardStationMiddle; } set { SetProperty(ref _aRailGlueBoardStationMiddle, value); } }

        private string _aRailGlueBoardStationMiddle_Switch = "ARailGlueBoardStationMiddle_Switch";
        [DisplayName("A轨胶板工位-中_关/开")]
        [Category("绑定信息")]
        [PropertySortOrder(117)]
        [Browsable(false)]
        public string ARailGlueBoardStationMiddle_Switch { get { return _aRailGlueBoardStationMiddle_Switch; } set { SetProperty(ref _aRailGlueBoardStationMiddle_Switch, value); } }

        private string _aRailGlueBoardStationMiddle_CurrentTemperature = "ARailGlueBoardStationMiddle_CurrentTemperature";
        [DisplayName("A轨胶板工位-中_当前温度")]
        [Category("绑定信息")]
        [PropertySortOrder(53)]
        [Browsable(false)]
        public string ARailGlueBoardStationMiddle_CurrentTemperature { get { return _aRailGlueBoardStationMiddle_CurrentTemperature; } set { SetProperty(ref _aRailGlueBoardStationMiddle_CurrentTemperature, value); } }

        private string _aRailGlueBoardStationMiddle_SetTemperature = "ARailGlueBoardStationMiddle_SetTemperature";
        [DisplayName("A轨胶板工位-中_设定温度")]
        [Category("绑定信息")]
        [PropertySortOrder(54)]
        [Browsable(false)]
        public string ARailGlueBoardStationMiddle_SetTemperature { get { return _aRailGlueBoardStationMiddle_SetTemperature; } set { SetProperty(ref _aRailGlueBoardStationMiddle_SetTemperature, value); } }

        private string _aRailGlueBoardStationMiddle_TemperatureRange = "ARailGlueBoardStationMiddle_TemperatureRange";
        [DisplayName("A轨胶板工位-中_温度范围")]
        [Category("绑定信息")]
        [PropertySortOrder(118)]
        [Browsable(false)]
        public string ARailGlueBoardStationMiddle_TemperatureRange { get { return _aRailGlueBoardStationMiddle_TemperatureRange; } set { SetProperty(ref _aRailGlueBoardStationMiddle_TemperatureRange, value); } }

        private string _aRailGlueBoardStationMiddle_IdleCloseInterval = "ARailGlueBoardStationMiddle_IdleCloseInterval";
        [DisplayName("A轨胶板工位-中_空闲关闭间隔")]
        [Category("绑定信息")]
        [PropertySortOrder(119)]
        [Browsable(false)]
        public string ARailGlueBoardStationMiddle_IdleCloseInterval { get { return _aRailGlueBoardStationMiddle_IdleCloseInterval; } set { SetProperty(ref _aRailGlueBoardStationMiddle_IdleCloseInterval, value); } }

        private string _aRailGlueBoardStationMiddle_PreheatTime = "ARailGlueBoardStationMiddle_PreheatTime";
        [DisplayName("A轨胶板工位-中_预热时间")]
        [Category("绑定信息")]
        [PropertySortOrder(120)]
        [Browsable(false)]
        public string ARailGlueBoardStationMiddle_PreheatTime { get { return _aRailGlueBoardStationMiddle_PreheatTime; } set { SetProperty(ref _aRailGlueBoardStationMiddle_PreheatTime, value); } }

        private string _aRailGlueBoardStationMiddle_HeatingTime = "ARailGlueBoardStationMiddle_HeatingTime";
        [DisplayName("A轨胶板工位-中_加热时间")]
        [Category("绑定信息")]
        [PropertySortOrder(121)]
        [Browsable(false)]
        public string ARailGlueBoardStationMiddle_HeatingTime { get { return _aRailGlueBoardStationMiddle_HeatingTime; } set { SetProperty(ref _aRailGlueBoardStationMiddle_HeatingTime, value); } }

        private string _aRailGlueBoardStationMiddle_DetectionEnabled = "ARailGlueBoardStationMiddle_DetectionEnabled";
        [DisplayName("A轨胶板工位-中_检测开启")]
        [Category("绑定信息")]
        [PropertySortOrder(122)]
        [Browsable(false)]
        public string ARailGlueBoardStationMiddle_DetectionEnabled { get { return _aRailGlueBoardStationMiddle_DetectionEnabled; } set { SetProperty(ref _aRailGlueBoardStationMiddle_DetectionEnabled, value); } }

        private string _aRailGlueBoardStationMiddle_WorkEnabled = "ARailGlueBoardStationMiddle_WorkEnabled";
        [DisplayName("A轨胶板工位-中_工作开启")]
        [Category("绑定信息")]
        [PropertySortOrder(123)]
        [Browsable(false)]
        public string ARailGlueBoardStationMiddle_WorkEnabled { get { return _aRailGlueBoardStationMiddle_WorkEnabled; } set { SetProperty(ref _aRailGlueBoardStationMiddle_WorkEnabled, value); } }

        private string _aRailGlueBoardStationMiddle_DelayTime = "ARailGlueBoardStationMiddle_DelayTime";
        [DisplayName("A轨胶板工位-中_延迟时间")]
        [Category("绑定信息")]
        [PropertySortOrder(124)]
        [Browsable(false)]
        public string ARailGlueBoardStationMiddle_DelayTime { get { return _aRailGlueBoardStationMiddle_DelayTime; } set { SetProperty(ref _aRailGlueBoardStationMiddle_DelayTime, value); } }

        private string _aRailDispensingStationMiddle2 = nameof(HeatingControlPropertyModelEdit.ARailDispensingStationMiddle2);
        [DisplayName("A轨点胶工位-中(2)")]
        [Category("绑定信息")]
        [PropertySortOrder(32)]
        [Browsable(false)]
        public string ARailDispensingStationMiddle2 { get { return _aRailDispensingStationMiddle2; } set { SetProperty(ref _aRailDispensingStationMiddle2, value); } }

        private string _aRailDispensingStationMiddle2_Switch = "ARailDispensingStationMiddle2_Switch";
        [DisplayName("A轨点胶工位-中(2)_关/开")]
        [Category("绑定信息")]
        [PropertySortOrder(125)]
        [Browsable(false)]
        public string ARailDispensingStationMiddle2_Switch { get { return _aRailDispensingStationMiddle2_Switch; } set { SetProperty(ref _aRailDispensingStationMiddle2_Switch, value); } }

        private string _aRailDispensingStationMiddle2_CurrentTemperature = "ARailDispensingStationMiddle2_CurrentTemperature";
        [DisplayName("A轨点胶工位-中(2)_当前温度")]
        [Category("绑定信息")]
        [PropertySortOrder(55)]
        [Browsable(false)]
        public string ARailDispensingStationMiddle2_CurrentTemperature { get { return _aRailDispensingStationMiddle2_CurrentTemperature; } set { SetProperty(ref _aRailDispensingStationMiddle2_CurrentTemperature, value); } }

        private string _aRailDispensingStationMiddle2_SetTemperature = "ARailDispensingStationMiddle2_SetTemperature";
        [DisplayName("A轨点胶工位-中(2)_设定温度")]
        [Category("绑定信息")]
        [PropertySortOrder(56)]
        [Browsable(false)]
        public string ARailDispensingStationMiddle2_SetTemperature { get { return _aRailDispensingStationMiddle2_SetTemperature; } set { SetProperty(ref _aRailDispensingStationMiddle2_SetTemperature, value); } }

        private string _aRailDispensingStationMiddle2_TemperatureRange = "ARailDispensingStationMiddle2_TemperatureRange";
        [DisplayName("A轨点胶工位-中(2)_温度范围")]
        [Category("绑定信息")]
        [PropertySortOrder(126)]
        [Browsable(false)]
        public string ARailDispensingStationMiddle2_TemperatureRange { get { return _aRailDispensingStationMiddle2_TemperatureRange; } set { SetProperty(ref _aRailDispensingStationMiddle2_TemperatureRange, value); } }

        private string _aRailDispensingStationMiddle2_IdleCloseInterval = "ARailDispensingStationMiddle2_IdleCloseInterval";
        [DisplayName("A轨点胶工位-中(2)_空闲关闭间隔")]
        [Category("绑定信息")]
        [PropertySortOrder(127)]
        [Browsable(false)]
        public string ARailDispensingStationMiddle2_IdleCloseInterval { get { return _aRailDispensingStationMiddle2_IdleCloseInterval; } set { SetProperty(ref _aRailDispensingStationMiddle2_IdleCloseInterval, value); } }

        private string _aRailDispensingStationMiddle2_PreheatTime = "ARailDispensingStationMiddle2_PreheatTime";
        [DisplayName("A轨点胶工位-中(2)_预热时间")]
        [Category("绑定信息")]
        [PropertySortOrder(128)]
        [Browsable(false)]
        public string ARailDispensingStationMiddle2_PreheatTime { get { return _aRailDispensingStationMiddle2_PreheatTime; } set { SetProperty(ref _aRailDispensingStationMiddle2_PreheatTime, value); } }

        private string _aRailDispensingStationMiddle2_HeatingTime = "ARailDispensingStationMiddle2_HeatingTime";
        [DisplayName("A轨点胶工位-中(2)_加热时间")]
        [Category("绑定信息")]
        [PropertySortOrder(129)]
        [Browsable(false)]
        public string ARailDispensingStationMiddle2_HeatingTime { get { return _aRailDispensingStationMiddle2_HeatingTime; } set { SetProperty(ref _aRailDispensingStationMiddle2_HeatingTime, value); } }

        private string _aRailDispensingStationMiddle2_DetectionEnabled = "ARailDispensingStationMiddle2_DetectionEnabled";
        [DisplayName("A轨点胶工位-中(2)_检测开启")]
        [Category("绑定信息")]
        [PropertySortOrder(130)]
        [Browsable(false)]
        public string ARailDispensingStationMiddle2_DetectionEnabled { get { return _aRailDispensingStationMiddle2_DetectionEnabled; } set { SetProperty(ref _aRailDispensingStationMiddle2_DetectionEnabled, value); } }

        private string _aRailDispensingStationMiddle2_WorkEnabled = "ARailDispensingStationMiddle2_WorkEnabled";
        [DisplayName("A轨点胶工位-中(2)_工作开启")]
        [Category("绑定信息")]
        [PropertySortOrder(131)]
        [Browsable(false)]
        public string ARailDispensingStationMiddle2_WorkEnabled { get { return _aRailDispensingStationMiddle2_WorkEnabled; } set { SetProperty(ref _aRailDispensingStationMiddle2_WorkEnabled, value); } }

        private string _aRailDispensingStationMiddle2_DelayTime = "ARailDispensingStationMiddle2_DelayTime";
        [DisplayName("A轨点胶工位-中(2)_延迟时间")]
        [Category("绑定信息")]
        [PropertySortOrder(132)]
        [Browsable(false)]
        public string ARailDispensingStationMiddle2_DelayTime { get { return _aRailDispensingStationMiddle2_DelayTime; } set { SetProperty(ref _aRailDispensingStationMiddle2_DelayTime, value); } }

        private string _aRailPreheatRight = nameof(HeatingControlPropertyModelEdit.ARailPreheatRight);
        [DisplayName("A轨预热-右")]
        [Category("绑定信息")]
        [PropertySortOrder(33)]
        [Browsable(false)]
        public string ARailPreheatRight { get { return _aRailPreheatRight; } set { SetProperty(ref _aRailPreheatRight, value); } }

        private string _aRailPreheatRight_Switch = "ARailPreheatRight_Switch";
        [DisplayName("A轨预热-右_关/开")]
        [Category("绑定信息")]
        [PropertySortOrder(133)]
        [Browsable(false)]
        public string ARailPreheatRight_Switch { get { return _aRailPreheatRight_Switch; } set { SetProperty(ref _aRailPreheatRight_Switch, value); } }

        private string _aRailPreheatRight_CurrentTemperature = "ARailPreheatRight_CurrentTemperature";
        [DisplayName("A轨预热-右_当前温度")]
        [Category("绑定信息")]
        [PropertySortOrder(57)]
        [Browsable(false)]
        public string ARailPreheatRight_CurrentTemperature { get { return _aRailPreheatRight_CurrentTemperature; } set { SetProperty(ref _aRailPreheatRight_CurrentTemperature, value); } }

        private string _aRailPreheatRight_SetTemperature = "ARailPreheatRight_SetTemperature";
        [DisplayName("A轨预热-右_设定温度")]
        [Category("绑定信息")]
        [PropertySortOrder(58)]
        [Browsable(false)]
        public string ARailPreheatRight_SetTemperature { get { return _aRailPreheatRight_SetTemperature; } set { SetProperty(ref _aRailPreheatRight_SetTemperature, value); } }

        private string _aRailPreheatRight_TemperatureRange = "ARailPreheatRight_TemperatureRange";
        [DisplayName("A轨预热-右_温度范围")]
        [Category("绑定信息")]
        [PropertySortOrder(134)]
        [Browsable(false)]
        public string ARailPreheatRight_TemperatureRange { get { return _aRailPreheatRight_TemperatureRange; } set { SetProperty(ref _aRailPreheatRight_TemperatureRange, value); } }

        private string _aRailPreheatRight_IdleCloseInterval = "ARailPreheatRight_IdleCloseInterval";
        [DisplayName("A轨预热-右_空闲关闭间隔")]
        [Category("绑定信息")]
        [PropertySortOrder(135)]
        [Browsable(false)]
        public string ARailPreheatRight_IdleCloseInterval { get { return _aRailPreheatRight_IdleCloseInterval; } set { SetProperty(ref _aRailPreheatRight_IdleCloseInterval, value); } }

        private string _aRailPreheatRight_PreheatTime = "ARailPreheatRight_PreheatTime";
        [DisplayName("A轨预热-右_预热时间")]
        [Category("绑定信息")]
        [PropertySortOrder(136)]
        [Browsable(false)]
        public string ARailPreheatRight_PreheatTime { get { return _aRailPreheatRight_PreheatTime; } set { SetProperty(ref _aRailPreheatRight_PreheatTime, value); } }

        private string _aRailPreheatRight_HeatingTime = "ARailPreheatRight_HeatingTime";
        [DisplayName("A轨预热-右_加热时间")]
        [Category("绑定信息")]
        [PropertySortOrder(137)]
        [Browsable(false)]
        public string ARailPreheatRight_HeatingTime { get { return _aRailPreheatRight_HeatingTime; } set { SetProperty(ref _aRailPreheatRight_HeatingTime, value); } }

        private string _aRailPreheatRight_DetectionEnabled = "ARailPreheatRight_DetectionEnabled";
        [DisplayName("A轨预热-右_检测开启")]
        [Category("绑定信息")]
        [PropertySortOrder(138)]
        [Browsable(false)]
        public string ARailPreheatRight_DetectionEnabled { get { return _aRailPreheatRight_DetectionEnabled; } set { SetProperty(ref _aRailPreheatRight_DetectionEnabled, value); } }

        private string _aRailPreheatRight_WorkEnabled = "ARailPreheatRight_WorkEnabled";
        [DisplayName("A轨预热-右_工作开启")]
        [Category("绑定信息")]
        [PropertySortOrder(139)]
        [Browsable(false)]
        public string ARailPreheatRight_WorkEnabled { get { return _aRailPreheatRight_WorkEnabled; } set { SetProperty(ref _aRailPreheatRight_WorkEnabled, value); } }

        private string _aRailPreheatRight_DelayTime = "ARailPreheatRight_DelayTime";
        [DisplayName("A轨预热-右_延迟时间")]
        [Category("绑定信息")]
        [PropertySortOrder(140)]
        [Browsable(false)]
        public string ARailPreheatRight_DelayTime { get { return _aRailPreheatRight_DelayTime; } set { SetProperty(ref _aRailPreheatRight_DelayTime, value); } }

        private string _aRailPreheatRight2 = nameof(HeatingControlPropertyModelEdit.ARailPreheatRight2);
        [DisplayName("A轨预热-右(2)")]
        [Category("绑定信息")]
        [PropertySortOrder(34)]
        [Browsable(false)]
        public string ARailPreheatRight2 { get { return _aRailPreheatRight2; } set { SetProperty(ref _aRailPreheatRight2, value); } }

        private string _aRailPreheatRight2_Switch = "ARailPreheatRight2_Switch";
        [DisplayName("A轨预热-右(2)_关/开")]
        [Category("绑定信息")]
        [PropertySortOrder(141)]
        [Browsable(false)]
        public string ARailPreheatRight2_Switch { get { return _aRailPreheatRight2_Switch; } set { SetProperty(ref _aRailPreheatRight2_Switch, value); } }

        private string _aRailPreheatRight2_CurrentTemperature = "ARailPreheatRight2_CurrentTemperature";
        [DisplayName("A轨预热-右(2)_当前温度")]
        [Category("绑定信息")]
        [PropertySortOrder(59)]
        [Browsable(false)]
        public string ARailPreheatRight2_CurrentTemperature { get { return _aRailPreheatRight2_CurrentTemperature; } set { SetProperty(ref _aRailPreheatRight2_CurrentTemperature, value); } }

        private string _aRailPreheatRight2_SetTemperature = "ARailPreheatRight2_SetTemperature";
        [DisplayName("A轨预热-右(2)_设定温度")]
        [Category("绑定信息")]
        [PropertySortOrder(60)]
        [Browsable(false)]
        public string ARailPreheatRight2_SetTemperature { get { return _aRailPreheatRight2_SetTemperature; } set { SetProperty(ref _aRailPreheatRight2_SetTemperature, value); } }

        private string _aRailPreheatRight2_TemperatureRange = "ARailPreheatRight2_TemperatureRange";
        [DisplayName("A轨预热-右(2)_温度范围")]
        [Category("绑定信息")]
        [PropertySortOrder(142)]
        [Browsable(false)]
        public string ARailPreheatRight2_TemperatureRange { get { return _aRailPreheatRight2_TemperatureRange; } set { SetProperty(ref _aRailPreheatRight2_TemperatureRange, value); } }

        private string _aRailPreheatRight2_IdleCloseInterval = "ARailPreheatRight2_IdleCloseInterval";
        [DisplayName("A轨预热-右(2)_空闲关闭间隔")]
        [Category("绑定信息")]
        [PropertySortOrder(143)]
        [Browsable(false)]
        public string ARailPreheatRight2_IdleCloseInterval { get { return _aRailPreheatRight2_IdleCloseInterval; } set { SetProperty(ref _aRailPreheatRight2_IdleCloseInterval, value); } }

        private string _aRailPreheatRight2_PreheatTime = "ARailPreheatRight2_PreheatTime";
        [DisplayName("A轨预热-右(2)_预热时间")]
        [Category("绑定信息")]
        [PropertySortOrder(144)]
        [Browsable(false)]
        public string ARailPreheatRight2_PreheatTime { get { return _aRailPreheatRight2_PreheatTime; } set { SetProperty(ref _aRailPreheatRight2_PreheatTime, value); } }

        private string _aRailPreheatRight2_HeatingTime = "ARailPreheatRight2_HeatingTime";
        [DisplayName("A轨预热-右(2)_加热时间")]
        [Category("绑定信息")]
        [PropertySortOrder(145)]
        [Browsable(false)]
        public string ARailPreheatRight2_HeatingTime { get { return _aRailPreheatRight2_HeatingTime; } set { SetProperty(ref _aRailPreheatRight2_HeatingTime, value); } }

        private string _aRailPreheatRight2_DetectionEnabled = "ARailPreheatRight2_DetectionEnabled";
        [DisplayName("A轨预热-右(2)_检测开启")]
        [Category("绑定信息")]
        [PropertySortOrder(146)]
        [Browsable(false)]
        public string ARailPreheatRight2_DetectionEnabled { get { return _aRailPreheatRight2_DetectionEnabled; } set { SetProperty(ref _aRailPreheatRight2_DetectionEnabled, value); } }

        private string _aRailPreheatRight2_WorkEnabled = "ARailPreheatRight2_WorkEnabled";
        [DisplayName("A轨预热-右(2)_工作开启")]
        [Category("绑定信息")]
        [PropertySortOrder(147)]
        [Browsable(false)]
        public string ARailPreheatRight2_WorkEnabled { get { return _aRailPreheatRight2_WorkEnabled; } set { SetProperty(ref _aRailPreheatRight2_WorkEnabled, value); } }

        private string _aRailPreheatRight2_DelayTime = "ARailPreheatRight2_DelayTime";
        [DisplayName("A轨预热-右(2)_延迟时间")]
        [Category("绑定信息")]
        [PropertySortOrder(148)]
        [Browsable(false)]
        public string ARailPreheatRight2_DelayTime { get { return _aRailPreheatRight2_DelayTime; } set { SetProperty(ref _aRailPreheatRight2_DelayTime, value); } }

        private string _bRailPreheatLeft = nameof(HeatingControlPropertyModelEdit.BRailPreheatLeft);
        [DisplayName("B轨预热-左")]
        [Category("绑定信息")]
        [PropertySortOrder(35)]
        [Browsable(false)]
        public string BRailPreheatLeft { get { return _bRailPreheatLeft; } set { SetProperty(ref _bRailPreheatLeft, value); } }

        private string _bRailPreheatLeft_Switch = "BRailPreheatLeft_Switch";
        [DisplayName("B轨预热-左_关/开")]
        [Category("绑定信息")]
        [PropertySortOrder(149)]
        [Browsable(false)]
        public string BRailPreheatLeft_Switch { get { return _bRailPreheatLeft_Switch; } set { SetProperty(ref _bRailPreheatLeft_Switch, value); } }

        private string _bRailPreheatLeft_CurrentTemperature = "BRailPreheatLeft_CurrentTemperature";
        [DisplayName("B轨预热-左_当前温度")]
        [Category("绑定信息")]
        [PropertySortOrder(61)]
        [Browsable(false)]
        public string BRailPreheatLeft_CurrentTemperature { get { return _bRailPreheatLeft_CurrentTemperature; } set { SetProperty(ref _bRailPreheatLeft_CurrentTemperature, value); } }

        private string _bRailPreheatLeft_SetTemperature = "BRailPreheatLeft_SetTemperature";
        [DisplayName("B轨预热-左_设定温度")]
        [Category("绑定信息")]
        [PropertySortOrder(62)]
        [Browsable(false)]
        public string BRailPreheatLeft_SetTemperature { get { return _bRailPreheatLeft_SetTemperature; } set { SetProperty(ref _bRailPreheatLeft_SetTemperature, value); } }

        private string _bRailPreheatLeft_TemperatureRange = "BRailPreheatLeft_TemperatureRange";
        [DisplayName("B轨预热-左_温度范围")]
        [Category("绑定信息")]
        [PropertySortOrder(150)]
        [Browsable(false)]
        public string BRailPreheatLeft_TemperatureRange { get { return _bRailPreheatLeft_TemperatureRange; } set { SetProperty(ref _bRailPreheatLeft_TemperatureRange, value); } }

        private string _bRailPreheatLeft_IdleCloseInterval = "BRailPreheatLeft_IdleCloseInterval";
        [DisplayName("B轨预热-左_空闲关闭间隔")]
        [Category("绑定信息")]
        [PropertySortOrder(151)]
        [Browsable(false)]
        public string BRailPreheatLeft_IdleCloseInterval { get { return _bRailPreheatLeft_IdleCloseInterval; } set { SetProperty(ref _bRailPreheatLeft_IdleCloseInterval, value); } }

        private string _bRailPreheatLeft_PreheatTime = "BRailPreheatLeft_PreheatTime";
        [DisplayName("B轨预热-左_预热时间")]
        [Category("绑定信息")]
        [PropertySortOrder(152)]
        [Browsable(false)]
        public string BRailPreheatLeft_PreheatTime { get { return _bRailPreheatLeft_PreheatTime; } set { SetProperty(ref _bRailPreheatLeft_PreheatTime, value); } }

        private string _bRailPreheatLeft_HeatingTime = "BRailPreheatLeft_HeatingTime";
        [DisplayName("B轨预热-左_加热时间")]
        [Category("绑定信息")]
        [PropertySortOrder(153)]
        [Browsable(false)]
        public string BRailPreheatLeft_HeatingTime { get { return _bRailPreheatLeft_HeatingTime; } set { SetProperty(ref _bRailPreheatLeft_HeatingTime, value); } }

        private string _bRailPreheatLeft_DetectionEnabled = "BRailPreheatLeft_DetectionEnabled";
        [DisplayName("B轨预热-左_检测开启")]
        [Category("绑定信息")]
        [PropertySortOrder(154)]
        [Browsable(false)]
        public string BRailPreheatLeft_DetectionEnabled { get { return _bRailPreheatLeft_DetectionEnabled; } set { SetProperty(ref _bRailPreheatLeft_DetectionEnabled, value); } }

        private string _bRailPreheatLeft_WorkEnabled = "BRailPreheatLeft_WorkEnabled";
        [DisplayName("B轨预热-左_工作开启")]
        [Category("绑定信息")]
        [PropertySortOrder(155)]
        [Browsable(false)]
        public string BRailPreheatLeft_WorkEnabled { get { return _bRailPreheatLeft_WorkEnabled; } set { SetProperty(ref _bRailPreheatLeft_WorkEnabled, value); } }

        private string _bRailPreheatLeft_DelayTime = "BRailPreheatLeft_DelayTime";
        [DisplayName("B轨预热-左_延迟时间")]
        [Category("绑定信息")]
        [PropertySortOrder(156)]
        [Browsable(false)]
        public string BRailPreheatLeft_DelayTime { get { return _bRailPreheatLeft_DelayTime; } set { SetProperty(ref _bRailPreheatLeft_DelayTime, value); } }

        private string _bRailPreheatLeft2 = nameof(HeatingControlPropertyModelEdit.BRailPreheatLeft2);
        [DisplayName("B轨预热-左(2)")]
        [Category("绑定信息")]
        [PropertySortOrder(36)]
        [Browsable(false)]
        public string BRailPreheatLeft2 { get { return _bRailPreheatLeft2; } set { SetProperty(ref _bRailPreheatLeft2, value); } }

        private string _bRailPreheatLeft2_Switch = "BRailPreheatLeft2_Switch";
        [DisplayName("B轨预热-左(2)_关/开")]
        [Category("绑定信息")]
        [PropertySortOrder(157)]
        [Browsable(false)]
        public string BRailPreheatLeft2_Switch { get { return _bRailPreheatLeft2_Switch; } set { SetProperty(ref _bRailPreheatLeft2_Switch, value); } }

        private string _bRailPreheatLeft2_CurrentTemperature = "BRailPreheatLeft2_CurrentTemperature";
        [DisplayName("B轨预热-左(2)_当前温度")]
        [Category("绑定信息")]
        [PropertySortOrder(63)]
        [Browsable(false)]
        public string BRailPreheatLeft2_CurrentTemperature { get { return _bRailPreheatLeft2_CurrentTemperature; } set { SetProperty(ref _bRailPreheatLeft2_CurrentTemperature, value); } }

        private string _bRailPreheatLeft2_SetTemperature = "BRailPreheatLeft2_SetTemperature";
        [DisplayName("B轨预热-左(2)_设定温度")]
        [Category("绑定信息")]
        [PropertySortOrder(64)]
        [Browsable(false)]
        public string BRailPreheatLeft2_SetTemperature { get { return _bRailPreheatLeft2_SetTemperature; } set { SetProperty(ref _bRailPreheatLeft2_SetTemperature, value); } }

        private string _bRailPreheatLeft2_TemperatureRange = "BRailPreheatLeft2_TemperatureRange";
        [DisplayName("B轨预热-左(2)_温度范围")]
        [Category("绑定信息")]
        [PropertySortOrder(158)]
        [Browsable(false)]
        public string BRailPreheatLeft2_TemperatureRange { get { return _bRailPreheatLeft2_TemperatureRange; } set { SetProperty(ref _bRailPreheatLeft2_TemperatureRange, value); } }

        private string _bRailPreheatLeft2_IdleCloseInterval = "BRailPreheatLeft2_IdleCloseInterval";
        [DisplayName("B轨预热-左(2)_空闲关闭间隔")]
        [Category("绑定信息")]
        [PropertySortOrder(159)]
        [Browsable(false)]
        public string BRailPreheatLeft2_IdleCloseInterval { get { return _bRailPreheatLeft2_IdleCloseInterval; } set { SetProperty(ref _bRailPreheatLeft2_IdleCloseInterval, value); } }

        private string _bRailPreheatLeft2_PreheatTime = "BRailPreheatLeft2_PreheatTime";
        [DisplayName("B轨预热-左(2)_预热时间")]
        [Category("绑定信息")]
        [PropertySortOrder(160)]
        [Browsable(false)]
        public string BRailPreheatLeft2_PreheatTime { get { return _bRailPreheatLeft2_PreheatTime; } set { SetProperty(ref _bRailPreheatLeft2_PreheatTime, value); } }

        private string _bRailPreheatLeft2_HeatingTime = "BRailPreheatLeft2_HeatingTime";
        [DisplayName("B轨预热-左(2)_加热时间")]
        [Category("绑定信息")]
        [PropertySortOrder(161)]
        [Browsable(false)]
        public string BRailPreheatLeft2_HeatingTime { get { return _bRailPreheatLeft2_HeatingTime; } set { SetProperty(ref _bRailPreheatLeft2_HeatingTime, value); } }

        private string _bRailPreheatLeft2_DetectionEnabled = "BRailPreheatLeft2_DetectionEnabled";
        [DisplayName("B轨预热-左(2)_检测开启")]
        [Category("绑定信息")]
        [PropertySortOrder(162)]
        [Browsable(false)]
        public string BRailPreheatLeft2_DetectionEnabled { get { return _bRailPreheatLeft2_DetectionEnabled; } set { SetProperty(ref _bRailPreheatLeft2_DetectionEnabled, value); } }

        private string _bRailPreheatLeft2_WorkEnabled = "BRailPreheatLeft2_WorkEnabled";
        [DisplayName("B轨预热-左(2)_工作开启")]
        [Category("绑定信息")]
        [PropertySortOrder(163)]
        [Browsable(false)]
        public string BRailPreheatLeft2_WorkEnabled { get { return _bRailPreheatLeft2_WorkEnabled; } set { SetProperty(ref _bRailPreheatLeft2_WorkEnabled, value); } }

        private string _bRailPreheatLeft2_DelayTime = "BRailPreheatLeft2_DelayTime";
        [DisplayName("B轨预热-左(2)_延迟时间")]
        [Category("绑定信息")]
        [PropertySortOrder(164)]
        [Browsable(false)]
        public string BRailPreheatLeft2_DelayTime { get { return _bRailPreheatLeft2_DelayTime; } set { SetProperty(ref _bRailPreheatLeft2_DelayTime, value); } }

        private string _bRailGlueBoardStationMiddle = nameof(HeatingControlPropertyModelEdit.BRailGlueBoardStationMiddle);
        [DisplayName("B轨胶板工位-中")]
        [Category("绑定信息")]
        [PropertySortOrder(37)]
        [Browsable(false)]
        public string BRailGlueBoardStationMiddle { get { return _bRailGlueBoardStationMiddle; } set { SetProperty(ref _bRailGlueBoardStationMiddle, value); } }

        private string _bRailGlueBoardStationMiddle_Switch = "BRailGlueBoardStationMiddle_Switch";
        [DisplayName("B轨胶板工位-中_关/开")]
        [Category("绑定信息")]
        [PropertySortOrder(165)]
        [Browsable(false)]
        public string BRailGlueBoardStationMiddle_Switch { get { return _bRailGlueBoardStationMiddle_Switch; } set { SetProperty(ref _bRailGlueBoardStationMiddle_Switch, value); } }

        private string _bRailGlueBoardStationMiddle_CurrentTemperature = "BRailGlueBoardStationMiddle_CurrentTemperature";
        [DisplayName("B轨胶板工位-中_当前温度")]
        [Category("绑定信息")]
        [PropertySortOrder(65)]
        [Browsable(false)]
        public string BRailGlueBoardStationMiddle_CurrentTemperature { get { return _bRailGlueBoardStationMiddle_CurrentTemperature; } set { SetProperty(ref _bRailGlueBoardStationMiddle_CurrentTemperature, value); } }

        private string _bRailGlueBoardStationMiddle_SetTemperature = "BRailGlueBoardStationMiddle_SetTemperature";
        [DisplayName("B轨胶板工位-中_设定温度")]
        [Category("绑定信息")]
        [PropertySortOrder(66)]
        [Browsable(false)]
        public string BRailGlueBoardStationMiddle_SetTemperature { get { return _bRailGlueBoardStationMiddle_SetTemperature; } set { SetProperty(ref _bRailGlueBoardStationMiddle_SetTemperature, value); } }

        private string _bRailGlueBoardStationMiddle_TemperatureRange = "BRailGlueBoardStationMiddle_TemperatureRange";
        [DisplayName("B轨胶板工位-中_温度范围")]
        [Category("绑定信息")]
        [PropertySortOrder(166)]
        [Browsable(false)]
        public string BRailGlueBoardStationMiddle_TemperatureRange { get { return _bRailGlueBoardStationMiddle_TemperatureRange; } set { SetProperty(ref _bRailGlueBoardStationMiddle_TemperatureRange, value); } }

        private string _bRailGlueBoardStationMiddle_IdleCloseInterval = "BRailGlueBoardStationMiddle_IdleCloseInterval";
        [DisplayName("B轨胶板工位-中_空闲关闭间隔")]
        [Category("绑定信息")]
        [PropertySortOrder(167)]
        [Browsable(false)]
        public string BRailGlueBoardStationMiddle_IdleCloseInterval { get { return _bRailGlueBoardStationMiddle_IdleCloseInterval; } set { SetProperty(ref _bRailGlueBoardStationMiddle_IdleCloseInterval, value); } }

        private string _bRailGlueBoardStationMiddle_PreheatTime = "BRailGlueBoardStationMiddle_PreheatTime";
        [DisplayName("B轨胶板工位-中_预热时间")]
        [Category("绑定信息")]
        [PropertySortOrder(168)]
        [Browsable(false)]
        public string BRailGlueBoardStationMiddle_PreheatTime { get { return _bRailGlueBoardStationMiddle_PreheatTime; } set { SetProperty(ref _bRailGlueBoardStationMiddle_PreheatTime, value); } }

        private string _bRailGlueBoardStationMiddle_HeatingTime = "BRailGlueBoardStationMiddle_HeatingTime";
        [DisplayName("B轨胶板工位-中_加热时间")]
        [Category("绑定信息")]
        [PropertySortOrder(169)]
        [Browsable(false)]
        public string BRailGlueBoardStationMiddle_HeatingTime { get { return _bRailGlueBoardStationMiddle_HeatingTime; } set { SetProperty(ref _bRailGlueBoardStationMiddle_HeatingTime, value); } }

        private string _bRailGlueBoardStationMiddle_DetectionEnabled = "BRailGlueBoardStationMiddle_DetectionEnabled";
        [DisplayName("B轨胶板工位-中_检测开启")]
        [Category("绑定信息")]
        [PropertySortOrder(170)]
        [Browsable(false)]
        public string BRailGlueBoardStationMiddle_DetectionEnabled { get { return _bRailGlueBoardStationMiddle_DetectionEnabled; } set { SetProperty(ref _bRailGlueBoardStationMiddle_DetectionEnabled, value); } }

        private string _bRailGlueBoardStationMiddle_WorkEnabled = "BRailGlueBoardStationMiddle_WorkEnabled";
        [DisplayName("B轨胶板工位-中_工作开启")]
        [Category("绑定信息")]
        [PropertySortOrder(171)]
        [Browsable(false)]
        public string BRailGlueBoardStationMiddle_WorkEnabled { get { return _bRailGlueBoardStationMiddle_WorkEnabled; } set { SetProperty(ref _bRailGlueBoardStationMiddle_WorkEnabled, value); } }

        private string _bRailGlueBoardStationMiddle_DelayTime = "BRailGlueBoardStationMiddle_DelayTime";
        [DisplayName("B轨胶板工位-中_延迟时间")]
        [Category("绑定信息")]
        [PropertySortOrder(172)]
        [Browsable(false)]
        public string BRailGlueBoardStationMiddle_DelayTime { get { return _bRailGlueBoardStationMiddle_DelayTime; } set { SetProperty(ref _bRailGlueBoardStationMiddle_DelayTime, value); } }

        private string _bRailDispensingStationMiddle2 = nameof(HeatingControlPropertyModelEdit.BRailDispensingStationMiddle2);
        [DisplayName("B轨点胶工位-中(2)")]
        [Category("绑定信息")]
        [PropertySortOrder(38)]
        [Browsable(false)]
        public string BRailDispensingStationMiddle2 { get { return _bRailDispensingStationMiddle2; } set { SetProperty(ref _bRailDispensingStationMiddle2, value); } }

        private string _bRailDispensingStationMiddle2_Switch = "BRailDispensingStationMiddle2_Switch";
        [DisplayName("B轨点胶工位-中(2)_关/开")]
        [Category("绑定信息")]
        [PropertySortOrder(173)]
        [Browsable(false)]
        public string BRailDispensingStationMiddle2_Switch { get { return _bRailDispensingStationMiddle2_Switch; } set { SetProperty(ref _bRailDispensingStationMiddle2_Switch, value); } }

        private string _bRailDispensingStationMiddle2_CurrentTemperature = "BRailDispensingStationMiddle2_CurrentTemperature";
        [DisplayName("B轨点胶工位-中(2)_当前温度")]
        [Category("绑定信息")]
        [PropertySortOrder(67)]
        [Browsable(false)]
        public string BRailDispensingStationMiddle2_CurrentTemperature { get { return _bRailDispensingStationMiddle2_CurrentTemperature; } set { SetProperty(ref _bRailDispensingStationMiddle2_CurrentTemperature, value); } }

        private string _bRailDispensingStationMiddle2_SetTemperature = "BRailDispensingStationMiddle2_SetTemperature";
        [DisplayName("B轨点胶工位-中(2)_设定温度")]
        [Category("绑定信息")]
        [PropertySortOrder(68)]
        [Browsable(false)]
        public string BRailDispensingStationMiddle2_SetTemperature { get { return _bRailDispensingStationMiddle2_SetTemperature; } set { SetProperty(ref _bRailDispensingStationMiddle2_SetTemperature, value); } }

        private string _bRailDispensingStationMiddle2_TemperatureRange = "BRailDispensingStationMiddle2_TemperatureRange";
        [DisplayName("B轨点胶工位-中(2)_温度范围")]
        [Category("绑定信息")]
        [PropertySortOrder(174)]
        [Browsable(false)]
        public string BRailDispensingStationMiddle2_TemperatureRange { get { return _bRailDispensingStationMiddle2_TemperatureRange; } set { SetProperty(ref _bRailDispensingStationMiddle2_TemperatureRange, value); } }

        private string _bRailDispensingStationMiddle2_IdleCloseInterval = "BRailDispensingStationMiddle2_IdleCloseInterval";
        [DisplayName("B轨点胶工位-中(2)_空闲关闭间隔")]
        [Category("绑定信息")]
        [PropertySortOrder(175)]
        [Browsable(false)]
        public string BRailDispensingStationMiddle2_IdleCloseInterval { get { return _bRailDispensingStationMiddle2_IdleCloseInterval; } set { SetProperty(ref _bRailDispensingStationMiddle2_IdleCloseInterval, value); } }

        private string _bRailDispensingStationMiddle2_PreheatTime = "BRailDispensingStationMiddle2_PreheatTime";
        [DisplayName("B轨点胶工位-中(2)_预热时间")]
        [Category("绑定信息")]
        [PropertySortOrder(176)]
        [Browsable(false)]
        public string BRailDispensingStationMiddle2_PreheatTime { get { return _bRailDispensingStationMiddle2_PreheatTime; } set { SetProperty(ref _bRailDispensingStationMiddle2_PreheatTime, value); } }

        private string _bRailDispensingStationMiddle2_HeatingTime = "BRailDispensingStationMiddle2_HeatingTime";
        [DisplayName("B轨点胶工位-中(2)_加热时间")]
        [Category("绑定信息")]
        [PropertySortOrder(177)]
        [Browsable(false)]
        public string BRailDispensingStationMiddle2_HeatingTime { get { return _bRailDispensingStationMiddle2_HeatingTime; } set { SetProperty(ref _bRailDispensingStationMiddle2_HeatingTime, value); } }

        private string _bRailDispensingStationMiddle2_DetectionEnabled = "BRailDispensingStationMiddle2_DetectionEnabled";
        [DisplayName("B轨点胶工位-中(2)_检测开启")]
        [Category("绑定信息")]
        [PropertySortOrder(178)]
        [Browsable(false)]
        public string BRailDispensingStationMiddle2_DetectionEnabled { get { return _bRailDispensingStationMiddle2_DetectionEnabled; } set { SetProperty(ref _bRailDispensingStationMiddle2_DetectionEnabled, value); } }

        private string _bRailDispensingStationMiddle2_WorkEnabled = "BRailDispensingStationMiddle2_WorkEnabled";
        [DisplayName("B轨点胶工位-中(2)_工作开启")]
        [Category("绑定信息")]
        [PropertySortOrder(179)]
        [Browsable(false)]
        public string BRailDispensingStationMiddle2_WorkEnabled { get { return _bRailDispensingStationMiddle2_WorkEnabled; } set { SetProperty(ref _bRailDispensingStationMiddle2_WorkEnabled, value); } }

        private string _bRailDispensingStationMiddle2_DelayTime = "BRailDispensingStationMiddle2_DelayTime";
        [DisplayName("B轨点胶工位-中(2)_延迟时间")]
        [Category("绑定信息")]
        [PropertySortOrder(180)]
        [Browsable(false)]
        public string BRailDispensingStationMiddle2_DelayTime { get { return _bRailDispensingStationMiddle2_DelayTime; } set { SetProperty(ref _bRailDispensingStationMiddle2_DelayTime, value); } }

        private string _bRailPreheatRight = nameof(HeatingControlPropertyModelEdit.BRailPreheatRight);
        [DisplayName("B轨预热-右")]
        [Category("绑定信息")]
        [PropertySortOrder(39)]
        [Browsable(false)]
        public string BRailPreheatRight { get { return _bRailPreheatRight; } set { SetProperty(ref _bRailPreheatRight, value); } }

        private string _bRailPreheatRight_Switch = "BRailPreheatRight_Switch";
        [DisplayName("B轨预热-右_关/开")]
        [Category("绑定信息")]
        [PropertySortOrder(181)]
        [Browsable(false)]
        public string BRailPreheatRight_Switch { get { return _bRailPreheatRight_Switch; } set { SetProperty(ref _bRailPreheatRight_Switch, value); } }

        private string _bRailPreheatRight_CurrentTemperature = "BRailPreheatRight_CurrentTemperature";
        [DisplayName("B轨预热-右_当前温度")]
        [Category("绑定信息")]
        [PropertySortOrder(69)]
        [Browsable(false)]
        public string BRailPreheatRight_CurrentTemperature { get { return _bRailPreheatRight_CurrentTemperature; } set { SetProperty(ref _bRailPreheatRight_CurrentTemperature, value); } }

        private string _bRailPreheatRight_SetTemperature = "BRailPreheatRight_SetTemperature";
        [DisplayName("B轨预热-右_设定温度")]
        [Category("绑定信息")]
        [PropertySortOrder(70)]
        [Browsable(false)]
        public string BRailPreheatRight_SetTemperature { get { return _bRailPreheatRight_SetTemperature; } set { SetProperty(ref _bRailPreheatRight_SetTemperature, value); } }

        private string _bRailPreheatRight_TemperatureRange = "BRailPreheatRight_TemperatureRange";
        [DisplayName("B轨预热-右_温度范围")]
        [Category("绑定信息")]
        [PropertySortOrder(182)]
        [Browsable(false)]
        public string BRailPreheatRight_TemperatureRange { get { return _bRailPreheatRight_TemperatureRange; } set { SetProperty(ref _bRailPreheatRight_TemperatureRange, value); } }

        private string _bRailPreheatRight_IdleCloseInterval = "BRailPreheatRight_IdleCloseInterval";
        [DisplayName("B轨预热-右_空闲关闭间隔")]
        [Category("绑定信息")]
        [PropertySortOrder(183)]
        [Browsable(false)]
        public string BRailPreheatRight_IdleCloseInterval { get { return _bRailPreheatRight_IdleCloseInterval; } set { SetProperty(ref _bRailPreheatRight_IdleCloseInterval, value); } }

        private string _bRailPreheatRight_PreheatTime = "BRailPreheatRight_PreheatTime";
        [DisplayName("B轨预热-右_预热时间")]
        [Category("绑定信息")]
        [PropertySortOrder(184)]
        [Browsable(false)]
        public string BRailPreheatRight_PreheatTime { get { return _bRailPreheatRight_PreheatTime; } set { SetProperty(ref _bRailPreheatRight_PreheatTime, value); } }

        private string _bRailPreheatRight_HeatingTime = "BRailPreheatRight_HeatingTime";
        [DisplayName("B轨预热-右_加热时间")]
        [Category("绑定信息")]
        [PropertySortOrder(185)]
        [Browsable(false)]
        public string BRailPreheatRight_HeatingTime { get { return _bRailPreheatRight_HeatingTime; } set { SetProperty(ref _bRailPreheatRight_HeatingTime, value); } }

        private string _bRailPreheatRight_DetectionEnabled = "BRailPreheatRight_DetectionEnabled";
        [DisplayName("B轨预热-右_检测开启")]
        [Category("绑定信息")]
        [PropertySortOrder(186)]
        [Browsable(false)]
        public string BRailPreheatRight_DetectionEnabled { get { return _bRailPreheatRight_DetectionEnabled; } set { SetProperty(ref _bRailPreheatRight_DetectionEnabled, value); } }

        private string _bRailPreheatRight_WorkEnabled = "BRailPreheatRight_WorkEnabled";
        [DisplayName("B轨预热-右_工作开启")]
        [Category("绑定信息")]
        [PropertySortOrder(187)]
        [Browsable(false)]
        public string BRailPreheatRight_WorkEnabled { get { return _bRailPreheatRight_WorkEnabled; } set { SetProperty(ref _bRailPreheatRight_WorkEnabled, value); } }

        private string _bRailPreheatRight_DelayTime = "BRailPreheatRight_DelayTime";
        [DisplayName("B轨预热-右_延迟时间")]
        [Category("绑定信息")]
        [PropertySortOrder(188)]
        [Browsable(false)]
        public string BRailPreheatRight_DelayTime { get { return _bRailPreheatRight_DelayTime; } set { SetProperty(ref _bRailPreheatRight_DelayTime, value); } }

        private string _bRailPreheatRight2 = nameof(HeatingControlPropertyModelEdit.BRailPreheatRight2);
        [DisplayName("B轨预热-右(2)")]
        [Category("绑定信息")]
        [PropertySortOrder(40)]
        [Browsable(false)]
        public string BRailPreheatRight2 { get { return _bRailPreheatRight2; } set { SetProperty(ref _bRailPreheatRight2, value); } }

        private string _bRailPreheatRight2_Switch = "BRailPreheatRight2_Switch";
        [DisplayName("B轨预热-右(2)_关/开")]
        [Category("绑定信息")]
        [PropertySortOrder(189)]
        [Browsable(false)]
        public string BRailPreheatRight2_Switch { get { return _bRailPreheatRight2_Switch; } set { SetProperty(ref _bRailPreheatRight2_Switch, value); } }

        private string _bRailPreheatRight2_CurrentTemperature = "BRailPreheatRight2_CurrentTemperature";
        [DisplayName("B轨预热-右(2)_当前温度")]
        [Category("绑定信息")]
        [PropertySortOrder(71)]
        [Browsable(false)]
        public string BRailPreheatRight2_CurrentTemperature { get { return _bRailPreheatRight2_CurrentTemperature; } set { SetProperty(ref _bRailPreheatRight2_CurrentTemperature, value); } }

        private string _bRailPreheatRight2_SetTemperature = "BRailPreheatRight2_SetTemperature";
        [DisplayName("B轨预热-右(2)_设定温度")]
        [Category("绑定信息")]
        [PropertySortOrder(72)]
        [Browsable(false)]
        public string BRailPreheatRight2_SetTemperature { get { return _bRailPreheatRight2_SetTemperature; } set { SetProperty(ref _bRailPreheatRight2_SetTemperature, value); } }

        private string _bRailPreheatRight2_TemperatureRange = "BRailPreheatRight2_TemperatureRange";
        [DisplayName("B轨预热-右(2)_温度范围")]
        [Category("绑定信息")]
        [PropertySortOrder(190)]
        [Browsable(false)]
        public string BRailPreheatRight2_TemperatureRange { get { return _bRailPreheatRight2_TemperatureRange; } set { SetProperty(ref _bRailPreheatRight2_TemperatureRange, value); } }

        private string _bRailPreheatRight2_IdleCloseInterval = "BRailPreheatRight2_IdleCloseInterval";
        [DisplayName("B轨预热-右(2)_空闲关闭间隔")]
        [Category("绑定信息")]
        [PropertySortOrder(191)]
        [Browsable(false)]
        public string BRailPreheatRight2_IdleCloseInterval { get { return _bRailPreheatRight2_IdleCloseInterval; } set { SetProperty(ref _bRailPreheatRight2_IdleCloseInterval, value); } }

        private string _bRailPreheatRight2_PreheatTime = "BRailPreheatRight2_PreheatTime";
        [DisplayName("B轨预热-右(2)_预热时间")]
        [Category("绑定信息")]
        [PropertySortOrder(192)]
        [Browsable(false)]
        public string BRailPreheatRight2_PreheatTime { get { return _bRailPreheatRight2_PreheatTime; } set { SetProperty(ref _bRailPreheatRight2_PreheatTime, value); } }

        private string _bRailPreheatRight2_HeatingTime = "BRailPreheatRight2_HeatingTime";
        [DisplayName("B轨预热-右(2)_加热时间")]
        [Category("绑定信息")]
        [PropertySortOrder(193)]
        [Browsable(false)]
        public string BRailPreheatRight2_HeatingTime { get { return _bRailPreheatRight2_HeatingTime; } set { SetProperty(ref _bRailPreheatRight2_HeatingTime, value); } }

        private string _bRailPreheatRight2_DetectionEnabled = "BRailPreheatRight2_DetectionEnabled";
        [DisplayName("B轨预热-右(2)_检测开启")]
        [Category("绑定信息")]
        [PropertySortOrder(194)]
        [Browsable(false)]
        public string BRailPreheatRight2_DetectionEnabled { get { return _bRailPreheatRight2_DetectionEnabled; } set { SetProperty(ref _bRailPreheatRight2_DetectionEnabled, value); } }

        private string _bRailPreheatRight2_WorkEnabled = "BRailPreheatRight2_WorkEnabled";
        [DisplayName("B轨预热-右(2)_工作开启")]
        [Category("绑定信息")]
        [PropertySortOrder(195)]
        [Browsable(false)]
        public string BRailPreheatRight2_WorkEnabled { get { return _bRailPreheatRight2_WorkEnabled; } set { SetProperty(ref _bRailPreheatRight2_WorkEnabled, value); } }

        private string _bRailPreheatRight2_DelayTime = "BRailPreheatRight2_DelayTime";
        [DisplayName("B轨预热-右(2)_延迟时间")]
        [Category("绑定信息")]
        [PropertySortOrder(196)]
        [Browsable(false)]
        public string BRailPreheatRight2_DelayTime { get { return _bRailPreheatRight2_DelayTime; } set { SetProperty(ref _bRailPreheatRight2_DelayTime, value); } }


        public void CopyFrom(HeatingControlPropertyBindEditModel source)
        {
            if (source == null)
                return;

            string CopyOrDefault(string v, string def) => string.IsNullOrWhiteSpace(v) ? def : v;

            base.CopyFrom(source);
            TextFont = CopyOrDefault(source.TextFont, nameof(HeatingControlPropertyModelEdit.TextFont));
            TextColor = CopyOrDefault(source.TextColor, nameof(HeatingControlPropertyModelEdit.TextColor));
            BackColor = CopyOrDefault(source.BackColor, nameof(HeatingControlPropertyModelEdit.BackColor));
            IsDualValve = CopyOrDefault(source.IsDualValve, nameof(HeatingControlPropertyModelEdit.IsDualValve));
            IsDualTrack = CopyOrDefault(source.IsDualTrack, nameof(HeatingControlPropertyModelEdit.IsDualTrack));
            RightValveDispensingHead = CopyOrDefault(source.RightValveDispensingHead, nameof(HeatingControlPropertyModelEdit.RightValveDispensingHead));
            RightValveDispensingHead_Switch = CopyOrDefault(source.RightValveDispensingHead_Switch, "RightValveDispensingHead_Switch");
            RightValveDispensingHead_CurrentTemperature = CopyOrDefault(source.RightValveDispensingHead_CurrentTemperature, "RightValveDispensingHead_CurrentTemperature");
            RightValveDispensingHead_SetTemperature = CopyOrDefault(source.RightValveDispensingHead_SetTemperature, "RightValveDispensingHead_SetTemperature");
            RightValveDispensingHead_TemperatureRange = CopyOrDefault(source.RightValveDispensingHead_TemperatureRange, "RightValveDispensingHead_TemperatureRange");
            RightValveDispensingHead_IdleCloseInterval = CopyOrDefault(source.RightValveDispensingHead_IdleCloseInterval, "RightValveDispensingHead_IdleCloseInterval");
            RightValveDispensingHead_PreheatTime = CopyOrDefault(source.RightValveDispensingHead_PreheatTime, "RightValveDispensingHead_PreheatTime");
            RightValveDispensingHead_HeatingTime = CopyOrDefault(source.RightValveDispensingHead_HeatingTime, "RightValveDispensingHead_HeatingTime");
            RightValveDispensingHead_DetectionEnabled = CopyOrDefault(source.RightValveDispensingHead_DetectionEnabled, "RightValveDispensingHead_DetectionEnabled");
            RightValveDispensingHead_WorkEnabled = CopyOrDefault(source.RightValveDispensingHead_WorkEnabled, "RightValveDispensingHead_WorkEnabled");
            RightValveDispensingHead_DelayTime = CopyOrDefault(source.RightValveDispensingHead_DelayTime, "RightValveDispensingHead_DelayTime");
            RightValveCartridgeHeating = CopyOrDefault(source.RightValveCartridgeHeating, nameof(HeatingControlPropertyModelEdit.RightValveCartridgeHeating));
            RightValveCartridgeHeating_Switch = CopyOrDefault(source.RightValveCartridgeHeating_Switch, "RightValveCartridgeHeating_Switch");
            RightValveCartridgeHeating_CurrentTemperature = CopyOrDefault(source.RightValveCartridgeHeating_CurrentTemperature, "RightValveCartridgeHeating_CurrentTemperature");
            RightValveCartridgeHeating_SetTemperature = CopyOrDefault(source.RightValveCartridgeHeating_SetTemperature, "RightValveCartridgeHeating_SetTemperature");
            RightValveCartridgeHeating_TemperatureRange = CopyOrDefault(source.RightValveCartridgeHeating_TemperatureRange, "RightValveCartridgeHeating_TemperatureRange");
            RightValveCartridgeHeating_IdleCloseInterval = CopyOrDefault(source.RightValveCartridgeHeating_IdleCloseInterval, "RightValveCartridgeHeating_IdleCloseInterval");
            RightValveCartridgeHeating_PreheatTime = CopyOrDefault(source.RightValveCartridgeHeating_PreheatTime, "RightValveCartridgeHeating_PreheatTime");
            RightValveCartridgeHeating_HeatingTime = CopyOrDefault(source.RightValveCartridgeHeating_HeatingTime, "RightValveCartridgeHeating_HeatingTime");
            RightValveCartridgeHeating_DetectionEnabled = CopyOrDefault(source.RightValveCartridgeHeating_DetectionEnabled, "RightValveCartridgeHeating_DetectionEnabled");
            RightValveCartridgeHeating_WorkEnabled = CopyOrDefault(source.RightValveCartridgeHeating_WorkEnabled, "RightValveCartridgeHeating_WorkEnabled");
            RightValveCartridgeHeating_DelayTime = CopyOrDefault(source.RightValveCartridgeHeating_DelayTime, "RightValveCartridgeHeating_DelayTime");
            LeftValveDispensingHead = CopyOrDefault(source.LeftValveDispensingHead, nameof(HeatingControlPropertyModelEdit.LeftValveDispensingHead));
            LeftValveDispensingHead_Switch = CopyOrDefault(source.LeftValveDispensingHead_Switch, "LeftValveDispensingHead_Switch");
            LeftValveDispensingHead_CurrentTemperature = CopyOrDefault(source.LeftValveDispensingHead_CurrentTemperature, "LeftValveDispensingHead_CurrentTemperature");
            LeftValveDispensingHead_SetTemperature = CopyOrDefault(source.LeftValveDispensingHead_SetTemperature, "LeftValveDispensingHead_SetTemperature");
            LeftValveDispensingHead_TemperatureRange = CopyOrDefault(source.LeftValveDispensingHead_TemperatureRange, "LeftValveDispensingHead_TemperatureRange");
            LeftValveDispensingHead_IdleCloseInterval = CopyOrDefault(source.LeftValveDispensingHead_IdleCloseInterval, "LeftValveDispensingHead_IdleCloseInterval");
            LeftValveDispensingHead_PreheatTime = CopyOrDefault(source.LeftValveDispensingHead_PreheatTime, "LeftValveDispensingHead_PreheatTime");
            LeftValveDispensingHead_HeatingTime = CopyOrDefault(source.LeftValveDispensingHead_HeatingTime, "LeftValveDispensingHead_HeatingTime");
            LeftValveDispensingHead_DetectionEnabled = CopyOrDefault(source.LeftValveDispensingHead_DetectionEnabled, "LeftValveDispensingHead_DetectionEnabled");
            LeftValveDispensingHead_WorkEnabled = CopyOrDefault(source.LeftValveDispensingHead_WorkEnabled, "LeftValveDispensingHead_WorkEnabled");
            LeftValveDispensingHead_DelayTime = CopyOrDefault(source.LeftValveDispensingHead_DelayTime, "LeftValveDispensingHead_DelayTime");
            LeftValveCartridgeHeating = CopyOrDefault(source.LeftValveCartridgeHeating, "LeftValveCartridgeHeating");
            LeftValveCartridgeHeating_Switch = CopyOrDefault(source.LeftValveCartridgeHeating_Switch, "LeftValveCartridgeHeating_Switch");
            LeftValveCartridgeHeating_CurrentTemperature = CopyOrDefault(source.LeftValveCartridgeHeating_CurrentTemperature, "LeftValveCartridgeHeating_CurrentTemperature");
            LeftValveCartridgeHeating_SetTemperature = CopyOrDefault(source.LeftValveCartridgeHeating_SetTemperature, "LeftValveCartridgeHeating_SetTemperature");
            LeftValveCartridgeHeating_TemperatureRange = CopyOrDefault(source.LeftValveCartridgeHeating_TemperatureRange, "LeftValveCartridgeHeating_TemperatureRange");
            LeftValveCartridgeHeating_IdleCloseInterval = CopyOrDefault(source.LeftValveCartridgeHeating_IdleCloseInterval, "LeftValveCartridgeHeating_IdleCloseInterval");
            LeftValveCartridgeHeating_PreheatTime = CopyOrDefault(source.LeftValveCartridgeHeating_PreheatTime, "LeftValveCartridgeHeating_PreheatTime");
            LeftValveCartridgeHeating_HeatingTime = CopyOrDefault(source.LeftValveCartridgeHeating_HeatingTime, "LeftValveCartridgeHeating_HeatingTime");
            LeftValveCartridgeHeating_DetectionEnabled = CopyOrDefault(source.LeftValveCartridgeHeating_DetectionEnabled, "LeftValveCartridgeHeating_DetectionEnabled");
            LeftValveCartridgeHeating_WorkEnabled = CopyOrDefault(source.LeftValveCartridgeHeating_WorkEnabled, "LeftValveCartridgeHeating_WorkEnabled");
            LeftValveCartridgeHeating_DelayTime = CopyOrDefault(source.LeftValveCartridgeHeating_DelayTime, "LeftValveCartridgeHeating_DelayTime");

            ARailPreheatLeft = CopyOrDefault(source.ARailPreheatLeft, "ARailPreheatLeft");
            ARailPreheatLeft_Switch = CopyOrDefault(source.ARailPreheatLeft_Switch, "ARailPreheatLeft_Switch");
            ARailPreheatLeft_CurrentTemperature = CopyOrDefault(source.ARailPreheatLeft_CurrentTemperature, "ARailPreheatLeft_CurrentTemperature");
            ARailPreheatLeft_SetTemperature = CopyOrDefault(source.ARailPreheatLeft_SetTemperature, "ARailPreheatLeft_SetTemperature");
            ARailPreheatLeft_TemperatureRange = CopyOrDefault(source.ARailPreheatLeft_TemperatureRange, "ARailPreheatLeft_TemperatureRange");
            ARailPreheatLeft_IdleCloseInterval = CopyOrDefault(source.ARailPreheatLeft_IdleCloseInterval, "ARailPreheatLeft_IdleCloseInterval");
            ARailPreheatLeft_PreheatTime = CopyOrDefault(source.ARailPreheatLeft_PreheatTime, "ARailPreheatLeft_PreheatTime");
            ARailPreheatLeft_HeatingTime = CopyOrDefault(source.ARailPreheatLeft_HeatingTime, "ARailPreheatLeft_HeatingTime");
            ARailPreheatLeft_DetectionEnabled = CopyOrDefault(source.ARailPreheatLeft_DetectionEnabled, "ARailPreheatLeft_DetectionEnabled");
            ARailPreheatLeft_WorkEnabled = CopyOrDefault(source.ARailPreheatLeft_WorkEnabled, "ARailPreheatLeft_WorkEnabled");
            ARailPreheatLeft_DelayTime = CopyOrDefault(source.ARailPreheatLeft_DelayTime, "ARailPreheatLeft_DelayTime");

            ARailPreheatLeft2 = CopyOrDefault(source.ARailPreheatLeft2, "ARailPreheatLeft2");
            ARailPreheatLeft2_Switch = CopyOrDefault(source.ARailPreheatLeft2_Switch, "ARailPreheatLeft2_Switch");
            ARailPreheatLeft2_CurrentTemperature = CopyOrDefault(source.ARailPreheatLeft2_CurrentTemperature, "ARailPreheatLeft2_CurrentTemperature");
            ARailPreheatLeft2_SetTemperature = CopyOrDefault(source.ARailPreheatLeft2_SetTemperature, "ARailPreheatLeft2_SetTemperature");
            ARailPreheatLeft2_TemperatureRange = CopyOrDefault(source.ARailPreheatLeft2_TemperatureRange, "ARailPreheatLeft2_TemperatureRange");
            ARailPreheatLeft2_IdleCloseInterval = CopyOrDefault(source.ARailPreheatLeft2_IdleCloseInterval, "ARailPreheatLeft2_IdleCloseInterval");
            ARailPreheatLeft2_PreheatTime = CopyOrDefault(source.ARailPreheatLeft2_PreheatTime, "ARailPreheatLeft2_PreheatTime");
            ARailPreheatLeft2_HeatingTime = CopyOrDefault(source.ARailPreheatLeft2_HeatingTime, "ARailPreheatLeft2_HeatingTime");
            ARailPreheatLeft2_DetectionEnabled = CopyOrDefault(source.ARailPreheatLeft2_DetectionEnabled, "ARailPreheatLeft2_DetectionEnabled");
            ARailPreheatLeft2_WorkEnabled = CopyOrDefault(source.ARailPreheatLeft2_WorkEnabled, "ARailPreheatLeft2_WorkEnabled");
            ARailPreheatLeft2_DelayTime = CopyOrDefault(source.ARailPreheatLeft2_DelayTime, "ARailPreheatLeft2_DelayTime");

            ARailGlueBoardStationMiddle = CopyOrDefault(source.ARailGlueBoardStationMiddle, "ARailGlueBoardStationMiddle");
            ARailGlueBoardStationMiddle_Switch = CopyOrDefault(source.ARailGlueBoardStationMiddle_Switch, "ARailGlueBoardStationMiddle_Switch");
            ARailGlueBoardStationMiddle_CurrentTemperature = CopyOrDefault(source.ARailGlueBoardStationMiddle_CurrentTemperature, "ARailGlueBoardStationMiddle_CurrentTemperature");
            ARailGlueBoardStationMiddle_SetTemperature = CopyOrDefault(source.ARailGlueBoardStationMiddle_SetTemperature, "ARailGlueBoardStationMiddle_SetTemperature");
            ARailGlueBoardStationMiddle_TemperatureRange = CopyOrDefault(source.ARailGlueBoardStationMiddle_TemperatureRange, "ARailGlueBoardStationMiddle_TemperatureRange");
            ARailGlueBoardStationMiddle_IdleCloseInterval = CopyOrDefault(source.ARailGlueBoardStationMiddle_IdleCloseInterval, "ARailGlueBoardStationMiddle_IdleCloseInterval");
            ARailGlueBoardStationMiddle_PreheatTime = CopyOrDefault(source.ARailGlueBoardStationMiddle_PreheatTime, "ARailGlueBoardStationMiddle_PreheatTime");
            ARailGlueBoardStationMiddle_HeatingTime = CopyOrDefault(source.ARailGlueBoardStationMiddle_HeatingTime, "ARailGlueBoardStationMiddle_HeatingTime");
            ARailGlueBoardStationMiddle_DetectionEnabled = CopyOrDefault(source.ARailGlueBoardStationMiddle_DetectionEnabled, "ARailGlueBoardStationMiddle_DetectionEnabled");
            ARailGlueBoardStationMiddle_WorkEnabled = CopyOrDefault(source.ARailGlueBoardStationMiddle_WorkEnabled, "ARailGlueBoardStationMiddle_WorkEnabled");
            ARailGlueBoardStationMiddle_DelayTime = CopyOrDefault(source.ARailGlueBoardStationMiddle_DelayTime, "ARailGlueBoardStationMiddle_DelayTime");

            ARailDispensingStationMiddle2 = CopyOrDefault(source.ARailDispensingStationMiddle2, "ARailDispensingStationMiddle2");
            ARailDispensingStationMiddle2_Switch = CopyOrDefault(source.ARailDispensingStationMiddle2_Switch, "ARailDispensingStationMiddle2_Switch");
            ARailDispensingStationMiddle2_CurrentTemperature = CopyOrDefault(source.ARailDispensingStationMiddle2_CurrentTemperature, "ARailDispensingStationMiddle2_CurrentTemperature");
            ARailDispensingStationMiddle2_SetTemperature = CopyOrDefault(source.ARailDispensingStationMiddle2_SetTemperature, "ARailDispensingStationMiddle2_SetTemperature");
            ARailDispensingStationMiddle2_TemperatureRange = CopyOrDefault(source.ARailDispensingStationMiddle2_TemperatureRange, "ARailDispensingStationMiddle2_TemperatureRange");
            ARailDispensingStationMiddle2_IdleCloseInterval = CopyOrDefault(source.ARailDispensingStationMiddle2_IdleCloseInterval, "ARailDispensingStationMiddle2_IdleCloseInterval");
            ARailDispensingStationMiddle2_PreheatTime = CopyOrDefault(source.ARailDispensingStationMiddle2_PreheatTime, "ARailDispensingStationMiddle2_PreheatTime");
            ARailDispensingStationMiddle2_HeatingTime = CopyOrDefault(source.ARailDispensingStationMiddle2_HeatingTime, "ARailDispensingStationMiddle2_HeatingTime");
            ARailDispensingStationMiddle2_DetectionEnabled = CopyOrDefault(source.ARailDispensingStationMiddle2_DetectionEnabled, "ARailDispensingStationMiddle2_DetectionEnabled");
            ARailDispensingStationMiddle2_WorkEnabled = CopyOrDefault(source.ARailDispensingStationMiddle2_WorkEnabled, "ARailDispensingStationMiddle2_WorkEnabled");
            ARailDispensingStationMiddle2_DelayTime = CopyOrDefault(source.ARailDispensingStationMiddle2_DelayTime, "ARailDispensingStationMiddle2_DelayTime");

            ARailPreheatRight = CopyOrDefault(source.ARailPreheatRight, "ARailPreheatRight");
            ARailPreheatRight_Switch = CopyOrDefault(source.ARailPreheatRight_Switch, "ARailPreheatRight_Switch");
            ARailPreheatRight_CurrentTemperature = CopyOrDefault(source.ARailPreheatRight_CurrentTemperature, "ARailPreheatRight_CurrentTemperature");
            ARailPreheatRight_SetTemperature = CopyOrDefault(source.ARailPreheatRight_SetTemperature, "ARailPreheatRight_SetTemperature");
            ARailPreheatRight_TemperatureRange = CopyOrDefault(source.ARailPreheatRight_TemperatureRange, "ARailPreheatRight_TemperatureRange");
            ARailPreheatRight_IdleCloseInterval = CopyOrDefault(source.ARailPreheatRight_IdleCloseInterval, "ARailPreheatRight_IdleCloseInterval");
            ARailPreheatRight_PreheatTime = CopyOrDefault(source.ARailPreheatRight_PreheatTime, "ARailPreheatRight_PreheatTime");
            ARailPreheatRight_HeatingTime = CopyOrDefault(source.ARailPreheatRight_HeatingTime, "ARailPreheatRight_HeatingTime");
            ARailPreheatRight_DetectionEnabled = CopyOrDefault(source.ARailPreheatRight_DetectionEnabled, "ARailPreheatRight_DetectionEnabled");
            ARailPreheatRight_WorkEnabled = CopyOrDefault(source.ARailPreheatRight_WorkEnabled, "ARailPreheatRight_WorkEnabled");
            ARailPreheatRight_DelayTime = CopyOrDefault(source.ARailPreheatRight_DelayTime, "ARailPreheatRight_DelayTime");

            ARailPreheatRight2 = CopyOrDefault(source.ARailPreheatRight2, "ARailPreheatRight2");
            ARailPreheatRight2_Switch = CopyOrDefault(source.ARailPreheatRight2_Switch, "ARailPreheatRight2_Switch");
            ARailPreheatRight2_CurrentTemperature = CopyOrDefault(source.ARailPreheatRight2_CurrentTemperature, "ARailPreheatRight2_CurrentTemperature");
            ARailPreheatRight2_SetTemperature = CopyOrDefault(source.ARailPreheatRight2_SetTemperature, "ARailPreheatRight2_SetTemperature");
            ARailPreheatRight2_TemperatureRange = CopyOrDefault(source.ARailPreheatRight2_TemperatureRange, "ARailPreheatRight2_TemperatureRange");
            ARailPreheatRight2_IdleCloseInterval = CopyOrDefault(source.ARailPreheatRight2_IdleCloseInterval, "ARailPreheatRight2_IdleCloseInterval");
            ARailPreheatRight2_PreheatTime = CopyOrDefault(source.ARailPreheatRight2_PreheatTime, "ARailPreheatRight2_PreheatTime");
            ARailPreheatRight2_HeatingTime = CopyOrDefault(source.ARailPreheatRight2_HeatingTime, "ARailPreheatRight2_HeatingTime");
            ARailPreheatRight2_DetectionEnabled = CopyOrDefault(source.ARailPreheatRight2_DetectionEnabled, "ARailPreheatRight2_DetectionEnabled");
            ARailPreheatRight2_WorkEnabled = CopyOrDefault(source.ARailPreheatRight2_WorkEnabled, "ARailPreheatRight2_WorkEnabled");
            ARailPreheatRight2_DelayTime = CopyOrDefault(source.ARailPreheatRight2_DelayTime, "ARailPreheatRight2_DelayTime");

            BRailPreheatLeft = CopyOrDefault(source.BRailPreheatLeft, "BRailPreheatLeft");
            BRailPreheatLeft_Switch = CopyOrDefault(source.BRailPreheatLeft_Switch, "BRailPreheatLeft_Switch");
            BRailPreheatLeft_CurrentTemperature = CopyOrDefault(source.BRailPreheatLeft_CurrentTemperature, "BRailPreheatLeft_CurrentTemperature");
            BRailPreheatLeft_SetTemperature = CopyOrDefault(source.BRailPreheatLeft_SetTemperature, "BRailPreheatLeft_SetTemperature");
            BRailPreheatLeft_TemperatureRange = CopyOrDefault(source.BRailPreheatLeft_TemperatureRange, "BRailPreheatLeft_TemperatureRange");
            BRailPreheatLeft_IdleCloseInterval = CopyOrDefault(source.BRailPreheatLeft_IdleCloseInterval, "BRailPreheatLeft_IdleCloseInterval");
            BRailPreheatLeft_PreheatTime = CopyOrDefault(source.BRailPreheatLeft_PreheatTime, "BRailPreheatLeft_PreheatTime");
            BRailPreheatLeft_HeatingTime = CopyOrDefault(source.BRailPreheatLeft_HeatingTime, "BRailPreheatLeft_HeatingTime");
            BRailPreheatLeft_DetectionEnabled = CopyOrDefault(source.BRailPreheatLeft_DetectionEnabled, "BRailPreheatLeft_DetectionEnabled");
            BRailPreheatLeft_WorkEnabled = CopyOrDefault(source.BRailPreheatLeft_WorkEnabled, "BRailPreheatLeft_WorkEnabled");
            BRailPreheatLeft_DelayTime = CopyOrDefault(source.BRailPreheatLeft_DelayTime, "BRailPreheatLeft_DelayTime");

            BRailPreheatLeft2 = CopyOrDefault(source.BRailPreheatLeft2, "BRailPreheatLeft2");
            BRailPreheatLeft2_Switch = CopyOrDefault(source.BRailPreheatLeft2_Switch, "BRailPreheatLeft2_Switch");
            BRailPreheatLeft2_CurrentTemperature = CopyOrDefault(source.BRailPreheatLeft2_CurrentTemperature, "BRailPreheatLeft2_CurrentTemperature");
            BRailPreheatLeft2_SetTemperature = CopyOrDefault(source.BRailPreheatLeft2_SetTemperature, "BRailPreheatLeft2_SetTemperature");
            BRailPreheatLeft2_TemperatureRange = CopyOrDefault(source.BRailPreheatLeft2_TemperatureRange, "BRailPreheatLeft2_TemperatureRange");
            BRailPreheatLeft2_IdleCloseInterval = CopyOrDefault(source.BRailPreheatLeft2_IdleCloseInterval, "BRailPreheatLeft2_IdleCloseInterval");
            BRailPreheatLeft2_PreheatTime = CopyOrDefault(source.BRailPreheatLeft2_PreheatTime, "BRailPreheatLeft2_PreheatTime");
            BRailPreheatLeft2_HeatingTime = CopyOrDefault(source.BRailPreheatLeft2_HeatingTime, "BRailPreheatLeft2_HeatingTime");
            BRailPreheatLeft2_DetectionEnabled = CopyOrDefault(source.BRailPreheatLeft2_DetectionEnabled, "BRailPreheatLeft2_DetectionEnabled");
            BRailPreheatLeft2_WorkEnabled = CopyOrDefault(source.BRailPreheatLeft2_WorkEnabled, "BRailPreheatLeft2_WorkEnabled");
            BRailPreheatLeft2_DelayTime = CopyOrDefault(source.BRailPreheatLeft2_DelayTime, "BRailPreheatLeft2_DelayTime");

            BRailGlueBoardStationMiddle = CopyOrDefault(source.BRailGlueBoardStationMiddle, "BRailGlueBoardStationMiddle");
            BRailGlueBoardStationMiddle_Switch = CopyOrDefault(source.BRailGlueBoardStationMiddle_Switch, "BRailGlueBoardStationMiddle_Switch");
            BRailGlueBoardStationMiddle_CurrentTemperature = CopyOrDefault(source.BRailGlueBoardStationMiddle_CurrentTemperature, "BRailGlueBoardStationMiddle_CurrentTemperature");
            BRailGlueBoardStationMiddle_SetTemperature = CopyOrDefault(source.BRailGlueBoardStationMiddle_SetTemperature, "BRailGlueBoardStationMiddle_SetTemperature");
            BRailGlueBoardStationMiddle_TemperatureRange = CopyOrDefault(source.BRailGlueBoardStationMiddle_TemperatureRange, "BRailGlueBoardStationMiddle_TemperatureRange");
            BRailGlueBoardStationMiddle_IdleCloseInterval = CopyOrDefault(source.BRailGlueBoardStationMiddle_IdleCloseInterval, "BRailGlueBoardStationMiddle_IdleCloseInterval");
            BRailGlueBoardStationMiddle_PreheatTime = CopyOrDefault(source.BRailGlueBoardStationMiddle_PreheatTime, "BRailGlueBoardStationMiddle_PreheatTime");
            BRailGlueBoardStationMiddle_HeatingTime = CopyOrDefault(source.BRailGlueBoardStationMiddle_HeatingTime, "BRailGlueBoardStationMiddle_HeatingTime");
            BRailGlueBoardStationMiddle_DetectionEnabled = CopyOrDefault(source.BRailGlueBoardStationMiddle_DetectionEnabled, "BRailGlueBoardStationMiddle_DetectionEnabled");
            BRailGlueBoardStationMiddle_WorkEnabled = CopyOrDefault(source.BRailGlueBoardStationMiddle_WorkEnabled, "BRailGlueBoardStationMiddle_WorkEnabled");
            BRailGlueBoardStationMiddle_DelayTime = CopyOrDefault(source.BRailGlueBoardStationMiddle_DelayTime, "BRailGlueBoardStationMiddle_DelayTime");

            BRailDispensingStationMiddle2 = CopyOrDefault(source.BRailDispensingStationMiddle2, "BRailDispensingStationMiddle2");
            BRailDispensingStationMiddle2_Switch = CopyOrDefault(source.BRailDispensingStationMiddle2_Switch, "BRailDispensingStationMiddle2_Switch");
            BRailDispensingStationMiddle2_CurrentTemperature = CopyOrDefault(source.BRailDispensingStationMiddle2_CurrentTemperature, "BRailDispensingStationMiddle2_CurrentTemperature");
            BRailDispensingStationMiddle2_SetTemperature = CopyOrDefault(source.BRailDispensingStationMiddle2_SetTemperature, "BRailDispensingStationMiddle2_SetTemperature");
            BRailDispensingStationMiddle2_TemperatureRange = CopyOrDefault(source.BRailDispensingStationMiddle2_TemperatureRange, "BRailDispensingStationMiddle2_TemperatureRange");
            BRailDispensingStationMiddle2_IdleCloseInterval = CopyOrDefault(source.BRailDispensingStationMiddle2_IdleCloseInterval, "BRailDispensingStationMiddle2_IdleCloseInterval");
            BRailDispensingStationMiddle2_PreheatTime = CopyOrDefault(source.BRailDispensingStationMiddle2_PreheatTime, "BRailDispensingStationMiddle2_PreheatTime");
            BRailDispensingStationMiddle2_HeatingTime = CopyOrDefault(source.BRailDispensingStationMiddle2_HeatingTime, "BRailDispensingStationMiddle2_HeatingTime");
            BRailDispensingStationMiddle2_DetectionEnabled = CopyOrDefault(source.BRailDispensingStationMiddle2_DetectionEnabled, "BRailDispensingStationMiddle2_DetectionEnabled");
            BRailDispensingStationMiddle2_WorkEnabled = CopyOrDefault(source.BRailDispensingStationMiddle2_WorkEnabled, "BRailDispensingStationMiddle2_WorkEnabled");
            BRailDispensingStationMiddle2_DelayTime = CopyOrDefault(source.BRailDispensingStationMiddle2_DelayTime, "BRailDispensingStationMiddle2_DelayTime");

            BRailPreheatRight = CopyOrDefault(source.BRailPreheatRight, "BRailPreheatRight");
            BRailPreheatRight_Switch = CopyOrDefault(source.BRailPreheatRight_Switch, "BRailPreheatRight_Switch");
            BRailPreheatRight_CurrentTemperature = CopyOrDefault(source.BRailPreheatRight_CurrentTemperature, "BRailPreheatRight_CurrentTemperature");
            BRailPreheatRight_SetTemperature = CopyOrDefault(source.BRailPreheatRight_SetTemperature, "BRailPreheatRight_SetTemperature");
            BRailPreheatRight_TemperatureRange = CopyOrDefault(source.BRailPreheatRight_TemperatureRange, "BRailPreheatRight_TemperatureRange");
            BRailPreheatRight_IdleCloseInterval = CopyOrDefault(source.BRailPreheatRight_IdleCloseInterval, "BRailPreheatRight_IdleCloseInterval");
            BRailPreheatRight_PreheatTime = CopyOrDefault(source.BRailPreheatRight_PreheatTime, "BRailPreheatRight_PreheatTime");
            BRailPreheatRight_HeatingTime = CopyOrDefault(source.BRailPreheatRight_HeatingTime, "BRailPreheatRight_HeatingTime");
            BRailPreheatRight_DetectionEnabled = CopyOrDefault(source.BRailPreheatRight_DetectionEnabled, "BRailPreheatRight_DetectionEnabled");
            BRailPreheatRight_WorkEnabled = CopyOrDefault(source.BRailPreheatRight_WorkEnabled, "BRailPreheatRight_WorkEnabled");
            BRailPreheatRight_DelayTime = CopyOrDefault(source.BRailPreheatRight_DelayTime, "BRailPreheatRight_DelayTime");

            BRailPreheatRight2 = CopyOrDefault(source.BRailPreheatRight2, "BRailPreheatRight2");
            BRailPreheatRight2_Switch = CopyOrDefault(source.BRailPreheatRight2_Switch, "BRailPreheatRight2_Switch");
            BRailPreheatRight2_CurrentTemperature = CopyOrDefault(source.BRailPreheatRight2_CurrentTemperature, "BRailPreheatRight2_CurrentTemperature");
            BRailPreheatRight2_SetTemperature = CopyOrDefault(source.BRailPreheatRight2_SetTemperature, "BRailPreheatRight2_SetTemperature");
            BRailPreheatRight2_TemperatureRange = CopyOrDefault(source.BRailPreheatRight2_TemperatureRange, "BRailPreheatRight2_TemperatureRange");
            BRailPreheatRight2_IdleCloseInterval = CopyOrDefault(source.BRailPreheatRight2_IdleCloseInterval, "BRailPreheatRight2_IdleCloseInterval");
            BRailPreheatRight2_PreheatTime = CopyOrDefault(source.BRailPreheatRight2_PreheatTime, "BRailPreheatRight2_PreheatTime");
            BRailPreheatRight2_HeatingTime = CopyOrDefault(source.BRailPreheatRight2_HeatingTime, "BRailPreheatRight2_HeatingTime");
            BRailPreheatRight2_DetectionEnabled = CopyOrDefault(source.BRailPreheatRight2_DetectionEnabled, "BRailPreheatRight2_DetectionEnabled");
            BRailPreheatRight2_WorkEnabled = CopyOrDefault(source.BRailPreheatRight2_WorkEnabled, "BRailPreheatRight2_WorkEnabled");
            BRailPreheatRight2_DelayTime = CopyOrDefault(source.BRailPreheatRight2_DelayTime, "BRailPreheatRight2_DelayTime");
        }
    }

    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [ExpandableObjectDisplayMode(IsCategoryVisible = NullableBooleanType.No)]
    public class HeatingModuleInfo : MiniReactiveObject, IJsonValueConvert, IGriffinsBaseValue
    {
        public static readonly Guid Object_ID = new Guid("{D3B3A2B1-1C2D-4E4F-8A6B-7C8D9E0F1A2B}");

        private bool _switch;
        [DisplayName("关/开")]
        public bool Switch
        {
            get { return _switch; }
            set { SetProperty(ref _switch, value); }
        }

        private string _moduleName;
        [Browsable(false)]
        public string ModuleName
        {
            get { return _moduleName; }
            set { SetProperty(ref _moduleName, value); }
        }

        private int _currentTemperature;
        [DisplayName("当前温度")]
        public int CurrentTemperature
        {
            get { return _currentTemperature; }
            set { SetProperty(ref _currentTemperature, value); }
        }

        private int _setTemperature;
        [DisplayName("设定温度")]
        public int SetTemperature
        {
            get { return _setTemperature; }
            set { SetProperty(ref _setTemperature, value); }
        }

        private int _temperatureRange;
        [DisplayName("温度范围")]
        public int TemperatureRange
        {
            get { return _temperatureRange; }
            set { SetProperty(ref _temperatureRange, value); }
        }

        private int _idleCloseInterval;
        [DisplayName("空闲关闭间隔")]
        public int IdleCloseInterval
        {
            get { return _idleCloseInterval; }
            set { SetProperty(ref _idleCloseInterval, value); }
        }

        private int _preheatTime;
        [DisplayName("预热时间")]
        public int PreheatTime
        {
            get { return _preheatTime; }
            set { SetProperty(ref _preheatTime, value); }
        }

        private int _heatingTime;
        [DisplayName("加热时间")]
        public int HeatingTime
        {
            get { return _heatingTime; }
            set { SetProperty(ref _heatingTime, value); }
        }

        private bool _detectionEnabled;
        [DisplayName("检测开启")]
        public bool DetectionEnabled
        {
            get { return _detectionEnabled; }
            set { SetProperty(ref _detectionEnabled, value); }
        }

        private bool _workEnabled;
        [DisplayName("工作开启")]
        public bool WorkEnabled
        {
            get { return _workEnabled; }
            set { SetProperty(ref _workEnabled, value); }
        }

        private int _delayTime;
        [DisplayName("延迟时间")]
        public int DelayTime
        {
            get { return _delayTime; }
            set { SetProperty(ref _delayTime, value); }
        }

        bool IGriffinsBaseValue.IsObject_Byte => false;

        Guid IGriffinsBaseValue.GetObject_ID() => Object_ID;

        GriffinsBaseValue IGriffinsBaseValue.ToBaseValue()
        {
            ObjectValue_Json objectValue_Json = new ObjectValue_Json(Object_ID);
            objectValue_Json.JsonVal = ((IJsonValueConvert)this).ToJsonDataObject();
            return GriffinsBaseValue.Create(objectValue_Json);
        }

        void IGriffinsBaseValue.PopulateFromBaseValue(GriffinsBaseValue baseValue)
        {
            if (baseValue != null && baseValue.Val != null)
            {
                if (!(baseValue.Val is ObjectValue_Json))
                {
                    throw new Exception("对象值不是HeatingModuleInfo转换的");
                }

                if ((baseValue.Val as ObjectValue_Json).Object_ID != Object_ID)
                {
                    throw new Exception("对象值不是HeatingModuleInfo转换的");
                }

                ((IJsonValueConvert)this).FromJsonDataObject((baseValue.Val as ObjectValue_Json).JsonVal);
            }
        }

        void IJsonValueConvert.FromJsonDataObject(string jsonDataObject)
        {
            if (string.IsNullOrEmpty(jsonDataObject))
            {
                throw new ArgumentNullException(nameof(jsonDataObject));
            }

            using JsonDocument jsonDocument = JsonDocument.Parse(jsonDataObject);
            JsonElement rootElement = jsonDocument.RootElement;
            JsonElement value;
            Switch = rootElement.TryGetProperty("Switch", out value) && value.GetBoolean();
            ModuleName = rootElement.TryGetProperty("ModuleName", out value) ? value.GetString() : "";
            CurrentTemperature = rootElement.TryGetProperty("CurrentTemperature", out value) ? value.GetInt32() : 0;
            SetTemperature = rootElement.TryGetProperty("SetTemperature", out value) ? value.GetInt32() : 0;
            TemperatureRange = rootElement.TryGetProperty("TemperatureRange", out value) ? value.GetInt32() : 0;
            IdleCloseInterval = rootElement.TryGetProperty("IdleCloseInterval", out value) ? value.GetInt32() : 0;
            PreheatTime = rootElement.TryGetProperty("PreheatTime", out value) ? value.GetInt32() : 0;
            HeatingTime = rootElement.TryGetProperty("HeatingTime", out value) ? value.GetInt32() : 0;
            DetectionEnabled = rootElement.TryGetProperty("DetectionEnabled", out value) && value.GetBoolean();
            WorkEnabled = rootElement.TryGetProperty("WorkEnabled", out value) && value.GetBoolean();
            DelayTime = rootElement.TryGetProperty("DelayTime", out value) ? value.GetInt32() : 0;
        }

        string IJsonValueConvert.ToJsonDataObject()
        {
            var value = new
            {
                Switch = Switch,
                ModuleName = ModuleName,
                CurrentTemperature = CurrentTemperature,
                SetTemperature = SetTemperature,
                TemperatureRange = TemperatureRange,
                IdleCloseInterval = IdleCloseInterval,
                PreheatTime = PreheatTime,
                HeatingTime = HeatingTime,
                DetectionEnabled = DetectionEnabled,
                WorkEnabled = WorkEnabled,
                DelayTime = DelayTime,
            };
            return System.Text.Json.JsonSerializer.Serialize(value);
        }

        public override string ToString() => ModuleName ?? "加热模块";
    }

    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [ExpandableObjectDisplayMode(IsCategoryVisible = NullableBooleanType.No)]
    public class HeatingModulesInfo : MiniReactiveObject, IJsonValueConvert, IGriffinsBaseValue
    {
        public static readonly Guid Object_ID = new Guid("{E5F6D7C8-B9A8-4765-9876-543210987654}");

        private List<HeatingModuleInfo> _heatingModules;
        [DisplayName("加热模块")]
        public List<HeatingModuleInfo> HeatingModules
        {
            get { return _heatingModules; }
            set { SetProperty(ref _heatingModules, value); }
        }

        public HeatingModulesInfo()
        {
            HeatingModules = new List<HeatingModuleInfo>();
        }

        bool IGriffinsBaseValue.IsObject_Byte => false;

        Guid IGriffinsBaseValue.GetObject_ID() => Object_ID;

        GriffinsBaseValue IGriffinsBaseValue.ToBaseValue()
        {
            ObjectValue_Json objectValue_Json = new ObjectValue_Json(Object_ID);
            objectValue_Json.JsonVal = ((IJsonValueConvert)this).ToJsonDataObject();
            return GriffinsBaseValue.Create(objectValue_Json);
        }

        void IGriffinsBaseValue.PopulateFromBaseValue(GriffinsBaseValue baseValue)
        {
            if (baseValue != null && baseValue.Val != null)
            {
                if (!(baseValue.Val is ObjectValue_Json))
                {
                    throw new Exception("对象值不是HeatingModulesInfo转换的");
                }

                if ((baseValue.Val as ObjectValue_Json).Object_ID != Object_ID)
                {
                    throw new Exception("对象值不是HeatingModulesInfo转换的");
                }

                ((IJsonValueConvert)this).FromJsonDataObject((baseValue.Val as ObjectValue_Json).JsonVal);
            }
        }

        void IJsonValueConvert.FromJsonDataObject(string jsonDataObject)
        {
            if (string.IsNullOrEmpty(jsonDataObject))
            {
                throw new ArgumentNullException(nameof(jsonDataObject));
            }

            using JsonDocument jsonDocument = JsonDocument.Parse(jsonDataObject);
            JsonElement rootElement = jsonDocument.RootElement;
            JsonElement value;
            if (rootElement.TryGetProperty("HeatingModules", out value) && value.ValueKind == JsonValueKind.Array)
            {
                var heatingModules = new List<HeatingModuleInfo>();
                foreach (var item in value.EnumerateArray())
                {
                    var moduleJson = item.GetRawText();
                    var module = new HeatingModuleInfo();
                    ((IJsonValueConvert)module).FromJsonDataObject(moduleJson);
                    heatingModules.Add(module);
                }
                HeatingModules = heatingModules;
            }
        }

        string IJsonValueConvert.ToJsonDataObject()
        {
            var value = new
            {
                HeatingModules = HeatingModules ?? new List<HeatingModuleInfo>(),
            };
            return System.Text.Json.JsonSerializer.Serialize(value);
        }

        public override string ToString() => "加热模块";
    }
}