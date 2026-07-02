using Avalonia.Controls;
using Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.Models;
using Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.Views;
using ReactiveUI;
using System;

namespace Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.ViewModels
{
    /// <summary>
    /// 脚本界面-视图模型
    /// </summary>
    public class ScriptParamViewModel : ReactiveObject
    {
        #region 私有字段（数据源）

        /// <summary>
        /// 视图引用（用于弹窗等UI操作）
        /// </summary>
        private Control? _viewReference;
        private ScriptParamType _currentParamType;
        #endregion

        #region 属性（工作区）

        private object? _workArea;

        public object? WorkArea
        {
            get => _workArea;
            set
            {
                this.RaiseAndSetIfChanged(ref _workArea, value);
            }
        }

        /// <summary>
        /// 当前脚本参数类型
        /// </summary>
        public ScriptParamType CurrentParamType
        {
            get => _currentParamType;
            set
            {
                //更新当前类型，确保 copyto/from 的是正确的类型
                this.RaiseAndSetIfChanged(ref _currentParamType, value);
                SwitchWorkArea(value);
            }
        }
        #endregion

        #region 值改变事件

        /// <summary>
        /// 值改变事件
        /// </summary>
        public EventHandler? AfterModified;

        #endregion

        #region 视图模型

        /// <summary>
        /// 检测漏点胶-视图模型
        /// </summary>
        public MissingGlueDetectionParamViewModel MissingGlueDetectionParam { get; set; }
        /// <summary>
        /// 最大胶点面积检测-视图模型
        /// </summary>
        public MaxGlueAreaDetectionParamViewModel MaxGlueAreaDetectionParam { get; set; }
        /// <summary>
        /// 胶点直径个数偏移检测-视图模型
        /// </summary>
        public GlueDiameterCountOffsetParamViewModel GlueDiameterCountOffsetParam { get; set; }
        /// <summary>
        /// 禁用-视图模型
        /// </summary>
        public DisableChecksParamViewModel DisableChecksParam { get; set; }
        /// <summary>
        /// 总面积检测-视图模型
        /// </summary>
        public TotalAreaDetectionParamViewModel TotalAreaDetectionParam { get; set; }
        /// <summary>
        /// 胶点个数检测-视图模型
        /// </summary>
        public GlueCountDetectionParamViewModel GlueCountDetectionParam { get; set; }

        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public ScriptParamViewModel()
        {
            //初始化视图模型
            MissingGlueDetectionParam = new();
            MaxGlueAreaDetectionParam = new();
            GlueDiameterCountOffsetParam = new();
            DisableChecksParam = new();
            TotalAreaDetectionParam = new();
            GlueCountDetectionParam = new();

            // 初始化默认视图（默认为检测漏点胶）
            CurrentParamType = ScriptParamType.MissingGlueDetection;

            // 订阅子ViewModel的事件
            subscribeChildViewModelEvents();
        }

        #region 辅助方法
        /// <summary>
        /// 设置视图引用（用于弹窗等UI操作）
        /// </summary>
        public void SetViewReference(Control view)
        {
            _viewReference = view;
        }

        /// <summary>
        /// 从ViewModel复制数据到数据模型
        /// </summary>
        public void CopyFrom(ScriptParamCfgInfo model)
        {
            if (model == null) return;

            // 切换到对应的参数类型
            CurrentParamType = model.ParamType;

            // 根据类型复制具体配置
            switch (model.ParamType)
            {
                case ScriptParamType.MissingGlueDetection:
                    if (model.ParamCfg is MissingGlueDetectionCfgInfo missing)
                        MissingGlueDetectionParam.CopyFrom(missing);
                    break;
                case ScriptParamType.MaxGlueAreaDetection:
                    if (model.ParamCfg is MaxGlueAreaDetectionCfgInfo maxArea)
                        MaxGlueAreaDetectionParam.CopyFrom(maxArea);
                    break;
                case ScriptParamType.GlueDiameterCountOffset:
                    if (model.ParamCfg is GlueDiameterCountOffsetCfgInfo diameter)
                        GlueDiameterCountOffsetParam.CopyFrom(diameter);
                    break;
                case ScriptParamType.DisableChecks:
                    if (model.ParamCfg is DisableChecksCfgInfo disable)
                        DisableChecksParam.CopyFrom(disable);
                    break;
                case ScriptParamType.TotalAreaDetection:
                    if (model.ParamCfg is TotalAreaDetectionCfgInfo totalArea)
                        TotalAreaDetectionParam.CopyFrom(totalArea);
                    break;
                case ScriptParamType.GlueCountDetection:
                    if (model.ParamCfg is GlueCountDetectionCfgInfo count)
                        GlueCountDetectionParam.CopyFrom(count);
                    break;
            }
        }

