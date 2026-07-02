using System;
using System.Collections.Generic;

namespace GKG
{
    namespace SubMM
    {
        /// <summary>
        /// 轨道工作模式枚举
        /// </summary>
        public enum ERailWorkMode
        {
            /// <summary>
            /// 左进右出
            /// </summary>
            LeftInRightOut,

            /// <summary>
            /// 右进左出
            /// </summary>
            RightInLeftOut,

            /// <summary>
            /// 左进左出
            /// </summary>
            LeftInLeftOut,

            /// <summary>
            /// 右进右出
            /// </summary>
            RightInRightOut,
        }

        /// <summary>
        /// 工位电气配置参数结构
        /// </summary>
        public class WorkStationEleConfigParams
        {
            /// <summary>
            /// 是否有左感应器
            /// </summary>
            public bool HasLeftSensor { get; set; }

            /// <summary>
            /// 是否有右感应器
            /// </summary>
            public bool HasRightSensor { get; set; }

            /// <summary>
            /// 是否有接近感应器
            /// </summary>
            public bool HasProximitySensor { get; set; }

            /// <summary>
            /// 是否有左挡板
            /// </summary>
            public bool HasLeftBlock { get; set; }

            /// <summary>
            /// 是否有右挡板
            /// </summary>
            public bool HasRightBlock { get; set; }
        }

        /// <summary>
        /// 工位能力数据结构
        /// </summary>
        public class WorkStationCapability
        {
            /// <summary>
            /// 是否支持左进
            /// </summary>
            public bool IsSupportLeftIn { get; set; }

            /// <summary>
            /// 是否支持左出
            /// </summary>
            public bool IsSupportLeftOut { get; set; }

            /// <summary>
            /// 是否支持右进
            /// </summary>
            public bool IsSupportRightIn { get; set; }

            /// <summary>
            /// 是否支持右出
            /// </summary>
            public bool IsSupportRightOut { get; set; }
        }

        /// <summary>
        /// 工位运输速度档位
        /// </summary>
        public class WorkStationTransSpeedGear
        {
            /// <summary>
            /// 运输速度档位 ID
            /// </summary>
            public int TransSpeedGear { get; set; }

            /// <summary>
            /// 运输加速度
            /// </summary>
            public double TransAcceleration { get; set; }

            /// <summary>
            /// 运输速度
            /// </summary>
            public double TransSpeed { get; set; }
        }

        /// <summary>
        /// 工位运输速度档位列表
        /// </summary>
        public class WorkStationTransSpeedGearList : List<WorkStationTransSpeedGear>
        {
            public WorkStationTransSpeedGear Find(int gearID)
            {
                if (this.Count == 0)
                    throw new Exception();
                var gear = this.Find(g => g.TransSpeedGear == gearID);
                if (gear != null)
                {
                    return gear;
                }
                return this[0]; // 默认返回第一个档位的速度
            }
        }

        /// <summary>
        /// 工位电气初始化参数结构
        /// </summary>
        public class WorkStationEleInitParams
        {
            /// <summary>
            /// 左挡板气缸初始化参数
            /// </summary>
            public CylinderInitParameters LeftBlockCylinderParams { get; set; } = new CylinderInitParameters();

            /// <summary>
            /// 右挡板气缸初始化参数
            /// </summary>
            public CylinderInitParameters RightBlockCylinderParams { get; set; } = new CylinderInitParameters();

            /// <summary>
            /// 左感应器 ID
            /// </summary>
            public Guid LeftSensorID { get; set; } = Guid.Empty;

            /// <summary>
            /// 右感应器 ID
            /// </summary>
            public Guid RightSensorID { get; set; } = Guid.Empty;

            /// <summary>
            /// 接近感应器 ID
            /// </summary>
            public Guid ProximitySensorID { get; set; } = Guid.Empty;
        }

        /// <summary>
        /// 工位运输速度结构
        /// </summary>
        public class WorkStationTransSpeed
        {
            /// <summary>
            /// 运输速度档位 ID
            /// </summary>
            public int TransSpeedGearID { get; set; }
        }
    }
}
