using GKG.SubMM;
using Griffins.CompUI.FixingStructure.CompUI_FixingStructure.PageType.InitCfgPage.AxisFixInitConfig.ViewModels;
using Griffins.CompUI.FixingStructure.CompUI_FixingStructure.PageType.InitCfgPage.AxisFixInitConfig.Views;
using Griffins.Map.UI;
using Griffins.PF;
using System;

namespace Griffins.CompUI.FixingStructure.CompUI_FixingStructure.PageType.InitCfgPage.AxisFixInitConfig
{
    /// <summary>
    /// 初始化配置页运行态视图包装类，负责视图与视图模型之间的桥接。
    /// </summary>
    internal class AxisFixInitConfigPageTypeRunTimeCompUIView : IPageTypeRunTimeCompUIView
    {
        private readonly AxisFixInitConfigCompUIViewModel _viewModel;
        private event EventHandler _afterModified;

        public AxisFixInitConfigPageTypeRunTimeCompUIView(ICompUIRunTimeCallBack callBack)
        {
            _viewModel = new AxisFixInitConfigCompUIViewModel(callBack);
            _viewModel.AfterModified += (_, e) => _afterModified?.Invoke(this, e);
        }

        /// <summary>每次获取视图时创建新实例，避免重复挂载到父容器时报错。</summary>
        public object View
        {
            get
            {
                var view = new AxisFixInitConfigCompUIView();
                view.DataContext = _viewModel;
                return view;
            }
        }

        public OpMngCellID[] EditFuncMngCellIDs => null;

        public event EventHandler AfterModified
        {
            add => _afterModified += value;
            remove => _afterModified -= value;
        }

        public void SetReadOnly(bool readOnly)
        {
            _viewModel.ReadOnly = readOnly;
        }

        public void SetData(AxisFixSubMachineModulesInitCfg data)
        {
            _viewModel.SetData(data);
        }

        public AxisFixSubMachineModulesInitCfg GetData()
        {
            return _viewModel.GetData();
        }
    }
}
