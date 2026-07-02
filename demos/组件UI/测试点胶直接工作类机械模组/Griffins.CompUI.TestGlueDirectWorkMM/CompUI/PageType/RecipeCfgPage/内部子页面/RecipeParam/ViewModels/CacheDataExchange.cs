using Griffins.UI;
using System;
using System.Linq;
using System.Collections.Generic;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.Area;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.GlueDispensingStyle;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.TemplateSubPage.Command;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.Area;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.GlueDispensingStyle;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.TemplateSubPage;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.TemplateSubPage.Command;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.TemplateSubPage.SubTemplate;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.TemplateSubPage.TrajectorySequence;
using System.Collections.ObjectModel;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels
{
    /// <summary>
    /// 缓存数据交换
    /// </summary>
    internal class CacheDataExchange
    {
        static CacheDataExchange()
        {
            TemplateCfgViewModels = new ObservableCollection<TemplateCfgViewModel>();
        }
        #region 点胶样式配置数据源
        /// <summary>
        /// 点胶样式配置-视图模型
        /// </summary>
        private static GlueDispensingStyleCfgViewModel? _glueDispensingStyleCfgViewModel;
        public static GlueDispensingStyleCfgViewModel? GlueDispensingStyleCfgViewModel
        {
            get
            {
                return _glueDispensingStyleCfgViewModel;
            }
        }
        public static void SetGlueDispensingStyleCfgViewModel(GlueDispensingStyleCfgViewModel glueDispensingStyleCfgViewModel)
        {
            _glueDispensingStyleCfgViewModel = glueDispensingStyleCfgViewModel;
        }
        /// <summary>
        /// 订阅样式项改变事件
        /// </summary>
        /// <param name="styleItemChanged"></param>
        public static void SubscribStyleChanged(EventHandler<GlueDispensingStyleChangedEventArgs>? styleItemChanged)
        {
            if (_glueDispensingStyleCfgViewModel != null)
            {
                _glueDispensingStyleCfgViewModel.StyleChanged += styleItemChanged;
            }
        }

        /// <summary>
        /// 获取点胶样式配置信息
        /// </summary>
        /// <returns></returns>
        public static DispensingStyleCfgInfo GetDispensingStyleCfgInfo()
        {
            if (_glueDispensingStyleCfgViewModel == null)
                throw new Exception("未设置点胶样式配置视图模型");
            DispensingStyleCfgInfo dispensingStyleCfgInfo = new DispensingStyleCfgInfo();
            _glueDispensingStyleCfgViewModel.CopyTo(dispensingStyleCfgInfo);
            return dispensingStyleCfgInfo;
        }
        #endregion


        #region 模板信息数据源

        #region 模板信息

        /// <summary>
        /// 模板配置列表 视图模型
        /// </summary>
        public static TemplateCfgListViewModel? TemplateCfgListViewModel { set; get; }
        /// <summary>
        /// 模板信息数据源
        /// </summary>
        public static ObservableCollection<TemplateCfgViewModel> TemplateCfgViewModels { set; get; }
        public static void SetTemplateCfgListViewModel(TemplateCfgListViewModel templateCfgListViewModel)
        {
            TemplateCfgListViewModel = templateCfgListViewModel;
            TemplateCfgViewModels = TemplateCfgListViewModel.TemplateCfgViewModels;
        }
        /// <summary>
        /// 订阅模板项改变事件
        /// </summary>
        /// <param name="templateItemChanged"></param>
        public static void SubscribTemplateChanged(EventHandler<EventArgs>? templateItemChanged)
        {
            if (TemplateCfgListViewModel != null)
            {
                TemplateCfgListViewModel.TemplateItemChanged += templateItemChanged;
            }
        }
        /// <summary>
        /// 获取模板信息
        /// </summary>
        /// <returns></returns>
        public static List<ComBoxItem> GetTemplates()
        {
            var templateItems = CacheDataExchange.TemplateCfgViewModels.ToList();
            List<ComBoxItem> comBoxItems = new List<ComBoxItem>();
            foreach (var item in templateItems)
            {
                comBoxItems.Add(new ComBoxItem()
                {
                    Value = item.TemplateID,
                    DisplayName = item.TemplateName
                });
            }
            return comBoxItems;
        }
        #endregion

        #region 子模板信息
        /// <summary>
        /// 获取指定模板ID的子模板项信息列表
        /// </summary>
        public static List<SubTemplateItemViewModel> GetSubTemplatePointInfoes(Guid templateID)
        {
            var tempModel = TemplateCfgViewModels.FirstOrDefault(o => o.TemplateID == templateID);
            if (tempModel == null)
                return new List<SubTemplateItemViewModel>();
            return tempModel.SubTemplateConfigViewModel.SubTemplateListViewModel.ItemsSource.ToList();
        }
        /// <summary>
        /// 订阅指定模板ID的子模板项改变事件
        /// </summary>
        /// <param name="templateID"></param>
        /// <param name="subTemplateItemChanged"></param>
        public static void SubscribSubTemplateChanged(Guid templateID, EventHandler<EventArgs>? subTemplateItemChanged)
        {
            if (TemplateCfgListViewModel != null)
            {
                TemplateCfgViewModel? templateCfgViewModel = TemplateCfgListViewModel.TemplateCfgViewModels.FirstOrDefault(o => o.TemplateID == templateID);
                if (templateCfgViewModel != null)
                    templateCfgViewModel.SubTemplateConfigViewModel.SubTemplateListViewModel.SubTemplateItemChanged += subTemplateItemChanged;
            }
        }
        #endregion

        #region 轨迹序列数据源

        /// <summary>
        /// 获取指定模板ID的轨迹序列列表
        /// </summary>
        public static List<TrajectorySequenceModel> GetTrajectorySequenceModel(Guid templateID)
        {
            var tempModel = TemplateCfgViewModels.FirstOrDefault(o => o.TemplateID == templateID);
            if (tempModel == null)
                return new List<TrajectorySequenceModel>();
            return tempModel.TrajectorySequenceListViewModel.ItemsSource.ToList();
        }
        /// <summary>
        /// 订阅指定模板ID的轨迹序列项改变事件
        /// </summary>
        /// <param name="templateID"></param>
        /// <param name="subTemplateItemChanged"></param>
        public static void SubscribTrajectoryChanged(Guid templateID, EventHandler<EventArgs>? TrajectoryItemChanged)
        {
            if (TemplateCfgListViewModel != null)
            {
                TemplateCfgViewModel? templateCfgViewModel = TemplateCfgListViewModel.TemplateCfgViewModels.FirstOrDefault(o => o.TemplateID == templateID);
                if (templateCfgViewModel != null)
                    templateCfgViewModel.TrajectorySequenceListViewModel.TrajectoryItemChanged += TrajectoryItemChanged;
            }
        }
        #endregion

        #region 指令序列数据源

        /// <summary>
        /// 获取指定模板ID的指令序列列表
        /// </summary>
        public static List<CommandSequenceModel> GetCommandSequenceModel(Guid templateID)
        {
            var tempModel = TemplateCfgViewModels.FirstOrDefault(o => o.TemplateID == templateID);
            if (tempModel == null)
                return new List<CommandSequenceModel>();
            return tempModel.CommandSequenceListViewModel.ItemsSource.ToList();
        }

        #endregion

        #endregion

        #region 区域信息

        /// <summary>
        /// 区域信息列表 视图模型
        /// </summary>
        public static AreaListViewModel? AreaListViewModel { set; get; }
        /// <summary>
        /// 设置区域信息列表对象
        /// </summary>
        /// <param name="areaListViewModel"></param>
        public static void SetAreaListViewModel(AreaListViewModel areaListViewModel)
        {
            AreaListViewModel = areaListViewModel;
        }
        /// <summary>
        /// 订阅区域项改变事件
        /// </summary>
        /// <param name="areaItemChanged"></param>
        public static void SubscribAreaListItemChanged(EventHandler<EventArgs>? areaItemChanged)
        {
            if (AreaListViewModel != null)
            {
                AreaListViewModel.AreaItemChanged += areaItemChanged;
            }
        }
        /// <summary>
        /// 获取区域列表信息
        /// </summary>
        /// <returns></returns>
        public static AreaInfoList GetAreaListes()
        {
            AreaInfoList areaInfos = new AreaInfoList();
            if(AreaListViewModel==null)
                return areaInfos;   
            AreaListViewModel.CopyTo(areaInfos);
            return areaInfos;
        }
        #endregion

        #region 阀信息（模拟）

        /// <summary>
        /// 获取阀信息
        /// </summary>
        public static List<ValveInfo> GetValveInfoes()
        {
            var valveInfoes = new List<ValveInfo>();
            valveInfoes.Add(new ValveInfo() { ValveNumber = "Valve1", ValveName = "阀1" });
            valveInfoes.Add(new ValveInfo() { ValveNumber = "Valve2", ValveName = "阀2" });
            return valveInfoes;
        }

        #endregion

        #region 相机信息（模拟）

        /// <summary>
        /// 获取相机信息
        /// </summary>
        public static List<ValveInfo> GetCameraes()
        {
            var valveInfoes = new List<ValveInfo>();
            valveInfoes.Add(new ValveInfo() { ValveNumber = "Camera1", ValveName = "相机1" });
            return valveInfoes;
        }

        #endregion
    }
}
