using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ReactiveUI;

namespace GKG.UI.General
{
    /// <summary>
    /// 
    /// </summary>
    public partial class CalibrationResultView : UserControl
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public CalibrationResultView()
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
    /// 标定结果-视图模型
    /// </summary>
    public class CalibrationResultViewModel : ReactiveObject
    {
        private CalibrationResultInfo _calibrationResultInfo;
        /// <summary>
        /// 相机标定结果
        /// </summary>
        public TextBlockViewModel CameraCalibrationViewModel { get; }

        /// <summary>
        /// 相机VS胶阀结果R0
        /// </summary>
        public TextBlockViewModel CameraVsGluevalveR0ViewModel { get; }
        /// <summary>
        /// 相机VS胶阀结果90度
        /// </summary>
        public TextBlockViewModel CameraVsGluevalveNinetyViewModel { get; }
        /// <summary>
        /// 相机VS激光偏移结果
        /// </summary>
        public TextBlockViewModel CameraVsLaserViewModel { get; }
        /// <summary>
        /// 相机VS胶阀阀高度
        /// </summary>
        public TextBlockViewModel LaserVsGluevalveHightViewModel { get; }
        /// <summary>
        /// 相机比例标定结果字符串
        /// </summary>
        public string CameraRatioCalibrationXYStr
        {
            get => CameraCalibrationViewModel.Text;
            set => CameraCalibrationViewModel.Text = value;
        }

        /// <summary>
        /// 相机VS胶阀结果R0字符串
        /// </summary>
        public string CameraVsGluevalveR0Str
        {
            get => CameraVsGluevalveR0ViewModel.Text;
            set => CameraVsGluevalveR0ViewModel.Text = value;
        }

        /// <summary>
        /// 相机VS胶阀结果90度字符串
        /// </summary>
        public string CameraVsGluevalveNinetyStr
        {
            get => CameraVsGluevalveNinetyViewModel.Text;
            set => CameraVsGluevalveNinetyViewModel.Text = value;
        }
        /// <summary>
        /// 相机VS激光偏移结果字符串
        /// </summary>
        public string CameraVsLaserOffsetStr
        {
            get => CameraVsLaserViewModel.Text;
            set => CameraVsLaserViewModel.Text = value;
        }
        /// <summary>
        /// 激光VS胶阀阀高度字符串
        /// </summary>
        public string LaserVsGluevalveHightStr
        {
            get => LaserVsGluevalveHightViewModel.Text;
            set => LaserVsGluevalveHightViewModel.Text = value;
        }
        /// <summary>
        /// 
        /// </summary>
        public CalibrationResultViewModel()
        {
            CameraCalibrationViewModel = new TextBlockViewModel();
            CameraVsGluevalveR0ViewModel = new TextBlockViewModel();
            CameraVsGluevalveNinetyViewModel = new TextBlockViewModel();
            CameraVsLaserViewModel = new TextBlockViewModel();
            LaserVsGluevalveHightViewModel = new TextBlockViewModel();
            _calibrationResultInfo = new CalibrationResultInfo();
        }

