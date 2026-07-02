using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Griffins;

namespace GriffinsGeneralTestMM
{
	public class PauseObj
	{
		public int Status { get; set; }

		public void Wait()
		{
			switch (Status)
			{
				case 2://暂停
					wait();
					break;
				case 3://恢复

					break;
			}
		}

		private void wait()
		{
			while (Status == 2)
			{
				Thread.Sleep(100);
				if (GriffinsApplication.Terminated)
				{
					break;
				}
			}
		}
	}
}
