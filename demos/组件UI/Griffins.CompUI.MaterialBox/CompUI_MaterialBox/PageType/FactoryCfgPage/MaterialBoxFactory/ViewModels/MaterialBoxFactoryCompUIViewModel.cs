using Griffins.Map.UI;
using GKG.UI;
using ReactiveUI;
using System;
using BackendMaterialBoxFactoryCfg = GKG.SubMM.MaterialBoxSubMachineModulesFactoryCfg;

namespace Griffins.CompUI.MaterialBox.CompUI_MaterialBox.PageType.FactoryCfgPage.MaterialBoxFactory.ViewModels
{
    /// <summary>
    /// 工厂配置页视图模型，负责料口数量在界面与数据对象之间的同步。
    /// </summary>
    internal class MaterialBoxFactoryCompUIViewModel : ReactiveObject
    {
        /// <summary>当前缓存的工厂配置数据。</summary>
        private BackendMaterialBoxFactoryCfg _data = new();
        /// <summary>页面自定义标签，可供宿主或界面扩展使用。</summary>
        private object? _viewTag;
        /// <summary>是否只读。</summary>
        private bool _readOnly;

        /// <summary>送料口数量输入框视图模型。</summary>
        public TextInputViewModel FeedingPortCountViewModel { get; } = new();
        /// <summary>接料口数量输入框视图模型。</summary>
        public TextInputViewModel ReceivePortCountViewModel { get; } = new();

        /// <summary>页面数据发生修改时抛出的事件。</summary>
        public event EventHandler? AfterModified;

        /// <summary>视图标签。</summary>
        public object? ViewTag
        {
            get => _viewTag;
            set => this.RaiseAndSetIfChanged(ref _viewTag, value);
        }

        /// <summary>页面只读状态，同时联动两个输入框的可编辑性。</summary>
        public bool ReadOnly
        {
            get => _readOnly;
            set
            {
                this.RaiseAndSetIfChanged(ref _readOnly, value);
                var enabled = !_readOnly;
                FeedingPortCountViewModel.IsEnabled = enabled;
                ReceivePortCountViewModel.IsEnabled = enabled;
            }
        }

        /// <summary>创建工厂配置页视图模型，并订阅输入框修改事件。</summary>
        public MaterialBoxFactoryCompUIViewModel(ICompUIRunTimeCallBack callBack)
        {
            FeedingPortCountViewModel.ValueChanged += (_, __) => AfterModified?.Invoke(this, EventArgs.Empty);
            ReceivePortCountViewModel.ValueChanged += (_, __) => AfterModified?.Invoke(this, EventArgs.Empty);
            ReadOnly = false;
        }

        /// <summary>把工厂配置数据加载到界面输入框中。</summary>
        public void SetData(BackendMaterialBoxFactoryCfg data)
        {
            _data = data ?? new BackendMaterialBoxFactoryCfg();
            _data.FeedingPortFactoryCfg ??= new MaterialBoxSubMachineModules.FeedPort.FeedPortFactoryCfg();
            _data.ReceivePortFactoryCfg ??= new MaterialBoxSubMachineModules.FeedPort.FeedPortFactoryCfg();
            FeedingPortCountViewModel.Text = _data.FeedingPortFactoryCfg.PortCount.ToString();
            ReceivePortCountViewModel.Text = _data.ReceivePortFactoryCfg.PortCount.ToString();
        }

        /// <summary>从界面输入框收集工厂配置数据。</summary>
        public BackendMaterialBoxFactoryCfg GetData()
        {
            _data ??= new BackendMaterialBoxFactoryCfg();
            _data.FeedingPortFactoryCfg ??= new MaterialBoxSubMachineModules.FeedPort.FeedPortFactoryCfg();
            _data.ReceivePortFactoryCfg ??= new MaterialBoxSubMachineModules.FeedPort.FeedPortFactoryCfg();
            _data.FeedingPortFactoryCfg.PortCount = ParseInt(FeedingPortCountViewModel.Text);
            _data.ReceivePortFactoryCfg.PortCount = ParseInt(ReceivePortCountViewModel.Text);
            return _data;
        }

        /// <summary>把输入框文本安全转换成正整数，非法内容按 0 处理。</summary>
        private static int ParseInt(string? text)
        {
            return int.TryParse(text?.Trim(), out var value) && value > 0 ? value : 0;
        }
    }
}
