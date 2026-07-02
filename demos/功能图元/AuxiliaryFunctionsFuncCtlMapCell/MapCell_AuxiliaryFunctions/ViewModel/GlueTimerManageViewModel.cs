using Griffins;
using Griffins.Map.Cmd;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Text.Json;
using System.Threading.Tasks;

namespace GKG.Map.AuxiliaryFunctionsFuncCtlMapCell.ViewModel
{
    // ==========================================
    // 数据模型定义 (单行胶水记录)
    // ==========================================
    public class GlueTimeItem : ReactiveObject
    {
        private string _glueName = "New Glue";
        public string GlueName { get => _glueName; set => this.RaiseAndSetIfChanged(ref _glueName, value); }

        private double _timeSet = 3;
        public double TimeSet { get => _timeSet; set => this.RaiseAndSetIfChanged(ref _timeSet, value); }

        private string _unitSet = "Day";
        public string UnitSet { get => _unitSet; set => this.RaiseAndSetIfChanged(ref _unitSet, value); }

        private DateTime _start = DateTime.Now;
        public DateTime Start { get => _start; set => this.RaiseAndSetIfChanged(ref _start, value); }

        private string _used = "00:00:00";
        public string Used { get => _used; set => this.RaiseAndSetIfChanged(ref _used, value); }

        private string _remain = "00:00:00";
        public string Remain { get => _remain; set => this.RaiseAndSetIfChanged(ref _remain, value); }

        private double _scale;
        public double Scale { get => _scale; set => this.RaiseAndSetIfChanged(ref _scale, value); }

        private bool _isEnabled = false;
        public bool IsEnabled { get => _isEnabled; set => this.RaiseAndSetIfChanged(ref _isEnabled, value); }
    }

    // ==========================================
    // 窗体主 ViewModel
    // ==========================================
    public class GlueTimerManageViewModel : ReactiveObject
    {
        // 插件通讯依赖
        private readonly MapCmdExector _cmdExector;
        private readonly string _mapNo;

        // 通知 View 关闭的委托 (传入 bool 代表 DialogResult: true=OK, false=Cancel)
        public Action<bool> CloseAction { get; set; }

        public GlueTimeItemCollection GlueTimerCollection { get; } = new GlueTimeItemCollection();
        public ObservableCollection<GlueTimeItem> GlueTimeItems => GlueTimerCollection.Items;

        private int _selectedIndex = -1;
        public int SelectedIndex { get => _selectedIndex; set => this.RaiseAndSetIfChanged(ref _selectedIndex, value); }

        // 命令集合
        public ReactiveCommand<Unit, Unit> AddGlueItemCommand { get; }
        public ReactiveCommand<Unit, Unit> DeleteGlueItemCommand { get; }
        public ReactiveCommand<Unit, Unit> ResetAllGlueItemCommand { get; }
        public ReactiveCommand<GlueTimeItem, Unit> ResetGlueItemCommand { get; }
        public ReactiveCommand<Unit, Unit> OKCommand { get; }
        public ReactiveCommand<Unit, Unit> CancelCommand { get; }

        public GlueTimerManageViewModel(MapCmdExector cmdExector, string mapNo)
        {
            _cmdExector = cmdExector;
            _mapNo = mapNo;

            // 添加行
            AddGlueItemCommand = ReactiveCommand.Create(() =>
            {
                var newItem = new GlueTimeItem();
                CalculateTimeStatus(newItem); // 初始化状态
                GlueTimeItems.Add(newItem);
            });

            // 删除选中行
            DeleteGlueItemCommand = ReactiveCommand.Create(() =>
            {
                if (SelectedIndex >= 0 && SelectedIndex < GlueTimeItems.Count)
                {
                    GlueTimeItems.RemoveAt(SelectedIndex);
                }
            });

            // 重置所有项
            ResetAllGlueItemCommand = ReactiveCommand.Create(() =>
            {
                foreach (var item in GlueTimeItems)
                {
                    item.Start = DateTime.Now;
                    CalculateTimeStatus(item);
                }
            });

            // 重置单项 (传参为当前行的数据源)
            ResetGlueItemCommand = ReactiveCommand.Create<GlueTimeItem>(item =>
            {
                if (item != null)
                {
                    item.Start = DateTime.Now;
                    CalculateTimeStatus(item);
                }
            });

            // 确定按钮
            OKCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                await SaveDataAsync();
                CloseAction?.Invoke(true);
            });

