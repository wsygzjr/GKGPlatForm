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
            public class Path : CmdBase
            {
                public override CmdType CmdType => CmdType.Path;
                /// <summary>
                /// 轨迹参数
                /// </summary>
                public LinearMotionTrajectory motionTrajectory = new LinearMotionTrajectory();
                /// <summary>
                /// 加工前后工艺参数：工艺参数
                /// </summary>
                public PrePostProcessingParameters? PrePostProcessingParameters { get; set; }

            }
        }
    }
}