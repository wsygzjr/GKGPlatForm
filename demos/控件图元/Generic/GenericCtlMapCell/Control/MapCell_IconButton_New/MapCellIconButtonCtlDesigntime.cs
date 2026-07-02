using Avalonia.Media;
using System;

namespace GKG.Map.MapCell.Generic.IconButton
{
    class MapCellIconButtonCtlDesigntime : IControlMapCellDesigntime
    {
        private MapCellIconButtonCtlObj mapCellObj;
        private IControlMapCellDesigntimeCallBack iCallBack;

        public MapCellIconButtonCtlDesigntime(MapObjID mapCellID, string mapCellName)
        {
            // 传入 designTime = true 标识
            mapCellObj = new MapCellIconButtonCtlObj(mapCellID, mapCellName, true);
        }

        #region IMapCellDesigntime 成员

        IControlMapCellObject IControlMapCellDesigntime.ControlMapCellObject => mapCellObj;

        void IControlMapCellDesigntime.Init(IControlMapCellDesigntimeCallBack iCallBack)
        {
            this.iCallBack = iCallBack;
        }

        void IControlMapCellDesigntime.SetTextFont(FontInfo textFont)
        {
            // 若后续平台有快捷修改字体需求可在此映射
        }

        void IControlMapCellDesigntime.SetTextColor(Color textColor)
        {
            // 完美映射到文字颜色
            mapCellObj.IconButtonPropertyModelEdit.ForegroundColor = textColor;
        }

        void IControlMapCellDesigntime.SetLineColor(Color lineColor)
        {
            // 映射到边框颜色
            mapCellObj.IconButtonPropertyModelEdit.BorderBrush = lineColor;
        }

        void IControlMapCellDesigntime.SetFillColor(Color fillColor)
        {
            // 映射到背景颜色
            mapCellObj.IconButtonPropertyModelEdit.BackgroundColor = fillColor;
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