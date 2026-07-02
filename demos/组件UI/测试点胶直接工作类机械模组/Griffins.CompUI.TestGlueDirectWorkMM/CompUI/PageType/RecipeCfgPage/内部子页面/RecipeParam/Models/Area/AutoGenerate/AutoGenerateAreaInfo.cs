using Newtonsoft.JsonG.Linq;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.Area;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.Area.AutoGenerate;

/// <summary>
/// 自动生成区域信息实体
/// </summary>
public class AutoGenerateAreaInfo
{
    /// <summary>
    /// 区域自动生成方式枚举
    /// </summary>
    public AutoGenerateMode AutoGenerateMode { get; set; }

    /// <summary>
    /// 一维矩阵参数配置信息
    /// 当区域自动生成方式为一维时有效
    /// </summary>
    public OneDMatrixParamCfgInfo OneDMatrixParamCfgInfo { get; set; }

    /// <summary>
    /// 二维矩阵参数配置信息
    /// 当区域自动生成方式为二维时有效
    /// </summary>
    public TwoDMatrixParamCfgInfo TwoDMatrixParamCfgInfo { get; set; }

    public AutoGenerateAreaInfo()
    {
        OneDMatrixParamCfgInfo = new OneDMatrixParamCfgInfo();
        TwoDMatrixParamCfgInfo = new TwoDMatrixParamCfgInfo();
    }

    /// <summary>
    /// 从JObject反序列化
    /// </summary>
    /// <param name="jObject">JSON对象</param>
    public void FromJObject(JObject? jObject)
    {
        if (jObject == null) return;

        // 枚举类型
        AutoGenerateMode = jObject["AutoGenerateMode"]?.Value<string>() switch
        {
            nameof(AutoGenerateMode.TwoDimension) => AutoGenerateMode.TwoDimension,
            nameof(AutoGenerateMode.OneDimension) => AutoGenerateMode.OneDimension,
            _ => AutoGenerateMode.OneDimension 
        };

        // 嵌套对象：一维矩阵参数
        if (jObject["OneDMatrixParamCfgInfo"] is JObject oneDObj)
        {
            OneDMatrixParamCfgInfo ??= new OneDMatrixParamCfgInfo();
            OneDMatrixParamCfgInfo.FromJObject(oneDObj);
        }

        // 嵌套对象：二维矩阵参数
        if (jObject["TwoDMatrixParamCfgInfo"] is JObject twoDObj)
        {
            TwoDMatrixParamCfgInfo ??= new TwoDMatrixParamCfgInfo();
            TwoDMatrixParamCfgInfo.FromJObject(twoDObj);
        }
    }

    /// <summary>
    /// 序列化为JObject
    /// </summary>
    /// <returns>JSON对象</returns>
    public JObject ToJObject()
    {
        var jObject = new JObject();

        jObject["AutoGenerateMode"] = AutoGenerateMode.ToString();
        jObject["OneDMatrixParamCfgInfo"] = OneDMatrixParamCfgInfo?.ToJObject() ?? new JObject();
        jObject["TwoDMatrixParamCfgInfo"] = TwoDMatrixParamCfgInfo?.ToJObject() ?? new JObject();

        return jObject;
    }
}
