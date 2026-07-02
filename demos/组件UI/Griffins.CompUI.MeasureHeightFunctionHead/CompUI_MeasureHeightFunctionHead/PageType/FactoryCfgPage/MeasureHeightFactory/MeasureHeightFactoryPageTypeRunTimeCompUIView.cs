using Avalonia.Controls;
using Griffins.CompUI.MeasureHeightFunctionHead.CompUI_MeasureHeightFunctionHead.PageType.FactoryCfgPage.MeasureHeightFactory.ViewModels;
using Griffins.CompUI.MeasureHeightFunctionHead.CompUI_MeasureHeightFunctionHead.PageType.FactoryCfgPage.MeasureHeightFactory.Views;
using Griffins.Map.UI;
using Griffins.PF;
using GKG.SubMM;
using System;

namespace Griffins.CompUI.MeasureHeightFunctionHead.CompUI_MeasureHeightFunctionHead.PageType.FactoryCfgPage.MeasureHeightFactory
{
    /// <summary>
    /// 测高工厂配置页面类型运行时组件UI视图
    /// </summary>
    internal class MeasureHeightFactoryPageTypeRunTimeCompUIView : IPageTypeRunTimeCompUIView
    {
        private readonly MeasureHeightFactoryCompUIView view;
        private readonly MeasureHeightFactoryCompUIViewModel viewModel;
        private MeasureHeightFunctionHeadSubMachineModulesFactoryCfg data;
        private event EventHandler afterModified;

        /// <summary>
        /// 构造函数
        /// </summary>
        public MeasureHeightFactoryPageTypeRunTimeCompUIView()
        {
            view = new MeasureHeightFactoryCompUIView();
            viewModel = new MeasureHeightFactoryCompUIViewModel();
            view.DataContext = viewModel;
            viewModel.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(viewModel.MeasureHeightType))
                {
                    afterModified?.Invoke(this, EventArgs.Empty);
                }
            };
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
        public void SetData(MeasureHeightFunctionHeadSubMachineModulesFactoryCfg data)
        {
            this.data = data ?? new MeasureHeightFunctionHeadSubMachineModulesFactoryCfg();
            viewModel.SetData(this.data);
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns>数据</returns>
        public MeasureHeightFunctionHeadSubMachineModulesFactoryCfg GetData()
        {
            data = viewModel.GetData();
            return data;
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
