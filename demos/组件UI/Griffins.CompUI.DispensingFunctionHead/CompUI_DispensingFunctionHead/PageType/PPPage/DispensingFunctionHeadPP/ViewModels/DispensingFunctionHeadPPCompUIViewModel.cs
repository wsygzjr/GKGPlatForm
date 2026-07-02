using GF_Gereric;
using GKG;
using GKG.ElectronicControl.Dispenser;
using GKG.SubMM.Dispenser;
using GKG.UI;
using Griffins.Map.UI;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Griffins.CompUI.DispensingFunctionHead.CompUI_DispensingFunctionHead.PageType.PPPage.DispensingFunctionHeadPP.ViewModels
{
    /// <summary>
    /// 点胶机功能头配方配置页面ViewModel
    /// 管理胶量感应参数和阀参数列表
    /// </summary>
    public class DispensingFunctionHeadPPCompUIViewModel : ReactiveObject
    {
        private DispensingFunctionHeadSubMachineModulesPPCfg _data = new();
        private object _viewTag;
        private bool _readOnly;
        private bool _isSettingData; // 标志：正在设置数据，防止触发AfterModified
        private bool _isEditDialogOpen;
        private ValveParamsItemViewModel _editingValveParams;
        private ValveParamsItemViewModel _editingSourceValveParams;
        private ObservableCollection<ValveParamsItemViewModel> _valveParamsList;

        /// <summary>数据修改后触发的事件</summary>
        public event EventHandler AfterModified;

        /// <summary>视图标签</summary>
        public object ViewTag
        {
            get => _viewTag;
            set => this.RaiseAndSetIfChanged(ref _viewTag, value);
        }

        /// <summary>只读模式标志</summary>
        public bool ReadOnly
        {
            get => _readOnly;
            set
            {
                this.RaiseAndSetIfChanged(ref _readOnly, value);
                UpdateEnabledState(!_readOnly);
            }
        }

        #region 胶量感应参数

        /// <summary>胶量感应参数ViewModel</summary>
        public GlueAmountParamsViewModel GlueAmountViewModel { get; }

        #endregion

        #region 阀参数列表

        /// <summary>阀参数列表</summary>
        public ObservableCollection<ValveParamsItemViewModel> ValveParamsList
        {
            get => _valveParamsList;
            private set => this.RaiseAndSetIfChanged(ref _valveParamsList, value);
        }

        /// <summary>添加阀参数命令</summary>
        public ICommand AddValveParamsCommand { get; }

        /// <summary>编辑阀参数命令</summary>
        public ICommand EditValveParamsCommand { get; }

        /// <summary>删除阀参数命令</summary>
        public ICommand DeleteValveParamsCommand { get; }

        /// <summary>弹窗保存命令</summary>
        public ICommand SaveEditingValveParamsCommand { get; }

        /// <summary>弹窗取消命令</summary>
        public ICommand CancelEditingValveParamsCommand { get; }

        public bool IsEditDialogOpen
        {
            get => _isEditDialogOpen;
            set => this.RaiseAndSetIfChanged(ref _isEditDialogOpen, value);
        }

        public ValveParamsItemViewModel EditingValveParams
        {
            get => _editingValveParams;
            set => this.RaiseAndSetIfChanged(ref _editingValveParams, value);
        }

        #endregion

        public DispensingFunctionHeadPPCompUIViewModel(ICompUIRunTimeCallBack callBack)
        {
            // 初始化胶量感应参数ViewModel
            GlueAmountViewModel = new GlueAmountParamsViewModel();
            GlueAmountViewModel.AfterModified += OnValueChanged;

            // 初始化阀参数列表
            _valveParamsList = new ObservableCollection<ValveParamsItemViewModel>();

            // 初始化命令
            AddValveParamsCommand = ReactiveCommand.Create(AddValveParams);
            EditValveParamsCommand = ReactiveCommand.Create<ValveParamsItemViewModel>(EditValveParams);
            DeleteValveParamsCommand = ReactiveCommand.Create<ValveParamsItemViewModel>(DeleteValveParams);
            SaveEditingValveParamsCommand = ReactiveCommand.Create(SaveEditingValveParams);
            CancelEditingValveParamsCommand = ReactiveCommand.Create(CancelEditingValveParams);

            ReadOnly = false;
        }

        private void OnValueChanged(object sender, EventArgs e)
        {
            // 如果正在SetData，不触发AfterModified
            if (_isSettingData)
            {
                return;
            }

            AfterModified?.Invoke(sender, e);
        }

        public void SetData(DispensingFunctionHeadSubMachineModulesPPCfg data)
        {
            _isSettingData = true; // 开始设置数据
            try
            {
                _data = CloneData(data);

                // 加载胶量感应参数
                GlueAmountParams glueAmountParams = null;
                if (_data.glueAmountParams != null && _data.glueAmountParams.Length > 0)
                {
                    glueAmountParams = JsonObjConvert.FromJSonBytes<GlueAmountParams>(_data.glueAmountParams);
                }
                GlueAmountViewModel.CopyFrom(glueAmountParams);

                // 加载阀参数列表
                var loadedValveParams = new ObservableCollection<ValveParamsItemViewModel>();
                GKGPiezoValveFormulaParams valveFormulaParams = null;
                if (_data.GKGPiezoValveFormulaParams != null && _data.GKGPiezoValveFormulaParams.Length > 0)
                {
                    valveFormulaParams = JsonObjConvert.FromJSonBytes<GKGPiezoValveFormulaParams>(_data.GKGPiezoValveFormulaParams);
                }

                if (valveFormulaParams?.ValveParams != null)
                {
                    for (int i = 0; i < valveFormulaParams.ValveParams.Count; i++)
                    {
                        var itemViewModel = new ValveParamsItemViewModel();
                        itemViewModel.CopyFrom(valveFormulaParams.ValveParams[i], i + 1);
                        loadedValveParams.Add(itemViewModel);
                    }
                }

                ValveParamsList = loadedValveParams;
            }
            finally
            {
                _isSettingData = false; // 结束设置数据
            }
        }

        public DispensingFunctionHeadSubMachineModulesPPCfg GetData()
        {
            _data ??= new DispensingFunctionHeadSubMachineModulesPPCfg();

            // 保存胶量感应参数
            var glueAmountParams = GlueAmountViewModel.ToModel();
            _data.glueAmountParams = JsonObjConvert.ToJSonBytes(glueAmountParams);

            // 保存阀参数列表
            var valveFormulaParams = new GKGPiezoValveFormulaParams
            {
                ValveParams = ValveParamsList.Select(vm => vm.ToModel()).ToList()
            };
            _data.GKGPiezoValveFormulaParams = JsonObjConvert.ToJSonBytes(valveFormulaParams);

            return CloneData(_data);
        }

        private void UpdateEnabledState(bool enabled)
        {
            GlueAmountViewModel.UpdateEnabledState(enabled);
        }

        #region 阀参数列表操作

        private void AddValveParams()
        {
            if (ReadOnly) return;

            var newItem = new ValveParamsItemViewModel();
            newItem.Index = ValveParamsList.Count + 1;
            ValveParamsList.Add(newItem);
            OnValueChanged(this, EventArgs.Empty);
        }

        private void EditValveParams(ValveParamsItemViewModel item)
        {
            if (ReadOnly || item == null) return;

            _editingSourceValveParams = item;
            EditingValveParams = item.CloneForEdit();
            IsEditDialogOpen = true;
        }

        private void SaveEditingValveParams()
        {
            if (ReadOnly || _editingSourceValveParams == null || EditingValveParams == null)
            {
                CloseEditDialog();
                return;
            }

            _editingSourceValveParams.CopyEditableFieldsFrom(EditingValveParams);
            CloseEditDialog();
            OnValueChanged(this, EventArgs.Empty);
        }

        private void CancelEditingValveParams()
        {
            CloseEditDialog();
        }

        private void CloseEditDialog()
        {
            IsEditDialogOpen = false;
            EditingValveParams = null;
            _editingSourceValveParams = null;
        }

        private void DeleteValveParams(ValveParamsItemViewModel item)
        {
            if (ReadOnly || item == null) return;

            ValveParamsList.Remove(item);
            
            // 重新编号
            for (int i = 0; i < ValveParamsList.Count; i++)
            {
                ValveParamsList[i].Index = i + 1;
            }

            OnValueChanged(this, EventArgs.Empty);
        }

        #endregion

        private static DispensingFunctionHeadSubMachineModulesPPCfg CloneData(DispensingFunctionHeadSubMachineModulesPPCfg data)
        {
            if (data == null)
                return new DispensingFunctionHeadSubMachineModulesPPCfg();

            return JsonObjConvert.FromJSonBytes<DispensingFunctionHeadSubMachineModulesPPCfg>(JsonObjConvert.ToJSonBytes(data))
                ?? new DispensingFunctionHeadSubMachineModulesPPCfg();
        }
    }
}
