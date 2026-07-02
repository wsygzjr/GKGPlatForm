using Griffins.ImeIOT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GKG
{
    /// <summary>
    /// BadMark识别结果
    /// </summary>
    public class BadMarkResult
    {
        /// <summary>
        /// 是否识别成功
        /// </summary>
        public bool IsOk { get; set; } = false;
    }

    /// <summary>
    /// BadMark识别跳过类型枚举
    /// </summary>
    public enum BadMarkResultType
    {
        /// <summary>
        /// 识别OK跳过
        /// </summary>
        OkSkip,

        /// <summary>
        /// 识别NG跳过
        /// </summary>
        NgSkip
    }

    namespace MM
    {
        /// <summary>
        /// Mark
        /// </summary>
        public partial class MMCmdExecutor : IMMCmdExecutor
        {
            public partial class BadMark : CmdBase
            {
                public override CmdType CmdType => CmdType.BadMark;

                /// <summary>
                /// MarkID
                /// </summary>
                public string MarkID { get; set; } = "";

                /// <summary>
                /// BadMark识别结果跳过类型
                /// </summary>
                public BadMarkResultType ResultType { get; set; } = BadMarkResultType.OkSkip;

                /// <summary>
                /// Mark参数集合
                /// </summary>
                public MarkParam MarkParam { get; set; } = new MarkParam();

                /// <summary>
                /// mark结果
                /// </summary>
                public BadMarkResult Result = new BadMarkResult();

                public override void UpdateCmd(CmdBase cmd)
                {
                    // 识别OK跳过
                    if (ResultType == BadMarkResultType.OkSkip)
                    {
                        if (Result.IsOk == true)
                        {
                            cmd.Enabled = false;
                        }
                    }
                    // 识别NG跳过
                    else if (ResultType == BadMarkResultType.NgSkip)
                    {
                        if (Result.IsOk == false)
                        {
                            cmd.Enabled = false;
                        }
                    }
                }
            }
        }
    }
}