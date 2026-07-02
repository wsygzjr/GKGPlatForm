using GKG.ElectronicControl;
using GKG.Vision;

namespace GKG
{
    namespace MotionControl
    {
        public class CalibrationBase
        {
            protected CalibrationRuntimeContext runtimeContext = new CalibrationRuntimeContext();

            protected IMotionControlBase MotionControl => runtimeContext.MotionControl ?? throw new InvalidOperationException("MotionControl 未注入");

            protected IRobotDriver RobotDriver => runtimeContext.RobotDriver ?? throw new InvalidOperationException("RobotDriver 未注入");

            protected IVisionDriver VisionDriver => runtimeContext.VisionDriver ?? throw new InvalidOperationException("VisionDriver 未注入");

            protected IMotionCalculatorDriver MotionCalculatorDriver => runtimeContext.MotionCalculatorDriver ?? throw new InvalidOperationException("MotionCalculatorDriver 未注入");

            protected RobotExecutionContext RobotExecutionContext => runtimeContext.RobotExecutionContext ?? throw new InvalidOperationException("RobotExecutionContext 未注入");

            /// <summary>
            /// 标定类型
            /// </summary>
            public virtual CalibrationType Type => CalibrationType.Offset;

            protected virtual string id { get; set; } = "";
            public virtual string ID => id;

            protected virtual string functionHead { get; set; } = "";
            public virtual string FunctionHead => functionHead;

            public virtual void SetRuntimeContext(CalibrationRuntimeContext context)
            {
                runtimeContext = context ?? new CalibrationRuntimeContext();
            }

            /// <summary>
            /// 初始化
            /// </summary>
            /// <param name="initParams">初始化参数</param>
            public virtual void Init(byte[] initParams)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// 设置标定参数
            /// </summary>
            /// <param name="calibrationParameters"></param>
            public virtual void SetCalibrationParams(CalibrationParameters calibrationParameters)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// 标定
            /// </summary>
            /// <returns>偏移标定结果</returns>
            public virtual CalibrationResultBase Calibrate()
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// 获取标定结果
            /// </summary>
            /// <returns>偏移标定结果</returns>
            public virtual CalibrationResultBase GetCalibrationResult()
            {
                throw new NotImplementedException();
            }
        }
    }
}
