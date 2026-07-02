using Griffins.ImeIOT;

namespace GKG
{
    namespace MM
    {
        /// <summary>
        /// Path
        /// </summary>
        public partial class MMCmdExecutor : IMMCmdExecutor
        {
            public class Dot : CmdBase
            {
                public override CmdType CmdType => CmdType.Dot;
                /// <summary>
                /// 轨迹参数
                /// </summary>
                public DotMotionTrajectory motionTrajectory = new DotMotionTrajectory();
                /// <summary>
                /// 加工前后工艺参数：工艺参数
                /// </summary>
                public PrePostProcessingParameters? PrePostProcessingParameters { get; set; }

            }
        }
    }
}