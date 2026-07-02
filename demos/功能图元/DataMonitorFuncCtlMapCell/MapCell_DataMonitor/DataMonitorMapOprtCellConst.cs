using GF_Gereric;
using Griffins.Map;

namespace GKG.Map.DataMonitorFuncCtlMapCell
{
    public class DataMonitorMapOprtCellConst
    {
        // 基础属性ID
        public const string TextColor_MapOprtCellIDStr = "{325F7154-FE09-45FB-9FF1-9F2FE49C02B3}";
        public static readonly MapOprtCellID TextColor_MapOprtCellID = MapOprtCellID.Parse(TextColor_MapOprtCellIDStr);

        public const string BackColor_MapOprtCellIDStr = "{94AE6D92-E780-4960-9688-A798A0111BC9}";
        public static readonly MapOprtCellID BackColor_MapOprtCellID = MapOprtCellID.Parse(BackColor_MapOprtCellIDStr);

        public const string TextFont_MapOprtCellIDStr = "{7B2BE4AE-2AC8-40CF-B5DB-F0DC98A111EB}";
        public static readonly MapOprtCellID TextFont_MapOprtCellID = MapOprtCellID.Parse(TextFont_MapOprtCellIDStr);

        // 供阀气压栏属性ID
        public const string SupplyValvePressure_MapOprtCellIDStr = "{A1B2C3D4-E5F6-7890-ABCD-EF1234567890}";
        public static readonly MapOprtCellID SupplyValvePressure_MapOprtCellID = MapOprtCellID.Parse(SupplyValvePressure_MapOprtCellIDStr);

        public const string SupplyValvePressureValue_MapOprtCellIDStr = "{B2C3D4E5-F6A7-8901-BCDE-F23456789012}";
        public static readonly MapOprtCellID SupplyValvePressureValue_MapOprtCellID = MapOprtCellID.Parse(SupplyValvePressureValue_MapOprtCellIDStr);

        public const string SupplyValvePressureStatus_MapOprtCellIDStr = "{C3D4E5F6-A7B8-9012-CDEF-345678901ABC}";
        public static readonly MapOprtCellID SupplyValvePressureStatus_MapOprtCellID = MapOprtCellID.Parse(SupplyValvePressureStatus_MapOprtCellIDStr);

        // 供胶气压栏属性ID
        public const string SupplyGluePressure_MapOprtCellIDStr = "{C3D4E5F6-A7B8-9012-CDEF-345678901234}";
        public static readonly MapOprtCellID SupplyGluePressure_MapOprtCellID = MapOprtCellID.Parse(SupplyGluePressure_MapOprtCellIDStr);

        public const string SupplyGluePressureValue_MapOprtCellIDStr = "{D4E5F6A7-B8C9-0123-DEF0-456789012345}";
        public static readonly MapOprtCellID SupplyGluePressureValue_MapOprtCellID = MapOprtCellID.Parse(SupplyGluePressureValue_MapOprtCellIDStr);

        public const string SupplyGluePressureStatus_MapOprtCellIDStr = "{E4F5A6B7-C8D9-0123-DEF1-567890123456}";
        public static readonly MapOprtCellID SupplyGluePressureStatus_MapOprtCellID = MapOprtCellID.Parse(SupplyGluePressureStatus_MapOprtCellIDStr);

        // 喷嘴加热栏属性ID
        public const string NozzleHeating_MapOprtCellIDStr = "{E5F6A7B8-C9D0-1234-EF01-567890123456}";
        public static readonly MapOprtCellID NozzleHeating_MapOprtCellID = MapOprtCellID.Parse(NozzleHeating_MapOprtCellIDStr);

        public const string NozzleHeatingValue_MapOprtCellIDStr = "{F6A7B8C9-D0E1-2345-F012-678901234567}";
        public static readonly MapOprtCellID NozzleHeatingValue_MapOprtCellID = MapOprtCellID.Parse(NozzleHeatingValue_MapOprtCellIDStr);

        public const string NozzleHeatingStatus_MapOprtCellIDStr = "{F7A8B9C0-E1F2-3456-F013-789012345678}";
        public static readonly MapOprtCellID NozzleHeatingStatus_MapOprtCellID = MapOprtCellID.Parse(NozzleHeatingStatus_MapOprtCellIDStr);

        // 监控信息栏属性ID
        public const string SystemStatus_MapOprtCellIDStr = "{A7B8C9D0-E1F2-3456-0123-789012345678}";
        public static readonly MapOprtCellID SystemStatus_MapOprtCellID = MapOprtCellID.Parse(SystemStatus_MapOprtCellIDStr);

        public const string RunningTime_MapOprtCellIDStr = "{B8C9D0E1-F2A3-4567-1234-890123456789}";
        public static readonly MapOprtCellID RunningTime_MapOprtCellID = MapOprtCellID.Parse(RunningTime_MapOprtCellIDStr);

        public const string CycleCount_MapOprtCellIDStr = "{C9D0E1F2-A3B4-5678-2345-901234567890}";
        public static readonly MapOprtCellID CycleCount_MapOprtCellID = MapOprtCellID.Parse(CycleCount_MapOprtCellIDStr);

        public const string Temperature_MapOprtCellIDStr = "{D0E1F2A3-B4C5-6789-3456-012345678901}";
        public static readonly MapOprtCellID Temperature_MapOprtCellID = MapOprtCellID.Parse(Temperature_MapOprtCellIDStr);

        public const string Pressure_MapOprtCellIDStr = "{E1F2A3B4-C5D6-7890-4567-123456789012}";
        public static readonly MapOprtCellID Pressure_MapOprtCellID = MapOprtCellID.Parse(Pressure_MapOprtCellIDStr);

        // 事件ID
        public const string SaveParams_EventIDStr = "{B8C76B13-91E5-4029-B5D7-CBF60AEB1658}";
        public const string VerifyProgram_EventIDStr = "{28B77CF6-29A5-4EB0-B799-770A867F8954}";
        public const string Online_EventIDStr = "{52DDB9A0-8E2A-442F-90CF-06ADDF55BDDD}";
        public const string Offline_EventIDStr = "{817AC0E3-4E12-457A-91B2-AE486054559F}";
    }
}
