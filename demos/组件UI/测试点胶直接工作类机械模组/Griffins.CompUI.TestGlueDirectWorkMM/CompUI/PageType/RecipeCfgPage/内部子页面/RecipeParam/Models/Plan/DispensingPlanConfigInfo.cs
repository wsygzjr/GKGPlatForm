
using Newtonsoft.JsonG.Linq;
using System;
using System.Collections.Generic;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels;
using Griffins.CompUI.TestGlueDirectWorkMM.CompUI.PageType;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.Plan;

/// <summary>
/// 点胶方案配置
/// </summary>
public class DispensingPlanConfigInfo
{
    /// <summary>
    /// 方案信息
    /// </summary>
    public DispensingPlanInfo PlanInfo { get; set; }
    /// <summary>
    /// 点胶方案其他参数配置
    /// </summary>
    public DispensingPlanOtherConfigInfo DispensingPlanOtherConfigInfo { get; set; }

    public DispensingPlanConfigInfo()
    {
        PlanInfo = new DispensingPlanInfo();
        DispensingPlanOtherConfigInfo = new DispensingPlanOtherConfigInfo();
    }

    /// <summary>
    /// 从JObject反序列化
    /// </summary>
    /// <param name="jObject">JSON对象</param>
    public void FromJObject(JObject? jObject)
    {
        if (jObject == null) return;

        // 反序列化方案信息
        if (jObject["PlanInfo"] is JObject planObj)
        {
            PlanInfo ??= new DispensingPlanInfo();
            PlanInfo.FromJObject(planObj);
        }

        // 反序列化其他参数配置
        if (jObject["DispensingPlanOtherConfigInfo"] is JObject otherObj)
        {
            DispensingPlanOtherConfigInfo ??= new DispensingPlanOtherConfigInfo();
            DispensingPlanOtherConfigInfo.FromJObject(otherObj);
        }
    }

    /// <summary>
    /// 序列化为JObject
    /// </summary>
    /// <returns>JSON对象</returns>
    public JObject ToJObject()
    {
        var jObject = new JObject();
        jObject["PlanInfo"] = PlanInfo?.ToJObject() ?? new JObject();
        jObject["DispensingPlanOtherConfigInfo"] = DispensingPlanOtherConfigInfo?.ToJObject() ?? new JObject();
        return jObject;
    }
}

/// <summary>
/// 点胶方案其他参数配置
/// </summary>
public class DispensingPlanOtherConfigInfo
{
    /// <summary>
    /// 配方整板偏移量
    /// </summary>
    public Point2D RecipeBoardOffset { get; set; }

    /// <summary>
    /// 2D检测间隔大PCS数
    /// </summary>
    public int TwoDCheckIntervalPcs { get; set; }

    /// <summary>
    /// 空跑坏板（开关按钮：true=启用，false=禁用）
    /// </summary>
    public bool RunEmptyBadBoard { get; set; }

    /// <summary>
    /// 使用第一块板的Mark（开关按钮：true=启用，false=禁用）
    /// </summary>
    public bool UseFirstBoardMark { get; set; }

    public DispensingPlanOtherConfigInfo()
    {
        RecipeBoardOffset = new Point2D();
    }

    /// <summary>
    /// 从JObject反序列化
    /// </summary>
    public void FromJObject(JObject? jObject)
    {
        if (jObject == null) return;

        // 反序列化偏移量
        if (jObject["RecipeBoardOffset"] is JObject offsetObj)
        {
            RecipeBoardOffset ??= new Point2D();
            //RecipeBoardOffset.FromJObject(offsetObj);
        }

        // 基础类型
        TwoDCheckIntervalPcs = jObject["TwoDCheckIntervalPcs"]?.Value<int>() ?? 0;
        RunEmptyBadBoard = jObject["RunEmptyBadBoard"]?.Value<bool>() ?? false;
        UseFirstBoardMark = jObject["UseFirstBoardMark"]?.Value<bool>() ?? false;
    }

    /// <summary>
    /// 序列化为JObject
    /// </summary>
    public JObject ToJObject()
    {
        var jObject = new JObject();
        //jObject["RecipeBoardOffset"] = RecipeBoardOffset?.ToJObject() ?? new JObject();
        jObject["TwoDCheckIntervalPcs"] = TwoDCheckIntervalPcs;
        jObject["RunEmptyBadBoard"] = RunEmptyBadBoard;
        jObject["UseFirstBoardMark"] = UseFirstBoardMark;
        return jObject;
    }
}

/// <summary>
/// 点胶方案信息
/// </summary>
public class DispensingPlanInfo
{
    /// <summary>
    /// 点胶流程类型
    /// </summary>
    public DispensingProcessType DispensingProcessType { get; set; }
    /// <summary>
    /// 流程信息列表
    /// </summary>
    public List<DispensingProcessItemInfo> DispensingProcessInfos { get; set; }

