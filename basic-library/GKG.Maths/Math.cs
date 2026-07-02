using GKG.MotionControl;

namespace GKG
{
    namespace Maths
    {
        /// <summary>
        /// 运动计算
        /// </summary>
        public static partial class GKGMath
        {
            /// <summary>
            /// 计算两点线段长度
            /// </summary>
            public static double CalculateTwoPointsLength(Point2D start, Point2D end)
            {
                double deltaX = end.X - start.X;
                double deltaY = end.Y - start.Y;
                return System.Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
            }

            /// <summary>
            /// 计算三点圆弧长度
            /// </summary>
            public static double CalculateArcLengthByThreePoints(Point2D start, Point2D middle, Point2D end)
            {
                // 步骤：
                // 1. 计算三点确定的圆的圆心和半径
                // 2. 计算圆心到三点的夹角（弧度）
                // 3. 用弧度乘以半径得到弧长

                // 计算两点距离
                double a = CalculateTwoPointsLength(middle, end);
                double b = CalculateTwoPointsLength(start, end);
                double c = CalculateTwoPointsLength(start, middle);

                // 计算圆心坐标
                double A = middle.X - start.X;
                double B = middle.Y - start.Y;
                double C = end.X - start.X;
                double D = end.Y - start.Y;

                double E = A * (start.X + middle.X) + B * (start.Y + middle.Y);
                double F = C * (start.X + end.X) + D * (start.Y + end.Y);
                double G = 2.0 * (A * (end.Y - middle.Y) - B * (end.X - middle.X));

                if (System.Math.Abs(G) < 1e-10)
                    throw new ArgumentException("三点共线，无法确定圆弧");

                double cx = (D * E - B * F) / G;
                double cy = (A * F - C * E) / G;

                // 半径
                double r = System.Math.Sqrt((start.X - cx) * (start.X - cx) + (start.Y - cy) * (start.Y - cy));

                // 计算夹角
                double angle1 = System.Math.Atan2(start.Y - cy, start.X - cx);
                double angle2 = System.Math.Atan2(middle.Y - cy, middle.X - cx);
                double angle3 = System.Math.Atan2(end.Y - cy, end.X - cx);

                // 计算start->middle->end的夹角（弧度）
                double theta = System.Math.Abs(NormalizeAngle(angle3 - angle1));

                // 判断弧的方向（通过middle是否在start->end的顺时针还是逆时针方向）
                double cross = (middle.X - start.X) * (end.Y - start.Y) - (middle.Y - start.Y) * (end.X - start.X);
                if (cross < 0)
                {
                    // 顺时针，取补角
                    theta = 2 * System.Math.PI - theta;
                }

                // 弧长
                return r * theta;
            }

            /// <summary>
            /// 计算三点圆长度
            /// </summary>
            public static double CalculateCircleLengthByThreePoints(Point2D start, Point2D middle, Point2D end)
            {
                // 步骤：
                // 1. 计算三点确定的圆的圆心和半径
                // 2. 用半径计算圆的周长

                // 计算圆心坐标
                double A = middle.X - start.X;
                double B = middle.Y - start.Y;
                double C = end.X - start.X;
                double D = end.Y - start.Y;
                double E = A * (start.X + middle.X) + B * (start.Y + middle.Y);
                double F = C * (start.X + end.X) + D * (start.Y + end.Y);
                double G = 2.0 * (A * (end.Y - middle.Y) - B * (end.X - middle.X));

                if (System.Math.Abs(G) < 1e-10)
                    throw new ArgumentException("三点共线，无法确定圆");

                double cx = (D * E - B * F) / G;
                double cy = (A * F - C * E) / G;

                // 半径
                double r = System.Math.Sqrt((start.X - cx) * (start.X - cx) + (start.Y - cy) * (start.Y - cy));

                // 圆周长
                return 2 * System.Math.PI * r;
            }

