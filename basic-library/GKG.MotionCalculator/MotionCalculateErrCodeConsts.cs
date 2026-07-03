using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GKG.MotionControl
{
    public static class MotionCalculateErrCodeConsts
    {
        public const int MotionCalculateErrParamsLengthIncorrect = -4000;//输入参数长度不正确
        public const int MotionCalculateErrParamsIsNull = -4001;//输入参数为空
        public const int MotionCalculateErrParamsTypeIncorrect = -4002;//输入参数类型不正确
        public const int MotionCalculateErrDelayCloseTooMin = -4003;//提前关阀距离过小
        public const int AffineTransformPointCountError = -4004;//仿射变换点数不足
    }
}
