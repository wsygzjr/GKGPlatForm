using ReactiveUI;
using System.Collections.ObjectModel;
using Griffins.Map.UI;

namespace GKG.Map.Page.UIContainer.GridContainer.ViewModels
{
    /// <summary>
    /// 网格工作区视图模型 
    /// (作为数据总管，负责维护网格的行列结构与内部所有模块的状态，并直接绑定到 XAML 视图)
    /// </summary>
    public class WorkAreaViewModel : ReactiveObject
    {
        // 外部注入的加载委托，用于解耦策略层的加载逻辑
        private readonly Action<GridWorkAreaModel> _loadWorkAreaContentAction;

        #region 绑定属性

        private ObservableCollection<GridWorkAreaModel> _workAreaItems = new();
        /// <summary>
        /// 网格模块集合 (绑定到 UI 层 ItemsControl 的 ItemsSource)
        /// </summary>
        public ObservableCollection<GridWorkAreaModel> WorkAreaItems
        {
            get => _workAreaItems;
            set => this.RaiseAndSetIfChanged(ref _workAreaItems, value);
        }

        private int _gridRows;
        /// <summary>
        /// 网格全局行数 (绑定到 UniformGrid.Rows)
        /// </summary>
        public int GridRows
        {
            get => _gridRows;
            set => this.RaiseAndSetIfChanged(ref _gridRows, value);
        }

        private int _gridColumns;
        /// <summary>
        /// 网格全局列数 (绑定到 UniformGrid.Columns)
        /// </summary>
        public int GridColumns
        {
            get => _gridColumns;
            set => this.RaiseAndSetIfChanged(ref _gridColumns, value);
        }

        #endregion

        #region 构造与初始化

        /// <summary>
        /// 实例化网格工作区视图模型
        /// </summary>
        /// <param name="onWorkAreaLoad">当网格模块需要加载内容时触发的回调动作</param>
        public WorkAreaViewModel(Action<GridWorkAreaModel> onWorkAreaLoad)
        {
            _loadWorkAreaContentAction = onWorkAreaLoad ?? throw new ArgumentNullException(nameof(onWorkAreaLoad));
        }

        #endregion

        #region 核心业务方法

        /// <summary>
        /// 根据底层数据流加载并构建完整的网格结构
        /// </summary>
        /// <param name="workAreaInfos">底层保存的网格模块配置列表</param>
        /// <param name="rows">系统或用户决定的最终行数</param>
        /// <param name="columns">系统或用户决定的最终列数</param>
        public void LoadConfiguration(WorkAreaInfoList workAreaInfos, int rows, int columns)
        {
            ArgumentNullException.ThrowIfNull(workAreaInfos);

            // 1. 确立全局布局
            GridRows = rows;
            GridColumns = columns;

            // 2. 清理旧数据，准备装填新数据
            WorkAreaItems.Clear();

            // 3. 遍历解析底层配置
            foreach (var info in workAreaInfos)
            {
                // 解析内部独立的样式/对齐配置
                var uiCfgInfo = WorkAreaCfgInfo.FromJSonBytes(info.CfgInfo);

                var gridModel = new GridWorkAreaModel
                {
                    Id = info.WorkAreaID,
                    SubID = info.SubID,
                    Name = info.WorkAreaName,
                    WorkAreaKind = info.WorkAreaKind,
                    IsDefaultSubPage = info.IsDefaultSubPage,

                    HorizontalContentAlignment = uiCfgInfo.HorizontalContentAlignment,
                    VerticalContentAlignment = uiCfgInfo.VerticalContentAlignment,
                };

                WorkAreaItems.Add(gridModel);
            }
        }

        /// <summary>
        /// 并发触发所有空网格的内容加载
        /// <remarks>警告：调用此方法前，必须确保外部的命令策略 (CommandStrategy) 已装配完毕！</remarks>
        /// </summary>
        public void TriggerAllLoads()
        {
            foreach (var item in WorkAreaItems)
            {
                // 仅针对尚无内容的网格发起加载请求，避免重复渲染消耗性能
                if (item.CachedContent == null)
                {
                    _loadWorkAreaContentAction.Invoke(item);
                }
            }
        }

        #endregion
    }
}