using Newtonsoft.JsonG.Linq;

using System;
using System.Linq;
using System.Collections.Generic;
namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.TemplateSubPage.Mark
{

    /// <summary>
    /// Mark识别参数
    /// </summary>
    public class MarkPointRecognizeCfgInfo
    {
        ///// <summary>
        ///// 相机与光源控制配置信息
        ///// </summary>
        //public CameraLightSourceCtrCfgInfo CameraLightSourceCtrCfgInfo { set; get; } = new CameraLightSourceCtrCfgInfo();

        /// <summary>
        /// Mark识别模板参数 
        /// </summary>
        public MarkTemplateParamCfgInfo MarkTemplateParamCfgInfo { set; get; } = new MarkTemplateParamCfgInfo();

        /// <summary>
        /// Blob参数配置 
        /// </summary>
        public BlobParamCfgInfo BlobParamCfgInfo { set; get; } = new BlobParamCfgInfo();

        /// <summary>
        /// ROI参数配置 
        /// </summary>
        public RoiParamCfgInfo RoiParamCfgInfo { set; get; } = new RoiParamCfgInfo();

        /// <summary>
        /// 模板框参数配置 
        /// </summary>
        public TemplateBoxParamCfgInfo TemplateBoxParamCfgInfo { set; get; } = new TemplateBoxParamCfgInfo();

        /// <summary>
        /// 搜索框参数配置 
        /// </summary>
        public SearchBoxParamCfgInfo SearchBoxParamCfgInfo { set; get; } = new SearchBoxParamCfgInfo();

        /// <summary>
        /// 是否保存Mark结果图片
        /// </summary>
        public bool IsSaveMarkResultImage { get; set; } = false;

        /// <summary>
        /// 从JObject反序列化
        /// </summary>
        public void FromJObject(JObject? jObject)
        {
            if (jObject == null) return;

            //CameraLightSourceCtrCfgInfo.FromJObject(jObject["CameraLightSourceCtrCfgInfo"] as JObject);
            MarkTemplateParamCfgInfo.FromJObject(jObject["MarkTemplateParamCfgInfo"] as JObject);
            BlobParamCfgInfo.FromJObject(jObject["BlobParamCfgInfo"] as JObject);
            RoiParamCfgInfo.FromJObject(jObject["RoiParamCfgInfo"] as JObject);
            TemplateBoxParamCfgInfo.FromJObject(jObject["TemplateBoxParamCfgInfo"] as JObject);
            SearchBoxParamCfgInfo.FromJObject(jObject["SearchBoxParamCfgInfo"] as JObject);

            IsSaveMarkResultImage = jObject["IsSaveMarkResultImage"]?.Value<bool>() ?? false;
        }

        /// <summary>
        /// 序列化为JObject
        /// </summary>
        public JObject ToJObject()
        {
            return new JObject
            {
                //{ "CameraLightSourceCtrCfgInfo", CameraLightSourceCtrCfgInfo.ToJObject() },
                { "MarkTemplateParamCfgInfo", MarkTemplateParamCfgInfo.ToJObject() },
                { "BlobParamCfgInfo", BlobParamCfgInfo.ToJObject() },
                { "RoiParamCfgInfo", RoiParamCfgInfo.ToJObject() },
                { "TemplateBoxParamCfgInfo", TemplateBoxParamCfgInfo.ToJObject() },
                { "SearchBoxParamCfgInfo", SearchBoxParamCfgInfo.ToJObject() },
                { "IsSaveMarkResultImage", IsSaveMarkResultImage }
            };
        }

        /// <summary>
        /// 复制（优化版本，需要嵌套类也实现CopyFrom方法）
        /// </summary>
        public void CopyFrom(MarkPointRecognizeCfgInfo markPointRecognizeCfgInfo)
        {
            if (markPointRecognizeCfgInfo == null) return;

            //// 复制相机与光源控制配置
            //CameraLightSourceCtrCfgInfo.CopyFrom(markPointRecognizeCfgInfo.CameraLightSourceCtrCfgInfo);

            // 复制Mark识别模板参数
            MarkTemplateParamCfgInfo.CopyFrom(markPointRecognizeCfgInfo.MarkTemplateParamCfgInfo);

            // 复制Blob参数配置
            BlobParamCfgInfo.CopyFrom(markPointRecognizeCfgInfo.BlobParamCfgInfo);

            // 复制ROI参数配置
            RoiParamCfgInfo.CopyFrom(markPointRecognizeCfgInfo.RoiParamCfgInfo);

            // 复制模板框参数配置
            TemplateBoxParamCfgInfo.CopyFrom(markPointRecognizeCfgInfo.TemplateBoxParamCfgInfo);

            // 复制搜索框参数配置
            SearchBoxParamCfgInfo.CopyFrom(markPointRecognizeCfgInfo.SearchBoxParamCfgInfo);

            // 复制基础属性
            IsSaveMarkResultImage = markPointRecognizeCfgInfo.IsSaveMarkResultImage;
        }
    }

    /// <summary>
    /// Mark识别模板参数
    /// </summary>
    public class MarkTemplateParamCfgInfo
    {
        /// <summary>
        /// Mark通过分数
        /// </summary>
        public decimal PassScore { set; get; } = 80.0m;

        /// <summary>
        /// Mark角度
        /// </summary>
        public decimal Angle { set; get; } = 0.0m;

        /// <summary>
        /// 查找比例
        /// </summary>
        public decimal SearchRatio { set; get; } = 1.0m;

        /// <summary>
        /// 匹配模式
        /// </summary>
        public MatchMode MatchMode { set; get; } = MatchMode.ShapeMatch;

        /// <summary>
        /// 边缘黑白阈值
        /// </summary>
        public int EdgeBlackWhiteThreshold { set; get; } = 128;

        /// <summary>
        /// 边缘长度阈值
        /// </summary>
        public int EdgeLengthThreshold { set; get; } = 10;

        /// <summary>
        /// 开启平均灰度筛选
        /// </summary>
        public bool EnableAverageGrayFilter { set; get; } = false;

        /// <summary>
        /// 获取平均灰度方式
        /// </summary>
        public AverageGraySource AverageGraySource { set; get; } = AverageGraySource.TemplateBox;

        /// <summary>
        /// 平均灰度标准值
        /// </summary>
        public decimal AverageGrayStandardValue { set; get; } = 128.0m;

        /// <summary>
        /// 平均灰度值上下限比例
        /// </summary>
        public decimal AverageGrayRangeRatio { set; get; } = 0.1m;

        public MarkTemplateParamCfgInfo() { }

        /// <summary>
        /// 从JObject反序列化
        /// </summary>
        public void FromJObject(JObject? jObject)
        {
            if (jObject == null) return;

            PassScore = jObject["PassScore"]?.Value<decimal>() ?? 80.0m;
            Angle = jObject["Angle"]?.Value<decimal>() ?? 0.0m;
            SearchRatio = jObject["SearchRatio"]?.Value<decimal>() ?? 1.0m;
            EdgeBlackWhiteThreshold = jObject["EdgeBlackWhiteThreshold"]?.Value<int>() ?? 128;
            EdgeLengthThreshold = jObject["EdgeLengthThreshold"]?.Value<int>() ?? 10;
            AverageGrayStandardValue = jObject["AverageGrayStandardValue"]?.Value<decimal>() ?? 128.0m;
            AverageGrayRangeRatio = jObject["AverageGrayRangeRatio"]?.Value<decimal>() ?? 0.1m;

            EnableAverageGrayFilter = jObject["EnableAverageGrayFilter"]?.Value<bool>() ?? false;

            MatchMode = jObject["MatchMode"] != null
                ? (MatchMode)jObject["MatchMode"].Value<int>()
                : MatchMode.ShapeMatch;
            AverageGraySource = jObject["AverageGraySource"] != null
                ? (AverageGraySource)jObject["AverageGraySource"].Value<int>()
                : AverageGraySource.TemplateBox;
        }

        /// <summary>
        /// 序列化为JObject
        /// </summary>
        public JObject ToJObject()
        {
            return new JObject
            {
                { "PassScore", PassScore },
                { "Angle", Angle },
                { "SearchRatio", SearchRatio },
                { "MatchMode", (int)MatchMode },
                { "EdgeBlackWhiteThreshold", EdgeBlackWhiteThreshold },
                { "EdgeLengthThreshold", EdgeLengthThreshold },
                { "EnableAverageGrayFilter", EnableAverageGrayFilter },
                { "AverageGraySource", (int)AverageGraySource },
                { "AverageGrayStandardValue", AverageGrayStandardValue },
                { "AverageGrayRangeRatio", AverageGrayRangeRatio }
            };
        }
        public void CopyFrom(MarkTemplateParamCfgInfo source)
        {
            if (source == null) return;

            PassScore = source.PassScore;
            Angle = source.Angle;
            SearchRatio = source.SearchRatio;
            MatchMode = source.MatchMode;
            EdgeBlackWhiteThreshold = source.EdgeBlackWhiteThreshold;
            EdgeLengthThreshold = source.EdgeLengthThreshold;
            EnableAverageGrayFilter = source.EnableAverageGrayFilter;
            AverageGraySource = source.AverageGraySource;
            AverageGrayStandardValue = source.AverageGrayStandardValue;
            AverageGrayRangeRatio = source.AverageGrayRangeRatio;
        }
    }

    /// <summary>
    /// 匹配模式枚举
    /// </summary>
    public enum MatchMode
    {
        /// <summary>
        /// 形状匹配
        /// </summary>
        ShapeMatch = 0,
        /// <summary>
        /// 灰度匹配
        /// </summary>
        GrayMatch = 1
    }

    /// <summary>
    /// 获取平均灰度方式枚举
    /// </summary>
    public enum AverageGraySource
    {
        /// <summary>
        /// 模版框
        /// </summary>
        TemplateBox = 0,
        /// <summary>
        /// 搜索框
        /// </summary>
        SearchBox = 1,
        /// <summary>
        /// 自定义
        /// </summary>
        Custom = 2
    }

    /// <summary>
    /// Blob参数配置数据模型
    /// </summary>
    public class BlobParamCfgInfo
    {
        /// <summary>
        /// 开启Blob定位
        /// </summary>
        public bool EnableBlobPositioning { get; set; } = false;

        /// <summary>
        /// 开启矩形Mark
        /// </summary>
        public bool EnableRectangleMark { get; set; } = false;

        /// <summary>
        /// 低阈值min
        /// </summary>
        public int LowThresholdMin { get; set; } = 0;

        /// <summary>
        /// 低阈值max
        /// </summary>
        public int LowThresholdMax { get; set; } = 100;

        /// <summary>
        /// 高阈值min
        /// </summary>
        public int HighThresholdMin { get; set; } = 150;

        /// <summary>
        /// 高阈值max
        /// </summary>
        public int HighThresholdMax { get; set; } = 255;

        /// <summary>
        /// 杂质过滤
        /// </summary>
        public int ImpurityFilter { get; set; } = 5;

        /// <summary>
        /// 目标缝合
        /// </summary>
        public int TargetStitching { get; set; } = 3;

        /// <summary>
        /// 检测直径
        /// </summary>
        public decimal DetectionDiameter { get; set; } = 10.0m;

        /// <summary>
        /// 上下限比例
        /// </summary>
        public decimal UpperLowerLimitRatio { get; set; } = 0.2m;

        /// <summary>
        /// 圆度
        /// </summary>
        public decimal Circularity { get; set; } = 0.8m;

        /// <summary>
        /// 从JObject反序列化
        /// </summary>
        public void FromJObject(JObject? jObject)
        {
            if (jObject == null) return;

            // 布尔类型
            EnableBlobPositioning = jObject["EnableBlobPositioning"]?.Value<bool>() ?? false;
            EnableRectangleMark = jObject["EnableRectangleMark"]?.Value<bool>() ?? false;

            // 整数类型
            LowThresholdMin = jObject["LowThresholdMin"]?.Value<int>() ?? 0;
            LowThresholdMax = jObject["LowThresholdMax"]?.Value<int>() ?? 100;
            HighThresholdMin = jObject["HighThresholdMin"]?.Value<int>() ?? 150;
            HighThresholdMax = jObject["HighThresholdMax"]?.Value<int>() ?? 255;
            ImpurityFilter = jObject["ImpurityFilter"]?.Value<int>() ?? 5;
            TargetStitching = jObject["TargetStitching"]?.Value<int>() ?? 3;

            // 小数类型
            DetectionDiameter = jObject["DetectionDiameter"]?.Value<decimal>() ?? 10.0m;
            UpperLowerLimitRatio = jObject["UpperLowerLimitRatio"]?.Value<decimal>() ?? 0.2m;
            Circularity = jObject["Circularity"]?.Value<decimal>() ?? 0.8m;
        }

        /// <summary>
        /// 序列化为JObject
        /// </summary>
        public JObject ToJObject()
        {
            return new JObject
            {
                { "EnableBlobPositioning", EnableBlobPositioning },
                { "EnableRectangleMark", EnableRectangleMark },
                { "LowThresholdMin", LowThresholdMin },
                { "LowThresholdMax", LowThresholdMax },
                { "HighThresholdMin", HighThresholdMin },
                { "HighThresholdMax", HighThresholdMax },
                { "ImpurityFilter", ImpurityFilter },
                { "TargetStitching", TargetStitching },
                { "DetectionDiameter", DetectionDiameter },
                { "UpperLowerLimitRatio", UpperLowerLimitRatio },
                { "Circularity", Circularity }
            };
        }
        public void CopyFrom(BlobParamCfgInfo source)
        {
            if (source == null) return;

            EnableBlobPositioning = source.EnableBlobPositioning;
            EnableRectangleMark = source.EnableRectangleMark;
            LowThresholdMin = source.LowThresholdMin;
            LowThresholdMax = source.LowThresholdMax;
            HighThresholdMin = source.HighThresholdMin;
            HighThresholdMax = source.HighThresholdMax;
            ImpurityFilter = source.ImpurityFilter;
            TargetStitching = source.TargetStitching;
            DetectionDiameter = source.DetectionDiameter;
            UpperLowerLimitRatio = source.UpperLowerLimitRatio;
            Circularity = source.Circularity;
        }
    }

    #region ROI参数配置数据模型
    /// <summary>
    /// ROI参数配置
    /// </summary>
    public class RoiParamCfgInfo
    {
        /// <summary>
        /// ROI类型
        /// </summary>
        public RoiType RoiType { get; set; } = RoiType.Union;

        /// <summary>
        /// 选中的形状类型
        /// </summary>
        public RoiShapeType SelectedShapeType { get; set; } = RoiShapeType.Rectangle;

        /// <summary>
        /// ROI项列表
        /// </summary>
        public List<RoiItemInfo> RoiItemList { get; set; } = new List<RoiItemInfo>();

        /// <summary>
        /// 从JObject反序列化
        /// </summary>
        public void FromJObject(JObject? jObject)
        {
            if (jObject == null) return;

            // 枚举类型
            RoiType = jObject["RoiType"] != null
                ? (RoiType)jObject["RoiType"].Value<int>()
                : RoiType.Union;
            SelectedShapeType = jObject["SelectedShapeType"] != null
                ? (RoiShapeType)jObject["SelectedShapeType"].Value<int>()
                : RoiShapeType.Rectangle;

            // 列表类型
            var roiItemArray = jObject["RoiItemList"]?.Value<JArray>();
            if (roiItemArray != null)
            {
                RoiItemList = roiItemArray
                    .Select(item =>
                    {
                        var roiItem = new RoiItemInfo();
                        roiItem.FromJObject(item as JObject); 
                        return roiItem;
                    })
                    .ToList();
            }
        }

        /// <summary>
        /// 序列化为JObject
        /// </summary>
        public JObject ToJObject()
        {
            var jObject = new JObject
            {
                { "RoiType", (int)RoiType },
                { "SelectedShapeType", (int)SelectedShapeType }
            };

            var roiItemArray = new JArray();
            foreach (var roiItem in RoiItemList)
            {
                roiItemArray.Add(roiItem.ToJObject());
            }
            jObject["RoiItemList"] = roiItemArray;

            return jObject;
        }
        public void CopyFrom(RoiParamCfgInfo source)
        {
            if (source == null) return;

            RoiType = source.RoiType;
            SelectedShapeType = source.SelectedShapeType;

            // 复制列表
            RoiItemList.Clear();
            foreach (var roiItem in source.RoiItemList)
            {
                var newItem = new RoiItemInfo();
                newItem.CopyFrom(roiItem);
                RoiItemList.Add(newItem);
            }
        }
    }

    /// <summary>
    /// ROI项-数据模型
    /// </summary>
    public class RoiItemInfo
    {
        /// <summary>
        /// ROIID
        /// </summary>
        public Guid RoiId { get; set; } = Guid.NewGuid();

        /// <summary>
        /// ROI名称
        /// </summary>
        public string RoiName { get; set; } = string.Empty;

        /// <summary>
        /// 形状类型
        /// </summary>
        public RoiShapeType ShapeType { get; set; } = RoiShapeType.Rectangle;

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnabled { get; set; } = true;


        /// <summary>
        /// 从JObject反序列化（参数支持可空）
        /// </summary>
        public void FromJObject(JObject? jObject)
        {
            if (jObject == null) return;

            RoiId = Guid.TryParse(jObject["RoiId"]?.Value<string>(), out var guid) ? guid : Guid.NewGuid();
            RoiName = jObject["RoiName"]?.Value<string>() ?? string.Empty;
            IsEnabled = jObject["IsEnabled"]?.Value<bool>() ?? true;

            ShapeType = jObject["ShapeType"] != null
                ? (RoiShapeType)jObject["ShapeType"].Value<int>()
                : RoiShapeType.Rectangle;

           
        }

        /// <summary>
        /// 序列化为JObject
        /// </summary>
        public JObject ToJObject()
        {
            var jObject = new JObject
            {
                { "RoiId", RoiId.ToString() },
                { "RoiName", RoiName },
                { "ShapeType", (int)ShapeType },
                { "IsEnabled", IsEnabled },
              
            };

            return jObject;
        }
        public void CopyFrom(RoiItemInfo source)
        {
            if (source == null) return;

            RoiId = source.RoiId;
            RoiName = source.RoiName;
            ShapeType = source.ShapeType;
            IsEnabled = source.IsEnabled;
        }
    }

    /// <summary>
    /// ROI类型
    /// </summary>
    public enum RoiType
    {
        /// <summary>
        /// 并集
        /// </summary>
        Union = 0,
        /// <summary>
        /// 差集
        /// </summary>
        Difference = 1
    }

    /// <summary>
    /// ROI形状类型
    /// </summary>
    public enum RoiShapeType
    {
        /// <summary>
        /// 矩形
        /// </summary>
        Rectangle = 0,
        /// <summary>
        /// 旋转矩形
        /// </summary>
        RotatedRectangle = 1,
        /// <summary>
        /// 圆
        /// </summary>
        Circle = 2,
        /// <summary>
        /// 多边形
        /// </summary>
        Polygon = 3
    }
    #endregion

    /// <summary>
    /// 模板框参数配置-数据模型
    /// </summary>
    public class TemplateBoxParamCfgInfo
    {
        /// <summary>
        /// 模板框宽度
        /// </summary>
        public decimal Width { get; set; } = 50.0m;

        /// <summary>
        /// 模板框高度
        /// </summary>
        public decimal Height { get; set; } = 50.0m;

        /// <summary>
        /// 从JObject反序列化
        /// </summary>
        public void FromJObject(JObject? jObject)
        {
            if (jObject == null) return;
            Width = jObject["Width"]?.Value<decimal>() ?? 50.0m;
            Height = jObject["Height"]?.Value<decimal>() ?? 50.0m;
        }

        /// <summary>
        /// 序列化为JObject
        /// </summary>
        public JObject ToJObject()
        {
            return new JObject
            {
                { "Width", Width },
                { "Height", Height }
            };
        }
        public void CopyFrom(TemplateBoxParamCfgInfo source)
        {
            if (source == null) return;

            Width = source.Width;
            Height = source.Height;
        }
    }

    /// <summary>
    /// 搜索框参数配置-数据模型
    /// </summary>
    public class SearchBoxParamCfgInfo
    {
        /// <summary>
        /// 搜索框宽度
        /// </summary>
        public decimal Width { get; set; } = 50.0m;

        /// <summary>
        /// 搜索框高度
        /// </summary>
        public decimal Height { get; set; } = 50.0m;

        /// <summary>
        /// 从JObject反序列化
        /// </summary>
        public void FromJObject(JObject? jObject)
        {
            if (jObject == null) return;
            Width = jObject["Width"]?.Value<decimal>() ?? 50.0m;
            Height = jObject["Height"]?.Value<decimal>() ?? 50.0m;
        }

        /// <summary>
        /// 序列化为JObject
        /// </summary>
        public JObject ToJObject()
        {
            return new JObject
            {
                { "Width", Width },
                { "Height", Height }
            };
        }
        public void CopyFrom(SearchBoxParamCfgInfo source)
        {
            if (source == null) return;

            Width = source.Width;
            Height = source.Height;
        }
    }

}