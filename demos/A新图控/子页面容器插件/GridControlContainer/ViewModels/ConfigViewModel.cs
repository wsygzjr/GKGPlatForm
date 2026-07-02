using Avalonia.Controls;
using ReactiveUI;
using System.Reactive;
using Griffins.Map.UI;
using Griffins.Map.UI.CustomMenuCmdConfig;
using Griffins.UI;
using GKG.Map.Page.UIContainer.GridContainer.Models;

namespace GKG.Map.Page.UIContainer.GridContainer.ViewModels
{
    /// <summary>
    /// 配置窗口的视图模型
    /// </summary>
    public class ConfigViewModel : ReactiveObject
    {
        private bool _isLoading = false; // 拦截标志：防止在初始化赋值时触发不需要的集合增删
        private  Window? _ownerConfigWindow;

        /// <summary>
        /// 工作区配置
        /// </summary>
        public WorkAreaConfigViewModel WorkAreaConfigVM { get; } 

        private int _gridRows = 1;
        /// <summary>
        /// 网格行数属性，修改时自动调整工作区列表并触发配置修改事件
        /// </summary>
        public int GridRows
        {
            get => _gridRows;
            set
            {
                this.RaiseAndSetIfChanged(ref _gridRows, value);
                if (!_isLoading)
                {
                    AdjustWorkAreaItems();
                    doAfterModified(this, EventArgs.Empty);
                }
            }
        }

        private int _gridColumns = 1;
        /// <summary>
        /// 网格列数属性，修改时自动调整工作区列表并触发配置修改事件
        /// </summary>
        public int GridColumns
        {
            get => _gridColumns;
            set
            {
                this.RaiseAndSetIfChanged(ref _gridColumns, value);
                if (!_isLoading)
                {
                    AdjustWorkAreaItems();
                    doAfterModified(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// 关闭窗口命令
        /// </summary>
        public ReactiveCommand<Unit, Unit> CloseCommand { get; } = null!;

        /// <summary>
        /// 配置修改事件
        /// </summary>
        public event EventHandler? AfterModified;

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public ConfigViewModel()
        {
            WorkAreaConfigVM = new WorkAreaConfigViewModel();
            WorkAreaConfigVM.CanAddDelete = false;
            WorkAreaConfigVM.AfterModified += doAfterModified;
            WorkAreaConfigVM.WorkAreaModelKindItemSource = getWorkAreaModelKindItemSource();

            CloseCommand = ReactiveCommand.Create(CloseWindow);
        }

        /// <summary>
        /// 设置视图引用
        /// </summary>
        public void SetViewReference(Window ownerWindow)
        {
            _ownerConfigWindow = ownerWindow ?? throw new ArgumentNullException(nameof(ownerWindow), "配置窗口的所有者不能为空");
            WorkAreaConfigVM.SetViewReference(ownerWindow);
        }

        /// <summary>
		/// 注册属性面板的属性改变事件
		/// </summary>
		public void RegisterPropertyGridPropertyChanged()
        {
            WorkAreaConfigVM.RegisterPropertyGridPropertyChanged();
            WorkAreaConfigVM.OnTabSelected();
        }

        /// <summary>
        /// 从配置信息加载到视图模型
        /// </summary>
        /// <param name="source"></param>
        /// <param name="workAreaInfoes"></param>
        public void CopyFrom(GridContainerTemplateCfgInfo source, WorkAreaInfoList workAreaInfoes)
        {
            _isLoading = true;

            try
            {
                // 1. 直接读取新增的行列字段（做个容错，防止旧数据为0导致生成0个格子）
                GridRows = source.GridRows > 0 ? source.GridRows : 1;
                GridColumns = source.GridColumns > 0 ? source.GridColumns : 1;

                // 2. 加载底层数据（原样传入 PageStyleID）
                WorkAreaConfigVM.FillToVM(workAreaInfoes, source.PageStyleID);

                // 3. 最后保底同步一次集合数量，确保数据和界面完全一致
                AdjustWorkAreaItems();
            }
            finally
            {
                _isLoading = false; // 关闭保护锁
            }
        }

        /// <summary>
        /// 从视图模型提取配置信息
        /// </summary>
        /// <param name="cfgInfo"></param>
        /// <param name="workAreaInfoes"></param>
        public void Extract(GridContainerTemplateCfgInfo cfgInfo, WorkAreaInfoList workAreaInfoes)
        {
            // 清空并填充工作区信息（避免替换引用）
            workAreaInfoes.Clear();
            var extractedWorkAreas = WorkAreaConfigVM.Extract(out string pageStyleID);
            foreach (var area in extractedWorkAreas)
            {
                workAreaInfoes.Add(area);
            }
            cfgInfo.PageStyleID = pageStyleID;
            cfgInfo.GridRows = this.GridRows;
            cfgInfo.GridColumns = this.GridColumns;
        }

        /// <summary>
        /// 关闭所属窗口
        /// </summary>
        public void CloseWindow()
        {
            _ownerConfigWindow?.Close();
        }

        /// <summary>
        /// 根据当前的行数和列数，自动调整子 WorkAreaConfigVM 的 ItemsSource
        /// </summary>
        private void AdjustWorkAreaItems()
        {
            if (WorkAreaConfigVM == null || WorkAreaConfigVM.ItemsSource == null) return;

            int targetCount = GridRows * GridColumns;
            var items = WorkAreaConfigVM.ItemsSource;

            // 1. 扩大网格：补充不足的卡片
            while (items.Count < targetCount)
            {
                var newItem = new WorkAreaModel
                {
                    WorkAreaID = Guid.NewGuid().ToString(),
                    WorkAreaName = $"网格工作区 {items.Count + 1}",
                };
                items.Add(newItem);
            }

            // 2. 缩小网格：裁减尾部多余的卡片
            while (items.Count > targetCount)
            {
                items.RemoveAt(items.Count - 1);
            }

            // 3. 修正选中状态
            if (WorkAreaConfigVM.SelectedItem == null || !items.Contains(WorkAreaConfigVM.SelectedItem))
            {
                WorkAreaConfigVM.SelectedItem = items.FirstOrDefault();
            }
        }

        /// <summary>
        /// 获取工作区种类枚举值和名称列表
        /// </summary>
        private List<ComBoxItem> getWorkAreaModelKindItemSource()
        {
            var itemSource = new List<ComBoxItem>();
            itemSource.Add(new ComBoxItem() { Value = WorkAreaModelKind.SubPage, DisplayName = ResourceString.GotoSubPage });
            itemSource.Add(new ComBoxItem() { Value = WorkAreaModelKind.SubPageContainer, DisplayName = ResourceString.GotoSubPageContainer });
            itemSource.Add(new ComBoxItem() { Value = WorkAreaModelKind.Dynamic, DisplayName = ResourceString.Dynamic });
            return itemSource;
        }

        /// <summary>
        /// 触发配置修改事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void doAfterModified(object? sender, EventArgs e)
        {
            AfterModified?.Invoke(sender, e);
        }
    }
}
