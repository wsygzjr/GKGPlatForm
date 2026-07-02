namespace GKG
{
    namespace ElectronicControl
    {
        namespace General
        {
            public static class PressureControlErrCodeConsts
            {
                public const int PressureControlInitFail = -2000;// 气压控制初始化失败
                public const int PressureControlReturnsFail = -2001;// 气压控制返回数据错误
                public const int PressureControlSerialWriteFail = -2002;// 气压串口写入失败
                public const int PressureControlSerialReadFail = -2003;// 气压串口读取失败
                public const int PressureControlSerialInitParamsBad = -2004;// 初始化参数错误
            }
        }
    }
}