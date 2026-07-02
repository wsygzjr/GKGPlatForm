using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GKG
{
    namespace ElectronicControl
    {
        namespace General
        {
            public static class MeasureHeightConsts
            {
                public const string CommendReadSSZNSD33 = "010400640001";
                public const int MAX_GetLaserHErrorCnt = 5;
                public const int MIN_TestLaserValve = -50;
                public const int MAX_TestLaserValve = 50;
            }

            public static class MeasureHeightErrCodeConsts
            {
                public const int MeasureHeightNotInit = -1000;// 未初始化
                public const int MeasureHeightComNotOpen = -1001;// 串口未打开
                public const int MeasureHeightWriteFail = -1002;// 写串口失败
                public const int MeasureHeightReadFail = -1003;// 读串口失败
                public const int MeasureHeightReturnsFail = -1004;// 返回数据错误
                public const int ERR_MEASURE_HEIGHT_READ_FAIL = -1005;// 读高度值失败
            }
        }
    }
}