using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace GKG
{
    namespace PokaYoke
    {
        public class PokaYokeD5Dispenser : PokaYokeBase
        {
            /// <summary>
            /// 判断轴是否可以移动
            /// </summary>
            /// <param name="logicAxis"></param>
            /// <returns></returns>
            public override bool CheckCanMove(int logicAxis)
            {
                // TODO: Implement the logic to determine if the axis can move
                return true;
            }
        }
    }
}