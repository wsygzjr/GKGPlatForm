using GF_Gereric;
using GKG;
using GKG.SubMM;
using GKG.UI;
using ReactiveUI;
using System;
using System.Linq;

namespace Griffins.CompUI.GantryClean.CompUI_GantryClean.PageType.InitCfgPage.GantryCleanConfig.ViewModels
{
    public class GantryCleanConfigCompUIViewModel : ReactiveObject
    {
        private CleanParameters _data;

        public ToggleSwitchViewModel CleanSwitchViewModel { get; } = new ToggleSwitchViewModel();

        public ToggleSwitchViewModel WipeSwitchViewModel { get; } = new ToggleSwitchViewModel();

        public ToggleSwitchViewModel PurgeSwitchViewModel { get; } = new ToggleSwitchViewModel();

        public ToggleSwitchViewModel StopMachinePurgeSwitchViewModel { get; } = new ToggleSwitchViewModel();

        public ComboxViewModel CleanOrderComboxViewModel { get; } = new ComboxViewModel();

        public TextInputViewModel FunctionHeadIdTextViewModel { get; } = new TextInputViewModel();
        public TextInputViewModel CleanCountTextViewModel { get; } = new TextInputViewModel();
        public TextInputViewModel CCDToValveDistanceXTextViewModel { get; } = new TextInputViewModel();
        public TextInputViewModel CCDToValveDistanceYTextViewModel { get; } = new TextInputViewModel();

        public ComboxViewModel WipeModeComboxViewModel { get; } = new ComboxViewModel();
        public ComboxViewModel WipeTeachPosModeComboxViewModel { get; } = new ComboxViewModel();
        public TextInputViewModel WipePosXTextViewModel { get; } = new TextInputViewModel();
        public TextInputViewModel WipePosYTextViewModel { get; } = new TextInputViewModel();
        public TextInputViewModel WipePosZTextViewModel { get; } = new TextInputViewModel();
        public TextInputViewModel WipeCylinderCountTextViewModel { get; } = new TextInputViewModel();
        public TextInputViewModel RollPaperCountTextViewModel { get; } = new TextInputViewModel();
        public TextInputViewModel RollPaperSpeedTextViewModel { get; } = new TextInputViewModel();
        public TextInputViewModel CleanRadiusTextViewModel { get; } = new TextInputViewModel();
        public TextInputViewModel WipeSpeedTextViewModel { get; } = new TextInputViewModel();
        public ToggleSwitchViewModel WipeOpenForArraySwitchViewModel { get; } = new ToggleSwitchViewModel();
        public TextInputViewModel WipeSpaceTextViewModel { get; } = new TextInputViewModel();
        public TextInputViewModel XCountTextViewModel { get; } = new TextInputViewModel();
        public TextInputViewModel YCountTextViewModel { get; } = new TextInputViewModel();
        public TextInputViewModel CurrentRunIndexTextViewModel { get; } = new TextInputViewModel();
        public TextInputViewModel SetLoopTimesTextViewModel { get; } = new TextInputViewModel();
        public TextInputViewModel RealLoopTimesTextViewModel { get; } = new TextInputViewModel();
        public ToggleSwitchViewModel CheckCleanPaperSwitchViewModel { get; } = new ToggleSwitchViewModel();
        public TextInputViewModel ValveBackAndForthTimesTextViewModel { get; } = new TextInputViewModel();
        public TextInputViewModel ValveBackAndForthDisTextViewModel { get; } = new TextInputViewModel();
        public ToggleSwitchViewModel ReversalSwitchViewModel { get; } = new ToggleSwitchViewModel();
        public TextInputViewModel BrushRunTimeTextViewModel { get; } = new TextInputViewModel();
        public TextInputViewModel CenterWipeTimeTextViewModel { get; } = new TextInputViewModel();