            /// <summary>
            /// 计算一段轨迹的长度
            /// </summary>
            public static double CalculateTrajectoryLength(Point2D StartPoint, LinearMotionTrajectoryItem Trajectory)
            {
                double length = 0;
                switch (Trajectory.ItemType)
                {
                    case LinearMotionTrajectoryItemType.StraightLine:
                        {
                            LinearMotionTrajectoryItemStraightLine? sl = Trajectory.ItemBase as LinearMotionTrajectoryItemStraightLine;
                            if (sl?.EndPoint == null || sl.EndPoint.Position == null || sl.EndPoint.Position.Length < 2)
                            {
                                throw new Exception("sl.EndPoint == null 或长度不足2");
                            }
                            Point2D EndPoint = new Point2D(sl.EndPoint.Position[0].PositionValue, sl.EndPoint.Position[1].PositionValue);
                            length = CalculateTwoPointsLength(StartPoint, EndPoint);
                        }
                        break;

                    case LinearMotionTrajectoryItemType.ArcA:
                        {
                            LinearMotionTrajectoryItemArcA? arcA = Trajectory.ItemBase as LinearMotionTrajectoryItemArcA;
                            if (arcA?.MiddlePoint == null || arcA.MiddlePoint.Position == null || arcA.MiddlePoint.Position.Length < 2)
                            {
                                throw new Exception("arcA.MiddlePoint == null 或长度不足2");
                            }
                            if (arcA?.EndPoint == null || arcA.EndPoint.Position == null || arcA.EndPoint.Position.Length < 2)
                            {
                                throw new Exception("arcA.EndPoint == null 或长度不足2");
                            }
                            Point2D MiddlePoint = new Point2D(arcA.MiddlePoint.Position[0].PositionValue, arcA.MiddlePoint.Position[1].PositionValue);
                            Point2D EndPoint = new Point2D(arcA.EndPoint.Position[0].PositionValue, arcA.EndPoint.Position[1].PositionValue);
                            length = CalculateArcLengthByThreePoints(StartPoint, MiddlePoint, EndPoint);
                        }
                        break;

                    case LinearMotionTrajectoryItemType.CircleA:
                        {
                            LinearMotionTrajectoryItemCircleA? circleA = Trajectory.ItemBase as LinearMotionTrajectoryItemCircleA;
                            if (circleA?.MiddlePoint == null || circleA.MiddlePoint.Position == null || circleA.MiddlePoint.Position.Length < 2)
                            {
                                throw new Exception("arcA.MiddlePoint == null 或长度不足2");
                            }
                            if (circleA?.EndPoint == null || circleA.EndPoint.Position == null || circleA.EndPoint.Position.Length < 2)
                            {
                                throw new Exception("circleA.EndPoint == null 或长度不足2");
                            }
                            Point2D MiddlePoint = new Point2D(circleA.MiddlePoint.Position[0].PositionValue, circleA.MiddlePoint.Position[1].PositionValue);
                            Point2D EndPoint = new Point2D(circleA.EndPoint.Position[0].PositionValue, circleA.EndPoint.Position[1].PositionValue);
                            length = CalculateCircleLengthByThreePoints(StartPoint, MiddlePoint, EndPoint);
                        }
                        break;
                }
                return length;
            }

            /// <summary>
            /// 归一化角度到[0, 2PI]
            /// </summary>
            private static double NormalizeAngle(double angle)
            {
                while (angle < 0) angle += 2 * System.Math.PI;
                while (angle > 2 * System.Math.PI) angle -= 2 * System.Math.PI;
                return angle;
            }

            /// <summary>
            /// 给定一个线段的起始点、结束点，给定指定的点数，将这段线段分割成指定点数的点位
            /// </summary>
            public static Point2D[] SplitLineToPoints(Point2D start, Point2D end, int pointCount)
            {
                if (pointCount < 2)
                    throw new ArgumentException("点数必须大于等于2", nameof(pointCount));

                Point2D[] points = new Point2D[pointCount];
                double dx = (end.X - start.X) / (pointCount - 1);
                double dy = (end.Y - start.Y) / (pointCount - 1);

                for (int i = 0; i < pointCount; i++)
                {
                    points[i] = new Point2D(start.X + dx * i, start.Y + dy * i);
                }
                return points;
            }

