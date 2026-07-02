using GF_Gereric;
using System.Reflection.Emit;

namespace GKG
{
    public class VisionInitParameters
    {
        /// <summary>
        /// 视觉驱动名称。
        /// 默认使用 GVision。
        /// </summary>
        public string DriverName { get; set; } = "GVision";

        public int CameraID { get; set; } = 0;
        /// <summary>
        /// 是否启用飞拍
        /// </summary>
        public bool EnableFlying {  get; set; }
    }
    /// <summary>
    /// 相机参数
    /// </summary>
    public class CameraParameters
    {
        /// <summary>
        /// 相机索引
        /// </summary>
        public int CameraIndex { get; set; }

        /// <summary>
        /// 曝光阈值
        /// </summary>
        public int ExposureThreshold { get; set; }

        /// <summary>
        /// 曝光值
        /// </summary>
        public int ExposureValue { get; set; }

        /// <summary>
        /// 对比度阈值
        /// </summary>
        public int ContrastThreshold { get; set; }

        /// <summary>
        /// 对比度值
        /// </summary>
        public int ContrastValue { get; set; }
    }

    /// <summary>
    /// 光源参数
    /// </summary>
    public class LightParameters
    {
        /// <summary>
        /// 红光
        /// </summary>
        public int Red { get; set; }

        /// <summary>
        /// 绿光
        /// </summary>
        public int Green { get; set; }

        /// <summary>
        /// 蓝光
        /// </summary>
        public int Blue { get; set; }

        /// <summary>
        /// 深度
        /// </summary>
        public int Depth { get; set; }
    }

    /// <summary>
    /// 匹配模式
    /// </summary>
    public enum MatchingMode
    {
        /// <summary>
        /// 形状匹配
        /// </summary>
        ShapeMatching = 0,

        /// <summary>
        /// 灰度匹配
        /// </summary>
        GrayLevelMatching = 1,
    }

    /// <summary>
    /// 获取平均灰度值模式
    /// </summary>
    public enum GetAverageGrayLevelMode
    {
        /// <summary>
        /// 模板框
        /// </summary>
        Model = 0,

        /// <summary>
        /// 搜索框
        /// </summary>
        Search = 1,

        /// <summary>
        /// 自定义
        /// </summary>
        UserDefined = 2,
    }

    /// <summary>
    /// Mark脚本参数
    /// </summary>
    public class MarkModelParameters
    {
        /// <summary>
        /// Mark通过分数
        /// </summary>
        public double Score { get; set; }

        /// <summary>
        /// 角度范围
        /// </summary>
        public double Angle { get; set; }

        /// <summary>
        /// 查找比例(%)
        /// </summary>
        public int SearchRadio { get; set; }

        /// <summary>
        /// 匹配模式
        /// </summary>
        public MatchingMode MatchingMode { get; set; }

        /// <summary>
        /// 边缘黑白阈值
        /// </summary>
        public int EdgeBWThreshold { get; set; }

        /// <summary>
        /// 边缘长度阈值
        /// </summary>
        public int EdgeLengthThreshold { get; set; }

        /// <summary>
        /// 是否启用平均灰度值筛选
        /// </summary>
        public bool AverageGrayLevelFilteringEnabled { get; set; }

        /// <summary>
        /// 获取平均灰度值方式
        /// </summary>
        public GetAverageGrayLevelMode AverageGrayLevelMode { get; set; }

        /// <summary>
        /// 平均灰度标准值
        /// </summary>
        public int AverageGrayLevelValue { get; set; }

        /// <summary>
        /// 平均灰度值上下限比例
        /// </summary>
        public int AverageGrayLevelLimitRadio { get; set; }
    }

    /// <summary>
    /// BlobMark脚本参数
    /// </summary>
    public class BlobMarkParameters
    {
        /// <summary>
        /// 是否开启Blob定位
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// 是否矩形Mark
        /// </summary>
        public bool IsRectangleMark { get; set; }

