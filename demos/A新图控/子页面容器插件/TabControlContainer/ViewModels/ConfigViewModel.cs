using Avalonia.Controls;
using GKG.Map.Page.UIContainer.TabControlContainer.Models;
using Griffins.Map.UI;
using Griffins.Map.UI.CustomMenuCmdConfig;
using Griffins.UI;
using ReactiveUI;
using System.Reactive;

namespace GKG.Map.Page.UIContainer.TabControlContainer.ViewModels
{
    /// <summary>
    /// 配置窗口的视图模型
    /// </summary>
    public class ConfigViewModel : ReactiveObject
    {
        private  Window? _ownerConfigWindow;


        /// <summary>
        /// 工作区配置
        /// </summary>
        public WorkAreaConfigViewModel WorkAreaConfigVM { get; } 

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
            WorkAreaConfigVM.CanAddDelete = true;
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
        public void CopyFrom(TabControlContainerTemplateCfgInfo source, WorkAreaInfoList workAreaInfoes)
        {
            WorkAreaConfigVM.FillToVM(workAreaInfoes, source.PageStyleID);
        }

        /// <summary>
        /// 从视图模型提取配置信息
        /// </summary>
        /// <param name="cfgInfo"></param>
        /// <param name="workAreaInfoes"></param>
        public void Extract(TabControlContainerTemplateCfgInfo cfgInfo, WorkAreaInfoList workAreaInfoes)
        {
            // 清空并填充工作区信息（避免替换引用）
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
