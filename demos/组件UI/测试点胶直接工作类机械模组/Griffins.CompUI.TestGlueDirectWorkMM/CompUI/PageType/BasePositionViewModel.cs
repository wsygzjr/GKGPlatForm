using Avalonia.Controls;
using Griffins.UI;
using Griffins.UI.General;
using Newtonsoft.JsonG.Linq;
using ReactiveUI;
using System;
using System.Reactive;
using System.Threading.Tasks;

namespace Griffins.CompUI.TestGlueDirectWorkMM.CompUI.PageType
{
    /// <summary>
    /// 位置信息基类 视图模型
    /// 实现功能头移动到指定位置和坐标教导
    /// </summary>
    public class BasePositionViewModel : ReactiveObject
    {
        /// <summary>
        /// 视图引用（用于显示对话框）
        /// </summary>
        private Control? _viewReference;

        /// <summary>
        /// X轴坐标(mm) 数据模型（decimal类型）
        /// </summary>
        public NumericViewModel XViewModel { get; }

        /// <summary>
        /// Y轴坐标(mm) 数据模型
        /// </summary>
        public NumericViewModel YViewModel { get; }

        /// <summary>
        /// Z轴坐标(mm) 数据模型
        /// </summary>
        public NumericViewModel ZViewModel { get; }

        /// <summary>
        /// 值改变事件
        /// </summary>
        public EventHandler? AfterModified;
        /// <summary>
        /// X轴坐标
        /// </summary>
        public decimal X
        {
            get => XViewModel.Value;
            set
            {
                XViewModel.Value = value;
                this.RaisePropertyChanged(nameof(X));
            }
        }

        /// <summary>
        /// Y轴坐标
        /// </summary>
        public decimal Y
        {
            get => YViewModel.Value;
            set
            {
                YViewModel.Value = value;
                this.RaisePropertyChanged(nameof(Y));
            }
        }

        /// <summary>
        /// Z轴坐标
        /// </summary>
        public decimal Z
        {
            get => ZViewModel.Value;
            set
            {
                ZViewModel.Value = value;
                this.RaisePropertyChanged(nameof(Z));
            }
        }

        private bool _isOping;
        /// <summary>
        /// 操作中状态
        /// </summary>
        public bool IsOping
        {
            get => _isOping;
            set => this.RaiseAndSetIfChanged(ref _isOping, value);
        }

        private string _moveBtText = "相机到"; 
        /// <summary>
        /// 移动按钮显示文本
        /// </summary>
        public string MoveBtText
        {
            get => _moveBtText;
            set => this.RaiseAndSetIfChanged(ref _moveBtText, value);
        }

        /// <summary>
        /// 教导命令
        /// </summary>
        public ReactiveCommand<Unit, Unit> TeachCommand { get; protected set; }
        /// <summary>
        /// 相机到命令
        /// 针头到命令
        /// 激光到命令
        /// ...
        /// </summary>
        public ReactiveCommand<Unit, Unit> MoveCommand { get; protected set; }
        /// <summary>
        /// 
        /// </summary>
        public BasePositionViewModel()
        {
            // 坐标默认支持负数（如机械原点偏移），步长0.001mm
            XViewModel = new NumericViewModel { Increment = 0.001m,   DecimalPlaces = 3, Minimum = 0.000m, Maximum = 50.000m, };
            YViewModel = new NumericViewModel { Increment = 0.001m, DecimalPlaces = 3, Minimum = 0.000m, Maximum = 50.000m, };
            ZViewModel = new NumericViewModel { Increment = 0.001m, DecimalPlaces = 3, Minimum = 0.000m, Maximum = 50.000m, };
           

            var canExecute = this.WhenAnyValue(
                 x => x.IsOping,
                 teaching => !teaching
             );
            TeachCommand = ReactiveCommand.CreateFromTask(onTeach, canExecute);
            MoveCommand = ReactiveCommand.CreateFromTask(onMoveCamera, canExecute);

            // 订阅值变更事件
            subscribeValueChanges();
        }
       