            /// <summary>
            /// 给定一个圆弧的起始点、中间点、结束点，给定指定的点数，将这段圆弧分割成指定点数的点位
            /// </summary>
            public static Point2D[] SplitArcToPoints(Point2D start, Point2D middle, Point2D end, int pointCount)
            {
                if (pointCount < 2)
                    throw new ArgumentException("点数必须大于等于2", nameof(pointCount));

                // 计算圆心和半径
                double A = middle.X - start.X;
                double B = middle.Y - start.Y;
                double C = end.X - start.X;
                double D = end.Y - start.Y;
                double E = A * (start.X + middle.X) + B * (start.Y + middle.Y);
                double F = C * (start.X + end.X) + D * (start.Y + end.Y);
                double G = 2.0 * (A * (end.Y - middle.Y) - B * (end.X - middle.X));
                if (System.Math.Abs(G) < 1e-10)
                    throw new ArgumentException("三点共线，无法确定圆弧");

                double cx = (D * E - B * F) / G;
                double cy = (A * F - C * E) / G;
                double r = System.Math.Sqrt((start.X - cx) * (start.X - cx) + (start.Y - cy) * (start.Y - cy));

                // 计算起点、中点、终点的极角
                double angleStart = System.Math.Atan2(start.Y - cy, start.X - cx);
                double angleMiddle = System.Math.Atan2(middle.Y - cy, middle.X - cx);
                double angleEnd = System.Math.Atan2(end.Y - cy, end.X - cx);

                // 计算弧度方向
                double totalAngle = NormalizeAngle(angleEnd - angleStart);
                double cross = (middle.X - start.X) * (end.Y - start.Y) - (middle.Y - start.Y) * (end.X - start.X);
                if (cross < 0)
                {
                    // 顺时针，取补角
                    totalAngle = 2 * System.Math.PI - totalAngle;
                }

                // 分割点
                Point2D[] points = new Point2D[pointCount];
                for (int i = 0; i < pointCount; i++)
                {
                    double t = (double)i / (pointCount - 1);
                    double angle = angleStart + (cross < 0 ? -1 : 1) * totalAngle * t;
                    // 归一化角度
                    angle = NormalizeAngle(angle);
                    points[i] = new Point2D(
                        cx + r * System.Math.Cos(angle),
                        cy + r * System.Math.Sin(angle)
                    );
                }
                return points;
            }

            /// <summary>
            /// 给定一个圆上的三个点，第一个点为起始点、给定指定的点数，将这个圆分割成指定点数的点位
            /// </summary>
            public static Point2D[] SplitCircleToPoints(Point2D start, Point2D middle, Point2D end, int pointCount)
            {
                if (pointCount < 2)
                    throw new ArgumentException("点数必须大于等于2", nameof(pointCount));

                // 计算圆心和半径
                double A = middle.X - start.X;
                double B = middle.Y - start.Y;
                double C = end.X - start.X;
                double D = end.Y - start.Y;
                double E = A * (start.X + middle.X) + B * (start.Y + middle.Y);
                double F = C * (start.X + end.X) + D * (start.Y + end.Y);
                double G = 2.0 * (A * (end.Y - middle.Y) - B * (end.X - middle.X));
                if (System.Math.Abs(G) < 1e-10)
                    throw new ArgumentException("三点共线，无法确定圆");

                double cx = (D * E - B * F) / G;
                double cy = (A * F - C * E) / G;
                double r = System.Math.Sqrt((start.X - cx) * (start.X - cx) + (start.Y - cy) * (start.Y - cy));

                // 计算起点的极角
                double angleStart = System.Math.Atan2(start.Y - cy, start.X - cx);

                // 分割点
                Point2D[] points = new Point2D[pointCount];
                double deltaAngle = 2 * System.Math.PI / pointCount;
                for (int i = 0; i < pointCount; i++)
                {
                    double angle = angleStart + deltaAngle * i;
                    // 归一化角度
                    angle = NormalizeAngle(angle);
                    points[i] = new Point2D(
                        cx + r * System.Math.Cos(angle),
                        cy + r * System.Math.Sin(angle)
                    );
                }
                return points;
            }

