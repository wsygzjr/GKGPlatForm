using Avalonia.Controls;
using Griffins.CompUI.MaterialBox.CompUI_MaterialBox.PageType.FactoryCfgPage.MaterialBoxFactory.ViewModels;
using Griffins.CompUI.MaterialBox.CompUI_MaterialBox.PageType.FactoryCfgPage.MaterialBoxFactory.Views;
using Griffins.CompUI.MaterialBox.CompUI_MaterialBox.PageType.InitCfgPage.MaterialBoxInitConfig.Views;
using Griffins.Map.UI;
using Griffins.PF;
using System;
using BackendMaterialBoxFactoryCfg = GKG.SubMM.MaterialBoxSubMachineModulesFactoryCfg;

namespace Griffins.CompUI.MaterialBox.CompUI_MaterialBox.PageType.FactoryCfgPage.MaterialBoxFactory
{
    /// <summary>
    /// 工厂配置页运行态视图包装类，负责连接视图、视图模型与宿主接口。
    /// </summary>
    internal class MaterialBoxFactoryPageTypeRunTimeCompUIView : IPageTypeRunTimeCompUIView
    {
        /// <summary>工厂配置页的 Avalonia 视图对象。</summary>
        //private readonly MaterialBoxFactoryCompUIView _view;
        /// <summary>工厂配置页的视图模型对象。</summary>
        private readonly MaterialBoxFactoryCompUIViewModel _viewModel;
        /// <summary>对外抛出的修改事件。</summary>
        private event EventHandler _afterModified;

        /// <summary>创建运行态视图，并完成视图与视图模型绑定。</summary>
        public MaterialBoxFactoryPageTypeRunTimeCompUIView(ICompUIRunTimeCallBack callBack)
        {
            //_view = new MaterialBoxFactoryCompUIView();
            _viewModel = new MaterialBoxFactoryCompUIViewModel(callBack);
            //_view.DataContext = _viewModel;
            _viewModel.AfterModified += (_, e) => _afterModified?.Invoke(this, e);
        }

        /// <summary>返回实际显示的视图对象。</summary>
        public object View
        {
            get
            {
                //RemoveViewFromParent();
                var view = new MaterialBoxFactoryCompUIView();
                view.DataContext = _viewModel;
                return view;
            }
        }

        /// <summary>当前页面不暴露额外的功能管理单元。</summary>
        public OpMngCellID[] EditFuncMngCellIDs => null;

        /// <summary>页面数据修改后的通知事件。</summary>
        public event EventHandler AfterModified
        {
            add => _afterModified += value;
            remove => _afterModified -= value;
        }

        /// <summary>设置页面是否只读。</summary>
        public void SetReadOnly(bool readOnly)
        {
            _viewModel.ReadOnly = readOnly;
        }

        /// <summary>向页面写入工厂配置数据。</summary>
        public void SetData(BackendMaterialBoxFactoryCfg data)
        {
            _viewModel.SetData(data);
        }

        /// <summary>从页面收集工厂配置数据。</summary>
        public BackendMaterialBoxFactoryCfg GetData()
        {
            return _viewModel.GetData();
        }

        /// <summary>把视图从旧父容器中移除，避免重复挂载时报错。</summary>
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
    }
}
