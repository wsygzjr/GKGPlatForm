using DynamicData;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.TemplateSubPage.TrajectorySequence;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.TemplateSubPage.TrajectorySequence;
using ReactiveUI;
using System.Reactive;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.TemplateSubPage.Command
{
    /// <summary>
    /// 选择点胶轨迹弹窗-视图模型
    /// </summary>
    public class SelectDispensingTrajectoryWindowViewModel :
        DataGridListBaseViewModel<TrajectorySequenceBaseModel, TrajectorySequenceCfgInfo>
    {

        private bool? _dialogResult;
        /// <summary>
        /// 对话框结果（true:保存，false:取消，null:未操作）
        /// </summary>
        public bool? DialogResult
        {
            get => _dialogResult;
            set => this.RaiseAndSetIfChanged(ref _dialogResult, value);
        }
        /// <summary>
        /// 保存命令
        /// </summary>
        public ReactiveCommand<Unit, Unit> SaveCommand { get; }

        /// <summary>
        /// 取消命令
        /// </summary>
        public ReactiveCommand<Unit, Unit> CancelCommand { get; }
        /// <summary>
        /// 构造函数
        /// </summary>
        public SelectDispensingTrajectoryWindowViewModel(Guid templateID) :base()
        {
            SaveCommand = ReactiveCommand.Create(save);
            CancelCommand = ReactiveCommand.Create(cancel);
            this.ItemsSource.Clear();
            this.ItemsSource.AddRange(CacheDataExchange.GetTrajectorySequenceModel(templateID));
        }

        /// <summary>
        /// /保存逻辑
        /// </summary>
        private void save()
        {
            DialogResult = true;
        }

        /// <summary>
        /// 取消逻辑
        /// </summary>
        private void cancel()
        {
            DialogResult = false;
        }
        #region 重载基类方法
        /// <summary>
        /// 
        /// </summary>
        protected override async Task _addItem()
        {
            throw new Exception("不支持的操作");
        }

        /// <summary>
        /// 单条删除
        /// </summary>
        protected override async Task<bool> _deleteItem(TrajectorySequenceBaseModel trajectorySequenceModel)
        {
            throw new Exception("不支持的操作");
        }
        #endregion

    }
}