            /// <summary>
            /// 将一段轨迹分割成指定数量的点位（均匀间距）
            /// </summary>
            /// <param name="Trajectory">轨迹类型</param>
            /// <param name="start">起始点</param>
            /// <param name="middle">中间点</param>
            /// <param name="end">结束点</param>
            /// <param name="pointCount">点数</param>
            /// <returns>点位列表</returns>
            public static Point2D[] SplitTrajectoryToPoints(LinearMotionTrajectoryItemType Trajectory, Point2D start, Point2D middle, Point2D end, int pointCount)
            {
                switch (Trajectory)
                {
                    case LinearMotionTrajectoryItemType.StraightLine:
                        {
                            return SplitLineToPoints(start, end, pointCount);
                        }
                    case LinearMotionTrajectoryItemType.ArcA:
                        {
                            return SplitArcToPoints(start, middle, end, pointCount);
                        }
                    case LinearMotionTrajectoryItemType.CircleA:
                        {
                            return SplitCircleToPoints(start, middle, end, pointCount);
                        }
                    default:
                        return Array.Empty<Point2D>();
                }
            }

            /// <summary>
            /// 计算2D位置比较点
            /// </summary>
            public static MotionControlPositionComparisonTriggerPoint[] CalculatePositionComparisonTriggerPoints(ProductProcessingTrajectoryItem productProcessingTrajectories, double onePointTime)
            {
                ProcessingTrajectory? processingTrajectory = null;
                List<MotionControlPositionComparisonTriggerPoint> triggerPointsList = new List<MotionControlPositionComparisonTriggerPoint>();
                if (productProcessingTrajectories.IsProcessing)
                    processingTrajectory = (ProcessingTrajectory)productProcessingTrajectories;
                if (processingTrajectory == null || processingTrajectory.MotionTrajectory == null)
                {
                    // throw
                    throw new Exception($"processingTrajectory == null || processingTrajectory.MotionTrajectory == null");
                }
                if (processingTrajectory.MotionTrajectory.TrajectoryType == MotionTrajectoryType.Dot)
                {
                    // throw
                    throw new Exception($"processingTrajectory.MotionTrajectory.TrajectoryType == MotionTrajectoryType.Dot");
                }
                else
                {
                    //处理线轨迹
                    var line = (LinearMotionTrajectory)processingTrajectory.MotionTrajectory;
                    if (line == null || line.StartPoint == null || line.LinearMotionTrajectoryItems == null)
                    {
                        // throw
                        throw new Exception($"line == null || line.StartPoint == null || line.LinearMotionTrajectoryItems == null");
                    }

                    Point2D startPoint = new Point2D(line.StartPoint[0].PositionValue, line.StartPoint[1].PositionValue);

                    for (int i = 0; i < line.LinearMotionTrajectoryItems.Length; i++)
                    {
                        if (line.LinearMotionTrajectoryItems[i].ItemBase == null)
                        {
                            // throw
                            throw new Exception($"line.LinearMotionTrajectoryItems[i].ItemBase == null");
                        }
                        // 计算这段轨迹的总长度
                        double length = CalculateTrajectoryLength(startPoint, line.LinearMotionTrajectoryItems[i]);
                        double speed = 0;
                        var itemBase = line.LinearMotionTrajectoryItems[i].ItemBase;
                        if (itemBase?.InProcessingParameters != null)
                        {
                            speed = itemBase.InProcessingParameters.Value.ProcessingSpeed;
                        }
                        else
                        {
                            throw new Exception("InProcessingParameters 为空，无法获取 ProcessingSpeed");
                        }

                        // 计算这段轨迹运行需要的时间
                        double time = length / speed;
                        // 计算这段轨迹运行时间所对应的触发点数
                        int points = (int)(time / onePointTime);
                        if (points <= 1)
                        {
                            // throw
                            throw new Exception($"points <= 1");
                        }
                        Point2D MiddlePoint = new Point2D();
                        Point2D EndPoint = new Point2D();
                        switch (line.LinearMotionTrajectoryItems[i].ItemType)
                        {
                            case LinearMotionTrajectoryItemType.StraightLine:
                                {
                                    LinearMotionTrajectoryItemStraightLine? sl = line.LinearMotionTrajectoryItems[i].ItemBase as LinearMotionTrajectoryItemStraightLine;
                                    if (sl == null)
                                    {
                                        // throw
                                        return new MotionControlPositionComparisonTriggerPoint[0];
                                    }
                                    if (sl.EndPoint == null || sl.EndPoint.Position == null || sl.EndPoint.Position.Length < 2)
                                    {
                                        throw new Exception("sl.EndPoint == null 或长度不足2");
                                    }
                                    EndPoint = new Point2D(sl.EndPoint.Position[0].PositionValue, sl.EndPoint.Position[1].PositionValue);
                                }
                                break;

                            case LinearMotionTrajectoryItemType.ArcA:
                                {
                                    LinearMotionTrajectoryItemArcA? arcA = line.LinearMotionTrajectoryItems[i].ItemBase as LinearMotionTrajectoryItemArcA;
                                    if (arcA == null)
                                    {
                                        // throw
                                        return new MotionControlPositionComparisonTriggerPoint[0];
                                    }
                                    if (arcA.MiddlePoint == null || arcA.MiddlePoint.Position == null || arcA.MiddlePoint.Position.Length < 2)
                                    {
                                        throw new Exception("arcA.MiddlePoint == null 或长度不足2");
                                    }
                                    if (arcA.EndPoint == null || arcA.EndPoint.Position == null || arcA.EndPoint.Position.Length < 2)
                                    {
                                        throw new Exception("arcA.EndPoint == null 或长度不足2");
                                    }
                                    MiddlePoint = new Point2D(arcA.MiddlePoint.Position[0].PositionValue, arcA.MiddlePoint.Position[1].PositionValue);
                                    EndPoint = new Point2D(arcA.EndPoint.Position[0].PositionValue, arcA.EndPoint.Position[1].PositionValue);
                                }
                                break;

                            //case LinearMotionTrajectoryItemType.ArcB:
                            //    break;
                            case LinearMotionTrajectoryItemType.CircleA:
                                {
                                    LinearMotionTrajectoryItemCircleA? circleA = line.LinearMotionTrajectoryItems[i].ItemBase as LinearMotionTrajectoryItemCircleA;
                                    if (circleA?.MiddlePoint == null || circleA.MiddlePoint.Position == null || circleA.MiddlePoint.Position.Length < 2)
                                    {
                                        // throw
                                        throw new Exception("circleA.MiddlePoint == null 或长度不足2");
                                    }
                                    if (circleA?.EndPoint == null || circleA.EndPoint.Position == null || circleA.EndPoint.Position.Length < 2)
                                    {
                                        // throw
                                        throw new Exception("circleA.EndPoint == null 或长度不足2");
                                    }
                                    MiddlePoint = new Point2D(circleA.MiddlePoint.Position[0].PositionValue, circleA.MiddlePoint.Position[1].PositionValue);
                                    EndPoint = new Point2D(circleA.EndPoint.Position[0].PositionValue, circleA.EndPoint.Position[1].PositionValue);
                                }
                                break;
                        }
                        Point2D[] triggerPoints = SplitTrajectoryToPoints(line.LinearMotionTrajectoryItems[i].ItemType, startPoint, MiddlePoint, EndPoint, points);
                        foreach (var pt in triggerPoints)
                        {
                            triggerPointsList.Add(new MotionControlPositionComparisonTriggerPoint
                            {
                                X = pt.X,
                                Y = pt.Y
                            });
                        }
                        startPoint = EndPoint;
                    }
                    return triggerPointsList.ToArray();
                }
            }
        }

