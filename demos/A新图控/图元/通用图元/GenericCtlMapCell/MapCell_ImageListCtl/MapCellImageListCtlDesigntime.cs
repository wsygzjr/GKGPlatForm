using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using Griffins.Graph;

namespace Griffins.Map.CtlMapCell.Generic.ImageListCtl
{
    class MapCellImageListCtlDesigntime : IControlMapCellDesigntime
    {
        private MapCellImageListCtlObj mapCellObj;
        private IControlMapCellDesigntimeCallBack iCallBack;

        public MapCellImageListCtlDesigntime(MapObjID mapCellID, string mapCellName)
        {
            mapCellObj = new MapCellImageListCtlObj(mapCellID, mapCellName,true);
        }

        #region IMapCellDesigntime 成员

        /// <summary>
        /// 控件图元对象接口
        /// </summary>
        IControlMapCellObject IControlMapCellDesigntime.ControlMapCellObject 
        {
            get { return this.mapCellObj; }
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
        void IControlMapCellDesigntime.SetTextFont(Font textFont)
        {
        }

        /// <summary>
        /// 设置文本颜色
        /// </summary>
        /// <param name="textColor">文本颜色</param>
        void IControlMapCellDesigntime.SetTextColor(Color textColor)
        {
        }
        /// <summary>
        /// 设置线条颜色
        /// </summary>
        /// <param name="lineColor">线条颜色</param>
        void IControlMapCellDesigntime.SetLineColor(Color lineColor)
        {
        }
        /// <summary>
        /// 设置填充颜色
        /// </summary>
        /// <param name="fillColor">填充颜色</param>
        void IControlMapCellDesigntime.SetFillColor(Color fillColor)
        {
        }

        #endregion

        #region IMapObjCellDesignTimeBase 成员
        /// <summary>
        /// 图控对象单元基本对象
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
            this.mapCellObj.SetMapCellID(id);
        }

        // <summary>
        /// 设置图元名称
        /// </summary>
        /// <param name="name">图元名称</param>
        void IMapObjCellDesignTimeBase.SetName(string name)
        {
            this.mapCellObj.SetMapCellName(name);
        }

        /// <summary>
        /// 当图缩放参数调整时调用
        /// </summary>
        void IMapObjCellDesignTimeBase.OnZoomChanged()
        {
        }

        #endregion
    }
}
