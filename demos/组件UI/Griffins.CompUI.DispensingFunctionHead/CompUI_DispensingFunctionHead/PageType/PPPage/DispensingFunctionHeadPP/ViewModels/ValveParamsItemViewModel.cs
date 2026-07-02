using GKG.ElectronicControl.Dispenser;
using GKG.UI;
using ReactiveUI;
using System;
using System.Globalization;

namespace Griffins.CompUI.DispensingFunctionHead.CompUI_DispensingFunctionHead.PageType.PPPage.DispensingFunctionHeadPP.ViewModels
{
    /// <summary>
    /// 阀参数项ViewModel
    /// 用于DataGrid的单行数据绑定
    /// </summary>
    public class ValveParamsItemViewModel : ReactiveObject
    {
        private int _index;
        private string _upriseTimeMs;
        private string _dispenseTimeMs;
        private string _impactTimeMs;
        private string _intervalTimeMs;
        private string _pressureControlParam;
        private string _pointCount;
        private string _dispenseMode;
        private string _startStopState;

        /// <summary>序号</summary>
        public int Index
        {
            get => _index;
            set => this.RaiseAndSetIfChanged(ref _index, value);
        }

        /// <summary>上升时间(ms)</summary>
        public string UpriseTimeMs
        {
            get => _upriseTimeMs;
            set => this.RaiseAndSetIfChanged(ref _upriseTimeMs, value);
        }

        /// <summary>点胶时间(ms)</summary>
        public string DispenseTimeMs
        {
            get => _dispenseTimeMs;
            set => this.RaiseAndSetIfChanged(ref _dispenseTimeMs, value);
        }

        /// <summary>撞击时间(ms)</summary>
        public string ImpactTimeMs
        {
            get => _impactTimeMs;
            set => this.RaiseAndSetIfChanged(ref _impactTimeMs, value);
        }

        /// <summary>间歇时间(ms)</summary>
        public string IntervalTimeMs
        {
            get => _intervalTimeMs;
            set => this.RaiseAndSetIfChanged(ref _intervalTimeMs, value);
        }

        /// <summary>电压比(%)</summary>
        public string PressureControlParam
        {
            get => _pressureControlParam;
            set => this.RaiseAndSetIfChanged(ref _pressureControlParam, value);
        }

        /// <summary>打点数量</summary>
        public string PointCount
        {
            get => _pointCount;
            set => this.RaiseAndSetIfChanged(ref _pointCount, value);
        }

        /// <summary>点胶模式</summary>
        public string DispenseMode
        {
            get => _dispenseMode;
            set => this.RaiseAndSetIfChanged(ref _dispenseMode, value);
        }

        /// <summary>启停状态</summary>
        public string StartStopState
        {
            get => _startStopState;
            set => this.RaiseAndSetIfChanged(ref _startStopState, value);
        }

        public ValveParamsItemViewModel()
        {
            _upriseTimeMs = "0";
            _dispenseTimeMs = "0";
            _impactTimeMs = "0";
            _intervalTimeMs = "0";
            _pressureControlParam = "0";
            _pointCount = "0";
            _dispenseMode = "0";
            _startStopState = "0";
        }

        /// <summary>
        /// 从数据模型复制
        /// </summary>
        public void CopyFrom(GKGPiezoValveParams source, int index)
        {
            if (source == null) return;

            Index = index;
            UpriseTimeMs = source.UpriseTimeMs.ToString(CultureInfo.InvariantCulture);
            DispenseTimeMs = source.DispenseTimeMs.ToString(CultureInfo.InvariantCulture);
            ImpactTimeMs = source.ImpactTimeMs.ToString(CultureInfo.InvariantCulture);
            IntervalTimeMs = source.IntervalTimeMs.ToString(CultureInfo.InvariantCulture);
            PressureControlParam = source.PressureControlParam.ToString(CultureInfo.InvariantCulture);
            PointCount = source.PointCount.ToString(CultureInfo.InvariantCulture);
            DispenseMode = source.DispenseMode.ToString(CultureInfo.InvariantCulture);
            StartStopState = source.StartStopState.ToString(CultureInfo.InvariantCulture);
        }

        public void CopyEditableFieldsFrom(ValveParamsItemViewModel source)
        {
            if (source == null) return;

            UpriseTimeMs = source.UpriseTimeMs;
            DispenseTimeMs = source.DispenseTimeMs;
            ImpactTimeMs = source.ImpactTimeMs;
            IntervalTimeMs = source.IntervalTimeMs;
            PressureControlParam = source.PressureControlParam;
            PointCount = source.PointCount;
            DispenseMode = source.DispenseMode;
            StartStopState = source.StartStopState;
        }

        public ValveParamsItemViewModel CloneForEdit()
        {
            var copy = new ValveParamsItemViewModel
            {
                Index = Index
            };
            copy.CopyEditableFieldsFrom(this);
            return copy;
        }

        /// <summary>
        /// 复制到数据模型
        /// </summary>
        public GKGPiezoValveParams ToModel()
        {
            return new GKGPiezoValveParams
            {
                UpriseTimeMs = ParseDoubleOrDefault(UpriseTimeMs, 0),
                DispenseTimeMs = ParseDoubleOrDefault(DispenseTimeMs, 0),
                ImpactTimeMs = ParseDoubleOrDefault(ImpactTimeMs, 0),
                IntervalTimeMs = ParseDoubleOrDefault(IntervalTimeMs, 0),
                PressureControlParam = ParseIntOrDefault(PressureControlParam, 0),
                PointCount = ParseIntOrDefault(PointCount, 0),
                DispenseMode = ParseIntOrDefault(DispenseMode, 0),
                StartStopState = ParseIntOrDefault(StartStopState, 0)
            };
        }

        private static double ParseDoubleOrDefault(string text, double defaultValue)
        {
            return double.TryParse(
                text,
                NumberStyles.Float | NumberStyles.AllowThousands,
                CultureInfo.InvariantCulture,
                out var value)
                ? value
                : defaultValue;
        }

        private static int ParseIntOrDefault(string text, int defaultValue)
        {
            return int.TryParse(
                text,
                NumberStyles.Integer,
                CultureInfo.InvariantCulture,
                out var value)
                ? value
                : defaultValue;
        }
    }
}
