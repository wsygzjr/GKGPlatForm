using GF_Gereric;

namespace GKG
{
    namespace MotionControl
    {
        public class OffsetCalibrationInitParams
        {
            /// <summary>
            /// 功能头ID
            /// </summary>
            public string FunctionHead = "";
        }

        /// <summary>
        /// 偏移标定
        /// </summary>
        public class OffsetCalibration : CalibrationBase
        {
            /// <summary>
            /// 标定类型
            /// </summary>
            public override CalibrationType Type => CalibrationType.Offset;

            private OffsetCalibrationInitParams offsetCalibrationInitParams;

            /// <summary>
            /// 标定参数
            /// </summary>
            private OffsetCalibrationParameters calibrationParameter = new OffsetCalibrationParameters();

            /// <summary>
            /// 标定结果
            /// </summary>
            private OffsetCalibrationResult calibrationResult = new OffsetCalibrationResult();

            /// <summary>
            /// 初始化
            /// </summary>
            /// <param name="initParams">初始化参数</param>
            public override void Init(byte[] initParams)
            {
                offsetCalibrationInitParams = JsonObjConvert.FromJSonBytes<OffsetCalibrationInitParams>(initParams);
                functionHead = offsetCalibrationInitParams.FunctionHead;
            }

            /// <summary>
            /// 设置标定参数
            /// </summary>
            /// <param name="calibrationParameters"></param>
            public override void SetCalibrationParams(CalibrationParameters calibrationParameters)
            {
                calibrationParameter = (OffsetCalibrationParameters)calibrationParameters;
            }

            /// <summary>
            /// 标定
            /// </summary>
            /// <returns>偏移标定结果</returns>
            public override OffsetCalibrationResult Calibrate()
            {
                calibrationResult.FunctionHeadId = calibrationParameter.FunctionHeadId;
                calibrationResult.OffsetValue.X = calibrationParameter.CameraCoordinates.X - calibrationParameter.FunctionHeadCoordinates.X;
                calibrationResult.OffsetValue.Y = calibrationParameter.CameraCoordinates.Y - calibrationParameter.FunctionHeadCoordinates.Y;

                return calibrationResult;
            }

            /// <summary>
            /// 获取标定结果
            /// </summary>
            /// <returns>偏移标定结果</returns>
            public override OffsetCalibrationResult GetCalibrationResult()
            {
                return calibrationResult;
            }
        }
    }
}