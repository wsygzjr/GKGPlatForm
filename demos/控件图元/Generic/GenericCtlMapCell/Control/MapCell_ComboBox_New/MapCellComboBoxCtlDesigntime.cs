using Avalonia.Media;
using System;

namespace GKG.Map.MapCell.Generic.Control.MapCell_ComboBox
{
    internal class MapCellComboBoxCtlDesigntime : IControlMapCellDesigntime
    {
        private MapCellComboBoxCtlObj mapCellObj;
        private IControlMapCellDesigntimeCallBack iCallBack;

        public MapCellComboBoxCtlDesigntime(MapObjID mapCellID, string mapCellName)
        {
            mapCellObj = new MapCellComboBoxCtlObj(mapCellID, mapCellName);
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
            mapCellObj.ComboBoxPropertyModelEdit.ForegroundColor = textColor;
        }

        void IControlMapCellDesigntime.SetLineColor(Color lineColor)
        {
            mapCellObj.ComboBoxPropertyModelEdit.BorderBrush = lineColor;
        }

        void IControlMapCellDesigntime.SetFillColor(Color fillColor)
        {
            mapCellObj.ComboBoxPropertyModelEdit.BackgroundColor = fillColor;
        }

        IMapObjCellBase IMapObjCellDesignTimeBase.MapObjCellBase => mapCellObj;
        void IMapObjCellDesignTimeBase.SetID(MapObjID id) => mapCellObj.SetID(id);
        void IMapObjCellDesignTimeBase.SetName(string name) => mapCellObj.SetName(name);
        void IMapObjCellDesignTimeBase.OnZoomChanged() { }
    }
}