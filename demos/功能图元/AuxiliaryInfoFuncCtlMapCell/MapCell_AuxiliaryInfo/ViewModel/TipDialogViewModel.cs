using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Reactive;

namespace GKG.Map.AuxiliaryInfoFuncCtlMapCell.ViewModel
{
    #region 核心枚举定义

    /// <summary>
    /// 对话框图标类型 (View 层可根据此枚举通过 Converter 绑定不同的 SVG Path 和颜色)
    /// </summary>
    public enum DialogIconType
    {
        Tip,
        Warning,
        Alarm,
        Question,
        Help
    }

    /// <summary>
    /// 按钮组合类型
    /// </summary>
    public enum TipButtonCombo
    {
        Ok,
        YesNo,
        OkCancel,
        YesNoSkip,
        OkCancelSkip,
        RetryCancel
    }

    /// <summary>
    /// 弹窗最终的返回值类型
    /// </summary>
    public enum DialogResultType
    {
        None,
        Yes,
        No,
        Ok,
        Cancel,
        Skip,
        Retry
    }

    #endregion

    /// <summary>
    /// 现代化提示弹窗 ViewModel
    /// </summary>
    public class TipDialogViewModel : ReactiveObject
    {
        #region 基础显示属性

        [Reactive] public string Title { get; set; }
        [Reactive] public string TipTime { get; set; }
        [Reactive] public string Message { get; set; }
        [Reactive] public string Message2 { get; set; } // 用于突出显示的红色文字

        [Reactive] public DialogIconType IconType { get; set; }

        #endregion

        #region 泛化按钮组 (最多支持3个按钮)

        // 按钮 1 (通常位于左侧，如 YES, OK, Retry)
        [Reactive] public string Btn1Text { get; set; }
        [Reactive] public bool IsBtn1Visible { get; set; }
        public ReactiveCommand<Unit, Unit> Btn1Command { get; }
        private DialogResultType _btn1Result;

        // 按钮 2 (通常位于中间，如 Skip)
        [Reactive] public string Btn2Text { get; set; }
        [Reactive] public bool IsBtn2Visible { get; set; }
        public ReactiveCommand<Unit, Unit> Btn2Command { get; }
        private DialogResultType _btn2Result;

        // 按钮 3 (通常位于右侧，如 NO, Cancel)
        [Reactive] public string Btn3Text { get; set; }
        [Reactive] public bool IsBtn3Visible { get; set; }
        public ReactiveCommand<Unit, Unit> Btn3Command { get; }
        private DialogResultType _btn3Result;

        #endregion

        #region 结果与关闭交互

        /// <summary>
        /// 记录用户最终选择的结果
        /// </summary>
        public DialogResultType Result { get; private set; } = DialogResultType.None;

        /// <summary>
        /// 提供给 View 层绑定的关闭委托。
        /// VM 不直接操作 Window.Close()，而是触发委托让 View 自己关。
        /// </summary>
        public Action<DialogResultType> RequestClose { get; set; }

        #endregion

        #region 构造函数

        public TipDialogViewModel()
        {
            // 初始化时间戳
            TipTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            // 初始化命令：点击按钮时，记录结果并请求关闭弹窗
            Btn1Command = ReactiveCommand.Create(() => CloseDialog(_btn1Result));
            Btn2Command = ReactiveCommand.Create(() => CloseDialog(_btn2Result));
            Btn3Command = ReactiveCommand.Create(() => CloseDialog(_btn3Result));
        }

        #endregion

        #region 核心配置方法

        /// <summary>
        /// 配置并显示对话框参数 (替代了原来又长又臭的 ShowMessageBox 方法)
        /// </summary>
        public void Setup(
            string message,
            string title = "提示",
            DialogIconType iconType = DialogIconType.Tip,
            TipButtonCombo comboType = TipButtonCombo.Ok,
            string message2 = "")
        {
            Message = message;
            Message2 = message2;
            Title = title;
            IconType = iconType;

            // 根据组合模式配置按钮状态
            ConfigureButtons(comboType);
        }

        private void ConfigureButtons(TipButtonCombo comboType)
        {
            // 默认隐藏所有，按需开启
            IsBtn1Visible = false; IsBtn2Visible = false; IsBtn3Visible = false;

            switch (comboType)
            {
                case TipButtonCombo.Ok:
                    IsBtn1Visible = true;
                    Btn1Text = "确定";
                    _btn1Result = DialogResultType.Ok;
                    break;

                case TipButtonCombo.YesNo:
                    IsBtn1Visible = true; IsBtn3Visible = true;
                    Btn1Text = "是(YES)"; _btn1Result = DialogResultType.Yes;
                    Btn3Text = "否(NO)"; _btn3Result = DialogResultType.No;
                    break;

                case TipButtonCombo.OkCancel:
                    IsBtn1Visible = true; IsBtn3Visible = true;
                    Btn1Text = "确定"; _btn1Result = DialogResultType.Ok;
                    Btn3Text = "取消"; _btn3Result = DialogResultType.Cancel;
                    break;

                case TipButtonCombo.RetryCancel:
                    IsBtn1Visible = true; IsBtn3Visible = true;
                    Btn1Text = "重试"; _btn1Result = DialogResultType.Retry;
                    Btn3Text = "取消"; _btn3Result = DialogResultType.Cancel;
                    break;

                case TipButtonCombo.YesNoSkip:
                    IsBtn1Visible = true; IsBtn2Visible = true; IsBtn3Visible = true;
                    Btn1Text = "是"; _btn1Result = DialogResultType.Yes;
                    Btn2Text = "跳过"; _btn2Result = DialogResultType.Skip;
                    Btn3Text = "否"; _btn3Result = DialogResultType.No;
                    break;

                case TipButtonCombo.OkCancelSkip:
                    IsBtn1Visible = true; IsBtn2Visible = true; IsBtn3Visible = true;
                    Btn1Text = "确定"; _btn1Result = DialogResultType.Ok;
                    Btn2Text = "跳过"; _btn2Result = DialogResultType.Skip;
                    Btn3Text = "取消"; _btn3Result = DialogResultType.Cancel;
                    break;
            }
        }

        private void CloseDialog(DialogResultType result)
        {
            Result = result;
            RequestClose?.Invoke(result); // 触发 View 层的关闭逻辑
        }

        #endregion
    }
}
