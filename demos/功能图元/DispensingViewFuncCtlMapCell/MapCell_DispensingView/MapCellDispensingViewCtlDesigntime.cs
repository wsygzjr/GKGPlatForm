using Avalonia.Media;
using Griffins.Map;
using Griffins.Map.UI;
using Griffins.UI2;

namespace GKG.Map.DispensingViewFuncCtlMapCell
{
    class MapCellDispensingViewCtlDesigntime : IFunctionalMapCellDesigntime
    {
        private MapCellDispensingViewCtlObj mapCellObj;
        private IFunctionalMapCellDesigntimeCallBack iCallBack = null!;

        public MapCellDispensingViewCtlDesigntime(MapObjID mapCellID, string mapCellName)
        {
            mapCellObj = new MapCellDispensingViewCtlObj(mapCellID, mapCellName, true);
        }

        #region IFunctionalMapCellDesigntime 接口实现

        IFunctionalMapCellObject IFunctionalMapCellDesigntime.FunctionalMapCellObject => mapCellObj;

        void IFunctionalMapCellDesigntime.Init(IFunctionalMapCellDesigntimeCallBack iCallBack) => this.iCallBack = iCallBack;

        void IFunctionalMapCellDesigntime.SetTextFont(FontInfo textFont) => mapCellObj.DispensingViewPropertyModelEdit.TextFont = textFont;

        void IFunctionalMapCellDesigntime.SetTextColor(Color textColor) => mapCellObj.DispensingViewPropertyModelEdit.BackColor = textColor;

        void IFunctionalMapCellDesigntime.SetLineColor(Color lineColor) { }

        void IFunctionalMapCellDesigntime.SetFillColor(Color fillColor) { }

        #endregion

        #region IMapObjCellDesignTimeBase 接口实现

        IMapObjCellBase IMapObjCellDesignTimeBase.MapObjCellBase => mapCellObj;

        void IMapObjCellDesignTimeBase.SetID(MapObjID id) => mapCellObj.SetID(id);

        void IMapObjCellDesignTimeBase.SetName(string name) => mapCellObj.SetName(name);

        void IMapObjCellDesignTimeBase.OnZoomChanged() => mapCellObj.OnZoomChanged();

        #endregion
    }
}
