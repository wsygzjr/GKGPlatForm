using GF_Gereric;
using Griffins;
using Griffins.Map.Cmd;
using Griffins.Map.UI;
using ReactiveUI;
using System;
using System.Linq;

namespace GKG.Map.AuxiliaryInfoFuncCtlMapCell.ViewModel
{
    public class AuxiliaryCommandViewModel : ReactiveObject
    {
        private string _commandName;
        public string CommandName
        {
            get => _commandName;
            private set => this.RaiseAndSetIfChanged(ref _commandName, value);
        }

        private string _resultText;
        public string ResultText
        {
            get => _resultText;
            private set => this.RaiseAndSetIfChanged(ref _resultText, value);
        }

        private readonly MapCmdExector _cmdExector;
        private readonly string _mpNo;
        private readonly string _methodId;

        public AuxiliaryCommandViewModel(IFunctionalMapCellCallBack functionalMapCellCallBack, string mpNo, string methodId, string commandName)
        {
            _cmdExector = new MapCmdExector(functionalMapCellCallBack.INorthSvrCommandExec);
            _mpNo = mpNo;
            _methodId = methodId;

            _commandName = commandName;
            _resultText = string.Empty;

            Execute();
        }

        private void Execute()
        {
            try
            {
                GFBaseTypeParamValueList result = _cmdExector.ExecUIDataObjCommand(_mpNo, _methodId, new GFBaseTypeParamValueList());

                bool? ok = null;
                try
                {
                    ok = result?.FirstOrDefault(p => string.Equals(p.ID, "Result", StringComparison.Ordinal))?.Value?.ToPrimitiveValue<bool>();
                }
                catch
                {
                    ok = null;
                }

                ResultText = ok == true ? "结果: 成功" : ok == false ? "结果: 失败" : "结果: 未知";
            }
            catch (Exception ex)
            {
                ResultText = $"发送命令异常: {ex.Message}";
            }
        }
    }
}
