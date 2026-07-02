using Griffins.PF;

namespace GKG
{
    /// <summary>
    /// 响应信息
    /// </summary>
    /// <typeparam name="T">响应内容</typeparam>
    public class AxisInfosResponse : MutualInfoResponseBase
    {
        /// <summary>
        /// 轴信息列表
        /// </summary>
        public List<AxisInformation> AxisInformations { get; set; } = new List<AxisInformation>();
    }
}
