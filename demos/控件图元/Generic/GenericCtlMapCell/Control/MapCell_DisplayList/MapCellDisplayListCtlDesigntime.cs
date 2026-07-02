using Avalonia.Media;
using System;

namespace GKG.Map.MapCell.Generic.Control.MapCell_DisplayList
{
    internal class MapCellDisplayListCtlDesigntime : IControlMapCellDesigntime
    {
        private MapCellDisplayListCtlObj mapCellObj;
        private IControlMapCellDesigntimeCallBack iCallBack;

        public MapCellDisplayListCtlDesigntime(MapObjID mapCellID, string mapCellName)
        {
            mapCellObj = new MapCellDisplayListCtlObj(mapCellID, mapCellName);
        }

        IControlMapCellObject IControlMapCellDesigntime.ControlMapCellObject => mapCellObj;

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
        }

        void IControlMapCellDesigntime.SetFillColor(Color fillColor)
        {
        }

        IMapObjCellBase IMapObjCellDesignTimeBase.MapObjCellBase => mapCellObj;

        void IMapObjCellDesignTimeBase.SetID(MapObjID id)
        {
            mapCellObj.SetID(id);
        }

        void IMapObjCellDesignTimeBase.SetName(string name)
        {
            mapCellObj.SetName(name);
        }

        void IMapObjCellDesignTimeBase.OnZoomChanged()
        {
        }
    }
}