        /// <summary>
        /// 低阈值最小值
        /// </summary>
        public int LowThresholdMin { get; set; }

        /// <summary>
        /// 低阈值最大值
        /// </summary>
        public int LowThresholdMax { get; set; }

        /// <summary>
        /// 高阈值最小值
        /// </summary>
        public int HighThresholdMin { get; set; }

        /// <summary>
        /// 高阈值最大值
        /// </summary>
        public int HighThresholdMax { get; set; }

        /// <summary>
        /// 检测直径(mm)
        /// </summary>
        public double DetectDiameter { get; set; }

        /// <summary>
        /// 杂质过滤
        /// </summary>
        public int NoiseFilter { get; set; }

        /// <summary>
        /// 目标缝合
        /// </summary>
        public int TargetSewing { get; set; }

        /// <summary>
        /// 上下限比例(%)
        /// </summary>
        public int LimitRadio { get; set; }

        /// <summary>
        /// 圆度(0.0-1.0)
        /// </summary>
        public double Roundness { get; set; }
    }

    /// <summary>
    /// ROI类型
    /// </summary>
    public enum ROIType
    {
        /// <summary>
        /// 矩形
        /// </summary>
        Rect = 0,

        /// <summary>
        /// 圆形
        /// </summary>
        Circle = 1,

        /// <summary>
        /// 多边形
        /// </summary>
        Polygon = 2
    }

    /// <summary>
    /// ROI基类
    /// </summary>
    public class ROIBase
    {
        public virtual ROIType ROIType { get; }
    }

    /// <summary>
    /// 矩形ROI
    /// </summary>
    public class ROIRect : ROIBase
    {
        public override ROIType ROIType => ROIType.Rect;

        /// <summary>
        /// 左上角顶点
        /// </summary>
        public Point2D LeftTopPoint { get; set; }

        /// <summary>
        /// 右下角顶点
        /// </summary>
        public Point2D RightBottomPoint { get; set; }

        /// <summary>
        /// 角度
        /// </summary>
        public double Angle { get; set; }
    }

    /// <summary>
    /// 圆形ROI
    /// </summary>
    public class ROICircle : ROIBase
    {
        public override ROIType ROIType => ROIType.Rect;

        /// <summary>
        /// 圆心
        /// </summary>
        public Point2D CircleCenter { get; set; }

        /// <summary>
        /// 半径
        /// </summary>
        public double Radius { get; set; }
    }

    /// <summary>
    /// 多边形ROI
    /// </summary>
    public class ROIPolygon : ROIBase
    {
        public override ROIType ROIType => ROIType.Polygon;

        /// <summary>
        /// 多边形顶点列表
        /// </summary>
        public List<Point2D> VertexPoints { get; set; } = new List<Point2D>();
    }

    /// <summary>
    /// Mark脚本参数
    /// </summary>
    public class MarkScriptParameters
    {
        /// <summary>
        /// mark脚本参数
        /// </summary>
        public MarkModelParameters MarkModelParameters { get; set; } = new MarkModelParameters();

        /// <summary>
        /// blob参数
        /// </summary>
        public BlobMarkParameters BlobMarkParameters { get; set; } = new BlobMarkParameters();

        /// <summary>
        /// ROI区域搜索框
        /// </summary>
        public ROIBase? ROISearchRegion { get; set; }

        /// <summary>
        /// ROI区域模板框
        /// </summary>
        public ROIBase[]? ROIModelRegions { get; set; }
    }

    /// <summary>
    /// 搜索mark参数
    /// </summary>
    public class SearchMarkParams
    {
        public string ModelPath { get; set; } = "";
        public double mmPerPixel { get; set; }

        /// <summary>
        /// 相机参数
        /// </summary>
        public CameraParameters CameraParameters { get; set; } = new CameraParameters();

