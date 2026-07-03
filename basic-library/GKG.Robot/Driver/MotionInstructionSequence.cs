namespace GKG
{
    namespace MotionControl
    {
        /// <summary>
        /// 运控指令序列。
        /// 在原始 MotionInstructionBase[] 外补一层序列级语义，避免 Robot 只能依赖扫描指令内容猜执行方式。
        /// </summary>
        public class MotionInstructionSequence
        {
            public MotionInstructionSequenceType SequenceType { get; set; } = MotionInstructionSequenceType.StepByStep;

            public MotionInstructionBase[] Instructions { get; set; } = Array.Empty<MotionInstructionBase>();

            public byte[]? ExtendedParameters { get; set; }
        }
    }
}
