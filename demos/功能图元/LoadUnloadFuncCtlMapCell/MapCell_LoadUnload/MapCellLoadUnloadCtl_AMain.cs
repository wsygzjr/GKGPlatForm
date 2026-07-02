using Avalonia.Media.Imaging;
using Avalonia.Platform;
using GF_Gereric;
using Griffins.Map;
using Griffins.Map.UI;
using System;

namespace GKG.Map.LoadUnloadFuncCtlMapCell
{
    /// <summary>
    /// 上下料功能图元主入口类 (Plugin Main Entry)
    /// 为底层的 Griffins 工具箱提供图元基本信息和实例（运行时/设计时）的创建入口。
    /// </summary>
    [MapCellKindCategory(LoadUnloadMapCellGroup.LoadUnloadMapCellGroupID_Str)]
    [FunctionalMapCell("{0BE064D7-9B51-4761-A4B5-F53D7EED96D5}")]
    public class MapCellLoadUnloadCtl_AMain : GriffinsPluginMngClass, IFunctionalMapCell
    {
        /// <summary>
        /// 工具箱图标静态缓存，避免重复加载浪费内存
        /// </summary>
        private static Bitmap bitmap = null!;

        /// <summary>
        /// 静态构造函数加载工具箱图标，保证线程安全且只执行一次
        /// </summary>
        static MapCellLoadUnloadCtl_AMain()
        {
            try
            {
                var uri = new Uri("avares://GKG.Map.LoadUnloadFuncCtlMapCell/Assets/Images/LoadUnload.png");
                using (var stream = AssetLoader.Open(uri))
                {
                    bitmap = new Bitmap(stream);
                }
            }
            catch (Exception ex)
            {
                // 健壮性防御：防止因图片资源丢失（被误删或未包含进程序集）导致整个插件发生 TypeInitializationException 崩溃
                System.Diagnostics.Debug.WriteLine($"上下料图元工具箱图标加载失败: {ex.Message}");
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
        int IFunctionalMapCell.DefaultWidth => 710;

        /// <summary>
        /// 图元拖拽到画布上的默认高度
        /// </summary>
        int IFunctionalMapCell.DefaultHeight => 410;

        /// <summary>
        /// 图元在工具箱中显示的图标
        /// </summary>
        Bitmap IFunctionalMapCell.Ico => bitmap;

        /// <summary>
        /// 图元种类名称 (供多语言资源映射)
        /// </summary>
        string IFunctionalMapCell.MapCellKindName => ResourceA.LoadUnload;

        /// <summary>
        /// 创建并返回组态设计时 (Design-time) 的图元实例包装对象
        /// </summary>
        IFunctionalMapCellDesigntime IFunctionalMapCell.CreateMapCellDesigntime(MapObjID mapCellID, string mapCellName)
            => new MapCellLoadUnloadCtlDesigntime(mapCellID, mapCellName);

        /// <summary>
        /// 创建并返回系统运行时 (Run-time) 的图元核心控制对象
        /// </summary>
        IFunctionalMapCellObject IFunctionalMapCell.CreateMapCellObj(MapObjID mapCellID, string mapCellName)
            => new MapCellLoadUnloadCtlObj(mapCellID, mapCellName);

        #endregion
    }
}