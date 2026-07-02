using Avalonia.Media;
using Griffins.Map;
using Griffins.Map.UI;
using Griffins.UI2;

namespace GKG.Map.HeatingControlFuncCtlMapCell
{
    /// <summary>
    /// 加热控制图元设计时支持类
    /// 提供设计时环境下的图元创建和配置功能
    /// </summary>
    class MapCellHeatingControlCtlDesigntime : IFunctionalMapCellDesigntime
    {
        /// <summary>
        /// 图元对象实例
        /// </summary>
        private MapCellHeatingControlCtlObj mapCellObj;
        /// <summary>
        /// 设计时回调接口
        /// </summary>
        private IFunctionalMapCellDesigntimeCallBack iCallBack;

        /// <summary>
        /// 初始化设计时图元实例
        /// </summary>
        /// <param name="mapCellID">图元ID</param>
        /// <param name="mapCellName">图元名称</param>
        public MapCellHeatingControlCtlDesigntime(MapObjID mapCellID, string mapCellName)
        {
            mapCellObj = new MapCellHeatingControlCtlObj(mapCellID, mapCellName, true);
        }

        /// <summary>
        /// 功能图元对象接口
        /// </summary>
        IFunctionalMapCellObject IFunctionalMapCellDesigntime.FunctionalMapCellObject
        {
            get { return this.mapCellObj; }
        }

        /// <summary>
        /// 初始化设计时回调
        /// </summary>
        /// <param name="iCallBack">设计时回调接口</param>
        void IFunctionalMapCellDesigntime.Init(IFunctionalMapCellDesigntimeCallBack iCallBack)
        {
            this.iCallBack = iCallBack;
        }

        /// <summary>
        /// 设置文本字体
        /// </summary>
        /// <param name="textFont">字体信息</param>
        void IFunctionalMapCellDesigntime.SetTextFont(FontInfo textFont)
        {
            this.mapCellObj.HeatingControlPropertyModelEdit.TextFont = textFont;
        }

        /// <summary>
        /// 设置文本颜色
        /// </summary>
        /// <param name="textColor">文本颜色</param>
        void IFunctionalMapCellDesigntime.SetTextColor(Color textColor)
        {
            this.mapCellObj.HeatingControlPropertyModelEdit.TextColor = textColor;
        }

        /// <summary>
        /// 设置线条颜色
        /// </summary>
        /// <param name="lineColor">线条颜色</param>
        void IFunctionalMapCellDesigntime.SetLineColor(Color lineColor)
        {
        }

        /// <summary>
        /// 设置填充颜色
        /// </summary>
        /// <param name="fillColor">填充颜色</param>
        void IFunctionalMapCellDesigntime.SetFillColor(Color fillColor)
        {
        }

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
    }
}