        //public void Init(CalibrationResultInfo calibrationResultInfo)
        //{
        //    this._calibrationResultInfo= calibrationResultInfo;
        //    this.CameraRatioCalibrationXYStr = calibrationResultInfo.CameraRatioOffset.ToResultString();
        //    this.CameraVsGluevalveR0Str = calibrationResultInfo.CameraVsGluevalveResultInfo.R0Offset.ToResultString();
        //    this.CameraVsGluevalveNinetyStr = calibrationResultInfo.CameraVsGluevalveResultInfo.NinetyOffset.ToResultString();
        //    this.CameraVsLaserOffsetStr = calibrationResultInfo.CameraVsLaserResultInfo.ToResultString();
        //    this.LaserVsGluevalveHightStr = calibrationResultInfo.LaserVsGluevalveResultInfo.ToResultString();
        //}
        ///// <summary>
        ///// 提取参数
        ///// </summary>
        ///// <param name="calibrationResultInfo"></param>
        //public void Extract(CalibrationResultInfo calibrationResultInfo)
        //{
        //    calibrationResultInfo.CameraRatioOffset.FromResultString(CameraRatioCalibrationXYStr);
        //    calibrationResultInfo.CameraVsGluevalveResultInfo.R0Offset.FromResultString(CameraVsGluevalveR0Str);
        //    calibrationResultInfo.CameraVsGluevalveResultInfo.NinetyOffset.FromResultString(CameraVsGluevalveNinetyStr);
        //    calibrationResultInfo.CameraVsLaserResultInfo.FromResultString(CameraVsLaserOffsetStr);
        //    calibrationResultInfo.LaserVsGluevalveResultInfo.FromResultString(LaserVsGluevalveHightStr);
        //}
        /// <summary>
        /// 设置标定结果
        /// </summary>
        /// <param name="e"></param>
        public void  SetCalibrationResultInfo(CalibrationResultInfoChangedEventArgs e)
        {
            switch (e.CalibrationResultKind)
            {
                //相机VS胶阀R0偏移
                case CalibrationResultKind.CameraVsGluevalveR0Offset:
                    var r0Offset_Camera = (OffsetResultInfo)e.CalibrationResultInfo;
                    _calibrationResultInfo.CameraVsGluevalveResultInfo.R0Offset.X = r0Offset_Camera.X;
                    _calibrationResultInfo.CameraVsGluevalveResultInfo.R0Offset.Y = r0Offset_Camera.Y;
                    this.CameraVsGluevalveNinetyStr = _calibrationResultInfo.CameraVsGluevalveResultInfo.R0Offset.ToResultString();
                    break;

                //相机VS胶阀90度偏移
                case CalibrationResultKind.CameraVsGluevalveNinetyOffset:
                    var ninetyOffset_Camera = (OffsetResultInfo)e.CalibrationResultInfo;
                    _calibrationResultInfo.CameraVsGluevalveResultInfo.NinetyOffset.X = ninetyOffset_Camera.X;
                    _calibrationResultInfo.CameraVsGluevalveResultInfo.NinetyOffset.Y = ninetyOffset_Camera.Y;
                    this.CameraVsGluevalveR0Str = _calibrationResultInfo.CameraVsGluevalveResultInfo.NinetyOffset.ToResultString();
                    break;

                //相机V激光
                case CalibrationResultKind.CameraVsLaser:
                    var laser_Laser = (OffsetResultInfo)e.CalibrationResultInfo;
                    _calibrationResultInfo.CameraVsLaserResultInfo.X = laser_Laser.X;
                    _calibrationResultInfo.CameraVsLaserResultInfo.Y = laser_Laser.Y;
                    this.CameraVsGluevalveR0Str = _calibrationResultInfo.CameraVsLaserResultInfo.ToResultString();
                    break;

                //激光V胶阀
                case CalibrationResultKind.LaserVsGluevalve:
                    var gluevalve_Laser = (LaserVsGluevalveResultInfo)e.CalibrationResultInfo;
                    _calibrationResultInfo.LaserVsGluevalveResultInfo.LaserToPlaneValue = gluevalve_Laser.LaserToPlaneValue;
                    _calibrationResultInfo.LaserVsGluevalveResultInfo.ValveZAxisPosition = gluevalve_Laser.ValveZAxisPosition;
                    this.CameraVsLaserOffsetStr = _calibrationResultInfo.LaserVsGluevalveResultInfo.ToResultString();
                    break;

                //相机比例标定
                case CalibrationResultKind.CameraRatio:
                    var cameraRatioResultInfo = (CameraRatioResultInfo)e.CalibrationResultInfo;
                    _calibrationResultInfo.CameraRatioOffset = cameraRatioResultInfo;
                    this.CameraRatioCalibrationXYStr = _calibrationResultInfo.CameraRatioOffset.ToResultString();
                    break;

                default:
                    break;
            }
        }
    }

    #endregion

    /// <summary>
    /// 标定结果信息
    /// </summary>
    public class CalibrationResultInfo 
    {
        /// <summary>
        /// 相机比例标定结果
        /// </summary>
        public CameraRatioResultInfo CameraRatioOffset { get; set; }
        /// <summary>
        /// 相机VS胶阀结果
        /// </summary>
        public CameraVsGluevalveResultInfo CameraVsGluevalveResultInfo { get; set; }

        /// <summary>
        /// 相机VS激光偏移结果
        /// </summary>
        public OffsetResultInfo CameraVsLaserResultInfo { get; set; }

        /// <summary>
        /// 激光VS胶阀标定结果偏移信息
        /// </summary>
        public LaserVsGluevalveResultInfo LaserVsGluevalveResultInfo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public CalibrationResultInfo()
        {
            CameraRatioOffset = new CameraRatioResultInfo();
            CameraVsGluevalveResultInfo = new CameraVsGluevalveResultInfo();
            CameraVsLaserResultInfo = new OffsetResultInfo();
            LaserVsGluevalveResultInfo = new LaserVsGluevalveResultInfo();
        }

    }

