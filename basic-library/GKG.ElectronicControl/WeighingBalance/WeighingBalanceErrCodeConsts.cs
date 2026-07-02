namespace GKG
{
    namespace ElectronicControl
    {
        namespace General
        {
            public static class WeighingBalanceErrCodeConsts
            {
                public const int WeighingBalanceWriteCommandFail = -3000;// 发指令失败
                public const int WeighingBalanceReadCommandFail = -3001;// 读指令失败
                public const int WeighingBalanceParseDataFail = -3002;// 解析返回数据错误
            }
        }
    }
}