    public DispensingPlanInfo()
    {
        DispensingProcessInfos = new List<DispensingProcessItemInfo>();
    }

    /// <summary>
    /// 从JObject反序列化
    /// </summary>
    public void FromJObject(JObject? jObject)
    {
        if (jObject == null) return;

        // 枚举类型
        DispensingProcessType = jObject["DispensingProcessType"]?.Value<int>() switch
        {
            1 => DispensingProcessType.SegmentationStage,
            0 => DispensingProcessType.SingleStage,
            _ => DispensingProcessType.SingleStage
        };

        // 流程列表
        if (jObject["DispensingProcessInfos"] is JArray processArray)
        {
            DispensingProcessInfos ??= new List<DispensingProcessItemInfo>();
            DispensingProcessInfos.Clear();
            foreach (var itemObj in processArray)
            {
                if (itemObj is JObject itemJObj)
                {
                    var processItem = new DispensingProcessItemInfo();
                    processItem.FromJObject(itemJObj);
                    DispensingProcessInfos.Add(processItem);
                }
            }
        }
    }

    /// <summary>
    /// 序列化为JObject
    /// </summary>
    public JObject ToJObject()
    {
        var jObject = new JObject();
        jObject["DispensingProcessType"] = (int)DispensingProcessType;

        // 流程列表
        var processArray = new JArray();
        foreach (var item in DispensingProcessInfos ?? new List<DispensingProcessItemInfo>())
        {
            processArray.Add(item.ToJObject());
        }
        jObject["DispensingProcessInfos"] = processArray;

        return jObject;
    }
}

/// <summary>
/// 点胶流程项信息
/// </summary>
public class DispensingProcessItemInfo : EntityBase
{
    /// <summary>
    /// 流程名称
    /// </summary>
    public string ProcessName { get; set; } = "";
    /// <summary>
    /// 点胶流程类型
    /// </summary>
    public DispensingProcessType DispensingProcessType { get; set; }
    /// <summary>
    /// 点胶流程信息（多态处理）
    /// </summary>
    public DispensingProcessBaseInfo DispensingProcessInfo { get; set; }

    public DispensingProcessItemInfo()
    {
        DispensingProcessInfo = new SingleStagePlanInfo();
    }

    /// <summary>
    /// 从JObject反序列化（兼容多态）
    /// </summary>
    public new void FromJObject(JObject? jObject)
    {
        if (jObject == null) return;
        base.FromJObject(jObject);
        // 基础字段
        ProcessName = jObject["ProcessName"]?.Value<string>() ?? "";
        DispensingProcessType = jObject["DispensingProcessType"]?.Value<int>() switch
        {
            1 => DispensingProcessType.SegmentationStage,
            0 => DispensingProcessType.SingleStage,
            _ => DispensingProcessType.SingleStage
        };

        // 多态反序列化：根据流程类型创建对应实例
        var processBase = new DispensingProcessBaseInfo().Create(DispensingProcessType);
        if (jObject["DispensingProcessInfo"] is JObject processInfoObj)
        {
            if (processBase is SingleStagePlanInfo singleStage)
            {
                singleStage.FromJObject(processInfoObj);
                DispensingProcessInfo = singleStage;
            }
            else if (processBase is SegmentationStagePlanInfo segmentStage)
            {
                segmentStage.FromJObject(processInfoObj);
                DispensingProcessInfo = segmentStage;
            }
        }
    }

    /// <summary>
    /// 序列化为JObject（兼容多态）
    /// </summary>
    public new  JObject ToJObject()
    {
        var jObject = new JObject();
        jObject = base.ToJObject();
        jObject["ProcessName"] = ProcessName;
        jObject["DispensingProcessType"] = (int)DispensingProcessType;

        // 多态序列化
        if (DispensingProcessInfo is SingleStagePlanInfo singleStage)
        {
            jObject["DispensingProcessInfo"] = singleStage.ToJObject();
        }
        else if (DispensingProcessInfo is SegmentationStagePlanInfo segmentStage)
        {
            jObject["DispensingProcessInfo"] = segmentStage.ToJObject();
        }
        else
        {
            jObject["DispensingProcessInfo"] = new JObject();
        }

        return jObject;
    }
}

/// <summary>
/// 点胶流程类型枚举
/// </summary>
public enum DispensingProcessType
{
    /// <summary>
    /// 单段流程
    /// </summary>
    SingleStage = 0,
    /// <summary>
    /// 分段流程
    /// </summary>
    SegmentationStage = 1
}
