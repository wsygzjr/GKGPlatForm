using Avalonia.Controls;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.TemplateSubPage.Mark;
using ReactiveUI;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.TemplateSubPage.Mark
{
    /// <summary>
    /// Mark配置参数-视图模型
    /// </summary>
    public class MarkConfigViewModel : ReactiveObject
    {
        /// <summary>
        /// 值改变事件
        /// </summary>
        public EventHandler? AfterModified;
        /// <summary>
        /// Mark基础配置参数
        /// </summary>
        public MarkBasicConfigViewModel MarkBasicConfigViewModel { get; }
        /// <summary>
        /// Mark点列表视图模型
        /// </summary>
        public MarkPointListViewModel MarkPointListViewModel { get; }

        /// <summary>
        ///当前的选中的Mark点-视图模型
        /// </summary>
        private MarkPointViewModel? _curSelectMarkPointViewModel;
        public MarkPointViewModel? CurSelectMarkPointViewModel
        { 
            get=> _curSelectMarkPointViewModel; 
            set
            {
                _curSelectMarkPointViewModel = value;
                //更新UI显示
                this.RaisePropertyChanged(nameof(CurSelectMarkPointViewModel));
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public MarkConfigViewModel()
        {
            MarkBasicConfigViewModel = new MarkBasicConfigViewModel();
            MarkPointListViewModel=new MarkPointListViewModel();

            //选中的Mark改变
            this.WhenAnyValue(
                    vm => vm.MarkPointListViewModel.SelectedItem
                )
                .Subscribe(selectedPoint =>
                {
                    if (selectedPoint != null)
                    {
                        onMarkPointSelected(selectedPoint);
                    }
                });

            // 订阅值变更事件
            subscribeValueChanges();
        }

        /// <summary>
        /// 选中的Mark项改变
        /// </summary>
        /// <param name="selectedPoint"></param>
        private void onMarkPointSelected(MarkPointViewModel? selectedPoint)
        {
            CurSelectMarkPointViewModel = selectedPoint;
        }

        /// <summary>
        /// 从数据模型复制数据到ViewModel
        /// </summary>
        /// <param name="markConfigInfo"></param>
        public void CopyFrom(MarkConfigInfo markConfigInfo)
        {
            if (markConfigInfo == null) return;
            MarkBasicConfigViewModel.CopyFrom(markConfigInfo.MarkBaseConfigInfo);
            MarkPointListViewModel.CopyFrom(markConfigInfo.MarkPointInfoes);
        }

        /// <summary>
        /// 从ViewModel复制数据到数据模型
        /// </summary>
        /// <param name="markConfigInfo"></param>
        public void CopyTo(MarkConfigInfo markConfigInfo)
        {
            if (markConfigInfo == null) return;
            MarkBasicConfigViewModel.CopyTo(markConfigInfo.MarkBaseConfigInfo);
            MarkPointListViewModel.CopyTo(markConfigInfo.MarkPointInfoes);
        }
        /// <summary>
        /// 设置视图引用（用于弹窗）
        /// </summary>
        public void SetViewReference(Control view)
        {
            MarkPointListViewModel.SetViewReference(view);
            //GlobalVisionViewModel.CameraShowViewModel.SetViewReference(view);
            //需要在SetViewReference后添加默认mark,依赖view
            MarkPointListViewModel.AddDefaItem();
        }
        #region 值改变事件
        /// <summary>
        /// 订阅值改变事件
        /// </summary>
        private void subscribeValueChanges()
        {
            MarkBasicConfigViewModel.AfterModified += onAfterModified;
            MarkPointListViewModel.AfterModified += onAfterModified;

            this.MarkPointListViewModel.ItemsSource.CollectionChanged += (s, e) =>
            {
                //当删除所有后，清除界面数据
                if (this.MarkPointListViewModel.ItemsSource.Count == 0)
                    onMarkPointSelected(null);
            };
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