using GF_Gereric;
using Newtonsoft.JsonG.Linq;

namespace GKG
{
    /// <summary>
    /// 标定命令执行器
    /// </summary>
    public partial class CalibrationCmdExecutor
    {
        public class RequestParam
        {
            public virtual string ToJson()
            {
                return JsonObjConvert.ToJSon(this);
            }
            public virtual void FromJson(string json)
            {
                // 用当前对象的实际类型进行反序列化
                var obj = JsonObjConvert.FromJSon(json, this.GetType());
                // 将反序列化结果的属性赋值给当前对象
                foreach (var prop in this.GetType().GetProperties())
                {
                    var value = prop.GetValue(obj);
                    prop.SetValue(this, value);
                }
            }
        }
        #region 针头到
        /// <summary>
        /// 请求参数
        /// </summary>
        public class NeedleMoveTo_Param : RequestParam
            {
            /// <summary>
            /// 功能头ID
            /// </summary>
            public string FunctionHeadID { set; get; }
            /// <summary>
            /// 标定类型
            /// </summary>
            public CalibrationType CalibrationType { set; get; }

            ///<summary>
            ///三维坐标：移动到的坐标
            /// </summary>
            public Point3D Coordinates { get; set; }
            public NeedleMoveTo_Param()
            {
                FunctionHeadID = "";
                CalibrationType = CalibrationType.Offset;
                Coordinates = new Point3D();
            }
            public NeedleMoveTo_Param(string functionHeadID, CalibrationType calibrationType, Point3D coordinates)
            {
                this.FunctionHeadID = functionHeadID;
                this.CalibrationType = calibrationType;
                this.Coordinates = coordinates;
            }

        }
        /// <summary>
        /// 响应参数
        /// </summary>
        public class NeedleMoveTo_Response: ResponseBase
        {
            

        }
        #endregion

        #region 相机到
        /// <summary>
        /// 请求参数
        /// </summary>
        public class CamreaMoveTo_Param : RequestParam
            {
            /// <summary>
            /// 功能头ID
            /// </summary>
            public string FunctionHeadID { set; get; }
            /// <summary>
            /// 标定类型
            /// </summary>
            public CalibrationType CalibrationType { set; get; }

            ///<summary>
            ///三维坐标：移动到的坐标
            /// </summary>
            public Point3D Coordinates { get; set; }

            public CamreaMoveTo_Param()
            {
                FunctionHeadID = "";
                CalibrationType = CalibrationType.Offset;
                Coordinates = new Point3D();
            }
            public CamreaMoveTo_Param(string functionHeadID, CalibrationType calibrationType, Point3D coordinates)
            {
                this.FunctionHeadID = functionHeadID;
                this.CalibrationType = calibrationType;
                this.Coordinates = coordinates;
            }
        }
        /// <summary>
        /// 响应参数
        /// </summary>
        public class CamreaMoveTo_Response : ResponseBase
        {


        }
        #endregion

        #region 激光到
        /// <summary>
        /// 请求参数
        /// </summary>
        public class LaserMoveTo_Param : RequestParam
            {
            /// <summary>
            /// 功能头ID
            /// </summary>
            public string FunctionHeadID { set; get; }
            /// <summary>
            /// 标定类型
            /// </summary>
            public CalibrationType CalibrationType { set; get; }

            ///<summary>
            ///三维坐标：移动到的坐标
            /// </summary>
            public Point3D Coordinates { get; set; }
            public LaserMoveTo_Param()
            {
                FunctionHeadID = "";
                CalibrationType = CalibrationType.Offset;
                Coordinates = new Point3D();
            }
            public LaserMoveTo_Param(string functionHeadID, CalibrationType calibrationType, Point3D coordinates)
            {
                this.FunctionHeadID = functionHeadID;
                this.CalibrationType = calibrationType;
                this.Coordinates = coordinates;
            }
        }
        /// <summary>
        /// 响应参数
        /// </summary>
        public class LaserMoveTo_Response : ResponseBase
        {


        }
        #endregion

        #region 出胶
        /// <summary>
        /// 请求参数
        /// </summary>
        public class OutGlue_Param : RequestParam
            {
            /// <summary>
            /// 功能头ID
            /// </summary>
            public string FunctionHeadID { set; get; }
            /// <summary>
            /// 标定类型
            /// </summary>
            public CalibrationType CalibrationType { set; get; }
            /// <summary>
            /// 出胶点数
            /// </summary>
            public int PointCount { set; get; }
            public OutGlue_Param()
            {
                FunctionHeadID = "";
                CalibrationType = CalibrationType.Offset;
                PointCount = 0;
            }
            public OutGlue_Param(string functionHeadID, CalibrationType calibrationType, int pointCount)
            {
                this.FunctionHeadID = functionHeadID;
                this.CalibrationType = calibrationType;
                this.PointCount = pointCount;
            }
        }
        /// <summary>
        /// 响应参数
        /// </summary>
        public class OutGlue_Response : ResponseBase
        {


        }
        #endregion

        #region 标定
        /// <summary>
        /// 请求参数
        /// </summary>
        public class Calibrate_Param : RequestParam
            {
            /// <summary>
            /// 功能头ID
            /// </summary>
            public string FunctionHeadID { set; get; }
            /// <summary>
            /// 标定类型
            /// </summary>
            public CalibrationType CalibrationType { set; get; }

            /// <summary>
            /// 由标定类型决定
            /// </summary>
            public CalibrationParameters CalibrationParams { set; get; }
            public Calibrate_Param()
            {
                FunctionHeadID = "";
                CalibrationType = CalibrationType.Offset;
                CalibrationParams = new OffsetCalibrationParameters();
            }