        /// <summary>
        /// 仿射变换计算器
        /// </summary>
        public static class AffineTransformCalculator
        {
            /// <summary>
            /// 最小二乘法拟合仿射变换矩阵
            /// </summary>
            /// <param name="srcPoints">源点位</param>
            /// <param name="dstPoints">目标点位</param>
            /// <returns>仿射变换矩阵</returns>
            /// <exception cref="ArgumentException"></exception>
            public static double[,] CalculateAffineTransform(List<Point2D> srcPoints, List<Point2D> dstPoints)
            {
                // 检查输入点数
                if (srcPoints.Count != dstPoints.Count || srcPoints.Count < 3)
                    throw new ArgumentException("源点位和目标点位数量必须相同且至少为3");
                //throw new GKGException(MotionCalculateErrCodeConsts.AffineTransformPointCountError, MotionCalculateErr.AffineTransformPointCountError, MotionCalculateErr.AffineTransformPointCountError);

                int n = srcPoints.Count;
                double[,] A = new double[2 * n, 6];
                double[] B = new double[2 * n];

                for (int i = 0; i < n; i++)
                {
                    double x = srcPoints[i].X;
                    double y = srcPoints[i].Y;
                    double xPrime = dstPoints[i].X;
                    double yPrime = dstPoints[i].Y;

                    A[2 * i, 0] = x;
                    A[2 * i, 1] = y;
                    A[2 * i, 2] = 1;
                    A[2 * i, 3] = 0;
                    A[2 * i, 4] = 0;
                    A[2 * i, 5] = 0;
                    B[2 * i] = xPrime;

                    A[2 * i + 1, 0] = 0;
                    A[2 * i + 1, 1] = 0;
                    A[2 * i + 1, 2] = 0;
                    A[2 * i + 1, 3] = x;
                    A[2 * i + 1, 4] = y;
                    A[2 * i + 1, 5] = 1;
                    B[2 * i + 1] = yPrime;
                }

                // 正规方程法求解
                double[,] AtA = MatrixTransposeMultiply(A);
                double[] AtB = MatrixTransposeMultiplyVector(A, B);
                double[] X = SolveLinearSystem(AtA, AtB);

                // 构造仿射矩阵
                return new double[,]
                {
            { X[0], X[1], X[2] },
            { X[3], X[4], X[5] },
            { 0,    0,    1    }
                };
            }