    /// <summary>
    /// 相机比列定结果信息
    /// </summary>
    public class CameraRatioResultInfo
    {
        /// <summary>
        /// X偏移
        /// </summary>
        public decimal XOffset { get; set; }
        /// <summary>
        /// Y偏移
        /// </summary>
        public decimal YOffset { get; set; }
        /// <summary>
        /// 结果转字符串
        /// </summary>
        /// <returns></returns>
        public string ToResultString()
        {
            return $"{this.XOffset}|{this.YOffset}";
        }
        /// <summary>
        /// 字符串转结果信息
        /// </summary>
        /// <param name="str"></param>
        public void FromResultString(string str)
        {
            var tempStrs = str.Split('|');
            this.XOffset = decimal.Parse(tempStrs[0]);
            this.YOffset = decimal.Parse(tempStrs[1]);

        }
        /// <summary>
        /// 
        /// </summary>
        public CameraRatioResultInfo()
        {
        }

    }
    /// <summary>
    /// 相机VS胶阀标定结果信息
    /// </summary>
    public class CameraVsGluevalveResultInfo
    {
        /// <summary>
        /// 相机VS胶阀R0偏移
        /// </summary>
        public OffsetResultInfo R0Offset { get; set; }
        /// <summary>
        /// 相机VS胶阀90度偏移
        /// </summary>
        public OffsetResultInfo NinetyOffset { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public CameraVsGluevalveResultInfo()
        {
            R0Offset = new OffsetResultInfo();
            NinetyOffset = new OffsetResultInfo();
        }

    }
    /// <summary>
    /// 偏移标定结果偏移信息
    /// </summary>
    public class OffsetResultInfo
    {
        /// <summary>
        /// X坐标
        /// </summary>
        public decimal X { get; set; }
        /// <summary>
        /// Y坐标
        /// </summary>
        public decimal Y { get; set; }
        /// <summary>
        /// 偏移标定结果偏移信息
        /// </summary>
        public OffsetResultInfo()
        {
        }
        /// <summary>
        /// 结果转字符串
        /// </summary>
        /// <returns></returns>
        public string ToResultString()
        {
            return $"{this.X}|{this.Y}";
        }
        /// <summary>
        /// 字符串转结果信息
        /// </summary>
        /// <param name="str"></param>
        public void FromResultString(string str)
        {
            var tempStrs = str.Split('|');
            this.X = decimal.Parse(tempStrs[0]);
            this.Y = decimal.Parse(tempStrs[1]);
        }
    }
    /// <summary>
    /// 激光VS胶阀标定结果偏移信息
    /// </summary>
    public class LaserVsGluevalveResultInfo
    {
        /// <summary>
        /// 激光到平面的数值
        /// </summary>
        public decimal LaserToPlaneValue { get; set; }
        /// <summary>
        /// 阀碰到平面的Z轴位置
        /// </summary>
        public decimal ValveZAxisPosition { get; set; }
        /// <summary>
        /// 构造函数
        /// </summary>
        public LaserVsGluevalveResultInfo()
        {
        }
        /// <summary>
        /// 结果转字符串
        /// </summary>
        /// <returns></returns>
        public string ToResultString()
        {
            return $"{this.LaserToPlaneValue}|{this.ValveZAxisPosition}";
        }
        /// <summary>
        /// 字符串转结果信息
        /// </summary>
        /// <param name="str"></param>
        public void FromResultString(string str)
        {
            var tempStrs = str.Split('|');
            this.LaserToPlaneValue = decimal.Parse(tempStrs[0]);
            this.ValveZAxisPosition = decimal.Parse(tempStrs[1]);
        }
    }
    /// <summary>
    /// 相机标定结果信息
    /// </summary>
    public class CalibrationResultInfoChangedEventArgs : EventArgs
    {
        /// <summary>
        /// 标定结果类型
        /// </summary>
        public CalibrationResultKind CalibrationResultKind { get; }

        /// <summary>
        /// 标定结果信息,根据标定结果类型区分不同的结构信息
        /// </summary>
        public object CalibrationResultInfo { get; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="calibrationResultKind"></param>
        /// <param name="resultInfo"></param>
        public CalibrationResultInfoChangedEventArgs(CalibrationResultKind calibrationResultKind, object resultInfo)
        {
            CalibrationResultKind = calibrationResultKind;
            CalibrationResultInfo = resultInfo;
        }
    }
    /// <summary>
    /// 标定结果类型
    /// </summary>
    public enum CalibrationResultKind
    {
        /// <summary>
        /// 相机比例标定
        /// </summary>
        CameraRatio,
        /// <summary>
        /// 相机VS胶阀R0偏移
        /// </summary>
        CameraVsGluevalveR0Offset,
        /// <summary>
        /// 相机VS胶阀90度偏移
        /// </summary>
        CameraVsGluevalveNinetyOffset,
        /// <summary>
        /// 相机V激光
        /// </summary>
        CameraVsLaser,
        /// <summary>
        /// 激光V胶阀-改变
        /// </summary>
        LaserVsGluevalve,
    }
}