            public Calibrate_Param(string functionHeadID, CalibrationType calibrationType, CalibrationParameters calibrationParams)
            {
                this.FunctionHeadID = functionHeadID;
                this.CalibrationType = calibrationType;
                CalibrationParams = calibrationParams;
            }

            public override void FromJson(string json)
            {
                var jObject = JObject.Parse(json);
                FunctionHeadID = jObject[nameof(FunctionHeadID)]?.Value<string>() ?? "";
                CalibrationType = Enum.TryParse<CalibrationType>(jObject[nameof(CalibrationType)]?.Value<string>(), out var calType) ? calType : CalibrationType.Offset;
                // 根据标定类型反序列化不同的标定参数
                if (jObject[nameof(CalibrationParams)] is JObject calParamsObj)
                {
                    CalibrationParams = CalibrationParameters.Create(CalibrationType);
                    CalibrationParams?.FromJson(calParamsObj.ToString());
                }
            }

            public override string ToJson()
            {
                return JsonObjConvert.ToJSon(this);
            }

            public void FromJObject(JObject? jObject)
            {
                if (jObject != null)
                    this.FromJson(jObject.ToString());
            }

            public JObject ToJObject()
            {
                return JObject.Parse(this.ToJson());
            }
        }
        /// <summary>
        /// 响应参数
        /// </summary>
        public class Calibrate_Response : ResponseBase
        {
            /// <summary>
            /// 标定类型
            /// </summary>
            public CalibrationType CalibrationType { set; get; }

            /// <summary>
            /// 由标定类型决定，参照CalibrationParameters.cs文件中的标定类输出参数定义
            /// </summary>
            public CalibrationResultBase CalibrationResults { set; get; }
            public override void FromJson(string json)
            {
                var jObject = JObject.Parse(json);
                Success = jObject[nameof(Success)]?.Value<bool>() ?? false;
                ErrorMessage = jObject[nameof(ErrorMessage)]?.Value<string>() ?? "";
                CalibrationType = CalibrationType = Enum.TryParse<CalibrationType>(jObject[nameof(CalibrationType)]?.Value<string>(), out var calType) ? calType : CalibrationType.Offset;
                CalibrationResults = CalibrationResultBase.Create(CalibrationType);
                if (jObject[nameof(CalibrationResults)] is JObject calResultsObj)
                {
                    CalibrationResults?.FromJson(calResultsObj.ToString());
                }
            }

            public override string ToJson()
            {
                return JsonObjConvert.ToJSon(this);
            }
        }
        #endregion

        #region 获取标定结果
        /// <summary>
        /// 请求参数
        /// </summary>
        public class GetCalibrationResult_Param : RequestParam
            {
            /// <summary>
            /// 功能头ID
            /// </summary>
            public string FunctionHeadID { set; get; }
            /// <summary>
            /// 标定类型
            /// </summary>
            public CalibrationType CalibrationType { set; get; }
            public GetCalibrationResult_Param()
            {
                FunctionHeadID = "";
                CalibrationType = CalibrationType.Offset;
            }
            public GetCalibrationResult_Param(string functionHeadID, CalibrationType calibrationType)
            {
                this.FunctionHeadID = functionHeadID;
                this.CalibrationType = calibrationType;
            }
        }
        /// <summary>
        /// 响应参数
        /// </summary>
        public class GetCalibrationResult_Response : ResponseBase
        {
            /// <summary>
            /// 标定类型
            /// </summary>
            public CalibrationType CalibrationType { set; get; }
            /// <summary>
            /// 由标定类型决定，参照CalibrationParameters.cs文件中的标定类输出参数定义
            /// </summary>
            public CalibrationResultBase CalibrationResults { set; get; }
            public override void FromJson(string json)
            {
                var jObject = JObject.Parse(json);
                Success = jObject[nameof(Success)]?.Value<bool>() ?? false;
                ErrorMessage = jObject[nameof(ErrorMessage)]?.Value<string>() ?? "";
                CalibrationType = CalibrationType = Enum.TryParse<CalibrationType>(jObject[nameof(CalibrationType)]?.Value<string>(), out var calType) ? calType : CalibrationType.Offset;
                CalibrationResults = CalibrationResultBase.Create(CalibrationType);
                if (jObject[nameof(CalibrationResults)] is JObject calResultsObj)
                {
                    CalibrationResults?.FromJson(calResultsObj.ToString());
                }
            }

            public override string ToJson()
            {
                return JsonObjConvert.ToJSon(this);
            }
        }
        #endregion

    }

    #region 请求响应基类

    /// <summary>
    /// 响应参数
    /// </summary>
    public class ResponseBase
    {
        /// <summary>
        /// True:成功 Faslse:失败
        /// </summary>
        public bool Success { set; get; }
        /// <summary>
        /// 返回信息，在Success为false时，详细描述出错的原因；在Success为true时，描述提示内容。
        /// </summary>
        public string ErrorMessage { set; get; } = "";
        public virtual string ToJson()
        {
            return JsonObjConvert.ToJSon(this);
        }
        public virtual void FromJson(string json)
        {
            // 用当前对象的实际类型进行反序列化
            var obj = JsonObjConvert.FromJSon(json, this.GetType());
            // 将反序列化结果的属性赋值给当前对象
            foreach (var prop in this.GetType().GetProperties())
            {
                var value = prop.GetValue(obj);
                prop.SetValue(this, value);
            }
        }
    }
    #endregion
}
