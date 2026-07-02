using System;
using System.Collections.Generic;
using System.Text;
using Griffins;
using System.Windows.Forms;
using System.Drawing;
using Griffins.Graph;
using GF_Gereric;

namespace Griffins.Map.CtlMapCell.Generic.ObjectValueView
{
    [MapCellKindCategory(GenericMapCellGroup.GenericMapCellGroupID_Str)]
    [ControlMapCell("{866D734B-190E-451d-94DD-2F5BF4C46169}")]
    class MapCellObjectValueView_AMain : GriffinsPluginMngClass, IControlMapCell
    {
        #region IMapCell 成员

        void IControlMapCell.Init(string pluginFileName)
        {
        }

        int IControlMapCell.DefaultWidth
        {
            get { return 128; }
        }

        int IControlMapCell.DefaultHeight
        {
            get { return 128; }
        }

        Icon IControlMapCell.Ico
        {
            get { return ResourceA.XMLVal_View; }
        }

        string IControlMapCell.MapCellKindName
        {
            get { return ResourceA.ObjectValueView; }
        }

        /// <summary>
        /// 创建图控元插件设计时接口实例
        /// </summary>
        /// <param name="mapCellID">图元ID</param>
        /// <param name="mapCellName">图元名称</param>
        /// <returns>图控元插件设计时接口实例</returns>
        IControlMapCellDesigntime IControlMapCell.CreateMapCellDesigntime(MapObjID mapCellID, string mapCellName)
        {
            return new MapCellObjectValueViewDesigntime(mapCellID, mapCellName);
        }

        /// <summary>
        /// 创建图控元插件运行时接口实例
        /// </summary>
        /// <param name="mapCellID">图元ID</param>
        /// <param name="mapCellID">图元名称</param>
        /// <returns>图控元插件运行时接口实例</returns>
        IControlMapCellObject IControlMapCell.CreateMapCellObj(MapObjID mapCellID, string mapCellName)
        {
            return new MapCellObjectValueViewObj(mapCellID, mapCellName);
        }

        #endregion
    }

}