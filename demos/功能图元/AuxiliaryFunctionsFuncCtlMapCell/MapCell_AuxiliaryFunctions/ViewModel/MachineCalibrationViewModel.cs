using GKG.Map.AuxiliaryFunctionsFuncCtlMapCell.Model;
using GKG.Map.AuxiliaryFunctionsFuncCtlMapCell.ViewModel;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace GKG.Map.AuxiliaryFunctionsFuncCtlMapCell.ViewModel
{
    // ==========================================
    // 基础步骤类
    // ==========================================
    public abstract class CalibrationStepViewModel : ReactiveObject
    {
        public string StepTitle { get; protected set; }
        public bool IsDoubleValve { get; }

        private bool _isCompleted;
        public bool IsCompleted
        {
            get => _isCompleted;
            protected set => this.RaiseAndSetIfChanged(ref _isCompleted, value);
        }

        public Action<string> LogMessageAction { get; set; }

        protected CalibrationStepViewModel(string stepTitle, bool isDoubleValve)
        {
            StepTitle = stepTitle;
            IsDoubleValve = isDoubleValve;
            IsCompleted = false; // 默认未完成，锁住下一步
        }
    }

    // ==========================================
    // 第 1 步：开始校正
    // ==========================================
    public class StartStepViewModel : CalibrationStepViewModel
    {
        public ReactiveCommand<Unit, Unit> StartCommand { get; }

        public StartStepViewModel(bool isDoubleValve) : base("一键校正", isDoubleValve)
        {
            // 开始校正如果是纯软件切换，不需要等待框
            StartCommand = ReactiveCommand.Create(() =>
            {
                LogMessageAction?.Invoke("请开始校正...");
                IsCompleted = true;
            });
        }
    }

    // ==========================================
    // 第 2 步：排胶
    // ==========================================
    public class OutGlueStepViewModel : CalibrationStepViewModel
    {
        private int _leftPoints = 30;
        public int LeftPoints { get => _leftPoints; set => this.RaiseAndSetIfChanged(ref _leftPoints, value); }

        private int _rightPoints = 30;
        public int RightPoints { get => _rightPoints; set => this.RaiseAndSetIfChanged(ref _rightPoints, value); }

        public string RightButtonText { get; }
        public ReactiveCommand<Unit, Unit> LeftValveCommand { get; }
        public ReactiveCommand<Unit, Unit> RightValveCommand { get; }

        public OutGlueStepViewModel(bool isDoubleValve, Interaction<WaitingBoxContext, Unit> showWaitingBox)
            : base("排胶", isDoubleValve)
        {
            if (!isDoubleValve) IsCompleted = true; // 单阀直接允许跳过左边
            RightButtonText = isDoubleValve ? "右阀" : "排胶";

            LeftValveCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var ctx = new WaitingBoxContext
                {
                    Message = $"左阀正在排胶中... 点数: {LeftPoints}",
                    WorkTask = async () => { await Task.Delay(3000); } // 模拟耗时
                };
                await showWaitingBox.Handle(ctx);
                LogMessageAction?.Invoke($"左阀排胶完成");
                IsCompleted = true;
            });

            RightValveCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                string msg = isDoubleValve ? $"右阀正在排胶中... 点数: {RightPoints}" : $"正在排胶中... 点数: {RightPoints}";
                var ctx = new WaitingBoxContext
                {
                    Message = msg,
                    WorkTask = async () => { await Task.Delay(3000); } // 模拟耗时
                };
                await showWaitingBox.Handle(ctx);
                LogMessageAction?.Invoke(isDoubleValve ? $"右阀排胶完成" : $"排胶完成");
                IsCompleted = true;
            });
        }
    }

    // ==========================================
    // 第 3, 4, 6 步：通用双按钮页 (测高、精确校准、单点重校正)
    // ==========================================
    public class GenericActionStepViewModel : CalibrationStepViewModel
    {
        public string RightButtonText { get; }
        public ReactiveCommand<Unit, Unit> LeftActionCommand { get; }
        public ReactiveCommand<Unit, Unit> RightActionCommand { get; }

        public GenericActionStepViewModel(string title, bool isDoubleValve, Interaction<WaitingBoxContext, Unit> showWaitingBox)
            : base(title, isDoubleValve)
        {
            RightButtonText = isDoubleValve ? "右阀" : "执行";

            LeftActionCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var ctx = new WaitingBoxContext
                {
                    Message = $"左阀正在进行 {title}，请稍候...",
                    WorkTask = async () => { await Task.Delay(3000); }
                };
                await showWaitingBox.Handle(ctx);
                LogMessageAction?.Invoke($"左阀 {title} 完成");
                IsCompleted = true;
            });

            RightActionCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                string msg = isDoubleValve ? $"右阀正在进行 {title}" : $"正在进行 {title}";
                var ctx = new WaitingBoxContext
                {
                    Message = $"{msg}，请稍候...",
                    WorkTask = async () => { await Task.Delay(3000); }
                };
                await showWaitingBox.Handle(ctx);
                LogMessageAction?.Invoke($"{msg} 完成");
                IsCompleted = true;
            });
        }
    }

    // ==========================================
    // 第 5 步：旋转倾斜校准页
    // ==========================================
    public class ObliqueTeachStepViewModel : CalibrationStepViewModel
    {
        public ReactiveCommand<Unit, Unit> R0Command { get; }
        public ReactiveCommand<Unit, Unit> R90Command { get; }

        public ObliqueTeachStepViewModel(bool isDoubleValve, Interaction<WaitingBoxContext, Unit> showWaitingBox)
            : base("倾斜校准", isDoubleValve)
        {
            R0Command = ReactiveCommand.CreateFromTask(async () =>
            {
                var ctx = new WaitingBoxContext
                {
                    Message = "旋转0度开始校准，请稍候...",
                    WorkTask = async () => { await Task.Delay(3000); }
                };
                await showWaitingBox.Handle(ctx);
                LogMessageAction?.Invoke("旋转0度校准完成");
                IsCompleted = true;
            });

            R90Command = ReactiveCommand.CreateFromTask(async () =>
            {
                var ctx = new WaitingBoxContext
                {
                    Message = "旋转90度开始校准，请稍候...",
                    WorkTask = async () => { await Task.Delay(3000); }
                };
                await showWaitingBox.Handle(ctx);
                LogMessageAction?.Invoke("旋转90度校准完成");
                IsCompleted = true;
            });
        }
    }

    // ==========================================
    // 主向导状态机 ViewModel
    // ==========================================
    public class MachineCalibrationViewModel : ReactiveObject
    {
        public ObservableCollection<CalibrationStepViewModel> Steps { get; } = new();
        public ObservableCollection<string> Logs { get; } = new();

        public Interaction<WaitingBoxContext, Unit> ShowWaitingBoxDialog { get; }
        public Interaction<Unit, Unit> RequestCloseDialog { get; }

        private CalibrationStepViewModel _currentStep;
        public CalibrationStepViewModel CurrentStep
        {
            get => _currentStep;
            set => this.RaiseAndSetIfChanged(ref _currentStep, value);
        }

        private string _nextButtonText = "➔ 下一步";
        public string NextButtonText
        {
            get => _nextButtonText;
            set => this.RaiseAndSetIfChanged(ref _nextButtonText, value);
        }

        public ReactiveCommand<Unit, Unit> PrevCommand { get; }
        public ReactiveCommand<Unit, Unit> NextCommand { get; }

        public MachineCalibrationViewModel(bool isDoubleValve, CalibrationConfig config)
        {
            ShowWaitingBoxDialog = new Interaction<WaitingBoxContext, Unit>();
            RequestCloseDialog = new Interaction<Unit, Unit>();

            // 1. 动态装配流程步骤

            // 开始页是必须的
            Steps.Add(new StartStepViewModel(isDoubleValve));

            // 根据配置决定是否添加 排胶页
            if (config.HasOutGlue)
            {
                Steps.Add(new OutGlueStepViewModel(isDoubleValve, ShowWaitingBoxDialog));
            }

            // 根据配置决定是否添加 激光测高页
            if (config.HasLaser)
            {
                Steps.Add(new GenericActionStepViewModel("激光测高", isDoubleValve, ShowWaitingBoxDialog));
            }

            // 根据配置决定是否添加 精确校准页
            if (config.HasPreciseTeach)
            {
                Steps.Add(new GenericActionStepViewModel("精确校准", isDoubleValve, ShowWaitingBoxDialog));
            }

            // 根据配置决定是否添加 倾斜校准页
            if (config.HasObliqueTeach)
            {
                Steps.Add(new ObliqueTeachStepViewModel(isDoubleValve, ShowWaitingBoxDialog));
            }

            // 根据配置决定是否添加 单点重校正页
            if (config.HasOnePointWeight)
            {
                Steps.Add(new GenericActionStepViewModel("单点重校正", isDoubleValve, ShowWaitingBoxDialog));
            }

            // 2. 注入日志委托
            foreach (var step in Steps)
            {
                step.LogMessageAction = msg =>
                {
                    string time = DateTime.Now.ToString("[yyyy.MM.dd HH:mm:ss] ");
                    Logs.Insert(0, time + msg); // 最新日志插在最顶端
                };
            }

            CurrentStep = Steps[0];

            // 3. 上一步逻辑
            var canGoPrev = this.WhenAnyValue(x => x.CurrentStep, step => Steps.IndexOf(step) > 0);
            PrevCommand = ReactiveCommand.Create(() =>
            {
                int index = Steps.IndexOf(CurrentStep);
                CurrentStep = Steps[index - 1];
                UpdateNextButtonText();
            }, canGoPrev);

            // 4. 下一步逻辑 (受 IsCompleted 严格约束)
            var canGoNext = this.WhenAnyValue(
                x => x.CurrentStep,
                x => x.CurrentStep.IsCompleted,
                (step, isCompleted) => isCompleted);

            NextCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                int index = Steps.IndexOf(CurrentStep);
                if (index < Steps.Count - 1)
                {
                    CurrentStep = Steps[index + 1];
                    UpdateNextButtonText();
                }
                else
                {
                    // 已经是最后一页，触发关闭窗口
                    await RequestCloseDialog.Handle(Unit.Default);
                }
            }, canGoNext);
        }

        private void UpdateNextButtonText()
        {
            NextButtonText = Steps.IndexOf(CurrentStep) == Steps.Count - 1 ? "✔ 完成" : "➔ 下一步";
        }
    }
}