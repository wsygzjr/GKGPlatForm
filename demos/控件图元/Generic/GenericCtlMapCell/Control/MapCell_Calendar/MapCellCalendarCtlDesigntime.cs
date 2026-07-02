using Avalonia.Media;

namespace GKG.Map.MapCell.Generic.Control.MapCell_Calendar
{
    internal class MapCellCalendarCtlDesigntime : IControlMapCellDesigntime
    {
        private readonly MapCellCalendarCtlObj _mapCellObj;

        public MapCellCalendarCtlDesigntime(MapObjID mapCellID, string mapCellName)
        {
            _mapCellObj = new MapCellCalendarCtlObj(mapCellID, mapCellName);
        }

        IControlMapCellObject IControlMapCellDesigntime.ControlMapCellObject => _mapCellObj;

        void IControlMapCellDesigntime.Init(IControlMapCellDesigntimeCallBack iCallBack)
        {
        }

        void IControlMapCellDesigntime.SetTextFont(FontInfo textFont)
        {
        }

        void IControlMapCellDesigntime.SetTextColor(Color textColor)
        {
            _mapCellObj.CalendarPropertyModelEdit.TextInfo.FontColor = textColor;
        }

        void IControlMapCellDesigntime.SetLineColor(Color lineColor)
        {
            _mapCellObj.CalendarPropertyModelEdit.BrushInfo.BorderColor = lineColor;
        }

        void IControlMapCellDesigntime.SetFillColor(Color fillColor)
        {
            _mapCellObj.CalendarPropertyModelEdit.BrushInfo.BackgroundColor = fillColor;
        }

        IMapObjCellBase IMapObjCellDesignTimeBase.MapObjCellBase => _mapCellObj;

        void IMapObjCellDesignTimeBase.SetID(MapObjID id) => _mapCellObj.SetID(id);

        void IMapObjCellDesignTimeBase.SetName(string name) => _mapCellObj.SetName(name);

        void IMapObjCellDesignTimeBase.OnZoomChanged()
        {
        }
    }
}