        /// <summary>
        /// 光源参数
        /// </summary>
        public LightParameters LightParameters { get; set; } = new LightParameters();

        /// <summary>
        /// Mark脚本参数
        /// </summary>
        public MarkScriptParameters ScriptParameters { get; set; } = new MarkScriptParameters();
    }

    public enum PolarityType
    {
        /// <summary>
        /// 白底黑点
        /// </summary>
        WhiteBgBlackPixels,

        /// <summary>
        /// 黑底白点
        /// </summary>
        BlackBgWhitePixels
    }

    /// <summary>
    /// 检测通过类型
    /// </summary>
    public enum DetectionPassType
    {
        /// <summary>
        /// 总面积检测
        /// </summary>
        TotalAreaDetection,

        /// <summary>
        /// 胶点个数检测
        /// </summary>
        GlueSpotCountDetection,

        /// <summary>
        /// 最大胶点面积检测
        /// </summary>
        MaxGlueSpotAreaDetection,

        /// <summary>
        /// 胶点直径个数偏移检测
        /// </summary>
        GlueSpotDetection,

        /// <summary>
        /// 禁用检测
        /// </summary>
        DisableDection,

        /// <summary>
        /// 检测漏点胶
        /// </summary>
        MissingGlueSpotDetection
    }

    public class DetectionPassBase
    {
        /// <summary>
        /// 通过条件
        /// </summary>
        public virtual DetectionPassType PassType { get; }

        /// <summary>
        /// 检测条件类型
        /// </summary>
        public virtual DetectionConditionType ConditionType { get; set; }
    }

    public enum DetectionConditionType
    {
        /// <summary>
        /// 区间
        /// </summary>
        Range,

        /// <summary>
        /// 小于
        /// </summary>
        LessThan,

        /// <summary>
        /// 大于
        /// </summary>
        GreaterThan,

        /// <summary>
        /// 等于
        /// </summary>
        EqualTo
    }

    public class TotalAreaDetectionPass : DetectionPassBase
    {
        /// <summary>
        /// 通过条件
        /// </summary>
        public override DetectionPassType PassType => DetectionPassType.TotalAreaDetection;

        /// <summary>
        /// 检测条件类型
        /// </summary>
        private DetectionConditionType _conditionType;

        public override DetectionConditionType ConditionType
        {
            get => _conditionType;
            set
            {
                if (value == DetectionConditionType.EqualTo)
                {
                    _conditionType = DetectionConditionType.Range;
                }
                else
                {
                    _conditionType = value;
                }
            }
        }

        /// <summary>
        /// 最小面积
        /// </summary>
        public double MinArea { get; set; }

        /// <summary>
        /// 最大面积
        /// </summary>
        public double MaxArea { get; set; }
    }

    /// <summary>
    /// 点胶个数检测通过条件
    /// </summary>
    public class GlueSpotCountDetectionPass : DetectionPassBase
    {
        /// <summary>
        /// 通过条件
        /// </summary>
        public override DetectionPassType PassType => DetectionPassType.GlueSpotCountDetection;

        /// <summary>
        /// 检测条件类型
        /// </summary>
        private DetectionConditionType _conditionType;

        public override DetectionConditionType ConditionType
        {
            get => _conditionType;
            set
            {
                /// 面积不能用等于判断 强制改为区间判断
                if (value == DetectionConditionType.EqualTo)
                {
                    _conditionType = DetectionConditionType.Range;
                }
                else
                {
                    _conditionType = value;
                }
            }
        }

        /// <summary>
        /// 最小个数
        /// </summary>
        public int MinCount { get; set; }

        /// <summary>
        /// 最大个数
        /// </summary>
        public int MaxCount { get; set; }
    }

    /// <summary>
    /// 最大点胶面积检测通过条件
    /// </summary>
    public class MaxGlueSpotAreaDetectionPass : DetectionPassBase
    {
        /// <summary>
        /// 通过条件
        /// </summary>
        public override DetectionPassType PassType => DetectionPassType.MaxGlueSpotAreaDetection;

