using ReactiveUI;
using System;
using System.Reactive;

namespace Griffins.CompUI.EletronicManager.CompUI_EletronicManager.PageType.ElectricalMngPage.ControlCard.ViewModels
{
    public class ControlCardEditWindowViewModel : ReactiveObject
    {
        private bool? _dialogResult;

        private readonly ControlCardViewModel _originalData;
        private ControlCardViewModel _editCopy;

        public ControlCardViewModel EditCopy
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

        public ControlCardEditWindowViewModel(ControlCardViewModel controlCardModel)
        {
            _originalData = controlCardModel ?? new ControlCardViewModel();
            _editCopy = _originalData.Clone();

            var canSave = this.WhenAnyValue(
                x => x.EditCopy.ControlCardTypeModel.SelectedItem,
                x => x.EditCopy.MotionCardCustomIDViewModel.Text,
                x => x.EditCopy.MotionCardNameViewModel.Text,
                x => x.EditCopy.CardCountViewModel.Text,
                x => x.EditCopy.StateChannelCountInputViewModel.Text,
                x => x.EditCopy.AnalogChannelCountInputViewModel.Text,
                x => x.EditCopy.SupportAxisCountViewModel.Text,
                (type, cardId, cardName, cardCount, stateChannelCount, analogChannelCount, supportAxisCount) =>
                    type != null &&
                    !string.IsNullOrWhiteSpace(cardId) &&
                    !string.IsNullOrWhiteSpace(cardName) &&
                    IsValidNumber(cardCount, allowZero: false) &&
                    IsValidNumber(stateChannelCount, allowZero: true) &&
                    IsValidNumber(analogChannelCount, allowZero: true) &&
                    IsValidNumber(supportAxisCount, allowZero: false));

            SaveCommand = ReactiveCommand.Create(save, canSave);
            CancelCommand = ReactiveCommand.Create(cancel);
        }

        private static bool IsValidNumber(string? text, bool allowZero)
        {
            if (!int.TryParse(text?.Trim(), out var value))
                return false;

            return allowZero ? value >= 0 : value > 0;
        }

        private void save()
        {
            _originalData.CopyFrom(_editCopy);
            DialogResult = true;
        }

        private void cancel()
        {
            DialogResult = false;
        }
    }
}
