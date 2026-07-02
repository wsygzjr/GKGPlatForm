using GKG;
using System;

namespace GKG.SubMM.TransportMechanismModule
{
    /// <summary>运输机构初始化参数。</summary>
    public class TransportMechanismInitCfg
    {
        /// <summary>绑定的轴 ID，供当前运输机构对应侧的 Z 轴运动使用。</summary>
        public Guid BindingAxisId { get; set; } = Guid.Empty;

       

        /// <summary>初始化默认轴运动参数，配方未配置时可作为回退值。</summary>
        public NonProcessingTrajectoryParameters AxisMotionParameters { get; set; } = new NonProcessingTrajectoryParameters();

        /// <summary>按值复制初始化参数。</summary>
        public void CopyFrom(TransportMechanismInitCfg source)
        {
            BindingAxisId = source?.BindingAxisId ?? Guid.Empty;
            AxisMotionParameters = source?.AxisMotionParameters ?? new NonProcessingTrajectoryParameters();
        }
    }
}
