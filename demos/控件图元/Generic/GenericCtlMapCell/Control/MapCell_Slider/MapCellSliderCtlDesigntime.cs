using Avalonia.Media;
using System;

namespace GKG.Map.MapCell.Generic.Control.MapCell_Slider
{
    internal class MapCellSliderCtlDesigntime : IControlMapCellDesigntime
    {
        private MapCellSliderCtlObj mapCellObj;
        private IControlMapCellDesigntimeCallBack iCallBack;

        public MapCellSliderCtlDesigntime(MapObjID mapCellID, string mapCellName)
        {
            mapCellObj = new MapCellSliderCtlObj(mapCellID, mapCellName);
        }

        #region IMapCellDesigntime 成员

        /// <summary>
        /// 控件图元对象接口
        /// </summary>
        IControlMapCellObject IControlMapCellDesigntime.ControlMapCellObject
        {
            get { return mapCellObj; }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="iCallBack">控件图元设计时对象回调接口</param>
        void IControlMapCellDesigntime.Init(IControlMapCellDesigntimeCallBack iCallBack)
        {
            this.iCallBack = iCallBack;
        }

        /// <summary>
        /// 设置文本字体
        /// </summary>
        /// <param name="textFont">文本字体</param>
        void IControlMapCellDesigntime.SetTextFont(FontInfo textFont)
        {
            // 滑块控件不支持文本字体设置
        }

        /// <summary>
        /// 设置文本颜色
        /// </summary>
        /// <param name="textColor">文本颜色</param>
        void IControlMapCellDesigntime.SetTextColor(Color textColor)
        {
            // 滑块控件不支持文本颜色设置
        }
        /// <summary>
        /// 设置线条颜色
        /// </summary>
        /// <param name="lineColor">线条颜色</param>
        void IControlMapCellDesigntime.SetLineColor(Color lineColor)
        {
            // 设置线条颜色
        }
        /// <summary>
        /// 设置填充颜色
        /// </summary>
        /// <param name="fillColor">填充颜色</param>
        void IControlMapCellDesigntime.SetFillColor(Color fillColor)
        {
            mapCellObj.SliderPropertyModelEdit.BrushInfo.BackgroundColor = fillColor;
        }

        #endregion

        #region IMapObjCellDesignTimeBase 成员
        /// <summary>
        /// 图控对象单元基本对象
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
            // 缩放时的处理
        }

        #endregion
    }
}
