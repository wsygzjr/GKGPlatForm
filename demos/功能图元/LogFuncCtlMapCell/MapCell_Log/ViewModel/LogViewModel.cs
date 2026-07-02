using Avalonia.Media;
using ReactiveUI;
using Splat.ModeDetection;
using System;
using System.Collections.ObjectModel;
using System.Reactive;
// using GKG.Log; // 引用你底层的日志实体与网关

namespace GKG.Map.LogFuncCtlMapCell.ViewModel
{
    public class LogViewModel : ReactiveObject, IDisposable
    {
        private readonly LogPropertyModelEdit _model;

        #region 样式属性

        public string FontFamily => _model.TextFont.FontFamily.ToString();

        public double FontSize => _model.TextFont.FontSize;

        public IBrush BackgroundColor => new SolidColorBrush(_model.BackColor);

        public IBrush TextColor => new SolidColorBrush(_model.TextColor);

        #endregion

        #region 实时日志 (热通道)

        // 绑定到界面的实时数据源
        public ObservableCollection<LogItemModel> RealTimeLogs { get; } = new ObservableCollection<LogItemModel>();

        #endregion

        #region 历史日志 (冷通道)

        // 查询条件绑定
        private DateTimeOffset _searchStartTime = DateTimeOffset.Now.Date; // 默认今天零点
        public DateTimeOffset SearchStartTime { get => _searchStartTime; set => this.RaiseAndSetIfChanged(ref _searchStartTime, value); }

        private DateTimeOffset _searchEndTime = DateTimeOffset.Now;
        public DateTimeOffset SearchEndTime { get => _searchEndTime; set => this.RaiseAndSetIfChanged(ref _searchEndTime, value); }

        private string _searchModuleText = string.Empty;
        public string SearchModuleText { get => _searchModuleText; set => this.RaiseAndSetIfChanged(ref _searchModuleText, value); }

        // 绑定到界面的历史数据源
        public ObservableCollection<LogItemModel> HistoricalLogs { get; } = new ObservableCollection<LogItemModel>();

        #endregion

        // 供 view 绑定的 清空 和 查询 命令
        public ReactiveCommand<Unit, Unit> ClearRealTimeLogsCommand { get; }
        public ReactiveCommand<Unit, Unit> QueryHistoricalLogsCommand { get; }

        // 供 MapCellLogCtlObj 订阅，用于调用服务查询历史日志
        public Action? OnQueryClicked;

        public LogViewModel(LogPropertyModelEdit model)
        {
            _model = model ?? throw new ArgumentNullException(nameof(model));

            // 初始化命令
            ClearRealTimeLogsCommand = ReactiveCommand.Create(ClearLogs);
            QueryHistoricalLogsCommand = ReactiveCommand.Create(() =>
            {
                OnQueryClicked?.Invoke();
            });
        }

        /// <summary>
        /// 清空实时日志
        /// </summary>
        private void ClearLogs()
        {
            RealTimeLogs.Clear();
        }

        public void AppendRealTimeLog()
        {
            // Avalonia 跨线程安全添加
            Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
            {
                // 限制最大行数，防止内存撑爆 (例如最大 1000 行)
                if (RealTimeLogs.Count > 1000)
                {
                    RealTimeLogs.RemoveAt(0);
                }
                RealTimeLogs.Add(_model.NewLog);
            });
        }

        public void RefreshDisplayStyle()
        {
            this.RaisePropertyChanged(nameof(FontFamily));
            this.RaisePropertyChanged(nameof(FontSize));
            this.RaisePropertyChanged(nameof(BackgroundColor));
            this.RaisePropertyChanged(nameof(TextColor));
        }

        public void Dispose() { }
    }
}