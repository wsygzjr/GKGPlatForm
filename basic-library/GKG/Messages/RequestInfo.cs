using Griffins.PF;

namespace GKG.Messages
{
    /// <summary>
    /// 请求信息
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    public class RequestInfo<T> : MutualInfoBase
    {
        public T? Content { get; set; }
    }
}
