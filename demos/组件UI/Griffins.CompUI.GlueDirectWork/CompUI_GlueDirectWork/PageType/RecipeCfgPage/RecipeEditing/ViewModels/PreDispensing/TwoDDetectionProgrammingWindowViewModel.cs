using Avalonia.Controls;
using GKG.UI.General;
using Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.Models;
using ReactiveUI;
using System;

namespace Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.ViewModels
{
    /// <summary>
    /// 2D检测编程弹窗-视图模型
    /// </summary>
    public class TwoDDetectionProgrammingWindowViewModel : ReactiveObject
    {
        #region 私有字段

        private bool? _dialogResult;

        #endregion

        #region 视图模型

        /// <summary>
        /// 相机与光源控制
        /// </summary>
        public CameraLightSourceCtrViewModel CameraLightSourceCtrViewModel { set; get; }
        /// <summary>
        /// 图像预处理参数 视图模型
        /// </summary>
        public ImagePreProcessParamViewModel ImagePreProcessParamViewModel { get; }
        /// <summary>
        /// 通过条件编译 视图模型
        /// </summary>
        public PassConditionParamViewModel PassConditionParamViewModel { get; }
        /// <summary>
        /// 检测结果界面 视图模型
        /// </summary>
        public DetectionResultViewModel DetectionResultViewModel { get; }

        #endregion

        #region 值改变事件

        /// <summary>
        /// 值改变事件
        /// </summary>
        public EventHandler? AfterModified;

        #endregion

        #region 属性
        /// <summary>
        /// 对话框结果（true:保存，false:取消，null:未操作）
        /// </summary>
        public bool? DialogResult
        {
            get => _dialogResult;
            set => this.RaiseAndSetIfChanged(ref _dialogResult, value);
        }

        #endregion

        /// <summary>
        /// 构造函数（初始化组件、命令）
        /// </summary>
        public TwoDDetectionProgrammingWindowViewModel()
        {
            //初始化视图模型
            CameraLightSourceCtrViewModel = new CameraLightSourceCtrViewModel();
            ImagePreProcessParamViewModel = new();
            PassConditionParamViewModel = new();
            DetectionResultViewModel = new();

            // 绑定委托
            DetectionResultViewModel.GetLatestImagePreProcessCfg += GetImagePreProcessCfg;
            DetectionResultViewModel.GetLatestScriptParamCfg += GetScriptParamCfg;
            DetectionResultViewModel.GetLatestPassConditionCfg += GetPassConditionCfg;
            DetectionResultViewModel.LoadSelectedScriptCfg += LoadSelectedScriptCfg;
            DetectionResultViewModel.ResetScroptParamViewModel += PassConditionParamViewModel.ScriptParamViewModel.ResetScriptParamViewModel;

            // 订阅值改变事件
            subscribeValueChanges();
        }

        #region 私有方法

        /// <summary>
        /// 获取最新的图像预处理配置
        /// </summary>
        /// <returns>图像预处理配置</returns>
        private ImagePreProcessCfgInfo GetImagePreProcessCfg()
        {
            var cfg = new ImagePreProcessCfgInfo();
            ImagePreProcessParamViewModel.CopyTo(cfg);
            return cfg;
        }

        /// <summary>
        /// 获取最新的脚本参数配置
        /// </summary>
        /// <returns>脚本参数配置</returns>
        private ScriptParamCfgInfo GetScriptParamCfg()
        {
            var cfg = new ScriptParamCfgInfo();
            PassConditionParamViewModel.ScriptParamViewModel.CopyTo(cfg);
            return cfg;
        }

        /// <summary>
        /// 获取最新的通过条件配置
        /// </summary>
        /// <returns>通过条件配置</returns>
        private PassConditionCfgInfo GetPassConditionCfg()
        {
            var cfg = new PassConditionCfgInfo();
            PassConditionParamViewModel.CopyTo(cfg);
            return cfg;
        }

        /// <summary>
        /// 加载选中的脚本配置到编辑界面
        /// </summary>
        /// <param name="imgCfg">图像预处理配置</param>
        /// <param name="scriptCfg">脚本参数配置</param>
        private void LoadSelectedScriptCfg(ImagePreProcessCfgInfo? imgCfg, ScriptParamCfgInfo? scriptCfg, PassConditionCfgInfo? passCfg)
        {
            if (imgCfg != null)
                ImagePreProcessParamViewModel.CopyFrom(imgCfg);

            if (scriptCfg != null)
            {
                PassConditionParamViewModel.PassCondition = scriptCfg.ParamType;//更新通过条件
                PassConditionParamViewModel.ScriptParamViewModel.CopyFrom(scriptCfg);
            }

            if (passCfg != null)
            {
                PassConditionParamViewModel.CopyFrom(passCfg);
            }
        }

        #endregion

        #region 值改变事件订阅与处理

        /// <summary>
        /// 订阅值改变事件
        /// </summary>
        private void subscribeValueChanges()
        {
            ImagePreProcessParamViewModel.AfterModified += onAfterModified;
            PassConditionParamViewModel.AfterModified += onAfterModified;
            DetectionResultViewModel.AfterModified += onAfterModified;
        }

        /// <summary>
        /// 值改变事件
        /// </summary>
        private void onAfterModified(object? sender, EventArgs e)
        {
            AfterModified?.Invoke(sender, e);
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 从数据模型复制数据到ViewModel ff:疑问
        /// </summary>
        public void CopyFrom(TwoDDetectionProgrammingCfgInfo model)
        {
            ImagePreProcessParamViewModel.CopyFrom(model.ImagePreProcessCfgInfo);
            PassConditionParamViewModel.CopyFrom(model.PassConditionCfgInfo);
            DetectionResultViewModel.CopyFrom(model.DetectionResultCfgInfo);
        }

        /// <summary>
        /// 从ViewModel复制数据到数据模型
        /// </summary>
        public void CopyTo(TwoDDetectionProgrammingCfgInfo model)
        {
            ImagePreProcessParamViewModel.CopyTo(model.ImagePreProcessCfgInfo);
            PassConditionParamViewModel.CopyTo(model.PassConditionCfgInfo);
            DetectionResultViewModel?.CopyTo(model.DetectionResultCfgInfo);
        }

        /// <summary>
        /// 设置视图引用（用于弹窗）
        /// </summary>
        public void SetViewReference(Control view)
        {
            ImagePreProcessParamViewModel.SetViewReference(view);
            PassConditionParamViewModel.SetViewReference(view);
            PassConditionParamViewModel.ScriptParamViewModel.SetViewReference(view);
            DetectionResultViewModel.SetViewReference(view);
        }

        #endregion
    }
}
