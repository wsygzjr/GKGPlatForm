using Newtonsoft.JsonG.Linq;

namespace Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.Models
{
    /// <summary>
    /// 2D高级参数 - 数据模型
    /// </summary>
    public class TwoDAdvancedCfgInfo
    {
        /// <summary>
        /// SelShapeMin
        /// </summary>
        public int SelShapeMin { get; set; } = 10;

        /// <summary>
        /// 焊盘间隔消除大小
        /// </summary>
        public int PadGapEliminateSize { get; set; } = 5;

        /// <summary>
        /// 最小阈值
        /// </summary>
        public int MinimumThreshold { get; set; } = 20;

        /// <summary>
        /// X核
        /// </summary>
        public int XKernel { get; set; } = 3;

        /// <summary>
        /// 标准圆面积
        /// </summary>
        public int StandardCircleArea { get; set; } = 200;

        /// <summary>
        /// 动态阈值偏移
        /// </summary>
        public int DynamicThresholdOffset { get; set; } = 0;

        /// <summary>
        /// 膨胀大小2
        /// </summary>
        public int DilationSize2 { get; set; } = 1;

        /// <summary>
        /// 锡点高阈值
        /// </summary>
        public int SolderHighThreshold { get; set; } = 200;

        /// <summary>
        /// 使用动态阈值（0 = 否, 1 = 是）
        /// </summary>
        public int UseDynamicThreshold { get; set; } = 1;

        /// <summary>
        /// X方向平滑度
        /// </summary>
        public decimal SmoothnessX { get; set; } = 10m;

        /// <summary>
        /// EmpMaskWidth
        /// </summary>
        public int EmpMaskWidth { get; set; } = 50;

        /// <summary>
        /// 焊盘面积百分比
        /// </summary>
        public decimal PadAreaPercent { get; set; } = 80m;

        /// <summary>
        /// 从 JObject 反序列化（允许部分字段缺失时使用默认值）
        /// </summary>
        public void FromJObject(JObject? jObject)
        {
            if (jObject == null) return;

            SelShapeMin = jObject["SelShapeMin"]?.Value<int>() ?? SelShapeMin;
            PadGapEliminateSize = jObject["PadGapEliminateSize"]?.Value<int>() ?? PadGapEliminateSize;
            MinimumThreshold = jObject["MinimumThreshold"]?.Value<int>() ?? MinimumThreshold;
            XKernel = jObject["XKernel"]?.Value<int>() ?? XKernel;
            StandardCircleArea = jObject["StandardCircleArea"]?.Value<int>() ?? StandardCircleArea;
            DynamicThresholdOffset = jObject["DynamicThresholdOffset"]?.Value<int>() ?? DynamicThresholdOffset;
            DilationSize2 = jObject["DilationSize2"]?.Value<int>() ?? DilationSize2;
            SolderHighThreshold = jObject["SolderHighThreshold"]?.Value<int>() ?? SolderHighThreshold;
            UseDynamicThreshold = jObject["UseDynamicThreshold"]?.Value<int>() ?? UseDynamicThreshold;
            SmoothnessX = jObject["SmoothnessX"]?.Value<decimal>() ?? SmoothnessX;
            EmpMaskWidth = jObject["EmpMaskWidth"]?.Value<int>() ?? EmpMaskWidth;
            PadAreaPercent = jObject["PadAreaPercent"]?.Value<decimal>() ?? PadAreaPercent;
        }

        /// <summary>
        /// 序列化为 JObject
        /// </summary>
        public JObject ToJObject()
        {
            return new JObject
            {
                { "SelShapeMin", SelShapeMin },
                { "PadGapEliminateSize", PadGapEliminateSize },
                { "MinimumThreshold", MinimumThreshold },
                { "XKernel", XKernel },
                { "StandardCircleArea", StandardCircleArea },
                { "DynamicThresholdOffset", DynamicThresholdOffset },
                { "DilationSize2", DilationSize2 },
                { "SolderHighThreshold", SolderHighThreshold },
                { "UseDynamicThreshold", UseDynamicThreshold },
                { "SmoothnessX", SmoothnessX },
                { "EmpMaskWidth", EmpMaskWidth },
                { "PadAreaPercent", PadAreaPercent }
            };
        }

        /// <summary>
        /// 从另一个实例拷贝（浅拷贝）
        /// </summary>
        public void CopyFrom(TwoDAdvancedCfgInfo source)
        {
            if (source == null) return;

            SelShapeMin = source.SelShapeMin;
            PadGapEliminateSize = source.PadGapEliminateSize;
            MinimumThreshold = source.MinimumThreshold;
            XKernel = source.XKernel;
            StandardCircleArea = source.StandardCircleArea;
            DynamicThresholdOffset = source.DynamicThresholdOffset;
            DilationSize2 = source.DilationSize2;
            SolderHighThreshold = source.SolderHighThreshold;
            UseDynamicThreshold = source.UseDynamicThreshold;
            SmoothnessX = source.SmoothnessX;
            EmpMaskWidth = source.EmpMaskWidth;
            PadAreaPercent = source.PadAreaPercent;
        }
    }
}
