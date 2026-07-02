using Avalonia.Media;

namespace GKG.Map.MapCell.Generic.Link
{
    class MapCellLinkCtlDesigntime : IControlMapCellDesigntime
    {
        private readonly MapCellLinkCtlObj mapCellObj;
        private IControlMapCellDesigntimeCallBack iCallBack;

        public MapCellLinkCtlDesigntime(MapObjID mapCellID, string mapCellName)
        {
            mapCellObj = new MapCellLinkCtlObj(mapCellID, mapCellName, true);
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
            mapCellObj.LinkPropertyModelEdit.BrushInfo.TextColor = textColor;
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
