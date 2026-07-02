using Avalonia.Media;
using System;

namespace GKG.Map.MapCell.Generic.Control.MapCell_TextBox
{
    /// <summary>
    /// 文本输入框设计时对象
    /// </summary>
    internal class MapCellTextBoxCtlDesigntime : IControlMapCellDesigntime
    {
        private MapCellTextBoxCtlObj mapCellObj;
        private IControlMapCellDesigntimeCallBack iCallBack;

        public MapCellTextBoxCtlDesigntime(MapObjID mapCellID, string mapCellName)
        {
            // 设计时依然复用运行时对象，以便直接操作同一套属性模型
            mapCellObj = new MapCellTextBoxCtlObj(mapCellID, mapCellName);
        }

        IControlMapCellObject IControlMapCellDesigntime.ControlMapCellObject => mapCellObj;

        void IControlMapCellDesigntime.Init(IControlMapCellDesigntimeCallBack iCallBack)
        {
            // 设计器回调（可用于获取当前主题/字体等信息）
            this.iCallBack = iCallBack;
        }

        void IControlMapCellDesigntime.SetTextFont(FontInfo textFont)
        {
            // 预留：如需将设计器字体下发到控件，可在此处实现
        }

        void IControlMapCellDesigntime.SetTextColor(Color textColor)
        {
            // 设计器“文本颜色”注入
            mapCellObj.TextBoxPropertyModelEdit.TextInfo.FontColor = textColor;
        }

        void IControlMapCellDesigntime.SetLineColor(Color lineColor)
        {
            // 设计器“线条颜色”注入
            mapCellObj.TextBoxPropertyModelEdit.BrushInfo.BorderColor = lineColor;
        }

        void IControlMapCellDesigntime.SetFillColor(Color fillColor)
        {
            // 设计器“填充颜色”注入
            mapCellObj.TextBoxPropertyModelEdit.BrushInfo.BackgroundColor = fillColor;
        }

        IMapObjCellBase IMapObjCellDesignTimeBase.MapObjCellBase => mapCellObj;

        void IMapObjCellDesignTimeBase.SetID(MapObjID id)
        {
            mapCellObj.SetID(id);
        }

        void IMapObjCellDesignTimeBase.SetName(string name)
        {
            mapCellObj.SetName(name);
        }

        void IMapObjCellDesignTimeBase.OnZoomChanged()
        {
        }
    }
}
