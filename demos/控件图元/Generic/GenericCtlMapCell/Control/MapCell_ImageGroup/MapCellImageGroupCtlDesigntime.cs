using System;
using Avalonia.Media;

namespace GKG.Map.MapCell.Generic.Control.MapCell_ImageGroup
{
    class MapCellImageGroupCtlDesigntime : IControlMapCellDesigntime
    {
        private MapCellImageGroupCtlObj mapCellObj;
        private IControlMapCellDesigntimeCallBack iCallBack;

        public MapCellImageGroupCtlDesigntime(MapObjID mapCellID, string mapCellName)
        {
            mapCellObj = new MapCellImageGroupCtlObj(mapCellID, mapCellName, true);
        }

        #region IControlMapCellDesigntime 成员

        IControlMapCellObject IControlMapCellDesigntime.ControlMapCellObject
        {
            get { return this.mapCellObj; }
        }

        void IControlMapCellDesigntime.Init(IControlMapCellDesigntimeCallBack iCallBack)
        {
            this.iCallBack = iCallBack;
        }

        void IControlMapCellDesigntime.SetTextFont(FontInfo textFont)
        {
        }

        void IControlMapCellDesigntime.SetTextColor(Color textColor)
        {
        }

        void IControlMapCellDesigntime.SetLineColor(Color lineColor)
        {
            // 图片组图元不支持线条颜色设置
        }

        void IControlMapCellDesigntime.SetFillColor(Color fillColor)
        {
            // 图片组图元不支持填充颜色设置
        }

        #endregion

        #region IMapObjCellDesignTimeBase 成员

        IMapObjCellBase IMapObjCellDesignTimeBase.MapObjCellBase
        {
            get { return this.mapCellObj; }
        }

        void IMapObjCellDesignTimeBase.SetID(MapObjID id)
        {
            this.mapCellObj.SetID(id);
        }

        void IMapObjCellDesignTimeBase.SetName(string name)
        {
            this.mapCellObj.SetName(name);
        }

        void IMapObjCellDesignTimeBase.OnZoomChanged()
        {
        }

        #endregion
    }
}
