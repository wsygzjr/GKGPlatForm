using Avalonia.Media;
using Griffins.Map;
using Griffins.Map.UI;
using Griffins.UI2;

namespace GKG.Map.ProductionInformationFuncCtlMapCell
{
    /// <summary>
    /// 生产信息功能图元组态设计时 (Design-time) 包装类
    /// 负责在 Griffins 编辑器组态配置阶段，提供图元的视觉样式设置、缩放以及基础属性配置的透传接口。
    /// </summary>
    internal class MapCellProductionInformationCtlDesigntime : IFunctionalMapCellDesigntime
    {
        // 核心状态防御：使用 readonly 约束，彻底阻断运行期的非法重置或覆盖
        private readonly MapCellProductionInfoCtlObj _mapCellObj;

        // 保存设计时回调的句柄
        private IFunctionalMapCellDesigntimeCallBack _iCallBack = null!;

        /// <summary>
        /// 实例化设计时包装对象
        /// </summary>
        /// <param name="mapCellID">图元唯一编号</param>
        /// <param name="mapCellName">图元展示名称</param>
        public MapCellProductionInformationCtlDesigntime(MapObjID mapCellID, string mapCellName)
        {
            // 初始化真实的图元对象 (传入 true 标记当前处于组态设计时环境)
            _mapCellObj = new MapCellProductionInfoCtlObj(mapCellID, mapCellName, true);
        }

        #region IFunctionalMapCellDesigntime 成员实现

        /// <summary>
        /// 获取图元对象
        /// </summary>
        IFunctionalMapCellObject IFunctionalMapCellDesigntime.FunctionalMapCellObject => _mapCellObj;

        /// <summary>
        /// 初始化设计时回调环境
        /// </summary>
        void IFunctionalMapCellDesigntime.Init(IFunctionalMapCellDesigntimeCallBack iCallBack) => _iCallBack = iCallBack;

        // 以下为组态设计器环境下的样式设置空实现 (生产信息图元的样式统一由前端 XAML 直接接管)
        void IFunctionalMapCellDesigntime.SetTextFont(FontInfo textFont) { }
        void IFunctionalMapCellDesigntime.SetTextColor(Color textColor) { }
        void IFunctionalMapCellDesigntime.SetLineColor(Color lineColor) { }
        void IFunctionalMapCellDesigntime.SetFillColor(Color fillColor) { }

        #endregion

        #region IMapObjCellDesignTimeBase 成员实现

        /// <summary>
        /// 向上转型，获取基础图元抽象对象
        /// </summary>
        IMapObjCellBase IMapObjCellDesignTimeBase.MapObjCellBase => _mapCellObj;

        /// <summary>
        /// 透传：设置图元 ID
        /// </summary>
        void IMapObjCellDesignTimeBase.SetID(MapObjID id) => _mapCellObj.SetID(id);

        /// <summary>
        /// 透传：设置图元展示名称
        /// </summary>
        void IMapObjCellDesignTimeBase.SetName(string name) => _mapCellObj.SetName(name);

        /// <summary>
        /// 透传：画板缩放变更事件响应
        /// </summary>
        void IMapObjCellDesignTimeBase.OnZoomChanged()
        {
            // 若后续需要在设计器放大缩小时调整内部 UI，可在此处调用 _mapCellObj.OnZoomChanged()
        }

        #endregion
    }
}