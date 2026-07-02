using ReactiveUI;
using Griffins.Map;
using Griffins.Map.UI;
using GKG.Map.Page.UIContainer.GridContainer.Models;

namespace GKG.Map.Page.UIContainer.GridContainer.ViewModels
{
    /// <summary>
    /// 网格子页面容器预览视图模型 
    /// (作为整个网格组件的“大脑”，负责接收配置、分发数据，并调度设计时/运行时的命令策略)
    /// </summary>
    public class PreviewViewModel : ReactiveObject, IGridWorkAreaContext
    {
        #region 私有字段

        private byte[]? _cfgInfo;
        private WorkAreaInfoList? _workAreaInfos;

        // 策略执行官：在运行时/设计时装配后，专门负责获取视图数据
        private ICommandExecutionStrategy _commandStrategy = null!;

        #endregion

        #region 公开属性

        /// <summary>
        /// 工作区视图模型 (作为网格的数据总管，负责具体的行列渲染和 UI 绑定)
        /// </summary>
        public WorkAreaViewModel WorkAreaViewModel { get; }

        #endregion

        #region 构造函数

        public PreviewViewModel()
        {
            // 将内部的加载动作委托给 WorkAreaViewModel
            WorkAreaViewModel = new WorkAreaViewModel(OnWorkAreaLoad);
        }

        #endregion

        #region 初始化与配置加载

        /// <summary>
        /// 加载宿主传来的配置信息与工作区列表
        /// </summary>
        /// <param name="workAreaInfos">工作区配置列表</param>
        /// <param name="cfgInfo">序列化后的网格布局属性 (JSON byte 数组)</param>
        public void LoadConfiguration(WorkAreaInfoList workAreaInfos, byte[] cfgInfo)
        {
            ArgumentNullException.ThrowIfNull(workAreaInfos);

            _workAreaInfos = workAreaInfos;
            _cfgInfo = cfgInfo;

            int rows = 1;
            int columns = 1;

            if (cfgInfo?.Length > 0)
            {
                try
                {
                    var templateCfg = new GridContainerTemplateCfgInfo();
                    templateCfg.FromJsonBytes(cfgInfo);

                    rows = templateCfg.GridRows > 0 ? templateCfg.GridRows : rows;
                    columns = templateCfg.GridColumns > 0 ? templateCfg.GridColumns : columns;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"网格布局配置解析失败，将使用默认 {rows}x{columns} 布局: {ex.Message}");
                }
            }

            WorkAreaViewModel.LoadConfiguration(_workAreaInfos, rows, columns);
        }

        #endregion

        #region 策略装配与生命周期

        /// <summary>
        /// 设置设计时命令执行策略 (预览态)
        /// </summary>
        public void SetDesignTimeCommandStrategy()
        {
            _commandStrategy = new DesignTimeCommandStrategy(this);
            WorkAreaViewModel.TriggerAllLoads();
        }

        /// <summary>
        /// 设置运行时命令执行策略 (运行态)
        /// </summary>
        /// <param name="runtimeCallback">宿主提供的运行时回调接口</param>
        public void SetRuntimeCallback(ISubPageContainerRunTimeCallBack runtimeCallback)
        {
            ArgumentNullException.ThrowIfNull(runtimeCallback);

            _commandStrategy = new RuntimeCommandStrategy(this, runtimeCallback);
            WorkAreaViewModel.TriggerAllLoads();
        }

        /// <summary>
        /// 接收宿主指令，激活(聚焦/高亮)指定的子页面
        /// </summary>
        /// <param name="subPageID">目标子页面的全局唯一 ID</param>
        /// <returns>是否成功激活</returns>
        public bool ActivateSubPage(SubPageID subPageID)
        {
            if (subPageID == SubPageID.Empty)
                return true;

            // 将指令传给当前的策略执行官
            return _commandStrategy?.ActivateSubPage(subPageID) ?? false;
        }

        #endregion

        #region IGridWorkAreaContext 接口实现

        /// <summary>
        /// 为策略提供底层数据查询能力，根据 SubID 找回对应的 UI 模型
        /// </summary>
        GridWorkAreaModel? IGridWorkAreaContext.GetModelBySubID(Guid subID)
        {
            return WorkAreaViewModel.WorkAreaItems.FirstOrDefault(m => m.SubID == subID);
        }

        #endregion

        #region 私有辅助方法

        /// <summary>
        /// 当某个网格模块首次需要显示时触发的加载回调
        /// </summary>
        /// <param name="workAreaItem">需要加载内容的网格模型</param>
        private void OnWorkAreaLoad(GridWorkAreaModel workAreaItem)
        {
            _commandStrategy?.LoadWorkAreaContent(workAreaItem);
        }

        #endregion
    }
}