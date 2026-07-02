using Avalonia.Controls;
using Griffins.CompUI.MaterialBox.CompUI_MaterialBox.PageType.InitCfgPage.MaterialBoxInitConfig.ViewModels;
using Griffins.CompUI.MaterialBox.CompUI_MaterialBox.PageType.InitCfgPage.MaterialBoxInitConfig.Views;
using Griffins.Map.UI;
using Griffins.PF;
using System;
using BackendMaterialBoxInitCfg = GKG.SubMM.MaterialBoxSubMachineModulesInitCfg;

namespace Griffins.CompUI.MaterialBox.CompUI_MaterialBox.PageType.InitCfgPage.MaterialBoxInitConfig
{
    /// <summary>
    /// 初始化配置页运行态视图包装类，负责视图与视图模型之间的桥接。
    /// </summary>
    internal class MaterialBoxInitConfigPageTypeRunTimeCompUIView : IPageTypeRunTimeCompUIView
    {
        /// <summary>初始化配置视图对象。</summary>
        //private readonly MaterialBoxInitConfigCompUIView _view;
        /// <summary>初始化配置视图模型对象。</summary>
        private readonly MaterialBoxInitConfigCompUIViewModel _viewModel;
        /// <summary>对外转发的修改事件。</summary>
        private event EventHandler _afterModified;

        /// <summary>创建运行态初始化视图，并建立数据绑定关系。</summary>
        public MaterialBoxInitConfigPageTypeRunTimeCompUIView(ICompUIRunTimeCallBack callBack)
        {
            //_view = new MaterialBoxInitConfigCompUIView();
            _viewModel = new MaterialBoxInitConfigCompUIViewModel(callBack);
            //_view.DataContext = _viewModel;
            _viewModel.AfterModified += (_, e) => _afterModified?.Invoke(this, e);
        }

        /// <summary>返回宿主需要显示的视图对象。</summary>
        public object View
        {
            get
            {
                //RemoveViewFromParent();
                var view = new MaterialBoxInitConfigCompUIView();
                view.DataContext = _viewModel;
                return view;
            }
        }

        /// <summary>当前页面不提供额外的功能管理单元。</summary>
        public OpMngCellID[] EditFuncMngCellIDs => null;

        /// <summary>页面内容发生修改时通知宿主。</summary>
        public event EventHandler AfterModified
        {
            add => _afterModified += value;
            remove => _afterModified -= value;
        }

        /// <summary>设置当前初始化页面是否只读。</summary>
        public void SetReadOnly(bool readOnly)
        {
            _viewModel.ReadOnly = readOnly;
        }

        /// <summary>把初始化数据写入视图模型。</summary>
        public void SetData(BackendMaterialBoxInitCfg data)
        {
            _viewModel.SetData(data);
        }

        /// <summary>从视图模型收集初始化数据。</summary>
        public BackendMaterialBoxInitCfg GetData()
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
