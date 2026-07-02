using System;
using Avalonia.Media;

namespace GKG.Map.MapCell.Generic.RadioButton
{
    class MapCellRadioButtonCtlDesigntime : IControlMapCellDesigntime
    {
        private MapCellRadioButtonCtlObj mapCellObj;
        private IControlMapCellDesigntimeCallBack iCallBack;

        public MapCellRadioButtonCtlDesigntime(MapObjID mapCellID, string mapCellName)
        {
            mapCellObj = new MapCellRadioButtonCtlObj(mapCellID, mapCellName, true);
        }

        #region IMapCellDesigntime 成员

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
            this.mapCellObj.RadioButtonPropertyModelEdit.TextInfo.FontColor = textColor;
        }

        void IControlMapCellDesigntime.SetLineColor(Color lineColor)
        {
        }

        void IControlMapCellDesigntime.SetFillColor(Color fillColor)
        {
        }

        #endregion

        #region IMapObjCellDesignTimeBase 成员

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

        #endregion
    }
}
