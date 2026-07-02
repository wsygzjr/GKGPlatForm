using Avalonia;
using Griffins.ImeIOT;
using Griffins.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 

namespace GriffinsGeneralTestMM
{
	public static class TestSubMMMain
	{
		private static bool hasInit = false;
		private static FormAllSubMMWindow formAllSubMM;
        private static Dictionary<SubMMAlias, UctlTestSubMMView> subMMUIDict = new Dictionary<SubMMAlias, UctlTestSubMMView>();

        public static void Init() 
		{
			if (hasInit)
				return;
			hasInit = true;

			GenTestMMMain.Init();

			formAllSubMM = new FormAllSubMMWindow(GenTestMMMain.ExecPercent);
            FormAllSubMMViewModel formAllSubMMViewModel = new FormAllSubMMViewModel();
            formAllSubMM.DataContext = formAllSubMMViewModel;
            formAllSubMM.WindowState = Avalonia.Controls.WindowState.Normal;
			formAllSubMM.Show();
			formAllSubMM.Hide();
		}

		internal static void Show()
		{
			if (formAllSubMM == null)
				return; 
			formAllSubMM.Show();
		}

		internal static void DoExecPercentChanged(int execPercent)
		{
			formAllSubMM.AdjustCurExecPercent(execPercent);
		}

        ///// <summary>
        /////  创建子机械模组配置接口实例
        ///// </summary>
        ///// <param name="genSubMMInfo">子机械模组信息</param>
        ///// <param name="alias">子机械模组实例别名</param>
        ///// <returns>子机械模组配置接口实例</returns>
        //public static ISubMachineModulesConfig CreateSubMMConfig(GenSubMMInfo genSubMMInfo, SubMMAlias alias)
        //{
        //    UctlTestSubMMView uctlTestSubMM = getUctlTestSubMM(alias, genSubMMInfo);
        //    return new SubMachineModulesConfig(genSubMMInfo, alias, uctlTestSubMM);
        //}

        /// <summary>
        ///  创建子机械模组（复合子机械模组）命令执行对象接口实例
        /// </summary>
        /// <param name="genSubMMInfo">子机械模组信息</param>
        /// <param name="alias">子机械模组实例别名</param>
        /// <returns>子机械模组（复合子机械模组）命令执行对象接口实例</returns>
        public static ISubMMCmdExecutor CreateSubMMCmdExecutor(GenSubMMInfo genSubMMInfo, SubMMAlias alias)
        {
			UctlTestSubMMView uctlTestSubMM = GetUctlTestSubMM(alias, genSubMMInfo);
            return new GenTestSubMMCmdExecutor(genSubMMInfo, alias, uctlTestSubMM);
		}

        public static UctlTestSubMMView GetUctlTestSubMM(SubMMAlias alias, GenSubMMInfo genSubMMInfo)
        {
            if (subMMUIDict.ContainsKey(alias))
                return subMMUIDict[alias];
            else
            {
                UctlTestSubMMView uctlTestSubMM = new UctlTestSubMMView();
                uctlTestSubMM.ViewModel = new UctlTestSubMMViewModel(formAllSubMM);
                uctlTestSubMM.ViewModel.Init(genSubMMInfo, alias);
                uctlTestSubMM.DataContext = uctlTestSubMM.ViewModel; 
                Thickness Margin = new Thickness(10);
                uctlTestSubMM.Margin = Margin;

                formAllSubMM.AddTestSubMM(uctlTestSubMM);
                subMMUIDict[alias] = uctlTestSubMM;
                return uctlTestSubMM;
            } 
        }

        /// <summary>
        ///  创建子机械模组能力定义接口实例
        /// </summary>
        /// <param name="genSubMMInfo">子机械模组信息</param>
        /// <returns>子机械模组能力定义接口实例</returns>
        public static ISubMachineModulesCabilityDef CretaeSubMMCabilityDef(GenSubMMInfo genSubMMInfo)
        {
			return new SubMachineModulesCabilityDef(genSubMMInfo);
		}
		
    }
}
