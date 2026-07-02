using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GKG.DispenserMachineModules.Source
{
    public static class DispenserErrorCode
    {
        public const int RunMarkHandlerNull = -30001;//mark运行委托未设置
        public const int RunBadMarkHandlerNull = -30002;//mark运行委托未设置
        public const int EdgeEndPointPositionInvalid = -30003;//抓边点位错误，长度小于3    
    }
}