        public ToggleSwitchViewModel ToAndFroMoveSwitchViewModel { get; } = new ToggleSwitchViewModel();
        public ComboxViewModel PurgeTeachPosModeComboxViewModel { get; } = new ComboxViewModel();
        public TextInputViewModel PurgePosXTextViewModel { get; } = new TextInputViewModel();
        public TextInputViewModel PurgePosYTextViewModel { get; } = new TextInputViewModel();
        public TextInputViewModel PurgePosZTextViewModel { get; } = new TextInputViewModel();
        public TextInputViewModel PurgeGluePointsTextViewModel { get; } = new TextInputViewModel();
        public TextInputViewModel PurgeIntervalPcsTextViewModel { get; } = new TextInputViewModel();
        public TextInputViewModel PurgeIntervalMinPcsTextViewModel { get; } = new TextInputViewModel();
        public TextInputViewModel PurgeIntervalTimeSTextViewModel { get; } = new TextInputViewModel();
        public ToggleSwitchViewModel PurgeGlueOpenForArraySwitchViewModel { get; } = new ToggleSwitchViewModel();
        public TextInputViewModel PurgeGlueSpaceTextViewModel { get; } = new TextInputViewModel();
        public TextInputViewModel PurgeGlueXCountTextViewModel { get; } = new TextInputViewModel();
        public TextInputViewModel PurgeGlueYCountTextViewModel { get; } = new TextInputViewModel();
        public TextInputViewModel PurgeGlueCurrentRunIndexTextViewModel { get; } = new TextInputViewModel();
        public TextInputViewModel PurgeGlueSetLoopTimesTextViewModel { get; } = new TextInputViewModel();
        public TextInputViewModel PurgeGlueRealLoopTimesTextViewModel { get; } = new TextInputViewModel();
        public TextInputViewModel ToAndFroNumTextViewModel { get; } = new TextInputViewModel();
        public TextInputViewModel ToAndFroDistanceTextViewModel { get; } = new TextInputViewModel();
        public TextInputViewModel ToAndFroSpeedTextViewModel { get; } = new TextInputViewModel();
        public TextInputViewModel ToAndFroAccSpeedTextViewModel { get; } = new TextInputViewModel();
        public TextInputViewModel ActualvalueBoardNumTextViewModel { get; } = new TextInputViewModel();
        public TextInputViewModel ScraperDirectionTextViewModel { get; } = new TextInputViewModel();

        public ComboxViewModel CleanModeComboxViewModel { get; } = new ComboxViewModel();
        public ComboxViewModel CleanTeachPosModeComboxViewModel { get; } = new ComboxViewModel();
        public TextInputViewModel CleanPosXTextViewModel { get; } = new TextInputViewModel();
        public TextInputViewModel CleanPosYTextViewModel { get; } = new TextInputViewModel();
        public TextInputViewModel CleanPosZTextViewModel { get; } = new TextInputViewModel();
        public TextInputViewModel SpitGluePointsTextViewModel { get; } = new TextInputViewModel();
        public TextInputViewModel VacuumTimeSTextViewModel { get; } = new TextInputViewModel();
        public TextInputViewModel CleanIntervalPcsTextViewModel { get; } = new TextInputViewModel();
        public TextInputViewModel CleanIntervalMinPcsTextViewModel { get; } = new TextInputViewModel();
        public TextInputViewModel CleanIntervalTimeSTextViewModel { get; } = new TextInputViewModel();

        public event EventHandler AfterModified;

