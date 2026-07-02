using ReactiveUI;

namespace GKG.UI.General
{
    /// <summary>
    /// 光源信息
    /// </summary>
    public class LightSourceViewModel : ReactiveObject
    {
        /// <summary>
        /// 事件（通知外部数据变更）
        /// </summary>
        public event EventHandler? AfterModified;
        /// <summary>
        /// R通道数据模型
        /// </summary>
        public SliderNumericViewModel RViewModel { get; }

        /// <summary>
        /// G通道数据模型
        /// </summary>
        public SliderNumericViewModel GViewModel { get; }

        /// <summary>
        /// B通道数据模型
        /// </summary>
        public SliderNumericViewModel BViewModel { get; }

        /// <summary>
        /// D通道数据模型
        /// </summary>
        public SliderNumericViewModel DViewModel { get; }

        /// <summary>
        /// R通道值
        /// </summary>
        public decimal R
        {
            get => RViewModel.Value;
            set => RViewModel.Value = value;
        }

        /// <summary>
        /// G通道值
        /// </summary>
        public decimal G
        {
            get => GViewModel.Value;
            set => GViewModel.Value = value;
        }

        /// <summary>
        /// B通道值
        /// </summary>
        public decimal B
        {
            get => BViewModel.Value;
            set => BViewModel.Value = value;
        }

        /// <summary>
        /// D通道值
        /// </summary>
        public decimal D
        {
            get => DViewModel.Value;
            set => DViewModel.Value = value;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public LightSourceViewModel()
        {
            // 初始化值范围通常为0-255，步长1
            RViewModel = new SliderNumericViewModel { Minimum = 0, Maximum = 255, Increment = 1, Value = 0 };
            GViewModel = new SliderNumericViewModel { Minimum = 0, Maximum = 255, Increment = 1, Value = 0 };
            BViewModel = new SliderNumericViewModel { Minimum = 0, Maximum = 255, Increment = 1, Value = 0 };
            DViewModel = new SliderNumericViewModel { Minimum = 0, Maximum = 255, Increment = 1, Value = 0 };

            subscribeChildViewModelEvents();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lightSourceCfgInfo"></param>
        public void Init(LightSourceCfgInfo lightSourceCfgInfo)
        {
            this.R = lightSourceCfgInfo.R;
            this.G = lightSourceCfgInfo.G;
            this.B = lightSourceCfgInfo.B;
            this.D = lightSourceCfgInfo.D;
        }

        /// <summary>
        /// 提取参数
        /// </summary>
        /// <param name="lightSourceCfgInfo"></param>
        public void Extract(LightSourceCfgInfo lightSourceCfgInfo)
        {
            lightSourceCfgInfo.R = this.R;
            lightSourceCfgInfo.G = this.G;
            lightSourceCfgInfo.B = this.B;
            lightSourceCfgInfo.D = this.D;

        }

        /// <summary>
        /// 订阅子ViewModel的事件
        /// </summary>
        private void subscribeChildViewModelEvents()
        {
            RViewModel.ValueChanged += rViewModel_ValueChanged;
            GViewModel.ValueChanged += gViewModel_ValueChanged;
            BViewModel.ValueChanged += bViewModel_ValueChanged;
            DViewModel.ValueChanged += dViewModel_ValueChanged;
        }

        /// <summary>
        /// R改变事件
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="e">事件参数</param>
        private void rViewModel_ValueChanged(object? sender, ValueChangedEventArgs e)
        {
            if (e.NewValue == null)
                return;
            decimal newValue = (decimal)e.NewValue;

            onAfterModified();
        }
        /// <summary>
        /// R改变事件
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="e">事件参数</param>
        private void gViewModel_ValueChanged(object? sender, ValueChangedEventArgs e)
        {
            if (e.NewValue == null)
                return;
            decimal newValue = (decimal)e.NewValue;
            onAfterModified();
        }
        /// <summary>
        /// B改变事件
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="e">事件参数</param>
        private void bViewModel_ValueChanged(object? sender, ValueChangedEventArgs e)
        {
            if (e.NewValue == null)
                return;
            decimal newValue = (decimal)e.NewValue;
            onAfterModified();
        }
        /// <summary>
        /// D改变事件
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="e">事件参数</param>
        private void dViewModel_ValueChanged(object? sender, ValueChangedEventArgs e)
        {
            if (e.NewValue == null)
                return;
            decimal newValue = (decimal)e.NewValue;
            onAfterModified();
        }
        /// <summary>
        /// 触发配置修改事件
        /// </summary>
        private void onAfterModified()
        {
            AfterModified?.Invoke(this, EventArgs.Empty);
        }
    }
}
