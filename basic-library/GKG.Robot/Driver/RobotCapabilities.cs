namespace GKG
{
    namespace MotionControl
    {
        /// <summary>
        /// 机器人驱动能力描述。
        /// </summary>
        public class RobotCapabilities
        {
            public bool SupportsSingleAxisMove { get; set; } = true;

            public bool SupportsTwoAxisLinearInterpolation { get; set; }

            public bool SupportsThreeAxisLinearInterpolation { get; set; }

            public bool SupportsTwoAxisCircularInterpolation { get; set; }

            public bool SupportsContinuousInterpolationMotion { get; set; }

            public bool SupportsPositionComparison2D { get; set; }

            public bool SupportsManualPositionComparison { get; set; }

            public bool SupportsPositionLatch { get; set; }
        }
    }
}
