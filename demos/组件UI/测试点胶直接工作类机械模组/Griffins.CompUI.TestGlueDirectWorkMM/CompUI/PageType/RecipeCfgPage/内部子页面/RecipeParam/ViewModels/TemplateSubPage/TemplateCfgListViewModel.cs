using Avalonia.Controls;
using Griffins.UI.General;
using MsBox.Avalonia.Enums;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.TemplateSubPage;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.TemplateSubPage
{
    /// <summary>
    /// 模板配置列表-视图模型 
    /// </summary>
    public class TemplateCfgListViewModel : ReactiveObject
    {
        /// <summary>
        /// 视图引用（用于获取窗口上下文，显示对话框）
        /// </summary>
        private Control? _viewReference;
        /// <summary>
        /// 值改变事件
        /// </summary>
        public EventHandler? AfterModified;
        /// <summary>
        ///模板项变更事件
        /// </summary>
        public event EventHandler<EventArgs>? TemplateItemChanged;

        /// <summary>
        /// 模板配置列表
        /// </summary>
        private ObservableCollection<TemplateCfgViewModel> _templateCfgViewModels = new();
        public ObservableCollection<TemplateCfgViewModel> TemplateCfgViewModels
        {
            get => _templateCfgViewModels;
            set => this.RaiseAndSetIfChanged(ref _templateCfgViewModels, value);
        }

        /// <summary>
        /// 当前选中的模板模型
        /// </summary>
        private TemplateCfgViewModel? _selectedTemplateCfgViewModel;
        public TemplateCfgViewModel? SelectedTemplateCfgViewModel
        {
            get => _selectedTemplateCfgViewModel;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedTemplateCfgViewModel, value);
            }
        }
        #region 命令定义
        /// <summary>
        /// 添加模板命令
        /// </summary>
        public ReactiveCommand<Unit, Unit> AddCommand { get; }

        /// <summary>
        /// 删除模板命令
        /// </summary>
        public ReactiveCommand<TemplateCfgViewModel, Unit> DeleteCommand { get; }
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public TemplateCfgListViewModel()
        {
            TemplateCfgViewModels = new ObservableCollection<TemplateCfgViewModel>();
            // 只有当总数小于16时，才启用添加按钮
            var canSave = this.WhenAnyValue(
                x => x.TemplateCfgViewModels.Count,
                (count) => count < 16
            );
            AddCommand = ReactiveCommand.Create(addTemplateCfgInfo, canSave);
            DeleteCommand = ReactiveCommand.Create<TemplateCfgViewModel>(deleteTemplateCfgInfo);

            // 订阅值变更事件
            subscribeValueChanges();
        }



        /// <summary>
        /// 从数据模型复制数据到ViewModel
        /// </summary>
        /// <param name="templateCfgInfoes"></param>
        public void CopyFrom(TemplateCfgInfoList templateCfgInfoes)
        {
            TemplateCfgViewModels.Clear();
            if (_viewReference == null)
                throw new Exception("未设置上下文窗体");
            foreach (var templateCfgInfo in templateCfgInfoes)
            {
                var templateCfgViewModel = new TemplateCfgViewModel();
                templateCfgViewModel.AfterModified += onAfterModified;
                templateCfgViewModel.SetViewReference(_viewReference);
                templateCfgViewModel.CopyFrom(templateCfgInfo);
                TemplateCfgViewModels.Add(templateCfgViewModel);
            }

        }

        /// <summary>
        /// 从ViewModel复制数据到数据模型
        /// </summary>
        /// <param name="templateCfgInfoes"></param>
        public void CopyTo(TemplateCfgInfoList templateCfgInfoes)
        {
            templateCfgInfoes.Clear();
            foreach (var templateCfgViewModel in this.TemplateCfgViewModels)
            {
                var templateCfgInfo = new TemplateCfgInfo();
                templateCfgViewModel.CopyTo(templateCfgInfo);
                templateCfgInfoes.Add(templateCfgInfo);
            }
        }
        /// <summary>
        /// 设置视图引用（用于弹窗、对话框等UI操作）
        /// </summary>
        public void SetViewReference(Control view)
        {
            _viewReference= view;
        }

        /// <summary>
        /// 添加模板：创建新模型并打开编辑窗口
        /// </summary>
        private  void addTemplateCfgInfo()
        {
            var newTemplateCfgViewModel = new TemplateCfgViewModel();
            newTemplateCfgViewModel.SetViewReference(_viewReference!);
            var templateCfgInfo =new TemplateCfgInfo();
            templateCfgInfo.TemplateName= $"模板{_templateCfgViewModels.Count+1}";
            newTemplateCfgViewModel.CopyFrom(templateCfgInfo);
            newTemplateCfgViewModel.CachedView.DataContext = newTemplateCfgViewModel;
            newTemplateCfgViewModel.AfterModified += onAfterModified;
            TemplateCfgViewModels.Add(newTemplateCfgViewModel);
            SelectedTemplateCfgViewModel = newTemplateCfgViewModel;
        }

        /// <summary>
        /// 删除模板
        /// </summary>
        /// <param name="templateCfgViewModel">待删除的模板模型</param>
        private async void deleteTemplateCfgInfo(TemplateCfgViewModel templateCfgViewModel)
        {
            if (templateCfgViewModel == null) return;

            var confirmResult = await MessageBox.ShowConfirmDialog(
                title: "删除确认",
                message: $"确定要删除模板吗？\n删除后数据不可恢复。",
                _viewReference
            );

            if (confirmResult != ButtonResult.Yes) return;

            templateCfgViewModel.AfterModified -= onAfterModified;
            TemplateCfgViewModels.Remove(templateCfgViewModel);

            if (TemplateCfgViewModels.Count > 0)
            {
                SelectedTemplateCfgViewModel = TemplateCfgViewModels[0];
            }
            else
            {
                SelectedTemplateCfgViewModel = null;
            }
        }
        #region 值改变事件
        /// <summary>
        /// 订阅值改变事件
        /// </summary>
        private void subscribeValueChanges()
        {
            //订阅模板项改变事件
            this.TemplateCfgViewModels.CollectionChanged += (s, e) =>
            {
                TemplateItemChanged?.Invoke(this, new EventArgs());
                onAfterModified(this, new EventArgs());
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