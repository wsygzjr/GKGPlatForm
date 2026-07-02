using GKG.Maths;
using Griffins.ImeIOT;

namespace GKG
{
    /// <summary>
    /// Mark类型
    /// </summary>
    public enum MarkType
    {
        /// <summary>
        /// 模板基准
        /// </summary>
        TemplateBaseline,

        /// <summary>
        /// 定位补偿
        /// </summary>
        PositioningCompensation
    }

    /// <summary>
    /// Mark操作类型
    /// </summary>
    public enum EditType
    {
        /// <summary>
        /// 标准
        /// </summary>
        General = 0,

        /// <summary>
        /// 摄像头模组
        /// </summary>
        CameraGroup = 1,

        /// <summary>
        /// 边角中心
        /// </summary>
        EdgeCenter = 2,

        /// <summary>
        /// 自定义
        /// </summary>
        UserDefined = 3,
    }

    /// <summary>
    /// Mark点参数
    /// </summary>
    public class MarkParam
    {
        /// <summary>
        /// Mark点位
        /// </summary>
        public Point3D Position { get; set; } = new Point3D();

        /// <summary>
        /// 操作类型
        /// </summary>
        public EditType EditType { get; set; } = EditType.General;

        /// <summary>
        /// Mark模板路径
        /// </summary>
        public string MarkModelPath { get; set; } = string.Empty;
    }

    /// <summary>
    /// Mark点参数列表(包含备份mark)
    /// </summary>
    public class MarkList : List<MarkParam>
    {
    }

    /// <summary>
    /// Mark点结果
    /// </summary>
    public class MarkResult : SearchMarkResult
    {
        /// <summary>
        /// Mark位置
        /// </summary>
        public Point3D Position { get; set; }
    }

    /// <summary>
    /// Mark结果
    /// </summary>
    public class MarkResults : List<MarkResult>
    {
        /// <summary>
        /// MarkID
        /// </summary>
        public string MarkID { get; set; } = "";

        /// <summary>
        /// 是否识别OK
        /// </summary>
        public bool IsOk { get; set; } = false;

        private AffineTransformParams affineTransformParams = new AffineTransformParams();

        /// <summary>
        /// 利用mark数据补偿点位
        /// </summary>
        /// <param name="point3D"></param>
        public new void Add(MarkResult item)
        {
            base.Add(item);
            // 计算仿射变换参数
            if (this.Count == 1)
            {
                // 一个点的情况下直接使用视觉的计算结果
                affineTransformParams = new AffineTransformParams(this[0].Offset.X, this[0].Offset.Y, this[0].ScaleX, this[0].ScaleY, this[0].Angle, false);
            }
            else if (this.Count == 2)
            {
                // 两个点的情况下计算相似变换 平移+旋转+缩放
                affineTransformParams = SimilarityTransformCalculator.ComputeFromTwoPoints(
                    new Point2D(this[0].Position.X, this[0].Position.Y), new Point2D(this[1].Position.X, this[1].Position.Y),
                    new Point2D(this[0].Position.X + this[0].Offset.X, this[0].Position.Y + this[0].Offset.Y), new Point2D(this[1].Position.X + this[1].Offset.X, this[1].Position.Y + this[1].Offset.Y));
            }
            else
            {
                // 多于两个点的情况下计算仿射变换矩阵
                List<Point2D> srcPoints = new List<Point2D>();
                List<Point2D> dstPoints = new List<Point2D>();
                foreach (var mark in this)
                {
                    srcPoints.Add(new Point2D(mark.Position.X, mark.Position.Y));
                    dstPoints.Add(new Point2D(mark.Position.X + mark.Offset.X, mark.Position.Y + mark.Offset.Y));
                }
                affineTransformParams = AffineTransformCalculator.DecomposeAffineMatrix(AffineTransformCalculator.CalculateAffineTransform(srcPoints, dstPoints));
            }
        }

        /// <summary>
        /// 补偿计算
        /// </summary>
        /// <param name="point3D"></param>
        public void Compensate(ref Point3D point3D)
        {
            if (affineTransformParams != null)
            {
                // 进行仿射变换计算
                point3D = AffineTransformCalculator.TransformPoint(point3D, affineTransformParams);
            }
        }
    }

    namespace MM
    {
        /// <summary>
        /// Mark
        /// </summary>
        public partial class MMCmdExecutor : IMMCmdExecutor
        {
            public partial class Mark : CmdBase
            {
                public override CmdType CmdType => CmdType.Mark;
                /// <summary>
                /// MarkID
                /// </summary>
                public string MarkID { get; set; } = "";

                /// <summary>
                /// Mark点类型
                /// </summary>
                public MarkType MarkType { get; set; } = MarkType.PositioningCompensation;

                /// <summary>
                /// 是否启用角度
                /// </summary>
                public bool UseAngle { get; set; } = false;

                /// <summary>
                /// 是否分离
                /// </summary>
                public bool IsSeparated { get; set; } = false;

                /// <summary>
                /// Mark报警偏差值范围
                /// </summary>
                public double OffsetAlarmRange { get; set; }

                /// <summary>
                /// 未找到自动跳过
                /// </summary>
                public bool IsAutoJump { get; set; } = false;

                /// <summary>
                /// Mark参数集合
                /// </summary>
                public List<MarkList> MarkListCollection { get; set; } = new List<MarkList>();

                /// <summary>
                /// mark结果
                /// </summary>
                public MarkResults Results = new MarkResults();

                /// <summary>
                /// mark更新点位
                /// </summary>
                /// <param name="cmd"></param>
                public override void UpdateCmd(CmdBase cmd)
                {
                    // markNG 跳过
                    if (!this.Results.IsOk)
                        cmd.Enabled = false;

                    // 根据类型补偿点位
                    switch (cmd.CmdType)
                    {
                        case CmdType.Mark:
                            {
                                var mark = (Mark)cmd;
                                foreach (var marklist in mark.MarkListCollection)
                                {
                                    foreach (var item in marklist)
                                    {
                                        // 不能直接 ref 属性，需用临时变量
                                        var pos = item.Position;
                                        this.Results.Compensate(ref pos);
                                        item.Position = pos;
                                    }
                                }
                            }
                            break;

                        case CmdType.BadMark:
                            {
                                var badmark = (BadMark)cmd;
                                var pos = badmark.MarkParam.Position;
                                this.Results.Compensate(ref pos);
                                badmark.MarkParam.Position = pos;
                            }
                            break;
                    }
                }
            }
        }
    }
}