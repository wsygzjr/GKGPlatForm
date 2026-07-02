using Avalonia.Controls;
using Griffins.CompUI.MeasureHeightFunctionHead.CompUI_MeasureHeightFunctionHead.PageType.InitCfgPage.MeasureHeightInit.ViewModels;
using Griffins.CompUI.MeasureHeightFunctionHead.CompUI_MeasureHeightFunctionHead.PageType.InitCfgPage.MeasureHeightInit.Views;
using Griffins.Map.UI;
using Griffins.PF;
using GKG.SubMM;
using System;

namespace Griffins.CompUI.MeasureHeightFunctionHead.CompUI_MeasureHeightFunctionHead.PageType.InitCfgPage.MeasureHeightInit
{
    /// <summary>
    /// 测高初始化配置页面类型运行时组件UI视图
    /// </summary>
    internal class MeasureHeightInitPageTypeRunTimeCompUIView : IPageTypeRunTimeCompUIView, IDisposable
    {
        private readonly MeasureHeightInitCompUIView view;
        private readonly MeasureHeightInitCompUIViewModel viewModel;
        private MeasureHeightFunctionHeadSubMachineModulesInitCfg data;
        private event EventHandler afterModified;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="callBack">回调</param>
        public MeasureHeightInitPageTypeRunTimeCompUIView(ICompUIRunTimeCallBack callBack)
        {
            view = new MeasureHeightInitCompUIView();
            viewModel = new MeasureHeightInitCompUIViewModel(false, callBack);
            view.DataContext = viewModel;
            viewModel.AfterModified += (_, __) => afterModified?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 视图
        /// </summary>
        public object View
        {
            get
            {
                RemoveViewFromParent();
                return view;
            }
        }

        /// <summary>
        /// 编辑功能管理单元ID
        /// </summary>
        public OpMngCellID[] EditFuncMngCellIDs => null;

        /// <summary>
        /// 修改后事件
        /// </summary>
        public event EventHandler AfterModified
        {
            add => afterModified += value;
            remove => afterModified -= value;
        }

        /// <summary>
        /// 设置只读
        /// </summary>
        /// <param name="readOnly">是否只读</param>
        public void SetReadOnly(bool readOnly)
        {
            viewModel.ReadOnly = readOnly;
        }

        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="data">数据</param>
        public void SetData(MeasureHeightFunctionHeadSubMachineModulesInitCfg data)
        {
            this.data = data ?? new MeasureHeightFunctionHeadSubMachineModulesInitCfg();
            viewModel.SetData(this.data);
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns>数据</returns>
        public MeasureHeightFunctionHeadSubMachineModulesInitCfg GetData()
        {
            data = viewModel.GetData();
            return data;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            viewModel.Dispose();
        }

        /// <summary>
        /// 从父级移除视图
        /// </summary>
        private void RemoveViewFromParent()
        {
            if (view.Parent is Panel panelParent && panelParent.Children.Contains(view))
            {
                panelParent.Children.Remove(view);
            }
            else if (view.Parent is ContentControl contentParent && Equals(contentParent.Content, view))
            {
                contentParent.Content = null;
            }
        }
    }
}