        /// <summary>
        /// 检测条件类型
        /// </summary>
        private DetectionConditionType _conditionType;

        public override DetectionConditionType ConditionType
        {
            get => _conditionType;
            set
            {
                /// 面积不能用等于判断 强制改为区间判断
                if (value == DetectionConditionType.EqualTo)
                {
                    _conditionType = DetectionConditionType.Range;
                }
                else
                {
                    _conditionType = value;
                }
            }
        }

        /// <summary>
        /// 最小面积
        /// </summary>
        public double MinArea { get; set; }

        /// <summary>
        /// 最大面积
        /// </summary>
        public double MaxArea { get; set; }
    }

    /// <summary>
    /// 胶点直径偏移检测通过条件
    /// </summary>
    public class GlueSpotDetectionPass : DetectionPassBase
    {
        /// <summary>
        /// 通过条件
        /// </summary>
        public override DetectionPassType PassType => DetectionPassType.GlueSpotDetection;

        /// <summary>
        /// 检测条件类型
        /// </summary>
        private DetectionConditionType _conditionType;

        public override DetectionConditionType ConditionType
        {
            get => _conditionType;
            set
            {
                if (value != DetectionConditionType.Range)
                {
                    _conditionType = DetectionConditionType.Range;
                }
                else
                {
                    _conditionType = value;
                }
            }
        }

        /// <summary>
        /// 胶点直径下限(mm)
        /// </summary>
        public double MinDiameter { get; set; }

        /// <summary>
        /// 胶点直径上限(mm)
        /// </summary>
        public double MaxDiameter { get; set; }

        /// <summary>
        /// 点胶偏移允许值(um)X-
        /// </summary>
        public double MaxOffsetXNeg { get; set; }

        /// <summary>
        /// 点胶偏移允许值(um)X+
        /// </summary>
        public double MaxOffsetXPos { get; set; }

        /// <summary>
        /// 点胶偏移允许值(um)Y-
        /// </summary>
        public double MaxOffsetYNeg { get; set; }

        /// <summary>
        /// 点胶偏移允许值(um)Y+
        /// </summary>
        public double MaxOffsetYPos { get; set; }
    }

    /// <summary>
    /// Disable检测通过条件
    /// </summary>
    public class DisableDetectionPass : DetectionPassBase
    {
        /// <summary>
        /// 通过条件
        /// </summary>
        public override DetectionPassType PassType => DetectionPassType.DisableDection;

        /// <summary>
        /// 检测条件类型
        /// </summary>
        private DetectionConditionType _conditionType;

        public override DetectionConditionType ConditionType
        {
            get => _conditionType;
            set
            {
                if (value != DetectionConditionType.EqualTo)
                {
                    _conditionType = DetectionConditionType.EqualTo;
                }
                else
                {
                    _conditionType = value;
                }
            }
        }
    }

    /// <summary>
    /// 漏点胶检测通过条件
    /// </summary>
    public class MissingGlueSpotDetectionPass : DetectionPassBase
    {
        /// <summary>
        /// 通过条件
        /// </summary>
        public override DetectionPassType PassType => DetectionPassType.MissingGlueSpotDetection;

        /// <summary>
        /// 检测条件类型
        /// </summary>
        private DetectionConditionType _conditionType;

        public override DetectionConditionType ConditionType
        {
            get => _conditionType;
            set
            {
                if (value == DetectionConditionType.EqualTo)
                {
                    _conditionType = DetectionConditionType.Range;
                }
                else
                {
                    _conditionType = value;
                }
            }
        }

        /// <summary>
        /// 最大区域下限
        /// </summary>
        public double MinArea { get; set; }

        /// <summary>
        /// 最大区域上限
        /// </summary>
        public double MaxArea { get; set; }
    }

