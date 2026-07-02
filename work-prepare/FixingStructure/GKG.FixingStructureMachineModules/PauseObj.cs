using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Griffins;

namespace GKG
{
	namespace MM
	{
		static class PauseObj
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
}
