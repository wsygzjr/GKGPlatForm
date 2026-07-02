using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ReactiveUI;
using System.Reactive;

namespace GKG.UI.General
{
    /// <summary>
    /// 相机操作视图
    /// </summary>
    public partial class CameraOperationView : UserControl
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public CameraOperationView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }

    #region 视图模型
    /// <summary>
    /// 相机操作-视图模型
    /// </summary>
    public class CameraOperationViewModel : ReactiveObject
    {
        /// <summary>
        /// 事件（通知外部数据变更）
        /// </summary>
        public event EventHandler? AfterModified;
        /// <summary>
        /// 页面布局根视图引用（用于UI上下文关联）
        /// </summary>
        private Control? _viewReference;

        private bool _isMoveing; 
        /// <summary>
        /// 移动中状态
        /// </summary>
        public bool IsMoveing
        {
            get => _isMoveing;
            set => this.RaiseAndSetIfChanged(ref _isMoveing, value);
        }
      
        /// <summary>
        /// 图像显示设置配置信息
        /// </summary>
        public CameraShowViewModel CameraShowViewModel { set; get; }
        /// <summary>
        /// XY轴速度配置信息
        /// </summary>
        public AxisSpeedViewModel XYAxisSpeedViewModel { set; get; }
        /// <summary>
        /// Z轴速度配置信息
        /// </summary>
        public AxisSpeedViewModel ZAxisSpeedViewModel { set; get; }
        /// <summary>
        /// UA轴速度配置信息
        /// </summary>
        public AxisSpeedViewModel UAAxisSpeedViewModel { set; get; }

        /// <summary>
        /// 移动
        /// </summary>
        public ReactiveCommand<MoveDirection, Unit> MoveCommand { get; }
        /// <summary>
        /// 传递相机视频控件
        /// </summary>
        /// <param name="cameraShowViewModel"></param>
        public CameraOperationViewModel(CameraShowViewModel cameraShowViewModel)
        {
            CameraShowViewModel = cameraShowViewModel;
            XYAxisSpeedViewModel = new AxisSpeedViewModel();
            ZAxisSpeedViewModel = new AxisSpeedViewModel();
            UAAxisSpeedViewModel = new AxisSpeedViewModel();
            //GKG.UI.General.CameraOperationViewModel.MoveDirection
            MoveCommand = ReactiveCommand.CreateFromTask<MoveDirection, Unit>(onMoveCamera);
            subscribeChildViewModelEvents();
        }

        /// <summary>
        /// 设置视图引用
        /// </summary>
        public void SetViewReference(Control view)
        {
            _viewReference = view;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="cameraOperationCfgInfo">相机操作配置信息</param>
        public void Init(CameraOperationCfgInfo cameraOperationCfgInfo)
        {
            XYAxisSpeedViewModel.Init(cameraOperationCfgInfo.XYAxisSpeedCfgInfo);
            ZAxisSpeedViewModel.Init(cameraOperationCfgInfo.ZAxisSpeedCfgInfo);
            UAAxisSpeedViewModel.Init(cameraOperationCfgInfo.UAAxisSpeedCfgInfo);

        }
        /// <summary>
        /// 提取参数
        /// </summary>
        /// <param name="cameraOperationCfgInfo">相机操作配置信息</param>
        public void Extract(CameraOperationCfgInfo cameraOperationCfgInfo)
        {
            XYAxisSpeedViewModel.Extract(cameraOperationCfgInfo.XYAxisSpeedCfgInfo);
            ZAxisSpeedViewModel.Extract(cameraOperationCfgInfo.ZAxisSpeedCfgInfo);
            UAAxisSpeedViewModel.Extract(cameraOperationCfgInfo.UAAxisSpeedCfgInfo);

        }
        /// <summary>
        /// 订阅子ViewModel的事件
        /// </summary>
        private void subscribeChildViewModelEvents()
        {
            XYAxisSpeedViewModel.AfterModified += viewModel_ValueChanged;
            ZAxisSpeedViewModel.AfterModified += viewModel_ValueChanged;
            UAAxisSpeedViewModel.AfterModified += viewModel_ValueChanged;
        }
      
        /// <summary>
        /// 移动相机
        /// </summary>
        /// <returns></returns>
        private  async Task<Unit> onMoveCamera(MoveDirection moveDirection)
        {
            IsMoveing = true;
            try
            {

                //  模拟移动相机
                //await _moveCameraToPosition(X, Y, Z);
                //AxisViewModel.UpdatePosition(10, 20, 30);
            }
            catch (Exception ex)
            {
                await MessageBox.ShowErrorDialog("失败", $"{ex.Message}", _viewReference);
            }
            finally
            {
                IsMoveing = false;
            }
            return Unit.Default;

        }
        /// <summary>
        /// 界面配置信息改变事件
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="e">事件参数</param>
        private void viewModel_ValueChanged(object? sender, EventArgs e)
        {
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


    /// <summary>
    /// 移动方向枚举
    /// </summary>
    public enum MoveDirection
    {
        /// <summary>
        ///X正向移动
        /// </summary>
        XForward,
        /// <summary>
        ///X正向高速移动
        /// </summary>
        XHighSpeedForward,
        /// <summary>
        ///X负向移动
        /// </summary>
        XNegative,
        /// <summary>
        ///X负向高速移动
        /// </summary>
        XHighSpeedNegative,
        /// <summary>
        ///Y正向移动
        /// </summary>
        YForward,
        /// <summary>
        ///Y正向高速移动
        /// </summary>
        YHighSpeedForward,
        /// <summary>
        ///Y负向移动
        /// </summary>
        YNegative,
        /// <summary>
        ///Y负向高速移动
        /// </summary>
        YHighSpeedNegative,
        /// <summary>
        ///Z正向移动
        /// </summary>
        ZForward,
        /// <summary>
        ///Z正向高速移动
        /// </summary>
        ZHighSpeedForward,
        /// <summary>
        ///Z负向移动
        /// </summary>
        ZNegative,
        /// <summary>
        ///Z负向高速移动
        /// </summary>
        ZHighSpeedNegative,
    }


    #region 子页面视图模型

    /// <summary>
    /// 轴速度配置信息
    /// </summary>
    public class AxisSpeedViewModel : ReactiveObject
    {
        /// <summary>
        /// 事件（通知外部数据变更）
        /// </summary>
        public event EventHandler? AfterModified;
        /// <summary>
        /// 低速数据模型
        /// </summary>
        public NumericViewModel LowSpeedViewModel { get; }

        /// <summary>
        /// 高速数据模型
        /// </summary>
        public NumericViewModel HighSpeedViewModel { get; }

        /// <summary>
        /// XY轴低速（mm/s）
        /// </summary>
        public decimal LowSpeed
        {
            get => LowSpeedViewModel.Value;
            set => LowSpeedViewModel.Value = value;
        }

        /// <summary>
        /// XY轴高速（mm/s）
        /// </summary>
        public decimal HighSpeed
        {
            get => HighSpeedViewModel.Value;
            set => HighSpeedViewModel.Value = value;
        }
        /// <summary>
        /// 
        /// </summary>
        public AxisSpeedViewModel()
        {
            // 速度最小值0，步长0.1mm/s
            LowSpeedViewModel = new NumericViewModel { Minimum = 0, Increment = 0.1m, Value = 10,   DecimalPlaces = 1,   };
            HighSpeedViewModel = new NumericViewModel { Minimum = 0, Increment = 0.1m, Value = 50, DecimalPlaces = 1, };

            subscribeChildViewModelEvents();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="axisSpeedCfgInfo"></param>
        public void Init(AxisSpeedCfgInfo axisSpeedCfgInfo)
        {
            this.HighSpeed = axisSpeedCfgInfo.HighSpeed;
            this.LowSpeed = axisSpeedCfgInfo.LowSpeed;
        }

        /// <summary>
        /// 提取参数
        /// </summary>
        /// <param name="axisSpeedCfgInfo"></param>
        public void Extract(AxisSpeedCfgInfo axisSpeedCfgInfo)
        {
            axisSpeedCfgInfo.LowSpeed = this.LowSpeed;
            axisSpeedCfgInfo.HighSpeed = this.HighSpeed;

        }
        /// <summary>
        /// 订阅子ViewModel的事件
        /// </summary>
        private void subscribeChildViewModelEvents()
        {
            LowSpeedViewModel.ValueChanged += viewModel_ValueChanged;
            HighSpeedViewModel.ValueChanged += viewModel_ValueChanged;
        }

        /// <summary>
        /// 界面配置信息改变事件
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="e">事件参数</param>
        private void viewModel_ValueChanged(object? sender, ValueChangedEventArgs e)
        {
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
   
    #endregion
    #endregion

    #region 相机操作配置信息
    /// <summary>
    /// 相机操作配置信息
    /// </summary>
    public class CameraOperationCfgInfo
    {
        /// <summary>
        /// XY轴速度配置信息
        /// </summary>
        public AxisSpeedCfgInfo XYAxisSpeedCfgInfo { set; get; }
        /// <summary>
        /// Z轴速度配置信息
        /// </summary>
        public AxisSpeedCfgInfo ZAxisSpeedCfgInfo { set; get; }
        /// <summary>
        /// UA轴速度配置信息
        /// </summary>
        public AxisSpeedCfgInfo UAAxisSpeedCfgInfo { set; get; }
        /// <summary>
        /// 构造函数
        /// </summary>
        public CameraOperationCfgInfo()
        {
            XYAxisSpeedCfgInfo = new AxisSpeedCfgInfo();
            ZAxisSpeedCfgInfo = new AxisSpeedCfgInfo();
            UAAxisSpeedCfgInfo = new AxisSpeedCfgInfo();
        }

    }

    /// <summary>
    /// 轴速度配置信息
    /// </summary>
    public class AxisSpeedCfgInfo
    {
        /// <summary>
        /// XY轴低速（mm/s）
        /// </summary>
        public decimal LowSpeed { set; get; }
        /// <summary>
        /// XY轴高速（mm/s）
        /// </summary>
        public decimal HighSpeed { set; get; }
        /// <summary>
        /// 
        /// </summary>
        public AxisSpeedCfgInfo()
        {

        }

    }
    #endregion
}
