using GKG.Map.ProductionInformationFuncCtlMapCell.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Reactive;

namespace GKG.Map.ProductionInformationFuncCtlMapCell.ViewModel
{
    /// <summary>
    /// 通用消息弹窗视图模型 (ViewModel)
    /// 负责管理系统级弹窗的显示内容、图标状态及按钮交互信号。
    /// 作为纯状态容器，具体的弹窗阻断与关闭逻辑交由 View 层的后置代码或 Interaction 管线处理。
    /// </summary>
    public class MessageDialogViewModel : ReactiveObject
    {
        #region 数据绑定属性

        /// <summary>
        /// 弹窗触发的时间戳
        /// </summary>
        [Reactive]
        public DateTime CurrentTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 弹窗主标题
        /// </summary>
        [Reactive]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// 弹窗图标类型 (供前端 Converter 动态映射对应的矢量图标或主题色)
        /// </summary>
        [Reactive]
        public MessageDialogIconType IconType { get; set; }

        /// <summary>
        /// 具体的业务提示或异常消息内容
        /// </summary>
        [Reactive]
        public string Message { get; set; } = string.Empty;

        #endregion

        #region 按钮文案与可见性状态

        /// <summary>
        /// 获取或设置 [Yes] 按钮的显示文本
        /// </summary>
        [Reactive] public string ButtonContentYes { get; set; } = "YES";

        /// <summary>
        /// 获取或设置 [Ok] 按钮的显示文本
        /// </summary>
        [Reactive] public string ButtonContentOk { get; set; } = "OK";

        /// <summary>
        /// 获取或设置 [No] 按钮的显示文本
        /// </summary>
        [Reactive] public string ButtonContentNo { get; set; } = "NO";

        /// <summary>
        /// 获取或设置 [Yes] 按钮是否在界面上渲染显示
        /// </summary>
        [Reactive] public bool IsYesVisible { get; set; }

        /// <summary>
        /// 获取或设置 [Ok] 按钮是否在界面上渲染显示
        /// </summary>
        [Reactive] public bool IsOkVisible { get; set; }

        /// <summary>
        /// 获取或设置 [No] 按钮是否在界面上渲染显示
        /// </summary>
        [Reactive] public bool IsNoVisible { get; set; }

        #endregion

        #region 交互命令信号

        /// <summary>
        /// 确认/是 [Yes] 操作触发指令
        /// </summary>
        public ReactiveCommand<Unit, Unit> YesCommand { get; }

        /// <summary>
        /// 确定 [Ok] 操作触发指令
        /// </summary>
        public ReactiveCommand<Unit, Unit> OkCommand { get; }

        /// <summary>
        /// 取消/否 [No] 操作触发指令
        /// </summary>
        public ReactiveCommand<Unit, Unit> NoCommand { get; }

        #endregion

        /// <summary>
        /// 实例化通用消息弹窗视图模型
        /// </summary>
        public MessageDialogViewModel()
        {
            // 初始化纯交互信号命令。
            // 当命令触发时，仅作为意图信号向外广播，由绑定的 Window 或 Interaction 拦截并决定如何销毁弹窗实例。
            YesCommand = ReactiveCommand.Create(() => { });
            OkCommand = ReactiveCommand.Create(() => { });
            NoCommand = ReactiveCommand.Create(() => { });
        }
    }
}