            /// <summary>
            /// 矩阵转置乘以自身
            /// </summary>
            /// <param name="A"></param>
            /// <returns></returns>
            private static double[,] MatrixTransposeMultiply(double[,] A)
            {
                int rows = A.GetLength(0);
                int cols = A.GetLength(1);
                double[,] result = new double[cols, cols];

                for (int i = 0; i < cols; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        double sum = 0;
                        for (int k = 0; k < rows; k++)
                        {
                            sum += A[k, i] * A[k, j];
                        }
                        result[i, j] = sum;
                    }
                }
                return result;
            }

            /// <summary>
            /// 矩阵转置乘以向量
            /// </summary>
            /// <param name="A"></param>
            /// <param name="B"></param>
            /// <returns></returns>
            private static double[] MatrixTransposeMultiplyVector(double[,] A, double[] B)
            {
                int rows = A.GetLength(0);
                int cols = A.GetLength(1);
                double[] result = new double[cols];

                for (int i = 0; i < cols; i++)
                {
                    double sum = 0;
                    for (int k = 0; k < rows; k++)
                    {
                        sum += A[k, i] * B[k];
                    }
                    result[i] = sum;
                }
                return result;
            }

            /// <summary>
            /// 高斯消元法求解线性方程组
            /// </summary>
            /// <param name="A"></param>
            /// <param name="B"></param>
            /// <returns></returns>
            private static double[] SolveLinearSystem(double[,] A, double[] B)
            {
                int n = B.Length;
                double[,] aug = new double[n, n + 1];

                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        aug[i, j] = A[i, j];
                    }
                    aug[i, n] = B[i];
                }

