using Avalonia.Controls;
using Griffins.Map;
using Griffins.Map.UI;
using Griffins.Map.UI.CustomMenuCmdConfig;
using Griffins.UI;
using Griffins.UI2.PropertyGrid;
using NonMainFrame;
using NonMainFrameViewModel.Models;
using NonMainFrameViewModel.ViewModels.ToolbarMenu;
using NonMainFrameViewModel.ViewModels.TopMenu;
using NonMainFrameViewModel.ViewModels.WorkArea;
using ReactiveUI;
using System.Reactive;

namespace NonMainFrameViewModel.ViewModels
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
        /// 工作区配置
        /// </summary>
        public WorkAreaConfigViewModel WorkAreaConfigVM { get; } 

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
            GeneralConfigViewModel.AfterModified += doAfterModified;

            WorkAreaConfigVM = new WorkAreaConfigViewModel();
            WorkAreaConfigVM.CanAddDelete = false;
            WorkAreaConfigVM.AfterModified += doAfterModified;
            WorkAreaConfigVM.WorkAreaModelKindItemSource=getWorkAreaModelKindItemSource();
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
        }
        public void OnTabSelected(int selectIndex)
        {
            WorkAreaConfigVM.RegisterPropertyGridPropertyChanged();
            if (selectIndex == 1)
            {
                WorkAreaConfigVM.OnTabSelected();
            }
        }

        /// <summary>
        /// 从配置信息加载到视图模型
        /// </summary>
        /// <param name="source"></param>
        /// <param name="workAreaInfoes"></param>
        public void CopyFrom(NonMainPageFrameTemplateCfgInfo source, WorkAreaInfoList workAreaInfoes)
        {
            GeneralConfigViewModel.FillToVM(source.GeneralConfigInfo);
            WorkAreaConfigVM.FillToVM(workAreaInfoes, source.PageStyleID);
        }

        /// <summary>
        /// 从视图模型提取配置信息
        /// </summary>
        /// <param name="cfgInfo"></param>
        /// <param name="workAreaInfoes"></param>
        public void Extract(NonMainPageFrameTemplateCfgInfo cfgInfo, WorkAreaInfoList workAreaInfoes)
        {
            cfgInfo.GeneralConfigInfo = GeneralConfigViewModel.Extract();

            workAreaInfoes.Clear();
            var extractedWorkAreas = WorkAreaConfigVM.Extract(out string pageStyleID);
            foreach (var area in extractedWorkAreas)
            {
                workAreaInfoes.Add(area);
            }
            cfgInfo.PageStyleID = pageStyleID;
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
