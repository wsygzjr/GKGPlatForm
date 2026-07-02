using Newtonsoft.JsonG.Linq;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.TemplateSubPage.Mark;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels;
using System;
using System.Collections.Generic;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.Plan;

/// <summary>
/// 点胶流程基类
/// </summary>
public class DispensingProcessBaseInfo
{
    public DispensingProcessBaseInfo() { }

    /// <summary>
    /// 创建点胶流程
    /// </summary>
    public virtual DispensingProcessBaseInfo Create(DispensingProcessType dispensingProcessType)
    {
        switch (dispensingProcessType)
        {
            case DispensingProcessType.SingleStage:
                return new SingleStagePlanInfo();
            case DispensingProcessType.SegmentationStage:
                return new SegmentationStagePlanInfo();
            default:
                throw new Exception("不支持的点胶流程类型");
        }
    }

    /// <summary>
    /// 从JObject反序列化（虚方法，子类重写）
    /// </summary>
    public virtual void FromJObject(JObject? jObject) { }

    /// <summary>
    /// 序列化为JObject（虚方法，子类重写）
    /// </summary>
    public virtual JObject ToJObject() => new JObject();
}

/// <summary>
/// 单段流程信息
/// </summary>
public class SingleStagePlanInfo : DispensingProcessBaseInfo
{
    /// <summary>
    /// Mark配置信息
    /// </summary>
    public MarkConfigInfo MarkConfigInfo { get; set; }

    /// <summary>
    /// 方案中选择的区域信息列表
    /// </summary>
    public PlanAreaInfoList PlanAreaInfoes { set; get; }

    public SingleStagePlanInfo()
    {
        MarkConfigInfo = new MarkConfigInfo();
        PlanAreaInfoes = new PlanAreaInfoList();
    }

    /// <summary>
    /// 从JObject反序列化
    /// </summary>
    public override void FromJObject(JObject? jObject)
    {
        if (jObject == null) return;

        // Mark配置
        if (jObject["MarkConfigInfo"] is JObject markObj)
        {
            MarkConfigInfo ??= new MarkConfigInfo();
            MarkConfigInfo.FromJObject(markObj);
        }

        // 区域列表
        if (jObject["PlanAreaInfoes"] is JArray areaArray)
        {
            PlanAreaInfoes ??= new PlanAreaInfoList();
            PlanAreaInfoes.FromJObject(areaArray);
        }
    }

    /// <summary>
    /// 序列化为JObject
    /// </summary>
    public override JObject ToJObject()
    {
        var jObject = new JObject();
        jObject["MarkConfigInfo"] = MarkConfigInfo?.ToJObject() ?? new JObject();
        jObject["PlanAreaInfoes"] = PlanAreaInfoes?.ToJArray() ?? new JArray();
        return jObject;
    }
}

/// <summary>
/// 分段流程信息
/// </summary>
public class SegmentationStagePlanInfo : DispensingProcessBaseInfo
{
    /// <summary>
    /// 分段模式枚举
    /// </summary>
    public SegmentMode SegmentMode { get; set; }

    public SegmentationStagePlanInfo() { }

    /// <summary>
    /// 从JObject反序列化
    /// </summary>
    public override void FromJObject(JObject? jObject)
    {
        if (jObject == null) return;

        SegmentMode = jObject["SegmentMode"]?.Value<int>() switch
        {
            1 => SegmentMode.SecondBoardFeeding,
            2 => SegmentMode.ThirdBoardFeeding,
            0 => SegmentMode.BoardSplitting,
            _ => SegmentMode.BoardSplitting
        };
    }

    /// <summary>
    /// 序列化为JObject
    /// </summary>
    public override JObject ToJObject()
    {
        var jObject = new JObject();
        jObject["SegmentMode"] = (int)SegmentMode;
        return jObject;
    }
}

/// <summary>
/// 方案中选择的区域信息
/// </summary>
public class PlanAreaInfo : EntityBase
{
    /// <summary>
    /// 选择的区域ID
    /// </summary>
    public Guid AreaID { get; set; } = Guid.NewGuid();

    public PlanAreaInfo() { }

    /// <summary>
    /// 从JObject反序列化
    /// </summary>
    public new void FromJObject(JObject? jObject)
    {
        if (jObject == null) return;
        base.FromJObject(jObject);
        AreaID = Guid.TryParse(jObject["AreaID"]?.Value<string>(), out var guid) ? guid : Guid.NewGuid();
    }

    /// <summary>
    /// 序列化为JObject
    /// </summary>
    public new JObject ToJObject()
    {
        var jObject = new JObject();
        jObject = base.ToJObject();
        jObject["AreaID"] = AreaID.ToString();
        return jObject;
    }
}

/// <summary>
/// 方案中选择的区域信息列表
/// </summary>
public class PlanAreaInfoList : List<PlanAreaInfo>
{
    /// <summary>
    /// 从JArray反序列化
    /// </summary>
    public void FromJObject(JArray? jArray)
    {
        if (jArray == null) return;

        Clear();
        foreach (var itemObj in jArray)
        {
            if (itemObj is JObject itemJObj)
            {
                var areaInfo = new PlanAreaInfo();
                areaInfo.FromJObject(itemJObj);
                Add(areaInfo);
            }
        }
    }

    /// <summary>
    /// 序列化为JArray
    /// </summary>
    public JArray ToJArray()
    {
        var jArray = new JArray();
        foreach (var item in this)
        {
            jArray.Add(item.ToJObject());
        }
        return jArray;
    }
}

/// <summary>
/// 分段模式枚举
/// </summary>
public enum SegmentMode
{
    /// <summary>
    /// 分板模式
    /// </summary>
    BoardSplitting = 0,

    /// <summary>
    /// 二次进板模式
    /// </summary>
    SecondBoardFeeding = 1,

    /// <summary>
    /// 三次进板模式
    /// </summary>
    ThirdBoardFeeding = 2
}
