namespace GKG
{
    namespace PokaYoke
    {
        /// <summary>
        /// D5机型防呆
        /// </summary>
        public interface IPokaYokeBase
        {
            /// <summary>
            /// 判断轴是否可以移动
            /// </summary>
            /// <param name="logicAxis"></param>
            /// <returns></returns>
            bool CheckCanMove(int logicAxis);

            /// <summary>
            /// 上传轴位置
            /// </summary>
            /// <param name="logicAxis"></param>
            /// <param name="position"></param>
            void UpLoadAxisPosition(int logicAxis, double position);
        }

        /// <summary>
        /// 防呆基类
        /// </summary>
        public class PokaYokeBase : IPokaYokeBase
        {
            protected Dictionary<int, double> axisPositions = new Dictionary<int, double>();
            /// <summary>
            /// 判断轴是否可以移动
            /// </summary>
            /// <param name="logicAxis"></param>
            /// <returns></returns>
            public virtual bool CheckCanMove(int logicAxis)
            {
                // TODO: Implement the logic to determine if the axis can move
                return true;
            }

            /// <summary>
            /// 上传轴位置
            /// </summary>
            /// <param name="logicAxis"></param>
            /// <param name="position"></param>
            public void UpLoadAxisPosition(int logicAxis, double position)
            {
                if(axisPositions.ContainsKey(logicAxis))
                {
                    axisPositions[logicAxis] = position;
                }
                else
                {
                    axisPositions.Add(logicAxis, position);
                }
            }
        }
    }

}
