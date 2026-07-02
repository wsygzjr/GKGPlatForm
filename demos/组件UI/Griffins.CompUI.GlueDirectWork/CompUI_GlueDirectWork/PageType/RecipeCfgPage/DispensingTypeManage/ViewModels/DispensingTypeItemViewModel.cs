using ReactiveUI;
using System;

namespace Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.DispensingTypeManage.ViewModels
{
    internal sealed class DispensingTypeItemViewModel : ReactiveObject
    {
        private int _serialNumber;
        private string _dispensingName = string.Empty;
        private bool _isChecked;

        public event EventHandler? AfterModified;

        public int SerialNumber
        {
            get => _serialNumber;
            set
            {
                if (_serialNumber != value)
                {
                    this.RaiseAndSetIfChanged(ref _serialNumber, value);
                }
            }
        }

        public string DispensingName
        {
            get => _dispensingName;
            set
            {
                if (_dispensingName != value)
                {
                    this.RaiseAndSetIfChanged(ref _dispensingName, value);
                    AfterModified?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public bool IsChecked
        {
            get => _isChecked;
            set => this.RaiseAndSetIfChanged(ref _isChecked, value);
        }
    }
}
