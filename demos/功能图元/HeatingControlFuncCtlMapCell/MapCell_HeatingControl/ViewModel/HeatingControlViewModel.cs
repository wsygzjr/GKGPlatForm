using Avalonia;
using Avalonia.Media;
using Griffins.UI2;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive;

namespace GKG.Map.HeatingControlFuncCtlMapCell.ViewModel
{
    public class HeatingControlViewModel : ReactiveObject, IDisposable
    {
        private readonly Dictionary<string, HeatingModuleInfo> _modules;

        private FontInfo _textFont;
        public FontInfo TextFont
        {
            get { return _textFont; }
            set { this.RaiseAndSetIfChanged(ref _textFont, value); }
        }

        private Color _textColor;
        /// <summary>
        /// 文本颜色
        /// </summary>
        public Color TextColor
        {
            get { return _textColor; }
            set { this.RaiseAndSetIfChanged(ref _textColor, value); }
        }

        private Color _backColor;
        /// <summary>
        /// 背景颜色
        /// </summary>
        public Color BackColor
        {
            get { return _backColor; }
            set { this.RaiseAndSetIfChanged(ref _backColor, value); }
        }

        private bool _isDualValve;
        public bool IsDualValve
        {
            get { return _isDualValve; }
            set
            {
                if (this.RaiseAndSetIfChanged(ref _isDualValve, value))
                {
                    UpdateHeatingModules();
                }
            }
        }

        private bool _isDualTrack;
        public bool IsDualTrack
        {
            get { return _isDualTrack; }
            set
            {
                if (this.RaiseAndSetIfChanged(ref _isDualTrack, value))
                {
                    UpdateHeatingModules();
                }
            }
        }

        private List<HeatingModuleInfo> _heatingModules;
        public List<HeatingModuleInfo> HeatingModules
        {
            get { return _heatingModules; }
            set { this.RaiseAndSetIfChanged(ref _heatingModules, value); }
        }

        public ReactiveCommand<Point, Unit> PointerPressedCommand { get; }

        private Action _clickAction;

        public HeatingControlViewModel(HeatingControlPropertyModelEdit heatingControlPropertyModelEdit, Action clickAction)
        {
            this.TextFont = heatingControlPropertyModelEdit.TextFont;
            this.TextColor = heatingControlPropertyModelEdit.TextColor;
            this.BackColor = heatingControlPropertyModelEdit.BackColor;
            this.IsDualValve = heatingControlPropertyModelEdit.IsDualValve;
            this.IsDualTrack = heatingControlPropertyModelEdit.IsDualTrack;

            PointerPressedCommand = ReactiveCommand.Create<Point>(OnPointerPressed);
            _clickAction = clickAction;

            _modules = new Dictionary<string, HeatingModuleInfo>(StringComparer.Ordinal);
            InitModulesFromModel(heatingControlPropertyModelEdit);
            HeatingModules = new List<HeatingModuleInfo>();
            UpdateHeatingModules();
        }

        internal void SetHeatingModule(string modulePropName, HeatingModuleInfo module)
        {
            if (string.IsNullOrWhiteSpace(modulePropName))
                return;

            if (module == null)
            {
                _modules.Remove(modulePropName);
            }
            else
            {
                _modules[modulePropName] = module;
            }
            UpdateHeatingModules();
        }

        private void InitModulesFromModel(HeatingControlPropertyModelEdit model)
        {
            if (model == null)
                return;

            if (model.RightValveDispensingHead != null) _modules[nameof(HeatingControlPropertyModelEdit.RightValveDispensingHead)] = model.RightValveDispensingHead;
            if (model.RightValveCartridgeHeating != null) _modules[nameof(HeatingControlPropertyModelEdit.RightValveCartridgeHeating)] = model.RightValveCartridgeHeating;
            if (model.LeftValveDispensingHead != null) _modules[nameof(HeatingControlPropertyModelEdit.LeftValveDispensingHead)] = model.LeftValveDispensingHead;
            if (model.LeftValveCartridgeHeating != null) _modules[nameof(HeatingControlPropertyModelEdit.LeftValveCartridgeHeating)] = model.LeftValveCartridgeHeating;

            if (model.ARailPreheatLeft != null) _modules[nameof(HeatingControlPropertyModelEdit.ARailPreheatLeft)] = model.ARailPreheatLeft;
            if (model.ARailPreheatLeft2 != null) _modules[nameof(HeatingControlPropertyModelEdit.ARailPreheatLeft2)] = model.ARailPreheatLeft2;
            if (model.ARailGlueBoardStationMiddle != null) _modules[nameof(HeatingControlPropertyModelEdit.ARailGlueBoardStationMiddle)] = model.ARailGlueBoardStationMiddle;
            if (model.ARailDispensingStationMiddle2 != null) _modules[nameof(HeatingControlPropertyModelEdit.ARailDispensingStationMiddle2)] = model.ARailDispensingStationMiddle2;
            if (model.ARailPreheatRight != null) _modules[nameof(HeatingControlPropertyModelEdit.ARailPreheatRight)] = model.ARailPreheatRight;
            if (model.ARailPreheatRight2 != null) _modules[nameof(HeatingControlPropertyModelEdit.ARailPreheatRight2)] = model.ARailPreheatRight2;

            if (model.BRailPreheatLeft != null) _modules[nameof(HeatingControlPropertyModelEdit.BRailPreheatLeft)] = model.BRailPreheatLeft;
            if (model.BRailPreheatLeft2 != null) _modules[nameof(HeatingControlPropertyModelEdit.BRailPreheatLeft2)] = model.BRailPreheatLeft2;
            if (model.BRailGlueBoardStationMiddle != null) _modules[nameof(HeatingControlPropertyModelEdit.BRailGlueBoardStationMiddle)] = model.BRailGlueBoardStationMiddle;
            if (model.BRailDispensingStationMiddle2 != null) _modules[nameof(HeatingControlPropertyModelEdit.BRailDispensingStationMiddle2)] = model.BRailDispensingStationMiddle2;
            if (model.BRailPreheatRight != null) _modules[nameof(HeatingControlPropertyModelEdit.BRailPreheatRight)] = model.BRailPreheatRight;
            if (model.BRailPreheatRight2 != null) _modules[nameof(HeatingControlPropertyModelEdit.BRailPreheatRight2)] = model.BRailPreheatRight2;
        }

