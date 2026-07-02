using Avalonia;
using Avalonia.Media;
using Griffins.UI2;
using ReactiveUI;
using System;
using System.Reactive;
using System.Reactive.Linq;

namespace GKG.Map.AuxiliaryInfoFuncCtlMapCell.ViewModel
{
    /// <summary>
    /// 辅助信息功能图元视图模型类
    /// 负责管理辅助信息图元的数据和业务逻辑，包括字体、颜色、程序信息、设备信息等属性
    /// </summary>
    public class AuxiliaryInfoViewModel : ReactiveObject
    {
        #region 私有字段
        private readonly AuxiliaryInfoPropertyModelEdit _model;
        #endregion

        #region 响应式属性
        /// <summary>
        /// 字体信息
        /// </summary>
        private FontInfo _textFont;
        public FontInfo TextFont
        {
            get { return _textFont; }
            set { this.RaiseAndSetIfChanged(ref _textFont, value); }
        }

        /// <summary>
        /// 文本颜色
        /// </summary>
        private Color _textColor;
        public Color TextColor
        {
            get { return _textColor; }
            set { this.RaiseAndSetIfChanged(ref _textColor, value); }
        }

        /// <summary>
        /// 背景颜色
        /// </summary>
        private Color _backColor;
        public Color BackColor
        {
            get { return _backColor; }
            set { this.RaiseAndSetIfChanged(ref _backColor, value); }
        }

        /// <summary>
        /// 程序名称
        /// </summary>
        private string _programName;
        public string ProgramName
        {
            get { return _programName; }
            set { this.RaiseAndSetIfChanged(ref _programName, value); }
        }

        /// <summary>
        /// 工单号
        /// </summary>
        private string _workOrderNo;
        public string WorkOrderNo
        {
            get { return _workOrderNo; }
            set { this.RaiseAndSetIfChanged(ref _workOrderNo, value); }
        }

        /// <summary>
        /// 右阀胶水信息
        /// </summary>
        private string _rightValveGlueInfo;
        public string RightValveGlueInfo
        {
            get { return _rightValveGlueInfo; }
            set { this.RaiseAndSetIfChanged(ref _rightValveGlueInfo, value); }
        }

        /// <summary>
        /// 右阀胶水包装ID
        /// </summary>
        private string _rightValveGluePackageId;
        public string RightValveGluePackageId
        {
            get { return _rightValveGluePackageId; }
            set { this.RaiseAndSetIfChanged(ref _rightValveGluePackageId, value); }
        }

        /// <summary>
        /// 右阀胶水生产批次号
        /// </summary>
        private string _rightValveGlueBatchNo;
        public string RightValveGlueBatchNo
        {
            get { return _rightValveGlueBatchNo; }
            set { this.RaiseAndSetIfChanged(ref _rightValveGlueBatchNo, value); }
        }

        /// <summary>
        /// 右阀胶水制造料号
        /// </summary>
        private string _rightValveGlueMaterialNo;
        public string RightValveGlueMaterialNo
        {
            get { return _rightValveGlueMaterialNo; }
            set { this.RaiseAndSetIfChanged(ref _rightValveGlueMaterialNo, value); }
        }

        /// <summary>
        /// 右阀胶水生产日期
        /// </summary>
        private string _rightValveGlueProdDate;
        public string RightValveGlueProdDate
        {
            get { return _rightValveGlueProdDate; }
            set { this.RaiseAndSetIfChanged(ref _rightValveGlueProdDate, value); }
        }

        /// <summary>
        /// 双阀模式标识
        /// </summary>
        private bool _isDualValve;
        public bool IsDualValve
        {
            get { return _isDualValve; }
            set { this.RaiseAndSetIfChanged(ref _isDualValve, value); }
        }

        /// <summary>
        /// 左阀胶水信息
        /// </summary>
        private string _leftValveGlueInfo;
        public string LeftValveGlueInfo
        {
            get { return _leftValveGlueInfo; }
            set { this.RaiseAndSetIfChanged(ref _leftValveGlueInfo, value); }
        }

        /// <summary>
        /// 左阀胶水包装ID
        /// </summary>
        private string _leftValveGluePackageId;
        public string LeftValveGluePackageId
        {
            get { return _leftValveGluePackageId; }
            set { this.RaiseAndSetIfChanged(ref _leftValveGluePackageId, value); }
        }

