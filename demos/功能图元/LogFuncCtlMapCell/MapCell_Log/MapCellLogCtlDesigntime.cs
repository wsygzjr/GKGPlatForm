
using Avalonia.Media;
using Griffins.Map;
using Griffins.Map.UI;
using Griffins.UI2;

namespace GKG.Map.LogFuncCtlMapCell
{
    class MapCellLogCtlDesigntime : IFunctionalMapCellDesigntime
    {
        private MapCellLogCtlObj mapCellObj;
        private IFunctionalMapCellDesigntimeCallBack iCallBack;

        public MapCellLogCtlDesigntime(MapObjID mapCellID, string mapCellName)
        {
            mapCellObj = new MapCellLogCtlObj(mapCellID, mapCellName, true);
        }

        IFunctionalMapCellObject IFunctionalMapCellDesigntime.FunctionalMapCellObject
        {
            get { return this.mapCellObj; }
        }

        void IFunctionalMapCellDesigntime.Init(IFunctionalMapCellDesigntimeCallBack iCallBack)
        {
            this.iCallBack = iCallBack;
        }

        void IFunctionalMapCellDesigntime.SetTextFont(FontInfo textFont)
        {
            this.mapCellObj.LogPropertyModelEdit.TextFont = textFont;
        }

        void IFunctionalMapCellDesigntime.SetTextColor(Color textColor)
        {
            this.mapCellObj.LogPropertyModelEdit.TextColor = textColor;
        }

        void IFunctionalMapCellDesigntime.SetLineColor(Color lineColor)
        {
        }

        void IFunctionalMapCellDesigntime.SetFillColor(Color fillColor)
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
            this.mapCellObj.OnZoomChanged();
        }
    }
}
