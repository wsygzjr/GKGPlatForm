using System;

namespace GKG.ElectronicControl.Dispenser
{
    /// <summary>
    /// 供胶装置类型
    /// </summary>
    public enum GlueDispensingDeviceType
    {
        GKGGlueDispensingDevice = 0,
    }
    public interface IGlueDispensingDeviceBase
    {
        /// <summary>
        /// 剩余胶水时间(h)
        /// </summary>
        double RemainingGlueTime { get; }

        /// <summary>
        /// 剩余胶水重量(mg)
        /// </summary>
        double RemainingGlueWeight { get; }

        /// <summary>
        /// 剩余胶水板数(PCS)
        /// </summary>
        int RemainingGluePcs { get; }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="InitParams"></param>
        void Init(byte[] InitParams);

        /// <summary>
        /// 释放
        /// </summary>
        void Dispose();

        /// <summary>
        /// 设置胶量检测参数
        /// </summary>
        /// <param name="glueAmountParam"></param>
        void SetGlueAmountParams(byte[] glueAmountParam);

        /// <summary>
        /// 刷新胶量
        /// </summary>
        void RefreshGlueAmount();

        /// <summary>
        /// 读是否缺胶
        /// </summary>
        /// <returns></returns>
        bool ReadIsLackOfGlue();

        /// <summary>
        /// 获取胶水气压
        /// </summary>
        /// <returns></returns>
        double GetGlueAirPressure();

        /// <summary>
        /// 设置胶水气压
        /// </summary>
        /// <param name="pressure"></param>
        void SetGlueAirPressure(double pressure);

        /// <summary>
        /// 设备异常事件
        /// </summary>
        event EventHandler<EquipmentExceptionEventArgs> EquipmentException;

        /// <summary>
        /// 胶水液位变化事件
        /// </summary>
        event EventHandler<string> GlueLevelChanged;

        /// <summary>
        /// 胶水剩余时间不足
        /// </summary>
        event EventHandler<EquipmentExceptionEventArgs> LowRemainingGlueTimeAlarm;

        /// <summary>
        /// 胶水剩余重量不足
        /// </summary>
        event EventHandler<EquipmentExceptionEventArgs> LowRemainingGlueWeightAlarm;

        /// <summary>
        /// 胶水剩余板数不足
        /// </summary>
        event EventHandler<EquipmentExceptionEventArgs> LowRemainingGluePcsAlarm;

        /// <summary>
        /// 胶压超限报警
        /// </summary>
        event EventHandler<EquipmentExceptionEventArgs> GluePressureOutOfRangeAlarm;

        /// <summary>
        /// 空胶报警
        /// </summary>
        event EventHandler<EquipmentExceptionEventArgs> GlueEmptyAlarm;
    }

    public static class GlueDispensingDeviceFactory
    {
        public static IGlueDispensingDeviceBase CreateGlueDispensingDevice(GlueDispensingDeviceType glueDispensingDeviceType)
        {
            return glueDispensingDeviceType switch
            {
                GlueDispensingDeviceType.GKGGlueDispensingDevice => new GlueDispensingDeviceGKG(),
                _ => throw new ArgumentException(nameof(glueDispensingDeviceType), new Exception("不支持的供胶装置类型")),
            };
        }
    }
}