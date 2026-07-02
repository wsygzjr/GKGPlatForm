using System;
using Avalonia.Controls;
using Griffins.CompUI.SL.InitCfgPage.Models;
using Griffins.CompUI.SL.InitCfgPage.ViewModels;
using Griffins.CompUI.SL.InitCfgPage.Views;
using Griffins.Map;
using Griffins.Map.UI;
using Griffins.PF;

namespace Griffins.CompUI.SL.InitCfgPage
{
    internal class SingleTrackStationPageTypeRunTimeCompUIView : IPageTypeRunTimeCompUIView
    {
        private ICompUIRunTimeCallBack callBack;

        private SingleTrackStationCompUIView view;

        private readonly SingleTrackStationCompUIViewModel viewModel;

        private SingleTrackStationCompUIModel singleTrackStationCompUIModel;

        private event EventHandler afterModified;

        /// <summary>
        /// 编辑查看参数界面，应该从Control继承
        /// </summary>
        object IPageTypeRunTimeCompUIView.View
        {
            get
            {
                RemoveViewFromParent();
                return view;
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="callBack">回调方法</param>
        public SingleTrackStationPageTypeRunTimeCompUIView(ICompUIRunTimeCallBack callBack)
        {
            this.callBack = callBack;
            view = new();
            viewModel = new(false, callBack);
            view.DataContext = viewModel;

            // 事件绑定
            viewModel.PropertyChanged += ViewModelOnPropertyChanged;
        }

        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="transportMotorCompUIModel">数据源</param>
        public void SetData(SingleTrackStationCompUIModel compUIModel)
        {
            singleTrackStationCompUIModel = compUIModel ?? new();
            viewModel.SetData(singleTrackStationCompUIModel);
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns>模组数据</returns>
        public SingleTrackStationCompUIModel GetData()
        {
            return viewModel.GetData();
        }

        /// <summary>
        /// 绑定修改后事件
        /// </summary>
        event EventHandler IPageTypeRunTimeCompUIView.AfterModified
        {
            add
            {
                afterModified += value;
            }
            remove
            {
                afterModified -= value;
            }
        }

        /// <summary>
        /// 编辑权限所需的操作管理单元ID列表。通过判断用户权限中是否有该操作管理单元ID决定在界面是否可以进行编辑，
        /// 只要包含其中一个就认为该用户具有该功能的操作权限，否则只有只读权限。null或个数为0表示不控制编辑权限。
        /// </summary>
        OpMngCellID[] IPageTypeRunTimeCompUIView.EditFuncMngCellIDs => null;

        /// <summary>
        ///  设置是否只读
        /// </summary>
        /// <param name="readOnly">true:只读，false:读写</param>
        void IPageTypeRunTimeCompUIView.SetReadOnly(bool readOnly)
        {
            foreach (var station in viewModel.Stations)
            {
                station.ReadOnly = readOnly;
            }
        }

        /// <summary>
        /// 辅助方法：从原父容器移除View（兼容Panel/ContentControl等常见容器）
        /// </summary>
        private void RemoveViewFromParent()
        {
            if (view == null) return;

            if (view.Parent is Panel panelParent)
            {
                if (panelParent.Children.Contains(view))
                {
                    panelParent.Children.Remove(view);
                }
            }
            else if (view.Parent is ContentControl contentParent)
            {
                if (contentParent.Content == view)
                {
                    contentParent.Content = null;
                }
            }
        }

        /// <summary>
        /// 属性变更事件处理
        /// </summary>
        private void ViewModelOnPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(viewModel.Stations))
            {
                afterModified?.Invoke(sender, EventArgs.Empty);
            }
        }
    }
}
