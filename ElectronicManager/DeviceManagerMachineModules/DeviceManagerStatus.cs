using Griffins;
using Griffins.ImeIOT;

namespace GKG.MM
{
    public class DeviceManagerStatus : GFPropObjBase
    {
        private ImeRunMode _imeRunMode;
        [GFProp(GfPropReadWrite.ReadWrite, GFPropertyEditKind.Select, GriffinsValueRangeDefineMode.Enumeration, "运行模式")]
        public ImeRunMode ImeRunMode
        {
            get => _imeRunMode;
            set
            {
                _imeRunMode = value;
                base.RaisePropertyChanged(nameof(_imeRunMode));
            }
        }

        private RunModeList _runModeList = new RunModeList();
        //{
        //    { ImeRunMode.WorkMode.ToString(),ImeRunMode.WorkMode.ToString() }, 
        //    { ImeRunMode.ConfigMode.ToString(), ImeRunMode.ConfigMode.ToString() },
        //    { ImeRunMode.AgingMode.ToString(), ImeRunMode.AgingMode.ToString() } 
        //};
        [GFProp(GfPropReadWrite.ReadOnly, GFPropertyEditKind.Assignment, GriffinsValueRangeDefineMode.None, "运行模式列表", GriffinsBaseDataType.Object_Json, RunModeList.Object_IDStr)]

        public RunModeList RunModeList
        {
            get => _runModeList;
            set
            {
                _runModeList = value;
                base.RaisePropertyChanged(nameof(_runModeList));
            }
        }

        private string _curFormulaNumber = "";
        [GFProp(GfPropReadWrite.ReadWrite, GFPropertyEditKind.Assignment, GriffinsValueRangeDefineMode.None, "当前配方名称")]
        public string CurFormulaNumber
        {
            get => _curFormulaNumber;
            set
            {
                _curFormulaNumber = value;
                base.RaisePropertyChanged(nameof(_curFormulaNumber));
            }
        }

        private FormulaNumberList _formulaNumberList = new FormulaNumberList();
        [GFProp(GfPropReadWrite.ReadOnly, GFPropertyEditKind.Assignment, GriffinsValueRangeDefineMode.None, "配方名称列表", GriffinsBaseDataType.Object_Json, FormulaNumberList.Object_IDStr)]
        public FormulaNumberList FormulaNumberList
        {
            get => _formulaNumberList;
            set
            {
                _formulaNumberList = value;
                base.RaisePropertyChanged(nameof(_formulaNumberList));
            }
        }
        private ImeExecMode _execMode = ImeExecMode.Continuous;
        [GFProp(GfPropReadWrite.ReadWrite, GFPropertyEditKind.Select, GriffinsValueRangeDefineMode.Enumeration, "控制流程执行模式")]
        public ImeExecMode ExecMode
        {
            get => _execMode;
            set
            {
                _execMode = value;
                base.RaisePropertyChanged(nameof(_execMode));
            }
        }
    }
}
