using Avalonia.Media;
using Griffins.Map;
using Griffins.Map.UI;
using Griffins.UI2;

namespace GKG.Map.DeviceStatusFuncCtlMapCell.MapCell_DeviceStatus
{
    class MapCellDeviceStatusCtlDesigntime : IFunctionalMapCellDesigntime
    {
        private MapCellDeviceStatusCtlObj mapCellObj;
        private IFunctionalMapCellDesigntimeCallBack iCallBack;

        public MapCellDeviceStatusCtlDesigntime(MapObjID mapCellID, string mapCellName)
        {
            mapCellObj = new MapCellDeviceStatusCtlObj(mapCellID, mapCellName, true);
        }

        IFunctionalMapCellObject IFunctionalMapCellDesigntime.FunctionalMapCellObject => this.mapCellObj;
        void IFunctionalMapCellDesigntime.Init(IFunctionalMapCellDesigntimeCallBack iCallBack) => this.iCallBack = iCallBack;
        
        IMapObjCellBase IMapObjCellDesignTimeBase.MapObjCellBase => this.mapCellObj;
        void IMapObjCellDesignTimeBase.SetID(MapObjID id) => this.mapCellObj.SetID(id);
        void IMapObjCellDesignTimeBase.SetName(string name) => this.mapCellObj.SetName(name);
        void IMapObjCellDesignTimeBase.OnZoomChanged() { }

        // Implementing missing interface members
        void IFunctionalMapCellDesigntime.SetTextFont(FontInfo textFont) { }
        void IFunctionalMapCellDesigntime.SetTextColor(Color textColor) { }
        void IFunctionalMapCellDesigntime.SetLineColor(Color lineColor) { }
        void IFunctionalMapCellDesigntime.SetFillColor(Color fillColor) { }
    }
}
