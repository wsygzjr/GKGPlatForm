using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotionControl
{
    public class Tests
    {
        /// <summary>
        /// 初始化
        /// </summary>
        public static void Init()
        {
            MotionControlPluginManager.Init();
        }

        public static void Test()
        {
            IMotionControlBase gcMotionControl = MotionControlPluginManager.GetMotionControl(MotionCardType.GC800);
            int supportAxisNum=gcMotionControl.SupportAxisNum;
            IMotionControlCategoryA motionControlCategoryA = (IMotionControlCategoryA)gcMotionControl;
            motionControlCategoryA.OnlineSpeedChange(Guid.Empty,1);
        }

    }
}
