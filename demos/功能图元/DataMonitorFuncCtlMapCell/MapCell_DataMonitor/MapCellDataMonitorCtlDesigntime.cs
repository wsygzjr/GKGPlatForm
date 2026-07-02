using Avalonia.Media;
using GF_Gereric;
using Griffins.Map;
using Griffins.Map.UI;
using Griffins.UI2;

namespace GKG.Map.DataMonitorFuncCtlMapCell
{
    /// <summary>
    /// 数据监控图元设计时类
    /// 负责在设计时管理数据监控图元的行为和属性设置
    /// </summary>
    public class MapCellDataMonitorCtlDesigntime : IFunctionalMapCellDesigntime
    {
        #region 私有字段
        private MapCellDataMonitorCtlObj mapCellObj;
        private IFunctionalMapCellDesigntimeCallBack iCallBack;
        #endregion

        #region 构造函数
        /// <summary>
        /// 初始化数据监控图元设计时实例
        /// </summary>
        /// <param name="mapCellID">图元ID</param>
        /// <param name="mapCellName">图元名称</param>
        public MapCellDataMonitorCtlDesigntime(MapObjID mapCellID, string mapCellName)
        {
            mapCellObj = new MapCellDataMonitorCtlObj(mapCellID, mapCellName, true);
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
            this.mapCellObj.DataMonitorPropertyModelEdit.TextFont = textFont;
        }

        /// <summary>
        /// 设置文本颜色
        /// </summary>
        /// <param name="textColor">文本颜色</param>
        void IFunctionalMapCellDesigntime.SetTextColor(Color textColor)
        {
            this.mapCellObj.DataMonitorPropertyModelEdit.TextColor = textColor;
        }

        /// <summary>
        /// 设置线条颜色（数据监控图元不使用线条颜色）
        /// </summary>
        /// <param name="lineColor">线条颜色</param>
        void IFunctionalMapCellDesigntime.SetLineColor(Color lineColor)
        {
        }

        /// <summary>
        /// 设置填充颜色（数据监控图元不使用填充颜色）
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
