using System;
using System.ComponentModel;
using GF_Gereric;
using GKG.Map.MapCell.Generic.Control.MapCell_ComboBox.Models;
using PropertyModels.ComponentModel;

namespace GKG.Map.MapCell.Generic.Control.MapCell_ComboBox
{
    [Serializable]
    [MapPropertyOrder]
    [CategoryPriority("点位信息", 1)]
    [CategoryPriority("绑定信息", 2)]
    public class ComboBoxPropertyBindEditModel : ControlCellPropertyBindEditModel
    {
        private PropertyBindInfo _items = new PropertyBindInfo(GriffinsBaseDataType.Object_Json, RunModeList.Object_ID);
        private PropertyBindInfo _selectedItem = new PropertyBindInfo(GriffinsBaseDataType.String);

        [DisplayName("下拉项集合")]
        [Category("绑定信息")]
        [PropertySortOrder(6)]
        [BindMPPropertyID]
        public PropertyBindInfo Items
        {
            get => _items;
            set
            {
                var clone = new PropertyBindInfo(GriffinsBaseDataType.Object_Json, RunModeList.Object_ID);
                clone.CopyFrom(value);
                SetProperty(ref _items, clone);
            }
        }

        [DisplayName("当前选中项")]
        [Category("绑定信息")]
        [PropertySortOrder(7)]
        [BindMPPropertyID]
        public PropertyBindInfo SelectedItem
        {
            get => _selectedItem;
            set
            {
                var clone = new PropertyBindInfo(GriffinsBaseDataType.String);
                clone.CopyFrom(value);
                SetProperty(ref _selectedItem, clone);
            }
        }

        public void CopyFrom(ComboBoxPropertyBindEditModel source)
        {
            if (source == null) return;
            base.CopyFrom(source);
            Items = source.Items;
            SelectedItem = source.SelectedItem;
        }
    }
}