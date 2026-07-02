using System;
using System.Collections.Generic;
using System.Text;
using Griffins;
using System.Windows.Forms;
using System.Drawing;
using Griffins.Graph;
using GF_Gereric;

namespace Griffins.Map.CtlMapCell.Generic.ImageCtl
{
    [MapCellKindCategory(GenericMapCellGroup.GenericMapCellGroupID_Str)]
    [ControlMapCell("{9630CCAC-9E1C-4666-9231-7EA97A27D8DA}")]
    class MapCellImageCtl_AMain : GriffinsPluginMngClass, IControlMapCell
    {
        #region IMapCell 成员

        void IControlMapCell.Init(string pluginFileName)
        {
        }

        int IControlMapCell.DefaultWidth
        {
            get { return 32; }
        }

        int IControlMapCell.DefaultHeight
        {
            get { return 32; }
        }

        Icon IControlMapCell.Ico
        {
            get { return ResourceA.map16_2; }
        }

        string IControlMapCell.MapCellKindName
        {
            get { return ResourceA.ImageControl; }
        }

        /// <summary>
        /// 创建图控元插件设计时接口实例
        /// </summary>
        /// <param name="mapCellID">图元ID</param>
        /// <param name="mapCellName">图元名称</param>
        /// <returns>图控元插件设计时接口实例</returns>
        IControlMapCellDesigntime IControlMapCell.CreateMapCellDesigntime(MapObjID mapCellID, string mapCellName)
        {
            return new MapCellImageCtlDesigntime(mapCellID, mapCellName);
        }

        /// <summary>
        /// 创建图控元插件运行时接口实例
        /// </summary>
        /// <param name="mapCellID">图元ID</param>
        /// <param name="mapCellID">图元名称</param>
        /// <returns>图控元插件运行时接口实例</returns>
        IControlMapCellObject IControlMapCell.CreateMapCellObj(MapObjID mapCellID, string mapCellName)
        {
            return new MapCellImageCtlObj(mapCellID, mapCellName);
        }

        #endregion
    }

}