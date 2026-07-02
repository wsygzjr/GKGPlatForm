using Avalonia.Media;
using ReactiveUI;
using Griffins.ImeIOT;

namespace GKG.CompUI.DeviceManager.ControlPanel.ViewModels
{
    /// <summary>
    /// 执行模式卡片视图模型
    /// </summary>
    public class ExecModeCardViewModel : ReactiveObject
    {
        public ExecModeCardViewModel(
            ImeExecMode mode,
            string title,
            string iconNormal,
            string iconSelected,
            string normalBackground,
            string selectedBackground,
            string accentColor)
        {
            Mode = mode;
            Title = title;
            IconNormal = iconNormal;
            IconSelected = iconSelected;
            NormalBackground = normalBackground;
            SelectedBackground = selectedBackground;
            AccentColor = accentColor;
        }

        // 执行模式类型
        public ImeExecMode Mode { get; }
        // 显示标题
        public string Title { get; }
        // 正常状态图标
        public string IconNormal { get; }
        // 选中状态图标
        public string IconSelected { get; }
        // 正常背景色
        public string NormalBackground { get; }
        // 选中背景色
        public string SelectedBackground { get; }
        // 强调色
        public string AccentColor { get; }

        // 是否选中
        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                this.RaiseAndSetIfChanged(ref _isSelected, value);
                this.RaisePropertyChanged(nameof(CurrentIcon));
                this.RaisePropertyChanged(nameof(BackgroundBrush));
                this.RaisePropertyChanged(nameof(ForegroundBrush));
            }
        }

        // 当前显示的图标
        public string CurrentIcon => IsSelected ? IconSelected : IconNormal;

        // 背景画刷
        public IBrush BackgroundBrush =>
            new SolidColorBrush(Color.Parse(IsSelected ? SelectedBackground : NormalBackground));

        // 前景画刷
        public IBrush ForegroundBrush =>
            new SolidColorBrush(Color.Parse(IsSelected ? "#FFFFFF" : "#111827"));

        // 是否启用
        private bool _isEnabled = true;
        public bool IsEnabled
        {
            get => _isEnabled;
            set => this.RaiseAndSetIfChanged(ref _isEnabled, value);
        }
    }
}
