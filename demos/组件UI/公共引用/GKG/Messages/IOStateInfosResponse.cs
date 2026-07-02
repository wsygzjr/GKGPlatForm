using Griffins.PF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GKG
{
    public class IOStateInfosResponse : MutualInfoResponseBase
    {
        /// <summary>
        /// 状态量IO信息列表
        /// </summary>
        public List<IOStateInformation> IOStateInformations { get; set; } = new List<IOStateInformation>();
    }
}
