using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GKG
{
    public class GKGException : Exception
    {
        public GKGException(int errCode)
        {
            ErrCode = errCode;
        }

        public GKGException(int errCode, string errCodeStr, string errCodeText) : base(errCodeStr)
        {
            ErrCode = errCode;
            ErrCodeStr = errCodeStr;
            ErrCodeText = errCodeText;
        }

        /// <summary>
        /// 错误码
        /// </summary>
        public int ErrCode { get; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrCodeStr { get; }

        /// <summary>
        /// 错误描述
        /// </summary>
        public string ErrCodeText { get; }
    }
}