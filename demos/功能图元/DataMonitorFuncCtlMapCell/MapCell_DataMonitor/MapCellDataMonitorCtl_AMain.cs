using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using GF_Gereric;
using Griffins.Map;
using Griffins.Map.UI;
using System;

namespace GKG.Map.DataMonitorFuncCtlMapCell
{
    [MapCellKindCategory(DataMonitorMapCellGroup.DataMonitorMapCellGroupID_Str)]
    [FunctionalMapCell("{6E7CF40C-FC31-4C14-AF8A-21F49910A0CF}")]
    public class MapCellDataMonitorCtl_AMain : GriffinsPluginMngClass, IFunctionalMapCell
    {
        private static Bitmap bitmap = null;

        static MapCellDataMonitorCtl_AMain()
        {
            var assetLoader = AssetLoader.Open;
            // 路径格式：项目根命名空间;component/相对路径
            var uri = new Uri("avares://GKG.Map.DataMonitorFuncCtlMapCell/Assets/Images/DataMonitor.png");
            using (var stream = AssetLoader.Open(uri))
            {
                bitmap = new Bitmap(stream); // 加载为Bitmap
            }
        }

        void IFunctionalMapCell.Init(string pluginFileName)
        {
        }

        int IFunctionalMapCell.DefaultWidth
        {
            get { return 420; }
        }

        int IFunctionalMapCell.DefaultHeight
        {
            get { return 270; }
        }

        Bitmap IFunctionalMapCell.Ico
        {
            get { return bitmap; }
        }

        string IFunctionalMapCell.MapCellKindName
        {
            get { return ResourceA.DataMonitor; }
        }

        IFunctionalMapCellDesigntime IFunctionalMapCell.CreateMapCellDesigntime(MapObjID mapCellID, string mapCellName)
        {
            return new MapCellDataMonitorCtlDesigntime(mapCellID, mapCellName);
        }

        IFunctionalMapCellObject IFunctionalMapCell.CreateMapCellObj(MapObjID mapCellID, string mapCellName)
        {
            return new MapCellDataMonitorCtlObj(mapCellID, mapCellName);
        }
    }
}
