using Griffins.PF;

namespace GKG.Messages
{
    /// <summary>
    /// 响应信息
    /// </summary>
    /// <typeparam name="T">响应内容</typeparam>
    public class ResponseInfo<T> : MutualInfoResponseBase
    {
        public T? Content { get; set; }
    }
}
