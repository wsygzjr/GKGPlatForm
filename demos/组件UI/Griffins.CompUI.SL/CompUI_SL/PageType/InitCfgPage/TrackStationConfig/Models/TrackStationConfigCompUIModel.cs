namespace Griffins.CompUI.SL.InitCfgPage.Models
{
    public class TrackStationConfigCompUIModel
    {
        public bool HasLeftSensor { get; set; }

        public bool HasRightSensor { get; set; }

        public bool HasLeftCylinder { get; set; }

        public bool HasRightCylinder { get; set; }

        public SensorConfigCfgInfo? LeftSensorConfig { get; set; }

        public SensorConfigCfgInfo? RightSensorConfig { get; set; }

        public GKG.UI.General.CylinderConfigCfgInfo? LeftCylinderConfig { get; set; }

        public GKG.UI.General.CylinderConfigCfgInfo? RightCylinderConfig { get; set; }
    }
}
