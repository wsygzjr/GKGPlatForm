using Griffins;
using Griffins.Map.Cmd;
using ReactiveUI;
using System;
using System.Reactive;

namespace GKG.Map.AuxiliaryFunctionsFuncCtlMapCell.ViewModel
{
    public class OutGlueWaxSetViewModel : ReactiveObject
    {
        private readonly MapCmdExector _cmdExector;
        private readonly string _mapNo;

        // ==========================================
        // 界面绑定属性
        // ==========================================
        public string Title { get; }
        public string PointsLabel { get; }
        public bool IsOutGlue { get; }

        private int _points;
        public int Points { get => _points; set => this.RaiseAndSetIfChanged(ref _points, value); }

        private double _intervalTime;
        public double IntervalTime { get => _intervalTime; set => this.RaiseAndSetIfChanged(ref _intervalTime, value); }

        // 排胶次数模式
        private bool _isTimesMode = true;
        public bool IsTimesMode { get => _isTimesMode; set => this.RaiseAndSetIfChanged(ref _isTimesMode, value); }

        private int _times;
        public int Times { get => _times; set => this.RaiseAndSetIfChanged(ref _times, value); }

        // 持续时间模式
        private bool _isTotalTimeMode;
        public bool IsTotalTimeMode { get => _isTotalTimeMode; set => this.RaiseAndSetIfChanged(ref _isTotalTimeMode, value); }

        private double _totalTime;
        public double TotalTime { get => _totalTime; set => this.RaiseAndSetIfChanged(ref _totalTime, value); }

        // ==========================================
        // 命令与委托
        // ==========================================
        public ReactiveCommand<Unit, Unit> SaveCommand { get; }
        public ReactiveCommand<Unit, Unit> CancelCommand { get; }

        // 通知 View 关闭的事件
        public Action CloseAction { get; set; }

        public OutGlueWaxSetViewModel(bool isOutGlue, MapCmdExector cmdExector, string mapNo)
        {
            IsOutGlue = isOutGlue;
            _cmdExector = cmdExector;
            _mapNo = mapNo;

            // 根据类型动态设置界面文字
            if (isOutGlue)
            {
                Title = "一键排胶";
                PointsLabel = "排胶点数:";
            }
            else
            {
                Title = "一键排蜡";
                PointsLabel = "排蜡点数:";
            }

            LoadParams();

            // 联动互斥逻辑 (在 Avalonia 中 RadioButton 同组会自动互斥，这里加一层 ViewModel 保护)
            this.WhenAnyValue(x => x.IsTimesMode).Subscribe(v => IsTotalTimeMode = !v);
            this.WhenAnyValue(x => x.IsTotalTimeMode).Subscribe(v => IsTimesMode = !v);

            SaveCommand = ReactiveCommand.Create(() =>
            {
                SaveParams();
                CloseAction?.Invoke();
            });

            CancelCommand = ReactiveCommand.Create(() =>
            {
                CloseAction?.Invoke();
            });
        }

        private void LoadParams()
        {
            string cmdID = IsOutGlue ? "ReadPurgeParams" : "ReadWaxParams";
            try
            {
                GFBaseTypeParamValueList paramList = _cmdExector.ExecUIDataObjCommand(_mapNo, cmdID, new GFBaseTypeParamValueList());
                if (paramList != null)
                {
                    foreach (var param in paramList)
                    {
                        switch (param.ID)
                        {
                            case "Points":
                                Points = param.Value?.ToPrimitiveValue<int>() ?? 0;
                                break;
                            case "IntervalTime":
                                IntervalTime = param.Value?.ToPrimitiveValue<double>() ?? 0.0;
                                break;
                            case "Times":
                                Times = param.Value?.ToPrimitiveValue<int>() ?? 0;
                                break;
                            case "TotalTime":
                                TotalTime = param.Value?.ToPrimitiveValue<double>() ?? 0.0;
                                break;
                            case "IsTimesMode":
                                IsTimesMode = param.Value?.ToPrimitiveValue<bool>() ?? true;
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"执行 {cmdID} 失败: {ex.Message}");

                // 这里用假数据模拟加载
                Points = 30;
                IntervalTime = 0.5;
                Times = 10;
                TotalTime = 5.0;
                IsTimesMode = true;
            }
        }

        private void SaveParams()
        {
            string cmdID = IsOutGlue ? "SavePurgeParams" : "SaveWaxParams";
            try
            {
                GFBaseTypeParamValueList paramList = _cmdExector.ExecUIDataObjCommand(_mapNo, cmdID, new GFBaseTypeParamValueList
                {
                    new GFBaseTypeParamValue { ID = "Points", Value = new GriffinsBaseValue(Points) },
                    new GFBaseTypeParamValue { ID = "IntervalTime", Value = new GriffinsBaseValue(IntervalTime) },
                    new GFBaseTypeParamValue { ID = "Times", Value = new GriffinsBaseValue(Times) },
                    new GFBaseTypeParamValue { ID = "TotalTime", Value = new GriffinsBaseValue(TotalTime) },
                    new GFBaseTypeParamValue { ID = "IsTimesMode", Value = new GriffinsBaseValue(IsTimesMode) }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"执行 {cmdID} 失败: {ex.Message}");
            }
        }
    }
}