namespace GKG.Map.MapCell.Generic.Control.MapCell_DateInput
{
    internal class MapCellDateInputCtlDesigntime : IControlMapCellDesigntime
    {
        private readonly MapCellDateInputCtlObj _mapCellObj;

        public MapCellDateInputCtlDesigntime(MapObjID mapCellID, string mapCellName)
        {
            _mapCellObj = new MapCellDateInputCtlObj(mapCellID, mapCellName);
        }

        IControlMapCellObject IControlMapCellDesigntime.ControlMapCellObject => _mapCellObj;

        void IControlMapCellDesigntime.Init(IControlMapCellDesigntimeCallBack iCallBack)
        {
        }

        void IControlMapCellDesigntime.SetTextFont(FontInfo textFont)
        {
        }

        void IControlMapCellDesigntime.SetTextColor(Avalonia.Media.Color textColor)
        {
        }

        void IControlMapCellDesigntime.SetLineColor(Avalonia.Media.Color lineColor)
        {
        }

        void IControlMapCellDesigntime.SetFillColor(Avalonia.Media.Color fillColor)
        {
        }

        IMapObjCellBase IMapObjCellDesignTimeBase.MapObjCellBase => _mapCellObj;

        void IMapObjCellDesignTimeBase.SetID(MapObjID id) => _mapCellObj.SetID(id);

        void IMapObjCellDesignTimeBase.SetName(string name) => _mapCellObj.SetName(name);

        void IMapObjCellDesignTimeBase.OnZoomChanged()
        {
        }
    }
}
