using GKG.UI;
using ReactiveUI;
using System.Reactive;

namespace Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.ViewModels
{
    internal sealed class BoardWorkspaceViewModel : ReactiveObject
    {
        /// <summary>是否启用圆角平滑。</summary>
        private bool _enableCornerSmoothing;
        /// <summary>暂停时是否恢复点胶称重。</summary>
        private bool _pauseResumeGlueWeighing;
        /// <summary>暂停后是否回待机点。</summary>
        private bool _pauseReturnStandby;

        /// <summary>初始化整板参数工作区。</summary>
        public BoardWorkspaceViewModel()
        {
            CornerSmoothingCoeff = new NumericUpDownViewModel
            {
                DecimalPlaces = 2,
                Increment = 0.01m,
                Maximum = 999.99m,
            };
            GlueAlarmContinueDispensing = new NumericUpDownViewModel
            {
                DecimalPlaces = 0,
                Increment = 1m,
                Maximum = 999999m,
            };
            CalibrationReminderInterval = new NumericUpDownViewModel
            {
                DecimalPlaces = 0,
                Increment = 1m,
                Maximum = 999999m,
            };
            RefPointX = new NumericViewModel { DecimalPlaces = 2, Increment = 0.01m, Maximum = 99999.99m };
            RefPointY = new NumericViewModel { DecimalPlaces = 2, Increment = 0.01m, Maximum = 99999.99m };
            RefPointZ = new NumericViewModel { DecimalPlaces = 2, Increment = 0.01m, Maximum = 99999.99m };

            TeachCommand = ReactiveCommand.Create(() => { });
            FullBoardPreviewCommand = ReactiveCommand.Create(() => { });
        }

        /// <summary>圆角平滑系数输入控件。</summary>
        public NumericUpDownViewModel CornerSmoothingCoeff { get; }
        /// <summary>胶量报警后继续点胶阈值输入控件。</summary>
        public NumericUpDownViewModel GlueAlarmContinueDispensing { get; }
        /// <summary>校准提醒间隔输入控件。</summary>
        public NumericUpDownViewModel CalibrationReminderInterval { get; }
        /// <summary>参考点 X 输入控件。</summary>
        public NumericViewModel RefPointX { get; }
        /// <summary>参考点 Y 输入控件。</summary>
        public NumericViewModel RefPointY { get; }
        /// <summary>参考点 Z 输入控件。</summary>
        public NumericViewModel RefPointZ { get; }
        /// <summary>整板示教命令（预留）。</summary>
        public ReactiveCommand<Unit, Unit> TeachCommand { get; }
        /// <summary>整板预览命令（预留）。</summary>
        public ReactiveCommand<Unit, Unit> FullBoardPreviewCommand { get; }

        /// <summary>是否启用圆角平滑。</summary>
        public bool EnableCornerSmoothing
        {
            get => _enableCornerSmoothing;
            set => this.RaiseAndSetIfChanged(ref _enableCornerSmoothing, value);
        }

        /// <summary>暂停时是否恢复点胶称重。</summary>
        public bool PauseResumeGlueWeighing
        {
            get => _pauseResumeGlueWeighing;
            set => this.RaiseAndSetIfChanged(ref _pauseResumeGlueWeighing, value);
        }

        /// <summary>暂停后是否回待机点。</summary>
        public bool PauseReturnStandby
        {
            get => _pauseReturnStandby;
            set => this.RaiseAndSetIfChanged(ref _pauseReturnStandby, value);
        }
    }
}
