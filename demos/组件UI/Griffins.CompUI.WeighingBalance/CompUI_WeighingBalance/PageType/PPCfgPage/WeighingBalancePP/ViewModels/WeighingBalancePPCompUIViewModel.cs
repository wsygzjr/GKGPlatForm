using GF_Gereric;
using GKG;
using GKG.UI;
using GKG.SubMM.Dispenser;
using Griffins.Map.UI;
using ReactiveUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Griffins.CompUI.WeighingBalance.CompUI_WeighingBalance.PageType.PPCfgPage.WeighingBalancePP.ViewModels
{
    /// <summary>
    /// 称重配方参数视图模型
    /// </summary>
    public class WeighingBalancePPCompUIViewModel : ReactiveObject, IDisposable
    {
        private readonly bool isDesign;
        private readonly ICompUIRunTimeCallBack callBack;
        // 重点：左侧阀列表只展示 ID 和别名，真正可编辑的参数按阀 ID 缓存在这里；
        // 切换阀实例时直接切换编辑模型，避免把不同阀的数据混到同一个表单对象里。
        private readonly Dictionary<string, WeighingParameterEditorViewModel> parameterMap = new();
        private WeighingBalanceSubMachineModulesPPCfg loadedData = new();
        private bool readOnly;
        private bool isApplyingData;
        private ValveItemViewModel selectedValveItem;
        private WeighingParameterEditorViewModel selectedParameter;

        /// <summary>
        /// 修改后事件
        /// </summary>
        public event EventHandler AfterModified;

        /// <summary>
        /// 阀实例ID列表
        /// </summary>
        public ObservableCollection<ValveItemViewModel> ValveItems { get; } = new();

        /// <summary>
        /// 每个阀对应一套独立编辑模型和 GKG 控件树，避免切换阀时复用同一套下拉控件状态。
        /// </summary>
        public ObservableCollection<WeighingParameterEditorViewModel> ParameterEditors { get; } = new();

        /// <summary>
        /// 是否只读
        /// </summary>
        public bool ReadOnly
        {
            get => readOnly;
            set
            {
                this.RaiseAndSetIfChanged(ref readOnly, value);
                RaiseStateProperties();
            }
        }

        /// <summary>
        /// 是否可编辑
        /// </summary>
        public bool CanEdit => !ReadOnly && HasSelected;

        /// <summary>
        /// 是否有选中阀
        /// </summary>
        public bool HasSelected => SelectedParameter != null;

        /// <summary>
        /// 是否为空
        /// </summary>
        public bool IsEmpty => ValveItems.Count == 0;

        /// <summary>
        /// 是否单点称重模式
        /// </summary>
        public bool IsSinglePointMode => SelectedParameter?.IsSinglePointMode ?? false;

        /// <summary>
        /// 是否流量称重模式
        /// </summary>
        public bool IsMassFlowMode => SelectedParameter?.IsMassFlowMode ?? false;

        /// <summary>
        /// 当前选中阀
        /// </summary>
        public ValveItemViewModel SelectedValveItem
        {
            get => selectedValveItem;
            set
            {
                if (ReferenceEquals(selectedValveItem, value))
                {
                    return;
                }

                this.RaiseAndSetIfChanged(ref selectedValveItem, value);
                SelectedParameter = value != null && parameterMap.TryGetValue(value.ValveID, out var parameter)
                    ? parameter
                    : null;
            }
        }

        /// <summary>
        /// 当前选中称重参数
        /// </summary>
        public WeighingParameterEditorViewModel SelectedParameter
        {
            get => selectedParameter;
            private set
            {
                if (selectedParameter != null)
                {
                    selectedParameter.AfterModified -= OnSelectedParameterModified;
                    selectedParameter.IsSelected = false;
                }

                this.RaiseAndSetIfChanged(ref selectedParameter, value);

                if (selectedParameter != null)
                {
                    selectedParameter.IsSelected = true;
                    selectedParameter.AfterModified += OnSelectedParameterModified;
                }

                RaiseStateProperties();
            }
        }

        /// <summary>
        /// 构造函数（设计时）
        /// </summary>
        public WeighingBalancePPCompUIViewModel()
        {
            isDesign = true;
            SetData(CreateDesignData());
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="isDesign">是否设计模式</param>
        /// <param name="callBack">回调</param>
        public WeighingBalancePPCompUIViewModel(bool isDesign, ICompUIRunTimeCallBack callBack)
        {
            this.isDesign = isDesign;
            this.callBack = callBack;
        }

        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="data">数据</param>
        public void SetData(WeighingBalanceSubMachineModulesPPCfg data)
        {
            ApplyWithoutModified(() =>
            {
                loadedData = CloneData(data);
                var initCfg = LoadInitCfg();
                ApplyValveItems(initCfg, loadedData.WeighingParameters);
            });
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns>数据</returns>
        public WeighingBalanceSubMachineModulesPPCfg GetData()
        {
            var result = CloneData(loadedData);
            var list = new WeighingParameterArray();

            foreach (var valve in ValveItems)
            {
                if (valve == null)
                {
                    continue;
                }

                if (!parameterMap.TryGetValue(valve.ValveID, out var parameter))
                {
                    parameter = CreateParameterEditor(valve.ValveID, null);
                    parameterMap[valve.ValveID] = parameter;
                    ParameterEditors.Add(parameter);
                }

                var model = parameter.ToModel();
                model.FunctionHeadID = valve.ValveID;
                list.Add(model);
            }

            result.WeighingParameters = list;
            loadedData = CloneData(result);
            return result;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            foreach (var parameter in parameterMap.Values)
            {
                parameter.AfterModified -= OnSelectedParameterModified;
            }
        }

        /// <summary>
        /// 使用初始化配置和配方参数刷新阀列表
        /// </summary>
        /// <param name="initCfg">初始化配置</param>
        /// <param name="parameters">配方参数</param>
        private void ApplyValveItems(WeighingBalanceSubMachineModulesInitCfg initCfg, WeighingParameterArray parameters)
        {
            var previousValveID = SelectedValveItem?.ValveID;
            var valveDict = BuildValveDictionary(initCfg, parameters);

            SelectedParameter = null;
            ValveItems.Clear();
            ParameterEditors.Clear();
            parameterMap.Clear();

            foreach (var pair in valveDict)
            {
                var parameter = FindParameter(parameters, pair.Key);
                var editor = CreateParameterEditor(pair.Key, parameter);
                parameterMap[pair.Key] = editor;
                ParameterEditors.Add(editor);
                ValveItems.Add(new ValveItemViewModel(pair.Key, pair.Value));
            }

            var selected = !string.IsNullOrWhiteSpace(previousValveID)
                ? ValveItems.FirstOrDefault(item => item.ValveID == previousValveID)
                : null;
            SelectedValveItem = selected ?? ValveItems.FirstOrDefault();
            RaiseStateProperties();
        }

        /// <summary>
        /// 创建参数编辑模型
        /// </summary>
        /// <param name="valveID">阀实例ID</param>
        /// <param name="parameter">称重参数</param>
        /// <returns>编辑模型</returns>
        private WeighingParameterEditorViewModel CreateParameterEditor(string valveID, WeighingParameter parameter)
        {
            var editor = new WeighingParameterEditorViewModel();
            editor.CopyFrom(parameter ?? new WeighingParameter { FunctionHeadID = valveID });
            editor.FunctionHeadID = valveID;
            editor.AfterModified += (_, __) => NotifyDataModified();
            return editor;
        }

        /// <summary>
        /// 构建阀字典
        /// </summary>
        /// <param name="initCfg">初始化配置</param>
        /// <param name="parameters">配方参数</param>
        /// <returns>阀字典</returns>
        private Dictionary<string, string> BuildValveDictionary(WeighingBalanceSubMachineModulesInitCfg initCfg, WeighingParameterArray parameters)
        {
            if (initCfg?.ValveID != null && initCfg.ValveID.Count > 0)
            {
                return initCfg.ValveID
                    .Where(pair => !string.IsNullOrWhiteSpace(pair.Key))
                    .ToDictionary(pair => pair.Key, pair => string.IsNullOrWhiteSpace(pair.Value) ? pair.Key : pair.Value);
            }

            var fallback = new Dictionary<string, string>();
            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    if (parameter == null || string.IsNullOrWhiteSpace(parameter.FunctionHeadID))
                    {
                        continue;
                    }

                    if (!fallback.ContainsKey(parameter.FunctionHeadID))
                    {
                        fallback[parameter.FunctionHeadID] = parameter.FunctionHeadID;
                    }
                }
            }

            if (fallback.Count == 0 && isDesign)
            {
                fallback["Valve1"] = "阀1";
                fallback["Valve2"] = "阀2";
            }

            return fallback;
        }

        /// <summary>
        /// 加载初始化配置
        /// </summary>
        /// <returns>初始化配置</returns>
        private WeighingBalanceSubMachineModulesInitCfg LoadInitCfg()
        {
            if (isDesign || callBack == null)
            {
                return null;
            }

            try
            {
                var result = callBack.ExecConfigSvrCtlCmd(
                    WeighingBalanceSubMachineModulesConst.GetInitCfgCmdID,
                    new GFBaseTypeParamValueList());
                var json = TryExtractJson(result);
                if (string.IsNullOrWhiteSpace(json))
                {
                    return null;
                }

                return JsonObjConvert.FromJSonBytes<WeighingBalanceSubMachineModulesInitCfg>(Encoding.UTF8.GetBytes(json));
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 提取命令返回的 JSON
        /// </summary>
        /// <param name="value">返回值</param>
        /// <param name="depth">递归深度</param>
        /// <returns>JSON文本</returns>
        private static string TryExtractJson(object value, int depth = 0)
        {
            if (value == null || depth > 6)
            {
                return null;
            }

            if (value is string text)
            {
                return ExtractJsonText(text);
            }

            if (value is byte[] bytes)
            {
                return ExtractJsonText(Encoding.UTF8.GetString(bytes));
            }

            if (value is IEnumerable enumerable && value is not string)
            {
                foreach (var item in enumerable)
                {
                    var name = TryGetMemberAsString(item, "Name")
                        ?? TryGetMemberAsString(item, "Key")
                        ?? TryGetMemberAsString(item, "ParamID")
                        ?? TryGetMemberAsString(item, "ID");

                    if (string.Equals(name, "Result", StringComparison.OrdinalIgnoreCase))
                    {
                        var payload = TryGetMember(item, "Value")
                            ?? TryGetMember(item, "ParamValue")
                            ?? TryGetMember(item, "Val");
                        var resultJson = TryExtractJson(payload, depth + 1);
                        if (!string.IsNullOrWhiteSpace(resultJson))
                        {
                            return resultJson;
                        }
                    }

                    var itemJson = TryExtractJson(item, depth + 1);
                    if (!string.IsNullOrWhiteSpace(itemJson))
                    {
                        return itemJson;
                    }
                }
            }

            foreach (var memberName in new[] { "Value", "ParamValue", "Val", "BaseValue", "ObjValue", "Data", "StringValue" })
            {
                var payload = TryGetMember(value, memberName);
                if (payload == null || ReferenceEquals(payload, value))
                {
                    continue;
                }

                var resultJson = TryExtractJson(payload, depth + 1);
                if (!string.IsNullOrWhiteSpace(resultJson))
                {
                    return resultJson;
                }
            }

            return ExtractJsonText(value.ToString());
        }

        /// <summary>
        /// 从文本中截取 JSON
        /// </summary>
        /// <param name="text">文本</param>
        /// <returns>JSON文本</returns>
        private static string ExtractJsonText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return null;
            }

            var trimmed = text.Trim();
            if (trimmed.StartsWith("{") || trimmed.StartsWith("["))
            {
                return trimmed;
            }

            var start = trimmed.IndexOf('{');
            var end = trimmed.LastIndexOf('}');
            if (start >= 0 && end > start)
            {
                return trimmed.Substring(start, end - start + 1);
            }

            return null;
        }

        /// <summary>
        /// 通过反射读取成员
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="memberName">成员名</param>
        /// <returns>成员值</returns>
        private static object TryGetMember(object obj, string memberName)
        {
            if (obj == null)
            {
                return null;
            }

            try
            {
                var type = obj.GetType();
                var prop = type.GetProperty(memberName, BindingFlags.Public | BindingFlags.Instance);
                if (prop != null)
                {
                    return prop.GetValue(obj);
                }

                var field = type.GetField(memberName, BindingFlags.Public | BindingFlags.Instance);
                return field?.GetValue(obj);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 通过反射读取字符串成员
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="memberName">成员名</param>
        /// <returns>字符串</returns>
        private static string TryGetMemberAsString(object obj, string memberName)
        {
            return TryGetMember(obj, memberName)?.ToString();
        }

        /// <summary>
        /// 查找阀对应参数
        /// </summary>
        /// <param name="parameters">参数数组</param>
        /// <param name="valveID">阀实例ID</param>
        /// <returns>称重参数</returns>
        private static WeighingParameter FindParameter(WeighingParameterArray parameters, string valveID)
        {
            if (parameters == null || string.IsNullOrWhiteSpace(valveID))
            {
                return null;
            }

            return parameters.FirstOrDefault(parameter =>
                parameter != null &&
                string.Equals(parameter.FunctionHeadID, valveID, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// 复制配方配置
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns>复制数据</returns>
        private static WeighingBalanceSubMachineModulesPPCfg CloneData(WeighingBalanceSubMachineModulesPPCfg data)
        {
            var clone = new WeighingBalanceSubMachineModulesPPCfg
            {
                WeighingParameters = new WeighingParameterArray(),
            };

            if (data?.WeighingParameters == null)
            {
                return clone;
            }

            foreach (var parameter in data.WeighingParameters)
            {
                var editor = new WeighingParameterEditorViewModel();
                editor.CopyFrom(parameter);
                clone.WeighingParameters.Add(editor.ToModel());
            }

            return clone;
        }

        /// <summary>
        /// 触发状态属性刷新
        /// </summary>
        private void RaiseStateProperties()
        {
            this.RaisePropertyChanged(nameof(CanEdit));
            this.RaisePropertyChanged(nameof(HasSelected));
            this.RaisePropertyChanged(nameof(IsEmpty));
            this.RaisePropertyChanged(nameof(IsSinglePointMode));
            this.RaisePropertyChanged(nameof(IsMassFlowMode));
        }

        /// <summary>
        /// 选中参数修改
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="e">事件参数</param>
        private void OnSelectedParameterModified(object sender, EventArgs e)
        {
            RaiseStateProperties();
        }

        /// <summary>
        /// 通知数据修改
        /// </summary>
        private void NotifyDataModified()
        {
            if (isApplyingData)
            {
                return;
            }

            AfterModified?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 应用数据时屏蔽修改事件
        /// </summary>
        /// <param name="action">动作</param>
        private void ApplyWithoutModified(Action action)
        {
            isApplyingData = true;
            try
            {
                action();
            }
            finally
            {
                isApplyingData = false;
                RaiseStateProperties();
            }
        }

        /// <summary>
        /// 创建设计时数据
        /// </summary>
        /// <returns>配方配置</returns>
        private static WeighingBalanceSubMachineModulesPPCfg CreateDesignData()
        {
            var cfg = new WeighingBalanceSubMachineModulesPPCfg
            {
                WeighingParameters = new WeighingParameterArray
                {
                    new WeighingParameter { FunctionHeadID = "Valve1", Enabled = true },
                    new WeighingParameter { FunctionHeadID = "Valve2", Enabled = false },
                },
            };

            return cfg;
        }
    }

    /// <summary>
    /// 阀实例ID列表项
    /// </summary>
    public class ValveItemViewModel
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="valveID">阀实例ID</param>
        /// <param name="alias">别名</param>
        public ValveItemViewModel(string valveID, string alias)
        {
            ValveID = valveID ?? string.Empty;
            Alias = string.IsNullOrWhiteSpace(alias) ? ValveID : alias;
        }

        /// <summary>
        /// 阀实例ID
        /// </summary>
        public string ValveID { get; }

        /// <summary>
        /// 别名
        /// </summary>
        public string Alias { get; }
    }

    /// <summary>
    /// 单个称重参数编辑模型
    /// </summary>
    public class WeighingParameterEditorViewModel : ReactiveObject
    {
        private bool isApplyingData;
        private bool isSelected;
        private WeighingTimeItem[] weighingTimeItems = Array.Empty<WeighingTimeItem>();

        /// <summary>
        /// 修改后事件
        /// </summary>
        public event EventHandler AfterModified;

        /// <summary>
        /// 当前编辑模型是否为左侧选中的阀；只控制显示，不参与保存。
        /// </summary>
        public bool IsSelected
        {
            get => isSelected;
            set => this.RaiseAndSetIfChanged(ref isSelected, value);
        }

        public TextInputViewModel FunctionHeadIDViewModel { get; } = new() { IsEnabled = false };

        public ToggleSwitchViewModel EnabledViewModel { get; } = new();

        public ComboxViewModel WeighingModeViewModel { get; } = CreateComboViewModel();

        public NumericUpDownViewModel PreOpenValveTimeViewModel { get; } = CreateNumericViewModel(0, 6535, 1, 0);

        public NumericUpDownViewModel WeightCorrectionPercentViewModel { get; } = CreateNumericViewModel(0, 100, 0.01m, 2);

        public NumericUpDownViewModel WeightReferenceViewModel { get; } = CreateNumericViewModel(0, 6535, 0.1m, 1);

        public NumericUpDownViewModel WeighingPositionXViewModel { get; } = CreateNumericViewModel(-99999, 99999, 0.01m, 2);

        public NumericUpDownViewModel WeighingPositionYViewModel { get; } = CreateNumericViewModel(-99999, 99999, 0.01m, 2);

        public NumericUpDownViewModel WeighingPositionZViewModel { get; } = CreateNumericViewModel(-99999, 99999, 0.01m, 2);

        public ComboxViewModel PositionModeViewModel { get; } = CreateComboViewModel();

        public NumericUpDownViewModel SinglePointPointCountViewModel { get; } = CreateNumericViewModel(0, 6535, 1, 0);

        public NumericUpDownViewModel SinglePointSprayCountViewModel { get; } = CreateNumericViewModel(0, 6535, 1, 0);

        public NumericUpDownViewModel SinglePointOnceIntervalViewModel { get; } = CreateNumericViewModel(0, 6535, 0.1m, 1);

        public NumericUpDownViewModel SinglePointCycleCountViewModel { get; } = CreateNumericViewModel(0, 6535, 1, 0);

        public NumericUpDownViewModel SinglePointDotUpperLimitViewModel { get; } = CreateNumericViewModel(0, 6535, 0.01m, 2);

        public NumericUpDownViewModel SinglePointDotLowerLimitViewModel { get; } = CreateNumericViewModel(0, 6535, 0.01m, 2);

        public NumericUpDownViewModel MassFlowPointCountViewModel { get; } = CreateNumericViewModel(0, 6535, 1, 0);

        public NumericUpDownViewModel MassFlowSprayCountViewModel { get; } = CreateNumericViewModel(0, 6535, 1, 0);

        public NumericUpDownViewModel MassFlowOnceIntervalViewModel { get; } = CreateNumericViewModel(0, 6535, 0.1m, 1);

        public NumericUpDownViewModel MassFlowCycleCountViewModel { get; } = CreateNumericViewModel(0, 6535, 1, 0);

        public NumericUpDownViewModel QuantitativeWeightViewModel { get; } = CreateNumericViewModel(0, 6535, 0.01m, 2);

        public NumericUpDownViewModel QuantitativeSprayCountViewModel { get; } = CreateNumericViewModel(0, 6535, 1, 0);

        public NumericUpDownViewModel QuantitativeOnceIntervalViewModel { get; } = CreateNumericViewModel(0, 6535, 0.1m, 1);

        public NumericUpDownViewModel QuantitativeCycleCountViewModel { get; } = CreateNumericViewModel(0, 6535, 1, 0);

        public NumericUpDownViewModel QuantitativeTotalWeightViewModel { get; } = CreateNumericViewModel(0, 6535, 0.01m, 2);

        public NumericUpDownViewModel QuantitativeNRadioWeightViewModel { get; } = CreateNumericViewModel(0, 6535, 0.01m, 2);

        public NumericUpDownViewModel QuantitativeUpperLimitViewModel { get; } = CreateNumericViewModel(0, 6535, 0.01m, 2);

        public NumericUpDownViewModel QuantitativeLowerLimitViewModel { get; } = CreateNumericViewModel(0, 6535, 0.01m, 2);

        public ToggleSwitchViewModel WeighingTimeEnabledViewModel { get; } = new();

        public NumericUpDownViewModel WeighingTimeHourViewModel { get; } = CreateNumericViewModel(0, 23, 1, 0);

        public NumericUpDownViewModel WeighingTimeMinuteViewModel { get; } = CreateNumericViewModel(0, 59, 1, 0);

        /// <summary>
        /// 构造函数
        /// </summary>
        public WeighingParameterEditorViewModel()
        {
            WeighingModeViewModel.ItemsSource = new[]
            {
                CreateComboItem(WeighingType.SinglePointWeighing, "单点称重"),
                CreateComboItem(WeighingType.MassFlowWeighing, "流量称重"),
            };
            WeighingModeViewModel.DisplayMemberPath = nameof(ComBoxItem.DisplayName);

            PositionModeViewModel.ItemsSource = new[]
            {
                CreateComboItem(PositionTeachMode.CCDPosition, "CCD位置"),
                CreateComboItem(PositionTeachMode.FunctionHeadPosition, "功能头位置"),
            };
            PositionModeViewModel.DisplayMemberPath = nameof(ComBoxItem.DisplayName);

            SetSelectedValue(WeighingModeViewModel, WeighingType.SinglePointWeighing);
            SetSelectedValue(PositionModeViewModel, PositionTeachMode.CCDPosition);
            // 重点：GKG.UI 控件的实际宽度存放在各自的 ViewModel 中。
            // 每个阀实例都会创建独立编辑模型，所以这里主动写入 Medium，避免切换阀2/阀3后回到默认 Small 被截断。
            ApplyControlWidthLevel("Medium");
            UpdateWeighingTimeInputState();
            SubscribeControlViewModels();
        }

        /// <summary>
        /// 是否单点称重
        /// </summary>
        public bool IsSinglePointMode => WeighingMode == WeighingType.SinglePointWeighing;

        /// <summary>
        /// 是否流量称重
        /// </summary>
        public bool IsMassFlowMode => WeighingMode == WeighingType.MassFlowWeighing;

        /// <summary>
        /// 列表显示点数
        /// </summary>
        public decimal PointCount => IsMassFlowMode ? MassFlowPointCount : SinglePointPointCount;

        /// <summary>
        /// 功能头编号
        /// </summary>
        public string FunctionHeadID
        {
            get => FunctionHeadIDViewModel.Text;
            set => FunctionHeadIDViewModel.Text = value ?? string.Empty;
        }

        /// <summary>
        /// 启用
        /// </summary>
        public bool Enabled
        {
            get => EnabledViewModel.IsChecked;
            set => EnabledViewModel.IsChecked = value;
        }

        /// <summary>
        /// 称重模式
        /// </summary>
        public WeighingType WeighingMode
        {
            get => GetSelectedValue(WeighingModeViewModel, WeighingType.SinglePointWeighing);
            set
            {
                SetSelectedValue(WeighingModeViewModel, value);
                this.RaisePropertyChanged(nameof(WeighingModeDisplayName));
                this.RaisePropertyChanged(nameof(IsSinglePointMode));
                this.RaisePropertyChanged(nameof(IsMassFlowMode));
                this.RaisePropertyChanged(nameof(PointCount));
            }
        }

        /// <summary>
        /// 称重模式显示名
        /// </summary>
        public string WeighingModeDisplayName
        {
            get => WeighingMode switch
            {
                WeighingType.MassFlowWeighing => "流量称重",
                _ => "单点称重",
            };
            set
            {
                WeighingMode = value == "流量称重"
                    ? WeighingType.MassFlowWeighing
                    : WeighingType.SinglePointWeighing;
            }
        }

        /// <summary>
        /// 提前开阀时间
        /// </summary>
        public decimal PreOpenValveTime
        {
            get => PreOpenValveTimeViewModel.Value;
            set => PreOpenValveTimeViewModel.Value = value;
        }

        /// <summary>
        /// 单点喷胶点数
        /// </summary>
        public decimal SinglePointPointCount
        {
            get => SinglePointPointCountViewModel.Value;
            set
            {
                SinglePointPointCountViewModel.Value = value;
                this.RaisePropertyChanged(nameof(PointCount));
            }
        }

        /// <summary>
        /// 单点喷胶次数
        /// </summary>
        public decimal SinglePointSprayCount
        {
            get => SinglePointSprayCountViewModel.Value;
            set => SinglePointSprayCountViewModel.Value = value;
        }

        /// <summary>
        /// 单点间隔
        /// </summary>
        public decimal SinglePointOnceInterval
        {
            get => SinglePointOnceIntervalViewModel.Value;
            set => SinglePointOnceIntervalViewModel.Value = value;
        }

        /// <summary>
        /// 单点重复次数
        /// </summary>
        public decimal SinglePointCycleCount
        {
            get => SinglePointCycleCountViewModel.Value;
            set => SinglePointCycleCountViewModel.Value = value;
        }

        /// <summary>
        /// 单点重量上限
        /// </summary>
        public decimal SinglePointDotUpperLimit
        {
            get => SinglePointDotUpperLimitViewModel.Value;
            set => SinglePointDotUpperLimitViewModel.Value = value;
        }

        /// <summary>
        /// 单点重量下限
        /// </summary>
        public decimal SinglePointDotLowerLimit
        {
            get => SinglePointDotLowerLimitViewModel.Value;
            set => SinglePointDotLowerLimitViewModel.Value = value;
        }

        /// <summary>
        /// 流量喷胶点数
        /// </summary>
        public decimal MassFlowPointCount
        {
            get => MassFlowPointCountViewModel.Value;
            set
            {
                MassFlowPointCountViewModel.Value = value;
                this.RaisePropertyChanged(nameof(PointCount));
            }
        }

        /// <summary>
        /// 流量喷胶次数
        /// </summary>
        public decimal MassFlowSprayCount
        {
            get => MassFlowSprayCountViewModel.Value;
            set => MassFlowSprayCountViewModel.Value = value;
        }

        /// <summary>
        /// 流量间隔
        /// </summary>
        public decimal MassFlowOnceInterval
        {
            get => MassFlowOnceIntervalViewModel.Value;
            set => MassFlowOnceIntervalViewModel.Value = value;
        }

        /// <summary>
        /// 流量重复次数
        /// </summary>
        public decimal MassFlowCycleCount
        {
            get => MassFlowCycleCountViewModel.Value;
            set => MassFlowCycleCountViewModel.Value = value;
        }

        /// <summary>
        /// 定量喷胶重量
        /// </summary>
        public decimal QuantitativeWeight
        {
            get => QuantitativeWeightViewModel.Value;
            set => QuantitativeWeightViewModel.Value = value;
        }

        /// <summary>
        /// 定量喷胶次数
        /// </summary>
        public decimal QuantitativeSprayCount
        {
            get => QuantitativeSprayCountViewModel.Value;
            set => QuantitativeSprayCountViewModel.Value = value;
        }

        /// <summary>
        /// 定量间隔
        /// </summary>
        public decimal QuantitativeOnceInterval
        {
            get => QuantitativeOnceIntervalViewModel.Value;
            set => QuantitativeOnceIntervalViewModel.Value = value;
        }

        /// <summary>
        /// 定量重复次数
        /// </summary>
        public decimal QuantitativeCycleCount
        {
            get => QuantitativeCycleCountViewModel.Value;
            set => QuantitativeCycleCountViewModel.Value = value;
        }

        /// <summary>
        /// 定量总重量
        /// </summary>
        public decimal QuantitativeTotalWeight
        {
            get => QuantitativeTotalWeightViewModel.Value;
            set => QuantitativeTotalWeightViewModel.Value = value;
        }

        /// <summary>
        /// N倍出胶系数
        /// </summary>
        public decimal QuantitativeNRadioWeight
        {
            get => QuantitativeNRadioWeightViewModel.Value;
            set => QuantitativeNRadioWeightViewModel.Value = value;
        }

        /// <summary>
        /// 定量重量上限
        /// </summary>
        public decimal QuantitativeUpperLimit
        {
            get => QuantitativeUpperLimitViewModel.Value;
            set => QuantitativeUpperLimitViewModel.Value = value;
        }

        /// <summary>
        /// 定量重量下限
        /// </summary>
        public decimal QuantitativeLowerLimit
        {
            get => QuantitativeLowerLimitViewModel.Value;
            set => QuantitativeLowerLimitViewModel.Value = value;
        }

        /// <summary>
        /// 称重补偿百分比
        /// </summary>
        public decimal WeightCorrectionPercent
        {
            get => WeightCorrectionPercentViewModel.Value;
            set => WeightCorrectionPercentViewModel.Value = value;
        }

        /// <summary>
        /// 称重位置X
        /// </summary>
        public decimal WeighingPositionX
        {
            get => WeighingPositionXViewModel.Value;
            set => WeighingPositionXViewModel.Value = value;
        }

        /// <summary>
        /// 称重位置Y
        /// </summary>
        public decimal WeighingPositionY
        {
            get => WeighingPositionYViewModel.Value;
            set => WeighingPositionYViewModel.Value = value;
        }

        /// <summary>
        /// 称重位置Z
        /// </summary>
        public decimal WeighingPositionZ
        {
            get => WeighingPositionZViewModel.Value;
            set => WeighingPositionZViewModel.Value = value;
        }

        /// <summary>
        /// 位置教导模式
        /// </summary>
        public PositionTeachMode PositionMode
        {
            get => GetSelectedValue(PositionModeViewModel, PositionTeachMode.CCDPosition);
            set
            {
                SetSelectedValue(PositionModeViewModel, value);
                this.RaisePropertyChanged(nameof(PositionModeDisplayName));
            }
        }

        /// <summary>
        /// 位置教导模式显示名
        /// </summary>
        public string PositionModeDisplayName
        {
            get => PositionMode == PositionTeachMode.FunctionHeadPosition ? "功能头位置" : "CCD位置";
            set
            {
                PositionMode = value == "功能头位置"
                    ? PositionTeachMode.FunctionHeadPosition
                    : PositionTeachMode.CCDPosition;
            }
        }

        /// <summary>
        /// 定时定点称重启用
        /// </summary>
        public bool WeighingTimeEnabled
        {
            get => WeighingTimeEnabledViewModel.IsChecked;
            set
            {
                WeighingTimeEnabledViewModel.IsChecked = value;
                UpdateWeighingTimeInputState();
                this.RaisePropertyChanged(nameof(IsWeighingTimeInputEnabled));
            }
        }

        /// <summary>
        /// 定时时间输入是否可用
        /// </summary>
        public bool IsWeighingTimeInputEnabled => WeighingTimeEnabled;

        /// <summary>
        /// 称重时间小时
        /// </summary>
        public decimal WeighingTimeHour
        {
            get => WeighingTimeHourViewModel.Value;
            set => WeighingTimeHourViewModel.Value = value;
        }

        /// <summary>
        /// 称重时间分钟
        /// </summary>
        public decimal WeighingTimeMinute
        {
            get => WeighingTimeMinuteViewModel.Value;
            set => WeighingTimeMinuteViewModel.Value = value;
        }

        /// <summary>
        /// 基准单点重量
        /// </summary>
        public decimal WeightReference
        {
            get => WeightReferenceViewModel.Value;
            set => WeightReferenceViewModel.Value = value;
        }

        /// <summary>
        /// 从后端模型复制
        /// </summary>
        /// <param name="model">后端模型</param>
        public void CopyFrom(WeighingParameter model)
        {
            model ??= new WeighingParameter();

            ApplyWithoutModified(() =>
            {
                FunctionHeadID = model.FunctionHeadID ?? string.Empty;
                Enabled = model.Enabled;
                WeighingMode = model.WeighingMode;
                PreOpenValveTime = model.PreOpenValveTime;

                var singlePoint = model.SinglePointWeighingParams ?? new SinglePointWeighingParams();
                SinglePointPointCount = singlePoint.PointCount;
                SinglePointSprayCount = singlePoint.SprayCount;
                SinglePointOnceInterval = (decimal)singlePoint.OnceInterval;
                SinglePointCycleCount = singlePoint.CycleCount;
                SinglePointDotUpperLimit = (decimal)singlePoint.dDotUpperLimit;
                SinglePointDotLowerLimit = (decimal)singlePoint.dDotLowerLimit;

                var massFlow = model.MassFlowWeighingParams ?? new MassFlowWeighingParams();
                MassFlowPointCount = massFlow.PointCount;
                MassFlowSprayCount = massFlow.SprayCount;
                MassFlowOnceInterval = (decimal)massFlow.OnceInterval;
                MassFlowCycleCount = massFlow.CycleCount;

                var quantitative = model.QuantitativeWeighingParameters ?? new QuantitativeWeighingParameters();
                QuantitativeWeight = (decimal)quantitative.QuantitativeWeight;
                QuantitativeSprayCount = quantitative.SprayCount;
                QuantitativeOnceInterval = (decimal)quantitative.OnceInterval;
                QuantitativeCycleCount = quantitative.CycleCount;
                QuantitativeTotalWeight = (decimal)quantitative.TotalWeight;
                QuantitativeNRadioWeight = (decimal)quantitative.dNRadioWeight;
                QuantitativeUpperLimit = (decimal)quantitative.dUpperLimit;
                QuantitativeLowerLimit = (decimal)quantitative.dLowerLimit;

                WeightCorrectionPercent = (decimal)model.WeightCorrectionPercent;
                WeightReference = (decimal)model.WeightReference;

                var point = model.WeighingPosition ?? new Point3D();
                WeighingPositionX = (decimal)point.X;
                WeighingPositionY = (decimal)point.Y;
                WeighingPositionZ = (decimal)point.Z;
                PositionMode = model.PositionMode;

                var timeTable = model.WeighingTimeTable ?? new WeighingTimeTable();
                WeighingTimeEnabled = timeTable.Enabled;
                weighingTimeItems = CloneTimeItems(timeTable.WeighingTimeItems);
                var firstTime = weighingTimeItems.FirstOrDefault();
                WeighingTimeHour = firstTime?.Hour ?? 0;
                WeighingTimeMinute = firstTime?.Minute ?? 0;
            });
        }

        /// <summary>
        /// 转为后端模型
        /// </summary>
        /// <returns>后端模型</returns>
        public WeighingParameter ToModel()
        {
            var model = new WeighingParameter
            {
                FunctionHeadID = FunctionHeadID ?? string.Empty,
                Enabled = Enabled,
                WeighingMode = WeighingMode,
                PreOpenValveTime = (int)PreOpenValveTime,
                WeightCorrectionPercent = (double)WeightCorrectionPercent,
                WeightReference = (double)WeightReference,
                WeighingPosition = new Point3D((double)WeighingPositionX, (double)WeighingPositionY, (double)WeighingPositionZ),
                PositionMode = PositionMode,
                WeighingTimeTable = new WeighingTimeTable
                {
                    Enabled = WeighingTimeEnabled,
                    WeighingTimeItems = BuildTimeItems(),
                },
            };

            model.SinglePointWeighingParams.PointCount = (int)SinglePointPointCount;
            model.SinglePointWeighingParams.SprayCount = (int)SinglePointSprayCount;
            model.SinglePointWeighingParams.OnceInterval = (double)SinglePointOnceInterval;
            model.SinglePointWeighingParams.CycleCount = (int)SinglePointCycleCount;
            model.SinglePointWeighingParams.dDotUpperLimit = (double)SinglePointDotUpperLimit;
            model.SinglePointWeighingParams.dDotLowerLimit = (double)SinglePointDotLowerLimit;

            model.MassFlowWeighingParams.PointCount = (int)MassFlowPointCount;
            model.MassFlowWeighingParams.SprayCount = (int)MassFlowSprayCount;
            model.MassFlowWeighingParams.OnceInterval = (double)MassFlowOnceInterval;
            model.MassFlowWeighingParams.CycleCount = (int)MassFlowCycleCount;

            model.QuantitativeWeighingParameters.WeighingMode = WeighingMode;
            model.QuantitativeWeighingParameters.QuantitativeWeight = (double)QuantitativeWeight;
            model.QuantitativeWeighingParameters.SprayCount = (int)QuantitativeSprayCount;
            model.QuantitativeWeighingParameters.OnceInterval = (double)QuantitativeOnceInterval;
            model.QuantitativeWeighingParameters.CycleCount = (int)QuantitativeCycleCount;
            model.QuantitativeWeighingParameters.TotalWeight = (double)QuantitativeTotalWeight;
            model.QuantitativeWeighingParameters.dNRadioWeight = (double)QuantitativeNRadioWeight;
            model.QuantitativeWeighingParameters.dUpperLimit = (double)QuantitativeUpperLimit;
            model.QuantitativeWeighingParameters.dLowerLimit = (double)QuantitativeLowerLimit;

            return model;
        }

        private void SubscribeControlViewModels()
        {
            foreach (var controlViewModel in GetControlViewModels())
            {
                controlViewModel.ValueChanged += OnControlValueChanged;
            }
        }

        private IEnumerable<BasicControlViewModel> GetControlViewModels()
        {
            yield return FunctionHeadIDViewModel;
            yield return EnabledViewModel;
            yield return WeighingModeViewModel;
            yield return PreOpenValveTimeViewModel;
            yield return WeightCorrectionPercentViewModel;
            yield return WeightReferenceViewModel;
            yield return WeighingPositionXViewModel;
            yield return WeighingPositionYViewModel;
            yield return WeighingPositionZViewModel;
            yield return PositionModeViewModel;
            yield return SinglePointPointCountViewModel;
            yield return SinglePointSprayCountViewModel;
            yield return SinglePointOnceIntervalViewModel;
            yield return SinglePointCycleCountViewModel;
            yield return SinglePointDotUpperLimitViewModel;
            yield return SinglePointDotLowerLimitViewModel;
            yield return MassFlowPointCountViewModel;
            yield return MassFlowSprayCountViewModel;
            yield return MassFlowOnceIntervalViewModel;
            yield return MassFlowCycleCountViewModel;
            yield return QuantitativeWeightViewModel;
            yield return QuantitativeSprayCountViewModel;
            yield return QuantitativeOnceIntervalViewModel;
            yield return QuantitativeCycleCountViewModel;
            yield return QuantitativeTotalWeightViewModel;
            yield return QuantitativeNRadioWeightViewModel;
            yield return QuantitativeUpperLimitViewModel;
            yield return QuantitativeLowerLimitViewModel;
            yield return WeighingTimeEnabledViewModel;
            yield return WeighingTimeHourViewModel;
            yield return WeighingTimeMinuteViewModel;
        }

        private void ApplyControlWidthLevel(string widthLevel)
        {
            // 重点：统一调整所有子控件宽度，保证基础参数、单点、流量、定量和定时区域视觉一致。
            foreach (var controlViewModel in GetControlViewModels())
            {
                controlViewModel.SetWidth(widthLevel);
            }
        }

        private void OnControlValueChanged(object sender, EventArgs e)
        {
            if (sender == WeighingModeViewModel)
            {
                this.RaisePropertyChanged(nameof(WeighingModeDisplayName));
                this.RaisePropertyChanged(nameof(IsSinglePointMode));
                this.RaisePropertyChanged(nameof(IsMassFlowMode));
                this.RaisePropertyChanged(nameof(PointCount));
            }

            if (sender == PositionModeViewModel)
            {
                this.RaisePropertyChanged(nameof(PositionModeDisplayName));
            }

            if (sender == SinglePointPointCountViewModel || sender == MassFlowPointCountViewModel)
            {
                this.RaisePropertyChanged(nameof(PointCount));
            }

            if (sender == WeighingTimeEnabledViewModel)
            {
                UpdateWeighingTimeInputState();
                this.RaisePropertyChanged(nameof(IsWeighingTimeInputEnabled));
            }

            NotifyModified();
        }

        private void UpdateWeighingTimeInputState()
        {
            var enabled = WeighingTimeEnabled;
            WeighingTimeHourViewModel.IsEnabled = enabled;
            WeighingTimeMinuteViewModel.IsEnabled = enabled;
        }

        private static ComboxViewModel CreateComboViewModel()
        {
            return new ComboxViewModel
            {
                DisplayMemberPath = nameof(ComBoxItem.DisplayName),
                PlaceholderText = string.Empty,
            };
        }

        private static NumericUpDownViewModel CreateNumericViewModel(decimal minimum, decimal maximum, decimal increment, int decimalPlaces)
        {
            return new NumericUpDownViewModel
            {
                Minimum = minimum,
                Maximum = maximum,
                Increment = increment,
                DecimalPlaces = decimalPlaces,
            };
        }

        private static ComBoxItem CreateComboItem(object value, string displayName)
        {
            return new ComBoxItem { Value = value, DisplayName = displayName };
        }

        private static T GetSelectedValue<T>(ComboxViewModel viewModel, T defaultValue)
        {
            return viewModel.SelectedItem is ComBoxItem item && item.Value is T value
                ? value
                : defaultValue;
        }

        private static void SetSelectedValue<T>(ComboxViewModel viewModel, T value)
        {
            if (viewModel.ItemsSource == null)
            {
                return;
            }

            foreach (var item in viewModel.ItemsSource)
            {
                if (item is ComBoxItem comboItem && Equals(comboItem.Value, value))
                {
                    viewModel.SelectedItem = comboItem;
                    return;
                }
            }
        }

        /// <summary>
        /// 应用数据时屏蔽修改事件
        /// </summary>
        /// <param name="action">动作</param>
        private void ApplyWithoutModified(Action action)
        {
            isApplyingData = true;
            try
            {
                action();
            }
            finally
            {
                isApplyingData = false;
            }
        }

        /// <summary>
        /// 通知修改
        /// </summary>
        private void NotifyModified()
        {
            if (isApplyingData)
            {
                return;
            }

            AfterModified?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 复制定时时间项
        /// </summary>
        /// <param name="source">源数组</param>
        /// <returns>复制数组</returns>
        private static WeighingTimeItem[] CloneTimeItems(WeighingTimeItem[] source)
        {
            if (source == null || source.Length == 0)
            {
                return Array.Empty<WeighingTimeItem>();
            }

            return source
                .Where(item => item != null)
                .Select(item => new WeighingTimeItem { Hour = item.Hour, Minute = item.Minute })
                .ToArray();
        }

        /// <summary>
        /// 构建定时时间项
        /// </summary>
        /// <returns>时间项数组</returns>
        private WeighingTimeItem[] BuildTimeItems()
        {
            var items = CloneTimeItems(weighingTimeItems);
            if (items.Length == 0)
            {
                if (!WeighingTimeEnabled && WeighingTimeHour == 0 && WeighingTimeMinute == 0)
                {
                    return Array.Empty<WeighingTimeItem>();
                }

                items = new[] { new WeighingTimeItem() };
            }

            items[0].Hour = (int)WeighingTimeHour;
            items[0].Minute = (int)WeighingTimeMinute;
            return items;
        }
    }
}
