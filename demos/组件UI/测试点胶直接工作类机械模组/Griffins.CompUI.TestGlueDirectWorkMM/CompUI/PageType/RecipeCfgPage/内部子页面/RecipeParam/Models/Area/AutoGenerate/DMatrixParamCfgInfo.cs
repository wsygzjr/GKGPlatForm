using Griffins.CompUI.TestGlueDirectWorkMM.CompUI.PageType;
using Griffins.UI.General;
using Newtonsoft.JsonG.Linq;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.Area.AutoGenerate;
/// <summary>
/// 矩阵参数配置信息
/// </summary>
public class DMatrixParamCfgInfo
{
    /// <summary>
    /// 列数
    /// </summary>
    public int ColumnCount { get; set; } = 5;

    /// <summary>
    /// 行数
    /// </summary>
    public int RowCount { get; set; } = 1;

    /// <summary>
    /// 起始点
    /// </summary>
    public StartPointType StartPoint { get; set; } = StartPointType.TopLeft;

    /// <summary>
    /// 矩阵方向
    /// </summary>
    public MatrixDirectionType MatrixDirection { get; set; } = MatrixDirectionType.X;

    /// <summary>
    /// 路径形状
    /// </summary>
    public PathShapeType PathShape { get; set; } = PathShapeType.Line;

    /// <summary>
    /// 点阵左上位置
    /// </summary>
    public BasePositionInfo DotMatrix_LeftUpperPositionInfo { get; set; }

    /// <summary>
    /// 点阵右上位置
    /// </summary>
    public BasePositionInfo DotMatrix_RightUpperPositionInfo { get; set; }
    /// <summary>
    /// 点阵右下位置
    /// </summary>
    public BasePositionInfo DotMatrix_RightLowerPositionInfo { get; set; }
    public DMatrixParamCfgInfo()
    {
        DotMatrix_LeftUpperPositionInfo = new BasePositionInfo();
        DotMatrix_RightUpperPositionInfo = new BasePositionInfo();
        DotMatrix_RightLowerPositionInfo = new BasePositionInfo();
    }

    /// <summary>
    /// 从JObject反序列化
    /// </summary>
    public void FromJObject(JObject? jObject)
    {
        if (jObject == null) return;

       
        ColumnCount = jObject["ColumnCount"]?.Value<int>() ?? 5;
        RowCount = jObject["RowCount"]?.Value<int>() ?? 1;

        StartPoint = jObject["StartPoint"] != null
            ? (StartPointType)jObject["StartPoint"].Value<int>()
            : StartPointType.TopLeft;

        MatrixDirection = jObject["MatrixDirection"] != null
            ? (MatrixDirectionType)jObject["MatrixDirection"].Value<int>()
            : MatrixDirectionType.X;

        PathShape = jObject["PathShape"] != null
            ? (PathShapeType)jObject["PathShape"].Value<int>()
            : PathShapeType.Line;

        if (jObject["DotMatrix_LeftUpperPositionInfo"] is JObject dotMatrix_LeftUpperPositionInfo)
        {
            DotMatrix_LeftUpperPositionInfo.FromJObject(dotMatrix_LeftUpperPositionInfo);
        }
        if (jObject["DotMatrix_RightUpperPositionInfo"] is JObject dotMatrix_RightUpperPositionInfo)
        {
            DotMatrix_RightUpperPositionInfo.FromJObject(dotMatrix_RightUpperPositionInfo);
        }
        if (jObject["DotMatrix_RightLowerPositionInfo"] is JObject dotMatrix_RightLowerPositionInfo)
        {
            DotMatrix_RightLowerPositionInfo.FromJObject(dotMatrix_RightLowerPositionInfo);
        }
    }

    /// <summary>
    /// 序列化为JObject
    /// </summary>
    public JObject ToJObject()
    {
        return new JObject
            {
                { "ColumnCount", ColumnCount },
                { "RowCount", RowCount },
                { "StartPoint", (int)StartPoint },
                { "MatrixDirection", (int)MatrixDirection },
                { "PathShape", (int)PathShape },
                { "DotMatrix_LeftUpperPositionInfo", DotMatrix_LeftUpperPositionInfo.ToJObject() },
                { "DotMatrix_RightUpperPositionInfo", DotMatrix_RightUpperPositionInfo.ToJObject() },
                { "DotMatrix_RightLowerPositionInfo", DotMatrix_RightLowerPositionInfo.ToJObject() },

            };
    }

    /// <summary>
    /// 复制
    /// </summary>
    public void CopyFrom(DMatrixParamCfgInfo source)
    {
        if (source == null) return;
        
        ColumnCount = source.ColumnCount;
        RowCount = source.RowCount;
        StartPoint = source.StartPoint;
        MatrixDirection = source.MatrixDirection;
        PathShape = source.PathShape;
    }
}

#region 相关枚举定义

/// <summary>
/// 起始点类型
/// </summary>
public enum StartPointType
{
    /// <summary>
    /// 左上
    /// </summary>
    TopLeft,
    /// <summary>
    /// 右上
    /// </summary>
    TopRight,
    /// <summary>
    /// 左下
    /// </summary>
    BottomLeft,
    /// <summary>
    /// 右下
    /// </summary>
    BottomRight
}

/// <summary>
/// 矩阵方向类型
/// </summary>
public enum MatrixDirectionType
{
    /// <summary>
    /// X方向
    /// </summary>
    X,
    /// <summary>
    /// Y方向
    /// </summary>
    Y,
    /// <summary>
    /// Z方向
    /// </summary>
    Z
}

/// <summary>
/// 路径形状类型
/// </summary>
public enum PathShapeType
{
    /// <summary>
    /// S型
    /// </summary>
    S,
    /// <summary>
    /// 直线
    /// </summary>
    Line,
    /// <summary>
    /// 锯齿形
    /// </summary>
    Zigzag,
    /// <summary>
    /// 圆形
    /// </summary>
    Circle
}
#endregion
