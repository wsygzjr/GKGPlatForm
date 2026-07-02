using Griffins;
using System.Threading;

namespace GKG.SubMM.Dispenser
{
    internal static class PauseObj
    {
        public static int Status { get; set; }

        private static bool isWaiting;

        public static void Wait()
        {
            switch (Status)
            {
                case 1://暂停
                    wait();
                    break;

                case 2://恢复

                    break;
            }
        }

        private static void wait()
        {
            if (isWaiting)
                return;
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