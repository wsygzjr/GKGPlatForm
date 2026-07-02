using Griffins.CompUI.SL.InitCfgPage.Models;
using Griffins.CompUI.SL.InitCfgPage.Views;
using Griffins.Map.UI;
using ReactiveUI;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;
using System.IO;
using System.Reactive;
using System.Reactive.Disposables;

namespace Griffins.CompUI.SL.InitCfgPage.ViewModels
{
    internal class TrackStationConfigCompUIViewModel : ReactiveObject, IDisposable
    {
        private readonly bool isDesign;
        private readonly ICompUIRunTimeCallBack? callBack;

        private readonly CompositeDisposable _disposables = new();

        private Control? _viewReference;

        private SensorConfigCfgInfo? _leftSensorConfig;
        private SensorConfigCfgInfo? _rightSensorConfig;

        private GKG.UI.General.CylinderConfigCfgInfo? _leftCylinderConfig;
        private GKG.UI.General.CylinderConfigCfgInfo? _rightCylinderConfig;

        private object _viewTag;
        public object ViewTag
        {
            get => _viewTag;
            set => this.RaiseAndSetIfChanged(ref _viewTag, value);
        }

        private bool _readOnly;
        public bool ReadOnly
        {
            get => _readOnly;
            set => this.RaiseAndSetIfChanged(ref _readOnly, value);
        }

        private bool _hasLeftSensor;
        public bool HasLeftSensor
        {
            get => _hasLeftSensor;
            set => this.RaiseAndSetIfChanged(ref _hasLeftSensor, value);
        }

        private bool _hasRightSensor;
        public bool HasRightSensor
        {
            get => _hasRightSensor;
            set => this.RaiseAndSetIfChanged(ref _hasRightSensor, value);
        }

        private StationPreviewViewModel _stationPreview = new();
        public StationPreviewViewModel StationPreview
        {
            get => _stationPreview;
            private set => this.RaiseAndSetIfChanged(ref _stationPreview, value);
        }

        private bool _hasLeftCylinder;
        public bool HasLeftCylinder
        {
            get => _hasLeftCylinder;
            set => this.RaiseAndSetIfChanged(ref _hasLeftCylinder, value);
        }

        private bool _hasRightCylinder;
        public bool HasRightCylinder
        {
            get => _hasRightCylinder;
            set => this.RaiseAndSetIfChanged(ref _hasRightCylinder, value);
        }

        public ReactiveCommand<Unit, Unit> ConfigLeftSensorCommand { get; }
        public ReactiveCommand<Unit, Unit> ConfigRightSensorCommand { get; }
        public ReactiveCommand<Unit, Unit> ConfigLeftCylinderCommand { get; }
        public ReactiveCommand<Unit, Unit> ConfigRightCylinderCommand { get; }

        public TrackStationConfigCompUIViewModel()
        {
            isDesign = true;
            callBack = null;

            ConfigLeftSensorCommand = ReactiveCommand.Create(() => { });
            ConfigRightSensorCommand = ReactiveCommand.Create(() => { });
            ConfigLeftCylinderCommand = ReactiveCommand.Create(() => { });
            ConfigRightCylinderCommand = ReactiveCommand.Create(() => { });

            InitStationPreviewBinding();
        }

        public TrackStationConfigCompUIViewModel(bool isDesign, ICompUIRunTimeCallBack callBack)
        {
            this.isDesign = isDesign;
            this.callBack = callBack;

            ConfigLeftSensorCommand = ReactiveCommand.Create(OnConfigLeftSensor);
            ConfigRightSensorCommand = ReactiveCommand.Create(OnConfigRightSensor);
            ConfigLeftCylinderCommand = ReactiveCommand.Create(OnConfigLeftCylinder);
            ConfigRightCylinderCommand = ReactiveCommand.Create(OnConfigRightCylinder);

            InitStationPreviewBinding();
        }

        private void InitStationPreviewBinding()
        {
            // 仅复刻 StationFuncCtlMapCell 的“是否显示感应器图片”的逻辑：
            // - 总开关：任意一侧存在感应器即显示感应器区域
            // - 左/右：分别控制对应感应器图片是否显示
            this.WhenAnyValue(x => x.HasLeftSensor, x => x.HasRightSensor)
                .Subscribe(v =>
                {
                    var hasLeft = v.Item1;
                    var hasRight = v.Item2;

                    StationPreview.ShowSensors = hasLeft || hasRight;
                    StationPreview.ShowLeftSensorImage = hasLeft;
                    StationPreview.ShowRightSensorImage = hasRight;
                })
                .DisposeWith(_disposables);
        }