            // 取消按钮
            CancelCommand = ReactiveCommand.Create(() =>
            {
                CloseAction?.Invoke(false);
            });
        }

        // ==========================================
        // 业务逻辑封装
        // ==========================================

        /// <summary>
        /// 初始化加载数据
        /// </summary>
        public async Task InitDataAsync()
        {
            try
            {
                // 从底层获取数据对象
                GFBaseTypeParamValueList result = _cmdExector.ExecUIDataObjCommand(_mapNo, "LoadGlueTimers", new GFBaseTypeParamValueList());

                var baseValue = result?.FirstOrDefault(p => p.ID == "GlueTimersData")?.Value;

                if (baseValue != null)
                {
                    ((IGriffinsBaseValue)GlueTimerCollection).PopulateFromBaseValue(baseValue);

                    foreach (var item in GlueTimerCollection.Items)
                    {
                        CalculateTimeStatus(item);
                    }
                }

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"执行 LoadGlueTimers 失败: {ex.Message}");

                var mockData = new[]
                {
                    new GlueTimeItem
                    {
                        GlueName = "Loctite 3536 (正常)",
                        TimeSet = 7,
                        UnitSet = "Day",
                        Start = DateTime.Now.AddDays(-2),
                        IsEnabled = true
                    },
                    new GlueTimeItem
                    {
                        GlueName = "EPO-TEK H20E (已过期)",
                        TimeSet = 3,
                        UnitSet = "Day",
                        Start = DateTime.Now.AddDays(-5),
                        IsEnabled = true
                    },
                    new GlueTimeItem
                    {
                        GlueName = "Dow Corning (未启用)",
                        TimeSet = 14,
                        UnitSet = "Day",
                        Start = DateTime.Now,
                        IsEnabled = false
                    }
                };


                foreach (var item in mockData)
                {
                    CalculateTimeStatus(item);
                    GlueTimeItems.Add(item);
                }
            }
        }

        /// <summary>
        /// 保存数据到后端
        /// </summary>
        private async Task SaveDataAsync()
        {
            try
            {
                _cmdExector.ExecUIDataObjCommand(_mapNo, "SaveGlueTimers", new GFBaseTypeParamValueList
                {
                    new GFBaseTypeParamValue
                    {
                        ID = "GlueTimeItems",
                        Value = new GriffinsBaseValue(GlueTimerCollection)
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"执行 SaveGlueTimers 失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 核心计算逻辑：计算已用时间、剩余时间和进度条比例
        /// </summary>
        private void CalculateTimeStatus(GlueTimeItem item)
        {
            if (!item.IsEnabled)
            {
                item.Used = "00:00:00";
                item.Remain = "00:00:00";
                item.Scale = 0;
                return;
            }

            DateTime endTime = item.Start.AddDays(item.TimeSet);
            DateTime now = DateTime.Now;

            if (endTime <= now)
            {
                // 已过期
                TimeSpan totalUsedTime = endTime - item.Start;
                item.Used = $"{(int)totalUsedTime.TotalHours:D2}:{totalUsedTime.Minutes:D2}:{totalUsedTime.Seconds:D2}";
                item.Remain = "00:00:00";
                item.Scale = 1.0; // 进度条满 100%
            }
            else
            {
                // 倒计时中
                TimeSpan currentUsedTime = now - item.Start;
                TimeSpan remainTime = endTime - now;

                // 使用 TotalHours 转 int 确保超过 24 小时能正确显示，例如 "48:30:15"
                item.Used = $"{(int)currentUsedTime.TotalHours:D2}:{currentUsedTime.Minutes:D2}:{currentUsedTime.Seconds:D2}";
                item.Remain = $"{(int)remainTime.TotalHours:D2}:{remainTime.Minutes:D2}:{remainTime.Seconds:D2}";

                // 计算进度比例 (0.0 ~ 1.0)
                item.Scale = currentUsedTime.TotalMilliseconds / (endTime - item.Start).TotalMilliseconds;
            }
        }
    }


    /// <summary>
    /// 用于与底层 Griffins 框架交互的胶水计时列表包装类
    /// </summary>
    public class GlueTimeItemCollection : IGriffinsBaseValue
    {
        public static readonly Guid Object_ID = new Guid("{F046272A-AEA6-4AC5-BAB7-7E800C880734}");

        public ObservableCollection<GlueTimeItem> Items { get; } = new ObservableCollection<GlueTimeItem>();

        #region IGriffinsBaseValue 成员

        bool IGriffinsBaseValue.IsObject_Byte => false;

        Guid IGriffinsBaseValue.GetObject_ID()
        {
            return Object_ID;
        }


        GriffinsBaseValue IGriffinsBaseValue.ToBaseValue()
        {
            ObjectValue_Json objectValue_Json = new ObjectValue_Json(Object_ID)
            {
                JsonVal = toJson()
            };
            return GriffinsBaseValue.Create(objectValue_Json);
        }

        void IGriffinsBaseValue.PopulateFromBaseValue(GriffinsBaseValue baseValue)
        {
            if (baseValue != null && baseValue.Val != null)
            {
                if (!(baseValue.Val is ObjectValue_Json jsonVal))
                {
                    throw new Exception("对象值不是 ObjectValue_Json 类型");
                }

                if (jsonVal.Object_ID != Object_ID)
                {
                    throw new Exception("对象值不是 GlueTimeItemCollection 转换的");
                }

                fromJson(jsonVal.JsonVal);
            }
        }

        #endregion

        #region 序列化与反序列化

        private string toJson()
        {
            // 将 ObservableCollection 转换为 List 后再序列化
            return JsonSerializer.Serialize(this.Items.ToList());
        }

        private void fromJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) return;

            try
            {
                // 反序列化为普通 List
                var data = JsonSerializer.Deserialize<List<GlueTimeItem>>(json);
                if (data != null)
                {
                    this.Items.Clear();
                    foreach (var item in data)
                    {
                        this.Items.Add(item);
                    }
                }
            }
            catch (JsonException)
            {
                // 遇到被破坏的 JSON 数据不报错，维持当前界面的默认值或空列表
                System.Diagnostics.Debug.WriteLine("解析 GlueTimeItem 列表 JSON 失败");
            }
        }

        #endregion
    }

}