        /// <summary>
        /// 设置视图引用（供对话框使用）
        /// </summary>
        public void SetViewReference(Control view)
        {
            _viewReference = view ?? throw new ArgumentNullException(nameof(view), "视图引用不能为空");
        }
        /// <summary>
        /// 从基类数据模型加载X/Y/Z
        /// </summary>
        public void CopyFrom(BasePositionInfo info)
        {
            if (info == null) return;
            X = info.X;
            Y = info.Y;
            Z = info.Z;
        }

        /// <summary>
        /// 将X/Y/Z回写到基类数据模型
        /// </summary>
        public void CopyTo(BasePositionInfo info)
        {
            if (info == null) return;
            info.X = X;
            info.Y = Y;
            info.Z = Z;
        }

        #region 执行命令

        /// <summary>
        ///教导
        /// </summary>
        private  async Task onTeach()
        {
            IsOping = true;
            try
            {
                var (x, y, z) = await _getCurrentDevicePosition();
                X = x;
                Y = y;
                Z = z;
            }
            catch (Exception ex)
            {
                await MessageBox.ShowErrorDialog("失败", $"{ex.Message}", _viewReference);
            }
            finally
            {
                IsOping = false;
            }
        }

        /// <summary>
        /// 移动相机
        /// </summary>
        /// <returns></returns>
        private  async Task onMoveCamera()
        {
            IsOping = true;
            try
            {
                await _moveCameraToPosition(X, Y, Z);
            }
            catch (Exception ex)
            {
                await MessageBox.ShowErrorDialog("失败", $"{ex.Message}", _viewReference);
            }
            finally
            {
                IsOping = false;
            }
        }

        #endregion

        #region 设备交互基础方法（子类可重写）
        /// <summary>
        /// 获取当前设备位置
        /// </summary>
        protected virtual async Task<(decimal X, decimal Y, decimal Z)> _getCurrentDevicePosition()
        {
            return (0, 0, 0);
        }

        /// <summary>
        /// 移动相机到目标位置
        /// </summary>
        protected virtual async Task _moveCameraToPosition(decimal x, decimal y, decimal z)
        {
        }
        #endregion
        #region 值改变事件
        /// <summary>
        /// 订阅值改变事件
        /// </summary>
        private void subscribeValueChanges()
        {
            XViewModel.ValueChanged += XViewModel_ValueChanged;
            YViewModel.ValueChanged += YViewModel_ValueChanged;
            ZViewModel.ValueChanged += ZViewModel_ValueChanged;
        }

       
        #endregion
        private void XViewModel_ValueChanged(object? sender, ValueChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                X = (decimal)e.NewValue;
            }
            AfterModified?.Invoke(sender, e);
        }
        private void YViewModel_ValueChanged(object? sender, ValueChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                Y = (decimal)e.NewValue;
            }
            AfterModified?.Invoke(sender, e);
        }
        private void ZViewModel_ValueChanged(object? sender, ValueChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                Z = (decimal)e.NewValue;
            }
            AfterModified?.Invoke(sender, e);
        }

    }

    /// <summary>
    ///位置信息基类
    /// </summary>
    public class BasePositionInfo
    {
        /// <summary>
        ///X轴坐标(mm)
        /// </summary>
        public decimal X { set; get; }
        /// <summary>
        ///Y轴坐标(mm)
        /// </summary>
        public decimal Y { set; get; }
        /// <summary>
        ///Z轴坐标(mm)
        /// </summary>
        public decimal Z { set; get; }
        /// <summary>
        /// 
        /// </summary>
        public BasePositionInfo()
        {

        }

        /// <summary>
        /// 从 JObject 反序列化
        /// </summary>
        public void FromJObject(JObject? jObject)
        {
            if (jObject == null) return;
            X = jObject["X"]?.Value<decimal>() ?? 0.0m;
            Y = jObject["Y"]?.Value<decimal>() ?? 0.0m;
            Z = jObject["Z"]?.Value<decimal>() ?? 0.0m;
        }

        /// <summary>
        /// 序列化为 JObject
        /// </summary>
        public JObject ToJObject()
        {
            return new JObject
            {
                { "X", X },
                { "Y", Y },
                { "Z", Z }
            };
        }
    }
}