        public void SetData(TrackStationConfigCompUIModel model)
        {
            if (model == null)
            {
                return;
            }

            HasLeftSensor = model.HasLeftSensor;
            HasRightSensor = model.HasRightSensor;
            HasLeftCylinder = model.HasLeftCylinder;
            HasRightCylinder = model.HasRightCylinder;

            _leftSensorConfig = model.LeftSensorConfig;
            _rightSensorConfig = model.RightSensorConfig;

            _leftCylinderConfig = model.LeftCylinderConfig;
            _rightCylinderConfig = model.RightCylinderConfig;
        }

        public TrackStationConfigCompUIModel GetData()
        {
            return new TrackStationConfigCompUIModel()
            {
                HasLeftSensor = HasLeftSensor,
                HasRightSensor = HasRightSensor,
                HasLeftCylinder = HasLeftCylinder,
                HasRightCylinder = HasRightCylinder,
                LeftSensorConfig = _leftSensorConfig,
                RightSensorConfig = _rightSensorConfig,
                LeftCylinderConfig = _leftCylinderConfig,
                RightCylinderConfig = _rightCylinderConfig,
            };
        }

        public void SetViewReference(Control view)
        {
            _viewReference = view;
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }

        internal class StationPreviewViewModel : ReactiveObject
        {
            private const string AssetsBaseUri = "avares://Griffins.CompUI.SL/CompUI_SL/PageType/InitCfgPage/TrackStationConfig/Assets/Images/";

            private static Bitmap? LoadBitmap(string fileName)
            {
                try
                {
                    var uri = new Uri(AssetsBaseUri + fileName);
                    using Stream s = AssetLoader.Open(uri);
                    return new Bitmap(s);
                }
                catch
                {
                    return null;
                }
            }

            private static readonly Bitmap? _stationBmp = LoadBitmap("ctr-Station.png");
            private static readonly Bitmap? _stationActiveBmp = LoadBitmap("ctr-Station-active.png");
            private static readonly Bitmap? _noBmp = LoadBitmap("ctr-No.png");
            private static readonly Bitmap? _yesBmp = LoadBitmap("ctr-Yes.png");

            private bool _hasBoard;
            public bool HasBoard
            {
                get => _hasBoard;
                set
                {
                    this.RaiseAndSetIfChanged(ref _hasBoard, value);
                    UpdateStationImage();
                }
            }

            private bool _leftHasBoard;
            public bool LeftHasBoard
            {
                get => _leftHasBoard;
                set
                {
                    this.RaiseAndSetIfChanged(ref _leftHasBoard, value);
                    UpdateLeftSensorImage();
                }
            }

            private bool _rightHasBoard;
            public bool RightHasBoard
            {
                get => _rightHasBoard;
                set
                {
                    this.RaiseAndSetIfChanged(ref _rightHasBoard, value);
                    UpdateRightSensorImage();
                }
            }

            private bool _showSensors = true;
            public bool ShowSensors
            {
                get => _showSensors;
                set
                {
                    this.RaiseAndSetIfChanged(ref _showSensors, value);
                    this.RaisePropertyChanged(nameof(LeftSensorVisible));
                    this.RaisePropertyChanged(nameof(RightSensorVisible));
                }
            }

            private bool _showLeftSensorImage = true;
            public bool ShowLeftSensorImage
            {
                get => _showLeftSensorImage;
                set
                {
                    this.RaiseAndSetIfChanged(ref _showLeftSensorImage, value);
                    this.RaisePropertyChanged(nameof(LeftSensorVisible));
                }
            }

            private bool _showRightSensorImage = true;
            public bool ShowRightSensorImage
            {
                get => _showRightSensorImage;
                set
                {
                    this.RaiseAndSetIfChanged(ref _showRightSensorImage, value);
                    this.RaisePropertyChanged(nameof(RightSensorVisible));
                }
            }

            public bool LeftSensorVisible => ShowSensors && ShowLeftSensorImage;
            public bool RightSensorVisible => ShowSensors && ShowRightSensorImage;

            private Bitmap? _leftSensorImage;
            public Bitmap? LeftSensorImage
            {
                get => _leftSensorImage;
                private set => this.RaiseAndSetIfChanged(ref _leftSensorImage, value);
            }

            private Bitmap? _stationImage;
            public Bitmap? StationImage
            {
                get => _stationImage;
                private set => this.RaiseAndSetIfChanged(ref _stationImage, value);
            }

