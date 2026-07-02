using System;
using System.Reactive;
using System.Reactive.Disposables;
using Avalonia;
using Avalonia.Media;
using Griffins.UI2;
using ReactiveUI;

namespace GKG.Map.CommunicationFuncCtlMapCell.ViewModel
{
    public class CommunicationViewModel : ReactiveObject, IDisposable
    {
        #region 私有字段
        private readonly CommunicationPropertyModelEdit _communicationPropertyModelEdit;
        private readonly CompositeDisposable _disposables = new();
        private Action _communication;
        #endregion

        #region 响应式属性
        private FontInfo _fontInfo;
        public FontInfo TextFont
        {
            get { return _fontInfo; }
            set
            {
                this.RaiseAndSetIfChanged(ref _fontInfo, value);
            }
        }

        private Color _textColor;
        public Color TextColor
        {
            get { return _textColor; }
            set
            {
                this.RaiseAndSetIfChanged(ref _textColor, value);
            }
        }

        private Color _backColor;
        public Color BackColor
        {
            get { return _backColor; }
            set
            {
                this.RaiseAndSetIfChanged(ref _backColor, value);
            }
        }

        private double _raiseTime;
        public double RaiseTime
        {
            get => _raiseTime;
            set => this.RaiseAndSetIfChanged(ref _raiseTime, value);
        }

        private double _dispenseTime;
        public double DispenseTime
        {
            get => _dispenseTime;
            set => this.RaiseAndSetIfChanged(ref _dispenseTime, value);
        }

        private double _impactTime;
        public double ImpactTime
        {
            get => _impactTime;
            set => this.RaiseAndSetIfChanged(ref _impactTime, value);
        }

        private double _intermittentTime;
        public double IntermittentTime
        {
            get => _intermittentTime;
            set => this.RaiseAndSetIfChanged(ref _intermittentTime, value);
        }

        private double _voltageRatio;
        public double VoltageRatio
        {
            get => _voltageRatio;
            set => this.RaiseAndSetIfChanged(ref _voltageRatio, value);
        }

        private int _dotCount;
        public int DotCount
        {
            get => _dotCount;
            set => this.RaiseAndSetIfChanged(ref _dotCount, value);
        }

        private int _dispenseMode;
        public int DispenseMode
        {
            get => _dispenseMode;
            set => this.RaiseAndSetIfChanged(ref _dispenseMode, value);
        }

        private bool _afterStop;
        public bool AfterStop
        {
            get => _afterStop;
            set => this.RaiseAndSetIfChanged(ref _afterStop, value);
        }

        private int _totalCount;
        public int TotalCount
        {
            get => _totalCount;
            set => this.RaiseAndSetIfChanged(ref _totalCount, value);
        }
        #endregion

        #region 命令定义
        public ReactiveCommand<Point, Unit> PointerPressedCommand { get; }

        public ReactiveCommand<Unit, Unit> CommunicationCommand { get; }

        public ReactiveCommand<Unit, Unit> OpenSettingCommand { get; }
        #endregion

