using Avalonia;
using Griffins.ImeIOT;
using Griffins.Map;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 

namespace GriffinsGeneralTestMM
{
	public static class TestMMMain
	{
		private static bool hasInit = false;
		private static FormAllMMWindow formAllMM;
		private static Dictionary<MMAlias, UctlTestMMView> mmUIDict = new Dictionary<MMAlias, UctlTestMMView>();

		public static void Init() 
		{
            ////测试
            //while (Application.Current == null)
            //    Task.Delay(50);
            if (hasInit)
				return;
			hasInit = true;

			GenTestMMMain.Init();

			formAllMM = new FormAllMMWindow(GenTestMMMain.ExecPercent);
			formAllMM.WindowState = Avalonia.Controls.WindowState.Normal; 
            formAllMM.Show();
			formAllMM.Hide();
		}

		internal static void Show()
		{
			if (formAllMM == null)
				return;  

            formAllMM.Show();

		}

		internal static void DoExecPercentChanged(int execPercent)
		{
			formAllMM.AdjustCurExecPercent(execPercent);
		}

        ///// <summary>
        /////  创建机械模组配置接口
        ///// </summary>
        ///// <param name="genMMInfo">机械模组信息</param>
        ///// <param name="alias">机械模组实例ID</param>
        ///// <returns>机械模组配置接口</returns>
        //public static IMachineModulesConfig CreateMachineModulesConfig(GenMMInfo genMMInfo, MMAlias alias)
        //{
        //    UctlTestMMView uctlTestMM = getUctlTestMM(alias, genMMInfo);
        //    return new MachineModulesConfig(genMMInfo, alias, uctlTestMM);
        //}

        /// <summary>
        ///  创建机械模组命令执行对象接口
        /// </summary>
        /// <param name="genMMInfo">机械模组信息</param>
        /// <param name="alias">机械模组实例ID</param>
        /// <returns>机械模组命令执行对象接口</returns>
        public static IMMCmdExecutor CreateMMCmdExecutor(GenMMInfo genMMInfo, MMAlias alias)
        {
            UctlTestMMView uctlTestMM = GetUctlTestMM(alias, genMMInfo);
            return new GenTestMMCmdExecutor(genMMInfo, alias, uctlTestMM);
 		}

		public static UctlTestMMView GetUctlTestMM(MMAlias alias, GenMMInfo genMMInfo)
		{
			if (mmUIDict.ContainsKey(alias))
				return mmUIDict[alias];
			else
			{
                UctlTestMMView uctlTestMM = new UctlTestMMView();
				uctlTestMM.ViewModel= new UctlTestMMViewModel(formAllMM);
                //var uctlTestMMViewModel = new UctlTestMMViewModel();
                uctlTestMM.ViewModel.Init(genMMInfo, alias);
                uctlTestMM.DataContext = uctlTestMM.ViewModel;


                formAllMM.AddTestMM(uctlTestMM.ViewModel);
                mmUIDict[alias] = uctlTestMM;
				return uctlTestMM;
            }
		}

        /// <summary>
        ///  创建机械模组能力定义接口
        /// </summary>
        /// <param name="genMMInfo">机械模组信息</param>
        /// <returns>机械模组能力定义接口</returns>
        public static IMachineModulesCabilityDef CretaeMMCabilityDef(GenMMInfo genMMInfo)
        {
			return new MachineModulesCabilityDef(genMMInfo);
		}
		
    }
}
