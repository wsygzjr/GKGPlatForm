using ReactiveUI;
using System.Reactive;

namespace Griffins.CompUI.ElectricalMngObj.FactoryCfgPage.ViewModels
{
    /// <summary>
    /// IO设备编辑
    /// </summary>
    public class IODeviceEditViewModel : ReactiveObject
    {
        private bool? _dialogResult;
        // 原始对象（仅用于保存时同步）
        private readonly IODeviceInfoViewModel _originalData;
        private IODeviceInfoViewModel _editCopy;

        /// <summary>
        /// 绑定到界面的编辑副本
        /// </summary>
        public IODeviceInfoViewModel EditCopy
        {
            get => _editCopy;
            set => this.RaiseAndSetIfChanged(ref _editCopy, value);
        }
       
        /// <summary>
        /// 对话框结果（true:保存，false:取消，null:未操作）
        /// </summary>
        public bool? DialogResult
        {
            get => _dialogResult;
            set => this.RaiseAndSetIfChanged(ref _dialogResult, value);
        }

        // 命令
        /// <summary>
        /// 保存命令
        /// </summary>
        public ReactiveCommand<Unit, Unit> SaveCommand { get; }

        /// <summary>
        /// 取消命令
        /// </summary>
        public ReactiveCommand<Unit, Unit> CancelCommand { get; }

        public IODeviceEditViewModel(IODeviceInfoViewModel iODeviceInfoModel, bool isNew)
        {
            _originalData = iODeviceInfoModel ?? new IODeviceInfoViewModel();
            _editCopy = new IODeviceInfoViewModel();
            _editCopy.CopyFrom(_originalData);
            // 监听当前属性值的变化，与原始值对比,只要任意一个属性的当前值 ≠ 原始值，就允许保存（canSave = true）
            var canSave = this.WhenAnyValue(
                x => x.EditCopy.SelectedIODeviceModel,
                x => x.EditCopy.SelectedIODeviceID,
                (selectedIODeviceModel, selectedIODeviceID) =>  !string.IsNullOrEmpty(selectedIODeviceModel) &&
                                                                !string.IsNullOrEmpty(selectedIODeviceID) &&
                                                                !selectedIODeviceModel.Equals(_originalData.SelectedIODeviceModel) ||
                                                                !selectedIODeviceID.Equals(_originalData.SelectedIODeviceID));
            SaveCommand = ReactiveCommand.Create(save, canSave);
            CancelCommand = ReactiveCommand.Create(cancel); 
        }

        /// <summary>
        /// /保存逻辑
        /// </summary>
        private void save()
        {
            _originalData.SelectedIODeviceID = EditCopy.SelectedIODeviceID;
            _originalData.SelectedIODeviceModel = EditCopy.SelectedIODeviceModel;
            _originalData.SerialNumber = EditCopy.SerialNumber;
            DialogResult = true;
        }

        /// <summary>
        /// 取消逻辑
        /// </summary>
        private void cancel()
        {
            DialogResult = false;
        }

    }
}