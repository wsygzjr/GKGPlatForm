using ReactiveUI;
using System;
using System.Threading.Tasks;

namespace GKG.Map.AuxiliaryFunctionsFuncCtlMapCell.ViewModel
{
    public class WaitingBoxViewModel : ReactiveObject
    {
        // 绑定的提示文本
        private string _message;
        public string Message
        {
            get => _message;
            set => this.RaiseAndSetIfChanged(ref _message, value);
        }

        // 绑定的进度条当前值
        private double _progressValue;
        public double ProgressValue
        {
            get => _progressValue;
            set => this.RaiseAndSetIfChanged(ref _progressValue, value);
        }

        // 现代化的异步任务委托
        private readonly Func<Task> _workTask;
        private bool _isRunning;

        public WaitingBoxViewModel(Func<Task> workTask, string initialMessage)
        {
            _workTask = workTask;
            Message = initialMessage;
            ProgressValue = 0;
        }

        /// <summary>
        /// 启动后台任务和进度条更新
        /// </summary>
        public async Task StartWorkAsync()
        {
            _isRunning = true;

            // 触发假进度条更新，不阻塞主任务
            _ = UpdateFakeProgressAsync();

            try
            {
                if (_workTask != null)
                {
                    // 将任务扔到线程池中执行，防止阻塞 UI
                    await Task.Run(_workTask);
                }
            }
            catch (Exception ex)
            {
                Message = $"出现错误: {ex.Message}";
                await Task.Delay(2000); // 停留2秒让用户看到报错
            }
            finally
            {
                _isRunning = false;
                ProgressValue = 1000;  // 瞬间拉满进度条
                await Task.Delay(100); // 短暂延迟让动画走完
            }
        }

        /// <summary>
        /// 现代化的"障眼法"进度条更新，替代原本的 DispatcherTimer
        /// </summary>
        private async Task UpdateFakeProgressAsync()
        {
            var random = new Random();
            while (_isRunning)
            {
                if (ProgressValue < 300)
                    ProgressValue += random.Next(0, 3);
                else if (ProgressValue < 950)
                    ProgressValue += random.Next(0, 5);
                else if (ProgressValue < 999)
                    ProgressValue += random.NextDouble();

                // 异步等待 10 毫秒，不会卡死线程
                await Task.Delay(10);
            }
        }
    }
}
