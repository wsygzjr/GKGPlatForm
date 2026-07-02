using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using GF_Gereric;
using Griffins.UI2;
using ReactiveUI;
using System;
using System.ComponentModel;
using System.Reactive;

namespace GKG.Map.StatusInfoFuncCtlMapCell.ViewModel
{
    /// <summary>
    /// 状态信息视图模型
    /// </summary>
    public class StatusInfoViewModel : ReactiveObject
    {
        #region 响应式属性

        private FontInfo _textFont;
        /// <summary>
        /// 文本字体
        /// </summary>
        public FontInfo TextFont
        {
            get { return _textFont; }
            set { this.RaiseAndSetIfChanged(ref _textFont, value); }
        }

        private Color _textColor;
        /// <summary>
        /// 文本颜色
        /// </summary>
        public Color TextColor
        {
            get { return _textColor; }
            set { this.RaiseAndSetIfChanged(ref _textColor, value); }
        }

        private Color _backColor;
        /// <summary>
        /// 背景颜色
        /// </summary>
        public Color BackColor
        {
            get { return _backColor; }
            set { this.RaiseAndSetIfChanged(ref _backColor, value); }
        }

        private bool _leftValveGlueMonitorState;
        /// <summary>
        /// 左侧阀门胶水监控状态
        /// </summary>
        public bool LeftValveGlueMonitorState
        {
            get { return _leftValveGlueMonitorState; }
            set { this.RaiseAndSetIfChanged(ref _leftValveGlueMonitorState, value); }
        }

        private bool _rightValveGlueMonitorState;
        /// <summary>
        /// 右侧阀门胶水监控状态
        /// </summary>
        public bool RightValveGlueMonitorState
        {
            get { return _rightValveGlueMonitorState; }
            set { this.RaiseAndSetIfChanged(ref _rightValveGlueMonitorState, value); }
        }

        private bool _rightPressureCyclesAlarmState;
        /// <summary>
        /// 右侧压力周期报警状态
        /// </summary>
        public bool RightPressureCyclesAlarmState
        {
            get { return _rightPressureCyclesAlarmState; }
            set { this.RaiseAndSetIfChanged(ref _rightPressureCyclesAlarmState, value); }
        }

        private bool _leftValveQuantitativeGlueMonitorState;
        /// <summary>
        /// 左侧阀门定时定量胶水监控状态
        /// </summary>
        public bool LeftValveQuantitativeGlueMonitorState
        {
            get { return _leftValveQuantitativeGlueMonitorState; }
            set { this.RaiseAndSetIfChanged(ref _leftValveQuantitativeGlueMonitorState, value); }
        }

        private bool _leftValveRemainingMonitorState;
        /// <summary>
        /// 左侧阀门余量监控状态
        /// </summary>
        public bool LeftValveRemainingMonitorState
        {
            get { return _leftValveRemainingMonitorState; }
            set { this.RaiseAndSetIfChanged(ref _leftValveRemainingMonitorState, value); }
        }

        private bool _leftPressureCyclesAlarmState;
        /// <summary>
        /// 左侧压力周期报警状态
        /// </summary>
        public bool LeftPressureCyclesAlarmState
        {
            get { return _leftPressureCyclesAlarmState; }
            set { this.RaiseAndSetIfChanged(ref _leftPressureCyclesAlarmState, value); }
        }

        private bool _isDualValve;
        /// <summary>
        /// 是否为双阀模式
        /// </summary>
        public bool IsDualValve
        {
            get { return _isDualValve; }
            set { this.RaiseAndSetIfChanged(ref _isDualValve, value); }
        }

        private bool _rightValveQuantitativeGlueMonitorState;
        /// <summary>
        /// 右侧阀门定时定量胶水监控状态
        /// </summary>
        public bool RightValveQuantitativeGlueMonitorState
        {
            get { return _rightValveQuantitativeGlueMonitorState; }
            set { this.RaiseAndSetIfChanged(ref _rightValveQuantitativeGlueMonitorState, value); }
        }

        private bool _rightValveRemainingMonitorState;
        /// <summary>
        /// 右侧阀门余量监控状态
        /// </summary>
        public bool RightValveRemainingMonitorState
        {
            get { return _rightValveRemainingMonitorState; }
            set { this.RaiseAndSetIfChanged(ref _rightValveRemainingMonitorState, value); }
        }

