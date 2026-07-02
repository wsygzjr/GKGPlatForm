using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GKG
{
    public class PerfmTimer
    {
        // 核心高精度计时器
        private readonly Stopwatch _stopwatch = new Stopwatch();
        // 线程安全锁（工业场景多线程调用时的同步）
        private readonly object _lock = new object();
        // 计时状态标识
        private bool _isRunning;

        /// <summary>
        /// 启动计时（重复启动会重置计时起点）
        /// </summary>
        public void Start()
        {
            lock (_lock)
            {
                _stopwatch.Restart(); // 重置并启动（替代单独的Start+Reset）
                _isRunning = true;
            }
        }

        /// <summary>
        /// 停止计时（未启动时调用无操作）
        /// </summary>
        public void Stop()
        {
            lock (_lock)
            {
                if (!_isRunning) return;
                _stopwatch.Stop();
                _isRunning = false;
            }
        }

        /// <summary>
        /// 重置计时器状态（停止并清空耗时）
        /// </summary>
        public void Reset()
        {
            lock (_lock)
            {
                _stopwatch.Reset();
                _isRunning = false;
            }
        }

        /// <summary>
        /// 获取已耗时的TimeSpan（核心方法，所有单位转换基于此）
        /// </summary>
        /// <exception cref="InvalidOperationException">未启动计时时抛出</exception>
        public TimeSpan GetElapsedTime()
        {
            lock (_lock)
            {
                if (!_stopwatch.IsRunning && _stopwatch.ElapsedTicks == 0)
                {
                    throw new InvalidOperationException("计时器未启动，无法获取耗时");
                }
                return _stopwatch.Elapsed;
            }
        }

        /// <summary>
        /// 获取耗时（毫秒，保留3位小数）
        /// </summary>
        public double GetElapsedMilliseconds()
        {
            return GetElapsedTime().TotalMilliseconds;
        }

        /// <summary>
        /// 获取耗时（秒，保留6位小数）
        /// </summary>
        public double GetElapsedSeconds()
        {
            return GetElapsedTime().TotalSeconds;
        }

        /// <summary>
        /// 获取耗时（微秒，兼容.NET Framework/.NET Core）
        /// </summary>
        public long GetElapsedMicroseconds()
        {
            // 兼容方案：通过Ticks计算（1Tick = 100纳秒 = 0.1微秒）
            lock (_lock)
            {
                if (!_stopwatch.IsRunning && _stopwatch.ElapsedTicks == 0)
                {
                    throw new InvalidOperationException("计时器未启动，无法获取耗时");
                }
                return _stopwatch.ElapsedTicks * 100 / 1000; // Ticks → 微秒
            }
        }

        /// <summary>
        /// 获取耗时（纳秒，工业超高精度需求）
        /// </summary>
        public long GetElapsedNanoseconds()
        {
            lock (_lock)
            {
                if (!_stopwatch.IsRunning && _stopwatch.ElapsedTicks == 0)
                {
                    throw new InvalidOperationException("计时器未启动，无法获取耗时");
                }
                return _stopwatch.ElapsedTicks * 100; // 1Tick = 100纳秒
            }
        }

        // 只读属性：获取当前计时状态
        public bool IsRunning => _isRunning;
    }
}
