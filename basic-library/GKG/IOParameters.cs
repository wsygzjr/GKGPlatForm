using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace GKG
{
    /// <summary>
    /// IO状态量初始化参数
    /// </summary>
    public class IOStateInitParameters
    {
        public string deviceID { get; set; }
        public string channelID { get; set; }
        public Guid deviceGuid { get; set; }
    };

    ///<summary>
    ///IO状态量初始化参数列表：IO状态量初始化参数列表
    ///</summary>
    public class IOStateInitParametersList : List<IOStateInitParameters>
    {
    }

    /// <summary>
    /// 感应器循环读取参数
    /// </summary>
    public struct SensorCycleReadInterval
    {
        public int cycleReadNums { get; set; }
        public int cycleReadTimes { get; set; }
    }

    /// <summary>
    /// 通道参数
    /// </summary>
    public class ChannelParameters
    {
        public string channelID { get; set; }
        public EReadWriteMode channelMode { get; set; }
    }

    /// <summary>
    /// 通道参数列表
    /// </summary>
    public class ChannelParametersList : List<ChannelParameters>
    {
        public ChannelParameters Find(string channelID)
        {
            foreach (ChannelParameters p in this)
            {
                if (string.Compare(p.channelID, channelID, true) == 0)
                    return p;
            }
            return null;
        }
    }

    public static class ReadWriteModeConstStr
    {
        public const string ReadOnlyStr = "RO";

        public const string WriteOnlyStr = "WO";

        public const string ReadWriteStr = "RW";
    }
}