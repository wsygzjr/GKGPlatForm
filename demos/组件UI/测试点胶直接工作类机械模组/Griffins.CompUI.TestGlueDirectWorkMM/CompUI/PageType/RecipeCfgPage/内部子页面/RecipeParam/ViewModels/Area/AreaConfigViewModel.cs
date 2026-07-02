using Avalonia.Controls;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.Area;
using ReactiveUI;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.Area
{
    /// <summary>
    /// 区域配置参数-视图模型
    /// </summary>
    public class AreaConfigViewModel : ReactiveObject
    {
        /// <summary>
        /// 值改变事件
        /// </summary>
        public EventHandler? AfterModified;
        /// <summary>
        /// 区域列表视图模型
        /// </summary>
        public AreaListViewModel AreaListViewModel { get; }

        /// <summary>
        ///当前的选中的区域-视图模型
        /// </summary>
        private AreaItemViewModel _curSelectAreaItemViewModel;
        public AreaItemViewModel CurSelectAreaItemViewModel
        { 
            get=> _curSelectAreaItemViewModel; 
            set
            {
                _curSelectAreaItemViewModel = value;
                //更新UI显示
                this.RaisePropertyChanged(nameof(CurSelectAreaItemViewModel));
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public AreaConfigViewModel()
        {
            AreaListViewModel=new AreaListViewModel();

            CacheDataExchange.SetAreaListViewModel(AreaListViewModel);
            _curSelectAreaItemViewModel = new AreaItemViewModel();

            //选中的Area改变
            this.WhenAnyValue(
                    vm => vm.AreaListViewModel.SelectedItem
                )
                .Subscribe(areaItemViewModel =>
                {
                    if (areaItemViewModel != null)
                    {
                        onAreaSelected(areaItemViewModel);
                    }
                });

            // 订阅值变更事件
            subscribeValueChanges();
        }

        /// <summary>
        /// 选中的Area项改变
        /// </summary>
        /// <param name="areaItemViewModel"></param>
        private void onAreaSelected(AreaItemViewModel areaItemViewModel)
        {
            CurSelectAreaItemViewModel = areaItemViewModel;
        }


        /// <summary>
        /// 从数据模型复制数据到ViewModel
        /// </summary>
        public void CopyFrom(AreaConfigInfo areaConfigInfo)
        {
            if (areaConfigInfo == null) return;
            AreaListViewModel.CopyFrom(areaConfigInfo.AreaInfoes);

        }

        /// <summary>
        /// 从ViewModel复制数据到数据模型
        /// </summary>
        public void CopyTo(AreaConfigInfo areaConfigInfo)
        {
            if (areaConfigInfo == null) return;
            AreaListViewModel.CopyTo(areaConfigInfo.AreaInfoes);
        }
        /// <summary>
        /// 设置视图引用（用于弹窗）
        /// </summary>
        public void SetViewReference(Control view)
        {
            AreaListViewModel.SetViewReference(view);
            CurSelectAreaItemViewModel.SetViewReference(view);
            //GlobalVisionViewModel.CameraShowViewModel.SetViewReference(view);
        }
        #region 值改变事件
        /// <summary>
        /// 订阅值改变事件
        /// </summary>
        private void subscribeValueChanges()
        {
            AreaListViewModel.AfterModified += onAfterModified;
        }
        /// <summary>
        /// 值改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onAfterModified(object? sender, EventArgs e)
        {
            AfterModified?.Invoke(sender, e);
        }
        #endregion
    }

}