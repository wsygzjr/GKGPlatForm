using GF_Gereric;
using GKG.MotionControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GKG.Maths
{
    /// <summary>
    /// 圆弧方向
    /// </summary>
    public enum ArcDirection
    {
        Clockwise,// 顺时针
        CounterClockwise// 逆时针
    }

    public static partial class GKGMath
    {
        /// <summary>
        /// Point3D转换为AxisConstantValues数组
        /// </summary>
        /// <param name="point3D"></param>
        /// <returns></returns>
        public static AxisConstantValues[] point3DToAxisConstantValues(Point3D point3D)
        {
            return new AxisConstantValues[]
            {
                new AxisConstantValues { Axis = AxisConstants.X, PositionValue = point3D.X },
                new AxisConstantValues { Axis = AxisConstants.Y, PositionValue = point3D.Y },
                new AxisConstantValues { Axis = AxisConstants.Z, PositionValue = point3D.Z }
            };
        }

        /// <summary>
        /// 计算圆弧方向
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="middlePoint"></param>
        /// <param name="endPoint"></param>
        /// <returns></returns>
        public static ArcDirection CalculateArcDirection(Point3D startPoint, Point3D middlePoint, Point3D endPoint)
        {
            Point3D vector1 = new Point3D(middlePoint - startPoint);
            Point3D vector2 = new Point3D(endPoint - middlePoint);
            double crossProductZ = vector1.X * vector2.Y - vector1.Y * vector2.X;

            return crossProductZ > 0 ? ArcDirection.Clockwise : ArcDirection.CounterClockwise;
        }

        /// <summary>
        /// 计算圆弧圆心
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="middlePoint"></param>
        /// <param name="endPoint"></param>
        /// <returns></returns>
        /// <exception cref="GKGException"></exception>
        public static Point3D CalculateArcCenter(Point3D startPoint, Point3D middlePoint, Point3D endPoint)
        {
            double A = startPoint.X - middlePoint.X;
            double B = startPoint.Y - middlePoint.Y;
            double C = endPoint.X - middlePoint.X;
            double D = endPoint.Y - middlePoint.Y;
            double E = A * (startPoint.X + middlePoint.X) + B * (startPoint.Y + middlePoint.Y);
            double F = C * (endPoint.X + middlePoint.X) + D * (endPoint.Y + middlePoint.Y);
            double G = 2 * (A * D - B * C);
            if (G == 0)
            {
                throw new Exception("MotionCalculateErrParamsIsNull");
                //throw new GKGException(MotionCalculateErrCodeConsts.MotionCalculateErrParamsIsNull, MotionCalculateErr.MotionCalculateErrParamsIsNull, MotionCalculateErr.MotionCalculateErrParamsIsNull);
            }
            double centerX = (D * E - B * F) / G;
            double centerY = (A * F - C * E) / G;
            return new Point3D(centerX, centerY, 0);
        }

        /// <summary>
        /// 计算预处理点位
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="middlePoint"></param>
        /// <param name="endPoint"></param>
        /// <param name="advanceDistance"></param>
        /// <returns></returns>
        public static Point3D CalculatePrePoint(Point3D startPoint, Point3D middlePoint, Point3D endPoint, double advanceDistance)
        {
            // 计算圆心
            Point3D centerPoint = CalculateArcCenter(startPoint, middlePoint, endPoint);
            // 根据圆弧计算方向向量（起点到中点到终点的方向）（顺时针/逆时针）
            ArcDirection direction = CalculateArcDirection(startPoint, middlePoint, endPoint);
            // 计算半径
            double radius = Math.Sqrt(Math.Pow(startPoint.X - centerPoint.X, 2) + Math.Pow(startPoint.Y - centerPoint.Y, 2));
            // 计算角度
            double angle = Math.Atan2(startPoint.Y - centerPoint.Y, startPoint.X - centerPoint.X);
            // 预处理点位对应的角度
            double newAngle;
            if (direction == ArcDirection.Clockwise)
            {
                newAngle = angle - advanceDistance / radius;
            }
            else
            {
                newAngle = angle + advanceDistance / radius;
            }
            double preX = centerPoint.X + radius * Math.Cos(newAngle);
            double preY = centerPoint.Y + radius * Math.Sin(newAngle);
            return new Point3D(preX, preY, startPoint.Z);
        }

        /// <summary>
        /// 计算延后位置
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="middlePoint"></param>
        /// <param name="endPoint"></param>
        /// <param name="delayDistance"></param>
        /// <returns></returns>
        public static Point3D CalculatePostPoint(Point3D startPoint, Point3D middlePoint, Point3D endPoint, double delayDistance)
        {
            // 计算圆心
            Point3D centerPoint = CalculateArcCenter(startPoint, middlePoint, endPoint);
            // 根据圆弧计算方向向量（起点到中点到终点的方向）（顺时针/逆时针）
            ArcDirection direction = CalculateArcDirection(startPoint, middlePoint, endPoint);
            // 计算半径
            double radius = Math.Sqrt(Math.Pow(startPoint.X - centerPoint.X, 2) + Math.Pow(startPoint.Y - centerPoint.Y, 2));
            // 计算角度
            double angle = Math.Atan2(endPoint.Y - centerPoint.Y, endPoint.X - centerPoint.X);
            // 预处理点位对应的角度
            double newAngle;
            if (direction == ArcDirection.Clockwise)
            {
                newAngle = angle + delayDistance / radius;
            }
            else
            {
                newAngle = angle - delayDistance / radius;
            }
            double postX = centerPoint.X + radius * Math.Cos(newAngle);
            double postY = centerPoint.Y + radius * Math.Sin(newAngle);
            return new Point3D(postX, postY, endPoint.Z);
        }

        /// <summary>
        /// 计算两点距离
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
        public static double CalculateDistance(Point3D point1, Point3D point2)
        {
            return Math.Sqrt(Math.Pow(point1.X - point2.X, 2) + Math.Pow(point1.Y - point2.Y, 2) + Math.Pow(point1.Z - point2.Z, 2));
        }

        /// <summary>
        /// 计算圆弧长度
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="middlePoint"></param>
        /// <param name="endPoint"></param>
        /// <returns></returns>
        public static double CalculateArcLength(Point3D startPoint, Point3D middlePoint, Point3D endPoint)
        {
            // 计算圆弧的长度
            // 计算圆心
            Point3D centerPoint = CalculateArcCenter(startPoint, middlePoint, endPoint);
            // 根据圆弧计算方向向量（起点到中点到终点的方向）（顺时针/逆时针）
            ArcDirection direction = CalculateArcDirection(startPoint, middlePoint, endPoint);
            // 计算半径
            double radius = Math.Sqrt(Math.Pow(startPoint.X - centerPoint.X, 2) + Math.Pow(startPoint.Y - centerPoint.Y, 2));
            // 计算角度
            double angle = Math.Abs(Math.Atan2(startPoint.Y - centerPoint.Y, startPoint.X - centerPoint.X) - Math.Atan2(endPoint.Y - centerPoint.Y, endPoint.X - centerPoint.X));
            return radius * angle;
        }

        /// <summary>
        /// 计算圆的周长
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="middlePoint"></param>
        /// <param name="endPoint"></param>
        /// <returns></returns>
        public static double CalculateCircleLength(Point3D startPoint, Point3D middlePoint, Point3D endPoint)
        {
            // 计算圆的周长
            // 计算圆心
            Point3D centerPoint = CalculateArcCenter(startPoint, middlePoint, endPoint);
            // 根据圆弧计算方向向量（起点到中点到终点的方向）（顺时针/逆时针）
            ArcDirection direction = CalculateArcDirection(startPoint, middlePoint, endPoint);
            // 计算半径
            double radius = Math.Sqrt(Math.Pow(startPoint.X - centerPoint.X, 2) + Math.Pow(startPoint.Y - centerPoint.Y, 2));
            return radius * Math.PI * 2;
        }
    }
}
