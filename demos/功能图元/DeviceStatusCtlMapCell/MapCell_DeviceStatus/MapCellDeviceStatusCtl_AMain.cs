using Avalonia.Media.Imaging;
using Avalonia.Platform;
using GF_Gereric;
using GKG.Map.DeviceStatusFuncCtlMapCell.MapCell_DeviceStatus.Factories;
using Griffins.Map;
using Griffins.Map.UI;
using System;
using System.IO;

namespace GKG.Map.DeviceStatusFuncCtlMapCell.MapCell_DeviceStatus
{
    [MapCellKindCategory(DeviceStatusMapCellGroup.DeviceStatusMapCellGroupID_Str)]
    [FunctionalMapCell("{709841B7-1662-4C3D-9D25-0E954032F097}")]
    class MapCellDeviceStatusCtl_AMain : GriffinsPluginMngClass, IFunctionalMapCell
    {
        private static Bitmap bitmap = null;

        static MapCellDeviceStatusCtl_AMain()
        {
            try
            {
                DeviceStatusEditorFactoryRegistration.RegisterGlobal();
                var assetLoader = AssetLoader.Open;
                // 注意：这里可能需要修改资源路径，假设图片在 Assets/Images/ 下
                // 如果没有图片，可能需要先处理图片资源
                var uri = new Uri("avares://GKG.Map.DeviceStatusFuncCtlMapCell/Assets/Images/ctr-DeviceStatus.png");
                using(var stream = AssetLoader.Open(uri))
                 {
                    bitmap = new Bitmap(stream); // 加载为Bitmap
                }
                // 暂时使用原有的路径尝试，或者改用一个存在的图片
                // var uri = new Uri("avares://Griffins.Map.CtlMapCell.Generic/Assets/Images/ctr-ImageGroup.png");
            }
            catch
            {
                bitmap = null;
            }
        }

        void IFunctionalMapCell.Init(string pluginFileName)
        {
            DeviceStatusEditorFactoryRegistration.RegisterGlobal();
        }

        int IFunctionalMapCell.DefaultWidth => 200;
        int IFunctionalMapCell.DefaultHeight => 80;
        Bitmap IFunctionalMapCell.Ico => bitmap;
        string IFunctionalMapCell.MapCellKindName => "设备状态显示";

        IFunctionalMapCellDesigntime IFunctionalMapCell.CreateMapCellDesigntime(MapObjID mapCellID, string mapCellName)
        {
            return new MapCellDeviceStatusCtlDesigntime(mapCellID, mapCellName);
        }

        IFunctionalMapCellObject IFunctionalMapCell.CreateMapCellObj(MapObjID mapCellID, string mapCellName)
        {
            return new MapCellDeviceStatusCtlObj(mapCellID, mapCellName);
        }
    }
}
