using Avalonia.Controls;
using Avalonia.Threading;
using Griffins;
using Griffins.ImeIOT;
using Griffins.Map.UI;
using Griffins.UI;
using Griffins.UI.Test;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GriffinsGeneralTestMM
{
  /// <summary>
  /// 控制面板对象
  /// </summary>
   public class MainControlPanel : IControlPanel
    {
        private ControlPanelDefInfoList controlPanels;
        private ControlPanelDefInfo? curControlPanelDefInfo;
        /// <summary>
        /// 组件控制面板回调接口实例
        /// </summary>
        private IControlPanelCallBack iControlPanelCallBack;

        public MainControlPanel(ControlPanelDefInfoList controlPanels)
        {
            this.controlPanels = controlPanels;
        }
        #region IControlPanel 成员

        /// <summary>
        /// 组件控制面板初始化
        /// </summary>
        /// <param name="iControlPanelCallBack">组件控制面板回调接口实例</param>
        void IControlPanel.Init(IControlPanelCallBack iControlPanelCallBack)
        {
            this.iControlPanelCallBack=iControlPanelCallBack;
        }
        /// <summary>
        /// 获取控制面板信息列表，null或个数为0表示没有对应的控制面板
        /// </summary>
        /// <returns>该机械模组的控制面板信息列表</returns>
        ControlPanelViewInfoList IControlPanel.GetControlPanelViewInfos()
        {
            var controlPanelViewInfoes = new ControlPanelViewInfoList();
            if(controlPanels!=null)
            {
                foreach (var controlPanel in controlPanels)
                {
                    controlPanelViewInfoes.Add(new ControlPanelViewInfo()
                    {
                        ControlPanelID = controlPanel.ControlPanelID,
                        ControlPanelName = controlPanel.ControlPanelName,
                    });
                }
            }
            return controlPanelViewInfoes;
        }
        /// <summary>
        /// 显示控制面板
        /// </summary>
        ///  <param name="controlPanelID">控制面板ID</param>
        ///  <param name="owner">父窗口</param>
        async Task IControlPanel.ShowControlPanelAsync(string controlPanelID, object owner)
        {
            var controlPanel = controlPanels?.Find(o=>o.ControlPanelID==controlPanelID);
            if (controlPanel == null)
                return;
            curControlPanelDefInfo = controlPanel;
            int controlPanelWidth= controlPanel.ControlPanelWinSize?.Width??1124;
            int controlPanelHeight = controlPanel.ControlPanelWinSize?.Height?? 805;

            var methodExecuteWindow = new MethodExecuteWindow()
            {
                Width = controlPanelWidth,
                Height = controlPanelHeight,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Title = controlPanel.ControlPanelName,
                CanResize = false
            };
            var methodExecuteViewModel = new MethodExecuteViewModel(false);
            methodExecuteViewModel.SetViewReference(methodExecuteWindow);
            methodExecuteViewModel.SetDelegateGetGenMethodInfoes(onGetGenMethodInfoes);
            methodExecuteViewModel.SetDelegateExecute(onExecute);
            methodExecuteViewModel.SetDelegateAfterExecuteFinish(onAfterExecuteFinish);
            methodExecuteViewModel.ReloadMethodSource();
            methodExecuteWindow.DataContext = new MethodExecuteWindowViewModel(methodExecuteViewModel);

            bool dialogResult = await methodExecuteWindow.ShowDialog<bool>((Window)owner);

            //var vm = new ControlPanelViewModel(this.iControlPanelCallBack, controlPanel.Commands);
            //var window = new ControlPanelWindow()
            //{
            //    DataContext = vm,
            //    Width = controlPanelWidth,
            //    Height = controlPanelHeight,
            //    WindowStartupLocation = WindowStartupLocation.CenterScreen,
            //    CanResize = false
            //};
            //vm.SetViewReference(window);
            //await window.ShowDialog((Window)owner);


            //switch (controlPanelID)
            //{
            //    case ControlPanelConst.ControlPanelID_Test:

            //        break;
            //    case ControlPanelConst.ControlPanelID_Test1:
            //        var vm1 = new ControlPanel1ViewModel(this.iControlPanelCallBack);
            //        var window1 = new ControlPanel1Window()
            //        {
            //            DataContext = vm1,
            //            Width = 650,
            //            Height = 368,
            //            WindowStartupLocation = WindowStartupLocation.CenterScreen,
            //            CanResize = false
            //        };
            //        vm1.SetViewReference(window1);
            //        await window1.ShowDialog((Window)owner);
            //        break;
            //    default:
            //        break;
            //}
        }

        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="methodID"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private GFBaseTypeParamValueList onExecute(string methodID, GFBaseTypeParamValueList param)
        {
            var response = iControlPanelCallBack.ExecNormalCtlCmd(methodID, param);
            return response;
        }

        /// <summary>
        /// 获取命令列表
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private GenMethodInfoList onGetGenMethodInfoes()
        {
            GenMethodInfoList genMethodInfos = new GenMethodInfoList();
            if (curControlPanelDefInfo == null)
                return genMethodInfos;
            foreach (var gFMethodDefInfo in curControlPanelDefInfo.Commands)
            {
                GenMethodInfo genMethodInfo = GenMethodInfo.GetGenMethodInfo(gFMethodDefInfo);
                genMethodInfos.Add(genMethodInfo);
            }
            return genMethodInfos;
        }
        /// <summary>
        /// 命令执行完成
        /// </summary>
        /// <param name="genMethodInfo"></param>
        private void onAfterExecuteFinish(GenMethodInfo genMethodInfo)
        {
            //无需保存命令
        }

        #endregion
    }
}