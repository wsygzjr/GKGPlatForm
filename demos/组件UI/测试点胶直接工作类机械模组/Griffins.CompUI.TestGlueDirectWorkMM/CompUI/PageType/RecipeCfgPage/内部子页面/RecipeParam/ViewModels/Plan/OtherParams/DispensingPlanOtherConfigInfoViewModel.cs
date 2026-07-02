using Avalonia.Controls;
using Griffins.UI;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.Plan;
using ReactiveUI;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.Plan
{
    /// <summary>
    /// 点胶方案其他参数配置-视图模型
    /// </summary>
    public class DispensingPlanOtherConfigInfoViewModel : ReactiveObject
    {
        private Control? _viewReference;
        /// <summary>
        /// 值改变事件
        /// </summary>
        public EventHandler? AfterModified;
        #region 响应式控件ViewModel

        /// <summary>
        /// 2D检测间隔大PCS数
        /// </summary>
        public NumericViewModel TwoDCheckIntervalPcsViewModel { get; }
        /// <summary>
        /// 空跑坏板
        /// </summary>
        public ToggleSwitchViewModel RunEmptyBadBoardViewModel { get; }
        /// <summary>
        /// 使用第一块板的Mark
        /// </summary>
        public ToggleSwitchViewModel UseFirstBoardMarkViewModel { get; }

        /// <summary>
        /// 相机教导移动
        /// </summary>
        public CamreaPositionViewModel CamreaPositionViewModel { get; }
        #endregion

        #region 属性
        
        /// <summary>
        /// 2D检测间隔大PCS数
        /// </summary>
        public int TwoDCheckIntervalPcs
        {
            get => (int)TwoDCheckIntervalPcsViewModel.Value;
            set
            {
                TwoDCheckIntervalPcsViewModel.Value = value;
                this.RaisePropertyChanged(nameof(TwoDCheckIntervalPcs));
            }
        }

        /// <summary>
        /// 空跑坏板（true=启用，false=禁用）
        /// </summary>
        public bool RunEmptyBadBoard
        {
            get => RunEmptyBadBoardViewModel.IsChecked;
            set
            {
                RunEmptyBadBoardViewModel.IsChecked = value;
                this.RaisePropertyChanged(nameof(RunEmptyBadBoard));
            }
        }

        /// <summary>
        /// 使用第一块板的Mark（true=启用，false=禁用）
        /// </summary>
        public bool UseFirstBoardMark
        {
            get => UseFirstBoardMarkViewModel.IsChecked;
            set
            {
                UseFirstBoardMarkViewModel.IsChecked = value;
                this.RaisePropertyChanged(nameof(UseFirstBoardMark));
            }
        }
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public DispensingPlanOtherConfigInfoViewModel()
        {
            CamreaPositionViewModel = new CamreaPositionViewModel();
            

            TwoDCheckIntervalPcsViewModel = new NumericViewModel
            {
                Minimum = 0, 
                Maximum = 999,
                Increment = 1,
                Value = 0
            };

            RunEmptyBadBoardViewModel = new ToggleSwitchViewModel
            {
                IsChecked = false 
            };

            UseFirstBoardMarkViewModel = new ToggleSwitchViewModel
            {
                IsChecked = false
            };
            // 订阅值变更事件
            subscribeValueChanges();
        }

        #region 数据同步方法
        /// <summary>
        /// 从数据模型复制数据到ViewModel
        /// </summary>
        /// <param name="model">点胶方案其他参数配置模型</param>
        public void CopyFrom(DispensingPlanOtherConfigInfo model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model), "数据模型不能为空");

            // 整板偏移量
            CamreaPositionViewModel.X = (decimal)model.RecipeBoardOffset.X;
            CamreaPositionViewModel.Y = (decimal)model.RecipeBoardOffset.Y;
            // 2D检测间隔
            TwoDCheckIntervalPcs = model.TwoDCheckIntervalPcs;
            // 开关按钮
            RunEmptyBadBoard = model.RunEmptyBadBoard;
            UseFirstBoardMark = model.UseFirstBoardMark;
        }

        /// <summary>
        /// 从ViewModel复制数据到数据模型
        /// </summary>
        /// <param name="model">待填充的点胶方案其他参数配置模型</param>
        public void CopyTo(DispensingPlanOtherConfigInfo model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model), "数据模型不能为空");

            // 整板偏移量
            model.RecipeBoardOffset.X = (double)CamreaPositionViewModel.X;
            model.RecipeBoardOffset.Y = (double)CamreaPositionViewModel.Y;
            // 2D检测间隔
            model.TwoDCheckIntervalPcs = TwoDCheckIntervalPcs;
            // 开关按钮
            model.RunEmptyBadBoard = RunEmptyBadBoard;
            model.UseFirstBoardMark = UseFirstBoardMark;
        }
        /// <summary>
        /// 设置视图引用
        /// </summary>
        public void SetViewReference(Control view)
        {
            _viewReference = view;
            CamreaPositionViewModel.SetViewReference(view);
        }
        #endregion

        #region 私有方法

        #region 值改变事件
        /// <summary>
        /// 订阅值改变事件
        /// </summary>
        private void subscribeValueChanges()
        {
            CamreaPositionViewModel.AfterModified += onAfterModified;

            TwoDCheckIntervalPcsViewModel.ValueChanged += onValueChanged;
            RunEmptyBadBoardViewModel.ValueChanged += onValueChanged;
            UseFirstBoardMarkViewModel.ValueChanged += onValueChanged;
        }

        /// <summary>
        /// 值改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onValueChanged(object? sender, ValueChangedEventArgs e)
        {
            AfterModified?.Invoke(sender, e);
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
        #endregion
    }
}
