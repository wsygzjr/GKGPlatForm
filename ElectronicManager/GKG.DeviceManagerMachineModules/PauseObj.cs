using Griffins;
using System.Threading;

namespace GKG
{
    namespace MM
    {
        static class PauseObj
        {
            /// <summary>`1` 表示暂停，`2` 表示运行；由宿主生命周期方法统一切换。</summary>
            public static int Status { get; set; }

            private static bool isWaiting;

            /// <summary>在长流程里调用该方法即可感知暂停状态；恢复后自动继续向下执行。</summary>
            public static void Wait()
            {
                switch (Status)
                {
                    case 1:
                        wait();
                        break;
                    case 2:
                        break;
                }
            }

            /// <summary>轮询等待恢复或宿主退出；不主动抛错，只阻塞当前流程。</summary>
            private static void wait()
            {
                if (isWaiting)
                {
                    return;
                }

                while (Status == 1)
                {
                    Thread.Sleep(100);
                    if (GriffinsApplication.Terminated)
                    {
                        break;
                    }
                }

                isWaiting = false;
            }
        }
    }
}
