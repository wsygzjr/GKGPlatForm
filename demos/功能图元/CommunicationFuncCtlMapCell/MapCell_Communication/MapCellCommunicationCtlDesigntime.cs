using Avalonia.Media;
using Griffins.Map;
using Griffins.Map.UI;
using Griffins.UI2;

namespace GKG.Map.CommunicationFuncCtlMapCell
{
    class MapCellCommunicationCtlDesigntime : IFunctionalMapCellDesigntime
    {
        private MapCellCommunicationCtlObj mapCellObj;
        private IFunctionalMapCellDesigntimeCallBack iCallBack;

        public MapCellCommunicationCtlDesigntime(MapObjID mapCellID, string mapCellName)
        {
            mapCellObj = new MapCellCommunicationCtlObj(mapCellID, mapCellName, true);
        }

        IFunctionalMapCellObject IFunctionalMapCellDesigntime.FunctionalMapCellObject => this.mapCellObj;

        void IFunctionalMapCellDesigntime.Init(IFunctionalMapCellDesigntimeCallBack iCallBack)
        {
            this.iCallBack = iCallBack;
        }

        void IFunctionalMapCellDesigntime.SetTextFont(FontInfo textFont)
        {
            this.mapCellObj.CommunicationPropertyModelEdit.TextFont = textFont;
        }

        void IFunctionalMapCellDesigntime.SetTextColor(Color textColor)
        {
            this.mapCellObj.CommunicationPropertyModelEdit.TextColor = textColor;
        }

        void IFunctionalMapCellDesigntime.SetLineColor(Color lineColor)
        {
        }

        void IFunctionalMapCellDesigntime.SetFillColor(Color fillColor)
        {
        }

        IMapObjCellBase IMapObjCellDesignTimeBase.MapObjCellBase => this.mapCellObj;

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
            this.mapCellObj.SetButtonTextFont();
        }
    }
}
