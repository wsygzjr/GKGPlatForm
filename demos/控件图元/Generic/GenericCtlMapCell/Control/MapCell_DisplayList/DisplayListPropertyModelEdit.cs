using GKG.Map.MapCell.Generic.Control.MapCell_DisplayList.Objects;
using PropertyModels.ComponentModel;
using System;
using System.ComponentModel;

namespace GKG.Map.MapCell.Generic.Control.MapCell_DisplayList
{
    [Serializable]
    [MapPropertyOrder]
    [CategoryPriority("图元信息", 1)]
    [CategoryPriority("公共", 2)]
	/// <summary>
	/// 显示列表的属性编辑模型。
	/// 
	/// 该模型作为属性面板（PropertyGrid）的数据源：
	/// - CommonInfo：列配置、排序、是否允许全选等
	/// 
	/// 宿主通过 SetPropertyValue 写入属性值，本类负责把反序列化后的对象同步到内部字段，
	/// 并触发 RaisePropertyChanged 以刷新 UI。
	/// </summary>
    public class DisplayListPropertyModelEdit : ControlCellPropertyModelEdit
    {
        private DisplayListCommonInfo _commonInfo = new DisplayListCommonInfo();
        private string _selectedRowKeys = string.Empty;

        [DisplayName("公共设置")]
        [Category("公共")]
        [PropertySortOrder(1)]
        public DisplayListCommonInfo CommonInfo
        {
            get => _commonInfo;
            set => SetProperty(ref _commonInfo, value);
        }

        [Browsable(false)]
        public string SelectedRowKeys
        {
            get => _selectedRowKeys;
            set => SetProperty(ref _selectedRowKeys, value ?? string.Empty);
        }

        public override bool SetPropertyValue(string propertyID, MapCellPropValue propertyVal)
        {
            if (string.Compare(propertyID, nameof(CommonInfo)) == 0)
            {
                var src = propertyVal != null ? DeserializeObject<DisplayListCommonInfo>(propertyVal) : new DisplayListCommonInfo();
                _commonInfo ??= new DisplayListCommonInfo();

                // 逐字段复制，避免直接替换对象引用导致订阅关系丢失
                _commonInfo.Columns = src.Columns;
                _commonInfo.EnableSelectAll = src.EnableSelectAll;
                _commonInfo.SortField = src.SortField;
                _commonInfo.SortDirection = src.SortDirection;
                RaisePropertyChanged(nameof(CommonInfo));
                return true;
            }
            if (string.Compare(propertyID, nameof(SelectedRowKeys)) == 0)
            {
                SelectedRowKeys = propertyVal?.ToString() ?? string.Empty;
                return true;
            }
            return base.SetPropertyValue(propertyID, propertyVal);
        }

        private static T DeserializeObject<T>(MapCellPropValue val) where T : IMPPropObjectValue, new()
        {
            ObjectValue_Json objectValue_Json = val.ToObjectValue_Json();
            GriffinsBaseValue griffinsBaseValue = GriffinsBaseValue.Create(objectValue_Json);
            IMPPropObjectValue obj = new T();
            obj.PopulateFromBaseValue(griffinsBaseValue);
            return (T)obj;
        }

        public void CopyFrom(ControlCellPropertyModelEdit srcModel)
        {
            if (srcModel is not DisplayListPropertyModelEdit src)
                return;

            _commonInfo ??= new DisplayListCommonInfo();

            // 深拷贝核心字段，保证复制后的实例能正确刷新 UI
            _commonInfo.Columns = src.CommonInfo.Columns;
            _commonInfo.EnableSelectAll = src.CommonInfo.EnableSelectAll;
            _commonInfo.SortField = src.CommonInfo.SortField;
            _commonInfo.SortDirection = src.CommonInfo.SortDirection;
            SelectedRowKeys = src.SelectedRowKeys;
            RaisePropertyChanged(nameof(CommonInfo));
        }
    }
}
