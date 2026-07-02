using Avalonia.Media;
using Griffins.Map;
using Griffins.Map.UI;
using Griffins.UI2;

namespace GKG.Map.StationFuncCtlMapCell
{
    /// <summary>
    /// 工位功能图元在“组态设计时(Design-time)”的管理类
    /// </summary>
    class MapCellStationCtlDesigntime : IFunctionalMapCellDesigntime
    {
        private MapCellStationCtlObj mapCellObj;
        private IFunctionalMapCellDesigntimeCallBack iCallBack = null!;

        public MapCellStationCtlDesigntime(MapObjID mapCellID, string mapCellName)
        {
            mapCellObj = new MapCellStationCtlObj(mapCellID, mapCellName, true);
        }

        #region IFunctionalMapCellDesigntime 成员

        IFunctionalMapCellObject IFunctionalMapCellDesigntime.FunctionalMapCellObject => this.mapCellObj;

        void IFunctionalMapCellDesigntime.Init(IFunctionalMapCellDesigntimeCallBack iCallBack) => this.iCallBack = iCallBack;

        void IFunctionalMapCellDesigntime.SetTextFont(FontInfo textFont) { }

        void IFunctionalMapCellDesigntime.SetTextColor(Color textColor) { }

        void IFunctionalMapCellDesigntime.SetLineColor(Color lineColor) { }

        void IFunctionalMapCellDesigntime.SetFillColor(Color fillColor) { }

        #endregion

        #region IMapObjCellDesignTimeBase 成员

        IMapObjCellBase IMapObjCellDesignTimeBase.MapObjCellBase => this.mapCellObj;

        void IMapObjCellDesignTimeBase.SetID(MapObjID id) => this.mapCellObj.SetID(id);

        void IMapObjCellDesignTimeBase.SetName(string name) => this.mapCellObj.SetName(name);

        void IMapObjCellDesignTimeBase.OnZoomChanged() { }

        #endregion
    }
}