using GF_Gereric;
using Avalonia.Controls;
using GKG;
using GKG.SubMM;
using Griffins.CompUI.EletronicManager.CompUI_EletronicManager.Models;
using Griffins.CompUI.EletronicManager.CompUI_EletronicManager.PageType.DebugPage.ViewModels;
using Griffins.CompUI.EletronicManager.CompUI_EletronicManager.PageType.DebugPage.Views;
using Griffins.Map.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Griffins.CompUI.EletronicManager.CompUI_EletronicManager.PageType.DebugPage
{
    public enum AxisDebugInnerSubPageMode
    {
        AxisDebug,
        IOIn,
        IOOut,
    }

    public class AxisDebugInnerSubPageRunTime : IInnerSubPageRunTime
    {
        private AxisDebugInnerSubPageDesignCfg? _designCfgInfo;
        private ICompUIRunTimeCallBack? _callBack;
        private EventHandler? _afterDataModified;
        private RuntimeFactoryCfg? _factoryCfg;
        private byte[] _rawData = Array.Empty<byte>();
        private readonly AxisDebugInnerSubPageMode _pageMode;
        private AxisDebugWindowViewModel? _viewModel;

        public AxisDebugInnerSubPageRunTime(AxisDebugInnerSubPageMode pageMode)
        {
            _pageMode = pageMode;
        }

        public void Init(ICompUIRunTimeCallBack callBack)
        {
            _callBack = callBack;
        }

        void ISubPageRunTime.Init(byte[] viewCfgInfo)
        {
            SetCfgInfo(viewCfgInfo);
        }

        object ISubPageRunTime.View
        {
            get
            {
                _viewModel ??= CreateViewModel();

                Control view = _pageMode switch
                {
                    AxisDebugInnerSubPageMode.AxisDebug => new AxisDebugMotionView(),
                    AxisDebugInnerSubPageMode.IOIn => new AxisDebugIOInView(),
                    _ => new AxisDebugMotionView(),
                };

                view.DataContext = _viewModel;
                return view;
            }
        }

        public void StopPolling()
        {
            _viewModel?.Cleanup();
        }

        void ISubPageRunTime.SetViewCfgInfo(byte[] viewCfgInfo)
        {
            SetCfgInfo(viewCfgInfo);
        }

        private void SetCfgInfo(byte[] viewCfgInfo)
        {
            if (viewCfgInfo != null && viewCfgInfo.Length > 0)
            {
                _designCfgInfo = JsonObjConvert.FromJSonBytes<AxisDebugInnerSubPageDesignCfg>(viewCfgInfo);
            }
        }

        event EventHandler IInnerSubPageRunTime.AfterModified
        {
            add => _afterDataModified += value;
            remove => _afterDataModified -= value;
        }

        void IInnerSubPageRunTime.OnInit()
        {
        }

        void IInnerSubPageRunTime.SetData(byte[] data)
        {
            _rawData = data ?? Array.Empty<byte>();

            if (TryLoadFactoryCfgFromBackend())
            {
                return;
            }

            if (_rawData.Length == 0)
            {
                _factoryCfg = new RuntimeFactoryCfg();
                return;
            }

            _factoryCfg = JsonObjConvert.FromJSonBytes<RuntimeFactoryCfg>(_rawData)
                ?? new RuntimeFactoryCfg();
        }

        private bool TryLoadFactoryCfgFromBackend()
        {
            if (_callBack == null)
                return false;

            var result = _callBack.ExecConfigSvrCtlCmd(EletronicManagerSubMachineModulesConst.RtCmdGetElectricalFactoryCfg, new GFBaseTypeParamValueList());
            var factoryCfgJson = TryGetResultData(result);
            if (string.IsNullOrWhiteSpace(factoryCfgJson))
                return false;

            _factoryCfg = JsonObjConvert.FromJSon<RuntimeFactoryCfg>(factoryCfgJson);
            return _factoryCfg != null;
        }

        byte[] IInnerSubPageRunTime.GetData()
        {
            return _rawData;
        }

        private AxisDebugWindowViewModel CreateViewModel()
        {
            var axisDescriptors = ResolveAxisDescriptors();
            var ioDescriptors = ResolveIODeviceDescriptors();
            var defaultCardGuid = axisDescriptors.FirstOrDefault()?.CardGuid ?? Guid.Empty;
            var axisCount = axisDescriptors.Count > 0 ? axisDescriptors.Count : 1;
            return new AxisDebugWindowViewModel(_callBack, defaultCardGuid, axisCount, 0, axisDescriptors, ioDescriptors);
        }

        private IReadOnlyList<AxisDebugWindowViewModel.AxisDescriptor> ResolveAxisDescriptors()
        {
            var runtimeAxis = TryLoadAxisDescriptorsFromBackend();
            if (runtimeAxis.Count > 0)
                return runtimeAxis;

            var axisInfos = _factoryCfg?.AxisInformations ?? Array.Empty<AxisInformation>();
            var cardOrder = BuildCardOrderLookup();
            var configuredAxis = axisInfos
                .Where(x => x != null)
                .Select((x, index) => new
                {
                    AxisInfo = x,
                    OriginalIndex = index,
                    CardOrder = cardOrder.TryGetValue(x.MotionCardGuid, out var order) ? order : int.MaxValue,
                    Order = x.AxisNo
                })
                .OrderBy(x => x.CardOrder)
                .ThenBy(x => x.Order)
                .ThenBy(x => x.OriginalIndex)
                .Select(x => new AxisDebugWindowViewModel.AxisDescriptor(
                    x.AxisInfo.MotionCardGuid,
                    x.AxisInfo.AxisNo,
                    string.IsNullOrWhiteSpace(x.AxisInfo.AxisName) ? $"Axis{x.AxisInfo.AxisNo}" : x.AxisInfo.AxisName.Trim()))
                .ToList();

            if (configuredAxis.Count > 0)
                return configuredAxis;

            var cards = _factoryCfg?.EletronicFactoryParameters?.MotionControlCardInformations;
            if (cards == null)
                return Array.Empty<AxisDebugWindowViewModel.AxisDescriptor>();

            return cards
                .Where(x => x != null)
                .SelectMany(card =>
                {
                    var axisCount = card.MotionControlFactoryParameters?.Parameters?.Length ?? 0;
                    return Enumerable.Range(0, axisCount)
                        .Select(i => new AxisDebugWindowViewModel.AxisDescriptor(card.MotionCardID, i, $"Axis{i}"));
                })
                .ToList();
        }

        private IReadOnlyList<AxisDebugWindowViewModel.AxisDescriptor> TryLoadAxisDescriptorsFromBackend()
        {
            if (_callBack == null)
                return Array.Empty<AxisDebugWindowViewModel.AxisDescriptor>();

            var result = _callBack.ExecConfigSvrCtlCmd(EletronicManagerSubMachineModulesConst.RtCmdGetAxisConfigList, new GFBaseTypeParamValueList());
            var json = TryGetResultData(result);
            if (string.IsNullOrWhiteSpace(json))
                return Array.Empty<AxisDebugWindowViewModel.AxisDescriptor>();

            var groupedAxisInfos = JsonObjConvert.FromJSon<List<List<AxisInformation>>>(json);
            IEnumerable<AxisInformation>? allAxisInfos = groupedAxisInfos?.SelectMany(x => x ?? Enumerable.Empty<AxisInformation>());
            if (allAxisInfos == null)
            {
                var flatAxisInfos = JsonObjConvert.FromJSon<List<AxisInformation>>(json);
                allAxisInfos = flatAxisInfos ?? Enumerable.Empty<AxisInformation>();
            }

            var cardOrder = BuildCardOrderLookup();
            return allAxisInfos
                .Where(x => x != null)
                .OrderBy(x => cardOrder.TryGetValue(x.MotionCardGuid, out var order) ? order : int.MaxValue)
                .ThenBy(x => x.AxisNo)
                .Select(x => new AxisDebugWindowViewModel.AxisDescriptor(
                    x.MotionCardGuid,
                    x.AxisNo,
                    string.IsNullOrWhiteSpace(x.AxisName) ? $"Axis{x.AxisNo}" : x.AxisName.Trim()))
                .ToList();
        }

        private IReadOnlyList<AxisDebugWindowViewModel.IoDescriptor> ResolveIODeviceDescriptors()
        {
            var ioInfos = _factoryCfg?.IOStateInformations ?? Array.Empty<IOStateInformation>();
            return ioInfos
                .Where(x => x != null)
                .Select((x, index) => new
                {
                    CardGuid = x.DeviceGuid,
                    Sequence = index,
                    ChannelNo = x.ChannelId,
                    Name = string.IsNullOrWhiteSpace(x.IOName) ? x.ChannelId ?? string.Empty : x.IOName.Trim(),
                    CanWrite = x.EReadWriteMode == EReadWriteMode.WriteOnly || x.EReadWriteMode == EReadWriteMode.ReadWrite
                })
                .Where(x => x.ChannelNo != null)
                .OrderBy(x => x.Sequence)
                .Select(x => new AxisDebugWindowViewModel.IoDescriptor(x.CardGuid, x.ChannelNo, x.Name, x.CanWrite))
                .ToList();
        }

        private Dictionary<Guid, int> BuildCardOrderLookup()
        {
            return (_factoryCfg?.EletronicFactoryParameters?.MotionControlCardInformations ?? new List<RuntimeMotionControlCardInformations>())
                .Where(x => x != null)
                .Select((x, index) => new { x.MotionCardID, Index = index })
                .GroupBy(x => x.MotionCardID)
                .ToDictionary(x => x.Key, x => x.First().Index);
        }

        private static int ParseChannelNo(string? channelId)
        {
            if (string.IsNullOrWhiteSpace(channelId))
                return -1;

            var digits = new string(channelId.Where(char.IsDigit).ToArray());
            return int.TryParse(digits, out var channelNo) ? channelNo : -1;
        }

        private static string? TryGetResultData(GFBaseTypeParamValueList? result)
        {
            if (result == null)
                return null;

            return result["data"]?.ToString();
        }

        public bool CheckDataValid(out string[] inValidMsg)
        {
            throw new NotImplementedException();
        }

        private sealed class RuntimeFactoryCfg
        {
            public AxisInformation[]? AxisInformations { get; set; }

            public IOStateInformation[]? IOStateInformations { get; set; }

            public RuntimeEletronicFactoryParameters? EletronicFactoryParameters { get; set; }
        }

        private sealed class RuntimeEletronicFactoryParameters
        {
            public List<RuntimeMotionControlCardInformations>? MotionControlCardInformations { get; set; }
        }

        private sealed class RuntimeMotionControlCardInformations
        {
            public Guid MotionCardID { get; set; } = Guid.Empty;

            public MotionControlFactoryParameters? MotionControlFactoryParameters { get; set; }
        }
    }

    public class AxisDebugInnerSubPageDesignCfg
    {
        public int CardIndex { get; set; } = 0;

        public string MotionCardCustomID { get; set; } = string.Empty;
    }
}