        public GantryCleanConfigCompUIViewModel()
        {
            CleanOrderComboxViewModel.ItemsSource = new[]
            {
                new CleanOrderOption(EnumCleanOrder.CleanFirst, "先清洗"),
                new CleanOrderOption(EnumCleanOrder.WipeFirst, "先擦拭"),
            };

            WipeModeComboxViewModel.ItemsSource = new[]
            {
                new WipeModeOption(EnumWipeMode.WipeMode_Brush, "毛刷"),
                new WipeModeOption(EnumWipeMode.WipeMode_WipeCylinder, "气缸"),
                new WipeModeOption(EnumWipeMode.WipeMode_Paper, "转纸"),
                new WipeModeOption(EnumWipeMode.WipeMode_Center, "中心"),
            };
            WipeModeComboxViewModel.DisplayMemberPath = nameof(WipeModeOption.DisplayName);
            WipeModeComboxViewModel.ValueChanged += (s, e) =>
            {
                ensureNested();
                if (e.NewValue is WipeModeOption opt)
                {
                    _data.WipeParameters.WipeMode = opt.Value;
                    AfterModified?.Invoke(this, EventArgs.Empty);
                }
            };

            var teachPosModeItems = new[]
            {
                new TeachPosModeOption(EnumTeachCleanPosMode.TeachCleanPosMode_ValvePos, "阀位置"),
                new TeachPosModeOption(EnumTeachCleanPosMode.TeachCleanPosMode_CCDPos, "CCD位置"),
            };

            WipeTeachPosModeComboxViewModel.ItemsSource = teachPosModeItems;
            WipeTeachPosModeComboxViewModel.DisplayMemberPath = nameof(TeachPosModeOption.DisplayName);
            WipeTeachPosModeComboxViewModel.ValueChanged += (s, e) =>
            {
                ensureNested();
                if (e.NewValue is TeachPosModeOption opt)
                {
                    _data.WipeParameters.ETeachCleanPosMode = opt.Value;
                    AfterModified?.Invoke(this, EventArgs.Empty);
                }
            };

            PurgeTeachPosModeComboxViewModel.ItemsSource = teachPosModeItems;
            PurgeTeachPosModeComboxViewModel.DisplayMemberPath = nameof(TeachPosModeOption.DisplayName);
            PurgeTeachPosModeComboxViewModel.ValueChanged += (s, e) =>
            {
                ensureNested();
                if (e.NewValue is TeachPosModeOption opt)
                {
                    _data.PurgeGlueParameters.ETeachCleanPosMode = opt.Value;
                    AfterModified?.Invoke(this, EventArgs.Empty);
                }
            };

            CleanTeachPosModeComboxViewModel.ItemsSource = teachPosModeItems;
            CleanTeachPosModeComboxViewModel.DisplayMemberPath = nameof(TeachPosModeOption.DisplayName);
            CleanTeachPosModeComboxViewModel.ValueChanged += (s, e) =>
            {
                ensureNested();
                if (e.NewValue is TeachPosModeOption opt)
                {
                    _data.CleanGlueParameters.ETeachCleanPosMode = opt.Value;
                    AfterModified?.Invoke(this, EventArgs.Empty);
                }
            };

            CleanModeComboxViewModel.ItemsSource = new[]
            {
                new CleanModeOption(EnumCleanMode.CleanMode_Center, "中心清洁"),
                new CleanModeOption(EnumCleanMode.CleanMode_Ring, "旋转清洁"),
            };
            CleanModeComboxViewModel.DisplayMemberPath = nameof(CleanModeOption.DisplayName);
            CleanModeComboxViewModel.ValueChanged += (s, e) =>
            {
                ensureNested();
                if (e.NewValue is CleanModeOption opt)
                {
                    _data.CleanGlueParameters.ECleanMode = opt.Value;
                    AfterModified?.Invoke(this, EventArgs.Empty);
                }
            };
            CleanOrderComboxViewModel.DisplayMemberPath = nameof(CleanOrderOption.DisplayName);
            CleanOrderComboxViewModel.ValueChanged += (s, e) =>
            {
                if (_data == null)
                    return;

                if (e.NewValue is CleanOrderOption opt)
                {
                    _data.ECleanOrder = opt.Value;
                    AfterModified?.Invoke(this, EventArgs.Empty);
                }
            };

            wireToggle(CleanSwitchViewModel, () =>
            {
                ensureData();
                ensureNested();
                return _data.CleanGlueParameters.Enable;
            }, v =>
            {
                ensureData();
                ensureNested();
                _data.CleanGlueParameters.Enable = v;
            });

            wireToggle(WipeSwitchViewModel, () =>
            {
                ensureData();
                ensureNested();
                return _data.WipeParameters.Enable;
            }, v =>
            {
                ensureData();
                ensureNested();
                _data.WipeParameters.Enable = v;
            });

            wireToggle(PurgeSwitchViewModel, () =>
            {
                ensureData();
                ensureNested();
                return _data.PurgeGlueParameters.Enable;
            }, v =>
            {
                ensureData();
                ensureNested();
                _data.PurgeGlueParameters.Enable = v;
            });

            wireToggle(StopMachinePurgeSwitchViewModel, () =>
            {
                ensureData();
                ensureNested();
                return _data.PurgeGlueParameters.StopMachinePurgeOpen;
            }, v =>
            {
                ensureData();
                ensureNested();
                _data.PurgeGlueParameters.StopMachinePurgeOpen = v;
            });

            wireToggle(ToAndFroMoveSwitchViewModel, () =>
            {
                ensureNested();
                return _data.PurgeGlueParameters.ToAndFroMove;
            }, v =>
            {
                ensureNested();
                _data.PurgeGlueParameters.ToAndFroMove = v;
            });

            wireToggle(WipeOpenForArraySwitchViewModel, () =>
            {
                ensureNested();
                return _data.WipeParameters.WipeOpenForArray;
            }, v =>
            {
                ensureNested();
                _data.WipeParameters.WipeOpenForArray = v;
            });

            wireToggle(CheckCleanPaperSwitchViewModel, () =>
            {
                ensureNested();
                return _data.WipeParameters.CheckCleanPaper;
            }, v =>
            {
                ensureNested();
                _data.WipeParameters.CheckCleanPaper = v;
            });

            wireToggle(ReversalSwitchViewModel, () =>
            {
                ensureNested();
                return _data.WipeParameters.Reversal;
            }, v =>
            {
                ensureNested();
                _data.WipeParameters.Reversal = v;
            });

            wireToggle(PurgeGlueOpenForArraySwitchViewModel, () =>
            {
                ensureNested();
                return _data.PurgeGlueParameters.PurgeGlueOpenForArray;
            }, v =>
            {
                ensureNested();
                _data.PurgeGlueParameters.PurgeGlueOpenForArray = v;
            });

            wireTextInt(FunctionHeadIdTextViewModel, () => _data?.FunctionHeadId ?? 0, v => { ensureData(); _data.FunctionHeadId = v; });
            wireTextInt(CleanCountTextViewModel, () => _data?.CleanCount ?? 0, v => { ensureData(); _data.CleanCount = v; });
            wireTextDouble(CCDToValveDistanceXTextViewModel, () => _data?.CCDToValveDistanceX ?? 0, v => { ensureData(); _data.CCDToValveDistanceX = v; });
            wireTextDouble(CCDToValveDistanceYTextViewModel, () => _data?.CCDToValveDistanceY ?? 0, v => { ensureData(); _data.CCDToValveDistanceY = v; });

            wireTextDouble(WipePosXTextViewModel, () => _data?.WipeParameters?.Position?.X ?? 0, v => { ensureNested(); _data.WipeParameters.Position.X = v; });
            wireTextDouble(WipePosYTextViewModel, () => _data?.WipeParameters?.Position?.Y ?? 0, v => { ensureNested(); _data.WipeParameters.Position.Y = v; });
            wireTextDouble(WipePosZTextViewModel, () => _data?.WipeParameters?.Position?.Z ?? 0, v => { ensureNested(); _data.WipeParameters.Position.Z = v; });
            wireTextDouble(PurgePosXTextViewModel, () => _data?.PurgeGlueParameters?.Position?.X ?? 0, v => { ensureNested(); _data.PurgeGlueParameters.Position.X = v; });
            wireTextDouble(PurgePosYTextViewModel, () => _data?.PurgeGlueParameters?.Position?.Y ?? 0, v => { ensureNested(); _data.PurgeGlueParameters.Position.Y = v; });
            wireTextDouble(PurgePosZTextViewModel, () => _data?.PurgeGlueParameters?.Position?.Z ?? 0, v => { ensureNested(); _data.PurgeGlueParameters.Position.Z = v; });
            wireTextDouble(CleanPosXTextViewModel, () => _data?.CleanGlueParameters?.Position?.X ?? 0, v => { ensureNested(); _data.CleanGlueParameters.Position.X = v; });
            wireTextDouble(CleanPosYTextViewModel, () => _data?.CleanGlueParameters?.Position?.Y ?? 0, v => { ensureNested(); _data.CleanGlueParameters.Position.Y = v; });
            wireTextDouble(CleanPosZTextViewModel, () => _data?.CleanGlueParameters?.Position?.Z ?? 0, v => { ensureNested(); _data.CleanGlueParameters.Position.Z = v; });

            wireTextInt(WipeCylinderCountTextViewModel, () => _data?.WipeParameters?.WipeCylinderCount ?? 0, v => { ensureNested(); _data.WipeParameters.WipeCylinderCount = v; });
            wireTextInt(RollPaperCountTextViewModel, () => _data?.WipeParameters?.RollPaperCount ?? 0, v => { ensureNested(); _data.WipeParameters.RollPaperCount = v; });
            wireTextDouble(RollPaperSpeedTextViewModel, () => _data?.WipeParameters?.RollPaperSpeed ?? 0, v => { ensureNested(); _data.WipeParameters.RollPaperSpeed = v; });
            wireTextDouble(CleanRadiusTextViewModel, () => _data?.WipeParameters?.CleanRadius ?? 0, v => { ensureNested(); _data.WipeParameters.CleanRadius = v; });
            wireTextDouble(WipeSpeedTextViewModel, () => _data?.WipeParameters?.WipeSpeed ?? 0, v => { ensureNested(); _data.WipeParameters.WipeSpeed = v; });
            wireTextDouble(WipeSpaceTextViewModel, () => _data?.WipeParameters?.WipeSpace ?? 0, v => { ensureNested(); _data.WipeParameters.WipeSpace = v; });
            wireTextInt(XCountTextViewModel, () => _data?.WipeParameters?.XCount ?? 0, v => { ensureNested(); _data.WipeParameters.XCount = v; });
            wireTextInt(YCountTextViewModel, () => _data?.WipeParameters?.YCount ?? 0, v => { ensureNested(); _data.WipeParameters.YCount = v; });
            wireTextInt(CurrentRunIndexTextViewModel, () => _data?.WipeParameters?.CurrentRunIndex ?? 0, v => { ensureNested(); _data.WipeParameters.CurrentRunIndex = v; });
            wireTextInt(SetLoopTimesTextViewModel, () => _data?.WipeParameters?.SetLoopTimes ?? 0, v => { ensureNested(); _data.WipeParameters.SetLoopTimes = v; });
            wireTextInt(RealLoopTimesTextViewModel, () => _data?.WipeParameters?.RealLoopTimes ?? 0, v => { ensureNested(); _data.WipeParameters.RealLoopTimes = v; });
            wireTextInt(ValveBackAndForthTimesTextViewModel, () => _data?.WipeParameters?.ValveBackAndForthTimes ?? 0, v => { ensureNested(); _data.WipeParameters.ValveBackAndForthTimes = v; });
            wireTextDouble(ValveBackAndForthDisTextViewModel, () => _data?.WipeParameters?.ValveBackAndForthDis ?? 0, v => { ensureNested(); _data.WipeParameters.ValveBackAndForthDis = v; });
            wireTextInt(BrushRunTimeTextViewModel, () => _data?.WipeParameters?.BrushRunTime ?? 0, v => { ensureNested(); _data.WipeParameters.BrushRunTime = v; });
            wireTextInt(CenterWipeTimeTextViewModel, () => _data?.WipeParameters?.CenterWipeTime ?? 0, v => { ensureNested(); _data.WipeParameters.CenterWipeTime = v; });

            wireTextInt(PurgeGluePointsTextViewModel, () => _data?.PurgeGlueParameters?.PurgeGluePoints ?? 0, v => { ensureNested(); _data.PurgeGlueParameters.PurgeGluePoints = v; });
            wireTextInt(PurgeIntervalPcsTextViewModel, () => _data?.PurgeGlueParameters?.PurgeInterval_Pcs ?? 0, v => { ensureNested(); _data.PurgeGlueParameters.PurgeInterval_Pcs = v; });
            wireTextInt(PurgeIntervalMinPcsTextViewModel, () => _data?.PurgeGlueParameters?.PurgeIntervalMinPcs ?? 0, v => { ensureNested(); _data.PurgeGlueParameters.PurgeIntervalMinPcs = v; });
            wireTextInt(PurgeIntervalTimeSTextViewModel, () => _data?.PurgeGlueParameters?.PurgeIntervalTimeS ?? 0, v => { ensureNested(); _data.PurgeGlueParameters.PurgeIntervalTimeS = v; });
            wireTextDouble(PurgeGlueSpaceTextViewModel, () => _data?.PurgeGlueParameters?.PurgeGlueSpace ?? 0, v => { ensureNested(); _data.PurgeGlueParameters.PurgeGlueSpace = v; });
            wireTextInt(PurgeGlueXCountTextViewModel, () => _data?.PurgeGlueParameters?.PurgeGlueXCount ?? 0, v => { ensureNested(); _data.PurgeGlueParameters.PurgeGlueXCount = v; });
            wireTextInt(PurgeGlueYCountTextViewModel, () => _data?.PurgeGlueParameters?.PurgeGlueYCount ?? 0, v => { ensureNested(); _data.PurgeGlueParameters.PurgeGlueYCount = v; });
            wireTextInt(PurgeGlueCurrentRunIndexTextViewModel, () => _data?.PurgeGlueParameters?.PurgeGlueCurrentRunIndex ?? 0, v => { ensureNested(); _data.PurgeGlueParameters.PurgeGlueCurrentRunIndex = v; });
            wireTextInt(PurgeGlueSetLoopTimesTextViewModel, () => _data?.PurgeGlueParameters?.PurgeGlueSetLoopTimes ?? 0, v => { ensureNested(); _data.PurgeGlueParameters.PurgeGlueSetLoopTimes = v; });
            wireTextInt(PurgeGlueRealLoopTimesTextViewModel, () => _data?.PurgeGlueParameters?.PurgeGlueRealLoopTimes ?? 0, v => { ensureNested(); _data.PurgeGlueParameters.PurgeGlueRealLoopTimes = v; });
            wireTextInt(ToAndFroNumTextViewModel, () => _data?.PurgeGlueParameters?.ToAndFroNum ?? 0, v => { ensureNested(); _data.PurgeGlueParameters.ToAndFroNum = v; });
            wireTextDouble(ToAndFroDistanceTextViewModel, () => _data?.PurgeGlueParameters?.ToAndFroDistance ?? 0, v => { ensureNested(); _data.PurgeGlueParameters.ToAndFroDistance = v; });
            wireTextDouble(ToAndFroSpeedTextViewModel, () => _data?.PurgeGlueParameters?.ToAndFroSpeed ?? 0, v => { ensureNested(); _data.PurgeGlueParameters.ToAndFroSpeed = v; });
            wireTextDouble(ToAndFroAccSpeedTextViewModel, () => _data?.PurgeGlueParameters?.ToAndFroAccSpeed ?? 0, v => { ensureNested(); _data.PurgeGlueParameters.ToAndFroAccSpeed = v; });
            wireTextInt(ActualvalueBoardNumTextViewModel, () => _data?.PurgeGlueParameters?.ActualvalueBoardNum ?? 0, v => { ensureNested(); _data.PurgeGlueParameters.ActualvalueBoardNum = v; });
            wireTextInt(ScraperDirectionTextViewModel, () => _data?.PurgeGlueParameters?.ScraperDirection ?? 0, v => { ensureNested(); _data.PurgeGlueParameters.ScraperDirection = v; });

            wireTextInt(SpitGluePointsTextViewModel, () => _data?.CleanGlueParameters?.SpitGluePoints ?? 0, v => { ensureNested(); _data.CleanGlueParameters.SpitGluePoints = v; });
            wireTextInt(VacuumTimeSTextViewModel, () => _data?.CleanGlueParameters?.VacuumTimeS ?? 0, v => { ensureNested(); _data.CleanGlueParameters.VacuumTimeS = v; });
            wireTextInt(CleanIntervalPcsTextViewModel, () => _data?.CleanGlueParameters?.CleanIntervalPcs ?? 0, v => { ensureNested(); _data.CleanGlueParameters.CleanIntervalPcs = v; });
            wireTextInt(CleanIntervalMinPcsTextViewModel, () => _data?.CleanGlueParameters?.CleanIntervalMinPcs ?? 0, v => { ensureNested(); _data.CleanGlueParameters.CleanIntervalMinPcs = v; });
            wireTextInt(CleanIntervalTimeSTextViewModel, () => _data?.CleanGlueParameters?.CleanIntervalTimeS ?? 0, v => { ensureNested(); _data.CleanGlueParameters.CleanIntervalTimeS = v; });

            Init(new CleanParameters());
        }