        #region 构造函数
        public CommunicationViewModel(
            CommunicationPropertyModelEdit communicationPropertyModelEdit,
            Action communicationAction)
        {
            _communication = communicationAction;

            _communicationPropertyModelEdit = communicationPropertyModelEdit;
            CopyFrom(communicationPropertyModelEdit);

            CommunicationCommand = ReactiveCommand.Create(ExecuteCommunication);

            this.WhenAnyValue(x => x.RaiseTime).Subscribe(v => { if (_communicationPropertyModelEdit.RaiseTime != v) _communicationPropertyModelEdit.RaiseTime = v; }).DisposeWith(_disposables);
            this.WhenAnyValue(x => x.DispenseTime).Subscribe(v => { if (_communicationPropertyModelEdit.DispenseTime != v) _communicationPropertyModelEdit.DispenseTime = v; }).DisposeWith(_disposables);
            this.WhenAnyValue(x => x.ImpactTime).Subscribe(v => { if (_communicationPropertyModelEdit.ImpactTime != v) _communicationPropertyModelEdit.ImpactTime = v; }).DisposeWith(_disposables);
            this.WhenAnyValue(x => x.IntermittentTime).Subscribe(v => { if (_communicationPropertyModelEdit.IntermittentTime != v) _communicationPropertyModelEdit.IntermittentTime = v; }).DisposeWith(_disposables);
            this.WhenAnyValue(x => x.VoltageRatio).Subscribe(v => { if (_communicationPropertyModelEdit.VoltageRatio != v) _communicationPropertyModelEdit.VoltageRatio = v; }).DisposeWith(_disposables);
            this.WhenAnyValue(x => x.DotCount).Subscribe(v => { if (_communicationPropertyModelEdit.DotCount != v) _communicationPropertyModelEdit.DotCount = v; }).DisposeWith(_disposables);
            this.WhenAnyValue(x => x.DispenseMode).Subscribe(v => { if (_communicationPropertyModelEdit.DispenseMode != v) _communicationPropertyModelEdit.DispenseMode = v; }).DisposeWith(_disposables);
            this.WhenAnyValue(x => x.AfterStop).Subscribe(v => { if (_communicationPropertyModelEdit.AfterStop != v) _communicationPropertyModelEdit.AfterStop = v; }).DisposeWith(_disposables);
            this.WhenAnyValue(x => x.TotalCount).Subscribe(v => { if (_communicationPropertyModelEdit.TotalCount != v) _communicationPropertyModelEdit.TotalCount = v; }).DisposeWith(_disposables);

            _communicationPropertyModelEdit.PropertyChanged += CommunicationPropertyModelEdit_PropertyChanged;
            Disposable.Create(() => _communicationPropertyModelEdit.PropertyChanged -= CommunicationPropertyModelEdit_PropertyChanged)
                .DisposeWith(_disposables);
        }
        #endregion

        #region 公共方法
        public void Dispose()
        {
            _disposables.Dispose();
        }
        #endregion

        #region 私有方法
        private void CopyFrom(CommunicationPropertyModelEdit src)
        {
            this.TextFont = src.TextFont;
            this.TextColor = src.TextColor;
            this.BackColor = src.BackColor;

            this.RaiseTime = src.RaiseTime;
            this.DispenseTime = src.DispenseTime;
            this.ImpactTime = src.ImpactTime;
            this.IntermittentTime = src.IntermittentTime;
            this.VoltageRatio = src.VoltageRatio;
            this.DotCount = src.DotCount;
            this.DispenseMode = src.DispenseMode;
            this.AfterStop = src.AfterStop;
            this.TotalCount = src.TotalCount;
        }

        private void CommunicationPropertyModelEdit_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {

            switch (e.PropertyName)
            {
                case nameof(CommunicationPropertyModelEdit.TextFont):
                    TextFont = _communicationPropertyModelEdit.TextFont;
                    break;
                case nameof(CommunicationPropertyModelEdit.TextColor):
                    TextColor = _communicationPropertyModelEdit.TextColor;
                    break;
                case nameof(CommunicationPropertyModelEdit.BackColor):
                    BackColor = _communicationPropertyModelEdit.BackColor;
                    break;
                case nameof(CommunicationPropertyModelEdit.RaiseTime):
                    RaiseTime = _communicationPropertyModelEdit.RaiseTime;
                    break;
                case nameof(CommunicationPropertyModelEdit.DispenseTime):
                    DispenseTime = _communicationPropertyModelEdit.DispenseTime;
                    break;
                case nameof(CommunicationPropertyModelEdit.ImpactTime):
                    ImpactTime = _communicationPropertyModelEdit.ImpactTime;
                    break;
                case nameof(CommunicationPropertyModelEdit.IntermittentTime):
                    IntermittentTime = _communicationPropertyModelEdit.IntermittentTime;
                    break;
                case nameof(CommunicationPropertyModelEdit.VoltageRatio):
                    VoltageRatio = _communicationPropertyModelEdit.VoltageRatio;
                    break;
                case nameof(CommunicationPropertyModelEdit.DotCount):
                    DotCount = _communicationPropertyModelEdit.DotCount;
                    break;
                case nameof(CommunicationPropertyModelEdit.DispenseMode):
                    DispenseMode = _communicationPropertyModelEdit.DispenseMode;
                    break;
                case nameof(CommunicationPropertyModelEdit.AfterStop):
                    AfterStop = _communicationPropertyModelEdit.AfterStop;
                    break;
                case nameof(CommunicationPropertyModelEdit.TotalCount):
                    TotalCount = _communicationPropertyModelEdit.TotalCount;
                    break;
            }
        }
        #endregion

        #region 命令执行方法
        private void ExecuteCommunication()
        {
            _communication?.Invoke();
        }
        #endregion
    }
}
