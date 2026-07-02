using Avalonia;
using Avalonia.Controls;
using GKG.SubMM;
using Griffins.CompUI.FixingStructure.CompUI_FixingStructure.PageType.RecipeCfgPage.AxisFixRecipeConfig.ViewModels;
using Griffins.CompUI.FixingStructure.CompUI_FixingStructure.PageType.RecipeCfgPage.AxisFixRecipeConfig.Views;
using Griffins.Map.UI;
using Griffins.PF;
using System;

namespace Griffins.CompUI.FixingStructure.CompUI_FixingStructure.PageType.RecipeCfgPage.AxisFixRecipeConfig
{
    /// <summary>
    /// 配方页运行态视图包装类，负责数据绑定与页面生命周期清理。
    /// </summary>
    internal class AxisFixRecipeConfigPageTypeRunTimeCompUIView : IPageTypeRunTimeCompUIView
    {
        private readonly AxisFixRecipeConfigCompUIViewModel _viewModel;
        private event EventHandler _afterModified;

        public AxisFixRecipeConfigPageTypeRunTimeCompUIView(ICompUIRunTimeCallBack callBack)
        {
            _viewModel = new AxisFixRecipeConfigCompUIViewModel(callBack);
            _viewModel.AfterModified += (_, e) => _afterModified?.Invoke(this, e);
        }

        public object View
        {
            get
            {
                var view = new AxisFixRecipeConfigCompUIView();
                view.DataContext = _viewModel;
                view.DetachedFromVisualTree += OnViewDetachedFromVisualTree;
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

        public void SetData(AxisFixSubMachineModulesPPCfg data)
        {
            _viewModel.Init(data);
        }

        public AxisFixSubMachineModulesPPCfg GetData()
        {
            return _viewModel.GetData();
        }

        /// <summary>页面从可视树移除时停止运动并注销位置推送。</summary>
        private void OnViewDetachedFromVisualTree(object sender, VisualTreeAttachmentEventArgs e)
        {
            _viewModel.Cleanup();
        }
    }
}