        public void Init(CleanParameters data)
        {
            _data = data ?? new CleanParameters();
            ensureNested();

            CleanSwitchViewModel.IsChecked = _data.CleanGlueParameters.Enable;
            WipeSwitchViewModel.IsChecked = _data.WipeParameters.Enable;
            PurgeSwitchViewModel.IsChecked = _data.PurgeGlueParameters.Enable;
            StopMachinePurgeSwitchViewModel.IsChecked = _data.PurgeGlueParameters.StopMachinePurgeOpen;

            ToAndFroMoveSwitchViewModel.IsChecked = _data.PurgeGlueParameters.ToAndFroMove;

            WipeOpenForArraySwitchViewModel.IsChecked = _data.WipeParameters.WipeOpenForArray;
            CheckCleanPaperSwitchViewModel.IsChecked = _data.WipeParameters.CheckCleanPaper;
            ReversalSwitchViewModel.IsChecked = _data.WipeParameters.Reversal;

            PurgeGlueOpenForArraySwitchViewModel.IsChecked = _data.PurgeGlueParameters.PurgeGlueOpenForArray;

            FunctionHeadIdTextViewModel.Text = _data.FunctionHeadId.ToString();
            CleanCountTextViewModel.Text = _data.CleanCount.ToString();
            CCDToValveDistanceXTextViewModel.Text = _data.CCDToValveDistanceX.ToString();
            CCDToValveDistanceYTextViewModel.Text = _data.CCDToValveDistanceY.ToString();

            WipePosXTextViewModel.Text = _data.WipeParameters.Position.X.ToString();
            WipePosYTextViewModel.Text = _data.WipeParameters.Position.Y.ToString();
            WipePosZTextViewModel.Text = _data.WipeParameters.Position.Z.ToString();

            WipeCylinderCountTextViewModel.Text = _data.WipeParameters.WipeCylinderCount.ToString();
            RollPaperCountTextViewModel.Text = _data.WipeParameters.RollPaperCount.ToString();
            RollPaperSpeedTextViewModel.Text = _data.WipeParameters.RollPaperSpeed.ToString();
            CleanRadiusTextViewModel.Text = _data.WipeParameters.CleanRadius.ToString();
            WipeSpeedTextViewModel.Text = _data.WipeParameters.WipeSpeed.ToString();
            WipeSpaceTextViewModel.Text = _data.WipeParameters.WipeSpace.ToString();
            XCountTextViewModel.Text = _data.WipeParameters.XCount.ToString();
            YCountTextViewModel.Text = _data.WipeParameters.YCount.ToString();
            CurrentRunIndexTextViewModel.Text = _data.WipeParameters.CurrentRunIndex.ToString();
            SetLoopTimesTextViewModel.Text = _data.WipeParameters.SetLoopTimes.ToString();
            RealLoopTimesTextViewModel.Text = _data.WipeParameters.RealLoopTimes.ToString();
            ValveBackAndForthTimesTextViewModel.Text = _data.WipeParameters.ValveBackAndForthTimes.ToString();
            ValveBackAndForthDisTextViewModel.Text = _data.WipeParameters.ValveBackAndForthDis.ToString();
            BrushRunTimeTextViewModel.Text = _data.WipeParameters.BrushRunTime.ToString();
            CenterWipeTimeTextViewModel.Text = _data.WipeParameters.CenterWipeTime.ToString();

            PurgePosXTextViewModel.Text = _data.PurgeGlueParameters.Position.X.ToString();
            PurgePosYTextViewModel.Text = _data.PurgeGlueParameters.Position.Y.ToString();
            PurgePosZTextViewModel.Text = _data.PurgeGlueParameters.Position.Z.ToString();

            PurgeGluePointsTextViewModel.Text = _data.PurgeGlueParameters.PurgeGluePoints.ToString();
            PurgeIntervalPcsTextViewModel.Text = _data.PurgeGlueParameters.PurgeInterval_Pcs.ToString();
            PurgeIntervalMinPcsTextViewModel.Text = _data.PurgeGlueParameters.PurgeIntervalMinPcs.ToString();
            PurgeIntervalTimeSTextViewModel.Text = _data.PurgeGlueParameters.PurgeIntervalTimeS.ToString();
            PurgeGlueSpaceTextViewModel.Text = _data.PurgeGlueParameters.PurgeGlueSpace.ToString();
            PurgeGlueXCountTextViewModel.Text = _data.PurgeGlueParameters.PurgeGlueXCount.ToString();
            PurgeGlueYCountTextViewModel.Text = _data.PurgeGlueParameters.PurgeGlueYCount.ToString();
            PurgeGlueCurrentRunIndexTextViewModel.Text = _data.PurgeGlueParameters.PurgeGlueCurrentRunIndex.ToString();
            PurgeGlueSetLoopTimesTextViewModel.Text = _data.PurgeGlueParameters.PurgeGlueSetLoopTimes.ToString();
            PurgeGlueRealLoopTimesTextViewModel.Text = _data.PurgeGlueParameters.PurgeGlueRealLoopTimes.ToString();
            ToAndFroNumTextViewModel.Text = _data.PurgeGlueParameters.ToAndFroNum.ToString();
            ToAndFroDistanceTextViewModel.Text = _data.PurgeGlueParameters.ToAndFroDistance.ToString();
            ToAndFroSpeedTextViewModel.Text = _data.PurgeGlueParameters.ToAndFroSpeed.ToString();
            ToAndFroAccSpeedTextViewModel.Text = _data.PurgeGlueParameters.ToAndFroAccSpeed.ToString();
            ActualvalueBoardNumTextViewModel.Text = _data.PurgeGlueParameters.ActualvalueBoardNum.ToString();
            ScraperDirectionTextViewModel.Text = _data.PurgeGlueParameters.ScraperDirection.ToString();

            CleanPosXTextViewModel.Text = _data.CleanGlueParameters.Position.X.ToString();
            CleanPosYTextViewModel.Text = _data.CleanGlueParameters.Position.Y.ToString();
            CleanPosZTextViewModel.Text = _data.CleanGlueParameters.Position.Z.ToString();

            SpitGluePointsTextViewModel.Text = _data.CleanGlueParameters.SpitGluePoints.ToString();
            VacuumTimeSTextViewModel.Text = _data.CleanGlueParameters.VacuumTimeS.ToString();
            CleanIntervalPcsTextViewModel.Text = _data.CleanGlueParameters.CleanIntervalPcs.ToString();
            CleanIntervalMinPcsTextViewModel.Text = _data.CleanGlueParameters.CleanIntervalMinPcs.ToString();
            CleanIntervalTimeSTextViewModel.Text = _data.CleanGlueParameters.CleanIntervalTimeS.ToString();

            var wipeSrc = WipeModeComboxViewModel.ItemsSource as object[];
            var wipeSel = wipeSrc?.OfType<WipeModeOption>().FirstOrDefault(x => x.Value == _data.WipeParameters.WipeMode)
                         ?? wipeSrc?.OfType<WipeModeOption>().FirstOrDefault();
            if (wipeSel != null)
                WipeModeComboxViewModel.SelectedItem = wipeSel;

            var teachSrc = WipeTeachPosModeComboxViewModel.ItemsSource as object[];
            var teachSel = teachSrc?.OfType<TeachPosModeOption>().FirstOrDefault(x => x.Value == _data.WipeParameters.ETeachCleanPosMode)
                           ?? teachSrc?.OfType<TeachPosModeOption>().FirstOrDefault();
            if (teachSel != null)
                WipeTeachPosModeComboxViewModel.SelectedItem = teachSel;

            var purgeTeachSrc = PurgeTeachPosModeComboxViewModel.ItemsSource as object[];
            var purgeTeachSel = purgeTeachSrc?.OfType<TeachPosModeOption>().FirstOrDefault(x => x.Value == _data.PurgeGlueParameters.ETeachCleanPosMode)
                                ?? purgeTeachSrc?.OfType<TeachPosModeOption>().FirstOrDefault();
            if (purgeTeachSel != null)
                PurgeTeachPosModeComboxViewModel.SelectedItem = purgeTeachSel;

            var cleanModeSrc = CleanModeComboxViewModel.ItemsSource as object[];
            var cleanModeSel = cleanModeSrc?.OfType<CleanModeOption>().FirstOrDefault(x => x.Value == _data.CleanGlueParameters.ECleanMode)
                              ?? cleanModeSrc?.OfType<CleanModeOption>().FirstOrDefault();
            if (cleanModeSel != null)
                CleanModeComboxViewModel.SelectedItem = cleanModeSel;

            var cleanTeachSrc = CleanTeachPosModeComboxViewModel.ItemsSource as object[];
            var cleanTeachSel = cleanTeachSrc?.OfType<TeachPosModeOption>().FirstOrDefault(x => x.Value == _data.CleanGlueParameters.ETeachCleanPosMode)
                                ?? cleanTeachSrc?.OfType<TeachPosModeOption>().FirstOrDefault();
            if (cleanTeachSel != null)
                CleanTeachPosModeComboxViewModel.SelectedItem = cleanTeachSel;

            var src = CleanOrderComboxViewModel.ItemsSource as object[];
            var selected = src?.OfType<CleanOrderOption>().FirstOrDefault(x => x.Value == _data.ECleanOrder)
                           ?? src?.OfType<CleanOrderOption>().FirstOrDefault();
            if (selected != null)
                CleanOrderComboxViewModel.SelectedItem = selected;
        }

