using Avalonia.Media;
using Griffins.Map;
using Griffins.Map.UI;
using Griffins.UI2;

namespace GKG.Map.LoadUnloadFuncCtlMapCell
{
    /// <summary>
    /// 上下料功能图元组态设计时 (Design-time) 包装类
    /// 负责在 Griffins 编辑器组态配置阶段，提供图元的视觉样式设置、缩放以及基础属性配置的透传接口。
    /// </summary>
    internal class MapCellLoadUnloadCtlDesigntime : IFunctionalMapCellDesigntime
    {
        // 核心状态防御：加上 readonly 约束，彻底阻断运行期的非法重置
        private readonly MapCellLoadUnloadCtlObj _mapCellObj;

        // 保存设计时回调的句柄
        private IFunctionalMapCellDesigntimeCallBack _iCallBack = null!;

        /// <summary>
        /// 实例化设计时包装对象
        /// </summary>
        /// <param name="mapCellID">图元唯一编号</param>
        /// <param name="mapCellName">图元展示名称</param>
        public MapCellLoadUnloadCtlDesigntime(MapObjID mapCellID, string mapCellName)
        {
            // 初始化真实的控制对象 (传入 true 标记处于设计时环境)
            _mapCellObj = new MapCellLoadUnloadCtlObj(mapCellID, mapCellName, true);
        }

        #region IFunctionalMapCellDesigntime 成员实现

        /// <summary>
        /// 获取核心运行时对象句柄
        /// </summary>
        IFunctionalMapCellObject IFunctionalMapCellDesigntime.FunctionalMapCellObject => _mapCellObj;

        /// <summary>
        /// 初始化设计时回调环境
        /// </summary>
        void IFunctionalMapCellDesigntime.Init(IFunctionalMapCellDesigntimeCallBack iCallBack) => _iCallBack = iCallBack;

        // 以下为组态设计器环境下的样式设置空实现 (样式由前端 XAML 直接接管)
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
        void IMapObjCellDesignTimeBase.OnZoomChanged() => _mapCellObj.OnZoomChanged();

        #endregion
    }
}