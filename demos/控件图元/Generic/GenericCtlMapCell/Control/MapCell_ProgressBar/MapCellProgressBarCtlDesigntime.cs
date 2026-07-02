using System;
using Avalonia.Media;

namespace GKG.Map.MapCell.Generic.ProgressBar
{
    class MapCellProgressBarCtlDesigntime : IControlMapCellDesigntime
    {
        private MapCellProgressBarCtlObj mapCellObj;
        private IControlMapCellDesigntimeCallBack iCallBack;

        public MapCellProgressBarCtlDesigntime(MapObjID mapCellID, string mapCellName)
        {
            mapCellObj = new MapCellProgressBarCtlObj(mapCellID, mapCellName, true);
        }

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
            this.mapCellObj.ProgressBarPropertyModelEdit.TextInfo.FontColor = textColor;
        }

        void IControlMapCellDesigntime.SetLineColor(Color lineColor)
        {
        }

        void IControlMapCellDesigntime.SetFillColor(Color fillColor)
        {
        }

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
    }
}