        internal void UpdateHeatingModules()
        {
            var modules = new List<HeatingModuleInfo>();

            // 右阀模块始终显示
            if (_modules.TryGetValue(nameof(HeatingControlPropertyModelEdit.RightValveDispensingHead), out var rightDispensingHead) && rightDispensingHead != null) modules.Add(rightDispensingHead);
            if (_modules.TryGetValue(nameof(HeatingControlPropertyModelEdit.RightValveCartridgeHeating), out var rightCartridgeHeating) && rightCartridgeHeating != null) modules.Add(rightCartridgeHeating);

            // 双阀模式下显示左阀模块
            if (IsDualValve)
            {
                if (_modules.TryGetValue(nameof(HeatingControlPropertyModelEdit.LeftValveDispensingHead), out var leftDispensingHead) && leftDispensingHead != null) modules.Add(leftDispensingHead);
                if (_modules.TryGetValue(nameof(HeatingControlPropertyModelEdit.LeftValveCartridgeHeating), out var leftCartridgeHeating) && leftCartridgeHeating != null) modules.Add(leftCartridgeHeating);
            }

            // 其他轨道模块始终显示
            if (_modules.TryGetValue(nameof(HeatingControlPropertyModelEdit.ARailPreheatLeft), out var aRailPreheatLeft) && aRailPreheatLeft != null) modules.Add(aRailPreheatLeft);
            if (_modules.TryGetValue(nameof(HeatingControlPropertyModelEdit.ARailPreheatLeft2), out var aRailPreheatLeft2) && aRailPreheatLeft2 != null) modules.Add(aRailPreheatLeft2);
            if (_modules.TryGetValue(nameof(HeatingControlPropertyModelEdit.ARailGlueBoardStationMiddle), out var aRailGlueBoardStationMiddle) && aRailGlueBoardStationMiddle != null) modules.Add(aRailGlueBoardStationMiddle);
            if (_modules.TryGetValue(nameof(HeatingControlPropertyModelEdit.ARailDispensingStationMiddle2), out var aRailDispensingStationMiddle2) && aRailDispensingStationMiddle2 != null) modules.Add(aRailDispensingStationMiddle2);
            if (_modules.TryGetValue(nameof(HeatingControlPropertyModelEdit.ARailPreheatRight), out var aRailPreheatRight) && aRailPreheatRight != null) modules.Add(aRailPreheatRight);
            if (_modules.TryGetValue(nameof(HeatingControlPropertyModelEdit.ARailPreheatRight2), out var aRailPreheatRight2) && aRailPreheatRight2 != null) modules.Add(aRailPreheatRight2);

            if (IsDualTrack)
            {
                if (_modules.TryGetValue(nameof(HeatingControlPropertyModelEdit.BRailPreheatLeft), out var bRailPreheatLeft) && bRailPreheatLeft != null) modules.Add(bRailPreheatLeft);
                if (_modules.TryGetValue(nameof(HeatingControlPropertyModelEdit.BRailPreheatLeft2), out var bRailPreheatLeft2) && bRailPreheatLeft2 != null) modules.Add(bRailPreheatLeft2);
                if (_modules.TryGetValue(nameof(HeatingControlPropertyModelEdit.BRailGlueBoardStationMiddle), out var bRailGlueBoardStationMiddle) && bRailGlueBoardStationMiddle != null) modules.Add(bRailGlueBoardStationMiddle);
                if (_modules.TryGetValue(nameof(HeatingControlPropertyModelEdit.BRailDispensingStationMiddle2), out var bRailDispensingStationMiddle2) && bRailDispensingStationMiddle2 != null) modules.Add(bRailDispensingStationMiddle2);
                if (_modules.TryGetValue(nameof(HeatingControlPropertyModelEdit.BRailPreheatRight), out var bRailPreheatRight) && bRailPreheatRight != null) modules.Add(bRailPreheatRight);
                if (_modules.TryGetValue(nameof(HeatingControlPropertyModelEdit.BRailPreheatRight2), out var bRailPreheatRight2) && bRailPreheatRight2 != null) modules.Add(bRailPreheatRight2);
            }

            HeatingModules = modules;
        }

        private void OnPointerPressed(Point screenP)
        {
            _clickAction?.Invoke();
        }

        public void Dispose()
        {

        }
    }
}