    /// <summary>
    /// 2D脚本参数
    /// </summary>
    public class TwoDScriptParameters
    {
        /// <summary>
        /// 低阈值
        /// </summary>
        public int LowThreshold { get; set; }

        /// <summary>
        /// 高阈值
        /// </summary>
        public int HighThreshold { get; set; }

        /// <summary>
        /// 矩形度(0.0-1.0)
        /// </summary>
        public double Rectangularity { get; set; }

        /// <summary>
        /// 一维测量开关
        /// </summary>
        public bool OneDMeasurementEnabled { get; set; }

        /// <summary>
        /// 杂质处理开关
        public bool NoiseFilterEnabled { get; set; }

        /// </summary>
        /// 杂质过滤值
        /// </summary>
        public double NoiseFilterValue { get; set; }

        /// <summary>
        /// 目标缝合值
        /// </summary>
        public double TargetSewingValue { get; set; }

        /// <summary>
        /// 面积筛选开关
        /// </summary>
        public bool AreaFilteringEnabled { get; set; }

        /// <summary>
        /// 最小面积
        /// </summary>
        public double AreaFilteringMinValue { get; set; }

        /// <summary>
        /// 最大面积
        /// </summary>
        public double AreaFilteringMaxValue { get; set; }

        /// <summary>
        /// 填充孔洞开关
        /// </summary>
        public bool FillHolesEnabled { get; set; }

        /// <summary>
        /// 极性
        /// </summary>
        public PolarityType Polarity { get; set; }

        /// <summary>
        /// 检测通过条件
        /// </summary>
        public DetectionPassBase DetectionPass { get; set; } = new DisableDetectionPass();

        /// <summary>
        /// ROI列表
        /// </summary>
        public List<ROIBase> ROIList { get; set; } = new List<ROIBase>();
    }

    /// <summary>
    /// 搜索Blob参数
    /// </summary>
    public class SearchBlobParams
    {
        public string ModelPath { get; set; } = "";

        /// <summary>
        /// 相机参数
        /// </summary>
        public CameraParameters CameraParameters { get; set; } = new CameraParameters();

        /// <summary>
        /// 光源参数
        /// </summary>
        public LightParameters LightParameters { get; set; } = new LightParameters();

        /// <summary>
        /// 2D脚本参数
        /// </summary>
        public TwoDScriptParameters ScriptParameters { get; set; } = new TwoDScriptParameters();
    }

    /// <summary>
    /// 扫码参数
    /// </summary>
    public class ScanCodeParams
    {
        public int CameraIndex { get; set; }
        public string ModelPath { get; set; } = "";
    }

    /// <summary>
    /// 扫描Mark点结果
    /// </summary>
    public class SearchMarkResult
    {
        /// <summary>
        /// 是否OK
        /// </summary>
        public bool IsOk { get; set; }

        /// <summary>
        /// Mark偏移
        /// </summary>
        public Point2D Offset { get; set; }

        /// <summary>
        /// 角度
        /// </summary>
        public double Angle { get; set; }

        /// <summary>
        /// 缩放比例X
        /// </summary>
        public double ScaleX { get; set; }

        /// <summary>
        /// 缩放比例Y
        /// </summary>
        public double ScaleY { get; set; }
    }

    public class TwoDResultBase
    {
        /// <summary>
        /// 检测条件类型
        /// </summary>
        private DetectionPassType _detectionPassType;

        public virtual DetectionPassType PassType
        {
            get => _detectionPassType;
        }

        public bool IsOk { get; set; }

