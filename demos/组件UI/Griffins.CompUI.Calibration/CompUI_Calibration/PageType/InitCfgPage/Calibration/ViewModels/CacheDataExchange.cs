using System.Collections.Generic;

namespace Griffins.CompUI.Calibration.ViewModels
{
    internal class CacheDataExchange
    {
        static CacheDataExchange()
        {
        }

        public static List<ValveInfo> GetValveInfoes()
        {
            var valveInfoes = new List<ValveInfo>();
            valveInfoes.Add(new ValveInfo() { ValveNumber = "Valve1", ValveName = "阀1" });
            valveInfoes.Add(new ValveInfo() { ValveNumber = "Valve2", ValveName = "阀2" });
            return valveInfoes;
        }

        public static List<ValveInfo> GetCameraes()
        {
            var valveInfoes = new List<ValveInfo>();
            valveInfoes.Add(new ValveInfo() { ValveNumber = "Camera1", ValveName = "相机1" });
            return valveInfoes;
        }
    }

    public class ValveInfo
    {
        public string ValveNumber { set; get; } = "";
        public string ValveName { set; get; } = "";

        public ValveInfo()
        {
        }
    }
}
