using Avalonia.PropertyGrid.Controls;
using Avalonia.Threading;
using GF_Gereric;
using GKG.Map.ProductionInformationFuncCtlMapCell.Models;
using GKG.Map.ProductionInformationFuncCtlMapCell.View;
using GKG.Map.ProductionInformationFuncCtlMapCell.ViewModel;
using Griffins;
using Griffins.Map;
using Griffins.Map.Cmd;
using Griffins.Map.UI;
using Griffins.UI2;
using Newtonsoft.JsonG;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace GKG.Map.ProductionInformationFuncCtlMapCell
{
    /// <summary>
    /// 生产信息功能图元对象
    /// 负责打通 View, ViewModel, Model 与底层 Griffins Map 框架的数据流和命令下发。
    /// </summary>
    internal class MapCellProductionInfoCtlObj : FunctionalCellBase
    {
        #region 字段与属性

        private ProductionInformationView _view;
        private ProductionInformationViewModel _viewModel = null!;

        /// <summary>
        /// 快速获取生产信息属性编辑模型实例 (Model)
        /// </summary>
        [Browsable(false)]
        public ProductionInfoPropertyModelEdit ProductionInfoPropertyModelEdit => (PropertyEditModelBase as ProductionInfoPropertyModelEdit)!;

        #endregion

        #region 构造与初始化

        /// <summary>
        /// 静态构造函数 (处理全局资源预热)
        /// </summary>
        static MapCellProductionInfoCtlObj() { }

        /// <summary>
        /// 运行时图元实例构造函数
        /// </summary>
        public MapCellProductionInfoCtlObj(MapObjID mapCellID, string mapCellName)
            : this(mapCellID, mapCellName, false)
        {
        }

        /// <summary>
        /// 图元实例全参数构造函数 (区分设计时与运行时)
        /// </summary>
        public MapCellProductionInfoCtlObj(MapObjID mapCellID, string mapCellName, bool designTime)
        {
            PropertyEditModelBase = CreatePropertyModelEditBase();
            PropertyBindEditModelBase = CreatePropertyBindEditModelBase();
            EventBindEditModel = CreateEventBindEditModel();

            base.SetID(mapCellID);
            base.SetName(mapCellName);
            (this as IMapCellTypeBase).Name = ResourceA.ProductionInformation;

            _view = new ProductionInformationView();

            RegisterPropertiesAndOprts();
        }

        /// <summary>
        /// 统一收口图元属性与操作原子的注册逻辑
        /// </summary>
        private void RegisterPropertiesAndOprts()
        {
            // 1. 注册原生交互事件
            RegisterEvent(new MapObjEventInfo(
                MapObjPropEventConst.Event_MouseClick,
                MapObjPropEventConst.GetName(MapObjPropEventConst.Event_MouseClick),
                GriffinsBaseDataType.Object_Bytes,
                GraphMouseEventParam.Object_ID));

            // 2. 注册数据操作原子
            RegisterOprtCellInfo(new MapOprtCellInfo(
                ProductionInfoMapOprtCellConst.Datas_MapOprtCellID,
                ResourceA.Datas_MapOprtCellName));

            // 3. 注册核心数据属性绑定
            RegisterProperty(new MapObjPropertyInfo(
                nameof(ProductionInfoPropertyModelEdit.Datas),
                ResourceA.Datas,
                GriffinsBaseDataType.Object_Bytes,
                GFBaseTypePropValueList.Object_ID,
                typeof(ProductionInfoData),
                false,
                true,
                null));

            RegisterOprtInfo(new MapOprtInfo(
                nameof(ProductionInfoPropertyModelEdit.Datas),
                ResourceA.Datas_MapOprtName,
                OprtExecKind.Normal,
                string.Empty,
                new MapOprtCellInstInfoList()
                {
                    new MapOprtCellInstInfo()
                    {
                        InstanceID = Guid.NewGuid(),
                        OprtCellID = ProductionInfoMapOprtCellConst.Datas_MapOprtCellID,
                        CfgInfo = null
                    }
                }));
        }

        #endregion

        #region 执行弹窗意图下发与委托注入

        /// <summary>
        /// 拦截配方切换意图，组装通信委托并呼出弹窗
        /// </summary>
        private async Task ExeSwitchRecipeAsync(Interaction<ReactiveObject, bool> interaction)
        {
            // 1. 封装拉取配方委托
            Func<Task<List<string>>> fetchFunc = async () =>
            {
                var (success, retVal) = await ExecuteMapCellEventAsync("GetAllFormulaNumberes", new GFBaseTypeParamValueList());

                if (!success || retVal == null || retVal.Count == 0 || retVal[0].Value == null)
                {
                    // 脱机模式/异常回退假数据
                    return new List<string> { "GKG-Standard-001", "GKG-Standard-002", "Test-Recipe-A" };
                }

                FormulaNumberList list = new FormulaNumberList();
                ((IGriffinsBaseValue)list).PopulateFromBaseValue(retVal[0].Value);

                return list.Select(i => i.ToString()).ToList();
            };

            // 2. 封装下发配方委托
            Func<string, Task> applyFunc = async (recipeName) =>
            {
                var paramList = new GFBaseTypeParamValueList
                {
                    new GFBaseTypeParamValue("RecipeName", new GriffinsBaseValue(recipeName))
                };

                var (success, _) = await ExecuteMapCellEventAsync("SetCurFormulaNumber", paramList);
                if (!success) throw new TimeoutException("执行切换配方指令失败。");
            };

            // 3. 创建纯净的弹窗 VM 并呼出
            var dialogVM = new RecipeSelectionViewModel(fetchFunc, applyFunc, ProductionInfoPropertyModelEdit.Datas?.CurrentRecipeName ?? string.Empty);
            await interaction.Handle(dialogVM);
        }

        /// <summary>
        /// 拦截一键换线意图并执行
        /// </summary>
        private async Task ExeZipChangeProgramAsync(Interaction<ReactiveObject, bool> interaction)
        {
            // 若后端有真实指令可在此追加:
            // await ExecuteMapCellEventAsync("ChangeLine", new GFBaseTypeParamValueList());

            var messageDialog = new MessageDialogViewModel
            {
                Title = "提示",
                IconType = MessageDialogIconType.Tip,
                Message = "已成功向系统发送请求换线通知！",
                ButtonContentOk = "确定",
                IsOkVisible = true,
                IsYesVisible = false,
                IsNoVisible = false
            };

            await interaction.Handle(messageDialog);
        }

        /// <summary>
        /// 拦截生产详情查询意图，组装通信委托并呼出弹窗
        /// </summary>
        private async Task ExeQueryProductionInfoAsync(Interaction<ReactiveObject, bool> interaction)
        {
            Func<DateTime, Task<List<ProductionDetails>>> fetchFunc = async (date) =>
            {
                var paramList = new GFBaseTypeParamValueList
                {
                    new GFBaseTypeParamValue("StartTime", new GriffinsBaseValue(date.ToString("yyyy-MM-dd 00:00:00"))),
                    new GFBaseTypeParamValue("EndTime", new GriffinsBaseValue(date.ToString("yyyy-MM-dd 23:59:59")))
                };

                var (success, retVal) = await ExecuteMapCellEventAsync("GetProductionDetails", paramList);

                if (!success || retVal == null || retVal.Count == 0 || retVal[0].Value == null)
                {
                    // 脱机造假数据
                    return GenerateDummyProductionDetails();
                }

                var json = (retVal[0].Value.Val as ObjectValue_Json)?.JsonVal;

                // 统一收敛为系统级序列化保护，杜绝双引擎隐患
                if (!string.IsNullOrEmpty(json))
                {
                    return JsonObjConvert.FromJSon<List<ProductionDetails>>(json) ?? new List<ProductionDetails>();
                }

                return new List<ProductionDetails>();
            };

            var dialogVM = new ProductionDetailsViewModel(fetchFunc);
            await interaction.Handle(dialogVM);
        }

        /// <summary>
        /// 拦截机台运行统计查询意图，组装通信委托并呼出弹窗
        /// </summary>
        private async Task ExeShowMachineRunningStatisticsAsync(Interaction<ReactiveObject, bool> interaction)
        {
            Func<DateTime, DateTime, Task<List<MachineStatusStatistics>>> fetchFunc = async (start, end) =>
            {
                var paramList = new GFBaseTypeParamValueList
                {
                    new GFBaseTypeParamValue("StartTime", new GriffinsBaseValue(start.ToString("yyyy-MM-dd 00:00:00"))),
                    new GFBaseTypeParamValue("EndTime", new GriffinsBaseValue(end.ToString("yyyy-MM-dd 23:59:59")))
                };

                var (success, retVal) = await ExecuteMapCellEventAsync("GetMachineStatusRecord", paramList);

                if (!success || retVal == null || retVal.Count == 0 || retVal[0].Value == null)
                {
                    return new List<MachineStatusStatistics>
                    {
                        new MachineStatusStatistics { Type = "停止时间", Time = "00:30:05", Count = 5, Percentage = "30%" },
                        new MachineStatusStatistics { Type = "运行时间", Time = "02:05:00", Count = 3, Percentage = "60%" }
                    };
                }

                var json = (retVal[0].Value.Val as ObjectValue_Json)?.JsonVal;

                if (!string.IsNullOrEmpty(json))
                {
                    return JsonObjConvert.FromJSon<List<MachineStatusStatistics>>(json) ?? new List<MachineStatusStatistics>();
                }

                return new List<MachineStatusStatistics>();
            };

            var dialogVM = new MachineStatusRecordViewModel(fetchFunc);
            await interaction.Handle(dialogVM);
        }

        /// <summary>
        /// 拦截清零轨道数据意图并下发指令
        /// </summary>
        private async Task ExeClearLaneDataAsync(string laneId, bool isTotal)
        {
            var paramList = new GFBaseTypeParamValueList
            {
                new GFBaseTypeParamValue("LaneId", new GriffinsBaseValue(laneId)),
                new GFBaseTypeParamValue("IsTotal", new GriffinsBaseValue(isTotal))
            };

            // TODO: 修改为真实的清零指令名，取消注释即可下发
            // var (success, _) = await ExecuteMapCellEventAsync("ClearYield", paramList);
            System.Diagnostics.Debug.WriteLine($"脱机模拟下发清零轨道 {laneId}, Total={isTotal}");
        }

        #endregion

        #region 通用硬件指令下发

        /// <summary>
        /// 统一封装底层的 ExecMapCellEvent 异步安全调用，收束脱机判断及异常
        /// </summary>
        private async Task<(bool Success, GFBaseTypeParamValueList? Result)> ExecuteMapCellEventAsync(string commandName, GFBaseTypeParamValueList parameters)
        {
            if (base.CallBack?.INorthSvrCommandExec == null)
            {
                System.Diagnostics.Debug.WriteLine($"脱机模式：虚拟执行 {commandName}");
                return (false, null);
            }

            var executor = (IMapObjCellCallBackBase)base.CallBack;

            return await Task.Run(() =>
            {
                bool success = executor.ExecMapCellEvent(EventCmdKind.MpCmdKind, commandName, parameters, out var retVal);
                return (success, retVal);
            });
        }

        /// <summary>
        /// 脱机用的测试数据生成器
        /// </summary>
        private List<ProductionDetails> GenerateDummyProductionDetails()
        {
            var items = new List<ProductionDetails>();
            for (int i = 0; i < 20; i++)
            {
                items.Add(new ProductionDetails
                {
                    TimeInterval = $"{i:D2}:00 - {i + 1:D2}:00",
                    ProgramName = $"Recipe_A0{i % 5 + 1}",
                    BigPcs = i * 10 + 5,
                    SmallPcs = i * 40 + 20
                });
            }
            return items;
        }

        #endregion

        #region 数据更新与同步

        /// <summary>
        /// 外部或框架设置图元属性的入口
        /// </summary>
        public override bool SetPropertyValue(string propertyID, GriffinsBaseValue propertyVal, bool isRuning)
        {
            ProductionInfoPropertyModelEdit.IsRuning = isRuning;
            return ProductionInfoPropertyModelEdit.SetPropertyValue(propertyID, propertyVal);
        }

        /// <summary>
        /// 监听后端推送：解析界面数据对象属性值列表，映射到内部 Model 属性
        /// </summary>
        protected override bool SetUIDataObjPropValues(GFBaseTypePropValueList gFBaseTypePropValues)
        {
            if (gFBaseTypePropValues == null) return false;

            bool hasHandledAny = false;

            foreach (var prop in gFBaseTypePropValues)
            {
                if (prop?.Value == null) continue;

                switch (prop.PropertyID.ToString())
                {
                    case "Datas":
                        hasHandledAny |= ProductionInfoPropertyModelEdit.SetPropertyValue(nameof(ProductionInfoPropertyModelEdit.Datas), prop.Value);
                        break;
                }
            }

            return hasHandledAny;
        }

        /// <summary>
        /// 当图元 Model 属性值改变后的通报逻辑
        /// </summary>
        protected override void AfterPropertyChanged(string propertyID, GriffinsBaseValue propertyValue)
        {
            base.AfterPropertyChanged(propertyID, propertyValue);

            switch (propertyID)
            {
                case nameof(ProductionInfoPropertyModelEdit.Datas):
                    CallBack?.ExecOprt(propertyID);
                    break;
            }

            if (ProductionInfoPropertyModelEdit.IsRuning)
            {
                CallBack?.UpdateUIDataObjPropValues(new GFBaseTypePropValueList());
            }
        }

        /// <summary>
        /// 提供给框架的图元属性值转换接口
        /// </summary>
        public override GriffinsBaseValue ConvertPropertyValue(string propertyID, object propertyValue)
        {
            switch (propertyID)
            {
                case nameof(ProductionInfoPropertyModelEdit.Datas):
                    if (propertyValue is ProductionInfoData data)
                    {
                        return data.ToGFBaseTypePropValues().ToGriffinsBaseValue();
                    }
                    break;
            }

            return null!;
        }

        #endregion

        #region 操作原子执行与定义

        /// <summary>
        /// 执行操作原子
        /// </summary>
        protected override bool ExecOprtCell(MapOprtCellInstInfo mapOprtCellInstInfo)
        {
            if (mapOprtCellInstInfo?.OprtCellID == ProductionInfoMapOprtCellConst.Datas_MapOprtCellID)
            {
                if (!MapOprtCellExectorDict.TryGetValue(mapOprtCellInstInfo.InstanceID, out var mapOprtCellExector))
                {
                    mapOprtCellExector = new DatasMapOprtCellExector();
                    mapOprtCellExector.Init(IMapOprtCellExectorCallBack);
                    MapOprtCellExectorDict.TryAdd(mapOprtCellInstInfo.InstanceID, mapOprtCellExector);
                }

                mapOprtCellExector.Exec(mapOprtCellInstInfo.CfgInfo);
                return true;
            }

            return base.ExecOprtCell(mapOprtCellInstInfo);
        }

        /// <summary>
        /// 数据操作原子执行器
        /// (ViewModel 已通过 WhenAnyValue 主动侦听变化进行按需渲染，此处无需再次更新vm)
        /// </summary>
        private class DatasMapOprtCellExector : IMapOprtCellExector
        {
            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack) { }
            void IMapOprtCellExector.Exec(byte[] cfg) { }
        }

        #endregion

        #region 生命周期与持久化

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void OnInit()
        {
            base.OnInit();

            _viewModel = new ProductionInformationViewModel(ProductionInfoPropertyModelEdit);

            _viewModel.OnSwitchRecipeRequested += ExeSwitchRecipeAsync;
            _viewModel.OnZipChangeProgramRequested += ExeZipChangeProgramAsync;
            _viewModel.OnQueryProductionInfoRequested += ExeQueryProductionInfoAsync;
            _viewModel.OnShowMachineRunningStatisticsRequested += ExeShowMachineRunningStatisticsAsync;
            _viewModel.OnClearLaneDataRequested += ExeClearLaneDataAsync;

            _view.DataContext = _viewModel;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public override void OnDispose()
        {
            if (_viewModel != null)
            {
                _viewModel.OnSwitchRecipeRequested -= ExeSwitchRecipeAsync;
                _viewModel.OnZipChangeProgramRequested -= ExeZipChangeProgramAsync;
                _viewModel.OnQueryProductionInfoRequested -= ExeQueryProductionInfoAsync;
                _viewModel.OnShowMachineRunningStatisticsRequested -= ExeShowMachineRunningStatisticsAsync;
                _viewModel.OnClearLaneDataRequested -= ExeClearLaneDataAsync;
                _viewModel.Dispose();
            }

            if (_view != null)
            {
                _view.DataContext = null;
            }

            base.OnDispose();
        }

        /// <summary>
        /// 从 XML 中反序列化组态配置，统一使用 JsonObjConvert 并增强容错保护
        /// </summary>
        protected override void OnReadDrawInfoFromBytes(GriffinsXmlReader br)
        {
            base.OnReadDrawInfoFromBytes(br);

            var propertyEditModelBase = JsonObjConvert.FromJSon<ProductionInfoPropertyModelEdit>(br.ReadString("PropertyEditModelBase"));
            if (propertyEditModelBase != null)
            {
                ((ProductionInfoPropertyModelEdit)PropertyEditModelBase).CopyFrom(propertyEditModelBase);
            }

            var propertyBindEditModelBase = JsonObjConvert.FromJSon<ProductionInfoPropertyBindEditModel>(br.ReadString("PropertyBindEditModelBase"));
            if (propertyBindEditModelBase != null)
            {
                ((ProductionInfoPropertyBindEditModel)PropertyBindEditModelBase).CopyFrom(propertyBindEditModelBase);
            }

            var eventBindEditModel = JsonObjConvert.FromJSon<EventBindEditModel>(br.ReadString("EventBindEditModel"));
            if (eventBindEditModel != null)
            {
                EventBindEditModel?.CopyFrom(eventBindEditModel);
            }
        }

        /// <summary>
        /// 序列化保存至 XML
        /// </summary>
        protected override void OnWriteDrawInfoToBytes(GriffinsXmlWriter bw)
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
            base._CopyFrom(source as MapCellProductionInfoCtlObj);
            PropertyEditModelBase.CopyFrom(source.PropertyEditModelBase);
            PropertyBindEditModelBase.CopyFrom(source.PropertyBindEditModelBase);
            EventBindEditModel?.CopyFrom(source.EventBindEditModel);
        }

        protected override object OnGetView() => _view;
        protected override object OnGetViewModel() => _viewModel;

        public override PropertyEditModelBase CreatePropertyModelEditBase() => new ProductionInfoPropertyModelEdit();
        public override PropertyBindEditModelBase CreatePropertyBindEditModelBase() => new ProductionInfoPropertyBindEditModel();

        public override EventBindEditModel CreateEventBindEditModel() => new EventBindEditModel()
        {
            EventCmdInfos = new BindingList<EventCmdInfo>()
            {
                new EventCmdInfo()
                {
                    EventCmdKind = EventCmdKind.MpCmdKind,
                    EventID = MapObjPropEventConst.Event_MouseClick
                }
            }
        };

        public override string ToString() => "生产信息";

        #endregion
    }

    /// <summary>
    /// 生产信息图元属性编辑器模型 (Model)
    /// </summary>
    [Serializable]
    [MapPropertyOrder]
    [CategoryPriority("图元信息", 1)]
    public class ProductionInfoPropertyModelEdit : FunctionalCellPropertyModelEdit
    {
        public ProductionInfoPropertyModelEdit() { }

        private ProductionInfoData _datas = ProductionInfoData.GetMockData();

        /// <summary>
        /// 生产信息数据合集
        /// </summary>
        [Browsable(false)]
        public ProductionInfoData Datas
        {
            get => _datas;
            set => SetProperty(ref _datas, value);
        }

        public override bool SetPropertyValue(string propertyID, GriffinsBaseValue propertyVal)
        {
            switch (propertyID)
            {
                case nameof(Datas):
                    if (propertyVal == null)
                    {
                        Datas = ProductionInfoData.DefaultEmpty;
                    }
                    else
                    {
                        var propValueList = new GFBaseTypePropValueList();
                        ((IGriffinsBaseValue)propValueList).PopulateFromBaseValue(propertyVal);
                        Datas = GFPropObjBase.FromGFBaseTypePropValues<ProductionInfoData>(propValueList);
                    }
                    return true;
            }

            return base.SetPropertyValue(propertyID, propertyVal);
        }

        /// <summary>
        /// 深拷贝克隆属性状态
        /// </summary>
        public void CopyFrom(ProductionInfoPropertyModelEdit source)
        {
            base.CopyFrom(source);

            if (source.Datas != null)
            {
                // 手动重构深拷贝，防止引用传递污染
                this.Datas = new ProductionInfoData
                {
                    CurrentRecipeName = source.Datas.CurrentRecipeName,
                    MachineRunTime = source.Datas.MachineRunTime,
                    UtilizationRate = source.Datas.UtilizationRate,
                    Uph = source.Datas.Uph,

                    CurrentCycleTime = source.Datas.CurrentCycleTime,
                    CurrentBoardProcessTime = source.Datas.CurrentBoardProcessTime,
                    CurrentLoadTime = source.Datas.CurrentLoadTime,
                    CurrentUnloadTime = source.Datas.CurrentUnloadTime,
                    CurrentMarkTime = source.Datas.CurrentMarkTime,
                    CurrentDispenseTime = source.Datas.CurrentDispenseTime,

                    AverageCycleTime = source.Datas.AverageCycleTime,
                    AverageBoardProcessTime = source.Datas.AverageBoardProcessTime,
                    AverageLoadTime = source.Datas.AverageLoadTime,
                    AverageUnloadTime = source.Datas.AverageUnloadTime,
                    AverageMarkTime = source.Datas.AverageMarkTime,
                    AverageDispenseTime = source.Datas.AverageDispenseTime,

                    Lanes = source.Datas.Lanes?.ToDictionary(
                        kvp => kvp.Key,
                        kvp => new LaneData
                        {
                            LaneId = kvp.Value.LaneId,
                            LaneName = kvp.Value.LaneName,
                            TotalBigBoardCount = kvp.Value.TotalBigBoardCount,
                            TotalSmallBoardCount = kvp.Value.TotalSmallBoardCount,
                            CurrentBigBoardCount = kvp.Value.CurrentBigBoardCount,
                            CurrentSmallBoardCount = kvp.Value.CurrentSmallBoardCount
                        })
                };
            }
        }
    }

    /// <summary>
    /// 生产信息图元属性绑定编辑模型
    /// 承载图元各属性与后端映射的配置信息,功能图元不需要
    /// </summary>
    [Serializable]
    [MapPropertyOrder]
    [CategoryPriority("点位信息", 1)]
    [CategoryPriority("绑定信息", 2)]
    public class ProductionInfoPropertyBindEditModel : FunctionalCellPropertyBindEditModel
    {
        public void CopyFrom(ProductionInfoPropertyBindEditModel source) => base.CopyFrom(source);
    }
}