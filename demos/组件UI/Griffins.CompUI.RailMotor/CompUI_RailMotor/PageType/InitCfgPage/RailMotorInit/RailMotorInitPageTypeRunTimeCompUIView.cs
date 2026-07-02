using Griffins.CompUI.RailMotor.CompUI_RailMotor.PageType.InitCfgPage.RailMotorInit.ViewModels;
using Griffins.CompUI.RailMotor.CompUI_RailMotor.PageType.InitCfgPage.RailMotorInit.Views;
using GKG.SubMM;
using Griffins.Map.UI;
using Griffins.PF;
using System;

namespace Griffins.CompUI.RailMotor.CompUI_RailMotor.PageType.InitCfgPage.RailMotorInit
{
    /// <summary>
    /// 运输电机初始化配置运行态视图适配器：连接 Avalonia 视图与 ViewModel。
    /// </summary>
    internal class RailMotorInitPageTypeRunTimeCompUIView : IPageTypeRunTimeCompUIView
    {
        /// <summary>初始化配置 Avalonia 用户控件。</summary>
        private readonly RailMotorInitCompUIView _view;

        /// <summary>初始化配置视图模型。</summary>
        private readonly RailMotorInitCompUIViewModel _viewModel;

        /// <summary>数据变更事件订阅列表。</summary>
        private event EventHandler _afterModified;

        /// <summary>
        /// 创建视图与 ViewModel，并建立双向数据变更转发。
        /// </summary>
        public RailMotorInitPageTypeRunTimeCompUIView(ICompUIRunTimeCallBack callBack)
        {
            _view = new RailMotorInitCompUIView();
            _viewModel = new RailMotorInitCompUIViewModel(callBack);
            _view.DataContext = _viewModel;

            _viewModel.AfterModified += (_, e) => _afterModified?.Invoke(this, e);
        }

        /// <summary>供框架嵌入的实际 UI 控件。</summary>
        public object View => _view;

        /// <summary>编辑功能单元格 ID（本视图未使用）。</summary>
        public OpMngCellID[] EditFuncMngCellIDs => null;

        /// <summary>用户修改配置时触发，用于通知页面向宿主上报脏数据。</summary>
        public event EventHandler AfterModified
        {
            add => _afterModified += value;
            remove => _afterModified -= value;
        }

        /// <summary>设置界面控件是否只读。</summary>
        public void SetReadOnly(bool readOnly)
        {
            _viewModel.ReadOnly = readOnly;
        }

        /// <summary>将后端 InitCfg 数据绑定到界面控件。</summary>
        public void SetData(RailMotorSubMachineModulesInitCfg data)
        {
            _viewModel.SetData(data);
        }

        /// <summary>从界面控件收集当前 InitCfg 数据。</summary>
        public RailMotorSubMachineModulesInitCfg GetData()
        {
            return _viewModel.GetData();
        }
    }
}
