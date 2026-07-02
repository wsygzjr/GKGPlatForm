using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GKG
{
    public enum ECylinderPosType
    {
        Stretch,
        Retract,
    }

    public enum EThermostatType
    {
        ThermostatType_OML,
        ThermostatType_MZ,
        ThermostatType_YLoong2000,
        ThermostatType_YuDian,
    }

    public class CylinderPositionNumberInfo
    {
        public int PositionNumber { get; set; }
        public bool TelescopingState { get; set; }
    }

    public class CylinderPositionNumberInfoList : List<CylinderPositionNumberInfo>
    {
    }

    public enum EMotionType
    {
        MoveToTargetPos,
        MoveToTargetCoord
    }

    public class MotorPositionNumberInfo
    {
        public int PositionNumber { get; set; }

        public double Coord { get; set; }
    }

    public class MotorPositionNumberInfoList : List<MotorPositionNumberInfo>
    {
    }

    public class MotorStraightMoveInitParameters
    {
        public EMotionType MotionType { get; set; }
        public MotorPositionNumberInfoList MotorPositionNumberInfos { get; set; }
        public MotionControlAxisInitParams MotionControlAxisInitParameters { get; set; }
    }

    public class PositionNumCoordInfo
    {
        public int PositionNumber { get; set; }
        public List<AxisConstantValues> AxisConstantValuesList { get; set; }
    }
}