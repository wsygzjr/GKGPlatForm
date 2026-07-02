using Avalonia.Media;
using Griffins.Map;
using Griffins.Map.UI;
using Griffins.UI2;

namespace GKG.Map.CameraViewerFuncCtlMapCell;

class MapCellCameraViewerCtlDesigntime : IFunctionalMapCellDesigntime
{
    private MapCellCameraViewerCtlObj mapCellObj;
    private IFunctionalMapCellDesigntimeCallBack iCallBack = null!;

    public MapCellCameraViewerCtlDesigntime(MapObjID mapCellID, string mapCellName)
    {
        mapCellObj = new MapCellCameraViewerCtlObj(mapCellID, mapCellName, true);
    }

    #region IFunctionalMapCellDesigntime 成员

    /// <summary>
    /// 功能图元对象接口
    /// </summary>
    IFunctionalMapCellObject IFunctionalMapCellDesigntime.FunctionalMapCellObject
    {
        get { return this.mapCellObj; }
    }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="iCallBack">功能图元设计时对象回调接口</param>
    void IFunctionalMapCellDesigntime.Init(IFunctionalMapCellDesigntimeCallBack iCallBack)
    {
        this.iCallBack = iCallBack;
    }

    /// <summary>
    /// 设置文本字体
    /// </summary>
    /// <param name="textFont">文本字体</param>
    void IFunctionalMapCellDesigntime.SetTextFont(FontInfo textFont)
    {

    }

    /// <summary>
    /// 设置文本颜色
    /// </summary>
    /// <param name="textColor">文本颜色</param>
    void IFunctionalMapCellDesigntime.SetTextColor(Color textColor)
    {

    }
    /// <summary>
    /// 设置线条颜色
    /// </summary>
    /// <param name="lineColor">线条颜色</param>
    void IFunctionalMapCellDesigntime.SetLineColor(Color lineColor)
    {
    }
    /// <summary>
    /// 设置填充颜色
    /// </summary>
    /// <param name="fillColor">填充颜色</param>
    void IFunctionalMapCellDesigntime.SetFillColor(Color fillColor)
    {
    }

    #endregion

    #region IMapObjCellDesignTimeBase 成员
    /// <summary>
    /// 图控对象单元基本对象
    /// </summary>
    IMapObjCellBase IMapObjCellDesignTimeBase.MapObjCellBase
    {
        get { return this.mapCellObj; }
    }

    /// <summary>
    /// 设置图元ID
    /// </summary>
    /// <param name="id">图元ID</param>
    void IMapObjCellDesignTimeBase.SetID(MapObjID id)
    {
        this.mapCellObj.SetID(id);
    }

    // <summary>
    /// 设置图元名称
    /// </summary>
    /// <param name="name">图元名称</param>
    void IMapObjCellDesignTimeBase.SetName(string name)
    {
        this.mapCellObj.SetName(name);
    }

    /// <summary>
    /// 当图缩放参数调整时调用
    /// </summary>
    void IMapObjCellDesignTimeBase.OnZoomChanged()
    {
        //this.mapCellObj.SetButtonTextFont();
    }

    #endregion
}

