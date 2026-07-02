using ReactiveUI;

namespace GriffinsGeneralTestMM
{
    // 消息详情窗口的ViewModel，仅负责数据存储和绑定
    public class FormMessageDetailViewModel : ReactiveObject
    {
        private string _message = string.Empty;

        /// <summary>
        /// 要展示的消息文本（绑定到View的文本控件）
        /// </summary>
        public string Message
        {
            get => _message;
            set => this.RaiseAndSetIfChanged(ref _message, value);   
        }

        /// <summary>
        /// 构造函数：初始化消息文本
        /// </summary>
        /// <param name="message">需要展示的消息内容</param>
        public FormMessageDetailViewModel(string message)
        {
            Message = message;
        }
    } 
}