using Avalonia.Media;
using Griffins.Map;
using Griffins.Map.UI;
using Griffins.UI2;

namespace GKG.Map.AuxiliaryInfoFuncCtlMapCell
{
    /// <summary>
    /// 辅助信息图元设计时类
    /// 负责在设计时管理辅助信息图元的行为和属性设置
    /// </summary>
    class MapCellAuxiliaryInfoCtlDesigntime : IFunctionalMapCellDesigntime
    {
        #region 私有字段
        private MapCellAuxiliaryInfoCtlObj mapCellObj;
        private IFunctionalMapCellDesigntimeCallBack iCallBack;
        #endregion

        #region 构造函数
        /// <summary>
        /// 初始化辅助信息图元设计时实例
        /// </summary>
        /// <param name="mapCellID">图元ID</param>
        /// <param name="mapCellName">图元名称</param>
        public MapCellAuxiliaryInfoCtlDesigntime(MapObjID mapCellID, string mapCellName)
        {
            mapCellObj = new MapCellAuxiliaryInfoCtlObj(mapCellID, mapCellName, true);
        }
        #endregion

        #region IFunctionalMapCellDesigntime 成员
        /// <summary>
        /// 获取功能图元对象
        /// </summary>
        IFunctionalMapCellObject IFunctionalMapCellDesigntime.FunctionalMapCellObject
        {
            get { return this.mapCellObj; }
        }

        /// <summary>
        /// 初始化设计时回调接口
        /// </summary>
        /// <param name="iCallBack">设计时回调接口</param>
        void IFunctionalMapCellDesigntime.Init(IFunctionalMapCellDesigntimeCallBack iCallBack)
        {
            this.iCallBack = iCallBack;
        }

        /// <summary>
        /// 设置文本字体
        /// </summary>
        /// <param name="textFont">文本字体信息</param>
        void IFunctionalMapCellDesigntime.SetTextFont(FontInfo textFont)
        {
            this.mapCellObj.AuxiliaryInfoPropertyModelEdit.TextFont = textFont;
        }

        /// <summary>
        /// 设置文本颜色
        /// </summary>
        /// <param name="textColor">文本颜色</param>
        void IFunctionalMapCellDesigntime.SetTextColor(Color textColor)
        {
            this.mapCellObj.AuxiliaryInfoPropertyModelEdit.TextColor = textColor;
        }

        /// <summary>
        /// 设置线条颜色（辅助信息图元不使用线条颜色）
        /// </summary>
        /// <param name="lineColor">线条颜色</param>
        void IFunctionalMapCellDesigntime.SetLineColor(Color lineColor)
        {
        }

        /// <summary>
        /// 设置填充颜色（辅助信息图元不使用填充颜色）
        /// </summary>
        /// <param name="fillColor">填充颜色</param>
        void IFunctionalMapCellDesigntime.SetFillColor(Color fillColor)
        {
        }
        #endregion

        #region IMapObjCellDesignTimeBase 成员
        /// <summary>
        /// 获取图元单元格基础对象
        /// </summary>
        IMapObjCellBase IMapObjCellDesignTimeBase.MapObjCellBase
        {
            get { return this.mapCellObj; }
        }

        /// <summary>
        /// 设置图元ID
        /// </summary>
        /// <param name="id">图元ID</param>
        void IMapObjCellDesignTimeBase.SetID(MapObjID id)
        {
            this.mapCellObj.SetID(id);
        }

        /// <summary>
        /// 设置图元名称
        /// </summary>
        /// <param name="name">图元名称</param>
        void IMapObjCellDesignTimeBase.SetName(string name)
        {
            this.mapCellObj.SetName(name);
        }

        /// <summary>
        /// 缩放变化时的处理
        /// </summary>
        void IMapObjCellDesignTimeBase.OnZoomChanged()
        {
            this.mapCellObj.SetButtonTextFont();
        }
        #endregion
    }
}
