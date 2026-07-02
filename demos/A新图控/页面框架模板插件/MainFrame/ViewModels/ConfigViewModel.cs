using Avalonia.Controls;
using Griffins.Map.UI;
using Griffins.Map.UI.CustomMenuCmdConfig;
using Griffins.UI;
using Griffins.UI2.PropertyGrid;
using MainFrame.Models;
using ReactiveUI;
using System.Reactive;

namespace MainFrame.ViewModels
{
    /// <summary>
    /// 配置窗口的视图模型
    /// </summary>
    public class ConfigViewModel : ReactiveObject
    {
        private  Window? _ownerConfigWindow;
        /// <summary>
        /// 通用配置视图模型
        /// </summary>
        public GeneralConfigViewModel GeneralConfigViewModel { get; }
        /// <summary>
        /// 中间工作区配置
        /// </summary>
        public WorkAreaConfigViewModel WorkAreaConfigVM { get; }
        /// <summary>
        /// 左侧导航栏配置
        /// </summary>
        public WorkAreaConfigViewModel NavigationMenuConfigVM { get; } 

        /// <summary>
        /// 右侧工具栏配置
        /// </summary>
        public WorkAreaConfigViewModel ToolbarButtonsConfigVM { get; }

        /// <summary>
        /// 顶部菜单栏配置
        /// </summary>
        public WorkAreaConfigViewModel TopMenuConfigVM { get; }

        /// <summary>
        /// 底部信息栏配置
        /// </summary>
        public WorkAreaConfigViewModel ButtomColumnConfigVM { get; }

        /// <summary>
        /// 左下信息块配置
        /// </summary>
        public WorkAreaConfigViewModel LeftBottomInfoBlockConfigVM { get; }

        /// <summary>
        /// 关闭窗口命令
        /// </summary>
        public ReactiveCommand<Unit, Unit> CloseCommand { get; } = null!;

        /// <summary>
        /// Tab页切换命令
        /// </summary>
		public ReactiveCommand<int, Unit> TabSelectionChangedCommand { get; }

		/// <summary>
		/// 配置修改事件
		/// </summary>
		public event EventHandler? AfterModified;

        /// <summary>
        /// 构造函数
        /// </summary>
        public ConfigViewModel()
        {
            GeneralConfigViewModel = new GeneralConfigViewModel();

            WorkAreaConfigVM = new WorkAreaConfigViewModel();
            WorkAreaConfigVM.CanAddDelete = false;
            WorkAreaConfigVM.AfterModified += doAfterModified;
            WorkAreaConfigVM.WorkAreaModelKindItemSource = getWorkAreaModelKindItemSource();

            NavigationMenuConfigVM = new WorkAreaConfigViewModel();
            NavigationMenuConfigVM.CanAddDelete = false;
            NavigationMenuConfigVM.WorkAreaModelKindItemSource = getWorkAreaModelKindItemSource1();

            ToolbarButtonsConfigVM = new WorkAreaConfigViewModel();
            ToolbarButtonsConfigVM.CanAddDelete = false;
            ToolbarButtonsConfigVM.WorkAreaModelKindItemSource = getWorkAreaModelKindItemSource1();

            TopMenuConfigVM =new WorkAreaConfigViewModel();
            TopMenuConfigVM.CanAddDelete = false;
            TopMenuConfigVM.WorkAreaModelKindItemSource = getWorkAreaModelKindItemSource1();

            ButtomColumnConfigVM = new WorkAreaConfigViewModel();
            ButtomColumnConfigVM.CanAddDelete = false;
            ButtomColumnConfigVM.WorkAreaModelKindItemSource = getWorkAreaModelKindItemSource1();

            LeftBottomInfoBlockConfigVM = new WorkAreaConfigViewModel();
            LeftBottomInfoBlockConfigVM.CanAddDelete = false;
            LeftBottomInfoBlockConfigVM.WorkAreaModelKindItemSource = getWorkAreaModelKindItemSource1();

            GeneralConfigViewModel.AfterModified += doAfterModified;
            NavigationMenuConfigVM.AfterModified += doAfterModified;
            ToolbarButtonsConfigVM.AfterModified += doAfterModified;
            TopMenuConfigVM.AfterModified += doAfterModified;
            ButtomColumnConfigVM.AfterModified += doAfterModified;
            LeftBottomInfoBlockConfigVM.AfterModified += doAfterModified;

            // 初始化命令并设置可执行条件
            CloseCommand = ReactiveCommand.Create(CloseWindow);
			TabSelectionChangedCommand = ReactiveCommand.Create<int>(OnTabSelected);
        }

        /// <summary>
        /// 设置视图引用
        /// </summary>
        public void SetViewReference(Window ownerWindow)
        {
            _ownerConfigWindow = ownerWindow ?? throw new ArgumentNullException(nameof(ownerWindow), "配置窗口的所有者不能为空");
            GeneralConfigViewModel.SetViewReference(ownerWindow);
            WorkAreaConfigVM.SetViewReference(ownerWindow);
            NavigationMenuConfigVM.SetViewReference(ownerWindow);
            ToolbarButtonsConfigVM.SetViewReference(ownerWindow);
            TopMenuConfigVM.SetViewReference(ownerWindow);
            ButtomColumnConfigVM.SetViewReference(ownerWindow);
            LeftBottomInfoBlockConfigVM.SetViewReference(ownerWindow);
        }

