using Newtonsoft.JsonG.Linq;
using System;
using System.Linq;
using System.Collections.Generic;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.TemplateSubPage.Mark;
using Griffins.CompUI.TestGlueDirectWorkMM.CompUI.PageType;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.TemplateSubPage.Command
{
    /// <summary>
    /// 点胶指令基类
    /// </summary>
    public abstract class CommandSequenceBase
    {
        /// <summary>
        /// 从 JObject 反序列化
        /// </summary>
        /// <param name="jObject">JSON 对象</param>
        public void FromJObject(JObject? jObject)
        {
            if (jObject == null) return;
            _FromJObject(jObject);
        }

        /// <summary>
        /// 序列化为 JObject（对外统一接口）
        /// </summary>
        /// <returns>JSON 对象</returns>
        public JObject ToJObject()
        {
            var jObject = new JObject();
            _ToJObject(jObject);
            return jObject;
        }

        /// <summary>
        /// 子类实现：从 JObject 反序列化
        /// </summary>
        protected abstract void _FromJObject(JObject jObject);

        /// <summary>
        /// 子类实现：序列化为 JObject
        /// </summary>
        protected abstract void _ToJObject(JObject jObject);

        /// <summary>
        /// 工厂方法：根据指令类型创建对应实例
        /// </summary>
        /// <param name="commandType">指令类型</param>
        /// <returns>指令实例</returns>
        public static CommandSequenceBase Create(DispensingCommandType commandType)
        {
            return commandType switch
            {
                DispensingCommandType.Clean => new CleanCommandSequence(),
                DispensingCommandType.Delay => new DelayCommandSequence(),
                DispensingCommandType.NeedleLift => new LiftNeedleCommandSequence(),
                DispensingCommandType.Dispensing => new DispensingCommandSequence(),
                DispensingCommandType.SubDispensing => new SubDispensingCommandSequence(),
                DispensingCommandType.QrCode => new QrCodeCommandSequence(),
                DispensingCommandType.BadMark => new BadMarkCommandSequence(),
                DispensingCommandType.IO => new IOCommandSequence(),
                DispensingCommandType.TwoD => new TwoDCommandSequence(),
                DispensingCommandType.Coating => new CoatingCommandSequence(),
                DispensingCommandType.MeasurementHeight => new BaseHeightMeasurementCommandSequence(),
                DispensingCommandType.MeasurementThickness => new ThicknessMeasurementCommandSequence(),
                DispensingCommandType.EdgeGrasping => new ClampCommandSequence(),

                //DispensingCommandType.Segment => new SegmentCommandSequence(),
                //DispensingCommandType.ValveOpening => new ValveOpeningCommandSequence(), 
                _ => throw new ArgumentOutOfRangeException(nameof(commandType), "未知的点胶指令类型")
            };
        }
    }
    #region 点胶指令

    /// <summary>
    /// 点胶指令（序列化实现）
    /// </summary>
    public class DispensingCommandSequence : CommandSequenceBase
    {
        /// <summary>
        /// 轨迹序列ID列表
        /// </summary>
        public List<Guid> TrajectorySequenceIds { get; set; } = new List<Guid>();

        /// <summary>
        /// 循环次数
        /// </summary>
        private int _cycleCount = 1;
        public int CycleCount
        {
            get => _cycleCount;
            set => _cycleCount = value < 1 ? 1 : value;
        }

        /// <summary>
        /// 阀号
        /// </summary>
        private string _valveNumber = string.Empty;
        public string ValveNumber
        {
            get => _valveNumber;
            set => _valveNumber = string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
        }

        protected override void _FromJObject(JObject jObject)
        {
            // 循环次数
            CycleCount = jObject["CycleCount"]?.Value<int>() ?? 1;
            // 阀号
            ValveNumber = jObject["ValveNumber"]?.Value<string>() ?? string.Empty;
            // 轨迹序列ID列表
            var trajectoryIds = jObject["TrajectorySequenceIds"]?.Value<JArray>();
            if (trajectoryIds != null)
            {
                TrajectorySequenceIds = trajectoryIds
                    .Select(id => Guid.TryParse(id.Value<string>(), out var guid) ? guid : Guid.Empty)
                    .Where(guid => guid != Guid.Empty)
                    .ToList();
            }
        }

        protected override void _ToJObject(JObject jObject)
        {
            jObject["CycleCount"] = CycleCount;
            jObject["ValveNumber"] = ValveNumber;
            jObject["TrajectorySequenceIds"] = new JArray(TrajectorySequenceIds.Select(id => id.ToString()));
        }
    }

    /// <summary>
    /// 阀信息
    /// </summary>
    public class ValveInfo
    {
        /// <summary>
        /// 阀号
        /// </summary>
        public string ValveNumber { set; get; } = "";
        /// <summary>
        /// 阀名称
        /// </summary>
        public string ValveName { set; get; } = "";
        public ValveInfo()
        {
        }
    }
    #endregion

    #region 抬针指令

    /// <summary>
    /// 抬针指令
    /// </summary>
    public class LiftNeedleCommandSequence : CommandSequenceBase
    {
        /// <summary>
        /// 抬针高度
        /// </summary>
        public decimal Height { get; set; } = 0.0m;

        protected override void _FromJObject(JObject jObject)
        {
            Height = jObject["Height"]?.Value<decimal>() ?? 0.0m;
        }

        protected override void _ToJObject(JObject jObject)
        {
            jObject["Height"] = Height;
        }
    }

    #endregion



    #region 延时指令
    /// <summary>
    /// 延时指令
    /// </summary>
    public class DelayCommandSequence : CommandSequenceBase
    {
        /// <summary>
        /// 延时时间
        /// </summary>
        public decimal DelayTime { get; set; } = 0.0m;

        protected override void _FromJObject(JObject jObject)
        {
            DelayTime = jObject["DelayTime"]?.Value<decimal>() ?? 0.0m;
        }

        protected override void _ToJObject(JObject jObject)
        {
            jObject["DelayTime"] = DelayTime;
        }
    }
    #endregion

    #region 清胶指令

    /// <summary>
    /// 清胶指令
    /// </summary>
    public class CleanCommandSequence : CommandSequenceBase
    {
        /// <summary>
        /// 排胶点数
        /// </summary>
        public int DispensingPointCount { get; set; } = 1;

        /// <summary>
        /// 排胶顺序
        /// </summary>
        public DispensingOrder DispensingOrder { get; set; } = DispensingOrder.FirstDispensing;

        protected override void _FromJObject(JObject jObject)
        {
            DispensingPointCount = jObject["DispensingPointCount"]?.Value<int>() ?? 1;
            DispensingOrder = jObject["DispensingOrder"] != null
                ? (DispensingOrder)jObject["DispensingOrder"].Value<int>()
                : DispensingOrder.FirstDispensing;
        }

        protected override void _ToJObject(JObject jObject)
        {
            jObject["DispensingPointCount"] = DispensingPointCount;
            jObject["DispensingOrder"] = (int)DispensingOrder;
        }
    }

    /// <summary>
    /// 排胶顺序枚举
    /// </summary>
    public enum DispensingOrder
    {
        /// <summary>
        /// 先排胶
        /// </summary>
        FirstDispensing,
        /// <summary>
        /// 先清洁
        /// </summary>
        FirstClean,
    }
    #endregion

    #region 涂覆指令
    /// <summary>
    /// 涂覆指令
    /// </summary>
    public class CoatingCommandSequence : CommandSequenceBase
    {

        /// <summary>
        /// 选中的点胶中线样式ID
        /// </summary>
        public Guid SelectedMiddleLineStyleID { get; set; }

        /// <summary>
        /// 选中的点胶前后线样式ID
        /// </summary>
        public Guid SelectedBeforeAfterLineStyleID { get; set; }
        /// <summary>
        /// 形状选择
        /// </summary>
        public CoatingShape CoatingShape { get; set; } = CoatingShape.Line;

        /// <summary>
        /// 重量
        /// </summary>
        public decimal Weight { get; set; } = 0.01m;

        /// <summary>
        /// 重量类型
        /// </summary>
        public WeightType WeightType { get; set; } = WeightType.Gram;

        /// <summary>
        /// 间距（mm）
        /// </summary>
        public decimal Spacing { get; set; } = 1.0m;

        /// <summary>
        /// 初始位置
        /// </summary>
        public BasePositionInfo InitialPositionInfo { get; set; } = new BasePositionInfo();

        /// <summary>
        /// X向终点位置
        /// </summary>
        public BasePositionInfo XEndPositionInfo { get; set; } = new BasePositionInfo();

        /// <summary>
        /// Y向终点位置
        /// </summary>
        public BasePositionInfo YEndPositionInfo { get; set; } = new BasePositionInfo();

        protected override void _FromJObject(JObject jObject)
        {
            if (jObject["SelectedMiddleLineStyleID"] != null)
                SelectedMiddleLineStyleID = Guid.Parse(jObject["SelectedMiddleLineStyleID"].Value<string>());
            if (jObject["SelectedBeforeAfterLineStyleID"] != null)
                SelectedBeforeAfterLineStyleID = Guid.Parse(jObject["SelectedBeforeAfterLineStyleID"].Value<string>());

            CoatingShape = jObject["CoatingShape"] != null
                ? (CoatingShape)jObject["CoatingShape"].Value<int>()
                : CoatingShape.Line;
            WeightType = jObject["WeightType"] != null
                ? (WeightType)jObject["WeightType"].Value<int>()
                : WeightType.Gram;

            // 数值类型
            Weight = jObject["Weight"]?.Value<decimal>() ?? 0.01m;
            Spacing = jObject["Spacing"]?.Value<decimal>() ?? 1.0m;

            // 复杂类型
            InitialPositionInfo.FromJObject(jObject["InitialPositionInfo"] as JObject);
            XEndPositionInfo.FromJObject(jObject["XEndPositionInfo"] as JObject);
            YEndPositionInfo.FromJObject(jObject["YEndPositionInfo"] as JObject);
        }

        protected override void _ToJObject(JObject jObject)
        {
            jObject["SelectedMiddleLineStyleID"] = SelectedMiddleLineStyleID.ToString();
            jObject["SelectedBeforeAfterLineStyleID"] = SelectedBeforeAfterLineStyleID.ToString();

            jObject["CoatingShape"] = (int)CoatingShape;
            jObject["Weight"] = Weight;
            jObject["WeightType"] = (int)WeightType;
            jObject["Spacing"] = Spacing;
            jObject["InitialPositionInfo"] = InitialPositionInfo.ToJObject();
            jObject["XEndPositionInfo"] = XEndPositionInfo.ToJObject();
            jObject["YEndPositionInfo"] = YEndPositionInfo.ToJObject();
        }
    }

    /// <summary>
    /// 涂覆样式枚举
    /// </summary>
    public enum CoatingStyle
    {
        /// <summary>
        /// 样式一
        /// </summary>
        StyleOne,
        /// <summary>
        /// 样式二
        /// </summary>
        StyleTwo,
        /// <summary>
        /// 样式三
        /// </summary>
        StyleThree
    }

    /// <summary>
    /// 涂覆形状枚举
    /// </summary>
    public enum CoatingShape
    {
        /// <summary>
        /// 线
        /// </summary>
        Line,
        /// <summary>
        /// 矩形
        /// </summary>
        Rectangle,
        /// <summary>
        /// 回形
        /// </summary>
        Loop
    }

    /// <summary>
    /// 重量类型枚举
    /// </summary>
    public enum WeightType
    {
        /// <summary>
        /// 克
        /// </summary>
        Gram,
        /// <summary>
        /// 毫克
        /// </summary>
        Milligram
    }
    #endregion

    #region IO指令

    /// <summary>
    /// IO指令
    /// </summary>
    public class IOCommandSequence : CommandSequenceBase
    {
        /// <summary>
        /// IO类型
        /// </summary>
        public IOType IOType { get; set; } = IOType.Input;

        /// <summary>
        /// 功能
        /// </summary>
        public IOFunction IOFunction { get; set; } = IOFunction.StartStopButton;

        /// <summary>
        /// 状态
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// 超时时间（ms）
        /// </summary>
        public int Timeout { get; set; } = 1000;

        /// <summary>
        /// 指令序号
        /// </summary>
        public int CommandIndex { get; set; } = 1;

        protected override void _FromJObject(JObject jObject)
        {
            IOType = jObject["IOType"] != null ? (IOType)jObject["IOType"].Value<int>() : IOType.Input;
            IOFunction = jObject["IOFunction"] != null ? (IOFunction)jObject["IOFunction"].Value<int>() : IOFunction.StartStopButton;
            IsActive = jObject["IsActive"]?.Value<bool>() ?? true;
            Timeout = jObject["Timeout"]?.Value<int>() ?? 1000;
            CommandIndex = jObject["CommandIndex"]?.Value<int>() ?? 1;
        }

        protected override void _ToJObject(JObject jObject)
        {
            jObject["IOType"] = (int)IOType;
            jObject["IOFunction"] = (int)IOFunction;
            jObject["IsActive"] = IsActive;
            jObject["Timeout"] = Timeout;
            jObject["CommandIndex"] = CommandIndex;
        }
    }
    /// <summary>
    /// IO类型枚举
    /// </summary>
    public enum IOType
    {
        /// <summary>
        /// 输入
        /// </summary>
        Input,
        /// <summary>
        /// 输出
        /// </summary>
        Output
    }

    /// <summary>
    /// IO功能枚举
    /// </summary>
    public enum IOFunction
    {
        /// <summary>
        /// 启停按钮
        /// </summary>
        StartStopButton,
        /// <summary>
        /// 复位按钮
        /// </summary>
        ResetButton,
        /// <summary>
        /// 急停按钮
        /// </summary>
        EmergencyStopButton
    }
    #endregion

    #region BadMark设置指令

    /// <summary>
    /// BadMark设置指令
    /// </summary>
    public class BadMarkCommandSequence : CommandSequenceBase
    {
        /// <summary>
        /// 模式选择
        /// </summary>
        public BadMarkMode BadMarkMode { get; set; } = BadMarkMode.NGSkip;

        /// <summary>
        /// BadMark位置
        /// </summary>
        public BasePositionInfo BadMarkPositionInfo { get; set; } = new BasePositionInfo();

        /// <summary>
        /// 操作类型
        /// </summary>
        public BadMarkOperationType OperationType { get; set; } = BadMarkOperationType.InkDot;

        /// <summary>
        /// BadMark识别参数
        /// </summary>
        public MarkPointRecognizeCfgInfo MarkPointRecognizeCfgInfo { get; set; } = new MarkPointRecognizeCfgInfo();

        protected override void _FromJObject(JObject jObject)
        {
            // 枚举
            BadMarkMode = jObject["BadMarkMode"] != null ? (BadMarkMode)jObject["BadMarkMode"].Value<int>() : BadMarkMode.NGSkip;
            OperationType = jObject["OperationType"] != null ? (BadMarkOperationType)jObject["OperationType"].Value<int>() : BadMarkOperationType.InkDot;

            BadMarkPositionInfo.FromJObject(jObject["BadMarkPositionInfo"] as JObject);
            MarkPointRecognizeCfgInfo.FromJObject(jObject["MarkPointRecognizeCfgInfo"] as JObject);
        }

        protected override void _ToJObject(JObject jObject)
        {
            jObject["BadMarkMode"] = (int)BadMarkMode;
            jObject["OperationType"] = (int)OperationType;
            jObject["BadMarkPositionInfo"] = BadMarkPositionInfo.ToJObject();
            jObject["MarkPointRecognizeCfgInfo"] = MarkPointRecognizeCfgInfo.ToJObject();
        }
    }

    /// <summary>
    /// BadMark模式枚举
    /// </summary>
    public enum BadMarkMode
    {
        /// <summary>
        /// 识别结果NG跳过
        /// </summary>
        NGSkip,
        /// <summary>
        /// 识别结果OK跳过
        /// </summary>
        OKSkip
    }

    /// <summary>
    /// BadMark操作类型枚举
    /// </summary>
    public enum BadMarkOperationType
    {
        /// <summary>
        /// 墨点
        /// </summary>
        InkDot,
        /// <summary>
        /// 匹配
        /// </summary>
        Match
    }
    #endregion

    #region 2D设置指令

    /// <summary>
    /// 2D设置指令
    /// </summary>
    public class TwoDCommandSequence : CommandSequenceBase
    {
        /// <summary>
        /// 序号
        /// </summary>
        public int SerialNumber { get; set; } = 1;

        /// <summary>
        /// 初始位置
        /// </summary>
        public BasePositionInfo InitalPositionInfo { get; set; } = new BasePositionInfo();

        /// <summary>
        /// 胶宽检测
        /// </summary>
        public bool IsGlueWidthDetectionEnabled { get; set; } = false;

        /// <summary>
        /// 检测方式
        /// </summary>
        public DetectionMode DetectionMode { get; set; } = DetectionMode.DoNotSaveImage;

        protected override void _FromJObject(JObject jObject)
        {
            SerialNumber = jObject["SerialNumber"]?.Value<int>() ?? 1;
            IsGlueWidthDetectionEnabled = jObject["IsGlueWidthDetectionEnabled"]?.Value<bool>() ?? false;
            DetectionMode = jObject["DetectionMode"] != null ? (DetectionMode)jObject["DetectionMode"].Value<int>() : DetectionMode.DoNotSaveImage;
            InitalPositionInfo.FromJObject(jObject["InitalPositionInfo"] as JObject);
        }

        protected override void _ToJObject(JObject jObject)
        {
            jObject["SerialNumber"] = SerialNumber;
            jObject["IsGlueWidthDetectionEnabled"] = IsGlueWidthDetectionEnabled;
            jObject["DetectionMode"] = (int)DetectionMode;
            jObject["InitalPositionInfo"] = InitalPositionInfo.ToJObject();
        }
    }

    /// <summary>
    /// 检测方式枚举
    /// </summary>
    public enum DetectionMode
    {
        /// <summary>
        /// 不保存图片
        /// </summary>
        DoNotSaveImage,
        /// <summary>
        /// NG保存图片
        /// </summary>
        SaveNGImage,
        /// <summary>
        /// OK保存图片
        /// </summary>
        SaveOKImage,
        /// <summary>
        /// 保存所有图片
        /// </summary>
        SaveAllImages
    }
    #endregion

    #region 扫二维码指令
    /// <summary>
    /// 扫二维码指令
    /// </summary>
    public class QrCodeCommandSequence : CommandSequenceBase
    {
        /// <summary>
        /// 扫码方式
        /// </summary>
        public QrCodeScanMode ScanMode { get; set; } = QrCodeScanMode.CCD;

        /// <summary>
        /// 码类型
        /// </summary>
        public QrCodeType CodeType { get; set; } = QrCodeType.QRCode;

        /// <summary>
        /// 扫码数据列表
        /// </summary>
        public List<string> ScanDataList { get; set; } = new List<string>();

        /// <summary>
        /// 点阵左上位置
        /// </summary>
        public BasePositionInfo PositionInfo { get; set; }
        public QrCodeCommandSequence()
        {
            PositionInfo = new BasePositionInfo();
        }
        protected override void _FromJObject(JObject jObject)
        {
            ScanMode = jObject["ScanMode"] != null ? (QrCodeScanMode)jObject["ScanMode"].Value<int>() : QrCodeScanMode.CCD;
            CodeType = jObject["CodeType"] != null ? (QrCodeType)jObject["CodeType"].Value<int>() : QrCodeType.QRCode;

            // 字符串列表
            var scanDataArray = jObject["ScanDataList"]?.Value<JArray>();
            if (scanDataArray != null)
            {
                ScanDataList = scanDataArray.Select(item => item.Value<string>() ?? string.Empty).ToList();
            }
            if (jObject["PositionInfo"] is JObject positionInfo)
            {
                PositionInfo.FromJObject(positionInfo);
            }
        }

        protected override void _ToJObject(JObject jObject)
        {
            jObject["ScanMode"] = (int)ScanMode;
            jObject["CodeType"] = (int)CodeType;
            jObject["ScanDataList"] = new JArray(ScanDataList);
            jObject["PositionInfo"] = PositionInfo.ToJObject();
        }
    }


    /// <summary>
    /// 扫码方式枚举
    /// </summary>
    public enum QrCodeScanMode
    {
        /// <summary>
        /// CCD扫码
        /// </summary>
        CCD,
        /// <summary>
        /// 激光扫码
        /// </summary>
        Laser
    }

    /// <summary>
    /// 码类型枚举
    /// </summary>
    public enum QrCodeType
    {
        /// <summary>
        /// 一维码
        /// </summary>
        BarCode,
        /// <summary>
        /// 二维码QR
        /// </summary>
        QRCode,
        /// <summary>
        /// 二维码Martrix
        /// </summary>
        MatrixCode
    }
    #endregion

    #region 测高测厚度指令



    /// <summary>
    /// 基准版测高指令（序列化实现）
    /// </summary>
    public class BaseHeightMeasurementCommandSequence : CommandSequenceBase
    {
        /// <summary>
        /// 测高模式
        /// </summary>
        public HeightMeasurementMode MeasurementMode { get; set; } = HeightMeasurementMode.SinglePoint;

        /// <summary>
        /// 下限（mm）
        /// </summary>
        public decimal LowerLimit { get; set; } = 0.000m;

        /// <summary>
        /// 上限（mm）
        /// </summary>
        public decimal UpperLimit { get; set; } = 50.000m;

        /// <summary>
        /// 只用于防呆
        /// </summary>
        public bool IsOnlyForFoolproof { get; set; } = false;

        /// <summary>
        /// 三点偏差（mm）
        /// </summary>
        public decimal ThreePointDeviation { get; set; } = 0.000m;

        /// <summary>
        /// 测试值
        /// </summary>
        public string TestValue { get; set; } = "";

        /// <summary>
        /// 测高点位置
        /// </summary>
        public List<BasePositionInfo> MeasurementHeightPositionInfoes { get; set; } = new List<BasePositionInfo>();
        protected override void _FromJObject(JObject jObject)
        {
            if (jObject == null) return;
            MeasurementMode = jObject["MeasurementMode"] != null ? (HeightMeasurementMode)jObject["MeasurementMode"].Value<int>() : HeightMeasurementMode.SinglePoint;
            LowerLimit = jObject["LowerLimit"]?.Value<decimal>() ?? 0.000m;
            UpperLimit = jObject["UpperLimit"]?.Value<decimal>() ?? 50.000m;
            IsOnlyForFoolproof = jObject["IsOnlyForFoolproof"]?.Value<bool>() ?? false;
            ThreePointDeviation = jObject["ThreePointDeviation"]?.Value<decimal>() ?? 0.000m;
            TestValue = jObject["TestValue"]?.Value<string>() ?? "";

            // 测高点位置列表
            var positionArray = jObject["MeasurementHeightPositionInfoes"]?.Value<JArray>();
            if (positionArray != null)
            {
                MeasurementHeightPositionInfoes = positionArray
                    .Select(item =>
                    {
                        var pos = new BasePositionInfo();
                        pos.FromJObject(item as JObject);
                        return pos;
                    })
                    .ToList();
            }
        }

        protected override void _ToJObject(JObject jObject)
        {

            jObject["MeasurementMode"] = (int)MeasurementMode;
            jObject["LowerLimit"] = LowerLimit;
            jObject["IsOnlyForFoolproof"] = IsOnlyForFoolproof;
            jObject["ThreePointDeviation"] = ThreePointDeviation;
            jObject["TestValue"] = TestValue;

            // 测高点位置列表
            var positionArray = new JArray();
            foreach (var pos in MeasurementHeightPositionInfoes)
            {
                positionArray.Add(pos.ToJObject());
            }
            jObject["MeasurementHeightPositionInfoes"] = positionArray;
        }

    }

    /// <summary>
    /// 测厚度指令（序列化实现）
    /// </summary>
    public class ThicknessMeasurementCommandSequence : CommandSequenceBase
    {

        /// <summary>
        /// 厚度（mm）
        /// </summary>
        public decimal Thickness { get; set; } = 50.000m;

        /// <summary>
        /// 测试值
        /// </summary>
        public string TestValue { get; set; } = "";

        /// <summary>
        /// 基准点位置
        /// </summary>
        public BasePositionInfo BasePositionInfo { get; set; } = new BasePositionInfo();

        /// <summary>
        /// 厚度点位置
        /// </summary>
        public List<ThicknessPositionInfo> ThicknessPositionInfoes { get; set; } = new List<ThicknessPositionInfo>();
        protected override void _FromJObject(JObject jObject)
        {
            if (jObject == null) return;
            Thickness = jObject["Thickness"]?.Value<decimal>() ?? 0.000m;
            TestValue = jObject["TestValue"]?.Value<string>() ?? "";

            // 基准点位置
            BasePositionInfo.FromJObject(jObject["BasePositionInfo"] as JObject);

            // 厚度点位置列表
            var thicknessArray = jObject["ThicknessPositionInfoes"]?.Value<JArray>();
            if (thicknessArray != null)
            {
                ThicknessPositionInfoes = thicknessArray
                    .Select(item =>
                    {
                        var pos = new ThicknessPositionInfo();
                        pos.FromJObject(item as JObject);
                        return pos;
                    })
                    .ToList();
            }
        }

        protected override void _ToJObject(JObject jObject)
        {
            jObject["Thickness"] = Thickness;
            jObject["TestValue"] = TestValue;
            jObject["BasePositionInfo"] = BasePositionInfo.ToJObject();

            // 厚度点位置列表
            var thicknessArray = new JArray();
            foreach (var pos in ThicknessPositionInfoes)
            {
                thicknessArray.Add(pos.ToJObject());
            }
            jObject["ThicknessPositionInfoes"] = thicknessArray;

        }
    }
    /// <summary>
    /// 厚度点信息项
    /// </summary>
    public class ThicknessPositionInfo : BasePositionInfo
    {
        /// <summary>
        /// 下限（mm）
        /// </summary>
        public decimal LowerLimit { get; set; } = 0.000m;

        /// <summary>
        /// 上限（mm）
        /// </summary>
        public decimal UpperLimit { get; set; } = 50.000m;

        /// <summary>
        /// 厚度（mm）
        /// </summary>
        public decimal Thickness { get; set; } = 50.000m;

        /// <summary>
        /// 从 JObject 反序列化
        /// </summary>
        public new void FromJObject(JObject? jObject)
        {
            if (jObject == null) return;
            base.FromJObject(jObject);
            if (jObject == null) return;
            LowerLimit = jObject["LowerLimit"]?.Value<decimal>() ?? 0.000m;
            UpperLimit = jObject["UpperLimit"]?.Value<decimal>() ?? 50.000m;
            Thickness = jObject["Thickness"]?.Value<decimal>() ?? 50.000m;
        }

        /// <summary>
        /// 序列化为 JObject
        /// </summary>
        public new JObject ToJObject()
        {
            var jObject = base.ToJObject();
            jObject["LowerLimit"] = LowerLimit;
            jObject["UpperLimit"] = UpperLimit;
            jObject["Thickness"] = Thickness;
            return jObject;
        }
    }

    /// <summary>
    /// 测高模式枚举
    /// </summary>
    public enum HeightMeasurementMode
    {
        /// <summary>
        /// 单点测高
        /// </summary>
        SinglePoint,
        /// <summary>
        /// 三点测高
        /// </summary>
        ThreePoint
    }
    #endregion

    #region 子点胶指令

    /// <summary>
    /// 子点胶指令（序列化实现）
    /// </summary>
    public class SubDispensingCommandSequence : CommandSequenceBase
    {
        /// <summary>
        /// 子模板ID
        /// </summary>
        public Guid SubTemplateID { get; set; } = Guid.Empty;

        protected override void _FromJObject(JObject jObject)
        {
            var subTemplateIdStr = jObject["SubTemplateID"]?.Value<string>();
            SubTemplateID = Guid.TryParse(subTemplateIdStr, out var guid) ? guid : Guid.Empty;
        }

        protected override void _ToJObject(JObject jObject)
        {
            jObject["SubTemplateID"] = SubTemplateID.ToString();
        }
    }
    #endregion

    #region 抓边指令

    /// <summary>
    /// 抓边指令（序列化实现）
    /// </summary>
    public class ClampCommandSequence : CommandSequenceBase
    {
        /// <summary>
        /// 初始位置
        /// </summary>
        public BasePositionInfo InitalPositionInfo { get; set; } = new BasePositionInfo();
        /// <summary>
        /// 轨迹序列ID列表
        /// </summary>
        public List<Guid> TrajectorySequenceIds { get; set; } = new List<Guid>();

        /// <summary>
        /// 循环次数
        /// </summary>
        private int _cycleCount = 1;
        public int CycleCount
        {
            get => _cycleCount;
            set => _cycleCount = value < 1 ? 1 : value;
        }

        /// <summary>
        /// 阀号
        /// </summary>
        private string _valveNumber = string.Empty;
        public string ValveNumber
        {
            get => _valveNumber;
            set => _valveNumber = string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
        }

        protected override void _FromJObject(JObject jObject)
        {
            // 循环次数
            CycleCount = jObject["CycleCount"]?.Value<int>() ?? 1;
            // 阀号
            ValveNumber = jObject["ValveNumber"]?.Value<string>() ?? string.Empty;
            // 轨迹序列ID列表
            var trajectoryIds = jObject["TrajectorySequenceIds"]?.Value<JArray>();
            if (trajectoryIds != null)
            {
                TrajectorySequenceIds = trajectoryIds
                    .Select(id => Guid.TryParse(id.Value<string>(), out var guid) ? guid : Guid.Empty)
                    .Where(guid => guid != Guid.Empty)
                    .ToList();
            }
        }

        protected override void _ToJObject(JObject jObject)
        {
            jObject["CycleCount"] = CycleCount;
            jObject["ValveNumber"] = ValveNumber;
            jObject["TrajectorySequenceIds"] = new JArray(TrajectorySequenceIds.Select(id => id.ToString()));
        }
    }
    #endregion
}