            private Bitmap? _rightSensorImage;
            public Bitmap? RightSensorImage
            {
                get => _rightSensorImage;
                private set => this.RaiseAndSetIfChanged(ref _rightSensorImage, value);
            }

            private void UpdateLeftSensorImage()
            {
                LeftSensorImage = LeftHasBoard ? _yesBmp : _noBmp;
            }

            private void UpdateRightSensorImage()
            {
                RightSensorImage = RightHasBoard ? _yesBmp : _noBmp;
            }

            private void UpdateStationImage()
            {
                StationImage = HasBoard ? (_stationActiveBmp ?? _stationBmp) : _stationBmp;
            }

            public StationPreviewViewModel()
            {
                StationImage = _stationBmp;
                LeftSensorImage = _noBmp;
                RightSensorImage = _noBmp;
            }
        }

        private void OnConfigLeftSensor()
        {
            if (isDesign)
            {
                return;
            }

            showSensorConfigDialog(isLeft: true);
        }

        private void OnConfigRightSensor()
        {
            if (isDesign)
            {
                return;
            }

            showSensorConfigDialog(isLeft: false);
        }

        private void OnConfigLeftCylinder()
        {
            if (isDesign)
            {
                return;
            }

            showCylinderConfigDialog(isLeft: true);
        }

        private void OnConfigRightCylinder()
        {
            if (isDesign)
            {
                return;
            }

            showCylinderConfigDialog(isLeft: false);
        }

        private void showCylinderConfigDialog(bool isLeft)
        {
            if (_viewReference == null)
            {
                return;
            }

            var cfgModel = isLeft ? _leftCylinderConfig : _rightCylinderConfig;
            cfgModel ??= new GKG.UI.General.CylinderConfigCfgInfo();

            var cfgVm = new GKG.UI.General.CylinderConfigViewModel();
            cfgVm.SetViewReference(_viewReference);
            cfgVm.CopyFrom(cfgModel);

            var cfgView = new GKG.UI.General.CylinderConfigView
            {
                DataContext = cfgVm,
            };

            var ownerWindow = TopLevel.GetTopLevel(_viewReference) as Window;

            var window = new Window
            {
                Title = "气缸配置",
                Width = 900,
                Height = 650,
                Padding = new Thickness(10),
                Content = cfgView,
                WindowStartupLocation = ownerWindow != null ? WindowStartupLocation.CenterOwner : WindowStartupLocation.CenterScreen,
            };

            window.Closed += (_, __) =>
            {
                cfgVm.CopyTo(cfgModel);

                if (isLeft)
                {
                    _leftCylinderConfig = cfgModel;
                }
                else
                {
                    _rightCylinderConfig = cfgModel;
                }

                this.RaisePropertyChanged(nameof(HasLeftCylinder));
                this.RaisePropertyChanged(nameof(HasRightCylinder));
            };

            if (ownerWindow != null)
            {
                _ = window.ShowDialog(ownerWindow);
            }
            else
            {
                window.Show();
            }
        }

        private void showSensorConfigDialog(bool isLeft)
        {
            if (_viewReference == null)
            {
                return;
            }

            var cfgModel = isLeft ? _leftSensorConfig : _rightSensorConfig;
            cfgModel ??= new SensorConfigCfgInfo();

            var cfgVm = new SensorConfigViewModel();
            cfgVm.CopyFrom(cfgModel);

            var cfgView = new SensorConfigView
            {
                DataContext = cfgVm,
            };

            var ownerWindow = TopLevel.GetTopLevel(_viewReference) as Window;

            var window = new Window
            {
                Title = "感应器配置",
                Width = 650,
                Height = 260,
                Padding = new Thickness(10),
                Content = cfgView,
                WindowStartupLocation = ownerWindow != null ? WindowStartupLocation.CenterOwner : WindowStartupLocation.CenterScreen,
            };

            window.Closed += (_, __) =>
            {
                cfgVm.CopyTo(cfgModel);

                if (isLeft)
                {
                    _leftSensorConfig = cfgModel;
                }
                else
                {
                    _rightSensorConfig = cfgModel;
                }

                this.RaisePropertyChanged(nameof(HasLeftSensor));
                this.RaisePropertyChanged(nameof(HasRightSensor));
            };

            if (ownerWindow != null)
            {
                _ = window.ShowDialog(ownerWindow);
            }
            else
            {
                window.Show();
            }
        }
    }
}
