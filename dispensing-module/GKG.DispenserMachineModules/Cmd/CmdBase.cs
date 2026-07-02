using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GKG
{
    public enum CmdType
    {
        Mark,
        BadMark,
        Edge,
        Path,
        Dot
    }

    public class CmdBase
    {
        /// <summary>
        /// 指令类型
        /// </summary>
        public virtual CmdType CmdType { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// 描述信息
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 运行接口
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public virtual void Run()
        {
            throw new NotImplementedException();
        }
        public virtual void UpdateCmd(CmdBase cmd)
        {

        }
    }
}