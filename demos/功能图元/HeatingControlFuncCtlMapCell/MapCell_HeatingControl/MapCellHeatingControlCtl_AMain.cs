using Avalonia.Media.Imaging;
using Avalonia.Platform;
using GF_Gereric;
using Griffins.Map;
using Griffins.Map.UI;
using System;

namespace GKG.Map.HeatingControlFuncCtlMapCell
{
    /// <summary>
    /// 加热控制图元主入口类
    /// 定义图元的基本信息和创建工厂方法
    /// </summary>
    [MapCellKindCategory(HeatingControlMapCellGroup.HeatingControlMapCellGroupID_Str)]
    [FunctionalMapCell("{F1B25EC4-DBE9-45DD-9E5B-4A9D3C0C8F76}")]
    class MapCellHeatingControlCtl_AMain : GriffinsPluginMngClass, IFunctionalMapCell
    {
        /// <summary>
        /// 图标静态实例
        /// </summary>
        private static Bitmap bitmap = null;

        /// <summary>
        /// 静态构造函数，加载图元图标
        /// </summary>
        static MapCellHeatingControlCtl_AMain()
        {
            var assetLoader = AssetLoader.Open;
            var uri = new Uri("avares://GKG.Map.HeatingControlFuncCtlMapCell/Assets/Images/HeatingControl.png");
            using (var stream = AssetLoader.Open(uri))
            {
                bitmap = new Bitmap(stream); // 加载为Bitmap
            }
        }

        /// <summary>
        /// 初始化插件（未实现）
        /// </summary>
        /// <param name="pluginFileName">插件文件名</param>
        void IFunctionalMapCell.Init(string pluginFileName)
        {
        }

        /// <summary>
        /// 获取默认宽度
        /// </summary>
        int IFunctionalMapCell.DefaultWidth
        {
            get { return 1080; }
        }

        /// <summary>
        /// 获取默认高度
        /// </summary>
        int IFunctionalMapCell.DefaultHeight
        {
            get { return 750; }
        }

        /// <summary>
        /// 获取图元图标
        /// </summary>
        Bitmap IFunctionalMapCell.Ico
        {
            get { return bitmap; }
        }

        /// <summary>
        /// 获取图元类型名称
        /// </summary>
        string IFunctionalMapCell.MapCellKindName
        {
            get { return ResourceA.HeatingControl; }
        }

        /// <summary>
        /// 创建设计时图元实例
        /// </summary>
        /// <param name="mapCellID">图元ID</param>
        /// <param name="mapCellName">图元名称</param>
        /// <returns>设计时图元实例</returns>
        IFunctionalMapCellDesigntime IFunctionalMapCell.CreateMapCellDesigntime(MapObjID mapCellID, string mapCellName)
        {
            return new MapCellHeatingControlCtlDesigntime(mapCellID, mapCellName);
        }

        /// <summary>
        /// 创建运行时图元对象实例
        /// </summary>
        /// <param name="mapCellID">图元ID</param>
        /// <param name="mapCellName">图元名称</param>
        /// <returns>运行时图元对象实例</returns>
        IFunctionalMapCellObject IFunctionalMapCell.CreateMapCellObj(MapObjID mapCellID, string mapCellName)
        {
            return new MapCellHeatingControlCtlObj(mapCellID, mapCellName);
        }
    }
}
