using System;

namespace GKG
{
    namespace ElectronicControl
    {
        /// <summary>
        /// 运控接口
        /// </summary>

        /// <summary>
        /// A类运控接口
        /// </summary>
        /// <remarks>
        /// 该接口用于实现A类运动控制的相关功能
        /// </remarks>
        public interface IMotionControlCategoryA : IMotionControlBase
        {
            /// <summary>
            /// 在线变速
            /// </summary>
            /// <param name="axisGuid">轴锁定句柄</param>
            /// <param name="targetSpeed">目标速度</param>
            void OnlineSpeedChange(Guid axisGuid, double targetSpeed);

            /// <summary>
            /// 在线变位
            /// </summary>
            /// <param name="axisGuid">轴锁定句柄</param>
            /// <param name="targetPosition">目标位置</param>
            void OnlineTargetPositionChange(Guid axisGuid, double targetPosition);

            /// <summary>
            /// 设置平面2D补偿参数
            /// </summary>
            /// <param name="axisGuidList">轴锁定句柄列表</param>
            /// <param name="compensationParams">平面2D补偿参数</param>
            void Set2DCompensationParameters(Guid[] axisGuidList, MotionControl2DOffsetParameters compensationParams);

            /// <summary>
            /// 平面2D补偿开关
            /// </summary>
            /// <param name="axisGuidList">轴锁定句柄列表</param>
            /// <param name="isEnabled">是否启用</param>
            void Set2DCompensationEnabled(Guid[] axisGuidList, int CompensationCoordinateSystemId, bool isEnabled);

            /// <summary>
            /// 获取单点平面2D补偿数据
            /// </summary>
            /// <param name="coordinateSystemId">补偿坐标系ID</param>
            /// <param name="point">补偿点位坐标</param>
            /// <returns>2D补偿量</returns>
            Struct2DOffsetParameters Get2DCompensationParameters(int[] axisList, int coordinateSystemId, Point2D point);

            /// <summary>
            /// 获取绝对运动移动指令规划时间
            /// </summary>
            /// <param name="axis">轴号</param>
            /// <returns>int:规划时间(ms)</returns>
            int GetAbsoluteMotionPlanningTime(int axis);

            /// <summary>
            /// 获取绝对运动移动指令规划时间
            /// </summary>
            /// <param name="axisGuid">轴锁定句柄</param>
            /// <returns>int:规划时间(ms)</returns>
            int GetAbsoluteMotionPlanningTime(Guid axisGuid);

            /// <summary>
            /// 设置位置锁存
            /// </summary>
            /// <param name="axisGuid">轴锁定句柄</param>
            /// <param name="positionLatchCaptureLogic">位置锁存捕获逻辑</param>
            /// <param name="channel">位置锁存触发通道号</param>
            /// <param name="positionLatchSignalTriggerMode">位置锁存信号触发模式</param>
            /// <param name="triggerCount">触发次数</param>
            void SetPositionLatch(Guid axisGuid, MotionControlPositionLatchCaptureLogic positionLatchCaptureLogic, int channel, MotionControlPositionLatchSignalTriggerMode positionLatchSignalTriggerMode, short level, int triggerCount);

            /// <summary>
            /// 位置锁存开关
            /// </summary>
            /// <param name="axisGuid">轴锁定句柄</param>
            /// <param name="isEnabled">是否启用</param>
            void SetPositionLatchEnabled(Guid axisGuid, bool isEnabled);

            /// <summary>
            /// 获取位置锁存
            /// </summary>
            /// <param name="axisGuid">轴锁定句柄</param>
            /// <returns>位置锁存结果</returns>
            double[] GetPositionLatchResult(Guid axisGuid);

            /// <summary>
            /// 获取位置锁存
            /// </summary>
            /// <param name="axis">轴号</param>
            /// <returns>位置锁存结果</returns>
            double[] GetPositionLatchResult(int axis);

            /// <summary>
            /// 位置比较输出
            /// </summary>
            /// <param name="coordinateSystemNo">坐标系编号</param>
            /// <param name="axisGuidList">轴锁定句柄列表</param>
            /// <param name="PositionComparisonTriggerPoints">位置比较触发点列表</param>
            /// <param name="motionTrajectoryList">运动轨迹列表</param>
            void PositionComparison2D(int coordinateSystemNo, Guid[] axisGuidList, MotionControlPositionComparisonTriggerPoint[] PositionComparisonTriggerPoints, MotionInstructionBase[] motionTrajectoryList);

            /// <summary>
            /// 连续插补运动
            /// </summary>
            /// <param name="coordinateSystemNo">坐标系编号</param>
            /// <param name="axisGuidList">轴锁定句柄列表</param>
            /// <param name="motionTrajectoryList">运动轨迹列表</param>
            void ContinuousInterpolationMotion(int coordinateSystemNo, Guid[] axisGuidList, MotionInstructionBase[] motionTrajectoryList);

            /// <summary>
            /// 手动位置比较输出
            /// </summary>
            /// <param name="axisGuidList">轴锁定句柄列表</param>
            /// <param name="channel">比较通道号</param>
            /// <param name="startLevel">起始电平信号</param>
            /// <param name="pulseOutputMode">脉冲输出模式</param>
            /// <param name="triggerCount">触发点数</param>
            /// <param name="openTime">开阀时间(ms)</param>
            /// <param name="closeTime">关阀时间(ms)</param>
            /// <remarks>channel参数可以传入多个通道，触发时同时触发，适用于双阀，如果是单阀，则channel只包含一个通道</remarks>
            void ManualPositionComparison(Guid[] axisGuidList, int[] channel, short startLevel, int pulseOutputMode, int triggerCount, double openTime, double closeTime);

            /// <summary>
            /// 停止手动位置比较输出(位置比较模式，电平模式停止手动出胶)
            /// </summary>
            /// <remarks>停止所有通道的触发</remarks>
            void StopManualPositionComparison();
        }

        /*public static class MotionControlCategoryAFactory
        {
            private static IMotionControlCategoryA CreateMotionControl(MotionCardType motionCardType)
            {
                switch (motionCardType)
                {
                    case MotionCardType.GC800:
                        return new MotionControlGaoChAuto();

                    default:
                        throw new NotImplementedException();
                }
            }
        }*/
    }
}