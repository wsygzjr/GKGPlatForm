using GF_Gereric;

/// <summary>
/// 标定参数
/// </summary>
namespace GKG
{
    /// <summary>
    /// 标定类型枚举
    /// </summary>
    public enum CalibrationType
    {
        ///<summary>
        ///相机比例标定
        ///</summary>
        CameraScale,

        /// <summary>
        /// 偏移标定
        /// </summary>
        Offset,

        /// <summary>
        /// 激光测高标定
        /// </summary>
        LaserHeight,

        /// <summary>
        /// 飞拍标定
        /// </summary>
        FlyingCCD,
    }

    #region 标定类输入参数

    /// <summary>
    /// 基础标定结构体(虚基类)
    /// </summary>
    public abstract class CalibrationParameters
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

        public static CalibrationParameters Create(CalibrationType Type)
        {
            switch (Type)
            {
                case CalibrationType.Offset:
                    return new OffsetCalibrationParameters();

                case CalibrationType.LaserHeight:
                    return new LaserAndFunctionHeadCalibrationParameters();

                case CalibrationType.CameraScale:
                    return new CameraScaleCalibrationParameters();

                case CalibrationType.FlyingCCD:
                    return new FlyingCalibrationParameters();
            }
            return null;
        }
    }

    /// <summary>
    /// 偏移类标定数据结构：基础标定结构
    /// </summary>
    public class OffsetCalibrationParameters : CalibrationParameters
    {
        /// <summary>
        /// string：功能头编号
        /// </summary>
        public string FunctionHeadId { get; set; } = string.Empty;

        ///<summary>
        ///三维坐标：相机坐标
        /// </summary>
        public Point3D CameraCoordinates { get; set; }

        ///<summary>
        ///三维坐标：功能头坐标
        /// </summary>
        public Point3D FunctionHeadCoordinates { get; set; }
    }

    /// <summary>
    /// 激光和功能头标定数据结构：基础标定结构
    /// </summary>
    public class LaserAndFunctionHeadCalibrationParameters : CalibrationParameters
    {
        /// <summary>
        /// string：功能头编号
        /// </summary>
        public string FunctionHeadId { get; set; } = string.Empty;

        /// <summary>
        /// 三维坐标：测高位置(相机位置)
        /// </summary>
        public Point3D LaserCoordinates { get; set; }

        /// <summary>
        /// double：蘑菇头限制高度（单位：mm）
        /// </summary>
        public double MushroomHeadLimitHeight { get; set; }

        /// <summary>
        /// double：Z轴下降速度（单位：mm/s）
        /// </summary>
        public double ZAxisDescentSpeed { get; set; }
    }

    /// <summary>
    /// 相机比例标定数据结构：基础标定结构
    /// </summary>
    public class CameraScaleCalibrationParameters : CalibrationParameters
    {
        /// <summary>
        /// string：功能头编号
        /// </summary>
        public string FunctionHeadId { get; set; } = string.Empty;

        /// <summary>
        /// 三维坐标：相机坐标
        /// </summary>
        public Point3D CameraCoordinates { get; set; }

        /// <summary>
        /// double：double：运动步距
        /// </summary>
        public double MotionStep { get; set; }

        /// <summary>
        /// byte[]：视觉模板数据
        /// </summary>
        public byte[] VisionTemplateData { get; set; } = Array.Empty<byte>();
    }

    /// <summary>
    /// 飞拍方向枚举
    /// </summary>
    public enum FlyingDirection
    {
        /// <summary>
        /// X方向
        /// </summary>
        X,

        /// <summary>
        /// Y方向
        /// </summary>
        Y
    }

    /// <summary>
    /// 飞拍标定数据结构：基础标定结构
    /// </summary>
    public class FlyingCalibrationParameters : CalibrationParameters
    {
        /// <summary>
        /// string：功能头编号
        /// </summary>
        public string FunctionHeadId { get; set; } = string.Empty;

        /// <summary>
        /// 三维坐标：飞拍位置
        /// </summary>
        public Point3D FlyingCoordinates { get; set; }

        /// <summary>
        /// double：飞拍速度（单位：mm/s）
        /// </summary>
        public double FlyingSpeed { get; set; }

        /// <summary>
        /// double：提前加速距离(单位：mm)
        /// </summary>
        public double PreAccelerationDistance { get; set; }

        /// <summary>
        /// double：飞拍首点整定时间(单位：ms)
        /// </summary>
        public double FlyingCalibrationTime { get; set; }

        /// <summary>
        /// 飞拍方向枚举：飞拍方向
        /// </summary>
        public FlyingDirection FlyingDirection { get; set; }
    }

    #endregion 标定类输入参数

    #region 标定类输出参数

    /// <summary>
    /// 基础标定结果结构(虚基类)
    /// </summary>
    public abstract class CalibrationResultBase
    {
        public virtual object Calculate(object point)
        {
            return null;
        }

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

        public static CalibrationResultBase Create(CalibrationType Type)
        {
            return Type switch
            {
                CalibrationType.CameraScale => new CameraScaleCalibrationResult(),
                CalibrationType.Offset => new OffsetCalibrationResult(),
                CalibrationType.LaserHeight => new LaserAndFunctionHeadCalibrationResult(),
                CalibrationType.FlyingCCD => new FlyingCalibrationResult(),
                _ => throw new NotSupportedException()
            };
        }
    }

    /// <summary>
    /// 偏移类标定结果数据结构：基础标定结果结构
    /// </summary>
    public class OffsetCalibrationResult : CalibrationResultBase
    {
        ///<summary>
        ///string：功能头编号
        ///</summary>
        public string FunctionHeadId { get; set; } = string.Empty;

        /// <summary>
        /// 二维坐标：偏移值
        /// </summary>
        public Point2D OffsetValue { get; set; }

        public override object Calculate(object point)
        {
            Point3D CCDPosition = (Point3D)point;
            return new Point3D(CCDPosition.X + OffsetValue.X, CCDPosition.Y + OffsetValue.Y, CCDPosition.Z);
        }
    }

    /// <summary>
    /// 激光和功能头标定结果数据结构：基础标定结果结构
    /// </summary>
    public class LaserAndFunctionHeadCalibrationResult : CalibrationResultBase
    {
        /// <summary>
        /// string：功能头编号
        /// </summary>
        public string FunctionHeadId { get; set; } = string.Empty;

        /// <summary>
        /// double：阀碰到平面的Z轴位置
        /// </summary>
        public double ValveZAxisPosition { get; set; }

        /// <summary>
        /// double：激光到平面的数值
        /// </summary>
        public double LaserToPlaneValue { get; set; }

        public MeasureHeightPositiveDir MeasureHeightPositiveDir { get; set; }

        public override object Calculate(object height)
        {
            double dCurLaserValue = (double)height;
            double destPos = 0;
            if (MeasureHeightPositiveDir == MeasureHeightPositiveDir.Up)
            {
                //上- 下+
                double dOrgZPosd = ValveZAxisPosition - LaserToPlaneValue;
                destPos = dOrgZPosd + dCurLaserValue;
            }
            else
            {
                //上+ 下-
                double dOrgZPosd = ValveZAxisPosition + LaserToPlaneValue;
                destPos = dOrgZPosd - dCurLaserValue;
            }
            return destPos;
        }
    }

    /// <summary>
    /// 相机比例标定结果数据结构：基础标定结果结构
    /// </summary>
    public class CameraScaleCalibrationResult : CalibrationResultBase
    {
        /// <summary>
        /// string：功能头编号
        /// </summary>
        public string FunctionHeadId { get; set; } = string.Empty;

        /// <summary>
        /// double：X方向像素比例
        /// </summary>
        public double XPixelScale { get; set; }

        /// <summary>
        /// double：Y方向像素比例
        /// </summary>
        public double YPixelScale { get; set; }

        /// <summary>
        /// 像素转机械坐标变换矩阵
        /// </summary>
        public double[] hvHomMat2D = new double[9];

        /// <summary>
        /// 机械转像素坐标变换矩阵
        /// </summary>
        public double[] worldHomMat2D = new double[9];

        public override object Calculate(object point)
        {
            Point2D pixelPoint = (Point2D)point;
            //使用像素转机械坐标变换矩阵进行转换
            double x = hvHomMat2D[0] * pixelPoint.X + hvHomMat2D[1] * pixelPoint.Y + hvHomMat2D[2];
            double y = hvHomMat2D[3] * pixelPoint.X + hvHomMat2D[4] * pixelPoint.Y + hvHomMat2D[5];
            double w = hvHomMat2D[6] * pixelPoint.X + hvHomMat2D[7] * pixelPoint.Y + hvHomMat2D[8];
            return new Point2D(x / w, y / w);
        }
    }

    /// <summary>
    /// 飞拍标定结果数据结构：基础标定结果结构
    /// </summary>
    public class FlyingCalibrationResult : CalibrationResultBase
    {
        /// <summary>
        /// string：功能头编号
        /// </summary>
        public string FunctionHeadId { get; set; } = string.Empty;

        /// <summary>
        /// double：正方向飞拍角度
        /// </summary>
        public double PositiveFlyingAngle { get; set; }

        /// <summary>
        /// double：负方向飞拍角度
        /// </summary>
        public double NegativeFlyingAngle { get; set; }

        /// <summary>
        /// double：正方向飞拍X偏移
        /// </summary>
        public double PositiveFlyingXOffset { get; set; }

        /// <summary>
        /// double：正方向飞拍Y偏移
        /// </summary>
        public double PositiveFlyingYOffset { get; set; }

        /// <summary>
        /// double：负方向飞拍X偏移
        /// </summary>
        public double NegativeFlyingXOffset { get; set; }

        /// <summary>
        /// double：负方向飞拍Y偏移
        /// </summary>
        public double NegativeFlyingYOffset { get; set; }

        /// <summary>
        /// double：飞拍速度（单位：mm/s）
        /// </summary>
        public double FlyingSpeed { get; set; }

        /// <summary>
        /// double：提前加速距离(单位：mm)
        /// </summary>
        public double PreAccelerationDistance { get; set; }

        /// <summary>
        /// double：飞拍首点整定时间(单位：ms)
        /// </summary>
        public double FlyingCalibrationTime { get; set; }

        /// <summary>
        /// 飞拍方向枚举：飞拍方向
        /// </summary>
        public FlyingDirection FlyingDirection { get; set; }
    }

    /// <summary>
    /// 标定结果结构
    /// </summary>
    public class CalibrationResults
    {
        /// <summary>
        /// 标定类型枚举：标定类型
        /// </summary>
        public CalibrationType CalibrationType { get; set; }

        /// <summary>
        /// byte[]：标定结果
        /// </summary>
        public byte[]? CalibrationResult { get; set; }
    }

    #endregion 标定类输出参数
}