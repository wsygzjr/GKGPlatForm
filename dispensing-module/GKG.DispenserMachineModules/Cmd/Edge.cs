using Griffins.ImeIOT;

namespace GKG
{
    /// <summary>
    /// 抓边点
    /// </summary>
    public class EdgePoint : MotionTrajectoryPoint
    {
        /// <summary>
        /// 视野ID
        /// </summary>
        public int VisualFieldIndex { get; set; }

        /// <summary>
        /// 抓边样式参数
        /// </summary>
        public SearchEdgeParams EdgeStyleParam { get; set; } = new SearchEdgeParams();

        public EdgePoint(int VisualFieldIndex, MotionTrajectoryPoint linearMotionTrajectoryPoint)
        {
            this.Position = linearMotionTrajectoryPoint.Position;
            this.InProcessingParameters = linearMotionTrajectoryPoint.InProcessingParameters;
        }

        public EdgePoint(int visualFieldIndex, MotionTrajectoryPoint linearMotionTrajectoryPoint, SearchEdgeParams edgeStyleParam)
        {
            this.VisualFieldIndex = visualFieldIndex;
            this.Position = linearMotionTrajectoryPoint.Position;
            this.InProcessingParameters = linearMotionTrajectoryPoint.InProcessingParameters;
            this.EdgeStyleParam = edgeStyleParam;
        }
    }

    public class EdgeDot : DotMotionTrajectory
    {
        /// <summary>
        /// 视野ID
        /// </summary>
        public int VisualFieldIndex { get; set; }

        /// <summary>
        /// 抓边样式参数
        /// </summary>
        public SearchEdgeParams EdgeStyleParam { get; set; } = new SearchEdgeParams();
    }

    /// <summary>
    /// 抓边直线轨迹
    /// </summary>
    public class EdgeStraightLine : LinearMotionTrajectoryItemStraightLine
    {
        public EdgeStraightLine(int visualFieldIndex, LinearMotionTrajectoryItemStraightLine straightLine)
        {
            this.EndPoint = new EdgePoint(visualFieldIndex, straightLine?.EndPoint);
        }

        public EdgeStraightLine(int visualFieldIndex, LinearMotionTrajectoryItemStraightLine straightLine, SearchEdgeParams edgeStyleParam)
        {
            this.EndPoint = new EdgePoint(visualFieldIndex, straightLine?.EndPoint, edgeStyleParam);
        }
    }

    /// <summary>
    /// 抓边圆弧A轨迹
    /// </summary>
    public class EdgeArcA : LinearMotionTrajectoryItemArcA
    {
        public EdgeArcA(int visualFieldIndex, LinearMotionTrajectoryItemArcA arcA, SearchEdgeParams edgeStyleParam)
        {
            this.MiddlePoint = new EdgePoint(visualFieldIndex, arcA?.MiddlePoint, edgeStyleParam);
            this.EndPoint = new EdgePoint(visualFieldIndex, arcA?.EndPoint, edgeStyleParam);
        }

        public EdgeArcA(int visualFieldIndex, LinearMotionTrajectoryItemArcA arcA, SearchEdgeParams midEdgeStyleParam, SearchEdgeParams endEdgeStyleParam)
        {
            this.MiddlePoint = new EdgePoint(visualFieldIndex, arcA?.MiddlePoint, midEdgeStyleParam);
            this.EndPoint = new EdgePoint(visualFieldIndex, arcA?.EndPoint, endEdgeStyleParam);
        }
    }

    /// <summary>
    /// 抓边圆弧B轨迹
    /// </summary>
    public class EdgeArcB : LinearMotionTrajectoryItemArcB
    {
        public EdgeArcB(int visualFieldIndex, LinearMotionTrajectoryItemArcB arcB, SearchEdgeParams edgeStyleParam)
        {
            this.CenterPoint = new EdgePoint(visualFieldIndex, arcB?.CenterPoint, edgeStyleParam);
            this.EndPoint = new EdgePoint(visualFieldIndex, arcB?.EndPoint, edgeStyleParam);
        }

        public EdgeArcB(int visualFieldIndex, LinearMotionTrajectoryItemArcB arcB, SearchEdgeParams centerEdgeStyleParam, SearchEdgeParams endEdgeStyleParam)
        {
            this.CenterPoint = new EdgePoint(visualFieldIndex, arcB?.CenterPoint, centerEdgeStyleParam);
            this.EndPoint = new EdgePoint(visualFieldIndex, arcB?.EndPoint, endEdgeStyleParam);
        }
    }

    /// <summary>
    /// 抓边圆轨迹
    /// </summary>
    public class EdgeCircleA : LinearMotionTrajectoryItemCircleA
    {
        public EdgeCircleA(int visualFieldIndex, LinearMotionTrajectoryItemCircleA circleA, SearchEdgeParams edgeStyleParam)
        {
            this.MiddlePoint = new EdgePoint(visualFieldIndex, circleA?.MiddlePoint, edgeStyleParam);
            this.EndPoint = new EdgePoint(visualFieldIndex, circleA?.EndPoint, edgeStyleParam);
        }

        public EdgeCircleA(int visualFieldIndex, LinearMotionTrajectoryItemArcA arcA, SearchEdgeParams midEdgeStyleParam, SearchEdgeParams endEdgeStyleParam)
        {
            this.MiddlePoint = new EdgePoint(visualFieldIndex, arcA?.MiddlePoint, midEdgeStyleParam);
            this.EndPoint = new EdgePoint(visualFieldIndex, arcA?.EndPoint, endEdgeStyleParam);
        }
    }

    /// <summary>
    /// 抓边循环结构
    /// </summary>
    public class EdgeLoopParam
    {
        /// <summary>
        /// 抓边运行偏移
        /// </summary>
        public Point3D Offset { get; set; } = new Point3D();

        /// <summary>
        /// 抓边运行重量
        /// </summary>
        public double Weight { get; set; } = 0.0;
    }

    /// <summary>
    /// 抓边循环结构
    /// </summary>
    public class EdgeLoopParams
    {
        /// <summary>
        /// 循环次数
        /// </summary>
        public int LoopCount { get; set; } = 0;

        /// <summary>
        /// 循环参数列表
        /// </summary>
        public List<EdgeLoopParam> LoopParams { get; set; } = new List<EdgeLoopParam>();
    }

    namespace MM
    {
        /// <summary>
        /// Mark
        /// </summary>
        public partial class MMCmdExecutor : IMMCmdExecutor
        {
            /// <summary>
            /// 抓边指令
            /// </summary>
            public class Edge : CmdBase
            {
                public override CmdType CmdType => CmdType.Edge;
                /// <summary>
                /// 抓边视野基准位置
                /// </summary>
                ///
                /// <remarks>
                /// 抓边相对位置，主要针对阵列生成的点位，或是放置的点位，不针对视觉补偿后的点位，视觉补偿还是要更新抓边轨迹列表位置
                /// </remarks>
                public Point3D Position { get; set; } = new Point3D();

                /// <summary>
                /// 抓边轨迹项集合
                /// </summary>
                public List<MotionTrajectoryBase>? EdgeTrajectory { get; set; }

                /// <summary>
                /// 抓边循环参数
                /// </summary>
                public EdgeLoopParams EdgeLoopParams { get; set; } = new EdgeLoopParams();
            }
        }
    }
}