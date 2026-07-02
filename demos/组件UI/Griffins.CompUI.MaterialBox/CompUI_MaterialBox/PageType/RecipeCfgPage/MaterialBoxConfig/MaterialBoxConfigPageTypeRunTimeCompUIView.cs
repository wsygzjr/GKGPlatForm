using Avalonia;
using Avalonia.Controls;
using Griffins.CompUI.MaterialBox.CompUI_MaterialBox.PageType.FactoryCfgPage.MaterialBoxFactory.Views;
using Griffins.CompUI.MaterialBox.CompUI_MaterialBox.PageType.RecipeCfgPage.MaterialBoxConfig.ViewModels;
using Griffins.CompUI.MaterialBox.CompUI_MaterialBox.PageType.RecipeCfgPage.MaterialBoxConfig.Views;
using Griffins.Map.UI;
using Griffins.PF;
using System;
using BackendMaterialBoxPPCfg = GKG.SubMM.MaterialBoxSubMachineModulesPPCfg;

namespace Griffins.CompUI.MaterialBox.CompUI_MaterialBox.PageType.RecipeCfgPage.MaterialBoxConfig
{
    /// <summary>
    /// 配方页中“MaterialBox配置”视图的运行态包装类。
    /// </summary>
    internal class MaterialBoxConfigPageTypeRunTimeCompUIView : IPageTypeRunTimeCompUIView
    {
        /// <summary>配方配置视图对象。</summary>
        //private readonly MaterialBoxConfigCompUIView _view;
        /// <summary>配方配置视图模型对象。</summary>
        private readonly MaterialBoxConfigCompUIViewModel _viewModel;
        /// <summary>对外转发的数据修改事件。</summary>
        private event EventHandler? _afterModified;

        /// <summary>创建运行态视图，并完成视图与视图模型绑定。</summary>
        public MaterialBoxConfigPageTypeRunTimeCompUIView(ICompUIRunTimeCallBack callBack)
        {
           // _view = new MaterialBoxConfigCompUIView();
            _viewModel = new MaterialBoxConfigCompUIViewModel(callBack);
           // _view.DataContext = _viewModel;
            _viewModel.AfterModified += (_, e) => _afterModified?.Invoke(this, e);
            //_view.DetachedFromVisualTree += OnViewDetachedFromVisualTree;
        }

        /// <summary>返回宿主需要显示的配方配置视图。</summary>
        public object View
        {
            get
            {
                //RemoveViewFromParent();
                var view = new MaterialBoxConfigCompUIView();
                view.DataContext = _viewModel;
                view.DetachedFromVisualTree += OnViewDetachedFromVisualTree;
                return view;
            }
        }

        /// <summary>当前页面不提供额外的功能管理单元。</summary>
        public OpMngCellID[] EditFuncMngCellIDs => null;

        /// <summary>页面内容变化时通知宿主刷新状态。</summary>
        public event EventHandler AfterModified
        {
            add => _afterModified += value;
            remove => _afterModified -= value;
        }

        /// <summary>当前页面暂无只读态切换逻辑，预留给后续扩展。</summary>
        public void SetReadOnly(bool readOnly)
        {
        }

        /// <summary>把前端配方数据写入视图模型。</summary>
        public void SetData(BackendMaterialBoxPPCfg data)
        {
            _viewModel.Init(data);
        }

        /// <summary>从视图模型收集最新的前端配方数据。</summary>
        public BackendMaterialBoxPPCfg GetData()
        {
            return _viewModel.GetData();
        }

        /// <summary>移除旧的父容器引用，避免同一视图重复挂载时报错。</summary>
        //private void RemoveViewFromParent()
        //{
        //    if (_view.Parent is Panel panelParent && panelParent.Children.Contains(_view))
        //    {
        //        panelParent.Children.Remove(_view);
        //    }
        //    else if (_view.Parent is ContentControl contentParent && Equals(contentParent.Content, _view))
        //    {
        //        contentParent.Content = null;
        //    }
        //}

        /// <summary>页面从可视树移除时执行清理逻辑，停止轮询并收尾运动状态。</summary>
        private void OnViewDetachedFromVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
        {
            _viewModel.Cleanup();
        }
    }
}
