using System;
using System.Collections.Generic;

namespace Griffins.CompUI.BasicRobot.CompUI_BasicRobot.PageType.InitCfgPage.MechanicalArm.Models
{
    /// <summary>
    /// 机械手组件配置（对齐 motion-control 的 MechanicalArmCfgInfo 结构与字段类型）
    /// </summary>
    //public class MechanicalArmCfgInfo
    //{
    //    /// <summary>
    //    /// 点位运动模式
    //    /// </summary>
    //    public MotionMode motionMode;

    //    /// <summary>
    //    /// 分段速度配置列表
    //    /// </summary>
    //    public SegmentationSpeed[] SegmentationSpeedList { get; set; } = [];

    //    /// <summary>
    //    /// 从另一个实例拷贝（浅拷贝）
    //    /// </summary>
    //    public void CopyFrom(MechanicalArmCfgInfo source)
    //    {
    //        if (source == null)
    //        {
    //            return;
    //        }

    //        motionMode = source.motionMode;
    //        SegmentationSpeedList = source.SegmentationSpeedList;
    //    }
    //}

    ///// <summary>
    ///// 点位运动模式（对齐 motion-control 的 MotionMode）
    ///// </summary>
    //public enum MotionMode
    //{
    //    /// <summary>
    //    /// 联动
    //    /// </summary>
    //    UnionControl = 0,

    //    /// <summary>
    //    /// 不联动
    //    /// </summary>
    //    NotUnionControl = 1,
    //}

    ///// <summary>
    ///// 分段速度模式（对齐 motion-control 的 SegmentedSpeedMode）
    ///// </summary>
    //public enum SegmentedSpeedMode
    //{
    //    /// <summary>
    //    /// 高速
    //    /// </summary>
    //    HightSpeed = 0,

    //    /// <summary>
    //    /// 中速
    //    /// </summary>
    //    MiddleSpeed = 1,

    //    /// <summary>
    //    /// 低速
    //    /// </summary>
    //    LowSpeed = 2,
    //}

    ///// <summary>
    ///// 分段速度配置（对齐 motion-control 的 SegmentationSpeed）
    ///// </summary>
    //public class SegmentationSpeed
    //{
    //    /// <summary>
    //    /// 轴号
    //    /// </summary>
    //    public int AxisNo { get; set; }

    //    /// <summary>
    //    /// 分段速度模式
    //    /// </summary>
    //    public SegmentedSpeedMode Mode { get; set; }

    //    /// <summary>
    //    /// 分段速度范围列表
    //    /// </summary>
    //    public SegmentedSpeedRange[] SegmentedSpeedRangeList { get; set; } = [];
    //}

    ///// <summary>
    ///// 分段速度范围（对齐 motion-control 的 SegmentedSpeedRange；字段类型为 double）
    ///// </summary>
    //public struct SegmentedSpeedRange
    //{
    //    public double Range;

    //    public double Speed;

    //    public double Accleration;
    //}
}
