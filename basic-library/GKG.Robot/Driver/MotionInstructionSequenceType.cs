namespace GKG
{
    namespace MotionControl
    {
        /// <summary>
        /// 指令序列执行类型。
        /// 用于显式声明 Robot 应按哪种模式解释和执行这段运控指令序列。
        /// </summary>
        public enum MotionInstructionSequenceType
        {
            /// <summary>
            /// 一步一步执行
            /// </summary>
            StepByStep = 0,
            /// <summary>
            /// 连续插补
            /// </summary>
            ContinuousInterpolationMotion = 1,
        }
    }
}
