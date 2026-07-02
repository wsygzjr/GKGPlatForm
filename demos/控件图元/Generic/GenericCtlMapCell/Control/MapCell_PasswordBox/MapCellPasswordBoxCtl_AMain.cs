using Avalonia.Media.Imaging;
using Avalonia.Platform;
using GF_Gereric;
using Griffins;
using System;

namespace GKG.Map.MapCell.Generic.PasswordBox
{
    /// <summary>
    /// 密码输入框图元主入口类
    /// </summary>
    [MapCellKindCategory(GenericMapCellGroup.GenericMapCellGroupID_Str)]
    [ControlMapCell("{C2D3E4F5-1234-4AC8-BF66-281412CDE201}")]
    class MapCellPasswordBoxCtl_AMain : GriffinsPluginMngClass, IControlMapCell
    {
        private static Bitmap bitmap = null;

        static MapCellPasswordBoxCtl_AMain()
        {
            var assetLoader = AssetLoader.Open;
            // 路径格式：项目根命名空间;component/相对路径
            var uri = new Uri("avares://GKG.Map.MapCell.Generic/Assets/Images/ctr-PasswordBox.png");
            try
            {
                using (var stream = AssetLoader.Open(uri))
                {
                    bitmap = new Bitmap(stream);
                }
            }
            catch
            {
                // 如果图标不存在，使用默认图标或null
                bitmap = null;
            }
        }

        #region IMapCell 成员

        void IControlMapCell.Init(string pluginFileName)
        {
        }

        int IControlMapCell.DefaultWidth
        {
            get { return 150; }
        }

        int IControlMapCell.DefaultHeight
        {
            get { return 50; }
        }

        Bitmap IControlMapCell.Ico
        {
            get { return bitmap; }
        }

        string IControlMapCell.MapCellKindName
        {
            get { return ResourceA.PasswordBox; }
        }

        /// <summary>
        /// 创建图控元插件设计时接口实例
        /// </summary>
        /// <param name="mapCellID">图元ID</param>
        /// <param name="mapCellName">图元名称</param>
        /// <returns>图控元插件设计时接口实例</returns>
        IControlMapCellDesigntime IControlMapCell.CreateMapCellDesigntime(MapObjID mapCellID, string mapCellName)
        {
            return new MapCellPasswordBoxCtlDesigntime(mapCellID, mapCellName);
        }

        /// <summary>
        /// 创建图控元插件运行时接口实例
        /// </summary>
        /// <param name="mapCellID">图元ID</param>
        /// <param name="mapCellName">图元名称</param>
        /// <returns>图控元插件运行时接口实例</returns>
        IControlMapCellObject IControlMapCell.CreateMapCellObj(MapObjID mapCellID, string mapCellName)
        {
            return new MapCellPasswordBoxCtlObj(mapCellID, mapCellName);
        }

        #endregion
    }
}
