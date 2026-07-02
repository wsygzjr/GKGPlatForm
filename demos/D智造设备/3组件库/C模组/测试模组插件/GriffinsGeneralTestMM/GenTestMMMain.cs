using Avalonia.Threading;
using Griffins.ImeIOT;
using Griffins.Map;
using Griffins.Theme;
using System;

namespace GriffinsGeneralTestMM
{
    public static class GenTestMMMain
    {
        private static bool _hasInit = false;
        private static FormTestMMMainWindow? _formTestMMMain; // Avalonia窗口实例
        private static FormTestMMMainViewModel? _viewModel; // 核心VM实例

        /// <summary>
        /// 初始化（适配Avalonia+MVVM）
        /// </summary>
        public static void Init()
        {
            ThemeManager.Init();       // 加载主题库
            Griffins.UI2.ResourceManager.Load();      // 加载UI的样式和资源文件
            Griffins.UI.Test.ResourceManager.Load();  // 加载UI.Test的样式和资源文件
            ResourceManager.Load();  // 加载本项目的样式和资源文件
              
            if (_hasInit) return;
            _hasInit = true;

			Dispatcher.UIThread.Invoke(() =>
			{
				// 创建Avalonia窗口和VM
				_formTestMMMain = new FormTestMMMainWindow();
				_viewModel = new FormTestMMMainViewModel();
				_formTestMMMain.DataContext = _viewModel;
				// 订阅VM的参数变更事 
				_viewModel.AfterParamChanged += ViewModel_AfterParamChanged;

				// 初始化全局静态属性
				execPercent = _viewModel.ExecPercent;
				closAllMsgShow = _viewModel.ClosAllMsgShow;
				isTestMode = _viewModel.IsTestMode;

				// 显示窗口
				_formTestMMMain.Show();
			});
			
        }

        /// <summary>
        /// VM参数变更事件处理 
        /// </summary>
        private static void ViewModel_AfterParamChanged(object? sender, EventArgs e)
        {
            if (_viewModel == null) return;

            // 更新全局静态属性
            execPercent = _viewModel.ExecPercent;
            closAllMsgShow = _viewModel.ClosAllMsgShow;
            isTestMode = _viewModel.IsTestMode;

            // 保留原通知逻辑
            TestMMMain.DoExecPercentChanged(execPercent);
            TestSubMMMain.DoExecPercentChanged(execPercent);
        }

        // 以下属性完全保留原定义，仅从VM获取值
        private static int execPercent;
        internal static int ExecPercent => execPercent;

        private static bool closAllMsgShow;
        internal static bool ClosAllMsgShow => closAllMsgShow;

        private static bool isTestMode = false;
        public static bool IsTestMode => isTestMode;
    }
}