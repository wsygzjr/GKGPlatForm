using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GKG
{
    namespace ElectronicControl
    {
        public static class MotionErrCodeConsts
        {
            public const int ERR_MOTION_SUCCESS = 0; // 成功
            public const int ERR_MOTION_AXISLOCK_FAIL = -1; // 轴锁定失败
            public const int ERR_MOTION_MOVE_FAIL = -2; // 运动失败
            public const int ERR_MOTION_POKAYOKE_FAIL = -3; // 防呆检查失败
            public const int ERR_MOTION_AXISBINDING_NOTEXIST = -4; // 轴绑定不存在
            public const int ERR_MOTION_INIT_FAIL = -5; // 运控卡初始化失败
            public const int ERR_MOTION_BADPARAM = -6; // 参数错误
            public const int ERR_MOTION_ABSULOTEMOVE_FAIL = -7; // 绝对运动失败
            public const int ERR_MOTION_RELATIVEMOVE_FAIL = -8; // 相对运动失败
            public const int ERR_MOTION_VELOCITYMOVE_FAIL = -9; // 连续运动失败
            public const int ERR_MOTION_AXISHOME_FAIL = -10; // 回零运动失败
            public const int ERR_MOTION_SETSOFTLIMIT_FAIL = -11; // 设置软限位失败
            public const int ERR_MOTION_INTERPOLATION_FAIL = -12; // 插补运动失败
            public const int ERR_MOTION_ONLINESPEEDCHANGE_FAIL = -13; // 在线变速失败
            public const int ERR_MOTION_ONLINETARGETPOSITIONCHANGE_FAIL = -14; // 在线变位失败
            public const int ERR_MOTION_ROBOT_MOVE_TIMEOUT = -15; // 机械手运动超时
        }
    }
}