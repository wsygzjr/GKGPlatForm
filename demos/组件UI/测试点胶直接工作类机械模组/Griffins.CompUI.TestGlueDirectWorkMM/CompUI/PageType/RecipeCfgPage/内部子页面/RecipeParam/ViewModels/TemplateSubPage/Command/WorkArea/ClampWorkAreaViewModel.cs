using Avalonia.Controls;
using Avalonia.VisualTree;
using Griffins.UI;
using Griffins.UI.General;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.TemplateSubPage.Command;
using ReactiveUI;
using System.Reactive;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.TemplateSubPage.Command
{

    /// <summary>
    /// 抓边指令工作区-视图模型
    /// </summary>
    public class ClampWorkAreaViewModel : ReactiveObject
    {
        /// <summary>
        /// 页面布局对象
        /// </summary>
        private Control? _viewReference;
        /// <summary>
        /// 初始位置-视图模型
        /// </summary>
        public CamreaPositionViewModel InitalPositionViewModel { get; }
        /// <summary>
        /// 序号（标签）
        /// </summary>
        public TextBlockViewModel SerialNumberViewModel { get; }

        

        #region 响应式属性

        /// <summary>
        /// 序号
        /// </summary>
        public int SerialNumber
        {
            get => int.Parse(SerialNumberViewModel.Text);
            set => SerialNumberViewModel.Text = value.ToString();
        }


        #endregion

        /// <summary>
        /// 抓边弹窗命令
        /// </summary>
        public ReactiveCommand<Unit, Unit> ClampWindowCommand { get; }
        /// <summary>
        /// 构造函数
        /// </summary>
        public ClampWorkAreaViewModel()
        {

            InitalPositionViewModel = new CamreaPositionViewModel();

            // 序号（标签）
            SerialNumberViewModel = new TextBlockViewModel { Text = "1" };

            ClampWindowCommand = ReactiveCommand.CreateFromTask(onClampWindowCommand);
        }

        /// <summary>
        /// 从数据模型复制数据到ViewModel
        /// </summary>
        /// <param name="command"></param>
        public void CopyFrom(ClampCommandSequence command)
        {
            if (command == null) return;
            InitalPositionViewModel.CopyFrom(command.InitalPositionInfo);


        }

        /// <summary>
        /// 从ViewModel复制数据到数据模型
        /// </summary>
        /// <param name="command"></param>
        public void CopyTo(ClampCommandSequence command)
        {
            if (command == null) return;

            InitalPositionViewModel.CopyTo(command.InitalPositionInfo);

        }
        /// <summary>
        /// 设置视图引用
        /// </summary>
        public void SetViewReference(Control view)
        {
            _viewReference = view;
        }

        #region 执行命令

        /// <summary>
        ///抓边弹窗
        /// </summary>
        private async Task onClampWindowCommand()
        {
            var parentWindow = _viewReference?.GetVisualRoot() as Window;
            if (parentWindow == null)
            {
                await MessageBox.ShowErrorDialog("错误", "无法获取窗口上下文，操作失败", _viewReference);
                return;
            }

            //var editViewModel = new ClampWindowViewModel();
            //var editWindow = new ClampWindow
            //{
            //    DataContext = editViewModel,
            //    Height=1020,
            //    Width=1920,
            //    WindowStartupLocation = WindowStartupLocation.CenterScreen
            //};

            //editViewModel.SetViewReference(editWindow);

            //var result = await editWindow.ShowDialog<bool>(parentWindow);
            //if ((result))
            //{
            //    //editViewModel.CopyTo();
            //}
        }

        #endregion

    }

}
