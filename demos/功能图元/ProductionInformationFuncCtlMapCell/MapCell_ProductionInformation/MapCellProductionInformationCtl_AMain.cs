using Avalonia.Media.Imaging;
using Avalonia.Platform;
using GF_Gereric;
using Griffins.Map;
using Griffins.Map.UI;
using System;

namespace GKG.Map.ProductionInformationFuncCtlMapCell
{
    /// <summary>
    /// 生产信息功能图元主入口类 (Plugin Main Entry)
    /// 为底层的 Griffins 提供图元基本信息和实例（运行时/设计时）的创建入口。
    /// </summary>
    [MapCellKindCategory(ProductionInformationMapCellGroup.ProductionInformationMapCellGroupID_Str)]
    [FunctionalMapCell("{34FF961A-938B-49E9-B778-142C5CDA5661}")]
    public class MapCellProductionInformationCtl_AMain : GriffinsPluginMngClass, IFunctionalMapCell
    {
        /// <summary>
        /// 图元图标静态缓存，避免重复加载浪费内存
        /// </summary>
        private static Bitmap bitmap = null!;

        /// <summary>
        /// 静态构造函数加载图元图标，保证线程安全且只执行一次
        /// </summary>
        static MapCellProductionInformationCtl_AMain()
        {
            try
            {
                var uri = new Uri("avares://GKG.Map.ProductionInformationFuncCtlMapCell/Assets/Images/ctr-Pressure.png");
                using var stream = AssetLoader.Open(uri);
                bitmap = new Bitmap(stream);
            }
            catch (Exception ex)
            {
                // 健壮性防御：防止因图片资源丢失（被误删或生成操作未设置为 AvaloniaResource）
                // 导致整个插件发生 TypeInitializationException 崩溃，同时输出具体错误便于排查。
                System.Diagnostics.Debug.WriteLine($"[生产信息图元] 工具箱图标加载失败: {ex.Message}");
            }
        }

        #region IFunctionalMapCell 成员实现

        /// <summary>
        /// 初始化插件，框架加载时调用
        /// </summary>
        void IFunctionalMapCell.Init(string pluginFileName) { }

        /// <summary>
        /// 图元拖拽到画布上的默认宽度
        /// </summary>
        int IFunctionalMapCell.DefaultWidth => 760;

        /// <summary>
        /// 图元拖拽到画布上的默认高度
        /// </summary>
        int IFunctionalMapCell.DefaultHeight => 410;

        /// <summary>
        /// 图元在组态工具栏显示的图标
        /// </summary>
        Bitmap IFunctionalMapCell.Ico => bitmap;

        /// <summary>
        /// 图元名称 (供多语言资源映射)
        /// </summary>
        string IFunctionalMapCell.MapCellKindName => ResourceA.ProductionInformation;

        /// <summary>
        /// 创建并返回组态设计时 (Design-time) 的图元实例包装对象
        /// </summary>
        IFunctionalMapCellDesigntime IFunctionalMapCell.CreateMapCellDesigntime(MapObjID mapCellID, string mapCellName)
            => new MapCellProductionInformationCtlDesigntime(mapCellID, mapCellName);

        /// <summary>
        /// 创建并返回系统运行时 (Run-time) 的图元核心控制对象
        /// </summary>
        IFunctionalMapCellObject IFunctionalMapCell.CreateMapCellObj(MapObjID mapCellID, string mapCellName)
            => new MapCellProductionInfoCtlObj(mapCellID, mapCellName);

        #endregion
    }
}