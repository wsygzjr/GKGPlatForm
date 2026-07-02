namespace GKG
{
    namespace ElectronicControl
    {
        public interface IBaseCommunicate
        {
            public bool IsOpen { get; }

            /// <summary>
            /// 初始化
            /// </summary>
            /// <param name="initCfg"></param>
            void Init(byte[] initCfg);

            /// <summary>
            /// 设置通讯参数
            /// </summary>
            /// <param name="commuParam"></param>
            void SetCommuParam(string commuParam);

            /// <summary>
            /// 打开通讯对象
            /// </summary>
            /// <param name="timeOut"></param>
            /// <returns></returns>
            bool Open(int timeOut);

            /// <summary>
            /// 关闭通讯对象
            /// </summary>
            /// <returns></returns>
            bool Close();

            /// <summary>
            /// 写数据
            /// </summary>
            /// <param name="sendBytes"></param>
            /// <returns></returns>
            bool Write(byte[] sendBytes);

            /// <summary>
            /// 写数据
            /// </summary>
            /// <param name="sendString"></param>
            /// <returns></returns>
            bool Write(string sendString);

            /// <summary>
            /// 读数据
            /// </summary>
            /// <param name="readBytes"></param>
            /// <returns></returns>
            //bool Read(out byte[] readBytes);

            bool ReadTimeout(int timeOut, out byte[] readBytes);

            bool ReadLength(int length, out byte[] readBytes);

            /// <summary>
            /// 清空读缓存
            /// </summary>
            void ClearReadBuffer();
        }
    }
}