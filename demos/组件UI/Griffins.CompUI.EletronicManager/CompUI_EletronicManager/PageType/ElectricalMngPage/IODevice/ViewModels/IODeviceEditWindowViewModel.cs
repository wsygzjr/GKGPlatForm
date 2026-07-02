using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;

namespace Griffins.CompUI.EletronicManager.CompUI_EletronicManager.PageType.ElectricalMngPage.IODevice.ViewModels
{
    public class IODeviceEditWindowViewModel : ReactiveObject
    {
        private bool? _dialogResult;
        private readonly IODeviceInfoViewModel _originalData;
        private IODeviceInfoViewModel _editCopy;

        public IODeviceInfoViewModel EditCopy
        {
            get => _editCopy;
            set => this.RaiseAndSetIfChanged(ref _editCopy, value);
        }

        public bool? DialogResult
        {
            get => _dialogResult;
            set => this.RaiseAndSetIfChanged(ref _dialogResult, value);
        }

        public ReactiveCommand<Unit, Unit> SaveCommand { get; }

        public ReactiveCommand<Unit, Unit> CancelCommand { get; }

        public IODeviceEditWindowViewModel(
            IODeviceInfoViewModel model,
            IEnumerable<IODeviceInfoViewModel.ControlCardChannelOption>? controlCardOptions)
        {
            _originalData = model ?? new IODeviceInfoViewModel();
            _editCopy = new IODeviceInfoViewModel();
            _editCopy.CopyFrom(_originalData);
            _editCopy.BindControlCards(controlCardOptions);

            var canSave = this.WhenAnyValue(
                x => x.EditCopy.IOName,
                x => x.EditCopy.DeviceId,
                x => x.EditCopy.ChannelSelection,
                (name, deviceId, channel) =>
                    !string.IsNullOrWhiteSpace(name) &&
                    !string.IsNullOrWhiteSpace(deviceId) &&
                    !string.IsNullOrWhiteSpace(channel));

            SaveCommand = ReactiveCommand.Create(save, canSave);
            CancelCommand = ReactiveCommand.Create(cancel);
        }

        private void save()
        {
            _originalData.IOName = EditCopy.IOName;
            _originalData.DeviceId = EditCopy.DeviceId;
            _originalData.ChannelSelection = EditCopy.ChannelSelection;
            _originalData.StateReverse = EditCopy.StateReverse;
            _originalData.ReadWriteMode = EditCopy.ReadWriteMode;
            DialogResult = true;
        }

        private void cancel()
        {
            DialogResult = false;
        }
    }
}
