using ReactiveUI;
using System;

namespace Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.FuncHeadGroup.ViewModels
{
    internal sealed class FuncHeadGroupRowViewModel : ReactiveObject
    {
        private int _serialNumber;
        private string _functionHeadDisplayName = string.Empty;

        public event EventHandler? AfterModified;

        public int SerialNumber
        {
            get => _serialNumber;
            set => this.RaiseAndSetIfChanged(ref _serialNumber, value);
        }

        /// <summary>功能头组显示名称（用于列表编辑与持久化）。</summary>
        public string FunctionHeadDisplayName
        {
            get => _functionHeadDisplayName;
            set
            {
                var t = value ?? string.Empty;
                if (string.Equals(_functionHeadDisplayName, t, StringComparison.Ordinal))
                    return;
                this.RaiseAndSetIfChanged(ref _functionHeadDisplayName, t);
                AfterModified?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
