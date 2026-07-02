using Avalonia.Controls;
using Griffins.UI;
using ReactiveUI;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Reactive;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.TemplateSubPage.TrajectorySequence
{

    /// <summary>
    /// 平移点 视图模型
    /// </summary>
    public class TranslationPositionViewModel : ReactiveObject
    {
        private bool? _dialogResult;
        /// <summary>
        /// 视图引用（用于显示对话框）
        /// </summary>
        private Control? _viewReference;

        /// <summary>
        /// X轴坐标(mm) 数据模型（decimal类型）
        /// </summary>
        public NumericViewModel XViewModel { get; }

        /// <summary>
        /// Y轴坐标(mm) 数据模型
        /// </summary>
        public NumericViewModel YViewModel { get; }

        /// <summary>
        /// Z轴坐标(mm) 数据模型
        /// </summary>
        public NumericViewModel ZViewModel { get; }

        /// <summary>
        /// X轴坐标
        /// </summary>
        public decimal X
        {
            get => XViewModel.Value;
            set => XViewModel.Value = value;
        }

        /// <summary>
        /// Y轴坐标
        /// </summary>
        public decimal Y
        {
            get => YViewModel.Value;
            set => YViewModel.Value = value;
        }

        /// <summary>
        /// Z轴坐标
        /// </summary>
        public decimal Z
        {
            get => ZViewModel.Value;
            set => ZViewModel.Value = value;
        }
        /// <summary>
		/// 对话框结果（true:保存，false:取消，null:未操作）
		/// </summary>
		public bool? DialogResult
        {
            get => _dialogResult;
            set => this.RaiseAndSetIfChanged(ref _dialogResult, value);
        }

        // 命令
        /// <summary>
        /// 保存命令
        /// </summary>
        public ReactiveCommand<Unit, Unit> SaveCommand { get; }

        /// <summary>
        /// 取消命令
        /// </summary>
        public ReactiveCommand<Unit, Unit> CancelCommand { get; }
        public TranslationPositionViewModel()
        {  
            // 坐标默认支持负数（如机械原点偏移），步长0.001mm
            XViewModel = new NumericViewModel { Increment = 0.001m, DecimalPlaces = 3, Minimum = 0.000m, Maximum = 50.000m, };
            YViewModel = new NumericViewModel { Increment = 0.001m, DecimalPlaces = 3, Minimum = 0.000m, Maximum = 50.000m, };
            ZViewModel = new NumericViewModel { Increment = 0.001m, DecimalPlaces = 3, Minimum = 0.000m, Maximum = 50.000m, };

            SaveCommand = ReactiveCommand.Create(save);
            CancelCommand = ReactiveCommand.Create(cancel);
        }

        /// <summary>
        /// 设置视图引用（供对话框使用）
        /// </summary>
        public void SetViewReference(Control view)
        {
            _viewReference = view ?? throw new ArgumentNullException(nameof(view), "视图引用不能为空");
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
    }

}
