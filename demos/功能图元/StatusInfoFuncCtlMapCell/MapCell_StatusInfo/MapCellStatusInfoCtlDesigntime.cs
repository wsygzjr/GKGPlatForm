using Avalonia.Media;
using GF_Gereric;
using Griffins.Map;
using Griffins.Map.UI;
using Griffins.UI2;

namespace GKG.Map.StatusInfoFuncCtlMapCell
{
    /// <summary>
    /// 状态信息图元设计时对象
    /// 提供设计时环境下的图元创建和配置功能
    /// </summary>
    class MapCellStatusInfoCtlDesigntime : IFunctionalMapCellDesigntime
    {
        /// <summary>
        /// 图元对象实例
        /// </summary>
        private readonly MapCellStatusInfoCtlObj mapCellObj;
        /// <summary>
        /// 设计时回调接口
        /// </summary>
        private IFunctionalMapCellDesigntimeCallBack iCallBack;

        /// <summary>
        /// 初始化设计时图元实例
        /// </summary>
        /// <param name="mapCellID">图元ID</param>
        /// <param name="mapCellName">图元名称</param>
        public MapCellStatusInfoCtlDesigntime(MapObjID mapCellID, string mapCellName)
        {
            mapCellObj = new MapCellStatusInfoCtlObj(mapCellID, mapCellName, true);
        }

        #region IFunctionalMapCellDesigntime 成员

        /// <summary>
        /// 获取功能图元对象
        /// </summary>
        IFunctionalMapCellObject IFunctionalMapCellDesigntime.FunctionalMapCellObject
        {
            get { return mapCellObj; }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="iCallBack">功能图元设计时对象回调接口</param>
        void IFunctionalMapCellDesigntime.Init(IFunctionalMapCellDesigntimeCallBack iCallBack)
        {
            this.iCallBack = iCallBack;
        }

        /// <summary>
        /// 设置文本字体
        /// </summary>
        /// <param name="textFont">文本字体</param>
        void IFunctionalMapCellDesigntime.SetTextFont(FontInfo textFont)
        {
            mapCellObj.StatusInfoPropertyModelEdit.TextFont = textFont;
        }

        /// <summary>
        /// 设置文本颜色
        /// </summary>
        /// <param name="textColor">文本颜色</param>
        void IFunctionalMapCellDesigntime.SetTextColor(Color textColor)
        {
            mapCellObj.StatusInfoPropertyModelEdit.TextColor = textColor;
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

        #endregion

        #region IMapObjCellDesignTimeBase 成员

        /// <summary>
        /// 获取图元单元格基础对象
        /// </summary>
        IMapObjCellBase IMapObjCellDesignTimeBase.MapObjCellBase
        {
            get { return mapCellObj; }
        }

        /// <summary>
        /// 设置图元ID
        /// </summary>
        /// <param name="id">图元ID</param>
        void IMapObjCellDesignTimeBase.SetID(MapObjID id)
        {
            mapCellObj.SetID(id);
        }

        /// <summary>
        /// 设置图元名称
        /// </summary>
        /// <param name="name">图元名称</param>
        void IMapObjCellDesignTimeBase.SetName(string name)
        {
            mapCellObj.SetName(name);
        }

        /// <summary>
        /// 当图缩放参数调整时调用
        /// </summary>
        void IMapObjCellDesignTimeBase.OnZoomChanged()
        {
            mapCellObj.SetButtonTextFont();
        }

        #endregion
    }
}
