using Avalonia.Media.Imaging;
using Avalonia.Platform;
using GF_Gereric;
using Griffins.Map;
using Griffins.Map.UI;
using System;

namespace GKG.Map.StatusInfoFuncCtlMapCell
{
    [MapCellKindCategory(StatusInfoMapCellGroup.StatusInfoMapCellGroupID_Str)]
    [FunctionalMapCell("{D6E9702A-EDAA-4E0F-9A98-4D0B06C5A0B5}")]
    /// <summary>
    /// 状态信息功能图元插件入口
    /// </summary>
    class MapCellStatusInfoCtl_AMain : GriffinsPluginMngClass, IFunctionalMapCell
    {
        #region 私有字段
        /// <summary>
        /// 图标静态实例
        /// </summary>
        private static Bitmap bitmap = null;
        #endregion

        #region 静态构造
        /// <summary>
        /// 静态构造函数，加载图元图标
        /// </summary>
        static MapCellStatusInfoCtl_AMain()
        {
            var uri = new Uri("avares://GKG.Map.StatusInfoFuncCtlMapCell/Assets/Images/StatusInfo.png");
            try
            {
                using (var stream = AssetLoader.Open(uri))
                {
                    bitmap = new Bitmap(stream);
                }
            }
            catch
            {
                bitmap = null;
            }
        }
        #endregion

        #region IFunctionalMapCell 成员
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
            get { return 520; }
        }

        /// <summary>
        /// 获取默认高度
        /// </summary>
        int IFunctionalMapCell.DefaultHeight
        {
            get { return 70; }
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
            get { return ResourceA.StatusInfo; }
        }

        /// <summary>
        /// 创建图控元插件设计时接口实例
        /// </summary>
        /// <param name="mapCellID">图元ID</param>
        /// <param name="mapCellName">图元名称</param>
        /// <returns>图控元插件设计时接口实例</returns>
        IFunctionalMapCellDesigntime IFunctionalMapCell.CreateMapCellDesigntime(MapObjID mapCellID, string mapCellName)
        {
            return new MapCellStatusInfoCtlDesigntime(mapCellID, mapCellName);
        }

        /// <summary>
        /// 创建图控元插件运行时接口实例
        /// </summary>
        /// <param name="mapCellID">图元ID</param>
        /// <param name="mapCellName">图元名称</param>
        /// <returns>图控元插件运行时接口实例</returns>
        IFunctionalMapCellObject IFunctionalMapCell.CreateMapCellObj(MapObjID mapCellID, string mapCellName)
        {
            return new MapCellStatusInfoCtlObj(mapCellID, mapCellName);
        }
        #endregion
    }
}
