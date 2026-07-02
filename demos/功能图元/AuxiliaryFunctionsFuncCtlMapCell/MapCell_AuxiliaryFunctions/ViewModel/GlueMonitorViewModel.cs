using ReactiveUI;
using System;
using System.Reactive;

namespace GKG.Map.AuxiliaryFunctionsFuncCtlMapCell.ViewModel
{
    // 最底层的单项指标模型 (时间/重量/Pcs)
    public class MetricItemViewModel : ReactiveObject
    {
        public string Name { get; }

        private string _valueText;
        public string ValueText
        {
            get => _valueText;
            set => this.RaiseAndSetIfChanged(ref _valueText, value);
        }

        private bool _isAlarm;
        public bool IsAlarm
        {
            get => _isAlarm;
            set => this.RaiseAndSetIfChanged(ref _isAlarm, value);
        }

        public MetricItemViewModel(string name, string initialValue, bool isAlarm = false)
        {
            Name = name;
            ValueText = initialValue;
            IsAlarm = isAlarm;
        }
    }

    // 单个阀门的模型 (左阀/右阀)
    public class ValveViewModel : ReactiveObject
    {
        public string Title { get; }
        public bool IsVisible { get; }

        public MetricItemViewModel TimeMetric { get; }
        public MetricItemViewModel WeightMetric { get; }
        public MetricItemViewModel PcsMetric { get; }

        public ReactiveCommand<Unit, Unit> RefreshCommand { get; }

        public ValveViewModel(string title, bool isVisible, Action onRefreshed)
        {
            Title = title;
            IsVisible = isVisible;

            // 初始化假数据，实际开发中这里从你的硬件读取
            TimeMetric = new MetricItemViewModel("时间(h)", "0", true);   // 模拟报警状态
            WeightMetric = new MetricItemViewModel("重量(mg)", "0", true); // 模拟报警状态
            PcsMetric = new MetricItemViewModel("Pcs", "0", true);         // 模拟报警状态

            // 刷新命令
            RefreshCommand = ReactiveCommand.Create(() =>
            {
                // 模拟刷新后余量恢复，报警解除
                TimeMetric.ValueText = "24"; TimeMetric.IsAlarm = false;
                WeightMetric.ValueText = "500"; WeightMetric.IsAlarm = false;
                PcsMetric.ValueText = "1000"; PcsMetric.IsAlarm = false;

                // 通知外部（主ViewModel）已经刷新过了
                onRefreshed?.Invoke();
            });
        }
    }

    // 整个窗口的主 ViewModel
    public class GlueMonitorViewModel : ReactiveObject
    {
        public ValveViewModel LeftValve { get; }
        public ValveViewModel RightValve { get; }

        // 标记是否已经刷新过（用于防呆逻辑）
        private bool _hasRefreshed;
        public bool HasRefreshed
        {
            get => _hasRefreshed;
            set => this.RaiseAndSetIfChanged(ref _hasRefreshed, value);
        }

        // 下一步命令
        public ReactiveCommand<Unit, Unit> NextStepCommand { get; }

        public GlueMonitorViewModel(bool isDoubleValve)
        {
            // 只要有任何一个阀门刷新，就解锁下一步
            Action onValveRefreshed = () => HasRefreshed = true;

            LeftValve = new ValveViewModel("左阀刷新", isVisible: isDoubleValve, onValveRefreshed);
            RightValve = new ValveViewModel("右阀刷新", isVisible: true, onValveRefreshed);

            // 响应式条件：仅当 HasRefreshed 为 true 时，NextStepCommand 才允许执行
            var canGoNext = this.WhenAnyValue(x => x.HasRefreshed);
            NextStepCommand = ReactiveCommand.Create(() => { /* 执行下一步逻辑 */ }, canGoNext);
        }
    }
}
