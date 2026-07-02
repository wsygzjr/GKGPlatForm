using Newtonsoft.JsonG.Linq;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.Area.AutoGenerate;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.Area.CustomEdit;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels;
using System.Linq;
using System.Reactive.Subjects;
using System;
using System.Collections.Generic;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.Area;

/// <summary>
/// 区域配置
/// </summary>
public class AreaConfigInfo
{
    /// <summary>
    /// 区域信息列表
    /// </summary>
    public AreaInfoList AreaInfoes { get; set; } 
    public AreaConfigInfo()
    {
        AreaInfoes=new AreaInfoList();  
    }
    /// <summary>
    /// 从JObject反序列化
    /// </summary>
    /// <param name="jObject">JSON对象</param>
    public void FromJObject(JObject? jObject)
    {
        if (jObject == null) return;

        // 反序列化区域信息列表
        if (jObject["AreaInfoes"] is JArray areaArray)
        {
            AreaInfoes ??= new AreaInfoList();
            AreaInfoes.FromJObject(areaArray);
        }
    }

    /// <summary>
    /// 序列化为JObject
    /// </summary>
    /// <returns>JSON对象</returns>
    public JObject ToJObject()
    {
        var jObject = new JObject();
        jObject["AreaInfoes"] = AreaInfoes?.ToJArray() ?? new JArray();
        return jObject;
    }
}
/// <summary>
/// 区域信息实体（继承EntityBase）
/// </summary>
public class AreaInfo : EntityBase
{
    /// <summary>
    /// 区域ID
    /// </summary>
    public Guid AreaID { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 区域名称
    /// </summary>
    public string AreaName { get; set; } = string.Empty;

    /// <summary>
    /// 区域编辑方式
    /// </summary>
    public AreaEditMode EditMode { get; set; }

    /// <summary>
    /// 自动生成区域信息
    /// </summary>
    public AutoGenerateAreaInfo AutoGenerateAreaInfo { get; set; }
    /// <summary>
    /// 点胶执行对象列表
    /// </summary>
    public List<DispensingExecutionObject> DispensingExecutionObjects { get; set; }
    public AreaInfo()
    {
        AutoGenerateAreaInfo=new AutoGenerateAreaInfo();
        DispensingExecutionObjects = new List<DispensingExecutionObject>();
    }
    /// <summary>
    /// 从JObject反序列化
    /// </summary>
    /// <param name="jObject">JSON对象</param>
    public new void FromJObject(JObject? jObject)
    {
        if (jObject == null) return;

        base.FromJObject(jObject);
        // 基础字段
        AreaID = Guid.TryParse(jObject["AreaID"]?.Value<string>(), out var guid) ? guid : Guid.NewGuid();
        AreaName = jObject["AreaName"]?.Value<string>() ?? string.Empty;

        // 枚举类型
        EditMode = jObject["EditMode"]?.Value<string>() switch
        {
            nameof(AreaEditMode.CustomEdit) => AreaEditMode.CustomEdit,
            nameof(AreaEditMode.AutoGenerate) => AreaEditMode.AutoGenerate,
            _ => AreaEditMode.AutoGenerate 
        };

        // 嵌套对象：自动生成区域信息
        if (jObject["AutoGenerateAreaInfo"] is JObject autoGenObj)
        {
            AutoGenerateAreaInfo ??= new AutoGenerateAreaInfo();
            AutoGenerateAreaInfo.FromJObject(autoGenObj);
        }

        // 集合：点胶执行对象列表
        if (jObject["DispensingExecutionObjects"] is JArray execArray)
        {
            DispensingExecutionObjects ??= new List<DispensingExecutionObject>();
            DispensingExecutionObjects.Clear();
            foreach (var itemObj in execArray)
            {
                if (itemObj is JObject itemJObj)
                {
                    var execObj = new DispensingExecutionObject();
                    execObj.FromJObject(itemJObj);
                    DispensingExecutionObjects.Add(execObj);
                }
            }
        }
    }

    /// <summary>
    /// 序列化为JObject
    /// </summary>
    /// <returns>JSON对象</returns>
    public new JObject ToJObject()
    {
        var jObject = new JObject();

        jObject=base.ToJObject();
        // 基础字段
        jObject["AreaID"] = AreaID.ToString();
        jObject["AreaName"] = AreaName;
        jObject["EditMode"] = EditMode.ToString();

        // 嵌套对象：自动生成区域信息
        jObject["AutoGenerateAreaInfo"] = AutoGenerateAreaInfo?.ToJObject() ?? new JObject();

        // 集合：点胶执行对象列表
        var execArray = new JArray();
        foreach (var item in DispensingExecutionObjects ?? new List<DispensingExecutionObject>())
        {
            execArray.Add(item.ToJObject());
        }
        jObject["DispensingExecutionObjects"] = execArray;

        return jObject;
    }
}
/// <summary>
/// 区域信息列表
/// </summary>
public class AreaInfoList : List<AreaInfo>
{
    /// <summary>
    /// 从JArray反序列化
    /// </summary>
    /// <param name="jArray">JSON数组</param>
    public void FromJObject(JArray? jArray)
    {
        if (jArray == null) return;

        Clear();
        foreach (var itemObj in jArray)
        {
            if (itemObj is JObject itemJObject)
            {
                var areaInfo = new AreaInfo();
                areaInfo.FromJObject(itemJObject);
                Add(areaInfo);
            }
        }
    }

    /// <summary>
    /// 序列化为JArray
    /// </summary>
    /// <returns>JSON数组</returns>
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
/// 区域编辑方式枚举
/// </summary>
public enum AreaEditMode
{
    /// <summary>
    /// 采用规则自动生成
    /// </summary>
    AutoGenerate,
    /// <summary>
    /// 自定义编辑
    /// </summary>
    CustomEdit
}
/// <summary>
/// 区域自动生成方式枚举
/// </summary>
public enum AutoGenerateMode
{
    /// <summary>
    /// 一维
    /// </summary>
    OneDimension,
    /// <summary>
    /// 二维
    /// </summary>
    TwoDimension
}