        private string _aWaitingAddGlueTime;
        /// <summary>
        /// 等待添加胶水时间A
        /// </summary>
        public string AWaitingAddGlueTime
        {
            get { return _aWaitingAddGlueTime; }
            set { this.RaiseAndSetIfChanged(ref _aWaitingAddGlueTime, value); }
        }

        private string _bWaitingAddGlueTime;
        /// <summary>
        /// 等待添加胶水时间B
        /// </summary>
        public string BWaitingAddGlueTime
        {
            get { return _bWaitingAddGlueTime; }
            set { this.RaiseAndSetIfChanged(ref _bWaitingAddGlueTime, value); }
        }

        private Bitmap _statusOkImage;
        /// <summary>
        /// 状态正常时显示的图标
        /// </summary>
        public Bitmap StatusOkImage
        {
            get { return _statusOkImage; }
            set { this.RaiseAndSetIfChanged(ref _statusOkImage, value); }
        }

        private Bitmap _statusNgImage;
        /// <summary>
        /// 状态为false时显示的图标
        /// </summary>
        public Bitmap StatusNgImage
        {
            get { return _statusNgImage; }
            set { this.RaiseAndSetIfChanged(ref _statusNgImage, value); }
        }

        #endregion

        #region 命令

        /// <summary>
        /// 鼠标按下命令
        /// </summary>
        public ReactiveCommand<Point, Unit> PointerPressedCommand { get; }

        #endregion

        #region 私有字段

        private readonly StatusInfoPropertyModelEdit _model;
        private readonly Action _clickAction;

        #endregion

        #region 构造函数

        /// <summary>
        /// 初始化状态信息视图模型
        /// </summary>
        /// <param name="statusInfoPropertyModelEdit">状态信息属性模型</param>
        /// <param name="clickAction">点击动作</param>
        public StatusInfoViewModel(StatusInfoPropertyModelEdit statusInfoPropertyModelEdit, Action clickAction)
        {
            _model = statusInfoPropertyModelEdit;
            TextFont = statusInfoPropertyModelEdit.TextFont;
            TextColor = statusInfoPropertyModelEdit.TextColor;
            BackColor = statusInfoPropertyModelEdit.BackColor;
            LeftValveGlueMonitorState = false;
            LeftValveQuantitativeGlueMonitorState = false;
            LeftValveRemainingMonitorState = false;
            LeftPressureCyclesAlarmState = false;
            RightValveGlueMonitorState = false;
            RightValveQuantitativeGlueMonitorState = false;
            RightValveRemainingMonitorState = false;
            RightPressureCyclesAlarmState = false;
            IsDualValve = false;
            AWaitingAddGlueTime = "00:00";
            BWaitingAddGlueTime = "00:00";

            StatusOkImage = TryLoadBitmap("avares://Griffins.Map.StatusInfoFuncCtlMapCell/Assets/Images/Green.png");
            StatusNgImage = TryLoadBitmap("avares://Griffins.Map.StatusInfoFuncCtlMapCell/Assets/Images/Gray.png");

            _clickAction = clickAction;
            PointerPressedCommand = ReactiveCommand.Create<Point>(OnPointerPressed);

            _model.PropertyChanged += Model_PropertyChanged;
        }

        #endregion

        /// <summary>
        /// 模型属性变更事件处理
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">属性变更事件参数</param>
        private void Model_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(StatusInfoPropertyModelEdit.TextFont):
                    TextFont = _model.TextFont;
                    break;
                case nameof(StatusInfoPropertyModelEdit.TextColor):
                    TextColor = _model.TextColor;
                    break;
                case nameof(StatusInfoPropertyModelEdit.BackColor):
                    BackColor = _model.BackColor;
                    break;
            }
        }

        #region 私有方法

        /// <summary>
        /// 鼠标按下事件处理
        /// </summary>
        /// <param name="screenP">屏幕坐标</param>
        private void OnPointerPressed(Point screenP)
        {
            _clickAction?.Invoke();
        }

        /// <summary>
        /// 尝试加载位图资源
        /// </summary>
        /// <param name="uriText">资源URI</param>
        /// <returns>加载的位图，失败时返回null</returns>
        private static Bitmap TryLoadBitmap(string uriText)
        {
            try
            {
                var uri = new Uri(uriText);
                using (var stream = AssetLoader.Open(uri))
                {
                    return new Bitmap(stream);
                }
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