        public static TwoDResultBase Create(DetectionPassType type)
        {
            return type switch
            {
                DetectionPassType.TotalAreaDetection => new TotalAreaTwoDResult(),
                DetectionPassType.GlueSpotCountDetection => new GlueSpotCountTwoDResult(),
                DetectionPassType.MaxGlueSpotAreaDetection => new MaxGlueSpotAreaTwoDResult(),
                DetectionPassType.GlueSpotDetection => new GlueSpotTwoDResult(),
                DetectionPassType.MissingGlueSpotDetection => new MissingGlueSpotTwoDResult(),
                DetectionPassType.DisableDection => new DisableTwoDResult(),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
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
    }

    public class TotalAreaTwoDResult : TwoDResultBase
    {
        /// <summary>
        /// 检测条件类型
        /// </summary>
        public override DetectionPassType PassType
        {
            get => DetectionPassType.TotalAreaDetection;
        }

        public double Area { get; set; }
    }

    public class GlueSpotCountTwoDResult : TwoDResultBase
    {
        /// <summary>
        /// 检测条件类型
        /// </summary>
        public override DetectionPassType PassType
        {
            get => DetectionPassType.GlueSpotCountDetection;
        }

        public int Count { get; set; }
    }

    public class MaxGlueSpotAreaTwoDResult : TwoDResultBase
    {
        /// <summary>
        /// 检测条件类型
        /// </summary>
        public override DetectionPassType PassType
        {
            get => DetectionPassType.MaxGlueSpotAreaDetection;
        }

        public double MaxArea { get; set; }
    }

    public class GlueSpotTwoDResult : TwoDResultBase
    {
        /// <summary>
        /// 检测条件类型
        /// </summary>
        public override DetectionPassType PassType
        {
            get => DetectionPassType.GlueSpotDetection;
        }

        public List<Point2D> OffsetList { get; set; } = new List<Point2D>();
        public List<double> DiameterList { get; set; } = new List<double>();
    }

    public class MissingGlueSpotTwoDResult : TwoDResultBase
    {
        /// <summary>
        /// 检测条件类型
        /// </summary>
        public override DetectionPassType PassType
        {
            get => DetectionPassType.MissingGlueSpotDetection;
        }

        public double MaxArea { get; set; }
    }

    public class DisableTwoDResult : TwoDResultBase
    {
        /// <summary>
        /// 检测条件类型
        /// </summary>
        public override DetectionPassType PassType
        {
            get => DetectionPassType.DisableDection;
        }

        
    }

    public class TwoDResult
    {
        public bool IsOk { get; set; }
        public List<TwoDResultBase> Results { get; set; } = new List<TwoDResultBase>();
    }

    /// <summary>
    /// 极值边缘类型枚举
    /// </summary>
    public enum EdgePolarityType
    {
        /// <summary>
        /// 白到黑
        /// </summary>
        WhiteToBlack,

        /// <summary>
        /// 黑到白
        /// </summary>
        BlackToWhite,
    }

    /// <summary>
    /// 抓边样式类型
    /// </summary>
    public class EdgeStyleParams
    {
        /// <summary>
        /// 极值边缘类型
        /// </summary>
        public EdgePolarityType EdgePolarity { get; set; } = EdgePolarityType.BlackToWhite;

        /// <summary>
        /// 门限报警量(mm)
        public double ThresholdAlarmValue { get; set; }

        /// <summary>
        /// 门限抓边偏移
        /// </summary>
        public double ThresholdEdgeOffset { get; set; }
    }

    /// <summary>
    /// 抓边参数
    /// </summary>
    public class SearchEdgeParams
    {
        public string ModelPath { get; set; } = "";
        public double MmPerPixel { get; set; }

        /// <summary>
        /// 抓边样式参数
        /// </summary>
        public EdgeStyleParams EdgeStyleParams { get; set; } = new EdgeStyleParams();

        /// <summary>
        /// 相机参数
        /// </summary>
        public CameraParameters CameraParameters { get; set; } = new CameraParameters();

        /// <summary>
        /// 光源参数
        /// </summary>
        public LightParameters LightParameters { get; set; } = new LightParameters();
    }
    public class SearchEdgeResult
    {
        public bool IsOk { get; set; } = false;
        public Point2D Offset { get; set; } = new Point2D();
    }
}