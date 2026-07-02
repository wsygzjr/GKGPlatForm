using Newtonsoft.JsonG.Linq;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.Area.AutoGenerate;

/// <summary>
/// 二维矩阵参数配置信息
/// </summary>
public class TwoDMatrixParamCfgInfo: OneDMatrixParamCfgInfo
{
    /// <summary>
    /// 二维矩阵参数配置信息
    /// </summary>
    public DMatrixParamCfgInfo TwoOfDMatrixParamCfgInfo { get; set; }

    public TwoDMatrixParamCfgInfo()
    {
        TwoOfDMatrixParamCfgInfo = new DMatrixParamCfgInfo();
    }

    /// <summary>
    /// 从JObject反序列化
    /// </summary>
    public new void FromJObject(JObject? jObject)
    {
        if (jObject == null) return;
        base.FromJObject(jObject);

        if (jObject["TwoOfDMatrixParamCfgInfo"] is JObject twoOfDMatrixParamCfgInfo)
        {
            TwoOfDMatrixParamCfgInfo.FromJObject(twoOfDMatrixParamCfgInfo);
        }
    }

    /// <summary>
    /// 序列化为JObject
    /// </summary>
    public new JObject ToJObject()
    {
        JObject jObject= base.ToJObject();
        jObject["TwoOfDMatrixParamCfgInfo"] = TwoOfDMatrixParamCfgInfo.ToJObject();
        return jObject;
    }

    /// <summary>
    /// 复制
    /// </summary>
    public void CopyFrom(TwoDMatrixParamCfgInfo source)
    {
        if (source == null) return;
        base.CopyFrom(source);
        TwoOfDMatrixParamCfgInfo.CopyFrom(source.TwoOfDMatrixParamCfgInfo);
    }
}