        public CleanParameters GetData()
        {
            ensureData();
            ensureNested();
            if (CleanOrderComboxViewModel.SelectedItem is CleanOrderOption opt)
                _data.ECleanOrder = opt.Value;
            return _data;
        }

        private void wireToggle(ToggleSwitchViewModel vm, Func<bool> getter, Action<bool> setter)
        {
            vm.ValueChanged += (s, e) =>
            {
                if (e.NewValue is bool b)
                {
                    setter(b);
                    AfterModified?.Invoke(this, EventArgs.Empty);
                }
            };

            vm.IsChecked = getter();
        }

        private void wireTextInt(TextInputViewModel vm, Func<int> getter, Action<int> setter)
        {
            vm.ValueChanged += (s, e) =>
            {
                if (e.NewValue is string text && int.TryParse(text, out var v))
                {
                    setter(v);
                    AfterModified?.Invoke(this, EventArgs.Empty);
                }
            };

            vm.Text = getter().ToString();
        }

        private void wireTextDouble(TextInputViewModel vm, Func<double> getter, Action<double> setter)
        {
            vm.ValueChanged += (s, e) =>
            {
                if (e.NewValue is string text && double.TryParse(text, out var v))
                {
                    setter(v);
                    AfterModified?.Invoke(this, EventArgs.Empty);
                }
            };

            vm.Text = getter().ToString();
        }

