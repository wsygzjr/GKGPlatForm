using System.Threading;
using Griffins;

namespace GKG.SubMM;

/// <summary>
/// 暂停/恢复状态控制对象
/// 用于控制子机械模组的运行状态（暂停/恢复）
/// </summary>
public static class PauseObj
{
    #region 字段

    private static int _status;
    private static bool _isWaiting;
    private static readonly object _lockObj = new object();

    #endregion

    #region 属性

    /// <summary>
    /// 运行状态
    /// 1 = 暂停，2 = 运行/恢复
    /// </summary>
    public static int Status
    {
        get => _status;
        set
        {
            lock (_lockObj)
            {
                _status = value;
                // 状态改变时唤醒等待的线程
                if (_status == 2 && _isWaiting)
                {
                    _isWaiting = false;
                }
            }
        }
    }

    #endregion

    #region 公共方法

    /// <summary>
    /// 等待（如果处于暂停状态）
    /// 在暂停状态下阻塞当前线程，直到状态恢复
    /// </summary>
    public static void Wait()
    {
        if (_status == 1)
        {
            WaitForResume();
        }
    }

    /// <summary>
    /// 强制等待（无论当前状态如何）
    /// </summary>
    public static void ForceWait()
    {
        WaitForResume();
    }

    #endregion

    #region 私有方法

    /// <summary>
    /// 等待恢复
    /// </summary>
    private static void WaitForResume()
    {
        lock (_lockObj)
        {
            if (_isWaiting)
                return;

            _isWaiting = true;

            while (_status == 1)
            {
                // 检查应用程序是否已终止
                if (GriffinsApplication.Terminated)
                    break;

                // 短暂休眠，避免 CPU 占用过高
                Thread.Sleep(100);
            }

            _isWaiting = false;
        }
    }

    #endregion
}
