using Avalonia.Layout;
using Griffins.Map.UI;
using ReactiveUI;
using System;

namespace GKG.Map.Page.UIContainer.GridContainer.ViewModels
{
    /// <summary>
    /// 网格工作区数据模型 (代表 Grid 布局中的一个独立单元格/模块)
    /// </summary>
    public class GridWorkAreaModel : ReactiveObject
    {
        private string _id = string.Empty;
        /// <summary>
        /// 网格模块唯一标识 
        /// <!--对工作区配置信息中的 WorkAreaID -->
        /// </summary>
        public string Id
        {
            get => _id;
            set => this.RaiseAndSetIfChanged(ref _id, value);
        }

        private string _name = string.Empty;
        /// <summary>
        /// 网格模块名称 
        /// <!--例如："网格模块 1"，通常用于在配置列表中展示 -->
        /// </summary>
        public string Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }

        private Guid _subID;
        /// <summary>
        /// 网格模块内放置的子页面实例ID或子页面容器实例ID
        /// <!-- 在WorkAreaKind为Dynamic时，选择的默认的子页面实例ID或子页面容器实例ID -->
        /// </summary>
        public Guid SubID
        {
            get => _subID;
            set => this.RaiseAndSetIfChanged(ref _subID, value);
        }

        private WorkAreaKind _workAreaKind;
        /// <summary>
        /// 网格模块内放置的内容类型
        /// </summary>
        public WorkAreaKind WorkAreaKind
        {
            get => _workAreaKind;
            set => this.RaiseAndSetIfChanged(ref _workAreaKind, value);
        }

        private bool _isDefaultSubPage;
        /// <summary>
        /// 是否为加载默认的子页面
        /// <!--工作区种类为动态加载时有效 true:子页面 false:子页面容器 -->
        /// </summary>
        public bool IsDefaultSubPage
        {
            get => _isDefaultSubPage;
            set => this.RaiseAndSetIfChanged(ref _isDefaultSubPage, value);
        }

        private object _cachedContent = null!;
        /// <summary>
        /// 网格内实际呈现的视图内容 (UI 控件或 View)
        /// </summary>
        public object CachedContent
        {
            get => _cachedContent;
            set => this.RaiseAndSetIfChanged(ref _cachedContent, value);
        }

        private HorizontalAlignment _horizontalContentAlignment;
        /// <summary>
        /// 网格工作区内部子内容在水平方向的对齐方式
        /// </summary>
        public HorizontalAlignment HorizontalContentAlignment
        {
            get => _horizontalContentAlignment;
            set => this.RaiseAndSetIfChanged(ref _horizontalContentAlignment, value);
        }

        private VerticalAlignment _verticalContentAlignment;
        /// <summary>
        /// 网格工作区内部子内容在垂直方向的对齐方式
        /// </summary>
        public VerticalAlignment VerticalContentAlignment
        {
            get => _verticalContentAlignment;
            set => this.RaiseAndSetIfChanged(ref _verticalContentAlignment, value);
        }

        /// <summary>
        /// 验证模型数据是否完整有效
        /// </summary>
        /// <returns>有效返回true，否则返回false</returns>
        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Id) && !string.IsNullOrWhiteSpace(Name);
        }
    }
}