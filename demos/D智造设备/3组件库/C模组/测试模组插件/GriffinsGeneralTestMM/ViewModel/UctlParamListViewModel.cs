using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Threading;
using Griffins;
using Griffins.ImeIOT;
using Griffins.UI;
using ReactiveUI;
using System;
using System.Collections.Generic;

namespace GriffinsGeneralTestMM
{
    /// <summary>
    /// 参数列表整体 ViewModel（管理一组参数控件）
    /// </summary>
    public class UctlParamListViewModel : ReactiveObject
    {
        // 参数字典：key=参数ID，value=参数控件接口
        private readonly Dictionary<string, IParamUI> _paramUIDict;
        private bool _readOnly;
        private StackPanel _stackPanel_UctlParamList;

        /// <summary>
        /// 任意参数值修改事件
        /// </summary>
        public event ParamValChangedEventHandler? AfterParamValModified;

        public UctlParamListViewModel()
        {
            _paramUIDict = new Dictionary<string, IParamUI>();
        }

        /// <summary>
        /// 整体只读控制
        /// </summary>
        public bool ReadOnly
        {
            get => _readOnly;
            set
            {
                this.RaiseAndSetIfChanged(ref _readOnly, value);
                // 同步所有子控件
                SetReadOnly();
            }
        }

        /// <summary>
        /// 承载所有参数控件的布局容器
        /// </summary>
        public StackPanel StackPanel_UctlParamList
        {
            get => _stackPanel_UctlParamList;
            set => this.RaiseAndSetIfChanged(ref _stackPanel_UctlParamList, value);
        }

        /// <summary>
        /// 获取/设置整组参数值
        /// </summary>
        public GFBaseTypeParamValueList ParamValues
        {
            get => GetParamValues();
            set => SetParamValues(value);
        }

        /// <summary>
        /// 初始化测试数据
        /// </summary>
        public void InitTestData()
        {
            var genObjectDefInfo = new GenParamObjectDefInfo
            {
                LabelWidth = 190,
                ParamInfoes = new GFParamDefInfoList
                {
                    new GFParamDefInfo("TestProp1", "测试属性1", GriffinsBaseDataType.String),
                    new GFParamDefInfo("TestProp2", "测试属性2", GriffinsBaseDataType.Integer),
                }
            };
            Init(genObjectDefInfo);
        }

        /// <summary>
        /// 根据配置初始化所有参数控件
        /// </summary>
        public void Init(GenParamObjectDefInfo genObjectDefInfo)
        {
            if (genObjectDefInfo == null) return;
            Dispatcher.UIThread.Invoke(() =>
            {
                // 创建垂直布局
                StackPanel_UctlParamList = new StackPanel
                {
                    Orientation = Orientation.Vertical,
                    Margin = new Avalonia.Thickness(0, 0, 8, 0)
                };

                _paramUIDict.Clear();
                var propertyList = genObjectDefInfo.ToGenPropertyInfoes();

                foreach (var info in propertyList)
                {
                    // 创建对应类型的参数控件
                    var paramUI = CreateParamUI(info);
                    paramUI.ParamID = info.ParamID;
                    paramUI.ReadOnly = _readOnly;
                    paramUI.SetLabelWidth(genObjectDefInfo.LabelWidth); 
                    paramUI.SetParamName(info.ParamName);

                    // 监听控件值变化
                    paramUI.AfterModified += ParamUI_AfterModified;

                    _paramUIDict.Add(info.ParamID, paramUI);
                    StackPanel_UctlParamList.Children.Add((Control)paramUI);
                }
            });
        }

        /// <summary>
        /// 根据数据类型创建参数控件
        /// </summary>
        private IParamUI CreateParamUI(GenPropertyInfo info)
        {
            return info.DataType switch
            {
                GriffinsBaseDataType.String => new UctlParamTextView(),
                GriffinsBaseDataType.Integer => new UctlParamIntView(),
                GriffinsBaseDataType.Decimal => new UctlParamDecimalView(),
                GriffinsBaseDataType.DateTime => new UctlParamDateTimeView(),
                GriffinsBaseDataType.Guid => new UctlParamGuidView(),
                GriffinsBaseDataType.Object_Json => new UctlParamJsonView(),
                GriffinsBaseDataType.Object_Bytes => new UctlParamByteView(),
                GriffinsBaseDataType.Bool => new UctlParamBoolView(),
                _ => throw new NotSupportedException("不支持的参数类型")
            };
        }

        /// <summary>
        /// 子控件值修改 → 转发给外部
        /// </summary>
        private void ParamUI_AfterModified(object sender, EventArgs e)
        {
            var paramUI = (IParamUI)sender;
            var paramValue = new GFBaseTypeParamValue
            {
                ID = paramUI.ParamID,
                Value = paramUI.ParamVal
            };

            // 触发列表级修改事件
            AfterParamValModified?.Invoke(this, new ParamValChangedEventArgs(paramValue));
        }

        /// <summary>
        /// 批量设置所有子控件只读
        /// </summary>
        private void SetReadOnly()
        {
            foreach (var paramUI in _paramUIDict.Values)
                paramUI.ReadOnly = _readOnly;
        }

        /// <summary>
        /// 统一调整所有参数控件宽度
        /// </summary>
        public void ResizeParamUIs(int width)
        {
            foreach (var paramUI in _paramUIDict.Values)
            {
                if (paramUI is Control ctrl)
                {
                    ctrl.Width = width;
                    ctrl.Height = 24;
                }
            }
        }

        /// <summary>
        /// 收集所有参数当前值
        /// </summary>
        private GFBaseTypeParamValueList GetParamValues()
        {
            var paramValues = new GFBaseTypeParamValueList();
            foreach (var kvp in _paramUIDict)
            {
                paramValues.Add(new GFBaseTypeParamValue
                {
                    ID = kvp.Key,
                    Value = kvp.Value.ParamVal
                });
            }
            return paramValues;
        }

        /// <summary>
        /// 批量设置参数值
        /// </summary>
        private void SetParamValues(GFBaseTypeParamValueList paramValues)
        {
            if (paramValues == null) return;
            foreach (var item in paramValues)
                SetParamValue(item.ID, item.Value);
        }

        /// <summary>
        /// 设置单个参数值
        /// </summary>
        public void SetParamValue(string paramID, GriffinsBaseValue value)
        {
            if (_paramUIDict.TryGetValue(paramID, out var paramUI))
            {
                paramUI.ParamVal = value;
                //paramUI.ParamVal = value.Val; 
            }
        }

        /// <summary>
        /// 获取单个参数值
        /// </summary>
        public GriffinsBaseValue GetParamValue(string paramID)
        {
            return _paramUIDict.TryGetValue(paramID, out var paramUI)
                ? paramUI.ParamVal
                : null;
        }
    }
}