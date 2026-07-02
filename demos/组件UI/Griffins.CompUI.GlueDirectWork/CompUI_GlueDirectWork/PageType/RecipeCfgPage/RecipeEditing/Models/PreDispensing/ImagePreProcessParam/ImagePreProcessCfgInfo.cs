using GKG;
using Newtonsoft.JsonG.Linq;

namespace Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.Models
{
    /// <summary>
    /// 图像预处理参数 - 数据模型
    /// </summary>
    public class ImagePreProcessCfgInfo
    {
        /// <summary>
        /// 低阈值（0-255）
        /// </summary>
        public int LowThreshold { get; set; } = 50;

        /// <summary>
        /// 高阈值（0-255）
        /// </summary>
        public int HighThreshold { get; set; } = 150;

        /// <summary>
        /// 矩形度（0-100，保留小数）
        /// </summary>
        public decimal Rectangularity { get; set; } = 70m;

        /// <summary>
        /// 是否启用一维测量
        /// </summary>
        public bool EnableOneDimensionalMeasurement { get; set; } = false;

        /// <summary>
        /// 是否启用杂质处理
        /// </summary>
        public bool ImpurityProcessing { get; set; } = false;

        /// <summary>
        /// 检测框切片个数
        /// </summary>
        public int DetectionFrameSliceCount { get; set; } = 10;

        /// <summary>
        /// 允许胶宽超出范围百分比（%）
        /// </summary>
        public decimal GlueWidthExceedPercent { get; set; } = 10m;

        /// <summary>
        /// 杂质过滤值（单位：mm）
        /// </summary>
        public int ImpurityFilterValue { get; set; } = 100;

        /// <summary>
        /// 目标缝合（单位：mm）
        /// </summary>
        public int TargetStitching { get; set; } = 200;

        /// <summary>
        /// 胶条数
        /// </summary>
        public int GlueStripCount { get; set; } = 1;

        /// <summary>
        /// 是否启用面积筛选
        /// </summary>
        public bool EnableAreaFilter { get; set; } = false;

        /// <summary>
        /// 最小面积（单位：mm²）
        /// </summary>
        public int MinimumArea { get; set; } = 50;

        /// <summary>
        /// 最大面积（单位：mm²）
        /// </summary>
        public int MaximumArea { get; set; } = 5000;

        /// <summary>
        /// 是否填充孔洞
        /// </summary>
        public bool FillHoles { get; set; } = false;

        /// <summary>
        /// 极性枚举
        /// </summary>
        public PolarityType Polarity { get; set; } = PolarityType.WhiteBackgroundBlackDot;

        /// <summary>
        /// 2D高级参数
        /// </summary>
        public TwoDAdvancedCfgInfo TwoDAdvancedCfgInfo { get; set; } = new();


        /// <summary>
        /// 从 JObject 反序列化（允许部分字段缺失时使用默认值）
        /// </summary>
        public void FromJObject(JObject? jObject)
        {
            if (jObject == null) return;

            // 布尔类型
            EnableOneDimensionalMeasurement = jObject["EnableOneDimensionalMeasurement"]?.Value<bool>() ?? EnableOneDimensionalMeasurement;
            ImpurityProcessing = jObject["ImpurityProcessing"]?.Value<bool>() ?? ImpurityProcessing;
            EnableAreaFilter = jObject["EnableAreaFilter"]?.Value<bool>() ?? EnableAreaFilter;
            FillHoles = jObject["FillHoles"]?.Value<bool>() ?? FillHoles;

            // 整数类型
            LowThreshold = jObject["LowThreshold"]?.Value<int>() ?? LowThreshold;
            HighThreshold = jObject["HighThreshold"]?.Value<int>() ?? HighThreshold;
            DetectionFrameSliceCount = jObject["DetectionFrameSliceCount"]?.Value<int>() ?? DetectionFrameSliceCount;
            ImpurityFilterValue = jObject["ImpurityFilterValue"]?.Value<int>() ?? ImpurityFilterValue;
            TargetStitching = jObject["TargetStitching"]?.Value<int>() ?? TargetStitching;
            GlueStripCount = jObject["GlueStripCount"]?.Value<int>() ?? GlueStripCount;
            MinimumArea = jObject["MinimumArea"]?.Value<int>() ?? MinimumArea;
            MaximumArea = jObject["MaximumArea"]?.Value<int>() ?? MaximumArea;

            // 小数类型
            Rectangularity = jObject["Rectangularity"]?.Value<decimal>() ?? Rectangularity;
            GlueWidthExceedPercent = jObject["GlueWidthExceedPercent"]?.Value<decimal>() ?? GlueWidthExceedPercent;

            // 枚举类型（存在则按 int 转枚举，否则保留默认）
            Polarity = jObject["Polarity"] != null
                ? (PolarityType)jObject["Polarity"].Value<int>()
                : Polarity;

            // 2D高级参数反序列化
            TwoDAdvancedCfgInfo.FromJObject(jObject["TwoDAdvancedCfgInfo"] as JObject);

        }

        /// <summary>
        /// 序列化为 JObject
        /// </summary>
        public JObject ToJObject()
        {
            var obj = new JObject
            {
                { "LowThreshold", LowThreshold },
                { "HighThreshold", HighThreshold },
                { "Rectangularity", Rectangularity },
                { "EnableOneDimensionalMeasurement", EnableOneDimensionalMeasurement },
                { "ImpurityProcessing", ImpurityProcessing },
                { "DetectionFrameSliceCount", DetectionFrameSliceCount },
                { "GlueWidthExceedPercent", GlueWidthExceedPercent },
                { "ImpurityFilterValue", ImpurityFilterValue },
                { "TargetStitching", TargetStitching },
                { "GlueStripCount", GlueStripCount },
                { "EnableAreaFilter", EnableAreaFilter },
                { "MinimumArea", MinimumArea },
                { "MaximumArea", MaximumArea },
                { "FillHoles", FillHoles },
                { "Polarity", (int)Polarity },
                { "TwoDAdvancedCfgInfo", TwoDAdvancedCfgInfo.ToJObject() }
            };

            return obj;
        }

        /// <summary>
        /// 从另一个实例拷贝（浅拷贝）
        /// </summary>
        public void CopyFrom(ImagePreProcessCfgInfo source)
        {
            if (source == null) return;

            LowThreshold = source.LowThreshold;
            HighThreshold = source.HighThreshold;
            Rectangularity = source.Rectangularity;
            EnableOneDimensionalMeasurement = source.EnableOneDimensionalMeasurement;
            ImpurityProcessing = source.ImpurityProcessing;
            DetectionFrameSliceCount = source.DetectionFrameSliceCount;
            GlueWidthExceedPercent = source.GlueWidthExceedPercent;
            ImpurityFilterValue = source.ImpurityFilterValue;
            TargetStitching = source.TargetStitching;
            GlueStripCount = source.GlueStripCount;
            EnableAreaFilter = source.EnableAreaFilter;
            MinimumArea = source.MinimumArea;
            MaximumArea = source.MaximumArea;
            FillHoles = source.FillHoles;
            Polarity = source.Polarity;
            TwoDAdvancedCfgInfo.CopyFrom(source.TwoDAdvancedCfgInfo);
        }
    }
}