        /// <summary>
        /// 左阀胶水生产批次号
        /// </summary>
        private string _leftValveGlueBatchNo;
        public string LeftValveGlueBatchNo
        {
            get { return _leftValveGlueBatchNo; }
            set { this.RaiseAndSetIfChanged(ref _leftValveGlueBatchNo, value); }
        }

        /// <summary>
        /// 左阀胶水制造料号
        /// </summary>
        private string _leftValveGlueMaterialNo;
        public string LeftValveGlueMaterialNo
        {
            get { return _leftValveGlueMaterialNo; }
            set { this.RaiseAndSetIfChanged(ref _leftValveGlueMaterialNo, value); }
        }

        /// <summary>
        /// 左阀胶水生产日期
        /// </summary>
        private string _leftValveGlueProdDate;
        public string LeftValveGlueProdDate
        {
            get { return _leftValveGlueProdDate; }
            set { this.RaiseAndSetIfChanged(ref _leftValveGlueProdDate, value); }
        }

        /// <summary>
        /// 设备ID
        /// </summary>
        private string _deviceId;
        public string DeviceId
        {
            get { return _deviceId; }
            set { this.RaiseAndSetIfChanged(ref _deviceId, value); }
        }

        /// <summary>
        /// 设备名称
        /// </summary>
        private string _deviceName;
        public string DeviceName
        {
            get { return _deviceName; }
            set { this.RaiseAndSetIfChanged(ref _deviceName, value); }
        }

        /// <summary>
        /// Mac地址
        /// </summary>
        private string _macAddress;
        public string MacAddress
        {
            get { return _macAddress; }
            set { this.RaiseAndSetIfChanged(ref _macAddress, value); }
        }

        /// <summary>
        /// IP地址
        /// </summary>
        private string _ipAddress;
        public string IpAddress
        {
            get { return _ipAddress; }
            set { this.RaiseAndSetIfChanged(ref _ipAddress, value); }
        }

        /// <summary>
        /// 产品信息
        /// </summary>
        private string _productInfo;
        public string ProductInfo
        {
            get { return _productInfo; }
            set { this.RaiseAndSetIfChanged(ref _productInfo, value); }
        }

        /// <summary>
        /// 胶水名称
        /// </summary>
        private string _glueName;
        public string GlueName
        {
            get { return _glueName; }
            set { this.RaiseAndSetIfChanged(ref _glueName, value); }
        }

        /// <summary>
        /// 压电阀序号
        /// </summary>
        private string _piezoValveSerialNo;
        public string PiezoValveSerialNo
        {
            get { return _piezoValveSerialNo; }
            set { this.RaiseAndSetIfChanged(ref _piezoValveSerialNo, value); }
        }

        /// <summary>
        /// 阀ID
        /// </summary>
        private string _valveId;
        public string ValveId
        {
            get { return _valveId; }
            set { this.RaiseAndSetIfChanged(ref _valveId, value); }
        }
        #endregion

        #region 命令定义
        /// <summary>
        /// 保存参数命令
        /// </summary>
        public ReactiveCommand<Unit, Unit> SaveParamsCommand { get; }
        
        /// <summary>
        /// 验证程序命令
        /// </summary>
        public ReactiveCommand<Unit, Unit> VerifyProgramCommand { get; }
        
        /// <summary>
        /// 上线命令
        /// </summary>
        public ReactiveCommand<Unit, Unit> OnlineCommand { get; }
        
        /// <summary>
        /// 下线命令
        /// </summary>
        public ReactiveCommand<Unit, Unit> OfflineCommand { get; }
        #endregion

        public Interaction<TipDialogViewModel, DialogResultType> ShowTipDialog = new();

