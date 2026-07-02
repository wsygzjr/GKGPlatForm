using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using Avalonia.Threading;
using System;
using System.Threading.Tasks;

namespace GriffinsGeneralTestMM
{
    /// <summary>
    /// 资源管理类
    /// </summary>
    public static class ResourceManager
    {
        private static bool _loaded = false;
        /// <summary>
        /// 加载资源
        /// </summary>
        public static void Load()
        {
            if (_loaded)
                return;
            if (Application.Current == null)
                throw new InvalidOperationException("应用尚未初始化完成");

            LoadResourceInBackground();

            _loaded = true;
        }

        // 后台线程中安全操作 UI 资源
        private static void LoadResourceInBackground()
        {
            // 切到 UI 线程  核心
             Dispatcher.UIThread.Invoke(() =>
            { 
                // 添加到窗口/应用程序资源
                if (Application.Current != null)
                {
                    //加载页面类型个性化样式
                    Application.Current.Styles.Add(loadStyleFile("avares://GriffinsGeneralTestMM/Styles/GlobalStyles.axaml"));
                } 
            });
        }

        /// <summary>
        /// 加载资源字典文件
        /// </summary>
        /// <param name="uri">主题资源文件路径</param>
        /// <returns>加载完成的资源字典</returns>
        private static ResourceDictionary loadResourceDictionary(string uri)
        {
            if (string.IsNullOrWhiteSpace(uri))
                throw new ArgumentNullException(nameof(uri), "主题资源 URI 不能为空");

            try
            {
                var resourceUri = new Uri(uri);
                var resourceDict = (ResourceDictionary)AvaloniaXamlLoader.Load(resourceUri);
                return resourceDict ?? new ResourceDictionary();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"加载主题资源失败（URI：{uri}）：{ex.Message}");
                // 返回空资源字典，避免后续逻辑异常中断
                return new ResourceDictionary();
            }
        }

        /// <summary>
        /// 加载样式
        /// </summary>
        /// <param name="styleUri">样式文件 URI（支持 avares:// 跨程序集格式）</param>
        /// <returns>加载完成的样式集合</returns>
        private static Styles loadStyleFile(string styleUri)
        {
            if (string.IsNullOrWhiteSpace(styleUri))
                throw new ArgumentNullException(nameof(styleUri), "样式资源 URI 不能为空");

            try
            {
                var uri = new Uri(styleUri);
                var loadedStyles = AvaloniaXamlLoader.Load(uri) as Styles;
                return loadedStyles ?? new Styles();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"加载样式资源失败（URI：{styleUri}）：{ex.Message}");
                return new Styles();
            }
        }
    }
}