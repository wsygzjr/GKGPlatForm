using Avalonia.Controls;
using Griffins.Map.UI;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace GKG.Map.Page.UIContainer.TabControlContainer.ViewModels
{
    /// <summary>
    /// 工作区选项卡视图模型
    /// </summary>
    public class WorkAreaViewModel : ReactiveObject, IDisposable
    {
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        private WorkAreaInfoList? _workAreaInfoes;

        private ObservableCollection<TabWorkAreaModel> _workAreaInfoItems = new ObservableCollection<TabWorkAreaModel>();
        /// <summary>
        /// 选项卡集合
        /// </summary>
        public ObservableCollection<TabWorkAreaModel> WorkAreaInfoItems
        {
            get => _workAreaInfoItems;
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(WorkAreaInfoItems));
                this.RaiseAndSetIfChanged(ref _workAreaInfoItems, value);
                reSubscribeToSelections();
            }
        }

        private TabWorkAreaModel? _selectedWorkArea;
        /// <summary>
        /// 当前选中的选项卡
        /// </summary>
        public TabWorkAreaModel? SelectedWorkArea
        {
            get => _selectedWorkArea;
            set
            {
                if (_selectedWorkArea != value)
                {
                    this.RaiseAndSetIfChanged(ref _selectedWorkArea, value);
                    if (value != null) OnTabSelected(value);
                }
            }
        }

        /// <summary>
        /// 选项卡改变委托
        /// </summary>
        private readonly Action<TabWorkAreaModel> _onTabChanged;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="onTabChanged">选项卡改变时的处理委托</param>
        public WorkAreaViewModel(Action<TabWorkAreaModel> onTabChanged)
        {
            _onTabChanged = onTabChanged ?? throw new ArgumentNullException(nameof(onTabChanged));

        }

        /// <summary>
        /// 改变指定选项卡的内容
        /// </summary>
        /// <param name="content">要显示的内容</param>
        /// <param name="subID">子页面ID或容器ID</param>
        public void ChangeCurrentContent(object content, Guid subID)
        {
            if (content == null) throw new ArgumentNullException(nameof(content));
            if (_workAreaInfoes == null)
                return;

            // 查找对应的选项卡
            var workAreaInfo = _workAreaInfoes.FirstOrDefault(o => o.SubID == subID);
            var workAreaItem = workAreaInfo != null
                ? WorkAreaInfoItems.FirstOrDefault(o => o.Id == workAreaInfo.WorkAreaID)
                : null;
		
			if (workAreaItem != null)
            {
				updateSelection(workAreaItem.Id);
				workAreaItem.CachedContent = content;
            }
        }

        /// <summary>
        /// 加载配置信息并初始化选项卡
        /// </summary>
        /// <param name="workAreaInfoes">工作区配置列表</param>
        public void LoadConfiguration(WorkAreaInfoList workAreaInfoes)
        {
            if (workAreaInfoes == null) throw new ArgumentNullException(nameof(workAreaInfoes));

            _workAreaInfoes = workAreaInfoes;
            WorkAreaInfoItems.Clear();

            // 添加选项卡
            foreach (var workAreaInfo in workAreaInfoes)
            {
                var workAreaCfgInfo = WorkAreaCfgInfo.FromJSonBytes(workAreaInfo.CfgInfo);
                var tabWorkAreaModel = new TabWorkAreaModel
                {
                    Id = workAreaInfo.WorkAreaID,
                    Name = workAreaInfo.WorkAreaName,
                    IsSelected = false,
                    HorizontalContentAlignment = workAreaCfgInfo.HorizontalContentAlignment,
                    VerticalContentAlignment = workAreaCfgInfo.VerticalContentAlignment,
                };
               

                WorkAreaInfoItems.Add(tabWorkAreaModel);
            }

            reSubscribeToSelections();
        }
        
        /// <summary>
        /// 默认选中第一个选项卡
        /// </summary>
        public void SetDefaultTab()
        {
            // 选中第一个选项卡
            var firstTab = WorkAreaInfoItems.FirstOrDefault();
            if (firstTab != null)
            {
                SelectedWorkArea = firstTab;
            }
        }

        /// <summary>
        /// 处理选项卡选中事件
        /// </summary>
        private void OnTabSelected(TabWorkAreaModel tabInfo)
        {
            if(tabInfo == null) return;

            // 更新选中状态
            updateSelection(tabInfo.Id);

            // 加载内容
            if (tabInfo.CachedContent == null)
            {
                try
                {
                    _onTabChanged(tabInfo);
                }
                catch (Exception ex)
                {
                    tabInfo.CachedContent = GetDefaultContent($"加载失败: {ex.Message}");
                }
            }
            try
            {
                //ContentControl parentCtrl = null;
                //if ((tabInfo.CachedContent as Control).Parent != null)
                //{
                //    parentCtrl = (ContentControl)(tabInfo.CachedContent as Control).Parent;
                //    parentCtrl.Content = null;
                //}
                //if(parentCtrl!=null)
                //{
                //    parentCtrl.Content = tabInfo.CachedContent!;
                //}
                ////CurrentContent = null;


            }
            catch (Exception ex)
            {

                throw;
            }

        }

        /// <summary>
        /// 更新选项卡选中状态
        /// </summary>
        private void updateSelection(string selectedTabId)
        {
            foreach (var tab in WorkAreaInfoItems)
            {
                tab.IsSelected = tab.Id == selectedTabId;
            }
        }

        /// <summary>
        /// 重新订阅选项卡选中状态变化
        /// </summary>
        private void reSubscribeToSelections()
        {
            // 订阅所有选项卡的选中状态
            foreach (var item in WorkAreaInfoItems)
            {
                item.WhenAnyValue(x => x.IsSelected)
                    .Where(isSelected => isSelected)
                    .Subscribe(_ => SelectedWorkArea = item)
                    .DisposeWith(_disposables);
            }
        }

        /// <summary>
        /// 创建默认内容控件
        /// </summary>
        private TextBlock GetDefaultContent(string message)
        {
            return new TextBlock
            {
                Text = message,
                Foreground = Avalonia.Media.Brushes.Gray,
                Margin = new Avalonia.Thickness(10)
            };
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}
