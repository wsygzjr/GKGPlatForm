using GKG;

namespace GKG.SubMM.TransportMechanismModule
{
    /// <summary>运输机构配方参数。</summary>
    public class TransportMechanismPPCfg
    {
        /// <summary>轴运动轨迹参数，优先作为运行时运动参数来源。</summary>
        public NonProcessingTrajectoryParameters AxisMotionParameters { get; set; }

        /// <summary>是否启用按步距移动；关闭时步进距离可由槽位位置差推算。</summary>
        public bool UseStepDistance { get; set; }

        /// <summary>启用步距时单次移动距离。</summary>
        public double StepDistance { get; set; }

        /// <summary>按值复制配方参数。</summary>
        public void CopyFrom(TransportMechanismPPCfg source)
        {
            AxisMotionParameters = source?.AxisMotionParameters ?? new NonProcessingTrajectoryParameters();
            UseStepDistance = source?.UseStepDistance ?? false;
            StepDistance = source?.StepDistance ?? 0;
        }
    }
}
