using GKG.MotionControl;
using System.Collections.Concurrent;

namespace GKG.SubMM
{
    public class Calibration
    {
        public string ID;
        public CalibrationType Type;
    }

    public class CalibrationInitParams
    {
        public string ID;
        public byte[] InitParams;
    }

    public class CalibrationResults
    {
        public string ID;
        public CalibrationResultBase calibrationResult;
    }

    public class CalibrationDictionary : ConcurrentDictionary<string, CalibrationBase>
    {
        public CalibrationBase Find(string functionHeadId, CalibrationType type)
        {
            foreach(var item in this)
            {
                if (item.Value.FunctionHead == functionHeadId && item.Value.Type == type)
                {
                    return item.Value;
                }
            }
            return null;
        }
    }

    public class GetCalibrationResultParams
    {
        public string FunctionHeadID;
        public CalibrationType Type;
    }

    public class FunctionHeadInfos
    {
        public string ID;
        public string Name;
    }

}
