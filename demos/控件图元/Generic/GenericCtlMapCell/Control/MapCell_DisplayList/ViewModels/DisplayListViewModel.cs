using GKG.Map.MapCell.Generic.Control.MapCell_DisplayList.Objects;
using PropertyModels.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace GKG.Map.MapCell.Generic.Control.MapCell_DisplayList.ViewModels
{
	/// <summary>
	/// 显示列表的 ViewModel。
	/// 
	/// 数据流：
	/// DisplayListPropertyModelEdit(CommonInfo) -> DisplayListViewModel -> DisplayListView
	/// 
	/// 当前版本为了演示/设计时预览，会在 EnsureTestRows 中根据列配置生成测试数据，
	/// 并支持根据 SortField/SortDirection 做排序。
	/// </summary>
    public class DisplayListViewModel : PropertyModels.ComponentModel.ReactiveObject, IDisposable
    {
        private readonly DisplayListPropertyModelEdit _propertyModel;
        private bool _disposed;

		/// <summary>
		/// 显示列表公共信息（列、排序、全选等）。
		/// 该对象属于 PropertyModel，ViewModel 仅做转发与 UI 刷新触发。
		/// </summary>
        public DisplayListCommonInfo CommonInfo => _propertyModel.CommonInfo;

        private ObservableCollection<DisplayListTestRow> _rows = new ObservableCollection<DisplayListTestRow>();
        public ObservableCollection<DisplayListTestRow> Rows
        {
            get => _rows;
            private set
            {
                _rows = value ?? new ObservableCollection<DisplayListTestRow>();
                RaisePropertyChanged(nameof(Rows));
            }
        }

        public DisplayListViewModel()
            : this(new DisplayListPropertyModelEdit())
        {
        }

        public DisplayListViewModel(DisplayListPropertyModelEdit propertyModel)
        {
            _propertyModel = propertyModel;

            // 监听属性模型变化，转发通知给 View。
            _propertyModel.PropertyChanged += PropertyModel_PropertyChanged;
            _propertyModel.CommonInfo.PropertyChanged += ChildPropertyChanged;
        }

        public void ReloadFromModel()
        {
            RaisePropertyChanged(nameof(CommonInfo));
            RaisePropertyChanged(nameof(Rows));
        }

        public void EnsureTestRows()
        {
            // 根据列配置得到字段 key 列表。
            // - 有 FieldID 时优先用 FieldID
            // - 否则回退到 DisplayName
            var cols = CommonInfo?.Columns;
            var keys = (cols == null || cols.Count == 0)
                ? new List<string> { "Name", "Age", "Gender" }
                : cols.Select(c => string.IsNullOrWhiteSpace(c.FieldID) ? (c.DisplayName ?? string.Empty) : c.FieldID)
                      .Where(s => !string.IsNullOrWhiteSpace(s))
                      .Distinct()
                      .ToList();

            if (keys.Count == 0)
                keys.AddRange(new[] { "Name", "Age", "Gender" });

            if (_rows != null && _rows.Count > 0)
            {
                // 列结构未变化时，不重新生成，避免丢失勾选状态。
                var existingKeys = _rows[0].Values.Keys.ToHashSet(StringComparer.OrdinalIgnoreCase);
                var desiredKeys = keys.ToHashSet(StringComparer.OrdinalIgnoreCase);
                if (existingKeys.SetEquals(desiredKeys))
                    return;
            }

            var seed = new (string Name, int Age, string Gender)[]
            {
                ("张三", 23, "男"),
                ("李四", 41, "男"),
                ("王五", 35, "女"),
                ("赵六", 18, "女"),
                ("周七", 27, "男"),
                ("吴八", 52, "女"),
                ("郑九", 30, "男"),
                ("孙十", 45, "女"),
            };

            var list = new ObservableCollection<DisplayListTestRow>();
            for (var i = 0; i < seed.Length; i++)
            {
                var row = new DisplayListTestRow();
                foreach (var key in keys)
                {
                    // 针对常用字段做简单的填充，其他字段留空。
                    var lower = key.Trim().ToLowerInvariant();
                    if (lower.Contains("age") || key.Contains("年龄"))
                        row.Values[key] = seed[i].Age.ToString(CultureInfo.InvariantCulture);
                    else if (lower.Contains("gender") || lower.Contains("sex") || key.Contains("性别"))
                        row.Values[key] = seed[i].Gender;
                    else if (lower.Contains("name") || key.Contains("姓名"))
                        row.Values[key] = seed[i].Name;
                    else
                        row.Values[key] = string.Empty;
                }
                list.Add(row);
            }

            Rows = list;
            ApplySort();
        }

        public void ApplySort()
        {
            var field = CommonInfo?.SortField;
            if (string.IsNullOrWhiteSpace(field) || _rows == null || _rows.Count <= 1)
                return;

            var cols = CommonInfo?.Columns;
            if (cols != null)
            {
                // SortField 在属性面板可能选择的是列显示名；这里尝试映射到 FieldID。
                var byDisplay = cols.FirstOrDefault(c => string.Equals(c.DisplayName, field, StringComparison.OrdinalIgnoreCase));
                if (byDisplay != null && !string.IsNullOrWhiteSpace(byDisplay.FieldID))
                    field = byDisplay.FieldID;
            }

            var dir = CommonInfo?.SortDirection ?? DisplayListSortDirection.Asc;

            IEnumerable<DisplayListTestRow> sorted;
            if (_rows.All(r => double.TryParse(r.Get(field), NumberStyles.Any, CultureInfo.InvariantCulture, out _)))
            {
                // 全部可解析为数字时，用数值排序
                sorted = dir == DisplayListSortDirection.Asc
                    ? _rows.OrderBy(r => double.Parse(r.Get(field), CultureInfo.InvariantCulture))
                    : _rows.OrderByDescending(r => double.Parse(r.Get(field), CultureInfo.InvariantCulture));
            }
            else
            {
                // 否则用字符串排序（忽略大小写）
                sorted = dir == DisplayListSortDirection.Asc
                    ? _rows.OrderBy(r => r.Get(field), StringComparer.OrdinalIgnoreCase)
                    : _rows.OrderByDescending(r => r.Get(field), StringComparer.OrdinalIgnoreCase);
            }

            Rows = new ObservableCollection<DisplayListTestRow>(sorted);
        }

        private void PropertyModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
        }

        private void ChildPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // CommonInfo 任意字段变化都可能影响 UI（列、排序、全选），统一通知 CommonInfo 刷新。
            if (sender is DisplayListCommonInfo)
                RaisePropertyChanged(nameof(CommonInfo));
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _propertyModel.PropertyChanged -= PropertyModel_PropertyChanged;
                _propertyModel.CommonInfo.PropertyChanged -= ChildPropertyChanged;
            }

            _disposed = true;
        }
    }

    public class DisplayListTestRow : PropertyModels.ComponentModel.ReactiveObject
    {
        private bool _isSelected;
        private bool _isCurrent;

        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        public bool IsCurrent
        {
            get => _isCurrent;
            set
            {
                if (SetProperty(ref _isCurrent, value))
                    RaisePropertyChanged(nameof(IsHighlighted));
            }
        }

        public bool IsHighlighted => IsCurrent;

        public Dictionary<string, string> Values { get; } = new Dictionary<string, string>();

		/// <summary>
		/// 根据字段名获取单元格文本。
		/// </summary>
        public string Get(string field)
        {
            if (string.IsNullOrWhiteSpace(field))
                return string.Empty;
            return Values.TryGetValue(field, out var v) ? (v ?? string.Empty) : string.Empty;
        }
    }
}
