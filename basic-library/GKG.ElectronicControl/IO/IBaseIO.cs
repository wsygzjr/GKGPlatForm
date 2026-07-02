using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GKG
{
    namespace ElectronicControl
    {
        public interface IBaseIO

        {
            /// <summary>
            /// 初始化
            /// </summary>
            /// <param name="initCfg">初始化参数</param>
            void InitIOCard(string initCfg);

            /// <summary>
            /// 反初始化
            /// </summary>
            void UnInitIOCard();

            /// <summary>
            /// 模拟量读取
            /// </summary>
            /// <param name="channelID"></param>
            /// <returns></returns>
            Decimal AnalogRead(string channelID);

            /// <summary>
            /// 写模拟量
            /// </summary>
            /// <param name="channelID">通道号</param>
            /// <param name="analog">模拟量值</param>
            void AnalogWrite(string channelID, decimal analog);

            /// <summary>
            /// 读状态量
            /// </summary>
            /// <param name="channelID">通道号</param>
            /// <returns></returns>
            bool StateRead(string channelID);

            /// <summary>
            /// 写状态量
            /// </summary>
            /// <param name="channelID"></param>
            /// <param name="state"></param>
            void StateWrite(string channelID, bool state);

            ChannelParametersList AnalogChannelList { get; }

            ChannelParametersList StateChannelList { get; }

            EReadWriteMode ReadWriteMode { get; }
        }

        public interface  IBaseStateIO
        {
            /// <summary>
            /// 初始化
            /// </summary>
            /// <param name="initCfg"></param>
            void Init(byte[] initCfg);

            /// <summary>
            /// 读取状态量
            /// </summary>
            /// <returns></returns>
            bool Read();

            /// <summary>
            /// 写入状态量
            /// </summary>
            /// <param name="state"></param>
            void Write(bool state);

            bool CheckValue(bool state, int outTime = 100);

            void SetDeviceInstance(IBaseIO baseIO);
        }
    }
}
