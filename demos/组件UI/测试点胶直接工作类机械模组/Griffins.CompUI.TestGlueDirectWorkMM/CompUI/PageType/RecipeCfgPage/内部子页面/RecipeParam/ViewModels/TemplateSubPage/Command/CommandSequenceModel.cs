using Avalonia.Controls;
using DispensingPageType.Views.RecipeParamCfgPage.ParametersAndCalculation;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.TemplateSubPage.Command;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.TemplateSubPage.TrajectorySequence;
using ReactiveUI;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.TemplateSubPage.Command
{
    /// <summary>
    /// 指令序列配置模型
    /// </summary>
    public class CommandSequenceModel : CommandSequenceBaseModel
    {
        #region 私有字段
        /// <summary>
        /// 所属模板ID
        /// </summary>
        private Guid _templateID;

        // 视图实例字段
        private DispensingTrajectoryWorkAreaView? _dispensingTrajectoryWorkAreaView;
        private LiftNeedleWorkAreaView? _liftNeedleWorkAreaView;
        private DelayWorkAreaView? _delayWorkAreaView;
        private CleanWorkAreaView? _cleanWorkAreaView;
        private CoatingWorkAreaView? _coatingWorkAreaView;
        private IOWorkAreaView? _ioWorkAreaView;
        private BadMarkWorkAreaView? _badMarkWorkAreaView;
        private TwoDWorkAreaView? _twoDWorkAreaView;
        private QrCodeWorkAreaView? _qrCodeWorkAreaView;
        private BaseHeightMeasurementWorkAreaView? _baseHeightMeasurementWorkAreaView;
        private ThicknessMeasurementWorkAreaView? _thicknessMeasurementWorkAreaView;
        private SubDispensingWorkAreaView? _subDispensingWorkAreaView;
        private ClampWorkAreaView? _clampWorkAreaView;
        #endregion

        #region 视图模型字段
        /// <summary>
        /// 点胶轨迹序列指令
        /// </summary>
        private DispensingTrajectoryWorkAreaViewModel? _dispensingTrajectoryWorkAreaViewModel;

        /// <summary>
        /// 抬针指令
        /// </summary>
        private LiftNeedleWorkAreaViewModel? _liftNeedleWorkAreaViewModel;

        /// <summary>
        /// 延时指令
        /// </summary>
        public DelayWorkAreaViewModel? _delayWorkAreaViewModel;

        /// <summary>
        /// 清胶指令
        /// </summary>
        private CleanWorkAreaViewModel? CleanWorkAreaViewModel;

        /// <summary>
        /// 涂覆指令
        /// </summary>
        private CoatingWorkAreaViewModel? _coatingWorkAreaViewModel;

        /// <summary>
        /// IO指令
        /// </summary>
        private IOWorkAreaViewModel? _ioWorkAreaViewModel;

        /// <summary>
        /// BadMark设置指令
        /// </summary>
        private BadMarkWorkAreaViewModel? _badMarkWorkAreaViewModel;

        /// <summary>
        /// 2D设置指令
        /// </summary>
        private TwoDWorkAreaViewModel? _twoDWorkAreaViewModel;

        /// <summary>
        /// 扫二维码指令
        /// </summary>
        private QrCodeWorkAreaViewModel? _qrCodeWorkAreaViewModel;

        /// <summary>
        /// 测高度指令
        /// </summary>
        private BaseHeightMeasurementWorkAreaViewModel? _measurementHeightWorkAreaViewModel;
        /// <summary>
        /// 测厚度指令
        /// </summary>
        private ThicknessMeasurementWorkAreaViewModel? _measurementThicknessWorkAreaViewModel;
        /// <summary>
        /// 子点胶指令
        /// </summary>
        private SubDispensingWorkAreaViewModel? _subDispensingWorkAreaViewModel;
        /// <summary>
        /// 抓边指令
        /// </summary>
        private ClampWorkAreaViewModel? _clampWorkAreaViewModel;
        #endregion

        #region 响应式控件ViewModel
        /// <summary>
        /// 工作区的视图实例
        /// </summary>
        private object? _workAreaView = null; 
        public object? WorkAreaView 
        { 
            get=> _workAreaView; 
            set
            {
                _workAreaView = value;
                this.RaisePropertyChanged(nameof(WorkAreaView));
            }
        }

        #endregion

        #region 对外暴露的属性

        #endregion

        #region 事件与构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public CommandSequenceModel()
        {
           
        }
        #endregion

        #region 公共方法

        /// <summary>
        /// 设置所属模板ID
        /// </summary>
        /// <param name="templateID"></param>
        public void SetTemplateID(Guid templateID)
        {
            this._templateID = templateID;
        }
        /// <summary>
        /// 设置视图引用
        /// </summary>
        public new void SetViewReference(Control view)
        {
            base.SetViewReference(view);
        }

        /// <summary>
        /// 从信息对象复制数据
        /// </summary>
        /// <param name="info"></param>
        public  void CopyFrom(CommandSequenceCfgInfo info)
        {
            if (info == null) return;

            base.CopyFrom(info);

            // 根据指令类型加载对应参数
            switch (DispensingCommandType)
            {
                case DispensingCommandType.Clean:
                    if (CleanWorkAreaViewModel == null)
                        throw new Exception("清胶指令视图模型为空");
                    CleanWorkAreaViewModel.CopyFrom((CleanCommandSequence)info.CommandSequence);
                    break;

                case DispensingCommandType.Delay:
                    if (_delayWorkAreaViewModel == null)
                        throw new Exception("延时指令信息视图模型为空");
                    _delayWorkAreaViewModel.CopyFrom((DelayCommandSequence)info.CommandSequence);
                    break;

                case DispensingCommandType.NeedleLift:
                    if (_liftNeedleWorkAreaViewModel == null)
                        throw new Exception("抬针指令视图模型为空");
                    _liftNeedleWorkAreaViewModel.CopyFrom((LiftNeedleCommandSequence)info.CommandSequence);
                    break;

                case DispensingCommandType.BadMark:
                    if (_badMarkWorkAreaViewModel == null)
                        throw new Exception("BadMark指令视图模型为空");
                    _badMarkWorkAreaViewModel.CopyFrom((BadMarkCommandSequence)info.CommandSequence);
                    break;

                case DispensingCommandType.IO:
                    if (_ioWorkAreaViewModel == null)
                        throw new Exception("IO指令视图模型为空");
                    _ioWorkAreaViewModel.CopyFrom((IOCommandSequence)info.CommandSequence);
                    break;

                case DispensingCommandType.Coating:
                    if (_coatingWorkAreaViewModel == null)
                        throw new Exception("涂覆指令视图模型为空");
                    _coatingWorkAreaViewModel.CopyFrom((CoatingCommandSequence)info.CommandSequence);
                    break;

                case DispensingCommandType.TwoD:
                    if (_twoDWorkAreaViewModel == null)
                        throw new Exception("2D设置指令视图模型为空");
                    _twoDWorkAreaViewModel.CopyFrom((TwoDCommandSequence)info.CommandSequence);
                    break;

                case DispensingCommandType.Dispensing:
                    if (_dispensingTrajectoryWorkAreaViewModel == null)
                        throw new Exception("点胶轨迹视图模型为空");
                    _dispensingTrajectoryWorkAreaViewModel.CopyFrom((DispensingCommandSequence)info.CommandSequence);
                    break;

                case DispensingCommandType.QrCode:
                    if (_qrCodeWorkAreaViewModel == null)
                        throw new Exception("二维码扫码视图模型为空");
                    _qrCodeWorkAreaViewModel.CopyFrom((QrCodeCommandSequence)info.CommandSequence);
                    break;

                case DispensingCommandType.MeasurementHeight:
                    if (_measurementHeightWorkAreaViewModel == null)
                        throw new Exception("测高指令视图模型为空");
                    _measurementHeightWorkAreaViewModel.CopyFrom((BaseHeightMeasurementCommandSequence)info.CommandSequence);
                    break;
                case DispensingCommandType.MeasurementThickness:
                    if (_measurementThicknessWorkAreaViewModel == null)
                        throw new Exception("测高测厚指令视图模型为空");
                    _measurementThicknessWorkAreaViewModel.CopyFrom((ThicknessMeasurementCommandSequence)info.CommandSequence);
                    break;
                case DispensingCommandType.SubDispensing:
                    if (_subDispensingWorkAreaViewModel == null)
                        throw new Exception("子点胶指令视图模型为空");
                    _subDispensingWorkAreaViewModel.CopyFrom((SubDispensingCommandSequence)info.CommandSequence);
                    break;
                case DispensingCommandType.Clamp:
                    if (_clampWorkAreaViewModel == null)
                        throw new Exception("抓边指令视图模型为空");
                    _clampWorkAreaViewModel.CopyFrom((ClampCommandSequence)info.CommandSequence);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 复制到信息对象
        /// </summary>
        /// <param name="targetInfo"></param>
        public  void CopyTo(CommandSequenceCfgInfo targetInfo)
        {
            if (targetInfo == null) return;

            base.CopyTo(targetInfo);
           
            // 根据指令类型提取对应参数
            switch (DispensingCommandType)
            {
                case DispensingCommandType.Clean:
                    if (CleanWorkAreaViewModel == null)
                        throw new Exception("清胶指令视图模型为空");
                    var cleanCommand = new CleanCommandSequence();
                    CleanWorkAreaViewModel.CopyTo(cleanCommand);
                    targetInfo.CommandSequence = cleanCommand;
                    break;

                case DispensingCommandType.Delay:
                    if (_delayWorkAreaViewModel == null)
                        throw new Exception("延时指令信息视图模型为空");
                    var delayCommand = new DelayCommandSequence();
                    _delayWorkAreaViewModel.CopyTo(delayCommand);
                    targetInfo.CommandSequence = delayCommand;
                    break;

                case DispensingCommandType.NeedleLift:
                    if (_liftNeedleWorkAreaViewModel == null)
                        throw new Exception("抬针指令视图模型为空");
                    var liftNeedleCommand = new LiftNeedleCommandSequence();
                    _liftNeedleWorkAreaViewModel.CopyTo(liftNeedleCommand);
                    targetInfo.CommandSequence = liftNeedleCommand;
                    break;

                case DispensingCommandType.BadMark:
                    if (_badMarkWorkAreaViewModel == null)
                        throw new Exception("BadMark指令视图模型为空");
                    var badMarkCommand = new BadMarkCommandSequence();
                    _badMarkWorkAreaViewModel.CopyTo(badMarkCommand);
                    targetInfo.CommandSequence = badMarkCommand;
                    break;

                case DispensingCommandType.IO:
                    if (_ioWorkAreaViewModel == null)
                        throw new Exception("IO指令视图模型为空");
                    var ioCommand = new IOCommandSequence();
                    _ioWorkAreaViewModel.CopyTo(ioCommand);
                    targetInfo.CommandSequence = ioCommand;
                    break;

                case DispensingCommandType.Coating:
                    if (_coatingWorkAreaViewModel == null)
                        throw new Exception("涂覆指令视图模型为空");
                    var coatingCommand = new CoatingCommandSequence();
                    _coatingWorkAreaViewModel.CopyTo(coatingCommand);
                    targetInfo.CommandSequence = coatingCommand;
                    break;

                case DispensingCommandType.TwoD:
                    if (_twoDWorkAreaViewModel == null)
                        throw new Exception("2D设置指令视图模型为空");
                    var twoDCommand = new TwoDCommandSequence();
                    _twoDWorkAreaViewModel.CopyTo(twoDCommand);
                    targetInfo.CommandSequence = twoDCommand;
                    break;

                case DispensingCommandType.Dispensing:
                    if (_dispensingTrajectoryWorkAreaViewModel == null)
                        throw new Exception("点胶轨迹视图模型为空");
                    var dispensingCommand = new DispensingCommandSequence();
                    _dispensingTrajectoryWorkAreaViewModel.CopyTo(dispensingCommand);
                    targetInfo.CommandSequence = dispensingCommand;
                    break;

                case DispensingCommandType.QrCode:
                    if (_qrCodeWorkAreaViewModel == null)
                        throw new Exception("二维码扫码视图模型为空");
                    var qrCodeCommand = new QrCodeCommandSequence();
                    _qrCodeWorkAreaViewModel.CopyTo(qrCodeCommand);
                    targetInfo.CommandSequence = qrCodeCommand;
                    break;

                case DispensingCommandType.MeasurementHeight:
                    if (_measurementHeightWorkAreaViewModel == null)
                        throw new Exception("测高指令视图模型为空");
                    var measurementCommand = new BaseHeightMeasurementCommandSequence();
                    _measurementHeightWorkAreaViewModel.CopyTo(measurementCommand);
                    targetInfo.CommandSequence = measurementCommand;
                    break;
                case DispensingCommandType.MeasurementThickness:
                    if (_measurementThicknessWorkAreaViewModel == null)
                        throw new Exception("测厚指令视图模型为空");
                    var measurementCommandThick = new ThicknessMeasurementCommandSequence();
                    _measurementThicknessWorkAreaViewModel.CopyTo(measurementCommandThick);
                    targetInfo.CommandSequence = measurementCommandThick;
                    break;
                case DispensingCommandType.SubDispensing:
                    if (_subDispensingWorkAreaViewModel == null)
                        throw new Exception("子点胶指令视图模型为空");
                    var subdispensingCommand = new SubDispensingCommandSequence();
                    _subDispensingWorkAreaViewModel.CopyTo(subdispensingCommand);
                    targetInfo.CommandSequence = subdispensingCommand;
                    break;
                case DispensingCommandType.Clamp:
                    if (_clampWorkAreaViewModel == null)
                        throw new Exception("抓边指令视图模型为空");
                    var clampCommand = new ClampCommandSequence();
                    _clampWorkAreaViewModel.CopyTo(clampCommand);
                    targetInfo.CommandSequence = clampCommand;
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// 获取轨迹序列信息
        /// </summary>
        /// <returns></returns>
        public List<TrajectorySequenceBaseModel> GetTrajectorySequenceBaseModel()
        {
            List<TrajectorySequenceBaseModel> trajectorySequenceBaseModels = new List<TrajectorySequenceBaseModel>();
            if (_dispensingTrajectoryWorkAreaViewModel == null)
                return trajectorySequenceBaseModels;
           return _dispensingTrajectoryWorkAreaViewModel.ItemsSource.Cast<TrajectorySequenceBaseModel>().ToList();
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 根据指令类型更新相关控件
        /// </summary>
        protected override void updateTrajectoryByDispensingCommandType()
        {
            switch (DispensingCommandType)
            {
                case DispensingCommandType.Clean:
                    CleanWorkAreaViewModel = new CleanWorkAreaViewModel();
                    CleanWorkAreaViewModel.SetViewReference(_viewReference!);
                    CleanWorkAreaViewModel.CopyFrom(new CleanCommandSequence());
                    CleanWorkAreaViewModel.AfterModified += onAfterModified;
                    _cleanWorkAreaView = new CleanWorkAreaView { DataContext = CleanWorkAreaViewModel };
                    WorkAreaView = _cleanWorkAreaView;
                    break;

                case DispensingCommandType.Delay:
                    _delayWorkAreaViewModel = new DelayWorkAreaViewModel();
                    _delayWorkAreaViewModel.SetViewReference(_viewReference!);
                    _delayWorkAreaViewModel.CopyFrom(new DelayCommandSequence());
                    _delayWorkAreaViewModel.AfterModified += onAfterModified;
                    _delayWorkAreaView = new DelayWorkAreaView { DataContext = _delayWorkAreaViewModel };
                    WorkAreaView = _delayWorkAreaView;
                    break;

                case DispensingCommandType.NeedleLift:
                    _liftNeedleWorkAreaViewModel = new LiftNeedleWorkAreaViewModel();
                    _liftNeedleWorkAreaViewModel.SetViewReference(_viewReference!);
                    _liftNeedleWorkAreaViewModel.CopyFrom(new LiftNeedleCommandSequence());
                    _liftNeedleWorkAreaViewModel.AfterModified += onAfterModified;
                    _liftNeedleWorkAreaView = new LiftNeedleWorkAreaView { DataContext = _liftNeedleWorkAreaViewModel };
                    WorkAreaView = _liftNeedleWorkAreaView;
                    break;

                case DispensingCommandType.BadMark:
                    _badMarkWorkAreaViewModel = new BadMarkWorkAreaViewModel();
                    _badMarkWorkAreaViewModel.SetViewReference(_viewReference!);
                    _badMarkWorkAreaViewModel.CopyFrom(new BadMarkCommandSequence());
                    _badMarkWorkAreaViewModel.AfterModified += onAfterModified;
                    _badMarkWorkAreaView = new BadMarkWorkAreaView { DataContext = _badMarkWorkAreaViewModel };
                    WorkAreaView = _badMarkWorkAreaView;
                    break;

                case DispensingCommandType.IO:
                    _ioWorkAreaViewModel = new IOWorkAreaViewModel();
                    _ioWorkAreaViewModel.SetViewReference(_viewReference!);
                    _ioWorkAreaViewModel.CopyFrom(new IOCommandSequence());
                    _ioWorkAreaViewModel.AfterModified += onAfterModified;
                    _ioWorkAreaView = new IOWorkAreaView { DataContext = _ioWorkAreaViewModel };
                    WorkAreaView = _ioWorkAreaView;
                    break;

                case DispensingCommandType.Coating:
                    _coatingWorkAreaViewModel = new CoatingWorkAreaViewModel();
                    _coatingWorkAreaViewModel.SetViewReference(_viewReference!);
                    _coatingWorkAreaViewModel.CopyFrom(new CoatingCommandSequence());
                    _coatingWorkAreaViewModel.AfterModified += onAfterModified;
                    _coatingWorkAreaView = new CoatingWorkAreaView { DataContext = _coatingWorkAreaViewModel };
                    WorkAreaView = _coatingWorkAreaView;
                    break;

                case DispensingCommandType.TwoD:
                    _twoDWorkAreaViewModel = new TwoDWorkAreaViewModel();
                    _twoDWorkAreaViewModel.SetViewReference(_viewReference!);
                    _twoDWorkAreaViewModel.CopyFrom(new TwoDCommandSequence());
                    _twoDWorkAreaViewModel.AfterModified += onAfterModified;
                    _twoDWorkAreaView = new TwoDWorkAreaView { DataContext = _twoDWorkAreaViewModel };
                    WorkAreaView = _twoDWorkAreaView;
                    break;

                case DispensingCommandType.Dispensing:
                    _dispensingTrajectoryWorkAreaViewModel = new DispensingTrajectoryWorkAreaViewModel(_templateID);
                    _dispensingTrajectoryWorkAreaViewModel.SetViewReference(_viewReference!);
                    _dispensingTrajectoryWorkAreaViewModel.CopyFrom(new DispensingCommandSequence());
                    _dispensingTrajectoryWorkAreaViewModel.AfterModified += onAfterModified;
                    _dispensingTrajectoryWorkAreaView = new DispensingTrajectoryWorkAreaView { DataContext = _dispensingTrajectoryWorkAreaViewModel };
                    WorkAreaView = _dispensingTrajectoryWorkAreaView;
                    break;

                case DispensingCommandType.QrCode:
                    _qrCodeWorkAreaViewModel = new QrCodeWorkAreaViewModel();
                    _qrCodeWorkAreaViewModel.SetViewReference(_viewReference!);
                    _qrCodeWorkAreaViewModel.CopyFrom(new QrCodeCommandSequence());
                    _qrCodeWorkAreaViewModel.AfterModified += onAfterModified;
                    _qrCodeWorkAreaView = new QrCodeWorkAreaView { DataContext = _qrCodeWorkAreaViewModel };
                    WorkAreaView = _qrCodeWorkAreaView;
                    break;
                case DispensingCommandType.MeasurementHeight:
                    _measurementHeightWorkAreaViewModel = new BaseHeightMeasurementWorkAreaViewModel();
                    _measurementHeightWorkAreaViewModel.SetViewReference(_viewReference!);
                    _measurementHeightWorkAreaViewModel.CopyFrom(new BaseHeightMeasurementCommandSequence());
                    _measurementHeightWorkAreaViewModel.AfterModified += onAfterModified;
                    _baseHeightMeasurementWorkAreaView = new BaseHeightMeasurementWorkAreaView { DataContext = _measurementHeightWorkAreaViewModel };
                    WorkAreaView = _baseHeightMeasurementWorkAreaView;
                    break;
                case DispensingCommandType.MeasurementThickness:
                    _measurementThicknessWorkAreaViewModel = new ThicknessMeasurementWorkAreaViewModel();
                    _measurementThicknessWorkAreaViewModel.SetViewReference(_viewReference!);
                    _measurementThicknessWorkAreaViewModel.CopyFrom(new ThicknessMeasurementCommandSequence());
                    _measurementThicknessWorkAreaViewModel.AfterModified += onAfterModified;
                    _thicknessMeasurementWorkAreaView = new ThicknessMeasurementWorkAreaView { DataContext = _measurementThicknessWorkAreaViewModel };
                    WorkAreaView = _thicknessMeasurementWorkAreaView;
                    break;
                case DispensingCommandType.SubDispensing:
                    _subDispensingWorkAreaViewModel = new SubDispensingWorkAreaViewModel(_templateID);
                    _subDispensingWorkAreaViewModel.SetViewReference(_viewReference!);
                    _subDispensingWorkAreaViewModel.CopyFrom(new SubDispensingCommandSequence());
                    _subDispensingWorkAreaViewModel.AfterModified += onAfterModified;
                    _subDispensingWorkAreaView = new SubDispensingWorkAreaView { DataContext = _subDispensingWorkAreaViewModel };
                    WorkAreaView = _subDispensingWorkAreaView;
                    break;
                case DispensingCommandType.Clamp:
                    _clampWorkAreaViewModel = new ClampWorkAreaViewModel();
                    _clampWorkAreaViewModel.SetViewReference(_viewReference!);
                    _clampWorkAreaViewModel.CopyFrom(new ClampCommandSequence());
                    _clampWorkAreaView = new ClampWorkAreaView { DataContext = _clampWorkAreaViewModel };
                    WorkAreaView = _clampWorkAreaView;
                    break;
                default:
                    WorkAreaView = null;
                    break;
            }
        }
        #region 值改变事件
        /// <summary>
        /// 值改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onAfterModified(object? sender, EventArgs e)
        {
            AfterModified?.Invoke(sender, e);
        }
        #endregion

        #endregion
    }
}