using ReactiveUI;
using System.Reactive;

namespace Griffins.CompUI.ElectricalMngObj.FactoryCfgPage.ViewModels
{
    /// <summary>
    /// 运控卡编辑窗口-视图模型
    /// </summary>
    public class ControlCardEditWindowViewModel : ReactiveObject
    {
        private bool? _dialogResult;

        // 原始对象（仅用于保存时同步）
        private readonly ControlCardViewModel _originalData;
        private ControlCardViewModel _editCopy;

        /// <summary>
        /// 绑定到界面的编辑副本
        /// </summary>
        public ControlCardViewModel EditCopy
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

        /// <summary>
        /// 保存命令
        /// </summary>
        public ReactiveCommand<Unit, Unit> SaveCommand { get; }

        /// <summary>
        /// 取消命令
        /// </summary>
        public ReactiveCommand<Unit, Unit> CancelCommand { get; }

        public ControlCardEditWindowViewModel(ControlCardViewModel controlCardModel, bool isNew)
        {
            _originalData = controlCardModel ?? new ControlCardViewModel();
            _editCopy = new ControlCardViewModel();
            _editCopy.CopyFrom(_originalData);



            //监听当前属性值的变化，与原始值对比,只要任意一个属性的当前值 ≠ 原始值，就允许保存（canSave = true）
            var canSave = this.WhenAnyValue(
                x => x.EditCopy.SelectedControlCardKind,
                x => x.EditCopy.SelectedControlCardType,
                x => x.EditCopy.CadID,
                (selectedIODeviceModel, selectedIODeviceID, cadID) => !string.IsNullOrEmpty(selectedIODeviceModel) &&
                                                                !string.IsNullOrEmpty(selectedIODeviceID) &&
                                                                !selectedIODeviceModel.Equals(_originalData.SelectedControlCardKind) ||
                                                                !cadID.Equals(_originalData.CadID) ||
                                                                !selectedIODeviceID.Equals(_originalData.SelectedControlCardType)
            );


            SaveCommand = ReactiveCommand.Create(save, canSave);
            CancelCommand = ReactiveCommand.Create(cancel); 
        }
       
        /// <summary>
        /// /保存逻辑
        /// </summary>
        private void save()
        {
            _originalData.CadID = EditCopy.CadID;
            _originalData.SelectedControlCardKind = EditCopy.SelectedControlCardKind;
            _originalData.SelectedControlCardType = EditCopy.SelectedControlCardType;
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