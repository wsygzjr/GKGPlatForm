using Avalonia.Media;
using Griffins.Map;
using Griffins.Map.UI;
using Griffins.UI2;

namespace GKG.Map.Inspection2DFuncCtlMapCell
{
    public class MapCellInspection2DCtlDesigntime : IFunctionalMapCellDesigntime
    {
        private readonly MapCellInspection2DCtlObj mapCellObj;
        private IFunctionalMapCellDesigntimeCallBack iCallBack = null!;

        public MapCellInspection2DCtlDesigntime(MapObjID mapCellID, string mapCellName)
        {
            mapCellObj = new MapCellInspection2DCtlObj(mapCellID, mapCellName, true);
        }

        #region IFunctionalMapCellDesigntime 接口实现

        IFunctionalMapCellObject IFunctionalMapCellDesigntime.FunctionalMapCellObject => mapCellObj;

        void IFunctionalMapCellDesigntime.Init(IFunctionalMapCellDesigntimeCallBack iCallBack) => this.iCallBack = iCallBack;

        void IFunctionalMapCellDesigntime.SetTextFont(FontInfo textFont) => mapCellObj.Inspection2DPropertyModelEdit.TextFont = textFont;

        void IFunctionalMapCellDesigntime.SetTextColor(Color textColor) => mapCellObj.Inspection2DPropertyModelEdit.TextColor = textColor;

        void IFunctionalMapCellDesigntime.SetLineColor(Color lineColor) { }

        void IFunctionalMapCellDesigntime.SetFillColor(Color fillColor) { }

        #endregion

        #region IMapObjCellDesignTimeBase 成员

        IMapObjCellBase IMapObjCellDesignTimeBase.MapObjCellBase => mapCellObj;

        void IMapObjCellDesignTimeBase.SetID(MapObjID id) => mapCellObj.SetID(id);

        void IMapObjCellDesignTimeBase.SetName(string name) => mapCellObj.SetName(name);

        void IMapObjCellDesignTimeBase.OnZoomChanged() => mapCellObj.OnZoomChanged();

        #endregion
    }
}
