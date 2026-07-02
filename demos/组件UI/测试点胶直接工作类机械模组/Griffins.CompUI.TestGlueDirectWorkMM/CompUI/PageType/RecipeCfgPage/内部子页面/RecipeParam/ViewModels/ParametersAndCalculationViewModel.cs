using Avalonia.Controls;
using Griffins.Map.UI;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.Area;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.BasicParameters;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.DatumPoint;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.GlueDispensingStyle;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.Plan;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.TemplateSubPage;
using ReactiveUI;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels
{
    /// <summary>
    /// 工艺参数与计算轨迹子页面的视图模型
    /// </summary>
    public class ParametersAndCalculationViewModel : ReactiveObject
    {
        /// <summary>
        /// 内部子页面配置信息
        /// </summary>
        private byte[]? _cfgInfo;
        private ParametersAndCalculationCfgInfo _recipeparamCfgInfo;
        /// <summary>
        /// 值改变事件
        /// </summary>
        public EventHandler? AfterModified;
        /// <summary>
        /// 基础信息参数配置-视图模型
        /// </summary>
        public BasicParametersViewModel BasicParametersViewModel { get; }
        /// <summary>
        /// 点胶样式配置-视图模型
        /// </summary>
        public GlueDispensingStyleCfgViewModel GlueDispensingStyleCfgViewModel { get; }
        /// <summary>
        /// 模板配置列表-视图模型
        /// </summary>
        public TemplateCfgListViewModel TemplateCfgListViewModel { get; }

        /// <summary>
        /// 区域配置参数-视图模型
        /// </summary>
        public AreaConfigViewModel AreaConfigViewModel { get; }

        /// <summary>
        /// 方案配置参数
        /// </summary>
        public PlanConfigViewModel PlanConfigViewModel { get; }

        /// <summary>
        /// 基准点参数配置
        /// </summary>
        public DatumPointConfigInfoViewModel DatumPointConfigInfoViewModel { get; }
        /// <summary>
        /// 构造函数
        /// </summary>
        public ParametersAndCalculationViewModel()
        {
            _recipeparamCfgInfo = new ParametersAndCalculationCfgInfo();

            BasicParametersViewModel = new BasicParametersViewModel();
            GlueDispensingStyleCfgViewModel = new GlueDispensingStyleCfgViewModel();
            CacheDataExchange.SetGlueDispensingStyleCfgViewModel(GlueDispensingStyleCfgViewModel);
            TemplateCfgListViewModel = new TemplateCfgListViewModel();
            CacheDataExchange.SetTemplateCfgListViewModel(TemplateCfgListViewModel);

            AreaConfigViewModel=new AreaConfigViewModel();
            PlanConfigViewModel=new PlanConfigViewModel();
            DatumPointConfigInfoViewModel = new DatumPointConfigInfoViewModel();
            // 订阅值变更事件
            subscribeValueChanges();
        }
        /// <summary>
        /// 设置视图引用
        /// </summary>
        public void SetViewReference(Control view)
        {
            BasicParametersViewModel.SetViewReference(view);
            GlueDispensingStyleCfgViewModel.SetViewReference(view);
            TemplateCfgListViewModel.SetViewReference(view);
            AreaConfigViewModel.SetViewReference(view);
            PlanConfigViewModel.SetViewReference(view);
            DatumPointConfigInfoViewModel.SetViewReference(view);
        }
        #region 值改变事件
        /// <summary>
        /// 订阅值改变事件
        /// </summary>
        private void subscribeValueChanges()
        {
            BasicParametersViewModel.AfterModified += onAfterModified;
            GlueDispensingStyleCfgViewModel.AfterModified += onAfterModified;
            TemplateCfgListViewModel.AfterModified += onAfterModified;
            AreaConfigViewModel.AfterModified += onAfterModified;
            PlanConfigViewModel.AfterModified += onAfterModified;
            DatumPointConfigInfoViewModel.AfterModified += onAfterModified;
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

        #region 内部子页面接口

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="cfgInfo">内部子页面配置信息</param>
        public void Init( byte[]? cfgInfo)
        {
            _cfgInfo = cfgInfo;
            loadCfgInfo(cfgInfo);
        }

        /// <summary>
        /// 内部子页面配置信息
        /// </summary>
        public byte[]? CfgInfo
        {
            get
            {
                extract(_recipeparamCfgInfo);
                _cfgInfo = _recipeparamCfgInfo.ToJsonBytes();
                return _cfgInfo;
            }
        }

        /// <summary>
        /// 提取配置参数
        /// </summary>
        /// <param name="recipeparamcfgCfgInfo"></param>
        private void extract(ParametersAndCalculationCfgInfo recipeparamcfgCfgInfo)
        {
            BasicParametersViewModel.CopyTo(recipeparamcfgCfgInfo.BasicParametersCfgInfo);
            GlueDispensingStyleCfgViewModel.CopyTo(recipeparamcfgCfgInfo.DispensingStyleCfgInfo);
            TemplateCfgListViewModel.CopyTo(recipeparamcfgCfgInfo.TemplateCfgInfoes);
            AreaConfigViewModel.CopyTo(recipeparamcfgCfgInfo.AreaConfigInfo);
            PlanConfigViewModel.CopyTo(recipeparamcfgCfgInfo.PlanConfigInfo);
            DatumPointConfigInfoViewModel.CopyTo(recipeparamcfgCfgInfo.DatumPointConfigInfo);

        }

        /// <summary>
        /// 加载配置信息
        /// </summary>
        /// <param name="cfgInfo"></param>
        private void loadCfgInfo(byte[]? cfgInfo)
        {
            if (cfgInfo != null)
            {
                try
                {
                    _recipeparamCfgInfo.FromJSonBytes(cfgInfo);
                }
                catch
                {
                    _recipeparamCfgInfo = new ParametersAndCalculationCfgInfo();
                }
            }
            BasicParametersViewModel.CopyFrom(_recipeparamCfgInfo.BasicParametersCfgInfo);
            GlueDispensingStyleCfgViewModel.CopyFrom(_recipeparamCfgInfo.DispensingStyleCfgInfo);
            TemplateCfgListViewModel.CopyFrom(_recipeparamCfgInfo.TemplateCfgInfoes);
            AreaConfigViewModel.CopyFrom(_recipeparamCfgInfo.AreaConfigInfo);
            PlanConfigViewModel.CopyFrom(_recipeparamCfgInfo.PlanConfigInfo);
            DatumPointConfigInfoViewModel.CopyFrom(_recipeparamCfgInfo.DatumPointConfigInfo);
        }
        #endregion
    }
}