using System;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.TemplateSubPage.Command
{
    /// <summary>
    /// 点胶指令类型枚举
    /// </summary>
    public enum DispensingCommandType
    {
        /// <summary>
        /// 清洁指令
        /// </summary>
        Clean,

        /// <summary>
        /// 延时指令
        /// </summary>
        Delay,

        /// <summary>
        /// 抬针指令
        /// </summary>
        NeedleLift,

        /// <summary>
        /// 涂覆指令
        /// </summary>
        Coating,

        /// <summary>
        /// 点胶指令
        /// </summary>
        Dispensing,
        /// <summary>
        /// 子点胶指令
        /// </summary>
        SubDispensing,
        /// <summary>
        /// 扫码
        /// </summary>
        QrCode,
        /// <summary>
        /// BadMark设置
        /// </summary>
        BadMark,
        /// <summary>
        /// IO
        /// </summary>
        IO,
        /// <summary>
        /// 2D
        /// </summary>
        TwoD,
        /// <summary>
        /// 测高指令
        /// </summary>
        MeasurementHeight,
        /// <summary>
        /// 测厚指令
        /// </summary>
        MeasurementThickness,
        /// <summary>
        /// 分段指令
        /// </summary>
        Segment,
        /// <summary>
        /// 抓边指令
        /// </summary>
        EdgeGrasping,
        /// <summary>
        /// 开阀指令
        /// </summary>
        ValveOpening,
        /// <summary>
        /// 开阀指令
        /// </summary>
        Clamp,
    }
}