namespace GKG
{
    namespace Vision
    {
        /// <summary>
        /// 取图像成功事件参数
        /// </summary>
        public sealed class GrabImageSucceededEventArgs : EventArgs
        {
            public byte[] ImageBytes { get; private set; }

            public int CamID { get; private set; }

            public GrabImageSucceededEventArgs(byte[] imageBytes)
                : this(imageBytes, -1)
            {
            }

            public GrabImageSucceededEventArgs(byte[] imageBytes, int camId)
            {
                ImageBytes = imageBytes ?? new byte[0];
                CamID = camId;
            }
        }

        /// <summary>
        /// 前端视觉接口定义
        /// </summary>
        public interface IFrontendVisionService
        {
            event EventHandler<GrabImageSucceededEventArgs> GrabImageSucceeded;
            void Dispose();
        }
    }
}
