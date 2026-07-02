using Avalonia.Controls;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.GlueDispensingStyle;
using ReactiveUI;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.GlueDispensingStyle
{
    /// <summary>
    /// 点胶样式配置-视图模型
    /// </summary>
    public class GlueDispensingStyleCfgViewModel : ReactiveObject
    {
        /// <summary>
        ///点胶样式变更事件
        /// </summary>
        public event EventHandler<GlueDispensingStyleChangedEventArgs>? StyleChanged;
          /// <summary>
        /// 值改变事件
        /// </summary>
        public EventHandler? AfterModified;
        /// <summary>
        /// 点胶点样式参数配置
        /// </summary>
        public DispensingPointStyleCfgViewModel DispensingPointStyleCfgViewModel { set; get; }
        /// <summary>
        /// 点胶线样式参数配置
        /// </summary>
        public DispensingLineStyleCfgViewModel DispensingLineStyleCfgViewModel { set; get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public GlueDispensingStyleCfgViewModel()
        {
            DispensingPointStyleCfgViewModel = new DispensingPointStyleCfgViewModel();
            DispensingLineStyleCfgViewModel = new DispensingLineStyleCfgViewModel();
           
            // 订阅值变更事件
            subscribeValueChanges();
        }


        /// <summary>
        /// 从数据模型复制数据到ViewModel
        /// </summary>
        /// <param name="cfgInfo"></param>
        public void CopyFrom(DispensingStyleCfgInfo cfgInfo)
        {
            DispensingPointStyleCfgViewModel.CopyFrom(cfgInfo.DispensingPointStyleCfgInfo);
            DispensingLineStyleCfgViewModel.CopyFrom(cfgInfo.DispensingLineStyleCfgInfo);
        }

        /// <summary>
        /// 从ViewModel复制数据到数据模型
        /// </summary>
        /// <param name="cfgInfo"></param>
        public void CopyTo(DispensingStyleCfgInfo cfgInfo)
        {
            DispensingPointStyleCfgViewModel.CopyTo(cfgInfo.DispensingPointStyleCfgInfo);
            DispensingLineStyleCfgViewModel.CopyTo(cfgInfo.DispensingLineStyleCfgInfo);
        }
        /// <summary>
        /// 设置视图引用（用于弹窗、对话框等UI操作）
        /// </summary>
        public void SetViewReference(Control view)
        {
            DispensingPointStyleCfgViewModel.SetViewReference(view);
            DispensingLineStyleCfgViewModel.SetViewReference(view);
        }
        private void DispensingPointStyleCfgViewModel_StyleChanged(object? sender, EventArgs e)
        {
            StyleChanged?.Invoke(this, new GlueDispensingStyleChangedEventArgs(GlueDispensingStyleChangedType.Point));
        }
        private void DispensingLineStyleCfgViewModel_StyleChanged(object? sender, EventArgs e)
        {
            StyleChanged?.Invoke(this, new GlueDispensingStyleChangedEventArgs(GlueDispensingStyleChangedType.Line));
        }
        #region 值改变事件
        /// <summary>
        /// 订阅值改变事件
        /// </summary>
        private void subscribeValueChanges()
        {
            DispensingPointStyleCfgViewModel.AfterModified += onAfterModified;
            DispensingLineStyleCfgViewModel.AfterModified += onAfterModified;

            DispensingPointStyleCfgViewModel.StyleItemChanged += DispensingPointStyleCfgViewModel_StyleChanged;
            DispensingLineStyleCfgViewModel.StyleItemChanged += DispensingLineStyleCfgViewModel_StyleChanged;
        }

       
        /// <summary>
        /// 值改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onAfterModified(object? sender, EventArgs e)
        {
            AfterModified?.Invoke(sender, e);
        }
        #endregion
    }

    /// <summary>
    /// 点胶样式变更事件参数
    /// </summary>
    public class GlueDispensingStyleChangedEventArgs : EventArgs
    {
        /// <summary>
        /// 点胶样式变更类型
        /// </summary>
        public GlueDispensingStyleChangedType ChangedType { get; }

        public GlueDispensingStyleChangedEventArgs(GlueDispensingStyleChangedType changedType)
        {
            ChangedType = changedType;
        }
    }
    /// <summary>
    /// 点胶样式变更类型
    /// </summary>
    public enum GlueDispensingStyleChangedType
    {
        Point,
        Line
    }
}