        /// <summary>
        /// 从数据模型复制数据到ViewModel
        /// </summary>
        public void CopyTo(ScriptParamCfgInfo model)
        {
            if (model == null) return;

            model.ParamType = CurrentParamType;

            switch (CurrentParamType)
            {
                case ScriptParamType.MissingGlueDetection:
                    var missing = new MissingGlueDetectionCfgInfo();
                    MissingGlueDetectionParam.CopyTo(missing);
                    model.ParamCfg = missing;
                    break;
                case ScriptParamType.MaxGlueAreaDetection:
                    var maxArea = new MaxGlueAreaDetectionCfgInfo();
                    MaxGlueAreaDetectionParam.CopyTo(maxArea);
                    model.ParamCfg = maxArea;
                    break;
                case ScriptParamType.GlueDiameterCountOffset:
                    var diameter = new GlueDiameterCountOffsetCfgInfo();
                    GlueDiameterCountOffsetParam.CopyTo(diameter);
                    model.ParamCfg = diameter;
                    break;
                case ScriptParamType.DisableChecks:
                    var disable = new DisableChecksCfgInfo();
                    DisableChecksParam.CopyTo(disable);
                    model.ParamCfg = disable;
                    break;
                case ScriptParamType.TotalAreaDetection:
                    var totalArea = new TotalAreaDetectionCfgInfo();
                    TotalAreaDetectionParam.CopyTo(totalArea);
                    model.ParamCfg = totalArea;
                    break;
                case ScriptParamType.GlueCountDetection:
                    var count = new GlueCountDetectionCfgInfo();
                    GlueCountDetectionParam.CopyTo(count);
                    model.ParamCfg = count;
                    break;
            }
        }


        /// <summary>
        /// 切换工作区视图
        /// </summary>
        /// <param name="type">脚本参数类型</param>
        public void SwitchWorkArea(ScriptParamType type)
        {
            switch (type)
            {
                case ScriptParamType.MissingGlueDetection:
                    WorkArea = new MissingGlueDetectionParamView { DataContext = MissingGlueDetectionParam };
                    break;
                case ScriptParamType.MaxGlueAreaDetection:
                    WorkArea = new MaxGlueAreaDetectionParamView { DataContext = MaxGlueAreaDetectionParam };
                    break;
                case ScriptParamType.GlueDiameterCountOffset:
                    WorkArea = new GlueDiameterCountOffsetParamView { DataContext = GlueDiameterCountOffsetParam };
                    break;
                case ScriptParamType.DisableChecks:
                    WorkArea = new DisableChecksParamView { DataContext = DisableChecksParam };
                    break;
                case ScriptParamType.TotalAreaDetection:
                    WorkArea = new TotalAreaDetectionParamView { DataContext = TotalAreaDetectionParam };
                    break;
                case ScriptParamType.GlueCountDetection:
                    WorkArea = new GlueCountDetectionParamView { DataContext = GlueCountDetectionParam };
                    break;
                default:
                    WorkArea = null;
                    break;
            }
        }

        /// <summary>
        /// 重置脚本参数视图模型配置
        /// </summary>
        public void ResetScriptParamViewModel()
        {
            MissingGlueDetectionParam = new();
            MaxGlueAreaDetectionParam = new();
            GlueDiameterCountOffsetParam = new();
            DisableChecksParam = new();
            TotalAreaDetectionParam = new();
            GlueCountDetectionParam = new();
        }

        #endregion

        #region 值改变订阅

        /// <summary>
        /// 订阅子ViewModel的事件
        /// </summary>
        private void subscribeChildViewModelEvents()
        {
            MissingGlueDetectionParam.AfterModified += onAfterModified;
            MaxGlueAreaDetectionParam.AfterModified += onAfterModified;
            GlueDiameterCountOffsetParam.AfterModified += onAfterModified;
            DisableChecksParam.AfterModified += onAfterModified;
            TotalAreaDetectionParam.AfterModified += onAfterModified;
            GlueCountDetectionParam.AfterModified += onAfterModified;
        }

        /// <summary>
        /// 值改变事件
        /// </summary>
        private void onAfterModified(object? sender, EventArgs e)
        {
            AfterModified?.Invoke(sender, e);
        }

        #endregion
    }
}
