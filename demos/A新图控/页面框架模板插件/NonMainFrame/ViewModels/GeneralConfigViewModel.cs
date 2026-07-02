using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using Avalonia.VisualTree;
using Griffins.UI.General;
using NonMainFrameViewModel.Models;
using ReactiveUI;
using System.Reactive;
using System.Reflection.Metadata;

namespace NonMainFrameViewModel.ViewModels
{
    /// <summary>
    /// 通用配置视图模型
    /// </summary>
    public class GeneralConfigViewModel : ReactiveObject
    {
        /// <summary>
        /// 页面布局对象
        /// </summary>
        private Control? _viewReference;
        private Bitmap? _gotorootpageIconBitmap;
        private Bitmap? _closeIconBitmap;
        private string _tileName;

        /// <summary>
        /// 跳转到首页图标位图对象
        /// </summary>
        public Bitmap? GotoRootPageIconBitmap
        {
            get => _gotorootpageIconBitmap;
            set => this.RaiseAndSetIfChanged(ref _gotorootpageIconBitmap, value);
        }

        /// <summary>
        /// 跳转到上级页面图标位图对象
        /// </summary>
        public Bitmap? GotoParentPageIconBitmap
        {
            get => _closeIconBitmap;
            set => this.RaiseAndSetIfChanged(ref _closeIconBitmap, value);
        }

        /// <summary>
        /// 标题名称
        /// </summary>
        public string TileName
        {
            get => _tileName;
            set
            {
                // 如果值发生了实际改变，再触发更新和通知
                if (_tileName != value)
                {
                    this.RaiseAndSetIfChanged(ref _tileName, value);
                    onConfigModified(); // 关键：输入文字后也要触发修改事件
                }
            }
        }

        /// <summary>
        ///  跳转到首页图标命令
        /// </summary>
        public ReactiveCommand<Unit, Unit> SelectGotoRootPageIconCommand { get; }

        /// <summary>
        ///  跳转到上级页面图标图标命令
        /// </summary>
        public ReactiveCommand<Unit, Unit> SelectGotoParentPageIconCommand { get; }

        /// <summary>
        /// 配置修改事件
        /// </summary>
        public event EventHandler? AfterModified;

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public GeneralConfigViewModel()
        {
            SelectGotoRootPageIconCommand = ReactiveCommand.CreateFromTask(selectGotoRootPageIconAsync);
            SelectGotoParentPageIconCommand = ReactiveCommand.CreateFromTask(selectGotoParentPageIconAsync);
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

            GotoRootPageIconBitmap = source.GotoRootPageIcon;
            GotoParentPageIconBitmap = source.GotoParentPageIcon;
            TileName = source.TileName;
        }

        /// <summary>
        /// 从视图模型提取配置信息
        /// </summary>
        public GeneralConfigInfo Extract()
        {
            GeneralConfigInfo cfgInfo = new GeneralConfigInfo();
            cfgInfo.GotoRootPageIcon = GotoRootPageIconBitmap;
            cfgInfo.GotoParentPageIcon = GotoParentPageIconBitmap;
            cfgInfo.TileName = TileName;
            return cfgInfo;
        }


        /// <summary>
        /// 
        /// </summary>
        private async Task selectGotoRootPageIconAsync()
        {
            var bitmap = await selectImageFileAndLoadBitmapAsync();
            if (bitmap != null)
            {
                GotoRootPageIconBitmap = bitmap;
                onConfigModified();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private async Task selectGotoParentPageIconAsync()
        {
            var bitmap = await selectImageFileAndLoadBitmapAsync();
            if (bitmap != null)
            {
                GotoParentPageIconBitmap = bitmap;
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