                for (int i = 0; i < n; i++)
                {
                    int pivot = i;
                    for (int j = i + 1; j < n; j++)
                    {
                        if (System.Math.Abs(aug[j, i]) > System.Math.Abs(aug[pivot, i]))
                            pivot = j;
                    }
                    // 逐元素交换
                    if (pivot != i)
                    {
                        for (int k = 0; k <= n; k++)
                        {
                            double temp = aug[i, k];
                            aug[i, k] = aug[pivot, k];
                            aug[pivot, k] = temp;
                        }
                    }

                    double div = aug[i, i];
                    for (int j = i; j <= n; j++)
                    {
                        aug[i, j] /= div;
                    }

                    for (int j = i + 1; j < n; j++)
                    {
                        double factor = aug[j, i];
                        for (int k = i; k <= n; k++)
                        {
                            aug[j, k] -= factor * aug[i, k];
                        }
                    }
                }

                double[] X = new double[n];
                for (int i = n - 1; i >= 0; i--)
                {
                    X[i] = aug[i, n];
                    for (int j = i + 1; j < n; j++)
                    {
                        X[i] -= aug[i, j] * X[j];
                    }
                }
                return X;
            }

            /// <summary>
            /// 分解矩阵为平移、旋转、缩放、镜像
            /// </summary>
            /// <param name="matrix">变换矩阵</param>
            /// <param name="tx">平移量X</param>
            /// <param name="ty">平移量Y</param>
            /// <param name="scaleX">缩放比例X</param>
            /// <param name="scaleY">缩放比例Y</param>
            /// <param name="rotation">旋转角度(°)</param>
            /// <param name="mirror">是否镜像</param>
            public static AffineTransformParams DecomposeAffineMatrix(double[,] matrix)
            {
                AffineTransformParams affineTransformParams = new AffineTransformParams();
                affineTransformParams.Tx = matrix[0, 2];
                affineTransformParams.Ty = matrix[1, 2];

                double a = matrix[0, 0];
                double b = matrix[0, 1];
                double d = matrix[1, 0];
                double e = matrix[1, 1];

                affineTransformParams.ScaleX = System.Math.Sqrt(a * a + b * b);
                affineTransformParams.ScaleX = System.Math.Sqrt(d * d + e * e);

                affineTransformParams.RotationDeg = System.Math.Atan2(b, a) * 180 / System.Math.PI;

                affineTransformParams.Mirror = (a * e - b * d) < 0;

                return affineTransformParams;
            }

            /// <summary>
            /// 应用仿射变换
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <param name="tx"></param>
            /// <param name="ty"></param>
            /// <param name="scaleX"></param>
            /// <param name="scaleY"></param>
            /// <param name="rotationDeg"></param>
            /// <param name="mirror"></param>
            /// <returns></returns>
            public static Point2D TransformPoint(Point2D point, AffineTransformParams affineTransformParams)
            {
                return TransformPoint(point, affineTransformParams.Tx, affineTransformParams.Ty, affineTransformParams.ScaleX, affineTransformParams.ScaleY, affineTransformParams.RotationDeg, affineTransformParams.Mirror);
            }

            public static Point2D TransformPoint(Point2D point, double tx, double ty, double scaleX, double scaleY, double rotationDeg, bool mirror)
            {
                double rad = rotationDeg * System.Math.PI / 180.0;

                // 镜像处理
                if (mirror) scaleX = -System.Math.Abs(scaleX);

                double a = scaleX * System.Math.Cos(rad);
                double b = -scaleY * System.Math.Sin(rad);
                double d = scaleX * System.Math.Sin(rad);
                double e = scaleY * System.Math.Cos(rad);

                double xNew = a * point.X + b * point.Y + tx;
                double yNew = d * point.X + e * point.Y + ty;

                return new Point2D(xNew, yNew);
            }

            public static Point3D TransformPoint(Point3D point, AffineTransformParams affineTransformParams)
            {
                var rst = TransformPoint(new Point2D(point.X, point.Y), affineTransformParams.Tx, affineTransformParams.Ty, affineTransformParams.ScaleX, affineTransformParams.ScaleY, affineTransformParams.RotationDeg, affineTransformParams.Mirror);
                return new Point3D(rst.X, rst.Y, point.Z);
            }
        }

        /// <summary>
        /// 相似变换计算器
        /// </summary>
        public static class SimilarityTransformCalculator
        {
            /// <summary>
            /// 计算相似变换参数（缩放、旋转、平移）从两对对应点（相似变换仅需两点足够）
            /// </summary>
            /// <param name="p1">源点1</param>
            /// <param name="q1">目标点1</param>
            /// <param name="p2">源点2</param>
            /// <param name="q2">目标点2</param>
            /// <returns name="scale">缩放比例</returns>
            /// <returns name="rotationDeg">旋转角度(°)</returns>
            /// <returns name="tx">平移量X</returns>
            /// <returns name="ty">平移量Y</returns>
            public static AffineTransformParams
                ComputeFromTwoPoints(
                    Point2D p1, Point2D q1,
                    Point2D p2, Point2D q2)
            {
                // 源点向量
                double dx = p2.X - p1.X;
                double dy = p2.Y - p1.Y;

                // 目标点向量
                double dxPrime = q2.X - q1.X;
                double dyPrime = q2.Y - q1.Y;

                // 缩放比例
                double scale = System.Math.Sqrt(dxPrime * dxPrime + dyPrime * dyPrime) /
                               System.Math.Sqrt(dx * dx + dy * dy);

                // 旋转角度（弧度）
                double theta = System.Math.Atan2(dyPrime, dxPrime) - System.Math.Atan2(dy, dx);

                // 平移量
                double cosTheta = System.Math.Cos(theta);
                double sinTheta = System.Math.Sin(theta);
                double tx = q1.X - scale * cosTheta * p1.X + scale * sinTheta * p1.Y;
                double ty = q1.Y - scale * sinTheta * p1.X - scale * cosTheta * p1.Y;

                return new AffineTransformParams(tx, ty, scale, scale, theta * 180 / System.Math.PI, false);
            }

            /// <summary>
            /// 应用变换到任意点
            /// </summary>
            /// <param name="point">源点位</param>
            /// <param name="scale">缩放比例</param>
            /// <param name="rotationDeg">旋转角度(°)</param>
            /// <param name="tx">平移量X</param>
            /// <param name="ty">平移量Y</param>
            /// <returns>计算结果位置</returns>
            public static Point2D TransformPoint(
                Point2D point,
                double scale, double rotationDeg, double tx, double ty)
            {
                double theta = rotationDeg * System.Math.PI / 180;
                double cosTheta = System.Math.Cos(theta);
                double sinTheta = System.Math.Sin(theta);

                double xPrime = scale * cosTheta * point.X - scale * sinTheta * point.Y + tx;
                double yPrime = scale * sinTheta * point.X + scale * cosTheta * point.Y + ty;

                return new Point2D(xPrime, yPrime);
            }
        }
        /// <summary>
        /// 仿射变换参数结构
        /// </summary>
        public class AffineTransformParams
        {
            public double Tx { get; set; } = 0;
            public double Ty { get; set; } = 0;
            public double ScaleX { get; set; } = 1;
            public double ScaleY { get; set; } = 1;
            public double RotationDeg { get; set; } = 0;
            public bool Mirror { get; set; } = false;

            public AffineTransformParams()
            {
            }

            public AffineTransformParams(double tx, double ty, double scaleX, double scaleY, double rotationDeg, bool mirror)
            {
                Tx = tx;
                Ty = ty;
                ScaleX = scaleX;
                ScaleY = scaleY;
                RotationDeg = rotationDeg;
                Mirror = mirror;
            }
        }
    }
}