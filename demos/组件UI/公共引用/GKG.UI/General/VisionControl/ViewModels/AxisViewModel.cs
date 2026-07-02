using ReactiveUI;

namespace GKG.UI.General
{
    /// <summary>
    /// 坐标轴配置信息
    /// </summary>
    public class AxisViewModel : ReactiveObject
    {
        /// <summary>
        /// X轴数据模型
        /// </summary>
        public TextBlockViewModel XViewModel { get; }

        /// <summary>
        /// Y轴数据模型
        /// </summary>
        public TextBlockViewModel YViewModel { get; }

        /// <summary>
        /// Z轴数据模型
        /// </summary>
        public TextBlockViewModel ZViewModel { get; }

        /// <summary>
        /// X轴坐标
        /// </summary>
        public decimal X
        {
            get => decimal.Parse(XViewModel.Text);
            set => XViewModel.Text = value.ToString();
        }

        /// <summary>
        /// Y轴坐标
        /// </summary>
        public decimal Y
        {
            get => decimal.Parse(YViewModel.Text);
            set => YViewModel.Text = value.ToString();
        }

        /// <summary>
        /// Z轴坐标
        /// </summary>
        public decimal Z
        {
            get => decimal.Parse(ZViewModel.Text);
            set => ZViewModel.Text = value.ToString();
        }
        /// <summary>
        /// 
        /// </summary>
        public AxisViewModel()
        {
            // 坐标支持正负值
            XViewModel = new TextBlockViewModel();
            YViewModel = new TextBlockViewModel();
            ZViewModel = new TextBlockViewModel();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="axisCfgInfo"></param>
        public void Init(AxisCfgInfo axisCfgInfo)
        {
            this.X = axisCfgInfo.X;
            this.Y = axisCfgInfo.Y;
            this.Z = axisCfgInfo.Z;
        }

        /// <summary>
        /// 提取参数
        /// </summary>
        /// <param name="axisCfgInfo"></param>
        public void Extract(AxisCfgInfo axisCfgInfo)
        {
            axisCfgInfo.X = this.X;
            axisCfgInfo.Y = this.Y;
            axisCfgInfo.Z = this.Z;

        }

        /// <summary>
        /// 更新位置信息
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public void UpdatePosition(decimal x, decimal y, decimal z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z; ;
        }
    }
}
