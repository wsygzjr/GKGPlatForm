using Avalonia.Layout;
using PropertyModels.ComponentModel;
using ReactiveUI;

namespace GKG.Map.Page.UIContainer.TabControlContainer.ViewModels
{
    /// <summary>
    /// Tab工作区模型
    /// </summary>
    public class TabWorkAreaModel : ReactiveUI.ReactiveObject
    {
        private string _id = string.Empty;
        /// <summary>
        /// 选项卡唯一标识
        /// </summary>
        public string Id
        {
            get => _id;
            set => this.RaiseAndSetIfChanged(ref _id, value);
        }

        private string _name = string.Empty;
        /// <summary>
        /// 选项卡显示名称
        /// </summary>
        public string Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }

        private object _cachedContent=null!;
        /// <summary>
        /// 选项卡内容缓存
        /// </summary>
        public object CachedContent
        {
            get => _cachedContent;
            set => this.RaiseAndSetIfChanged(ref _cachedContent, value);
        }

        private bool _isSelected;
        /// <summary>
        /// 是否选中状态
        /// </summary>
        public bool IsSelected
        {
            get => _isSelected;
            set => this.RaiseAndSetIfChanged(ref _isSelected, value);
        }

        private HorizontalAlignment _horizontalContentAlignment;
        /// <summary>
        ///工作区内部子内容在自身的水平对齐
        /// </summary>
        public HorizontalAlignment HorizontalContentAlignment
        {
            get => _horizontalContentAlignment;
            set => _horizontalContentAlignment = value;
        }
        private VerticalAlignment _verticalContentAlignment;
        
        /// <summary>
        /// 工作区内部子内容在自身的垂直对齐
        /// </summary>
        public VerticalAlignment VerticalContentAlignment
        {
            get => _verticalContentAlignment;
            set => _verticalContentAlignment = value;
        }
        
        /// <summary>
        /// 验证模型是否有效
        /// </summary>
        /// <returns>有效返回true，否则返回false</returns>
        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Id) && !string.IsNullOrWhiteSpace(Name);
        }
    }
}
