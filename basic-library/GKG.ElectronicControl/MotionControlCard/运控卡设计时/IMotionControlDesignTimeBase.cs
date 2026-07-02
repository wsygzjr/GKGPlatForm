using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GKG.ElectronicControl
{
    public interface IMotionControlDesignTimeBase
    {
        /// <summary>
        /// 创建卡参数编辑接口实例
        /// </summary>
        /// <returns>运控卡参数编辑接口实例</returns>
        IMotionCard CreateMotionCard();

        /// <summary>
        /// 创建轴参数编辑接口实例
        /// </summary>
        /// <returns>轴参数编辑接口实例</returns>
        IMotionAxis CreateMotionAxis();
    }

    public interface IMotionCard 
    {
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns></returns>
        byte[] GetData();

        void SetData(byte[] data);

        void Edit(object owner);
    }

    public interface IMotionAxis
    {
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns></returns>
        byte[] GetData();

        void SetData(byte[] data);

        void Edit(object owner);
    }
}
