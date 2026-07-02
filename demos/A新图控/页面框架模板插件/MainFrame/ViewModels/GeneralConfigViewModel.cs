using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using Avalonia.VisualTree;
using Griffins.UI.General;
using MainFrame.Models;
using ReactiveUI;
using System.Reactive;

namespace MainFrame.ViewModels
{
    /// <summary>
    /// 通用配置视图模型（窗口按钮图标配置）
    /// </summary>
    public class GeneralConfigViewModel : ReactiveObject
    {
        /// <summary>
        /// 页面布局对象
        /// </summary>
        private Control? _viewReference;
        private Bitmap? _minimizeIconBitmap;
        private Bitmap? _maximizeIconBitmap;
        private Bitmap? _closeIconBitmap;
        private bool _isMinimizeButtonEnabled = true;

        /// <summary>
        /// 最小化图标位图对象
        /// </summary>
        public Bitmap? MinimizeIconBitmap
        {
            get => _minimizeIconBitmap;
            set => this.RaiseAndSetIfChanged(ref _minimizeIconBitmap, value);
        }

        /// <summary>
        /// 最大化图标位图对象
        /// </summary>
        public Bitmap? MaximizeIconBitmap
        {
            get => _maximizeIconBitmap;
            set => this.RaiseAndSetIfChanged(ref _maximizeIconBitmap, value);
        }

        /// <summary>
        /// 关闭图标位图对象
        /// </summary>
        public Bitmap? CloseIconBitmap
        {
            get => _closeIconBitmap;
            set => this.RaiseAndSetIfChanged(ref _closeIconBitmap, value);
        }

        /// <summary>
        /// 最小化按钮启用状态
        /// </summary>
        public bool IsMinimizeButtonEnabled
        {
            get => _isMinimizeButtonEnabled;
            set
            {
                this.RaiseAndSetIfChanged(ref _isMinimizeButtonEnabled, value);
                onConfigModified();
            }
        }


        /// <summary>
        /// 选择最小化图标命令
        /// </summary>
        public ReactiveCommand<Unit, Unit> SelectMinimizeIconCommand { get; }

        /// <summary>
        /// 选择最大化图标命令
        /// </summary>
        public ReactiveCommand<Unit, Unit> SelectMaximizeIconCommand { get; }

        /// <summary>
        /// 选择关闭图标命令
        /// </summary>
        public ReactiveCommand<Unit, Unit> SelectCloseIconCommand { get; }

        /// <summary>
        /// 配置修改事件
        /// </summary>
        public event EventHandler? AfterModified;

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public GeneralConfigViewModel()
        {
            SelectMinimizeIconCommand = ReactiveCommand.CreateFromTask(SelectMinimizeIconAsync);
            SelectMaximizeIconCommand = ReactiveCommand.CreateFromTask(selectMaximizeIconAsync);
            SelectCloseIconCommand = ReactiveCommand.CreateFromTask(selectCloseIconAsync);
        }


        /// <summary>
        /// 设置视图引用
        /// </summary>
        public void SetViewReference(Control view)
        {
            _viewReference = view;
        }

        /// <summary>
        /// 从配置信息加载到视图模型
        /// </summary>
        /// <param name="source">通用配置信息</param>
        public void FillToVM(GeneralConfigInfo source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            
            MinimizeIconBitmap = source.MinimizeIcon;
            CloseIconBitmap = source.CloseIcon;
            _isMinimizeButtonEnabled = source.IsMinimizeButtonEnabled;
        }

        /// <summary>
        /// 从视图模型提取配置信息
        /// </summary>
        public GeneralConfigInfo Extract()
        {
            GeneralConfigInfo cfgInfo = new GeneralConfigInfo();
            cfgInfo.MinimizeIcon = MinimizeIconBitmap;
            cfgInfo.CloseIcon = CloseIconBitmap;
            cfgInfo.IsMinimizeButtonEnabled = IsMinimizeButtonEnabled;
            return cfgInfo;
        }

        /// <summary>
        /// 选择最小化图标
        /// </summary>
        private async Task SelectMinimizeIconAsync()
        {
            var bitmap = await selectImageFileAndLoadBitmapAsync();
            if (bitmap != null)
            {
                MinimizeIconBitmap = bitmap;
                onConfigModified();
            }
        }

        /// <summary>
        /// 选择最大化图标
        /// </summary>
        private async Task selectMaximizeIconAsync()
        {
            var bitmap = await selectImageFileAndLoadBitmapAsync();
            if (bitmap != null)
            {
                MaximizeIconBitmap = bitmap;
                onConfigModified();
            }
        }

        /// <summary>
        /// 选择关闭图标
        /// </summary>
        private async Task selectCloseIconAsync()
        {
            var bitmap = await selectImageFileAndLoadBitmapAsync();
            if (bitmap != null)
            {
                CloseIconBitmap = bitmap;
                onConfigModified();
            }
        }
        
        /// <summary>
        /// 选择图片文件并加载为Bitmap
        /// </summary>
        /// <returns>Bitmap对象</returns>
        private async Task<Bitmap?> selectImageFileAndLoadBitmapAsync()
        {
            var parentWindow = _viewReference?.GetVisualRoot() as Window;
            if (parentWindow == null)
            {
                await MessageBox.ShowErrorDialog("错误", "无法获取窗口上下文，操作失败", _viewReference);
                return null;
            }
            var topLevel = TopLevel.GetTopLevel(parentWindow);
            if (topLevel == null)
            {
                return null;
            }

            // 打开文件选择对话框
            var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "选择图片文件",
                AllowMultiple = false,
                FileTypeFilter = new[]
                {
                    new FilePickerFileType("位图文件")
                    {
                        Patterns = new[] { "*.bmp", "*.png", "*.jpg", "*.jpeg", "*.gif" },
                        AppleUniformTypeIdentifiers = new[] { "public.image" },
                        MimeTypes = new[] { "image/*" }
                    }
                }
            });

            if (files.Count > 0)
            {
                var filePath = files[0].Path.LocalPath;
                try
                {
                    // 使用流加载Bitmap，避免文件锁定
                    using var stream = await files[0].OpenReadAsync();
                    var bitmap = new Bitmap(stream);
                    return bitmap;
                }
                catch (Exception)
                {
                    return null;
                }
            }

            return null;
        }

        /// <summary>
        /// 触发配置修改事件
        /// </summary>
        private void onConfigModified()
        {
            AfterModified?.Invoke(this, EventArgs.Empty);
        }
    }
}