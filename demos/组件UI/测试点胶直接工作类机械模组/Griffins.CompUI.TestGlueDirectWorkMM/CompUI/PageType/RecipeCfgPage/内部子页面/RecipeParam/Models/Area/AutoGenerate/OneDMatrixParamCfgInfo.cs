using Newtonsoft.JsonG.Linq;
using System;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.Area.AutoGenerate;

/// <summary>
/// 一维矩阵参数配置信息
/// </summary>
public class OneDMatrixParamCfgInfo
{
    /// <summary>
    /// 所属模板ID
    /// </summary>
    public Guid TemplateId { get; set; }

    /// <summary>
    /// 启用换行抬针
    /// </summary>
    public bool EnableLiftNeedleOnNewLine { get; set; }

    /// <summary>
    /// 启用换行清洁
    /// </summary>
    public bool EnableCleanOnNewLine { get; set; }

    /// <summary>
    /// 抬针高度
    /// </summary>
    public decimal LiftNeedleHeight { get; set; } = 10.0m;

    /// <summary>
    /// 换行首点稳定时间
    /// </summary>
    public decimal NewLineFirstPointStableTime { get; set; } = 100;

    /// <summary>
    /// 一维矩阵参数配置信息
    /// </summary>
    public DMatrixParamCfgInfo OneOfDMatrixParamCfgInfo { get; set; }

    public OneDMatrixParamCfgInfo()
    {
        OneOfDMatrixParamCfgInfo = new DMatrixParamCfgInfo();
    }

    /// <summary>
    /// 从JObject反序列化
    /// </summary>
    public void FromJObject(JObject? jObject)
    {
        if (jObject == null) return;

        TemplateId = Guid.TryParse(jObject["TemplateId"]?.Value<string>(), out var cmdGuid) ? cmdGuid : Guid.NewGuid();
        EnableLiftNeedleOnNewLine = jObject["EnableLiftNeedleOnNewLine"]?.Value<bool>() ?? false;
        EnableCleanOnNewLine = jObject["EnableCleanOnNewLine"]?.Value<bool>() ?? false;
        LiftNeedleHeight = jObject["LiftNeedleHeight"]?.Value<decimal>() ?? 10.0m;
        NewLineFirstPointStableTime = jObject["NewLineFirstPointStableTime"]?.Value<decimal>() ?? 100;
        

        if (jObject["OneOfDMatrixParamCfgInfo"] is JObject oneOfDMatrixParamCfgInfo)
        {
            OneOfDMatrixParamCfgInfo.FromJObject(oneOfDMatrixParamCfgInfo);
        }
    }

    /// <summary>
    /// 序列化为JObject
    /// </summary>
    public JObject ToJObject()
    {
        return new JObject
            {
                { "TemplateId", TemplateId },
                { "EnableLiftNeedleOnNewLine", EnableLiftNeedleOnNewLine },
                { "EnableCleanOnNewLine", EnableCleanOnNewLine },
                { "LiftNeedleHeight", LiftNeedleHeight },
                { "NewLineFirstPointStableTime", NewLineFirstPointStableTime },
                { "OneOfDMatrixParamCfgInfo", OneOfDMatrixParamCfgInfo.ToJObject() },
            };
    }

    /// <summary>
    /// 复制
    /// </summary>
    public void CopyFrom(OneDMatrixParamCfgInfo source)
    {
        if (source == null) return;

        TemplateId = source.TemplateId;
        EnableLiftNeedleOnNewLine = source.EnableLiftNeedleOnNewLine;
        EnableCleanOnNewLine = source.EnableCleanOnNewLine;
        LiftNeedleHeight = source.LiftNeedleHeight;
        NewLineFirstPointStableTime = source.NewLineFirstPointStableTime;
    }
}
