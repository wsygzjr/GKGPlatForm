using Avalonia.Media;
using System;

namespace GKG.Map.MapCell.Generic.Control.Lable
{
    class MapCellLableCtlDesigntime : IControlMapCellDesigntime
    {
        private MapCellLableCtlObj mapCellObj;
        private IControlMapCellDesigntimeCallBack iCallBack;

        public MapCellLableCtlDesigntime(MapObjID mapCellID, string mapCellName)
        {
            // 传入 designTime = true 标识
            mapCellObj = new MapCellLableCtlObj(mapCellID, mapCellName, true);
        }

        #region IMapCellDesigntime 成员

        IControlMapCellObject IControlMapCellDesigntime.ControlMapCellObject => mapCellObj;

        void IControlMapCellDesigntime.Init(IControlMapCellDesigntimeCallBack iCallBack)
        {
            this.iCallBack = iCallBack;
        }

        void IControlMapCellDesigntime.SetTextFont(FontInfo textFont)
        {
            // 如果平台有传 FontInfo 的需求，可在此映射到 FontFamily/FontSize
        }

        void IControlMapCellDesigntime.SetTextColor(Color textColor)
        {
            // 完美映射到拍扁后的新模型属性
            mapCellObj.LablePropertyModelEdit.ForegroundColor = textColor;
        }

        void IControlMapCellDesigntime.SetLineColor(Color lineColor)
        {
            // 映射到边框色
            mapCellObj.LablePropertyModelEdit.BorderBrush = lineColor;
        }

        void IControlMapCellDesigntime.SetFillColor(Color fillColor)
        {
            // 映射到背景色
            mapCellObj.LablePropertyModelEdit.BackgroundColor = fillColor;
        }

        #endregion

        #region IMapObjCellDesignTimeBase 成员

        IMapObjCellBase IMapObjCellDesignTimeBase.MapObjCellBase => mapCellObj;

        void IMapObjCellDesignTimeBase.SetID(MapObjID id) => mapCellObj.SetID(id);

        void IMapObjCellDesignTimeBase.SetName(string name) => mapCellObj.SetName(name);

        void IMapObjCellDesignTimeBase.OnZoomChanged()
        {
        }

        #endregion
    }
}