        #region 构造函数
        /// <summary>
        /// 初始化辅助信息视图模型
        /// </summary>
        /// <param name="model">数据模型</param>
        /// <param name="offlineAction">下线动作</param>
        public AuxiliaryInfoViewModel(AuxiliaryInfoPropertyModelEdit model)
        {
            _model = model;

            #region 初始化属性值
            TextFont = model.TextFont;
            TextColor = model.TextColor;
            BackColor = model.BackColor;

            ProgramName = model.ProgramName;
            WorkOrderNo = model.WorkOrderNo;
            RightValveGlueInfo = model.RightValveGlueInfo;
            RightValveGluePackageId = model.RightValveGluePackageId;
            RightValveGlueBatchNo = model.RightValveGlueBatchNo;
            RightValveGlueMaterialNo = model.RightValveGlueMaterialNo;
            RightValveGlueProdDate = model.RightValveGlueProdDate;
            IsDualValve = model.IsDualValve;
            LeftValveGlueInfo = model.LeftValveGlueInfo;
            LeftValveGluePackageId = model.LeftValveGluePackageId;
            LeftValveGlueBatchNo = model.LeftValveGlueBatchNo;
            LeftValveGlueMaterialNo = model.LeftValveGlueMaterialNo;
            LeftValveGlueProdDate = model.LeftValveGlueProdDate;
            DeviceId = model.DeviceId;
            DeviceName = model.DeviceName;
            MacAddress = model.MacAddress;
            IpAddress = model.IpAddress;
            ProductInfo = model.ProductInfo;
            GlueName = model.GlueName;
            PiezoValveSerialNo = model.PiezoValveSerialNo;
            ValveId = model.ValveId;
            #endregion

            #region 设置ViewModel到Model的数据同步
            this.WhenAnyValue(x => x.ProgramName).Subscribe(v => { if (_model.ProgramName != v) _model.ProgramName = v; });
            this.WhenAnyValue(x => x.WorkOrderNo).Subscribe(v => { if (_model.WorkOrderNo != v) _model.WorkOrderNo = v; });
            this.WhenAnyValue(x => x.RightValveGlueInfo).Subscribe(v => { if (_model.RightValveGlueInfo != v) _model.RightValveGlueInfo = v; });
            this.WhenAnyValue(x => x.RightValveGluePackageId).Subscribe(v => { if (_model.RightValveGluePackageId != v) _model.RightValveGluePackageId = v; });
            this.WhenAnyValue(x => x.RightValveGlueBatchNo).Subscribe(v => { if (_model.RightValveGlueBatchNo != v) _model.RightValveGlueBatchNo = v; });
            this.WhenAnyValue(x => x.RightValveGlueMaterialNo).Subscribe(v => { if (_model.RightValveGlueMaterialNo != v) _model.RightValveGlueMaterialNo = v; });
            this.WhenAnyValue(x => x.RightValveGlueProdDate).Subscribe(v => { if (_model.RightValveGlueProdDate != v) _model.RightValveGlueProdDate = v; });
            this.WhenAnyValue(x => x.IsDualValve).Subscribe(v => { if (_model.IsDualValve != v) _model.IsDualValve = v; });
            this.WhenAnyValue(x => x.LeftValveGlueInfo).Subscribe(v => { if (_model.LeftValveGlueInfo != v) _model.LeftValveGlueInfo = v; });
            this.WhenAnyValue(x => x.LeftValveGluePackageId).Subscribe(v => { if (_model.LeftValveGluePackageId != v) _model.LeftValveGluePackageId = v; });
            this.WhenAnyValue(x => x.LeftValveGlueBatchNo).Subscribe(v => { if (_model.LeftValveGlueBatchNo != v) _model.LeftValveGlueBatchNo = v; });
            this.WhenAnyValue(x => x.LeftValveGlueMaterialNo).Subscribe(v => { if (_model.LeftValveGlueMaterialNo != v) _model.LeftValveGlueMaterialNo = v; });
            this.WhenAnyValue(x => x.LeftValveGlueProdDate).Subscribe(v => { if (_model.LeftValveGlueProdDate != v) _model.LeftValveGlueProdDate = v; });
            this.WhenAnyValue(x => x.DeviceId).Subscribe(v => { if (_model.DeviceId != v) _model.DeviceId = v; });
            this.WhenAnyValue(x => x.DeviceName).Subscribe(v => { if (_model.DeviceName != v) _model.DeviceName = v; });
            this.WhenAnyValue(x => x.MacAddress).Subscribe(v => { if (_model.MacAddress != v) _model.MacAddress = v; });
            this.WhenAnyValue(x => x.IpAddress).Subscribe(v => { if (_model.IpAddress != v) _model.IpAddress = v; });
            this.WhenAnyValue(x => x.ProductInfo).Subscribe(v => { if (_model.ProductInfo != v) _model.ProductInfo = v; });
            this.WhenAnyValue(x => x.GlueName).Subscribe(v => { if (_model.GlueName != v) _model.GlueName = v; });
            this.WhenAnyValue(x => x.PiezoValveSerialNo).Subscribe(v => { if (_model.PiezoValveSerialNo != v) _model.PiezoValveSerialNo = v; });
            this.WhenAnyValue(x => x.ValveId).Subscribe(v => { if (_model.ValveId != v) _model.ValveId = v; });
            #endregion

            #region 初始化命令
            SaveParamsCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var tipVm = new TipDialogViewModel();
                tipVm.Setup(
                    title: "保存参数",
                    message: "是否修改？",
                    iconType: DialogIconType.Question,
                    comboType: TipButtonCombo.OkCancel);

                var result = await ShowTipDialog.Handle(tipVm);
                if (result != DialogResultType.Ok) return;

                // TODO： 在这里执行保存参数的逻辑，例如调用服务接口等
            });
            VerifyProgramCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                if (string.IsNullOrWhiteSpace(WorkOrderNo))
                {
                    var alarmVm = new TipDialogViewModel();
                    alarmVm.Setup(
                        title: "验证程序",
                        message: "工单号不能为空！",
                        iconType: DialogIconType.Alarm,
                        comboType: TipButtonCombo.Ok);
                    await ShowTipDialog.Handle(alarmVm);
                    return;
                }