        private void ensureData()
        {
            _data ??= new CleanParameters();
        }

        private void ensureNested()
        {
            ensureData();
            _data.WipeParameters ??= new WipeParameters();
            _data.PurgeGlueParameters ??= new PurgeGlueParameters();
            _data.CleanGlueParameters ??= new CleanGlueParameters();

            _data.WipeParameters.Position ??= new Point3D();
            _data.PurgeGlueParameters.Position ??= new Point3D();
            _data.CleanGlueParameters.Position ??= new Point3D();
        }

        public class TeachPosModeOption
        {
            public EnumTeachCleanPosMode Value { get; }
            public string DisplayName { get; }

            public TeachPosModeOption(EnumTeachCleanPosMode value, string displayName)
            {
                Value = value;
                DisplayName = displayName;
            }
        }

        public class CleanOrderOption
        {
            public EnumCleanOrder Value { get; }
            public string DisplayName { get; }

            public CleanOrderOption(EnumCleanOrder value, string displayName)
            {
                Value = value;
                DisplayName = displayName;
            }
        }

        public class WipeModeOption
        {
            public EnumWipeMode Value { get; }
            public string DisplayName { get; }

            public WipeModeOption(EnumWipeMode value, string displayName)
            {
                Value = value;
                DisplayName = displayName;
            }
        }

        public class CleanModeOption
        {
            public EnumCleanMode Value { get; }
            public string DisplayName { get; }

            public CleanModeOption(EnumCleanMode value, string displayName)
            {
                Value = value;
                DisplayName = displayName;
            }
        }
    }
}
