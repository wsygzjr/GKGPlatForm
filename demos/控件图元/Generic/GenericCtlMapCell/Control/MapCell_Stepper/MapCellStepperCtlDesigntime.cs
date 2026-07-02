using System;
using Avalonia.Media;
using GKG.Map.MapCell.Generic.Stepper.Objects;

namespace GKG.Map.MapCell.Generic.Stepper
{
    class MapCellStepperCtlDesigntime : IControlMapCellDesigntime
    {
        private MapCellStepperCtlObj mapCellObj;
        private IControlMapCellDesigntimeCallBack iCallBack;

        public MapCellStepperCtlDesigntime(MapObjID mapCellID, string mapCellName)
        {
            mapCellObj = new MapCellStepperCtlObj(mapCellID, mapCellName, true);
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
            this.mapCellObj.StepperPropertyModelEdit.TextInfo.FontColorStr = textColor.ToString();
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