                // TODO: 在这里执行保存参数的逻辑，例如调用服务接口等

            });
            OnlineCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var tipVm = new TipDialogViewModel();
                tipVm.Setup(
                    title: "上线",
                    message: "是否上线？",
                    iconType: DialogIconType.Question,
                    comboType: TipButtonCombo.OkCancel);
                var result = await ShowTipDialog.Handle(tipVm);
                if (result != DialogResultType.Ok) return;

                // TODO： 在这里执行上线的逻辑，例如调用服务接口等
            });
            OfflineCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var tipVm = new TipDialogViewModel();
                tipVm.Setup(
                    title: "下线",
                    message: "是否下线？",
                    iconType: DialogIconType.Question,
                    comboType: TipButtonCombo.OkCancel);
                var result = await ShowTipDialog.Handle(tipVm);
                if (result != DialogResultType.Ok) return;
                // TODO： 在这里执行下线的逻辑，例如调用服务接口等
            });
            #endregion

            #region 设置Model到ViewModel的数据同步
            // Sync Model -> ViewModel when property panel changes values
            _model.PropertyChanged += (sender, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(_model.TextFont): TextFont = _model.TextFont; break;
                    case nameof(_model.TextColor): TextColor = _model.TextColor; break;
                    case nameof(_model.BackColor): BackColor = _model.BackColor; break;
                    case nameof(_model.ProgramName): ProgramName = _model.ProgramName; break;
                    case nameof(_model.WorkOrderNo): WorkOrderNo = _model.WorkOrderNo; break;
                    case nameof(_model.RightValveGlueInfo): RightValveGlueInfo = _model.RightValveGlueInfo; break;
                    case nameof(_model.RightValveGluePackageId): RightValveGluePackageId = _model.RightValveGluePackageId; break;
                    case nameof(_model.RightValveGlueBatchNo): RightValveGlueBatchNo = _model.RightValveGlueBatchNo; break;
                    case nameof(_model.RightValveGlueMaterialNo): RightValveGlueMaterialNo = _model.RightValveGlueMaterialNo; break;
                    case nameof(_model.RightValveGlueProdDate): RightValveGlueProdDate = _model.RightValveGlueProdDate; break;
                    case nameof(_model.IsDualValve): IsDualValve = _model.IsDualValve; break;
                    case nameof(_model.LeftValveGlueInfo): LeftValveGlueInfo = _model.LeftValveGlueInfo; break;
                    case nameof(_model.LeftValveGluePackageId): LeftValveGluePackageId = _model.LeftValveGluePackageId; break;
                    case nameof(_model.LeftValveGlueBatchNo): LeftValveGlueBatchNo = _model.LeftValveGlueBatchNo; break;
                    case nameof(_model.LeftValveGlueMaterialNo): LeftValveGlueMaterialNo = _model.LeftValveGlueMaterialNo; break;
                    case nameof(_model.LeftValveGlueProdDate): LeftValveGlueProdDate = _model.LeftValveGlueProdDate; break;
                    case nameof(_model.DeviceId): DeviceId = _model.DeviceId; break;
                    case nameof(_model.DeviceName): DeviceName = _model.DeviceName; break;
                    case nameof(_model.MacAddress): MacAddress = _model.MacAddress; break;
                    case nameof(_model.IpAddress): IpAddress = _model.IpAddress; break;
                    case nameof(_model.ProductInfo): ProductInfo = _model.ProductInfo; break;
                    case nameof(_model.GlueName): GlueName = _model.GlueName; break;
                    case nameof(_model.PiezoValveSerialNo): PiezoValveSerialNo = _model.PiezoValveSerialNo; break;
                    case nameof(_model.ValveId): ValveId = _model.ValveId; break;
                }
            };
            #endregion
        }
        #endregion
    }
}