        public void OnTabSelected(int selectIndex)
		{
            // selectIndex==0为常规选项卡
            // 中间工作区
            if (selectIndex == 1)
            {
                WorkAreaConfigVM.RegisterPropertyGridPropertyChanged();
                WorkAreaConfigVM.OnTabSelected();
            }
            // 左侧导航栏
            else if (selectIndex==2)
			{
                NavigationMenuConfigVM.RegisterPropertyGridPropertyChanged();
                NavigationMenuConfigVM.OnTabSelected();
			}
            // 顶部菜单栏
            else if (selectIndex == 3)
			{
                TopMenuConfigVM.RegisterPropertyGridPropertyChanged();
                TopMenuConfigVM.OnTabSelected();
			}
            // 右侧工具栏
			else if (selectIndex == 4)
			{
                ToolbarButtonsConfigVM.RegisterPropertyGridPropertyChanged();
                ToolbarButtonsConfigVM.OnTabSelected();
			}
            // 底部信息栏
            else if (selectIndex == 5)
            {
                ButtomColumnConfigVM.RegisterPropertyGridPropertyChanged();
                ButtomColumnConfigVM.OnTabSelected();
            }
            // 左下信息块
            else if (selectIndex == 6)
            {
                LeftBottomInfoBlockConfigVM.RegisterPropertyGridPropertyChanged();
                LeftBottomInfoBlockConfigVM.OnTabSelected();
            }
        }

        /// <summary>
        /// 从配置信息加载到视图模型
        /// </summary>
        /// <param name="source"></param>
        /// <param name="workAreaInfoes"></param>
        public void CopyFrom(MainPageFrameTemplateCfgInfo source, WorkAreaInfoList workAreaInfoes)
        {
            GeneralConfigViewModel.FillToVM(source.GeneralConfigInfo);
            WorkAreaConfigVM.FillToVM(workAreaInfoes, source.PageStyleID);
            NavigationMenuConfigVM.FillToVM(new WorkAreaInfoList() { source.NavigationMenu }, "");
            TopMenuConfigVM.FillToVM(new WorkAreaInfoList() { source.TopMenuCfgInfo },"");
            ToolbarButtonsConfigVM.FillToVM(new WorkAreaInfoList() { source.ToolbarButton }, "");
            ButtomColumnConfigVM.FillToVM(new WorkAreaInfoList() { source.ButtomColumn }, "");
            LeftBottomInfoBlockConfigVM.FillToVM(new WorkAreaInfoList() { source.LeftBottomInfoBlock }, "");
        }

        /// <summary>
        /// 从视图模型提取配置信息
        /// </summary>
        /// <param name="cfgInfo"></param>
        /// <param name="workAreaInfoes"></param>
        public void Extract(MainPageFrameTemplateCfgInfo cfgInfo, WorkAreaInfoList workAreaInfoes)
        {
            cfgInfo.GeneralConfigInfo = GeneralConfigViewModel.Extract();

            workAreaInfoes.Clear();
            var extractedWorkAreas = WorkAreaConfigVM.Extract(out string pageStyleID);
            foreach (var area in extractedWorkAreas)
            {
                workAreaInfoes.Add(area);
            }
            cfgInfo.PageStyleID = pageStyleID;

            cfgInfo.NavigationMenu = NavigationMenuConfigVM.Extract(out string pageTypeId0).FirstOrDefault()!;
            cfgInfo.TopMenuCfgInfo = TopMenuConfigVM.Extract(out string pageTypeId).FirstOrDefault()!;
            cfgInfo.ToolbarButton = ToolbarButtonsConfigVM.Extract(out string pageTypeId1).FirstOrDefault()!;
            cfgInfo.ButtomColumn = ButtomColumnConfigVM.Extract(out string pageTypeId2).FirstOrDefault()!;
            cfgInfo.LeftBottomInfoBlock = LeftBottomInfoBlockConfigVM.Extract(out string pageTypeId3).FirstOrDefault()!;
        }

        /// <summary>
        /// 关闭所属窗口
        /// </summary>
        public void CloseWindow()
        {
            _ownerConfigWindow?.Close();
        }

        /// <summary>
        /// 获取工作区种类枚举值和名称列表
        /// </summary>
        private List<ComBoxItem> getWorkAreaModelKindItemSource()
        {
            var itemSource = new List<ComBoxItem>();
            itemSource.Add(new ComBoxItem() { Value = WorkAreaModelKind.Dynamic, DisplayName = ResourceString.Dynamic });
            return itemSource;
        }

        /// <summary>
        /// 获取工作区种类枚举值和名称列表
        /// </summary>
        private List<ComBoxItem> getWorkAreaModelKindItemSource1()
        {
            var itemSource = new List<ComBoxItem>();
            itemSource.Add(new ComBoxItem() { Value = WorkAreaModelKind.SubPage, DisplayName = ResourceString.GotoSubPage });
            itemSource.Add(new ComBoxItem() { Value = WorkAreaModelKind.SubPageContainer, DisplayName = ResourceString.GotoSubPageContainer });
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
