using Avalonia.Media.Imaging;
using Avalonia.Platform;
using GF_Gereric;
using Griffins;
using System;

namespace GKG.Map.MapCell.Generic.Stepper
{
    [MapCellKindCategory(GenericMapCellGroup.GenericMapCellGroupID_Str)]
    [ControlMapCell("{9DEF6881-867E-44DF-9BF1-59005FA2B873}")]
    class MapCellStepperCtl_AMain : GriffinsPluginMngClass, IControlMapCell
    {
        private static Bitmap bitmap = null;
        static MapCellStepperCtl_AMain()
        {
            try
            {
                // 暂时使用默认图标或者 TextButton 图标
                var uri = new Uri("avares://GKG.Map.MapCell.Generic/Assets/Images/ctr-Stepper.png");
                using (var stream = AssetLoader.Open(uri))
                {
                    bitmap = new Bitmap(stream);
                }
            }
            catch
            {
                // 如果图标不存在，使用默认图标
            }
        }

        #region IMapCell 成员

        void IControlMapCell.Init(string pluginFileName)
        {
        }

        int IControlMapCell.DefaultWidth
        {
            get { return 200; }
        }

        int IControlMapCell.DefaultHeight
        {
            get { return 30; }
        }

        Bitmap IControlMapCell.Ico
        {
            get { return bitmap; }
        }

        string IControlMapCell.MapCellKindName
        {
            get { return "步进器"; }
        }

        IControlMapCellDesigntime IControlMapCell.CreateMapCellDesigntime(MapObjID mapCellID, string mapCellName)
        {
            return new MapCellStepperCtlDesigntime(mapCellID, mapCellName);
        }

        IControlMapCellObject IControlMapCell.CreateMapCellObj(MapObjID mapCellID, string mapCellName)
        {
            return new MapCellStepperCtlObj(mapCellID, mapCellName);
        }

        #endregion
    }
}
