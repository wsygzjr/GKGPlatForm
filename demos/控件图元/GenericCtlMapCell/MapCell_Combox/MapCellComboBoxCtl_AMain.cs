using System;
using System.Collections.Generic;
using System.Text;
using Griffins;
using System.Windows.Forms;
using System.Drawing;
using Griffins.Graph;
using GF_Gereric;

namespace Griffins.Map.CtlMapCell.Generic.ComboBox
{
    [MapCellKindCategory(GenericMapCellGroup.GenericMapCellGroupID_Str)]
    [ControlMapCell("{1D3BEDEE-A96F-416F-9668-078AA951C028}")]
    class MapCellComboBoxCtl_AMain : GriffinsPluginMngClass, IControlMapCell
    {
        #region IMapCell 成员

        void IControlMapCell.Init(string pluginFileName)
        {
        }

        int IControlMapCell.DefaultWidth
        {
            get { return 64; }
        }

        int IControlMapCell.DefaultHeight
        {
            get { return 22; }
        }

        Icon IControlMapCell.Ico
        {
            get { return ResourceA.Combox; }
        }

        string IControlMapCell.MapCellKindName
        {
            get { return ResourceA.ComboBoxName; }
        }

        /// <summary>
        /// 创建图控元插件设计时接口实例
        /// </summary>
        /// <param name="mapCellID">图元ID</param>
        /// <param name="mapCellName">图元名称</param>
        /// <returns>图控元插件设计时接口实例</returns>
        IControlMapCellDesigntime IControlMapCell.CreateMapCellDesigntime(MapObjID mapCellID, string mapCellName)
        {
            return new MapCellComboBoxCtlDesigntime(mapCellID, mapCellName);
        }

        /// <summary>
        /// 创建图控元插件运行时接口实例
        /// </summary>
        /// <param name="mapCellID">图元ID</param>
        /// <param name="mapCellID">图元名称</param>
        /// <returns>图控元插件运行时接口实例</returns>
        IControlMapCellObject IControlMapCell.CreateMapCellObj(MapObjID mapCellID, string mapCellName)
        {
            return new MapCellComboBoxCtlObj(mapCellID, mapCellName);
        }

        #endregion
    }

}