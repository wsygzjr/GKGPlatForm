using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GKG
{
    public class LoaderModuleParameters
    {
        #region 气缸、电机推杆配置页面

        public class CylinderPushRodFactoryParameters
        {
            public bool IsSupportHasMaterialCheck { get; set; }

            public bool IsSupportMaterialJamCheck { get; set; }
        }
        public class CylinderPushRodInitParameters
        {
            public ECylinderType CylinderType { get; set; }
            public CylinderInitParameters CylinderInitParameters { get; set; } = new CylinderInitParameters();
            public double PushCylinderSolenoidValveLiftDelay { get; set; }
        }
        public class MotorPushRodFactoryParameters
        {
            public bool IsSupportJamCheck { get; set; }

            public string deviceId { get; set; }
            public AxisBinding MotorRodAxis { get; set; }
        }
        public class MotorPushRodInitParameters
        {
            public double Speed { get; set; }
            public double Position { get; set; }
            public double PushDistance { get; set; }
            public double PushAxisSpeed { get; set; }
            public double PushAxisAcceleration { get; set; }
        }

        public class MotorPushRodPageInitParameters
        {
            public int PusherPhysicalAxis { get; set; }
            public double PushRodTimeout { get; set; }
            public double MaterialJamDetectionTime { get; set; }
        }

        public class CylinderPushRodPageInitParameters
        {
            public ECylinderType CylinderType { get; set; }
            public CylinderInitParameters CylinderInitParameters { get; set; } = new CylinderInitParameters();
            public double PushCylinderSolenoidValveLiftDelay { get; set; }
        }
        #endregion

    }
}
