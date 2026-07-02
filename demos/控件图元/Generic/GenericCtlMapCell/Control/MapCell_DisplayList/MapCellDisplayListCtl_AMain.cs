using Avalonia.Media.Imaging;
using Avalonia.Platform;
using GF_Gereric;
using Griffins;
using GKG.Map.MapCell.Generic.Control.MapCell_DisplayList.Factories;
using System;

namespace GKG.Map.MapCell.Generic.Control.MapCell_DisplayList
{
    [MapCellKindCategory(GenericMapCellGroup.GenericMapCellGroupID_Str)]
    [ControlMapCell("{6B2D9F8E-1D2B-4A43-BE3C-7E7A0F6E5D31}")]
	/// <summary>
	/// 显示列表控件图元插件入口。
	/// 
	/// 主要职责：
	/// - 注册属性面板自定义编辑器（列配置、排序字段/方向等）
	/// - 加载图元在工具箱中的图标资源
	/// - 提供设计时/运行时对象创建入口
	/// </summary>
    class MapCellDisplayListCtl_AMain : GriffinsPluginMngClass, IControlMapCell
    {
        private static Bitmap bitmap;

        static MapCellDisplayListCtl_AMain()
        {
            // 注册 PropertyGrid 的自定义编辑器。
            // 注意：Init 里也会调用一次，保证在不同加载时机下都能生效。
            DisplayListEditorFactoryRegistration.RegisterGlobal();
            try
            {
                // 路径格式：项目根命名空间;component/相对路径
                var uri = new Uri("avares://GKG.Map.MapCell.Generic/Assets/Images/ctr-DisplayList.png");
                using (var stream = AssetLoader.Open(uri))
                {
                    bitmap = new Bitmap(stream);
                }
            }
            catch (Exception ex)
            {
                bitmap = null;
            }
        }

        void IControlMapCell.Init(string pluginFileName)
        {
            // 某些宿主加载流程会跳过静态构造函数，保险起见在 Init 再注册一遍。
            DisplayListEditorFactoryRegistration.RegisterGlobal();
        }

        int IControlMapCell.DefaultWidth => 420;

        int IControlMapCell.DefaultHeight => 260;

        Bitmap IControlMapCell.Ico => bitmap;

        string IControlMapCell.MapCellKindName => "显示列表";

        IControlMapCellDesigntime IControlMapCell.CreateMapCellDesigntime(MapObjID mapCellID, string mapCellName)
        {
            return new MapCellDisplayListCtlDesigntime(mapCellID, mapCellName);
        }

        IControlMapCellObject IControlMapCell.CreateMapCellObj(MapObjID mapCellID, string mapCellName)
        {
            return new MapCellDisplayListCtlObj(mapCellID, mapCellName);
        }
    }
}
