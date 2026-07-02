using Avalonia.Controls;
using GKG.UI;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GKG.UI.General
{
    public class CylinderConfigViewModel : ReactiveObject
    {
        public ComboxViewModel ConfigTypeViewModel { get; }

        public SingleControlSingleLimitViewModel SingleControlSingleLimitViewModel { get; }

        public SingleControlDoubleLimitViewModel SingleControlDoubleLimitViewModel { get; }

        public DoubleControlSingleLimitViewModel DoubleControlSingleLimitViewModel { get; }

        public DoubleControlDoubleLimitViewModel DoubleControlDoubleLimitViewModel { get; }

        private ECylinderType _selectedConfigType;
        public ECylinderType SelectedConfigType
        {
            get => _selectedConfigType;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedConfigType, value);
                this.RaisePropertyChanged(nameof(CurrentLimitViewModel));
            }
        }

        public object CurrentLimitViewModel
        {
            get
            {
                return SelectedConfigType switch
                {
                    ECylinderType.SingleActSingleLimit => SingleControlSingleLimitViewModel,
                    ECylinderType.SingleActDoubleLimit => SingleControlDoubleLimitViewModel,
                    ECylinderType.DoubleActSingleLimit => DoubleControlSingleLimitViewModel,
                    ECylinderType.DoubleActDoubleLimit => DoubleControlDoubleLimitViewModel,
                    _ => SingleControlSingleLimitViewModel,
                };
            }
        }

        public event EventHandler? AfterModified;

        public CylinderConfigViewModel()
        {
            ConfigTypeViewModel = new ComboxViewModel();

            var items = new List<ComBoxItem>
            {
                new ComBoxItem { Value = ECylinderType.SingleActSingleLimit, DisplayName = "\u5355\u63a7\u5355\u9650\u4f4d" },
                new ComBoxItem { Value = ECylinderType.SingleActDoubleLimit, DisplayName = "\u5355\u63a7\u53cc\u9650\u4f4d" },
                new ComBoxItem { Value = ECylinderType.DoubleActSingleLimit, DisplayName = "\u53cc\u63a7\u5355\u9650\u4f4d" },
                new ComBoxItem { Value = ECylinderType.DoubleActDoubleLimit, DisplayName = "\u53cc\u63a7\u53cc\u9650\u4f4d" },
            };
            ConfigTypeViewModel.ItemsSource = items;
            ConfigTypeViewModel.DisplayMemberPath = nameof(ComBoxItem.DisplayName);
            ConfigTypeViewModel.SelectedItem = items[0];

            SingleControlSingleLimitViewModel = new();
            SingleControlDoubleLimitViewModel = new();
            DoubleControlSingleLimitViewModel = new();
            DoubleControlDoubleLimitViewModel = new();

            subscribeValueChanges();

            ConfigTypeViewModel.ValueChanged += (_, __) =>
            {
                SelectedConfigType = (ECylinderType)((ConfigTypeViewModel.SelectedItem as ComBoxItem)?.Value ?? ECylinderType.SingleActSingleLimit);
                AfterModified?.Invoke(this, EventArgs.Empty);
            };

            SelectedConfigType = ECylinderType.SingleActSingleLimit;
        }

        private void subscribeValueChanges()
        {
            SingleControlSingleLimitViewModel.AfterModified += onAnyValueChanged;
            SingleControlDoubleLimitViewModel.AfterModified += onAnyValueChanged;
            DoubleControlSingleLimitViewModel.AfterModified += onAnyValueChanged;
            DoubleControlDoubleLimitViewModel.AfterModified += onAnyValueChanged;
        }

        private void onAnyValueChanged(object? sender, EventArgs e)
        {
            AfterModified?.Invoke(sender, e);
        }

        public void SetViewReference(Control view)
        {
            SingleControlSingleLimitViewModel.SetViewReference(view);
            SingleControlDoubleLimitViewModel.SetViewReference(view);
            DoubleControlSingleLimitViewModel.SetViewReference(view);
            DoubleControlDoubleLimitViewModel.SetViewReference(view);
        }

        public void CopyFrom(CylinderConfigCfgInfo model)
        {
            if (model == null)
            {
                return;
            }

            SelectedConfigType = model.ConfigType;
            if (ConfigTypeViewModel.ItemsSource != null)
            {
                var targetItem = ConfigTypeViewModel.ItemsSource.Cast<ComBoxItem>()
                    .FirstOrDefault(o => (ECylinderType)o.Value == SelectedConfigType);
                if (targetItem != null)
                {
                    ConfigTypeViewModel.SelectedItem = targetItem;
                }
            }

            this.RaisePropertyChanged(nameof(CurrentLimitViewModel));

            model.SingleActSingleLimit ??= new();
            model.SingleActDoubleLimit ??= new();
            model.DoubleActSingleLimit ??= new();
            model.DoubleActDoubleLimit ??= new();

            SingleControlSingleLimitViewModel.CopyFrom(model.SingleActSingleLimit);
            SingleControlDoubleLimitViewModel.CopyFrom(model.SingleActDoubleLimit);
            DoubleControlSingleLimitViewModel.CopyFrom(model.DoubleActSingleLimit);
            DoubleControlDoubleLimitViewModel.CopyFrom(model.DoubleActDoubleLimit);
        }

        public void CopyTo(CylinderConfigCfgInfo model)
        {
            if (model == null)
            {
                return;
            }

            model.ConfigType = SelectedConfigType;

            model.SingleActSingleLimit ??= new();
            model.SingleActDoubleLimit ??= new();
            model.DoubleActSingleLimit ??= new();
            model.DoubleActDoubleLimit ??= new();

            SingleControlSingleLimitViewModel.CopyTo(model.SingleActSingleLimit);
            SingleControlDoubleLimitViewModel.CopyTo(model.SingleActDoubleLimit);
            DoubleControlSingleLimitViewModel.CopyTo(model.DoubleActSingleLimit);
            DoubleControlDoubleLimitViewModel.CopyTo(model.DoubleActDoubleLimit);
        }

        public void SetIOChannelOptions(IEnumerable<string>? channelIDs)
        {
            SingleControlSingleLimitViewModel.SetIOChannelOptions(channelIDs);
            SingleControlDoubleLimitViewModel.SetIOChannelOptions(channelIDs);
            DoubleControlSingleLimitViewModel.SetIOChannelOptions(channelIDs);
            DoubleControlDoubleLimitViewModel.SetIOChannelOptions(channelIDs);
        }

        public void SetIOChannelOptions(IEnumerable<GKG.IOStateInformation>? ioStates)
        {
            SingleControlSingleLimitViewModel.SetIOChannelOptions(ioStates);
            SingleControlDoubleLimitViewModel.SetIOChannelOptions(ioStates);
            DoubleControlSingleLimitViewModel.SetIOChannelOptions(ioStates);
            DoubleControlDoubleLimitViewModel.SetIOChannelOptions(ioStates);
        }
    }
}
